using Apteka.Configuration;
using Apteka.Services;
using FluentAssertions;
using Xunit;

namespace Apteka.UnitTests.Services;

public class SensitiveDataProtectorTests
{
	private static SensitiveDataProtector CreateProtector() => new(new AppSettings());

	[Fact]
	public void Protect_ThenUnprotect_RoundTripsOriginalValue()
	{
		var protector = CreateProtector();

		var protectedValue = protector.Protect("90010112345");
		var result = protector.Unprotect(protectedValue);

		result.Should().Be("90010112345");
	}

	[Fact]
	public void Protect_ProducesValuePrefixedWithEncV1()
	{
		var protector = CreateProtector();

		var protectedValue = protector.Protect("some-sensitive-value");

		protectedValue.Should().StartWith("enc:v1:");
	}

	[Fact]
	public void Protect_SameValueTwice_ProducesDifferentCiphertext()
	{
		var protector = CreateProtector();

		var first = protector.Protect("repeatable-value");
		var second = protector.Protect("repeatable-value");

		first.Should().NotBe(second);
	}

	[Fact]
	public void Protect_AlreadyProtectedValue_IsReturnedUnchanged()
	{
		var protector = CreateProtector();
		var protectedValue = protector.Protect("some-value");

		var protectedAgain = protector.Protect(protectedValue);

		protectedAgain.Should().Be(protectedValue);
	}

	[Fact]
	public void Protect_EmptyValue_ReturnsEmpty()
	{
		var protector = CreateProtector();

		protector.Protect(string.Empty).Should().Be(string.Empty);
	}

	[Fact]
	public void Protect_NullValue_ReturnsEmpty()
	{
		var protector = CreateProtector();

		protector.Protect(null).Should().Be(string.Empty);
	}

	[Fact]
	public void Unprotect_PlaintextValue_IsReturnedUnchanged()
	{
		var protector = CreateProtector();

		protector.Unprotect("plain-text-value").Should().Be("plain-text-value");
	}

	[Fact]
	public void Unprotect_MalformedProtectedValue_Throws()
	{
		var protector = CreateProtector();

		var act = () => protector.Unprotect("enc:v1:not-enough-parts");

		act.Should().Throw<InvalidOperationException>();
	}

	[Fact]
	public void Unprotect_TamperedCiphertext_ThrowsInsteadOfReturningGarbage()
	{
		var protector = CreateProtector();
		var protectedValue = protector.Protect("original-value");
		var parts = protectedValue.Split(':');
		var tamperedCiphertext = parts[4][..^1] + (parts[4][^1] == 'A' ? 'B' : 'A');
		var tampered = string.Join(':', parts[0], parts[1], parts[2], parts[3], tamperedCiphertext);

		var act = () => protector.Unprotect(tampered);

		act.Should().Throw<InvalidOperationException>();
	}

	[Fact]
	public void IsProtected_ProtectedValue_ReturnsTrue()
	{
		var protector = CreateProtector();
		var protectedValue = protector.Protect("a-value");

		protector.IsProtected(protectedValue).Should().BeTrue();
	}

	[Fact]
	public void IsProtected_PlaintextValue_ReturnsFalse()
	{
		var protector = CreateProtector();

		protector.IsProtected("plain-text-value").Should().BeFalse();
	}

	[Fact]
	public void IsProtected_Null_ReturnsFalse()
	{
		var protector = CreateProtector();

		protector.IsProtected(null).Should().BeFalse();
	}

	[Fact]
	public void Hash_SameValue_IsDeterministic()
	{
		var protector = CreateProtector();

		protector.Hash("90010112345").Should().Be(protector.Hash("90010112345"));
	}

	[Fact]
	public void Hash_DifferentValues_ProduceDifferentHashes()
	{
		var protector = CreateProtector();

		protector.Hash("90010112345").Should().NotBe(protector.Hash("90010112346"));
	}

	[Fact]
	public void Hash_ReturnsLowercaseHex()
	{
		var protector = CreateProtector();

		var hash = protector.Hash("90010112345");

		hash.Should().MatchRegex("^[0-9a-f]{64}$");
	}

	[Fact]
	public void Hash_TrimsInputLikeProtectDoes()
	{
		var protector = CreateProtector();

		protector.Hash("  90010112345  ").Should().Be(protector.Hash("90010112345"));
	}
}
