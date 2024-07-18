namespace RueI.Displays;

using System.Text;

using NorthwoodLib.Pools;

using RueI.Elements;
using RueI.Extensions;
using RueI.Parsing.Records;

/// <summary>
/// Provides a means of combining <see cref="Element"/>s.
/// </summary>
/// <remarks>
/// The <see cref="ElemCombiner"/> is a helper class that combines all of the <see cref="Element"/>s for a <see cref="DisplayCore"/> into a single string,
/// to be displayed as a hint.
/// </remarks>
public static class ElemCombiner
{
    /// <summary>
    /// Combines multiple <see cref="Element"/>s into a string.
    /// </summary>
    /// <param name="core">The <see cref="DisplayCore"/> of the player.</param>
    /// <param name="enumElems">The <see cref="IEnumerable{T}"/> of <see cref="Element"/>s to combine.</param>
    /// <returns>A <see cref="string"/> with all of the combined <see cref="Element"/>s.</returns>
    public static string Combine(DisplayCore core, IEnumerable<Element> enumElems)
    {
        List<Element> elements = ListPool<Element>.Shared.Rent(enumElems);

        if (!elements.Any())
        {
            return string.Empty;
        }

        StringBuilder sb = StringBuilderPool.Shared.Rent();

        float totalOffset = -8.465f;

        float lastPosition = 0;
        float lastOffset = 0;

        elements.Sort(CompareElement);

        UnityAlternative.Provider.LogDebug($"Combining {elements.Count} elements");

        for (int i = 0; i < elements.Count; i++)
        {
            Element curElement = elements[i];

            ParsedData parsedData = curElement.GetParsedData(core);

            float funcPos = curElement.GetFunctionalPosition();
            if (curElement.Options.HasFlagFast(Elements.Enums.ElementOptions.PreserveSpacing))
            {
                funcPos -= parsedData.Offset;
            }

            if (i != 0)
            {
                float calcedOffset = CalculateOffset(lastPosition, lastOffset, funcPos);
                sb.Append($"<line-height={calcedOffset}px>\n<line-height=40.665>");
                totalOffset += calcedOffset;
            }
            else
            {
                totalOffset += funcPos;
            }

            sb.Append(parsedData.Content);

            totalOffset += parsedData.Offset;
            lastPosition = funcPos;
            lastOffset = parsedData.Offset;
        }

        ListPool<Element>.Shared.Return(elements);
        sb.Insert(0, $"<line-height={totalOffset}px>\n<line-height=0><size=0>.\n</size><line-height=40.665>");

        // a period with a size of zero is appended here to ensure that trailing newlines still occur
        // since this is after all tags have been closed, its guaranteed to not
        // do anything at all except stop trailing newlines
        sb.Append("<line-height=0>\n<size=0>.");
        return StringBuilderPool.Shared.ToStringReturn(sb);
    }

    /// <summary>
    /// Combines multiple <see cref="Element"/>s into a string.
    /// </summary>
    /// <param name="enumElems">The <see cref="IEnumerable{T}"/> of <see cref="ParsedData"/>s to combine.</param>
    /// <returns>A <see cref="string"/> with all of the combined <see cref="ParsedData"/>s.</returns>
    public static string Combine(IEnumerable<ParsedData> enumElems)
    {
        List<ParsedData> elements = ListPool<ParsedData>.Shared.Rent(enumElems);

        if (!elements.Any())
        {
            return string.Empty;
        }

        StringBuilder sb = StringBuilderPool.Shared.Rent();

        float totalOffset = -8.465f;

        float lastPosition = 0;
        float lastOffset = 0;

        // elements.Sort(CompareElement);
        UnityAlternative.Provider.LogDebug($"Combining {elements.Count} elements");

        for (int i = 0; i < elements.Count; i++)
        {
            ParsedData parsedData = elements[i];

            float funcPos = 0;
            funcPos -= parsedData.Offset;

            if (i != 0)
            {
                float calcedOffset = CalculateOffset(lastPosition, lastOffset, funcPos);
                sb.Append($"<line-height={calcedOffset}px>\n<line-height=40.665>");
                totalOffset += calcedOffset;
            }
            else
            {
                totalOffset += funcPos;
            }

            sb.Append(parsedData.Content);

            totalOffset += parsedData.Offset;
            lastPosition = funcPos;
            lastOffset = parsedData.Offset;
        }

        ListPool<ParsedData>.Shared.Return(elements);
        sb.Insert(0, $"<line-height={totalOffset}px>\n<line-height=0><size=0>.\n</size><line-height=40.665>");

        // a period with a size of zero is appended here to ensure that trailing newlines still occur
        // since this is after all tags have been closed, its guaranteed to not
        // do anything at all except stop trailing newlines
        sb.Append("<line-height=0>\n<size=0>.");
        return StringBuilderPool.Shared.ToStringReturn(sb);
    }

    /// <summary>
    /// Calculates the offset for two hints.
    /// </summary>
    /// <param name="hintOnePos">The first hint's vertical position.</param>
    /// <param name="hintOneTotalLines">The first hint's total line-height, excluding the vertical position.</param>
    /// <param name="hintTwoPos">The second hint's vertical position.</param>
    /// <returns>A float indicating the new offset.</returns>
    public static float CalculateOffset(float hintOnePos, float hintOneTotalLines, float hintTwoPos)
    {
        float calc = hintOnePos + (2 * hintOneTotalLines) - hintTwoPos;
        return calc / -2;
    }

    private static int CompareElement(Element first, Element second) => first.ZIndex - second.ZIndex;
}