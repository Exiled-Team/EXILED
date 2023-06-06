// -----------------------------------------------------------------------
// <copyright file="KeycardPermissions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using System;

    using Features;
    using Features.Items;

    /// <summary>
    /// The types of permissions assigned to keycards.
    /// </summary>
    /// <seealso cref="Generator.KeycardPermissions"/>
    /// <seealso cref="Generator.SetPermissionFlag(KeycardPermissions, bool)"/>
    /// <seealso cref="Keycard.Permissions"/>
    [Flags]
    public enum KeycardPermissions
    {
        /// <summary>
        /// No permissions.
        /// </summary>
        None = 0,

        /// <summary>
        /// Opens checkpoints.
        /// </summary>
        Checkpoints = 1 << 0,

        /// <summary>
        /// Opens <see cref="DoorType.GateA">Gate A</see> and <see cref="DoorType.GateB">Gate B</see>.
        /// </summary>
        ExitGates = 1 << 1,

        /// <summary>
        /// Opens <see cref="DoorType.Intercom">the Intercom door</see>.
        /// </summary>
        Intercom = 1 << 2,

        /// <summary>
        /// Opens the Alpha Warhead detonation room on surface.
        /// </summary>
        AlphaWarhead = 1 << 3,

        /// <summary>
        /// Opens <see cref="DoorType.Scp914Gate"/>.
        /// </summary>
        ContainmentLevelOne = 1 << 4, // 0x0010

        /// <summary>
        /// <see cref="ContainmentLevelOne"/>, <see cref="Checkpoints"/>.
        /// </summary>
        ContainmentLevelTwo = 1 << 5, // 0x0020

        /// <summary>
        /// <see cref="ContainmentLevelTwo"/>, <see cref="Intercom"/>, <see cref="AlphaWarhead"/>.
        /// </summary>
        ContainmentLevelThree = 1 << 6, // 0x0040

        /// <summary>
        /// <see cref="Checkpoints"/>, Opens Light Containment armory.
        /// </summary>
        ArmoryLevelOne = 1 << 7, // 0x0080

        /// <summary>
        /// <see cref="ArmoryLevelOne"/>, <see cref="ExitGates"/>, Opens Heavy Containment armories.
        /// </summary>
        ArmoryLevelTwo = 1 << 8, // 0x0100

        /// <summary>
        /// <see cref="ArmoryLevelTwo"/>, <see cref="Intercom"/>, Opens MicroHID room.
        /// </summary>
        ArmoryLevelThree = 1 << 9, // 0x0200

        /// <summary>
        /// <see cref="Checkpoints"/>.
        /// </summary>
        ScpOverride = 1 << 10, // 0x0400
    }
}