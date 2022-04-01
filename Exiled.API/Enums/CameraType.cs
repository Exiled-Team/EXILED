// -----------------------------------------------------------------------
// <copyright file="CameraType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1124 // Do not use regions
namespace Exiled.API.Enums {
    /// <summary>
    /// Unique identifier for the different types of SCP-079 cameras.
    /// </summary>
    public enum CameraType {
        /// <summary>
        /// Represents an unknown camera.
        /// </summary>
        Unknown = 0,
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
        Lcz173Hallway = 11,

        /// <summary>
        /// Represents the camera outside the 173_ARMORY door.
        /// </summary>
        Lcz173Armory = 12,

        /// <summary>
        /// Represents the camera inside of SCP-173's containment chamber.
        /// </summary>
        Lcz173Containment = 13,

        /// <summary>
        /// Represents the camera above the 173_BOTTOM door.
        /// </summary>
        Lcz173Bottom = 14,

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
        Lcz914 = 36,

        /// <summary>
        /// Represents the camera outside SCP-914.
        /// </summary>
        Lcz914Hallway = 35,

        /// <summary>
        /// Represents the camera in the LCZ A <see cref="Lift"/> in LCZ.
        /// </summary>
        LczALight = 75,

        /// <summary>
        /// Represents the camera in the LCZ B <see cref="Lift"/> in LCZ.
        /// </summary>
        LczBLight = 17,

        /// <summary>
        /// Represents the camera facing the LCZ/HCZ A checkpoint, on the Light Containment side.
        /// </summary>
        LczAChkp = 74,

        /// <summary>
        /// Represents the camera facing the LCZ/HCZ B checkpoint, on the Light Containment side.
        /// </summary>
        LczBChkp = 18,

        /// <summary>
        /// Represents the camera facing the LCZ/HCZ A checkpoint, on the Heavy Containment side.
        /// </summary>
        HczAChkp = 76,

        /// <summary>
        /// Represents the camera facing the LCZ/HCZ B checkpoint, on the Heavy Containment side.
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
        Hcz079PreHallway = 77,

        /// <summary>
        /// Represents the camera in the hallway between the 079_FIRST and 079_SECOND doors.
        /// </summary>
        Hcz079Hallway = 78,

        /// <summary>
        /// Represents the camera inside SCP-079's containment chamber.
        /// </summary>
        Hcz079Interior = 198,

        /// <summary>
        /// Represents the camera outside of SCP-096's containment chamber.
        /// </summary>
        Hcz096 = 61,

        /// <summary>
        /// Represents the camera in the SCP-049 <see cref="Lift"/> hallway that is in front of the elevator.
        /// </summary>
        Hcz049Elevator = 67,

        /// <summary>
        /// Represents the camera in the SCP-049 <see cref="Lift"/> hallway that is not in front of the elevator.
        /// </summary>
        Hcz049Hall = 69,

        /// <summary>
        /// Represents the camera that faces toward the 049_ARMORY door.
        /// </summary>
        Hcz049Armory = 64,

        /// <summary>
        /// Represents the hallway camera in the server room.
        /// </summary>
        HczServerHall = 83,

        /// <summary>
        /// Represents the camera on the bottom level of the server room.
        /// </summary>
        HczServerBottom = 84,

        /// <summary>
        /// Represents the camera on the top level of the server room.
        /// </summary>
        HczServerTop = 85,

        /// <summary>
        /// Represents the camera in the HID hallway.
        /// </summary>
        HczHidHall = 71,

        /// <summary>
        /// Represents the camera inside the Micro-HID room.
        /// </summary>
        HczHidInterior = 72,

        /// <summary>
        /// Represents the camera outside of the alpha warhead <see cref="Lift"/> in HCZ.
        /// </summary>
        HczWarheadHall = 90,

        /// <summary>
        /// Represents the camera inside of the alpha warhead room.
        /// </summary>
        HczWarheadRoom = 89,

        /// <summary>
        /// Represents the camera above the alpha warhead switch.
        /// </summary>
        HczWarheadSwitch = 87,

        /// <summary>
        /// Represents the camera inside the alpha warhead armory.
        /// </summary>
        HczWarheadArmory = 88,

        /// <summary>
        /// Represents the camera inside SCP-939's containment room.
        /// </summary>
        Hcz939 = 79,

        /// <summary>
        /// Represents the camera above the HCZ_ARMORY door.
        /// </summary>
        HczArmory = 38,

        /// <summary>
        /// Represents the SCP-106 MAIN CAM camera (above the door leading to SCP-106's room).
        /// </summary>
        Hcz106First = 46,

        /// <summary>
        /// Represents the SCP-106 SECOND camera (outside the 106_PRIMARY door).
        /// </summary>
        Hcz106Second = 43,

        /// <summary>
        /// Represents the 106 ENT A camera (above the 106_PRIMARY door inside the containment room).
        /// </summary>
        Hcz106Primary = 45,

        /// <summary>
        /// Represents the 106 ENT B camera (above the 106_SECONDARY door inside the containment room).
        /// </summary>
        Hcz106Secondary = 44,

        /// <summary>
        /// Represents the camera above the femur breaker.
        /// </summary>
        Hcz106Recontainer = 47,

        /// <summary>
        /// Represents the camera facing toward the stairs in SCP-106's containment chamber.
        /// </summary>
        Hcz106Stairs = 48,

        /// <summary>
        /// Represents the camera facing toward the entrance zone checkpoint (in HCZ).
        /// </summary>
        HczChkpEz = 99,
        #endregion

        #region Ez

        /// <summary>
        /// Represents the camera facing toward the heavy containment zone checkpoint (in EZ).
        /// </summary>
        EzChkpHcz = 100,

        /// <summary>
        /// Represents the camera outside the INTERCOM door.
        /// </summary>
        EzIntercomHall = 50,

        /// <summary>
        /// Represents the camera inside the INTERCOM door.
        /// </summary>
        EzIntercomStairs = 52,

        /// <summary>
        /// Represents the camera facing the intercom.
        /// </summary>
        EzIntercomInterior = 49,

        /// <summary>
        /// Represents the camera inside of Gate A (entrance zone).
        /// </summary>
        EzGateA = 62,

        /// <summary>
        /// Represents the camera inside of Gate B (entrance zone).
        /// </summary>
        EzGateB = 10,
        #endregion

        #region Surface

        /// <summary>
        /// Represents the camera outside of the Gate A elevator (surface).
        /// </summary>
        GateA = 27,

        /// <summary>
        /// Represents the camera above the Gate A balcony.
        /// </summary>
        Bridge = 32,

        /// <summary>
        /// Represents the camera on the tower at Gate A.
        /// </summary>
        Tower = 28,

        /// <summary>
        /// Represents the camera facing the NUKE_SURFACE door.
        /// </summary>
        Backstreet = 29,

        /// <summary>
        /// Represents the camera facing the SURFACE_GATE door (Gate B side)
        /// </summary>
        SurfaceGate = 30,

        /// <summary>
        /// Represents the camera on the Gate B walkway.
        /// </summary>
        Streetcam = 33,

        /// <summary>
        /// Represents the HELIPAD camera.
        /// </summary>
        Helipad = 26,

        /// <summary>
        /// Represents the ESCAPE ZONE camera (facing toward the ESCAPE door).
        /// </summary>
        EscapeZone = 25,

        /// <summary>
        /// Represents the EXIT camera (above the Class-D and Scientist extraction point).
        /// </summary>
        Exit = 31,
        #endregion
    }
}
