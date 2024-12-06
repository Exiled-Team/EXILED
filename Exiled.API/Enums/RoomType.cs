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
        /// Light Containment Zone's Armory.
        /// </summary>
        LczArmory,

        /// <summary>
        /// Light Containment Zone's Curved Hall.
        /// </summary>
        LczCurve,

        /// <summary>
        /// Light Containment Zone's Straight Hall.
        /// </summary>
        LczStraight,

        /// <summary>
        /// Light Containment Zone's SCP-914 room.
        /// </summary>
        Lcz914,

        /// <summary>
        /// Light Containment Zone's 4-Way intersection.
        /// </summary>
        LczCrossing,

        /// <summary>
        /// Light Containment Zone's 3-Way intersection.
        /// </summary>
        LczTCross,

        /// <summary>
        /// Light Containment Zone's PC-15's room.
        /// </summary>
        LczCafe,

        /// <summary>
        /// Light Containment Zone's VT-00's room.
        /// </summary>
        LczPlants,

        /// <summary>
        /// Light Containment Zone's Water Closet.
        /// </summary>
        LczToilets,

        /// <summary>
        /// Light Containment Zone's Airlock room.
        /// </summary>
        LczAirlock,

        /// <summary>
        /// Light Containment Zone's PT-00 room.
        /// </summary>
        Lcz173,

        /// <summary>
        /// Light Containment Zone's Class-D spawn room.
        /// </summary>
        LczClassDSpawn,

        /// <summary>
        /// Light Containment Zone's Checkpoint B room.
        /// </summary>
        LczCheckpointB,

        /// <summary>
        /// Light Containment Zone's GR-18's room.
        /// </summary>
        LczGlassBox,

        /// <summary>
        /// Light Containment Zone's Checkpoint A room.
        /// </summary>
        LczCheckpointA,

        /// <summary>
        /// Heavy Containment Zone's SCP-079 room.
        /// </summary>
        Hcz079,

        /// <summary>
        /// Heavy Containment Zone's Entrance Checkpoint A room.
        /// </summary>
        HczEzCheckpointA,

        /// <summary>
        /// Heavy Containment Zone's Entrance Checkpoint B room.
        /// </summary>
        HczEzCheckpointB,

        /// <summary>
        /// Heavy Containment Zone's 3-Way Intersection + Armory room.
        /// </summary>
        HczArmory,

        /// <summary>
        /// Heavy Containment Zone's SCP-939 room.
        /// </summary>
        Hcz939,

        /// <summary>
        /// Heavy Containment Zone's MicroHID straight hall.
        /// </summary>
        HczHid,

        /// <summary>
        /// Heavy Containment Zone's SCP-049 + SCP-173's room.
        /// </summary>
        Hcz049,

        /// <summary>
        /// Heavy Containment Zone's 4-way intersection.
        /// </summary>
        HczCrossing,

        /// <summary>
        /// Heavy Containment Zone's SCP-106 room.
        /// </summary>
        Hcz106,

        /// <summary>
        /// Heavy Containment Zone's nuke room.
        /// </summary>
        HczNuke,

        /// <summary>
        /// Heavy Containment Zone's Tesla straight hall.
        /// </summary>
        HczTesla,

        /// <summary>
        /// Heavy Containment Zone's Servers room.
        /// </summary>
        HczServers,

        /// <summary>
        /// Heavy Containment Zone's 3-way intersection.
        /// </summary>
        HczTCross,

        /// <summary>
        /// Heavy Containment Zone's cruved hall.
        /// </summary>
        HczCurve,

        /// <summary>
        /// Heavy Containment Zone's SCP-096 room.
        /// </summary>
        Hcz096,

        /// <summary>
        /// Entrance Zone's Red Vent room.
        /// </summary>
        EzVent,

        /// <summary>
        /// Entrance Zone's Intercom room.
        /// </summary>
        EzIntercom,

        /// <summary>
        /// Entrance Zone's Gate A room.
        /// </summary>
        EzGateA,

        /// <summary>
        /// Entrance Zone's straight hall with PC's on a lower level.
        /// </summary>
        EzDownstairsPcs,

        /// <summary>
        /// Entrance Zone's curved hall.
        /// </summary>
        EzCurve,

        /// <summary>
        /// Entrance Zone's straight hall with PC's on the main level.
        /// </summary>
        EzPcs,

        /// <summary>
        /// Entrance Zone's 4-way intersection.
        /// </summary>
        EzCrossing,

        /// <summary>
        /// Entrance Zone's Red Collapsed Tunnel Room.
        /// </summary>
        EzCollapsedTunnel,

        /// <summary>
        /// Entrance Zone's straight hall with Dr.L's locked room.
        /// </summary>
        EzConference,

        /// <summary>
        /// Entrance Zone's straight hall
        /// </summary>
        EzStraight,

        /// <summary>
        /// Entrance Zone's Cafeteria Room.
        /// </summary>
        EzCafeteria,

        /// <summary>
        /// Entrance Zone's straight hall with PC's and upper level.
        /// </summary>
        EzUpstairsPcs,

        /// <summary>
        /// Entrance Zone's Gate B room.
        /// </summary>
        EzGateB,

        /// <summary>
        /// Entrance Zone's Shelter room.
        /// </summary>
        EzShelter,

        /// <summary>
        /// Entrance Zone's straight hall with Chef's locked room.
        /// </summary>
        EzChef,

        /// <summary>
        /// The Pocket Dimension.
        /// </summary>
        Pocket,

        /// <summary>
        /// The Surface.
        /// </summary>
        Surface,

        /// <summary>
        /// Heavy Containment Zone's straight hall.
        /// </summary>
        HczStraight,

        /// <summary>
        /// Entrance Zone's 3-way intersection.
        /// </summary>
        EzTCross,

        /// <summary>
        /// Light Containment ZOne's SCP-330 room.
        /// </summary>
        Lcz330,

        /// <summary>
        /// Entrance Zone's straight hall before the entrance/heavy checkpoint (Checkpoint Hallway A).
        /// </summary>
        EzCheckpointHallwayA,

        /// <summary>
        /// Entrance Zone's straight hall before the entrance/heavy checkpoint (Checkpoint Hallway B).
        /// </summary>
        EzCheckpointHallwayB,

        /// <summary>
        /// Heavy Containment Zone's test room's straight hall.
        /// </summary>
        HczTestRoom,

        /// <summary>
        /// Heavy Containment Zone's Elevator System A room.
        /// </summary>
        HczElevatorA,

        /// <summary>
        /// Heavy Containment Elevator Zone's System B room.
        /// </summary>
        HczElevatorB,

        /// <summary>
        /// Waterfall Room.
        /// </summary>
        HczCrossRoomWater,

        /// <summary>
        /// Heavy Containment Corner Room.
        /// </summary>
        HczCornerDeep,

        /// <summary>
        /// Heavy Containment Junk Intersection.
        /// </summary>
        HczIntersectionJunk,

        /// <summary>
        /// Intersection in Heavy Containment.
        /// </summary>
        HczIntersection,

        /// <summary>
        /// Straight Hallway variant in Heavy Containment.
        /// </summary>
        HczStraightC,

        /// <summary>
        /// Straight Hallway with Pipes in Heavy Containment.
        /// </summary>
        HczStraightPipeRoom,

        /// <summary>
        /// Straight Hallway variant in Heavy Containment.
        /// </summary>
        HczStraightVariant,
    }
}