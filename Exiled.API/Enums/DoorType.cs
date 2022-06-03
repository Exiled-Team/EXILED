// -----------------------------------------------------------------------
// <copyright file="DoorType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using System;

    /// <summary>
    /// Unique identifier for the different types of doors.
    /// </summary>
    public enum DoorType
    {
        /// <summary>
        /// Represents an unknown door.
        /// </summary>
        UnknownDoor = 0,

        /// <summary>
        /// Represents the 914 door.
        /// </summary>
        Scp914Door,

        /// <summary>
        /// Represents the GR18_INNER door.
        /// </summary>
        GR18Inner,

        /// <summary>
        /// Represents the Unsecured door.
        /// </summary>
        Scp049Gate,

        /// <summary>
        /// Represents the 049_ARMORY door.
        /// </summary>
        Scp049Armory,

        /// <summary>
        /// Represents the 079_FIRST door.
        /// </summary>
        Scp079First,

        /// <summary>
        /// Represents the 079_SECOND door.
        /// </summary>
        Scp079Second,

        /// <summary>
        /// Represents the 096 door.
        /// </summary>
        Scp096,

        /// <summary>
        /// Represents the 106_BOTTOM door.
        /// </summary>
        Scp106Bottom,

        /// <summary>
        /// Represents the 106_PRIMARY door.
        /// </summary>
        Scp106Primary,

        /// <summary>
        /// Represents the 106_SECONDARY door.
        /// </summary>
        Scp106Secondary,

        /// <summary>
        /// Represents the 173 gate.
        /// </summary>
        Scp173Gate,

        /// <summary>
        /// Represents the door between the 173 gate and the 173 armory.
        /// </summary>
        Scp173Connector,

        /// <summary>
        /// Represents the 173_ARMORY door.
        /// </summary>
        Scp173Armory,

        /// <summary>
        /// Represents the 173_BOTTOM door.
        /// </summary>
        Scp173Bottom,

        /// <summary>
        /// Represents the GR18 gate.
        /// </summary>
        GR18Gate,

        /// <inheritdoc cref="GR18Gate"/>
        [Obsolete("Use DoorType.GR18Gate instead.", true)]
        GR18 = GR18Gate,

        /// <summary>
        /// Represents the 914 gate.
        /// </summary>
        Scp914Gate,

        /// <inheritdoc cref="Scp914Gate"/>
        [Obsolete("Use DoorType.Scp914Gate instead.", true)]
        Scp914 = Scp914Gate,

        /// <summary>
        /// Represents the CHECKPOINT_ENT door.
        /// </summary>
        CheckpointEntrance,

        /// <summary>
        /// Represents the CHECKPOINT_LCZ_A door.
        /// </summary>
        CheckpointLczA,

        /// <summary>
        /// Represents the CHECKPOINT_LCZ_B door.
        /// </summary>
        CheckpointLczB,

        /// <summary>
        /// Represents any entrance zone styled door.
        /// </summary>
        EntranceDoor,

        /// <summary>
        /// Represents the ESCAPE_PRIMARY door.
        /// </summary>
        EscapePrimary,

        /// <summary>
        /// Represents the ESCAPE_SECONDARY door.
        /// </summary>
        EscapeSecondary,

        /// <summary>
        /// Represents the SERVERS_BOTTOM door.
        /// </summary>
        ServersBottom,

        /// <summary>
        /// Represents the GATE_A door.
        /// </summary>
        GateA,

        /// <summary>
        /// Represents the GATE_B door.
        /// </summary>
        GateB,

        /// <summary>
        /// Represents the HCZ_ARMORY door.
        /// </summary>
        HczArmory,

        /// <summary>
        /// Represents any heavy containment styled door.
        /// </summary>
        HeavyContainmentDoor,

        /// <summary>
        /// Represents the HID door.
        /// </summary>
        HID,

        /// <summary>
        /// Represents the HID_LEFT door.
        /// </summary>
        HIDLeft,

        /// <summary>
        /// Represents the HID_RIGHT door.
        /// </summary>
        HIDRight,

        /// <summary>
        /// Represents the INTERCOM door.
        /// </summary>
        Intercom,

        /// <summary>
        /// Represents the LCZ_ARMORY door.
        /// </summary>
        LczArmory,

        /// <summary>
        /// Represents the LCZ_CAFE door.
        /// </summary>
        LczCafe,

        /// <summary>
        /// Represents the LCZ_WC door.
        /// </summary>
        LczWc,

        /// <summary>
        /// Represents any light containment styled door.
        /// </summary>
        LightContainmentDoor,

        /// <summary>
        /// Represents the NUKE_ARMORY door.
        /// </summary>
        NukeArmory,

        /// <summary>
        /// Represents the NUKE_SURFACE door.
        /// </summary>
        NukeSurface,

        /// <summary>
        /// Represents any of the Class-D cell doors.
        /// </summary>
        PrisonDoor,

        /// <summary>
        /// Represents the SURFACE_GATE door.
        /// </summary>
        SurfaceGate,

        /// <summary>
        /// Represents the 330 door.
        /// </summary>
        Scp330,

        /// <summary>
        /// Represents the 330_CHAMBER door.
        /// </summary>
        Scp330Chamber,

        /// <summary>
        /// Represents the 012 door.
        /// </summary>
        [Obsolete("Removed from the game.", true)]
        Scp012,

        /// <summary>
        /// Represents the 012_BOTTOM door.
        /// </summary>
        [Obsolete("Removed from the game.", true)]
        Scp012Bottom,
    }
}
