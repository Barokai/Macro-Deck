using SuchByte.MacroDeck.Utils;

namespace MacroDeck.Tests;

[TestFixture]
public class LabelGeneratorTests
{
    [TestCase("⬇️ Download", "⬇ Download")]
    [TestCase("❤︎", "❤")]
    public void SanitizeLabelText_RemovesVariationSelectors(string input, string expected)
    {
        Assert.That(LabelGenerator.SanitizeLabelText(input), Is.EqualTo(expected));
    }

    [Test]
    public void NeedsUnicodeFallback_ReturnsFalse_ForPlainAsciiText()
    {
        Assert.That(LabelGenerator.NeedsUnicodeFallback("Download"), Is.False);
    }

    [TestCase("⬇️ Download")]
    [TestCase("😀")]
    public void NeedsUnicodeFallback_ReturnsTrue_ForSymbolOrEmojiText(string input)
    {
        Assert.That(LabelGenerator.NeedsUnicodeFallback(input), Is.True);
    }
}
