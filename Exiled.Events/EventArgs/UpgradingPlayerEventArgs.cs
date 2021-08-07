// -----------------------------------------------------------------------
// <copyright file="UpgradingPlayerEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    using global::Scp914;

    /// <summary>
    /// Contains all information before SCP-914 upgrades a player.
    /// </summary>
    public class UpgradingPlayerEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradingPlayerEventArgs"/> class.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> being upgraded.</param>
        /// <param name="heldOnly"><inheritdoc cref="HeldOnly"/></param>
        /// <param name="setting">The <see cref="Scp914KnobSetting"/> being used.</param>
        /// <param name="upgradeItems"><inheritdoc cref="UpgradeItems"/></param>
        public UpgradingPlayerEventArgs(Player player, bool upgradeItems, bool heldOnly, Scp914KnobSetting setting)
        {
            Player = player;
            UpgradeItems = upgradeItems;
            HeldOnly = heldOnly;
            KnobSetting = setting;
        }

        /// <summary>
        /// Gets the player being upgraded.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the event can continue.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not items will be upgraded.
        /// </summary>
        public bool UpgradeItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not only held items are upgraded.
        /// </summary>
        public bool HeldOnly { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Scp914KnobSetting"/> being used.
        /// </summary>
        public Scp914KnobSetting KnobSetting { get; set; }
    }
}
