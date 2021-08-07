// -----------------------------------------------------------------------
// <copyright file="HidState.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Possible <see cref="API.Features.Items.MicroHid"/> states.
    /// </summary>
    public enum HidState
    {
        /// <summary>
        /// Idling and not using energy.
        /// </summary>
        Idle,

        /// <summary>
        /// Powering up and using energy slowly.
        /// </summary>
        PoweringUp,

        /// <summary>
        /// Powering down and not using energy.
        /// </summary>
        PoweringDown,

        /// <summary>
        /// Fully powered up and ready to fire.
        /// </summary>
        Primed,

        /// <summary>
        /// Firing and using energy.
        /// </summary>
        Firing,
    }
}
