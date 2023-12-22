// -----------------------------------------------------------------------
// <copyright file="Scp559.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Interfaces;
    using MapGeneration;
    using UnityEngine;

    /// <summary>
    /// Represents a cake.
    /// </summary>
    public class Scp559 : IWrapper<Scp559Cake>
    {
        /// <summary>
        /// <see cref="Dictionary{TKey,TValue}"/> <see cref="Scp559Cake"/> to <see cref="Scp559"/>.
        /// </summary>
        internal static readonly Dictionary<Scp559Cake, Scp559> CakeToWrapper = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp559"/> class.
        /// </summary>
        /// <param name="cakeBase">The <see cref="Scp559Cake"/> instance.</param>
        public Scp559(Scp559Cake cakeBase)
        {
            Base = cakeBase;

            CakeToWrapper.Add(cakeBase, this);
        }

        /// <summary>
        /// Gets the list with all <see cref="Scp559"/> instances.
        /// </summary>
        public static IReadOnlyCollection<Scp559> List => CakeToWrapper.Values;

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey,TValue}"/> with rooms and amount of people in them.
        /// </summary>
        public static Dictionary<Room, int> PopulatedRooms => Scp559Cake.PopulatedRooms.ToDictionary(x => Room.Get(x.Key), x => x.Value);

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey,TValue}"/> with spawnpoint in rooms.
        /// </summary>
        public static Dictionary<RoomName, Vector4> SpawnPositions => Scp559Cake.Spawnpoints;

        /// <summary>
        /// Gets the list of all available spawnpoints.
        /// </summary>
        public static List<Vector4> AvailableSpawnpoints => Scp559Cake.PossiblePositions;

        /// <summary>
        /// Gets or sets offset for spawning near pedestals.
        /// </summary>
        public static Vector3 PedestalOffset
        {
            get => Scp559Cake.PedestalOffset;
            set => Scp559Cake.PedestalOffset.Set(value.x, value.y, value.z);
        }

        /// <inheritdoc/>
        public Scp559Cake Base { get; }

        /// <summary>
        /// Gets or sets how many slices are still on cake.
        /// </summary>
        public byte RemainingSlices
        {
            get => Base._remainingSlices;
            set => Base.Network_remainingSlices = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not cake is spawned.
        /// </summary>
        public bool IsSpawned
        {
            get => Base._isSpawned;
            set => Base.Network_isSpawned = value;
        }

        /// <summary>
        /// Gets or sets the time how much cake will still be usable.
        /// </summary>
        public float RemainingTime
        {
            get => Base._remainingTime;
            set => Base._remainingTime = value;
        }

        /// <summary>
        /// Gets or sets the minimum required time for cake to spawn.
        /// </summary>
        public float RespawnTime
        {
            get => Base._respawnTime;
            set => Base._respawnTime = value;
        }

        /// <summary>
        /// Gets or sets current position of cake.
        /// </summary>
        public Vector3 Position
        {
            get => Base._position;
            set => Base.Network_position = value;
        }

        /// <summary>
        /// Gets the <see cref="Scp559"/> by it's game instance.
        /// </summary>
        /// <param name="cake">Game instance.</param>
        /// <returns><see cref="Scp559"/>.</returns>
        public static Scp559 Get(Scp559Cake cake) => CakeToWrapper.TryGetValue(cake, out Scp559 scp559) ? scp559 : new Scp559(cake);

        /// <summary>
        /// Gets the <see cref="IEnumerable{T}"/> of SCP-559 which matches the predicate.
        /// </summary>
        /// <param name="predicate">Predicate to match.</param>
        /// <returns><see cref="IEnumerable{T}"/> of SCP-559.</returns>
        public static IEnumerable<Scp559> Get(Func<Scp559, bool> predicate) => List.Where(predicate);

        /// <summary>
        /// Tries to get available spawn point for SCP-559.
        /// </summary>
        /// <param name="pos">Position of spawn.</param>
        /// <param name="pedestal">Will be pedestal also spawned.</param>
        /// <returns><see langword="true"/> if position was found. Otherwise, <see langword="false"/>.</returns>
        public bool TryGetSpawnpoint(out Vector3 pos, out bool pedestal) => Base.TryGetSpawnPoint(out pos, out pedestal);
    }
}