// -----------------------------------------------------------------------
// <copyright file="DynamicSpawnPoint.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.Spawn
{
    using Exiled.CustomItems.API.Features;

    using YamlDotNet.Serialization;

    /// <summary>
    /// Handles dynamic spawn locations.
    /// </summary>
    public class DynamicSpawnPoint : SpawnPoint
    {
        private SpawnLocation location;

        /// <summary>
        /// Gets or sets the <see cref="SpawnLocation"/> for this item.
        /// </summary>
        public SpawnLocation Location
        {
            get => location;
            set
            {
                location = value;

                Position = value.GetPosition().ToVector();

                Name = value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets this location's spawn chance.
        /// </summary>
        public override float Chance { get; set; }

        /// <summary>
        /// Gets or sets this location's name.
        /// </summary>
        [YamlIgnore]
        public override string Name { get; set; }

        /// <summary>
        /// Gets or sets this location's name.
        /// </summary>
        [YamlIgnore]
        public override Vector Position { get; set; }
    }
}
