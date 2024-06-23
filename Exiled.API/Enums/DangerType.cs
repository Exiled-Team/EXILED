// -----------------------------------------------------------------------
// <copyright file="DangerType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using CustomPlayerEffects.Danger;

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
        /// Warhead.
        /// </summary>
        Warhead,

        /// <summary>
        /// Cardiac Arrest.
        /// </summary>
        CardiacArrest,

        /// <summary>
        /// Rage Target.
        /// </summary>
        RageTarget,

        /// <summary>
        /// Corroding.
        /// </summary>
        Corroding,

        /// <summary>
        /// Player Damaged.
        /// </summary>
        PlayerDamaged,

        /// <summary>
        /// Scp Encounter.
        /// </summary>
        ScpEncounter,

        /// <summary>
        /// Zombie Encounter.
        /// </summary>
        ZombieEncounter,

        /// <summary>
        /// Armed Enemy.
        /// </summary>
        ArmedEnemy,
    }
}