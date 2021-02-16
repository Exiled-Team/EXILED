// -----------------------------------------------------------------------
// <copyright file="StaticItemSpawn.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API
{
    using UnityEngine;

    /// <summary>
    /// Handles static spawn locations.
    /// </summary>
    public class StaticItemSpawn : CustomItemSpawn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticItemSpawn"/> class.
        /// </summary>
        /// <param name="position">The <see cref="Vector"/> for the item.</param>
        /// <param name="chance">The spawn chance for this location.</param>
        /// <param name="name">The name of this loctaion.</param>
        public StaticItemSpawn(Vector position, float chance, string name)
            : base(chance, name) => Position = position;

        /// <inheritdoc />
        public override Vector Position { get; }
    }
}
