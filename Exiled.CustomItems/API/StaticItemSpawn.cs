// -----------------------------------------------------------------------
// <copyright file="StaticItemSpawn.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API
{
    /// <summary>
    /// Handles static spawn locations.
    /// </summary>
    public sealed class StaticItemSpawn : CustomItemSpawn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticItemSpawn"/> class.
        /// </summary>
        /// <param name="position">The <see cref="Vector"/> for the item.</param>
        /// <param name="chance">The spawn chance for this location.</param>
        /// <param name="name">The name of this loctaion.</param>
        public StaticItemSpawn(Vector position, float chance, string name)
        {
            Position = position;
            Chance = chance;
            Name = name;
        }

        /// <inheritdoc />
        public override Vector Position { get; set; }

        /// <inheritdoc />
        public override float Chance { get; set; }

        /// <inheritdoc />
        public override string Name { get; set; }
    }
}
