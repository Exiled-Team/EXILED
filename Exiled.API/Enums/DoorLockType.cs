// -----------------------------------------------------------------------
// <copyright file="DoorLockType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using System;

    using Features;

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
        None = 1 << 0,

        /// <summary>
        /// Regular SCP-079 door lock.
        /// </summary>
        Regular079 = 1 << 1,

        /// <summary>
        /// SCP-079 lockdown room lock.
        /// </summary>
        Lockdown079 = 1 << 2,

        /// <summary>
        /// Alpha Warhead detonation lock.
        /// </summary>
        Warhead = 1 << 3,

        /// <summary>
        /// Locked via admin command.
        /// </summary>
        AdminCommand = 1 << 4,

        /// <summary>
        /// Locked by decontamination lockdown (after decon starts).
        /// </summary>
        DecontLockdown = 1 << 5, // 0x0010

        /// <summary>
        /// Locked by decontamination evacuation (during final countdown to decon).
        /// </summary>
        DecontEvacuate = 1 << 6, // 0x0020

        /// <summary>
        /// Special door features.
        /// </summary>
        SpecialDoorFeature = 1 << 7, // 0x0040

        /// <summary>
        /// Door has no power.
        /// </summary>
        NoPower = 1 << 8, // 0x0080

        /// <summary>
        /// Isloation.
        /// </summary>
        Isolation = 1 << 9, // 0x0100

        /// <summary>
        /// Locked down by SCP-2176.
        /// </summary>
        Lockdown2176 = 1 << 10,

        /// <summary>
        /// Locked down by Plugin.
        /// </summary>
        Plugin = 1 << 62,
    }
}