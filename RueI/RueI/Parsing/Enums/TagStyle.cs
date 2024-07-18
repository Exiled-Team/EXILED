namespace RueI.Parsing.Enums;

/// <summary>
/// Represents the valid characters for a delimiter.
/// </summary>
public enum TagStyle
{
    /// <summary>
    /// Indicates that a tag does not take parameters.
    /// </summary>
    NoParams,

    /// <summary>
    /// Indicates that a tag takes in a value (equal sign) param.
    /// </summary>
    ValueParam,

    /// <summary>
    /// Indicates that a tag takes in only attributes (space delimiter).
    /// </summary>
    Attributes,
}
