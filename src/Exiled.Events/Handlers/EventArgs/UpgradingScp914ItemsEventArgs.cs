// -----------------------------------------------------------------------
// <copyright file="UpgradingScp914ItemsEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.EventArgs
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
    using Scp914;

    /// <summary>
    /// Contains all informations before the SCP-914 machine upgrades items inside it.
    /// </summary>
    public class UpgradingScp914ItemsEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradingScp914ItemsEventArgs"/> class.
        /// </summary>
        /// <param name="scp914"><inheritdoc cref="Scp914"/></param>
        /// <param name="players"><inheritdoc cref="Players"/></param>
        /// <param name="items"><inheritdoc cref="Items"/></param>
        /// <param name="knobSetting"><inheritdoc cref="KnobSetting"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public UpgradingScp914ItemsEventArgs(Scp914Machine scp914, List<Player> players, List<Pickup> items, Scp914Knob knobSetting, bool isAllowed = true)
        {
            Scp914 = scp914;
            Players = players;
            Items = items;
            KnobSetting = knobSetting;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="Scp914Machine"/> instance.
        /// </summary>
        public Scp914Machine Scp914 { get; private set; }

        /// <summary>
        /// Gets or sets all players inside SCP-914.
        /// </summary>
        public List<Player> Players { get; set; }

        /// <summary>
        /// Gets or sets all items to be upgraded inside SCP-914.
        /// </summary>
        public List<Pickup> Items { get; set; }

        /// <summary>
        /// Gets or sets SCP-914 working knob setting.
        /// </summary>
        public Scp914Knob KnobSetting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
