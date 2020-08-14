// -----------------------------------------------------------------------
// <copyright file="Room.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;

    using UnityEngine;

    /// <summary>
    /// The in-game room.
    /// </summary>
    public class Room
    {
        private static readonly Dictionary<string, RoomType> RoomTypes = new Dictionary<string, RoomType>
            {
            { "LCZ_Armory", RoomType.LczArmory },
            { "LCZ_Curve", RoomType.LczCurve },
            { "LCZ_Straight", RoomType.LczStraight },
            { "LCZ_012", RoomType.Lcz012 },
            { "LCZ_914", RoomType.Lcz914 },
            { "LCZ_Crossing", RoomType.LczCrossing },
            { "LCZ_TCross", RoomType.LczTCross },
            { "LCZ_Cafe", RoomType.LczCafe },
            { "LCZ_Plants", RoomType.LczPlants },
            { "LCZ_Toilets", RoomType.LczToilets },
            { "LCZ_Airlock", RoomType.LczAirlock },
            { "LCZ_173", RoomType.Lcz173 },
            { "LCZ_ClassDSpawn", RoomType.LczClassDSpawn },
            { "LCZ_ChkpB", RoomType.LczChkpB },
            { "LCZ_372", RoomType.LczGlassBox },
            { "LCZ_ChkpA", RoomType.LczChkpA },
            { "HCZ_079", RoomType.Hcz079 },
            { "HCZ_EZ_Checkpoint", RoomType.HczEzCheckpoint },
            { "HCZ_Room3ar", RoomType.HczArmory },
            { "HCZ_Testroom", RoomType.Hcz939 },
            { "HCZ_Hid", RoomType.HczHid },
            { "HCZ_049", RoomType.Hcz049 },
            { "HCZ_ChkpA", RoomType.HczChkpA },
            { "HCZ_Crossing", RoomType.HczCrossing },
            { "HCZ_106", RoomType.Hcz106 },
            { "HCZ_Nuke", RoomType.HczNuke },
            { "HCZ_Tesla", RoomType.HczTesla },
            { "HCZ_Servers", RoomType.HczServers },
            { "HCZ_ChkpB", RoomType.HczChkpB },
            { "HCZ_Room3", RoomType.HczTCross },
            { "HCZ_457", RoomType.Hcz096 },
            { "HCZ_Curve", RoomType.HczCurve },
            { "EZ_Endoof", RoomType.EzVent },
            { "EZ_Intercom", RoomType.EzIntercom },
            { "EZ_GateA", RoomType.EzGateA },
            { "EZ_PCs_small", RoomType.EzDownstairsPcs },
            { "EZ_Curve", RoomType.EzCurve },
            { "EZ_PCs", RoomType.EzPcs },
            { "EZ_Crossing", RoomType.EzCrossing },
            { "EZ_CollapsedTunnel", RoomType.EzCollapsedTunnel },
            { "EZ_Smallrooms2", RoomType.EzConference },
            { "EZ_Straight", RoomType.EzStraight },
            { "EZ_Cafeteria", RoomType.EzCafeteria },
            { "EZ_upstairs", RoomType.EzUpstairsPcs },
            { "EZ_GateB", RoomType.EzGateB },
            { "EZ_Shelter", RoomType.EzShelter },
            { "Root_*&*Outside Cams", RoomType.Surface },
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="Room"/> class.
        /// </summary>
        /// <param name="name">The room name.</param>
        /// <param name="transform">The room transform.</param>
        /// <param name="position">The room position.</param>
        public Room(string name, Transform transform, Vector3 position)
        {
            Name = name;
            Transform = transform;
            Position = position;
            Zone = FindZone();
            RoomType = GetRoomType(name);
        }

        /// <summary>
        /// Gets the <see cref="Room"/> name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the <see cref="Room"/> <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public Transform Transform { get; }

        /// <summary>
        /// Gets the <see cref="Room"/> position.
        /// </summary>
        public Vector3 Position { get; }

        /// <summary>
        /// Gets the <see cref="ZoneType"/> in which the room is located.
        /// </summary>
        public ZoneType Zone { get; }

        /// <summary>
        /// Gets the <see cref="RoomType"/>.
        /// </summary>
        public RoomType RoomType { get; }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Player"/> in the <see cref="Room"/>.
        /// </summary>
        public IEnumerable<Player> Players => Player.List.Where(player => player.CurrentRoom.Transform == Transform);

        private static RoomType GetRoomType(string rawName)
        {
            // Try to remove brackets if they exist.
            var bracketStart = rawName.IndexOf('(') - 1;
            if (bracketStart > 0)
                rawName = rawName.Remove(bracketStart, rawName.Length - bracketStart);

            return RoomTypes.TryGetValue(rawName, out var roomType) ? roomType : RoomType.Unknown;
        }

        private ZoneType FindZone()
        {
            if (Transform.parent == null)
            {
                return ZoneType.Unspecified;
            }

            switch (Transform.parent.name)
            {
                case "HeavyRooms":
                    return ZoneType.HeavyContainment;
                case "LightRooms":
                    return ZoneType.LightContainment;
                case "EntranceRooms":
                    return ZoneType.Entrance;
                default:
                    return Position.y > 900 ? ZoneType.Surface : ZoneType.Unspecified;
            }
        }
    }
}
