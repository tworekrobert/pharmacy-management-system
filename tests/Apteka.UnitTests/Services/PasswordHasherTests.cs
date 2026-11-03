using Apteka.Services;
using FluentAssertions;
using Xunit;

namespace Apteka.UnitTests.Services;

public class PasswordHasherTests
{
	[Fact]
	public void Hash_ProducesPbkdf2FormattedString()
	{
		var hash = PasswordHasher.Hash("correct-password-1");

		hash.Should().StartWith("pbkdf2$");
		hash.Split('$').Should().HaveCount(4);
	}

	[Fact]
	public void Hash_SamePasswordTwice_ProducesDifferentHashes()
	{
		var first = PasswordHasher.Hash("same-password");
		var second = PasswordHasher.Hash("same-password");

		first.Should().NotBe(second);
	}

	[Fact]
	public void Hash_EmptyPassword_Throws()
	{
		var act = () => PasswordHasher.Hash(string.Empty);

		act.Should().Throw<InvalidOperationException>();
	}

	[Fact]
	public void Hash_WhitespacePassword_Throws()
	{
		var act = () => PasswordHasher.Hash("   ");

		act.Should().Throw<InvalidOperationException>();
	}

	[Fact]
	public void Verify_CorrectPassword_ReturnsTrue()
	{
		var hash = PasswordHasher.Hash("correct-password-1");

		PasswordHasher.Verify("correct-password-1", hash).Should().BeTrue();
	}

	[Fact]
	public void Verify_WrongPassword_ReturnsFalse()
	{
		var hash = PasswordHasher.Hash("correct-password-1");

		PasswordHasher.Verify("wrong-password", hash).Should().BeFalse();
	}

	[Fact]
	public void Verify_NullStoredHash_ReturnsFalse()
	{
		PasswordHasher.Verify("anything", null!).Should().BeFalse();
	}

	[Fact]
	public void Verify_EmptyStoredHash_ReturnsFalse()
	{
		PasswordHasher.Verify("anything", string.Empty).Should().BeFalse();
	}

	[Fact]
	public void Verify_LegacySha256Hash_StillVerifiesCorrectly()
	{
		var password = "legacy-password";
		var legacyHash = Convert.ToBase64String(
			System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(password)));

		PasswordHasher.Verify(password, legacyHash).Should().BeTrue();
	}

	[Fact]
	public void Verify_LegacySha256Hash_WrongPassword_ReturnsFalse()
	{
		var legacyHash = Convert.ToBase64String(
			System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes("legacy-password")));

		PasswordHasher.Verify("not-the-password", legacyHash).Should().BeFalse();
	}

	[Fact]
	public void Verify_TamperedHash_ReturnsFalse()
	{
		var hash = PasswordHasher.Hash("correct-password-1");
		var parts = hash.Split('$');
		var tampered = $"{parts[0]}${parts[1]}${parts[2]}${new string('A', parts[3].Length)}";

		PasswordHasher.Verify("correct-password-1", tampered).Should().BeFalse();
	}
}
