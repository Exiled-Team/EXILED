// -----------------------------------------------------------------------
// <copyright file="Scp956.cs" company="Exiled Team">
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
    /// A wrapper for <see cref="Scp956Pinata"/>.
    /// </summary>
    public static class Scp956
    {
        /// <summary>
        /// Gets the <see cref="Scp956Pinata"/> instance.
        /// </summary>
        public static Scp956Pinata Singleton => Scp956Pinata._instance;

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-956 is spawned.
        /// </summary>
        public static bool IsSpawned
        {
            get => Singleton._spawned;
            set => Singleton.Network_spawned = value;
        }

        /// <summary>
        /// Gets or sets current position.
        /// </summary>
        public static Vector3 Position
        {
            get => Singleton._syncPos;
            set => Singleton.Network_syncPos = value;
        }

        /// <summary>
        /// Gets or sets initial position.
        /// </summary>
        public static Vector3 InitPos
        {
            get => Singleton._initialPos;
            set => Singleton._initialPos = value;
        }

        /// <summary>
        /// Gets or sets current rotation.
        /// </summary>
        public static float Rotation
        {
            get => Singleton._syncRot;
            set => Singleton.Network_syncRot = value;
        }

        /// <summary>
        /// Gets or sets initial rotation.
        /// </summary>
        public static float InitRotation
        {
            get => Singleton._initialRot;
            set => Singleton._initialRot = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-956 is flying.
        /// </summary>
        public static bool IsFlying
        {
            get => Singleton._flying;
            set => Singleton.Network_flying = value;
        }

        /// <summary>
        /// Gets or sets current target of an SCP.
        /// </summary>
        public static Scp956Target CurrentTarget
        {
            get => Singleton._foundTarget;
            set => Singleton._foundTarget = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-956 should look like a capybara.
        /// </summary>
        public static bool IsCapybara { get; set; } = false;

        /// <summary>
        /// Gets or sets zones where SCP-956 can spawn.
        /// </summary>
        public static IEnumerable<ZoneType> AvailableZones
        {
            get => Singleton._spawnableZones.Select(x => x.GetZone());
            set => Singleton._spawnableZones = value.Select(x => x.GetZone()).ToArray();
        }

        /// <summary>
        /// Spawns behind the specified target.
        /// </summary>
        /// <param name="target">Player to spawn. If <paramref name="target"/> is <see langword="null"/>, will be chosen random.</param>
        public static void SpawnBehindTarget(Player target = null) => Singleton.SpawnBehindTarget((target ?? Player.List.GetRandomValue()).ReferenceHub);
    }
}