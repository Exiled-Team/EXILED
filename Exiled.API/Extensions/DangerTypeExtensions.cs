// -----------------------------------------------------------------------
// <copyright file="DangerTypeExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using CustomPlayerEffects.Danger;
    using Exiled.API.Enums;

    /// <summary>
    /// A set of extensions for <see cref="DangerType"/>.
    /// </summary>
    public static class DangerTypeExtensions
    {
        /// <summary>
        /// Gets a dictionary that maps each <see cref="DangerType"/> to its corresponding <see cref="System.Type"/>.
        /// </summary>
        public static ReadOnlyDictionary<DangerType, Type> DangerTypeToType { get; } = new(new Dictionary<DangerType, Type>()
        {
            { DangerType.Warhead, typeof(WarheadDanger) },
            { DangerType.CardiacArrest, typeof(CardiacArrestDanger) },
            { DangerType.RageTarget, typeof(RageTargetDanger) },
            { DangerType.Corroding, typeof(CorrodingDanger) },
            { DangerType.PlayerDamaged, typeof(PlayerDamagedDanger) },
            { DangerType.ScpEncounter, typeof(ScpEncounterDanger) },
            { DangerType.ZombieEncounter, typeof(ZombieEncounterDanger) },
            { DangerType.ArmedEnemy, typeof(ArmedEnemyDanger) },
        });

        /// <summary>
        /// Gets a dictionary that maps each <see cref="System.Type"/> to its corresponding <see cref="DangerType"/>.
        /// </summary>
        public static ReadOnlyDictionary<Type, DangerType> TypeToDangerType { get; } = new(DangerTypeToType.ToDictionary(x => x.Value, y => y.Key));

        /// <summary>
        /// Gets an instance of <see cref="System.Type"/> points to a danger.
        /// </summary>
        /// <param name="danger">The <see cref="DangerType"/> enum.</param>
        /// <returns>The <see cref="System.Type"/>.</returns>
        public static Type Type(this DangerType danger)
            => DangerTypeToType.TryGetValue(danger, out Type type) ? type : throw new InvalidOperationException("Invalid danger enum provided");

        /// <summary>
        /// Gets an instance of <see cref="System.Type"/> points to a danger.
        /// </summary>
        /// <param name="danger">The <see cref="DangerType"/> enum.</param>
        /// <param name="type">The type found with the corresponding DangerType.</param>
        /// <returns>Whether or not the type has been found.</returns>
        public static bool TryGetType(this DangerType danger, out Type type)
            => DangerTypeToType.TryGetValue(danger, out type);

        /// <summary>
        /// Gets the <see cref="DangerType"/> of the specified <see cref="DangerStackBase"/>.
        /// </summary>
        /// <param name="dangerStackBase">The <see cref="DangerStackBase"/> enum.</param>
        /// <returns>The <see cref="DangerType"/>.</returns>
        public static DangerType GetDangerType(this DangerStackBase dangerStackBase)
            => TypeToDangerType.TryGetValue(dangerStackBase.GetType(), out DangerType danger) ? danger : throw new InvalidOperationException("Invalid danger base provided");

        /// <summary>
        /// Gets the <see cref="DangerType"/> of the specified <see cref="DangerStackBase"/>.
        /// </summary>
        /// <param name="dangerStackBase">The <see cref="DangerStackBase"/> enum.</param>
        /// <param name="danger">The danger type found.</param>
        /// <returns>Whether or not the danger has been found.</returns>
        public static bool TryGetDangerType(this DangerStackBase dangerStackBase, out DangerType danger)
        {
            if (dangerStackBase == null || !TypeToDangerType.TryGetValue(dangerStackBase.GetType(), out danger))
            {
                danger = DangerType.None;
                return false;
            }

            return true;
        }
    }
}