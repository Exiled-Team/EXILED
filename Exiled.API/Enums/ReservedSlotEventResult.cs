// -----------------------------------------------------------------------
// <copyright file="ReservedSlotEventResult.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Result for ReservedSlots event.
    /// </summary>
    public enum ReservedSlotEventResult
    {
        /// <summary>
        /// Don't override the base game decision.
        /// </summary>
        UseBaseGameSystem,

        /// <summary>
        /// Override: player has a reserved slot.
        /// </summary>
        CanUseReservedSlots,

        /// <summary>
        /// Override: player doesn't have a reserved slot.
        /// </summary>
        CannotUseReservedSlots,

        /// <summary>
        /// Bypass the reserved slots system and allow the connection unconditionally.
        /// </summary>
        AllowConnectionUnconditionally,
    }
}