using Apteka.Services;
using FluentAssertions;
using Xunit;

namespace Apteka.UnitTests.Services;

public class PeselValidatorTests
{
	private const string ValidPesel = "44051203258";
	private const string BadChecksumPesel = "44051203259";

	[Fact]
	public void IsValid_CorrectChecksum_ReturnsTrue()
	{
		PeselValidator.IsValid(ValidPesel).Should().BeTrue();
	}

	[Fact]
	public void IsValid_WrongChecksum_ReturnsFalse()
	{
		PeselValidator.IsValid(BadChecksumPesel).Should().BeFalse();
	}

	[Fact]
	public void IsValid_TooShort_ReturnsFalse()
	{
		PeselValidator.IsValid("4405120325").Should().BeFalse();
	}

	[Fact]
	public void IsValid_TooLong_ReturnsFalse()
	{
		PeselValidator.IsValid("440512032580").Should().BeFalse();
	}

	[Fact]
	public void IsValid_NonNumeric_ReturnsFalse()
	{
		PeselValidator.IsValid("4405120325A").Should().BeFalse();
	}

	[Theory]
	[InlineData("+4405120325")]
	[InlineData("-4405120325")]
	[InlineData("4405 120325")]
	public void IsValid_SignOrSpaceInsideValue_ReturnsFalse(string pesel)
	{
		PeselValidator.IsValid(pesel).Should().BeFalse();
	}

	[Fact]
	public void IsValid_Null_ReturnsFalse()
	{
		PeselValidator.IsValid(null).Should().BeFalse();
	}

	[Fact]
	public void IsValid_Empty_ReturnsFalse()
	{
		PeselValidator.IsValid(string.Empty).Should().BeFalse();
	}

	[Fact]
	public void IsValid_Whitespace_ReturnsFalse()
	{
		PeselValidator.IsValid("           ").Should().BeFalse();
	}
}
