// -----------------------------------------------------------------------
// <copyright file="ExecutingRespawningEffectsEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using Respawning;

    /// <summary>
    /// Contains all information before executing respawning effects.
    /// </summary>
    public class ExecutingRespawningEffectsEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutingRespawningEffectsEventArgs"/> class.
        /// </summary>
        /// <param name="effectType">Effect to be executed.</param>
        /// <param name="team">The <see cref="SpawnableTeamType"/> team.</param>
        /// <param name="isAllowed">Indicates whether the event can be executed or not.</param>
        public ExecutingRespawningEffectsEventArgs(RespawnEffectsController.EffectType effectType, SpawnableTeamType team, bool isAllowed = true)
        {
            EffectType = effectType;
            Team = team;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets a value indicating what the type of effect.
        /// </summary>
        public RespawnEffectsController.EffectType EffectType { get; }

        /// <summary>
        /// Gets a value indicating what the spawnable team is.
        /// </summary>
        public SpawnableTeamType Team { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the effects will be executed.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
