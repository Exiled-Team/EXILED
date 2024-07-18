namespace RueI;

using System.Collections.ObjectModel;

using RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a variety of constant values.
/// </summary>
/// <remarks>
/// This class is mosty designed for internal use within RueI. However, it can still be useful for external use.
/// </remarks>
public static class Constants
{
    /// <summary>
    /// Gets the default height if a line-height is not provided.
    /// </summary>
    /// <remarks>Approximate.</remarks>
    public const float DEFAULTHEIGHT = 40.665f; // in pixels. this is barely approximate

    /// <summary>
    /// Gets the default size (in pixels) if a size is not provided.
    /// </summary>
    /// <remarks>Not approximate.</remarks>
    public const float DEFAULTSIZE = 34.7f; // in pixels. this is not approximate

    /// <summary>
    /// Gets the multiplier used to convert the size of a capital character to a smallcaps character.
    /// </summary>
    public const float CAPSTOSMALLCAPS = 0.8f;

    /// <summary>
    /// Gets the pixel increase for bold characters.
    /// </summary>
    public const float BOLDINCREASE = 2.429f;

    /// <summary>
    /// Gets the width of the display area (in pixels).
    /// </summary>
    public const float DISPLAYAREAWIDTH = 1200;

    /// <summary>
    /// Gets how many pixels are in an em.
    /// </summary>
    public const float EMSTOPIXELS = 34.7f;

    /// <summary>
    /// Gets the zero width space.
    /// </summary>
    public const char ZeroWidthSpace = '​';

    /// <summary>
    /// Gets the maximum name size allowed for a tag.
    /// </summary>
    public const int MAXTAGNAMESIZE = 13;

    /// <summary>
    /// Gets the limit on measurement values.
    /// </summary>
    public const int MEASUREMENTVALUELIMIT = 32768;

    /// <summary>
    /// Gets the ratelimit used for displaying hints.
    /// </summary>
    public static readonly TimeSpan HintRateLimit = TimeSpan.FromMilliseconds(525);

    /// <summary>
    /// Gets a list of allowed sizes of color param tags, ignoring the hashtag.
    /// </summary>
    public static ReadOnlyCollection<int> ValidColorSizes { get; } = new(new int[]
    {
        3,
        4,
        6,
        8,
    });

    /// <summary>
    /// Gets a <see cref="ReadOnlyCollection{T}"/> of valid alignments for <see cref="AlignTag"/>.
    /// </summary>
    public static ReadOnlyCollection<string> Alignments { get; } = new(new string[]
    {
        "left",
        "center",
        "right",
        "justified",
        "flush",
    });

    /// <summary>
    /// Gets a list of allowed colors for <see cref="ColorTag"/>.
    /// </summary>
    public static ReadOnlyCollection<string> Colors { get; } = new(new string[]
    {
        "black",
        "blue",
        "green",
        "orange",
        "purple",
        "red",
        "white",
        "yellow",
    });
}