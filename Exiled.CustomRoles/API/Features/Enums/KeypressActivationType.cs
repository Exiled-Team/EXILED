// -----------------------------------------------------------------------
// <copyright file="KeypressActivationType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.API.Features.Enums
{
    /// <summary>
    /// The Action type that should be triggered from a keypress trigger.
    /// </summary>
    public enum AbilityKeypressTriggerType
    {
        /// <summary>
        /// No action.
        /// </summary>
        None,

        /// <summary>
        /// Activate ability.
        /// </summary>
        Activate,

        /// <summary>
        /// Switch to next ability.
        /// </summary>
        SwitchForward,

        /// <summary>
        /// Switch to previous ability.
        /// </summary>
        SwitchBackward,

        /// <summary>
        /// Display information about the ability to the user.
        /// </summary>
        DisplayInfo,
    }
}