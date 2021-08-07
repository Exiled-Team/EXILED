// -----------------------------------------------------------------------
// <copyright file="DecalTypes.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using System;

    using UnityEngine;

    using Random = UnityEngine.Random;

    /// <summary>
    /// Unique identifier for types of weapon hits.
    /// </summary>
    public enum DecalType
    {
        /// <summary>
        /// Represents hitting metal.
        /// </summary>
        Metal,

        /// <summary>
        /// Represents hitting sand.
        /// </summary>
        Sand,

        /// <summary>
        /// Represents hitting stone.
        /// </summary>
        Stone,

        /// <summary>
        /// Represents a hit that water will leak from.
        /// </summary>
        WaterLeak,

        /// <summary>
        /// Represents a hit that leaks water from an extinguisher.
        /// </summary>
        WaterLeekExtinguisher,

        /// <summary>
        /// Represents hitting wood.
        /// </summary>
        Wood,

        /// <summary>
        /// Represents hitting flesh.
        /// </summary>
        Flesh,

        /// <summary>
        /// Represents an unknown hit.
        /// </summary>
        Unknown,
    }

    /// <summary>
    /// A class to help convert Decal objects to <see cref="DecalType"/> and back.
    /// </summary>
    public static class DecalTypes
    {
        /// <summary>
        /// Converts a <see cref="GameObject"/> to a <see cref="DecalType"/>.
        /// </summary>
        /// <param name="obj">The <see cref="GameObject"/> to convert.</param>
        /// <param name="shoot">The <see cref="GunShoot"/> the object originates from.</param>
        /// <returns>The <see cref="DecalType"/> of the object.</returns>
        public static DecalType GetDecalType(GameObject obj, GunShoot shoot) =>
            obj == shoot.metalHitEffect ? DecalType.Metal :
            obj == shoot.sandHitEffect ? DecalType.Sand :
            obj == shoot.stoneHitEffect ? DecalType.Stone :
            obj == shoot.waterLeakEffect ? DecalType.WaterLeak :
            obj == shoot.waterLeakExtinguishEffect ? DecalType.WaterLeekExtinguisher :
            obj == shoot.woodHitEffect ? DecalType.Wood :
            shoot.fleshHitEffects.Contains(obj) ? DecalType.Flesh : DecalType.Unknown;

        /// <summary>
        /// Converts a <see cref="DecalType"/> to a game object.
        /// </summary>
        /// <param name="type">The <see cref="DecalType"/> to convert.</param>
        /// <param name="shoot">The <see cref="GunShoot"/> used for conversion.</param>
        /// <returns>The Game Object that represents a decal type.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If you give it a decal type that the game doesn't support.</exception>
        public static GameObject GetDecalObject(DecalType type, GunShoot shoot)
        {
            switch (type)
            {
                case DecalType.Metal:
                    return shoot.metalHitEffect;
                case DecalType.Sand:
                    return shoot.sandHitEffect;
                case DecalType.Stone:
                    return shoot.stoneHitEffect;
                case DecalType.WaterLeak:
                    return shoot.waterLeakEffect;
                case DecalType.Wood:
                    return shoot.woodHitEffect;
                case DecalType.Flesh:
                    return shoot.fleshHitEffects[Random.Range(0, shoot.fleshHitEffects.Length - 1)];
                case DecalType.WaterLeekExtinguisher:
                    return shoot.waterLeakExtinguishEffect;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
