namespace RueI.Parsing;

/// <summary>
/// Defines the base class for all rich text tags.
/// </summary>
/// <typeparam name="T">The type of the closing tag.</typeparam>
public abstract class ClosingTag<T> : NoParamsTag
    where T : ClosingTag<T>, new()
{
    /// <summary>
    /// Gets the only name of this <see cref="ClosingTag{T}"/>.
    /// </summary>
    public abstract string Name { get; }

    /// <inheritdoc/>
    public sealed override string[] Names => new[] { Name };

    /// <inheritdoc/>
    public sealed override bool HandleTag(ParserContext context)
    {
        ApplyTo(context);
        context.ResultBuilder.Append($"<{Name}>");
        context.RemoveEndingTag<T>();
        return true;
    }

    /// <summary>
    /// Applies the effects <see cref="ClosingTag{T}"/> to a <see cref="ParserContext"/>.
    /// </summary>
    /// <param name="context">The context of the parser.</param>
    protected virtual void ApplyTo(ParserContext context)
    {
    }
}
