// -----------------------------------------------------------------------
// <copyright file="HotkeyButton.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums {
    /// <summary>
    /// Hotkey button types.
    /// </summary>
    public enum HotkeyButton {
        /// <summary>
        /// The hotkey button for selecting the keycard.
        /// </summary>
        Keycard = 0,

        /// <summary>
        /// The hotkey button for selecting the primary firearm.
        /// </summary>
        PrimaryFirearm = 1,

        /// <summary>
        /// The hotkey button for selecting the secondary firearm.
        /// </summary>
        SecondaryFirearm = 2,

        /// <summary>
        /// The hotkey button for selecting the medical item.
        /// </summary>
        Medical = 3,

        /// <summary>
        /// The hotkey button for selecting the grenade.
        /// </summary>
        Grenade = 4,
    }
}
