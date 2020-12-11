// -----------------------------------------------------------------------
// <copyright file="CameraType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Unique identifier for the different types of SCP-079 cameras.
    /// </summary>
    public enum CameraType
    {
#pragma warning disable SA1124 // Do not use regions
        #region Lcz

        /// <summary>
        /// Represents the camera inside the Class-D spawns.
        /// </summary>
        LczClassDSpawn = 55,

        /// <summary>
        /// Represents the camera inside SCP-372's containment room.
        /// </summary>
        LczGlassBox = 92,

        /// <summary>
        /// Represents the camera outside of SCP-173's containment chamber.
        /// </summary>
        Scp173Hallway = 11,

        /// <summary>
        /// Represents the camera outside the 173_ARMORY door.
        /// </summary>
        Scp173Armory = 12,

        /// <summary>
        /// Represents the camera inside of SCP-173's containment chamber.
        /// </summary>
        Scp173Containment = 13,

        /// <summary>
        /// Represents the camera above the 173_BOTTOM door.
        /// </summary>
        Scp173Bottom = 14,

        /// <summary>
        /// Represents the camera outside of SCP-012's room
        /// </summary>
        Lcz012 = 54,

        /// <summary>
        /// Represents the camera at the bottom of SCP-012's containment chamber.
        /// </summary>
        Lcz012Bottom = 53,

        /// <summary>
        /// Represents the LCZ cafe.
        /// </summary>
        LczCafe = 59,

        /// <summary>
        /// Represents the camera inside the LCZ armory.
        /// </summary>
        LczArmory = 81,

        /// <summary>
        /// Represents the plant room in LCZ.
        /// </summary>
        LczPlants = 82,

        /// <summary>
        /// Represents the camera inside the WC hallway.
        /// </summary>
        WC = 20,

        /// <summary>
        /// Represents the camera inside SCP-914.
        /// </summary>
        Scp914 = 36,

        /// <summary>
        /// Represents the camera outside SCP-914.
        /// </summary>
        Scp914Outside = 35,

        /// <summary>
        /// Represents the camera in the LCZ A <see cref="Lift"/> in LCZ.
        /// </summary>
        LczALight = 75,

        /// <summary>
        /// Represents the camera in the LCZ B <see cref="Lift"/> in LCZ.
        /// </summary>
        LczBLight = 17,

        /// <summary>
        /// Represents the camera outside of the HCZ A checkpoint (in LCZ).
        /// </summary>
        LczAChkp = 74,

        /// <summary>
        /// Represents the camera outside of the HCZ B checkpoint (in LCZ).
        /// </summary>
        LczBChkp = 18,

        /// <summary>
        /// Represents the camera outside of the LCZ A checkpoint (in LCZ).
        /// </summary>
        HczAChkp = 76,

        /// <summary>
        /// Represents the camera outside of the LCZ B checkpoint (in LCZ).
        /// </summary>
        HczBChkp = 16,
        #endregion

        #region Hcz

        /// <summary>
        /// Represents the camera in the LCZ A <see cref="Lift"/> in HCZ.
        /// </summary>
        LczAHeavy = 60,

        /// <summary>
        /// Represents the camera in the LCZ B <see cref="Lift"/> in heavy.
        /// </summary>
        LczBHeavy = 91,

        /// <summary>
        /// Represents the camera in the pre-hallway in front of 079_FIRST door.
        /// </summary>
        Scp079PreHallway = 77,

        /// <summary>
        /// Represents the camera in the hallway between the 079_FIRST and 079_SECOND doors.
        /// </summary>
        Scp079Hallway = 78,

        /// <summary>
        /// Represents the camera inside SCP-079's containment chamber.
        /// </summary>
        Scp079Interior = 198,

        /// <summary>
        /// Represents the camera outside of SCP-096's containment chamber.
        /// </summary>
        Scp096 = 61,

        /// <summary>
        /// Represents the camera in the SCP-049 <see cref="Lift"/> hallway that is in front of the elevator.
        /// </summary>
        Scp049Primary = 67,

        /// <summary>
        /// Represents the camera in the SCP-049 <see cref="Lift"/> hallway that is not in front of the elevator.
        /// </summary>
        Scp049Secondary = 69,

        /// <summary>
        /// Represents the camera that faces toward the 049_ARMORY door.
        /// </summary>
        Scp049Armory = 64,

        /// <summary>
        /// Represents the hallway camera in the server room.
        /// </summary>
        ServerHall = 83,

        /// <summary>
        /// Represents the camera on the bottom level of the server room.
        /// </summary>
        ServerBottom = 84,

        /// <summary>
        /// Represents the camera on the top level of the server room.
        /// </summary>
        ServerTop = 85,

        /// <summary>
        /// Represents the camera in the HID hallway.
        /// </summary>
        HIDHall = 71,

        /// <summary>
        /// Represents the camera inside the Micro-HID room.
        /// </summary>
        HIDInterior = 72,

        /// <summary>
        /// Represents the camera outside of the alpha warhead <see cref="Lift"/> in HCZ.
        /// </summary>
        NukeElevator = 90,

        /// <summary>
        /// Represents the camera inside of the alpha warhead room.
        /// </summary>
        NukeRoom = 89,

        /// <summary>
        /// Represents the camera above the alpha warhead switch.
        /// </summary>
        NukeSwitch = 87,

        /// <summary>
        /// Represents the camera inside the alpha warhead armory.
        /// </summary>
        NukeArmory = 88,

        /// <summary>
        /// Represents the camera inside SCP-939's containment room.
        /// </summary>
        Scp939 = 79,

        /// <summary>
        /// Represents the camera above the HCZ_ARMORY door.
        /// </summary>
        HczArmory = 38,

        /// <summary>
        /// Represents the camera above the door leading to SCP-106's room.
        /// </summary>
        Scp106First = 46,

        /// <summary>
        /// Represents the camera outside the 106_PRIMARY door.
        /// </summary>
        Scp106Second = 43,

        /// <summary>
        /// Represents the camera above the 106_PRIMARY door (inside the containment room).
        /// </summary>
        Scp106Primary = 45,

        /// <summary>
        /// Represents the camera above the 106_SECONDARY door (inside the containment room).
        /// </summary>
        Scp106Secondary = 44,

        /// <summary>
        /// Represents the camera above the femur breaker.
        /// </summary>
        Scp106Recontainer = 47,

        /// <summary>
        /// Represents the camera facing toward the stairs in SCP-106's containment chamber.
        /// </summary>
        Scp106Stairs = 48,

        /// <summary>
        /// Represents the camera facing toward the entrance zone checkpoint (in HCZ).
        /// </summary>
        ChkpEz = 99,
        #endregion

        #region Ez

        /// <summary>
        /// Represents the camera facing toward the heavy containment zone checkpoint (in EZ).
        /// </summary>
        ChkpHcz = 100,

        /// <summary>
        /// Represents the camera outside the INTERCOM door.
        /// </summary>
        IntercomHall = 50,

        /// <summary>
        /// Represents the camera inside the INTERCOM door.
        /// </summary>
        IntercomStairs = 52,

        /// <summary>
        /// Represents the camera facing the intercom.
        /// </summary>
        IntercomInside = 49,

        /// <summary>
        /// Represents the camera inside of Gate A (entrance zone).
        /// </summary>
        GateAEntrance = 62,

        /// <summary>
        /// Represents the camera inside of Gate B (entrance zone).
        /// </summary>
        GateBEntrance = 10,
        #endregion

        #region Surface

        /// <summary>
        /// Represents the camera outside of the Gate A elevator (surface).
        /// </summary>
        GateA = 27,

        /// <summary>
        /// Represents the camera above the Gate A balcony.
        /// </summary>
        GateAOutside = 32,

        /// <summary>
        /// Represents the camera on the tower at Gate A.
        /// </summary>
        GateATower = 28,

        /// <summary>
        /// Represents the camera facing the NUKE_SURFACE door.
        /// </summary>
        NukeSurface = 29,

        /// <summary>
        /// Represents the camera facing the SURFACE_GATE door (Gate B side)
        /// </summary>
        SurfaceGate = 30,

        /// <summary>
        /// Represents the camera on the Gate B walkway.
        /// </summary>
        GateB = 33,

        /// <summary>
        /// Represents the HELIPAD camera.
        /// </summary>
        GateBOutside = 26,

        /// <summary>
        /// Represents the camera facing toward the ESCAPE door.
        /// </summary>
        EscapeOutside = 25,

        /// <summary>
        /// Represents the camera above the Class-D and Scientist extraction point.
        /// </summary>
        EscapeInner = 31,

        /// <summary>
        /// Represents an unknown camera.
        /// </summary>
        Unknown = 0,
        #endregion
    }
}
