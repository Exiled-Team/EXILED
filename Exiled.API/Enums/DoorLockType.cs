// -----------------------------------------------------------------------
// <copyright file="DoorLockType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using System;

    using Features.Doors;

    /// <summary>
    /// All possible door locks.
    /// </summary>
    /// <seealso cref="Door.LockAll(float, DoorLockType)"/>
    /// <seealso cref="Door.ChangeLock(DoorLockType)"/>
    [Flags]
    public enum DoorLockType
    {
        /// <summary>
        /// Unlocked.
        /// </summary>
        None = 0,

        /// <summary>
        /// Regular SCP-079 door lock.
        /// </summary>
        Regular079 = 1,

        /// <summary>
        /// SCP-079 lockdown room lock.
        /// </summary>
        Lockdown079 = 2,

        /// <summary>
        /// Alpha Warhead detonation lock.
        /// </summary>
        Warhead = 4,

        /// <summary>
        /// Locked via admin command.
        /// </summary>
        AdminCommand = 8,

        /// <summary>
        /// Locked by decontamination lockdown (after decon starts).
        /// </summary>
        DecontLockdown = 16, // 0x0010

        /// <summary>
        /// Locked by decontamination evacuation (during final countdown to decon).
        /// </summary>
        DecontEvacuate = 32, // 0x0020

        /// <summary>
        /// Special door features.
        /// </summary>
        SpecialDoorFeature = 64, // 0x0040

        /// <summary>
        /// Door has no power.
        /// </summary>
        NoPower = 128, // 0x0080

        /// <summary>
        /// Isloation.
        /// </summary>
        Isolation = 256, // 0x0100

        /// <summary>
        /// Locked down by SCP-2176.
        /// </summary>
        Lockdown2176 = 512,
    }
}