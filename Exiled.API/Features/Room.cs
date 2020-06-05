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
        private ZoneType zone;

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
        }

        /// <summary>
        /// Gets or sets the <see cref="Room"/> name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the <see cref="Room"/> <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public Transform Transform { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="Room"/> position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets the <see cref="ZoneType"/> in which the room is located.
        /// </summary>
        public ZoneType Zone
        {
            get
            {
                if (zone != ZoneType.Unspecified)
                    return zone;

                if (Position.y == -1997f)
                    zone = ZoneType.Unspecified;
                else if (Position.y >= 0f && Position.y < 500f)
                    zone = ZoneType.LightContainment;
                else if (Position.y < -100 && Position.y > -1000f)
                    zone = ZoneType.HeavyContainment;
                else if (Name.Contains("ENT") || Name.Contains("INTERCOM"))
                    zone = ZoneType.Entrance;
                else if (Position.y >= 5)
                    zone = ZoneType.Surface;

                return zone;
            }
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Player"/> in the <see cref="Room"/>.
        /// </summary>
        public IEnumerable<Player> Players => Player.List.Values.Where(player => player.CurrentRoom.Name == Name);
    }
}