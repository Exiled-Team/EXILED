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

    using Scp049Role = API.Features.Roles.Scp049Role;

    /// <summary>
    ///     Contains all information before SCP-049 sense is activated.
    /// </summary>
    public class ActivatingSenseEventArgs : IScp049Event, IDeniableEvent
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
            Scp049 = player.Role.As<Scp049Role>();
            Target = target;
            IsAllowed = isAllowed;
            Cooldown = Scp049SenseAbility.AttemptFailCooldown;
            Duration = Scp049SenseAbility.EffectDuration;
        }

        /// <summary>
        /// Gets the Player who is playing as SCP-049.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp049Role Scp049 { get; }

        /// <summary>
        /// Gets the Player who the sense ability is affecting.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets or sets the cooldown of the ability.
        /// </summary>
        public float Cooldown { get; set; }

        /// <summary>
        /// Gets or sets the duration of the ability.
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the server will send 049 information on the recall.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
