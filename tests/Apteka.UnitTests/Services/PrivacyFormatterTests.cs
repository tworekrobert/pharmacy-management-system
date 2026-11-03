using Apteka.Services;
using FluentAssertions;
using Xunit;

namespace Apteka.UnitTests.Services;

public class PrivacyFormatterTests
{
	[Fact]
	public void MaskPesel_FullLengthPesel_MasksAllButLastFourDigits()
	{
		PrivacyFormatter.MaskPesel("44051203258").Should().Be("*******3258");
	}

	[Fact]
	public void MaskPesel_ExactlyFourCharacters_MasksEntirely()
	{
		PrivacyFormatter.MaskPesel("1234").Should().Be("****");
	}

	[Fact]
	public void MaskPesel_ShorterThanFourCharacters_MasksEntirely()
	{
		PrivacyFormatter.MaskPesel("12").Should().Be("**");
	}

	[Fact]
	public void MaskPesel_Null_ReturnsEmpty()
	{
		PrivacyFormatter.MaskPesel(null).Should().Be(string.Empty);
	}

	[Fact]
	public void MaskPesel_Empty_ReturnsEmpty()
	{
		PrivacyFormatter.MaskPesel(string.Empty).Should().Be(string.Empty);
	}

	[Fact]
	public void MaskPesel_Whitespace_ReturnsEmpty()
	{
		PrivacyFormatter.MaskPesel("   ").Should().Be(string.Empty);
	}

	[Fact]
	public void MaskPesel_PreservesTotalLength()
	{
		var masked = PrivacyFormatter.MaskPesel("44051203258");

		masked.Length.Should().Be(11);
	}
}
