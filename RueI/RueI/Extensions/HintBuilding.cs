namespace RueI.Extensions.HintBuilding;

using System.Drawing;
using System.Text;

using RueI.Parsing.Enums;

/// <summary>
/// Provides extensions for adding rich text tags to <see cref="StringBuilder"/>s.
/// </summary>
public static class HintBuilding
{
    /// <summary>
    /// Represents all of the options for the alignment of a string of text.
    /// </summary>
    public enum AlignStyle
    {
        /// <summary>
        /// Indicates that the text should be left-aligned.
        /// </summary>
        Left,

        /// <summary>
        /// Indicates that the text should be center-aligned.
        /// </summary>
        Center,

        /// <summary>
        /// Indicates that the text should be right-aligned.
        /// </summary>
        Right,

        /// <summary>
        /// Indicates that every line should be stretched to fill the display area, excluding the last line.
        /// </summary>
        Justified,

        /// <summary>
        /// Indicates that every line should be stretched to fill the display area. This includes the last line.
        /// </summary>
        Flush,
    }

    /// <summary>
    /// Converts a <see cref="Color"/> to a hex code string.
    /// </summary>
    /// <param name="color">The <see cref="Color"/> to convert.</param>
    /// <returns>The color as a hex code string.</returns>
    public static string ConvertToHex(Color color)
    {
        string alphaInclude = color.A switch
        {
            255 => string.Empty,
            _ => color.A.ToString("X2")
        };

        return $"#{color.R:X2}{color.G:X2}{color.B:X2}{alphaInclude}";
    }

    /// <summary>
    /// Adds a linebreak to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder AddLinebreak(this StringBuilder sb) => sb.Append('\n');

    /// <summary>
    /// Adds an alignment tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <param name="align">The <see cref="AlignStyle"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetAlignment(this StringBuilder sb, AlignStyle align)
    {
        string alignment = align switch
        {
            AlignStyle.Left => "left",
            AlignStyle.Right => "right",
            AlignStyle.Justified => "justified",
            AlignStyle.Flush => "flush",
            _ => "center",
        };

        return sb.Append($"<align={alignment}>");
    }

    /// <summary>
    /// Adds a size tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <param name="size">The size to include in the size tag.</param>
    /// <param name="style">The measurement style of the size tag.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetSize(this StringBuilder sb, float size, MeasurementUnit style = MeasurementUnit.Pixels)
    {
        string format = style switch
        {
            MeasurementUnit.Percentage => "%",
            MeasurementUnit.Ems => "ems",
            _ => string.Empty
        };

        return sb.Append($"<size={size}{format}>");
    }

    /// <summary>
    /// Adds a line-height tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <param name="size">The line height to include in the line-height tag.</param>
    /// <param name="style">The measurement style of the line-height tag.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetLineHeight(this StringBuilder sb, float size, MeasurementUnit style = MeasurementUnit.Pixels)
    {
        string format = style switch
        {
            MeasurementUnit.Percentage => "%",
            MeasurementUnit.Ems => "ems",
            _ => string.Empty
        };

        return sb.Append($"<line-height={size}{format}>");
    }

    /// <summary>
    /// Adds a color tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <param name="color">The color to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetColor(this StringBuilder sb, Color color)
    {
        return sb.Append($"<color={ConvertToHex(color)}>");
    }

    /// <summary>
    /// Adds a color tag to a <see cref="StringBuilder"/> from RGBA values.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <param name="r">The red (0-255) in the color.</param>
    /// <param name="g">The green (0-255) in the color.</param>
    /// <param name="b">The blue (0-255) in the color.</param>
    /// <param name="alpha">The optional alpha (0-255) of the color.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetColor(this StringBuilder sb, int r, int g, int b, int alpha = 255)
    {
        Color color = Color.FromArgb(alpha, r, g, b);
        return sb.SetColor(color);
    }

    /// <summary>
    /// Adds a color tag to a <see cref="StringBuilder"/> from RGBA values.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <param name="r">The red (0-255) in the color.</param>
    /// <param name="g">The green (0-255) in the color.</param>
    /// <param name="b">The blue (0-255) in the color.</param>
    /// <param name="alpha">The optional alpha (0-255) of the color.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetColor(this StringBuilder sb, byte r, byte g, byte b, byte alpha = 255)
    {
        Color color = Color.FromArgb(alpha, r, g, b);
        return sb.SetColor(color);
    }

    /// <summary>
    /// Adds a mark tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <param name="color">The color to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetMark(this StringBuilder sb, Color color)
    {
        return sb.Append($"<mark={ConvertToHex(color)}>");
    }

    /// <summary>
    /// Adds a mark tag to a <see cref="StringBuilder"/> from RGBA values.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <param name="r">The red (0-255) in the color.</param>
    /// <param name="g">The green (0-255) in the color.</param>
    /// <param name="b">The blue (0-255) in the color.</param>
    /// <param name="alpha">The optional alpha (0-255) of the color.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetMark(this StringBuilder sb, int r, int g, int b, int alpha)
    {
        Color color = Color.FromArgb(alpha, r, g, b);
        return sb.SetMark(color);
    }

    /// <summary>
    /// Adds a mark tag to a <see cref="StringBuilder"/> from RGBA values.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <param name="r">The red (0-255) in the color.</param>
    /// <param name="g">The green (0-255) in the color.</param>
    /// <param name="b">The blue (0-255) in the color.</param>
    /// <param name="alpha">The optional alpha (0-255) of the color.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetMark(this StringBuilder sb, byte r, byte g, byte b, byte alpha)
    {
        Color color = Color.FromArgb(alpha, r, g, b);
        return sb.SetMark(color);
    }

    /// <summary>
    /// Adds an alpha tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <param name="alpha">The alpha (0-255) of the color.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetAlpha(this StringBuilder sb, byte alpha) => sb.Append($"<alpha={alpha:X2}>");

    /// <summary>
    /// Adds an alpha tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <param name="alpha">The alpha (0-255) of the color.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetAlpha(this StringBuilder sb, int alpha) => sb.Append($"<alpha={alpha:X2}>");

    /// <summary>
    /// Adds a bold tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetBold(this StringBuilder sb) => sb.Append("<b>");

    /// <summary>
    /// Adds an italics tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetItalics(this StringBuilder sb) => sb.Append("<i>");

    /// <summary>
    /// Adds a strikethrough tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetStrikethrough(this StringBuilder sb) => sb.Append("<s>");

    /// <summary>
    /// Adds an underline tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetUnderline(this StringBuilder sb) => sb.Append("<u>");

    /// <summary>
    /// Adds an indent tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <param name="indent">The indent size to include in the indent tag.</param>
    /// <param name="style">The measurement style of the indent tag.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetIndent(this StringBuilder sb, float indent, MeasurementUnit style = MeasurementUnit.Pixels)
    {
        string format = style switch
        {
            MeasurementUnit.Percentage => "%",
            MeasurementUnit.Ems => "ems",
            _ => string.Empty
        };

        return sb.Append($"<indent={indent}{format}>");
    }

    /// <summary>
    /// Adds a horizontal scale tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <param name="scale">The scale size to include in the scale tag.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetScale(this StringBuilder sb, float scale) => sb.Append($"<scale={scale}>");

    /// <summary>
    /// Adds a monospace tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <param name="spacing">The size of the spacing.</param>
    /// <param name="style">The measurement style of the monospacing tag.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetMonospace(this StringBuilder sb, float spacing, MeasurementUnit style = MeasurementUnit.Pixels)
    {
        string format = style switch
        {
            MeasurementUnit.Percentage => "%",
            MeasurementUnit.Ems => "ems",
            _ => string.Empty
        };

        return sb.Append($"<mspace={spacing}{format}>");
    }

    /// <summary>
    /// Adds a case tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <param name="caseStyle">The case to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetCase(this StringBuilder sb, CaseStyle caseStyle)
    {
        string format = caseStyle switch
        {
            CaseStyle.Uppercase => "allcaps",
            CaseStyle.Lowercase => "lowercase",
            _ => "smallcaps",
        };

        return sb.Append($"<{format}>");
    }

    /// <summary>
    /// Adds an margins tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <param name="margins">The size of the margins.</param>
    /// <param name="style">The measurement style of the margins tag.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetMargins(this StringBuilder sb, float margins, MeasurementUnit style = MeasurementUnit.Pixels)
    {
        string format = style switch
        {
            MeasurementUnit.Percentage => "%",
            MeasurementUnit.Ems => "ems",
            _ => string.Empty
        };

        return sb.Append($"<margin={margins}{format}>");
    }

    /// <summary>
    /// Adds a nobreak tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetNobreak(this StringBuilder sb) => sb.Append("<nobr>");

    /// <summary>
    /// Adds a noparse tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetNoparse(this StringBuilder sb) => sb.Append("<noparse>");

    /// <summary>
    /// Adds a rotation tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <param name="rotation">The rotation (-180 to 180) of the tag..</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetRotation(this StringBuilder sb, int rotation) => sb.Append($"<rotate=\"{rotation}\">");

    /// <summary>
    /// Adds a subscript tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetSubscript(this StringBuilder sb) => sb.Append("<sub>");

    /// <summary>
    /// Adds a superscript tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetSuperscript(this StringBuilder sb) => sb.Append("<sup>");

    /// <summary>
    /// Adds a width tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <param name="offset">The size of the new width.</param>
    /// <param name="style">The measurement style of the width tag.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder SetWidth(this StringBuilder sb, float offset, MeasurementUnit style = MeasurementUnit.Pixels)
    {
        string format = style switch
        {
            MeasurementUnit.Percentage => "%",
            MeasurementUnit.Ems => "ems",
            _ => string.Empty
        };

        return sb.Append($"<width={offset}{format}>");
    }

    /// <summary>
    /// Adds a pos tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <param name="margins">The size of the pos tag.</param>
    /// <param name="style">The measurement style of the pos tag.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder AddHorizontalPos(this StringBuilder sb, float margins, MeasurementUnit style = MeasurementUnit.Pixels)
    {
        string format = style switch
        {
            MeasurementUnit.Percentage => "%",
            MeasurementUnit.Ems => "ems",
            _ => string.Empty
        };

        return sb.Append($"<pos={margins}{format}>");
    }

    /// <summary>
    /// Adds a space tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <param name="margins">The size of the pos tag.</param>
    /// <param name="style">The measurement style of the pos tag.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder AddSpace(this StringBuilder sb, float margins, MeasurementUnit style = MeasurementUnit.Pixels)
    {
        string format = style switch
        {
            MeasurementUnit.Percentage => "%",
            MeasurementUnit.Ems => "ems",
            _ => string.Empty
        };

        return sb.Append($"<space={margins}{format}>");
    }

    /// <summary>
    /// Adds a voffset tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <param name="offset">The size of the voffset tag.</param>
    /// <param name="style">The measurement style of the voffset tag.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder AddVOffset(this StringBuilder sb, float offset, MeasurementUnit style = MeasurementUnit.Pixels)
    {
        string format = style switch
        {
            MeasurementUnit.Percentage => "%",
            MeasurementUnit.Ems => "ems",
            _ => string.Empty
        };

        return sb.Append($"<voffset={offset}{format}>");
    }

    /// <summary>
    /// Adds a sprite tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <param name="index">The index (0-20) of the sprite tag.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder AddSprite(this StringBuilder sb, int index) => sb.Append($"<sprite={index}>");

    /// <summary>
    /// Adds a sprite tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <param name="index">The index (0-20) of the sprite tag.</param>
    /// <param name="color">The color of the sprite tag.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder AddSprite(this StringBuilder sb, int index, Color color) => sb.Append($"<sprite index={index} color={ConvertToHex(color)}>");

    /// <summary>
    /// Adds a closing color tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder CloseColor(this StringBuilder sb) => sb.Append("</color>");

    /// <summary>
    /// Adds a closing align tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder CloseAlign(this StringBuilder sb) => sb.Append("</align>");

    /// <summary>
    /// Adds a closing alpha tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder CloseAlpha(this StringBuilder sb) => sb.Append("</alpha>");

    /// <summary>
    /// Adds a closing size tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder CloseSize(this StringBuilder sb) => sb.Append("</size>");

    /// <summary>
    /// Adds a closing line-height tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder CloseLineHeight(this StringBuilder sb) => sb.Append("</line-height>");

    /// <summary>
    /// Adds a closing bold tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder CloseBold(this StringBuilder sb) => sb.Append("</b>");

    /// <summary>
    /// Adds a closing italics tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder CloseItalics(this StringBuilder sb) => sb.Append("</i>");

    /// <summary>
    /// Adds a closing strikethrough tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder CloseStrikethrough(this StringBuilder sb) => sb.Append("</s>");

    /// <summary>
    /// Adds a closing underline tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder CloseUnderline(this StringBuilder sb) => sb.Append("</u>");

    /// <summary>
    /// Adds a closing indent tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder CloseIndent(this StringBuilder sb) => sb.Append("</indent>");

    /// <summary>
    /// Adds a closing scale tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder CloseScale(this StringBuilder sb) => sb.Append("</scale>");

    /// <summary>
    /// Adds a closing monospace tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder CloseMonospace(this StringBuilder sb) => sb.Append("</mspace>");

    /// <summary>
    /// Adds a closing subscript tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder CloseSubscript(this StringBuilder sb) => sb.Append("</subscript>");

    /// <summary>
    /// Adds a closing superscript tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder CloseSuperscript(this StringBuilder sb) => sb.Append("</superscript>");

    /// <summary>
    /// Adds a closing rotation tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder CloseRotation(this StringBuilder sb) => sb.Append("</rotate>");

    /// <summary>
    /// Adds a closing margins tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder CloseMargins(this StringBuilder sb) => sb.Append("</margins>");

    /// <summary>
    /// Adds a closing mark tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder CloseMark(this StringBuilder sb) => sb.Append("</mark>");

    /// <summary>
    /// Adds a closing nobreak tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder CloseNobreak(this StringBuilder sb) => sb.Append("</nobr>");

    /// <summary>
    /// Adds a closing noparse tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder CloseNoparse(this StringBuilder sb) => sb.Append("</noparse>");

    /// <summary>
    /// Adds a closing voffset tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder CloseVOffset(this StringBuilder sb) => sb.Append("</voffset>");

    /// <summary>
    /// Adds a closing width tag to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
    /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
    public static StringBuilder CloseWidth(this StringBuilder sb) => sb.Append("</width>");
}