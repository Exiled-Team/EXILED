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
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Player"/> in the <see cref="Room"/>.
        /// </summary>
        public IEnumerable<Player> Players => Player.List.Where(player => player.CurrentRoom.Transform == Transform);

        private ZoneType FindZone()
        {
            if (Transform.parent == null)
            {
                return ZoneType.Unspecified;
            }
            else
            {
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
}
