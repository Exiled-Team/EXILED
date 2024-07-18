namespace RueI.Parsing.Tags;

/// <summary>
/// Provides a way to handle singletons of tags.
/// </summary>
/// <typeparam name="T">The <see cref="RichTextTag"/> type to share.</typeparam>
/// <remarks>
/// This class provides a way to guarantee that only one instance of a tag will ever be used by the parser,
/// since tags are not static to support inheritance but must act similar to it.
/// </remarks>
public static class SharedTag<T>
    where T : RichTextTag, new()
{
    /// <summary>
    /// Gets the shared singleton for this <see cref="RichTextTag"/>.
    /// </summary>
    public static T Singleton { get; } = new();
}
