namespace Exiled.API.Features.CustomStats
{
    using PlayerStatsSystem;

    /// <summary>
    /// A custom version of <see cref="StaminaStat"/> which allows the player's max amount of Stamina to be changed.
    /// </summary>
    public class CustomStaminaStat : StaminaStat
    {
        /// <inheritdoc/>
        public override float MaxValue => CustomMaxValue == default ? base.MaxValue : CustomMaxValue;

        /// <summary>
        /// Gets or sets the maximum amount of health the player will have.
        /// </summary>
        public float CustomMaxValue { get; set; }
    }
}