namespace Exiled.API.Enums
{
    /// <summary>
    /// An enum which represents the categories of an effect.
    /// </summary>
    [System.Flags]
    public enum EffectCategory
    {
        /// <summary>
        /// Represents an uncategorized effect.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents an effect with a positive player impact.
        /// </summary>
        Positive = 1,

        /// <summary>
        /// Represents an effect with a negative player impact.
        /// </summary>
        Negative = 2,

        /// <summary>
        /// Represents an effect which modifies a player's movement speed.
        /// </summary>
        Movement = 4,

        /// <summary>
        /// Represents an effect which deals damage to a player over time.
        /// </summary>
        Harmful = 8,
    }
}
