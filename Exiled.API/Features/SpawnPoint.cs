// -----------------------------------------------------------------------
// <copyright file="SpawnPoint.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;

    using UnityEngine;

    /// <summary>
    /// A wrapper class for a spawn location.
    /// </summary>
    public class SpawnPoint
    {
        /// <summary>
        /// A <see cref="List{T}"/> of <see cref="SpawnPoint"/>s on the map.
        /// </summary>
        private static readonly List<SpawnPoint> SpawnsValue = new(250);

        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnPoint"/> class.
        /// </summary>
        /// <param name="go">The <see cref="UnityEngine.GameObject"/> of the spawn.</param>
        /// <param name="role">The <see cref="global::RoleType"/> of the spawn.</param>
        internal SpawnPoint(GameObject go, RoleType role)
        {
            RoleType = role;
            GameObject = go;
            Room = Map.FindParentRoom(GameObject);
        }

        /// <summary>
        /// Gets a <see cref="IReadOnlyList{T}"/> of <see cref="SpawnPoint"/> which contains all the <see cref="SpawnPoint"/> instances.
        /// </summary>
        public static IReadOnlyList<SpawnPoint> List => SpawnsValue.AsReadOnly();

        /// <summary>
        /// Gets the <see cref="global::RoleType"/> related to this spawn.
        /// </summary>
        public RoleType RoleType { get; }

        /// <summary>
        /// Gets the spawn's <see cref="Features.Room"/>.
        /// </summary>
        public Room Room { get; }

        /// <summary>
        /// Gets the spawn's <see cref="ZoneType">zone</see>.
        /// </summary>
        public ZoneType Zone => Room?.Zone ?? ZoneType.Unspecified;

        /// <summary>
        /// Gets the spawn's <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        public GameObject GameObject { get; }

        /// <summary>
        /// Gets the spawn's <see cref="Vector3">position</see>.
        /// </summary>
        public Vector3 Position => GameObject.transform.position;

        /// <summary>
        /// Gets the spawn's <see cref="Quaternion">rotation</see>.
        /// </summary>
        public Quaternion Rotation => GameObject.transform.rotation;

        /// <summary>
        /// Gets the spawn's Y-facing rotation. This value indicates the direction that the spawn is facing.
        /// </summary>
        public float YRotation => Rotation.eulerAngles.y;

        /// <summary>
        /// Gets the spawn's <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public Transform Transform => GameObject.transform;

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> spawn points belonging to the provided <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The <see cref="global::RoleType"/> to filter.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="SpawnPoint"/>.</returns>
        public static IEnumerable<SpawnPoint> Get(RoleType role)
            => SpawnsValue.Where(spawn => spawn.RoleType == role);

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> spawn points belonging to the provided <paramref name="team"/>.
        /// </summary>
        /// <param name="team">The <see cref="global::Team"/> to filter.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="SpawnPoint"/>.</returns>
        public static IEnumerable<SpawnPoint> Get(Team team)
            => SpawnsValue.Where(spawn => spawn.RoleType.GetTeam() == team);

        /// <summary>
        /// Gets a random spawn point belonging to the provided <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The <see cref="global::RoleType"/> to filter.</param>
        /// <returns>A random <see cref="SpawnPoint"/>.</returns>
        public static SpawnPoint Random(RoleType role)
        {
            List<SpawnPoint> spawns = SpawnsValue.Where(spawn => spawn.RoleType == role).ToList();
            if (spawns.Count() > 0)
                return spawns[UnityEngine.Random.Range(0, spawns.Count())];
            return null;
        }

        /// <summary>
        /// Gets a random spawn point belonging to the provided <paramref name="team"/>.
        /// </summary>
        /// <param name="team">The <see cref="global::Team"/> to filter.</param>
        /// <returns>A random <see cref="SpawnPoint"/>.</returns>
        public static SpawnPoint Random(Team team)
        {
            List<SpawnPoint> spawns = SpawnsValue.Where(spawn => spawn.RoleType.GetTeam() == team).ToList();
            if (spawns.Count() > 0)
                return spawns[UnityEngine.Random.Range(0, spawns.Count())];
            return null;
        }

        /// <summary>
        /// Refreshes the spawnpoint list.
        /// </summary>
        internal static void Refresh()
        {
            SpawnsValue.Clear();
            foreach (var spawnData in SpawnpointManager.Positions)
            {
                foreach (GameObject spawn in spawnData.Value)
                {
                    SpawnsValue.Add(new SpawnPoint(spawn, spawnData.Key));
                }
            }
        }
    }
}
