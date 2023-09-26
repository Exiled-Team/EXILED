// -----------------------------------------------------------------------
// <copyright file="CameraType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1124 // Do not use regions
#pragma warning disable CS1591 // XML Comment Missing
#pragma warning disable SA1602 // Enumeration items should be documented

namespace Exiled.API.Enums
{
    using Features;

    /// <summary>
    /// Unique identifier for the different types of SCP-079 cameras.
    /// </summary>
    /// <seealso cref="Camera.Type"/>
    /// <seealso cref="Camera.Get(CameraType)"/>
    /// <seealso cref="Camera.TryGet(CameraType, out Camera)"/>
    public enum CameraType
    {
        /// <summary>
        /// Represents an unknown camera.
        /// </summary>
        Unknown,

        #region Ez
        EzChkptHall,
        EzCrossing,
        EzCurve,
        EzHallway,
        EzThreeWay,
        EzGateA,
        EzGateB,
        EzIntercomBottom,
        EzIntercomHall,
        EzIntercomPanel,
        EzIntercomStairs,
        EzLargeOffice,
        EzLoadingDock,
        EzMinorOffice,
        EzTwoStoryOffice,
        #endregion

        #region Hcz
        Hcz049ContChamber,
        Hcz049ElevTop,
        Hcz049Hallway,
        Hcz049TopFloor,
        Hcz049Outside,
        Hcz079Airlock,
        Hcz079ContChamber,
        Hcz079Hallway,
        Hcz079KillSwitch,
        Hcz096ContChamber,
        Hcz106Bridge,
        Hcz106Catwalk,
        Hcz106Recontainment,
        HczChkptEz,
        HczChkptHcz,
        HczHIDChamber,
        HczHIDHallway,
        Hcz939,
        HczArmory,
        HczArmoryInterior,
        HczCrossing,
        HczElevSysA,
        HczElevSysB,
        HczHallway,
        HczThreeWay,
        HczServersBottom,
        HczServersStairs,
        HczServersTop,
        HczTeslaGate,
        HczTestroomBridge,
        HczTestroomMain,
        HczTestroomOffice,
        HczWarheadArmory,
        HczWarheadControl,
        HczWarheadHallway,
        HczWarheadTop,
        #endregion

        #region Lcz
        Lcz173Bottom,
        Lcz173ContChamber,
        Lcz173Hall,
        Lcz173Stairs,
        Lcz914Airlock,
        Lcz914ContChamber,
        LczAirlock,
        LczArmory,
        LczCellblockBack,
        LczCellblockEntry,
        LczChkptAEntry,
        LczChkptAInner,
        LczChkptBEntry,
        LczChkptBInner,
        LczGlassroom,
        LczGlassroomEntry,
        LczGreenhouse,
        LczCrossing,
        LczCurve,
        LczElevSysA,
        LczElevSysB,
        LczHallway,
        LczThreeWay,
        LczPcOffice,
        LczRestrooms,
        LczTcHallway,
        LczTestChamber,
        #endregion

        #region Surface
        ExitPassage,
        GateASurface,
        GateBSurface,
        MainStreet,
        SurfaceAirlock,
        SurfaceBridge,
        TunnelEntrance,
        #endregion

        #region new
        Hcz173Outside,
        Hcz173Stairs,
        Hcz173ContChamber,
        Hcz173Hallway,
        HczCurve,
        #endregion
    }
}
