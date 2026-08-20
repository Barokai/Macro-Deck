using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using SuchByte.MacroDeck.ActionButton;

namespace SuchByte.MacroDeck.Utils;

public class LabelGenerator
{
    public static Image GetLabel(Image img,
        string text,
        ButtonLabelPosition buttonLabelPosition,
        Font font,
        Color textColor,
        Color shadowColor,
        SizeF shadowOffset)
    {
        if (img == null)
        {
            font.Dispose();
            return img;
        }

        using var ownedFont = font;

        if (string.IsNullOrEmpty(text))
        {
            return img;
        }

        if (NeedsUnicodeFallback(text))
        {
            return DrawLabelWithTextRenderer(img, text, buttonLabelPosition, ownedFont, textColor, shadowColor, shadowOffset);
        }

        text = SanitizeLabelText(text);
        if (string.IsNullOrEmpty(text))
        {
            return img;
        }

        return DrawLabelWithGraphicsPath(img, text, buttonLabelPosition, ownedFont, textColor);
    }

    private static Image DrawLabelWithGraphicsPath(Image img, string text, ButtonLabelPosition buttonLabelPosition, Font font, Color textColor)
    {
        using var g = Graphics.FromImage(img);
        using var sf = new StringFormat
        {
            Alignment = StringAlignment.Center
        };

        if (buttonLabelPosition == ButtonLabelPosition.TOP)
        {
            sf.LineAlignment = StringAlignment.Near;
        }
        else if (buttonLabelPosition == ButtonLabelPosition.CENTER)
        {
            sf.LineAlignment = StringAlignment.Center;
        }
        else
        {
            sf.LineAlignment = StringAlignment.Far;
        }

        using var p = new Pen(Color.Black, 2)
        {
            LineJoin = LineJoin.Round
        };

        using var b = new SolidBrush(textColor);

        var r = new Rectangle(2, 2, img.Width - 2, img.Height - 2);

        using var gp = new GraphicsPath();

        gp.AddString(text, font.FontFamily, (int)font.Style, font.Size * 5, r, sf);

        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

        g.DrawPath(p, gp);
        g.FillPath(b, gp);

        return img;
    }

    private static Image DrawLabelWithTextRenderer(Image img, string text, ButtonLabelPosition buttonLabelPosition, Font font, Color textColor, Color shadowColor, SizeF shadowOffset)
    {
        using var g = Graphics.FromImage(img);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

        var bounds = new Rectangle(2, 2, img.Width - 4, img.Height - 4);
        var flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak | TextFormatFlags.NoPadding;
        flags |= buttonLabelPosition switch
        {
            ButtonLabelPosition.TOP => TextFormatFlags.Top,
            ButtonLabelPosition.CENTER => TextFormatFlags.VerticalCenter,
            _ => TextFormatFlags.Bottom,
        };

        using var renderFont = CreateUnicodeFriendlyFont(font);
        var shadowRect = new Rectangle(
            bounds.X + (int)Math.Round(shadowOffset.Width),
            bounds.Y + (int)Math.Round(shadowOffset.Height),
            bounds.Width,
            bounds.Height);

        TextRenderer.DrawText(g, text, renderFont, shadowRect, shadowColor, flags);
        TextRenderer.DrawText(g, text, renderFont, bounds, textColor, flags);

        return img;
    }

    private static Font CreateUnicodeFriendlyFont(Font originalFont)
    {
        var targetSize = Math.Max(8f, originalFont.Size * 5f);
        try
        {
            return new Font("Segoe UI Emoji", targetSize, originalFont.Style, GraphicsUnit.Pixel);
        }
        catch (ArgumentException)
        {
            return new Font("Segoe UI", targetSize, originalFont.Style, GraphicsUnit.Pixel);
        }
    }

    internal static bool NeedsUnicodeFallback(string text)
    {
        foreach (var rune in text.EnumerateRunes())
        {
            if (rune.Value > 0x7F)
            {
                var category = Rune.GetUnicodeCategory(rune);
                if (category == UnicodeCategory.OtherSymbol || rune.Value >= 0x1F000)
                {
                    return true;
                }
            }
        }

        return false;
    }

    internal static string SanitizeLabelText(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        var sb = new StringBuilder(text.Length);
        foreach (var rune in text.EnumerateRunes())
        {
            if (rune.Value is 0xFE0E or 0xFE0F)
            {
                continue;
            }

            sb.Append(rune.ToString());
        }

        return sb.ToString();
    }
}
