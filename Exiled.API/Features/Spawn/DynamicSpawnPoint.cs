// -----------------------------------------------------------------------
// <copyright file="DynamicSpawnPoint.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Spawn
{
    using System;

    using Exiled.API.Extensions;
    using Exiled.API.Enums;

    using UnityEngine;

    using YamlDotNet.Serialization;

    /// <summary>
    /// Handles dynamic spawn locations.
    /// </summary>
    public class DynamicSpawnPoint : SpawnPoint
    {
        /// <summary>
        /// Gets or sets the <see cref="SpawnLocation"/> for this item.
        /// </summary>
        public SpawnLocation Location { get; set; }

        /// <inheritdoc/>
        public override float Chance { get; set; }

        /// <inheritdoc/>
        [YamlIgnore]
        public override string Name
        {
            get => Location.ToString();
            set => throw new InvalidOperationException("You cannot change the name of a dynamic spawn location.");
        }

        /// <inheritdoc/>
        [YamlIgnore]
        public override Vector3 Position
        {
            get => Location.GetPosition();
            set => throw new InvalidOperationException("You cannot change the spawn vector of a dynamic spawn location.");
        }
    }
}
