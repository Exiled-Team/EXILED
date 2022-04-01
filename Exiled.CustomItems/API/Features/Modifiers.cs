// -----------------------------------------------------------------------
// <copyright file="Modifiers.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.Features {
    /// <summary>
    /// Weapon modifiers.
    /// </summary>
    public struct Modifiers {
        /// <summary>
        /// Initializes a new instance of the <see cref="Modifiers"/> struct.
        /// </summary>
        /// <param name="barrelType"><inheritdoc cref="BarrelType"/></param>
        /// <param name="sightType"><inheritdoc cref="SightType"/></param>
        /// <param name="otherType"><inheritdoc cref="OtherType"/></param>
        public Modifiers(int barrelType, int sightType, int otherType) {
            BarrelType = barrelType;
            SightType = sightType;
            OtherType = otherType;
        }

        /// <summary>
        /// Gets a value indicating what <see cref="BarrelType"/> the weapon will have.
        /// </summary>
        public int BarrelType { get; private set; }

        /// <summary>
        /// Gets a value indicating what <see cref="SightType"/> the weapon will have.
        /// </summary>
        public int SightType { get; private set; }

        /// <summary>
        /// Gets a value indicating what <see cref="OtherType"/> the weapon will have.
        /// </summary>
        public int OtherType { get; private set; }
    }
}
