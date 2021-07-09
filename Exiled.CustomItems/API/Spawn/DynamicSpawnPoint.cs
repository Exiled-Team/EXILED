// -----------------------------------------------------------------------
// <copyright file="DynamicSpawnPoint.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.Spawn
{
    using System;

    using Exiled.CustomItems.API.Features;

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
#pragma warning disable CS0618 // Type or member is obsolete
        public override Vector Position
        {
            get => Location.GetPosition().ToVector();
            set => throw new InvalidOperationException("You cannot change the spawn vector of a dynamic spawn location.");
        }
    }
}
