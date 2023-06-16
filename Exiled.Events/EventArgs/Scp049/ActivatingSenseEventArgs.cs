// -----------------------------------------------------------------------
// <copyright file="ActivatingSenseEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp049
{
    using API.Features;

    using Interfaces;

    using PlayerRoles.PlayableScps.Scp049;

    /// <summary>
    ///     Contains all information before SCP-049 sense is activated.
    /// </summary>
    public class ActivatingSenseEventArgs : IPlayerEvent, ITargetEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivatingSenseEventArgs"/> class with information before SCP-049 sense is activated.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ActivatingSenseEventArgs(Player player, Player target, bool isAllowed = true)
        {
            Player = player;
            Target = target;
            IsAllowed = isAllowed;
            Cooldown = Scp049SenseAbility.AttemptFailCooldown;
            Duration = Scp049SenseAbility.EffectDuration;
        }

        /// <inheritdoc />
        public Player Player { get; }

        /// <inheritdoc />
        public Player Target { get; }

        /// <summary>
        /// Gets or sets the cooldown of the ability.
        /// </summary>
        public float Cooldown { get; set; }

        /// <summary>
        /// Gets or sets the duration of the ability.
        /// </summary>
        public float Duration { get; set; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; }
    }
}
