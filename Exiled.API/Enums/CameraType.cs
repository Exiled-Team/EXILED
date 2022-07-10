// -----------------------------------------------------------------------
// <copyright file="CameraType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1124 // Do not use regions
namespace Exiled.API.Enums
{
    /// <summary>
    /// Unique identifier for the different types of SCP-079 cameras.
    /// </summary>
    public enum CameraType
    {
        /// <summary>
        /// Represents an unknown camera.
        /// </summary>
        Unknown,
        #region Lcz

        /// <summary>
        /// Represents the camera facing the LCZ/HCZ A checkpoint, on the Light Containment side.
        /// </summary>
        HczAEntrance,

        /// <summary>
        /// Represents the camera facing the LCZ/HCZ B checkpoint, on the Light Containment side.
        /// </summary>
        HczBEntrance,

        /// <summary>
        /// Represents the camera outside the <c>173_ARMORY</c> door.
        /// </summary>
        Lcz173Armory,

        /// <summary>
        /// Represents the camera above the <c>173_BOTTOM</c> door.
        /// </summary>
        Lcz173Bottom,

        /// <summary>
        /// Represents the camera inside of SCP-173's containment chamber.
        /// </summary>
        Lcz173Chamber,

        /// <summary>
        /// Represents the camera outside of SCP-173's containment chamber.
        /// </summary>
        Lcz173Hallway,

        /// <summary>
        /// Represents the camera inside SCP-330's containment chamber.
        /// </summary>
        Lcz330Chamber,

        /// <summary>
        /// Represents the camera inside the hall leading to SCP-330's containment chamber.
        /// </summary>
        Lcz330Hall,

        /// <summary>
        /// Represents the camera inside SCP-914's containment chamber.
        /// </summary>
        Lcz914,

        /// <summary>
        /// Represents the camera outside SCP-914.
        /// </summary>
        Lcz914Hallway,

        /// <summary>
        /// Represents a camera inside of an LCZ airlock.
        /// </summary>
        LczAirlock,

        /// <summary>
        /// Represents the camera in the LCZ A <see cref="Lift"/> in LCZ.
        /// </summary>
        LczALifts,

        /// <summary>
        /// Represents the camera inside the LCZ armory.
        /// </summary>
        LczArmory,

        /// <summary>
        /// Represents the camera in the LCZ B <see cref="Lift"/> in LCZ.
        /// </summary>
        LczBLifts,

        /// <summary>
        /// Represents the LCZ cafe/office, also known as PC-15.
        /// </summary>
        LczCafe,

        /// <summary>
        /// Represents the camera inside the Class-D spawns.
        /// </summary>
        LczClassDSpawn,

        /// <summary>
        /// Represents a camera in a LCZ corner.
        /// </summary>
        LczCorner,

        /// <summary>
        /// Represents the camera inside SCP-372's containment room.
        /// </summary>
        LczGlassRoom,

        /// <summary>
        /// Represents the greenhouse room in LCZ.
        /// </summary>
        LczGreenhouse,

        /// <summary>
        /// Represents a camera in a LCZ hallway.
        /// </summary>
        LczHall,

        /// <summary>
        /// Represents a camera in a LCZ T-Intersection.
        /// </summary>
        LczTIntersection,

        /// <summary>
        /// Represents the camera inside the WC hallway.
        /// </summary>
        LczWC,

        /// <summary>
        /// Represents a camera in a LCZ X-Intersection.
        /// </summary>
        LczXIntersection,

        #endregion

        #region Hcz

        /// <summary>
        /// Represents the camera in the SCP-049 <see cref="Lift"/> hallway that is in front of the elevator.
        /// </summary>
        Hcz049Elevator,

        /// <summary>
        /// Represents one of the four cameras found in the hallway leading to SCP-049's containment chamber, and the SCP-049 armory.
        /// </summary>
        Hcz049Hall,

        /// <summary>
        /// Represents the camera that faces toward the <c>049_ARMORY</c> <see cref="Features.Door"/>.
        /// </summary>
        Hcz049Armory,

        /// <summary>
        /// Represents the camera inside SCP-079's control room.
        /// </summary>
        Hcz079Control,

        /// <summary>
        /// Represents the camera in the hallway between the 079_FIRST and 079_SECOND doors.
        /// </summary>
        Hcz079Hallway,

        /// <summary>
        /// Represents the camera inside SCP-079's containment chamber.
        /// </summary>
        Hcz079Main,

        /// <summary>
        /// Represents the camera in the pre-hallway in front of 079_FIRST door.
        /// </summary>
        Hcz079PreHallway,

        /// <summary>
        /// Represents the camera outside of SCP-096's containment chamber.
        /// </summary>
        Hcz096,

        /// <summary>
        /// Represents the SCP-106 MAIN CAM camera (above the door leading to SCP-106's room).
        /// </summary>
        Hcz106First,

        /// <summary>
        /// Represents the 106 ENT A camera (above the 106_PRIMARY door inside the containment room).
        /// </summary>
        Hcz106Primary,

        /// <summary>
        /// Represents the camera above the femur breaker.
        /// </summary>
        Hcz106Recontainer,

        /// <summary>
        /// Represents the SCP-106 SECOND camera (outside the 106_PRIMARY door).
        /// </summary>
        Hcz106Second,

        /// <summary>
        /// Represents the 106 ENT B camera (above the 106_SECONDARY door inside the containment room).
        /// </summary>
        Hcz106Secondary,

        /// <summary>
        /// Represents the camera facing toward the stairs in SCP-106's containment chamber.
        /// </summary>
        Hcz106Stairs,

        /// <summary>
        /// Represents the camera inside SCP-939's containment room.
        /// </summary>
        Hcz939,

        /// <summary>
        /// Represents the camera in the LCZ A <see cref="Lift"/> in HCZ.
        /// </summary>
        HczALifts,

        /// <summary>
        /// Represents the camera above the HCZ_ARMORY door.
        /// </summary>
        HczArmory,

        /// <summary>
        /// Represents the camera in the LCZ B <see cref="Lift"/> in heavy.
        /// </summary>
        HczBLifts,

        /// <summary>
        /// Represents a camera in a HCZ corner.
        /// </summary>
        HczCorner,

        /// <summary>
        /// Represents a camera in a HCZ hallway.
        /// </summary>
        HczHall,

        /// <summary>
        /// Represents the camera in the HID hallway.
        /// </summary>
        HczHidHall,

        /// <summary>
        /// Represents the camera inside the Micro-HID room.
        /// </summary>
        HczHidInterior,

        /// <summary>
        /// Represents the camera on the bottom level of the server room.
        /// </summary>
        HczServerBottom,

        /// <summary>
        /// Represents the hallway camera in the server room.
        /// </summary>
        HczServerHall,

        /// <summary>
        /// Represents the camera on the top level of the server room.
        /// </summary>
        HczServerTop,

        /// <summary>
        /// Represents a camera placed next to an HCZ tesla gate.
        /// </summary>
        HczTeslaGate,

        /// <summary>
        /// Represents the camera inside the alpha warhead armory.
        /// </summary>
        HczWarheadArmory,

        /// <summary>
        /// Represents the camera outside of the alpha warhead <see cref="Lift"/> in HCZ.
        /// </summary>
        HczWarheadHall,

        /// <summary>
        /// Represents the camera inside of the alpha warhead room.
        /// </summary>
        HczWarheadRoom,

        /// <summary>
        /// Represents the camera above the alpha warhead switch.
        /// </summary>
        HczWarheadSwitch,

        /// <summary>
        /// Represents the camera facing toward the entrance zone checkpoint (in HCZ).
        /// </summary>
        EzEntrance,

        /// <summary>
        /// Represents a camera in a HCZ T-Intersection.
        /// </summary>
        HczTIntersection,

        /// <summary>
        /// Represents a camera in a HCZ X-Intersection.
        /// </summary>
        HczXIntersection,

        /// <summary>
        /// Represents the camera facing the LCZ/HCZ A checkpoint, on the Heavy Containment side.
        /// </summary>
        LczAEntrance,

        /// <summary>
        /// Represents the camera facing the LCZ/HCZ B checkpoint, on the Heavy Containment side.
        /// </summary>
        LczBEntrance,
        #endregion

        #region Ez

        /// <summary>
        /// Represents a camera in an EZ corner.
        /// </summary>
        EzCorner,

        /// <summary>
        /// Represents the camera facing toward the heavy containment zone checkpoint (in EZ).
        /// </summary>
        HczEntrance,

        /// <summary>
        /// Represents the camera inside of Gate A (entrance zone).
        /// </summary>
        EzGateA,

        /// <summary>
        /// Represents the camera inside of Gate B (entrance zone).
        /// </summary>
        EzGateB,

        /// <summary>
        /// Represents a camera in an EZ hallway.
        /// </summary>
        EzHall,

        /// <summary>
        /// Represents the camera outside the INTERCOM door.
        /// </summary>
        EzIntercomHall,

        /// <summary>
        /// Represents the camera facing the intercom.
        /// </summary>
        EzIntercomInterior,

        /// <summary>
        /// Represents the camera inside the INTERCOM door.
        /// </summary>
        EzIntercomStairs,

        /// <summary>
        /// Represents a camera inside of an EZ office.
        /// </summary>
        EzOffice,

        /// <summary>
        /// Represents a camera in an EZ T-Intersection.
        /// </summary>
        EzTIntersection,

        /// <summary>
        /// Represents a camera in an EZ X-Intersection.
        /// </summary>
        EzXIntersection,

        #endregion

        #region Surface

        /// <summary>
        /// Represents the camera facing the NUKE_SURFACE door.
        /// </summary>
        Backstreet,

        /// <summary>
        /// Represents the camera above the Gate A balcony.
        /// </summary>
        Bridge,

        /// <summary>
        /// Represents the ESCAPE ZONE camera (facing toward the ESCAPE door).
        /// </summary>
        EscapeZone,

        /// <summary>
        /// Represents the EXIT camera (above the Class-D and Scientist extraction point).
        /// </summary>
        Exit,

        /// <summary>
        /// Represents the HELIPAD camera.
        /// </summary>
        Helipad,

        /// <summary>
        /// Represents the camera on the Gate B walkway.
        /// </summary>
        Streetcam,

        /// <summary>
        /// Represents the camera facing the SURFACE_GATE door (Gate B side)
        /// </summary>
        SurfaceGate,

        /// <summary>
        /// Represents the camera outside of the Gate A elevator (surface).
        /// </summary>
        SurfaceGateA,

        /// <summary>
        /// Represents the camera on the tower at Gate A.
        /// </summary>
        Tower,

        #endregion

        #region Unspecified

        /// <summary>
        /// A corner, zone unknown (either LCZ, HCZ, or EZ).
        /// </summary>
        Corner,

        /// <summary>
        /// An unspecified camera related to Gate A - Either the camera in EZ, or the closest camera to the elevator on the surface.
        /// </summary>
        GateA,

        /// <summary>
        /// An unspecified camera related to Gate B - Either the camera in EZ, or the camera near the surface gate on the surface.
        /// </summary>
        GateB,

        /// <summary>
        /// A hallway, zone unknown (either LCZ or HCZ).
        /// </summary>
        Hallway,

        /// <summary>
        /// An office, zone unknown (either LCZ or EZ).
        /// </summary>
        Office,

        /// <summary>
        /// A T-insection, zone unknown (either LCZ or HCZ).
        /// </summary>
        TIntersection,

        /// <summary>
        /// An X-intersection, zone unknown (either LCZ or HCZ).
        /// </summary>
        XIntersection,

        #endregion
    }
}
