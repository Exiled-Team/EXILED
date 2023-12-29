// -----------------------------------------------------------------------
// <copyright file="CustomHealthStat.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.Diagnostics;

    using PlayerStatsSystem;

    /// <summary>
    /// A custom version of <see cref="HealthStat"/> which allows the player's max amount of health to be changed.
    /// </summary>
    [DebuggerDisplay("Value = {CustomMaxValue}, Max = {MaxValue}")]
    public class CustomHealthStat : HealthStat
    {
        /// <inheritdoc/>
        public override float MaxValue => CustomMaxValue == default ? base.MaxValue : CustomMaxValue;

        /// <summary>
        /// Gets or sets the maximum amount of health the player will have.
        /// </summary>
        public float CustomMaxValue { get; set; }
    }
}