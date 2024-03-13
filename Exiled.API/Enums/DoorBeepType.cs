// -----------------------------------------------------------------------
// <copyright file="DoorBeepType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using Exiled.API.Features.Doors;

    /// <summary>
    /// Door beep types.
    /// </summary>
    /// <seealso cref="Door.PlaySound(DoorBeepType)"/>
    public enum DoorBeepType
    {
        /// <summary>
        /// Permission denied beep.
        /// </summary>
        PermissionDenied,

        /// <summary>
        /// Lock bypass is denied.
        /// </summary>
        LockBypassDenied,

        /// <summary>
        /// Interaction allowed.
        /// </summary>
        InteractionAllowed,
    }
}