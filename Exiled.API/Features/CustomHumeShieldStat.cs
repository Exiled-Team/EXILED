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
    /// A custom version of <see cref="HumeShieldStat"/> will permit to reset MaxHS when the player class change.
    /// </summary>
    public class CustomHumeShieldStat : HumeShieldStat
    {
        /// <inheritdoc/>
        public override void ClassChanged()
        {
            base.ClassChanged();

            Player.Get(Hub).MaxHS = default;
        }
    }
}