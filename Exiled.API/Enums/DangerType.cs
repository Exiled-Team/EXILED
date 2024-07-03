// -----------------------------------------------------------------------
// <copyright file="DangerType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using CustomPlayerEffects.Danger;
    using Exiled.API.Features.Roles;

    /// <summary>
    /// A list of Types used by exiled for <see cref="DangerStackBase"/>.
    /// </summary>
    public enum DangerType
    {
        /// <summary>
        /// Used when there's an error.
        /// </summary>
        None = -1,

        /// <summary>
        /// Warhead has been activated but not detonated.
        /// </summary>
        /// <remarks>Used for <see cref="WarheadDanger"/>.</remarks>
        Warhead,

        /// <summary>
        /// Has <see cref="EffectType.CardiacArrest"/>.
        /// </summary>
        /// <remarks>Used for <see cref="CardiacArrestDanger"/>.</remarks>
        CardiacArrest,

        /// <summary>
        /// <see cref="Scp096Role"/> rage target.
        /// </summary>
        /// <remarks>Used for <see cref="RageTargetDanger"/>.</remarks>
        RageTarget,

        /// <summary>
        /// Has <see cref="EffectType.Corroding"/>.
        /// </summary>
        /// <remarks>Used for <see cref="CorrodingDanger"/>.</remarks>
        Corroding,

        /// <summary>
        /// Has taken damage.
        /// </summary>
        /// <remarks>Used for <see cref="PlayerDamagedDanger"/>.</remarks>
        PlayerDamaged,

        /// <summary>
        /// Encountered an SCP
        /// </summary>
        /// <remarks>Used for <see cref="ScpEncounterDanger"/>.</remarks>
        ScpEncounter,

        /// <summary>
        /// Encountered <see cref="Scp0492Role"/>.
        /// </summary>
        /// <remarks>Used for <see cref="ZombieEncounterDanger"/>.</remarks>
        ZombieEncounter,

        /// <summary>
        /// Encountered an armed enemy.
        /// </summary>
        /// <remarks>Used for <see cref="ArmedEnemyDanger"/>.</remarks>
        ArmedEnemyEncounter,
    }
}