// -----------------------------------------------------------------------
// <copyright file="RoomType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Unique identifier for the different types of rooms.
    /// </summary>
    /// <seealso cref="Features.Room.Type"/>
    /// <seealso cref="Features.Room.Get(RoomType)"/>
    public enum RoomType
    {
        /// <summary>
        /// Unknown Room Type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Lower Containment Armory.
        /// </summary>
        LczArmory,

        /// <summary>
        /// Lower Containment L-Shaped Room.
        /// </summary>
        LczCurve,

        /// <summary>
        /// Lower Containment |-Shaped Room.
        /// </summary>
        LczStraight,

        /// <summary>
        /// Lower Containment SCP-914 Room.
        /// </summary>
        Lcz914,

        /// <summary>
        /// Lower Containment X-Shaped Room.
        /// </summary>
        LczCrossing,

        /// <summary>
        /// Lower Containment T-Shaped Room.
        /// </summary>
        LczTCross,

        /// <summary>
        /// Lower Containment Cafe Room.
        /// </summary>
        LczCafe,

        /// <summary>
        /// Lower Containment T-Shaped Plants Room.
        /// </summary>
        LczPlants,

        /// <summary>
        /// Lower Containment Toilets Room.
        /// </summary>
        LczToilets,

        /// <summary>
        /// Lower Containment Airlock Room.
        /// </summary>
        LczAirlock,

        /// <summary>
        /// Lower Containment SCP-173 Room.
        /// </summary>
        Lcz173,

        /// <summary>
        /// Lower Containment Class-D Spawn Room.
        /// </summary>
        LczClassDSpawn,

        /// <summary>
        /// Lower Containment Checkpoint B Room.
        /// </summary>
        LczCheckpointB,

        /// <summary>
        /// Lower Containment Glass Box Room.
        /// </summary>
        LczGlassBox,

        /// <summary>
        /// Lower Containment Checkpoint A Room.
        /// </summary>
        LczCheckpointA,

        /// <summary>
        /// Heavy Containment SCP-079 Room.
        /// </summary>
        Hcz079,

        /// <summary>
        /// Heavy Containment Entrance Checkpoint A Room.
        /// </summary>
        HczEzCheckpointA,

        /// <summary>
        /// Heavy Containment Entrance Checkpoint B Room.
        /// </summary>
        HczEzCheckpointB,

        /// <summary>
        /// Heavy Containment T-Shaped Armory Room.
        /// </summary>
        HczArmory,

        /// <summary>
        /// Heavy Containment SCP-939 Room.
        /// </summary>
        Hcz939,

        /// <summary>
        /// Heavy Containment HID-Spawn Room.
        /// </summary>
        HczHid,

        /// <summary>
        /// Heavy Containment SCP-049 Room.
        /// </summary>
        Hcz049,

        /// <summary>
        /// Heavy Containment X-Shaped Room.
        /// </summary>
        HczCrossing,

        /// <summary>
        /// Heavy Containment SCP-106 Room.
        /// </summary>
        Hcz106,

        /// <summary>
        /// Heavy Containment Nuke Room.
        /// </summary>
        HczNuke,

        /// <summary>
        /// Heavy Containment Tesla Room.
        /// </summary>
        HczTesla,

        /// <summary>
        /// Heavy Containment Servers Room.
        /// </summary>
        HczServers,

        /// <summary>
        /// Heavy Containment T-Shaped Room.
        /// </summary>
        HczTCross,

        /// <summary>
        /// Heavy Containment L-Shaped Room.
        /// </summary>
        HczCurve,

        /// <summary>
        /// Heavy Containment SCP-096 Room.
        /// </summary>
        Hcz096,

        /// <summary>
        /// Entrance Red Vent Room.
        /// </summary>
        EzVent,

        /// <summary>
        /// Entrance Intercom Room.
        /// </summary>
        EzIntercom,

        /// <summary>
        /// Entrance Gate A Room.
        /// </summary>
        EzGateA,

        /// <summary>
        /// Entrance PC Room With Downstairs.
        /// </summary>
        EzDownstairsPcs,

        /// <summary>
        /// Entrance L-Shaped Room.
        /// </summary>
        EzCurve,

        /// <summary>
        /// Entrance PC Room.
        /// </summary>
        EzPcs,

        /// <summary>
        /// Entrance X-Shaped Room.
        /// </summary>
        EzCrossing,

        /// <summary>
        /// Entrance Red Collapsed Tunnel Room.
        /// </summary>
        EzCollapsedTunnel,

        /// <summary>
        /// Entrance |-Shaped Dr.L Room.
        /// </summary>
        EzConference,

        /// <summary>
        /// Entrance |-Shaped Room
        /// </summary>
        EzStraight,

        /// <summary>
        /// Entrance Cafeteria Room.
        /// </summary>
        EzCafeteria,

        /// <summary>
        /// Entrance PC Room With Upstairs.
        /// </summary>
        EzUpstairsPcs,

        /// <summary>
        /// Entrance Gate B Room.
        /// </summary>
        EzGateB,

        /// <summary>
        /// Entrance Shelter Room.
        /// </summary>
        EzShelter,

        /// <summary>
        /// Pocket Dimension.
        /// </summary>
        Pocket,

        /// <summary>
        /// The Surface.
        /// </summary>
        Surface,

        /// <summary>
        /// Heavy Containment |-Shaped Room.
        /// </summary>
        HczStraight,

        /// <summary>
        /// Entrance T-Shaped Room.
        /// </summary>
        EzTCross,

        /// <summary>
        /// Lower Containment SCP-330 Room.
        /// </summary>
        Lcz330,

        /// <summary>
        /// Entrance Room before Checkpoint.
        /// </summary>
        EzCheckpointHallway,

        /// <summary>
        /// Heavy Containment Test Room.
        /// </summary>
        HczTestRoom,

        /// <summary>
        /// Heavy Containment Elevator System A Room.
        /// </summary>
        HczElevatorA,

        /// <summary>
        /// Heavy Containment Elevator System B Room.
        /// </summary>
        HczElevatorB,
    }
}