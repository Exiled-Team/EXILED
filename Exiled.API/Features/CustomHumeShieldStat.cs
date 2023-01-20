// -----------------------------------------------------------------------
// <copyright file="CustomHumeShieldStat.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using PlayerStatsSystem;

    /// <summary>
    /// A custom version of <see cref="HumeShieldStat"/> which allows the player's max amount of Hume shield to be changed.
    /// </summary>
    public class CustomHumeShieldStat : HumeShieldStat
    {
        /// <inheritdoc/>
        public override float MaxValue => CustomMaxValue == default ? base.MaxValue : CustomMaxValue;

        /// <summary>
        /// Gets or sets the maximum amount of health the player will have.
        /// </summary>
        public float CustomMaxValue { get; set; }
    }
}