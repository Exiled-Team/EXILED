// -----------------------------------------------------------------------
// <copyright file="UpgradedPickupEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp914
{
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Interfaces;
    using global::Scp914;

    /// <summary>
    /// Contains all information after SCP-914 upgrades an item.
    /// </summary>
    public class UpgradedPickupEventArgs : IPickupEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradedPickupEventArgs"/> class.
        /// </summary>
        /// <param name="pickup"><inheritdoc cref="Pickup"/></param>
        /// <param name="scp914KnobSetting"><inheritdoc cref="Setting"/></param>
        public UpgradedPickupEventArgs(Pickup pickup, Scp914KnobSetting scp914KnobSetting)
        {
            Pickup = pickup;
            Setting = scp914KnobSetting;
        }

        /// <summary>
        /// Gets the upgraded pickup.
        /// </summary>
        public Pickup Pickup { get; }

        /// <summary>
        /// Gets the <see cref="Scp914KnobSetting"/> on which item was upgraded.
        /// </summary>
        public Scp914KnobSetting Setting { get; }
    }
}