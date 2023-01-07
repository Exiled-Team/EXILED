// -----------------------------------------------------------------------
// <copyright file="PingType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Enum that represents the type of SCP-079 ping.
    /// </summary>
    public enum PingType : byte
    {
        /// <summary>
        /// Represents a generator ping.
        /// </summary>
        Generator,

        /// <summary>
        /// Represents a projectile ping.
        /// </summary>
        Projectile,

        /// <summary>
        /// Represents a Micro-HID ping.
        /// </summary>
        MicroHid,

        /// <summary>
        /// Represents a human ping.
        /// </summary>
        Human,

        /// <summary>
        /// Represents an elevator ping.
        /// </summary>
        Elevator,

        /// <summary>
        /// Represents a door ping.
        /// </summary>
        Door,

        /// <summary>
        /// Represents a general ping.
        /// </summary>
        Default,
    }
}
