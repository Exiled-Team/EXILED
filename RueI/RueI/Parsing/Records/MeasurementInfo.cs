namespace RueI.Parsing.Records;

using System.Text;
using NorthwoodLib.Pools;
using RueI.Parsing.Enums;

/// <summary>
/// Defines a record that contains information about measurement info.
/// </summary>
/// <param name="value">The value of the measurement.</param>
/// <param name="style">The style of the measurement.</param>
/// <remarks>
/// This provides a convenient way to specify both the value and unit for a measurement,
/// as the base value when converted to pixels can differ depending on the
/// context of the measurement.
/// </remarks>
public record struct MeasurementInfo(float value, MeasurementUnit style)
{
    /// <summary>
    /// Attempts to extract a <see cref="MeasurementInfo"/> from a string.
    /// </summary>
    /// <param name="content">The content to parse.</param>
    /// <param name="info">The returned info, if true.</param>
    /// <returns>true if the string was valid, otherwise false.</returns>
    public static bool TryParse(string content, out MeasurementInfo info)
    {
        StringBuilder paramBuffer = StringBuilderPool.Shared.Rent(25);
        MeasurementUnit style = MeasurementUnit.Pixels;

        bool hasPeriod = false;

        foreach (char ch in content)
        {
            if (ch == 'e')
            {
                style = MeasurementUnit.Ems;
                break;
            }
            else if (ch == '%')
            {
                style = MeasurementUnit.Percentage;
                break;
            }
            else if (ch == 'p') // pixels
            {
                break;
            }
            else if (ch == '.')
            {
                if (!hasPeriod)
                {
                    hasPeriod = true;
                    paramBuffer.Append('.');
                }
            }
            else if (char.IsDigit(ch))
            {
                paramBuffer.Append(ch);
            }
        }

        string bufferString = StringBuilderPool.Shared.ToStringReturn(paramBuffer);
        if (float.TryParse(bufferString, out float result) && result < Constants.MEASUREMENTVALUELIMIT)
        {
            info = new(result, style);
            return true;
        }
        else
        {
            info = default;
            return false;
        }
    }

    /// <summary>
    /// Gets a string representation of this <see cref="MeasurementInfo"/>.
    /// </summary>
    /// <returns>A new value.</returns>
    public override readonly string ToString()
    {
        return value.ToString() + style switch
        {
            MeasurementUnit.Ems => "e",
            MeasurementUnit.Percentage => "%",
            _ => string.Empty,
        };
    }
}
