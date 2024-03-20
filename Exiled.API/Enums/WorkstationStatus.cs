// -----------------------------------------------------------------------
// <copyright file="WorkstationStatus.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// All available workstation statuses.
    /// </summary>
    /// <seealso cref="API.Features.Workstation.CurrentStatus"/>
    public enum WorkstationStatus : byte
    {
        /// <summary>
        /// Workstation is currently offline.
        /// </summary>
        Offline,

        /// <summary>
        /// Workstation is currently powering up.
        /// </summary>
        PoweringUp,

        /// <summary>
        /// Workstation is currently powering down.
        /// </summary>
        PoweringDown,

        /// <summary>
        /// Workstation is currently online.
        /// </summary>
        Online,
    }
}