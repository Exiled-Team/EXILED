// -----------------------------------------------------------------------
// <copyright file="SlappingPlayerEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp3114
{
    using API.Features;
    using Interfaces;
    using PlayerRoles.PlayableScps.Scp3114;

    using Scp3114Role = Exiled.API.Features.Roles.Scp3114Role;

    /// <summary>
    ///     Contains all information before SCP-3114 slaps a player.
    /// </summary>
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    // ReSharper disable MemberCanBePrivate.Global
    public sealed class SlappingPlayerEventArgs : IScp3114Event, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SlappingPlayerEventArgs" /> class.
        /// </summary>
        /// <param name="instance">
        ///     The <see cref="Scp3114Slap"/> instance which this is being instantiated from.
        /// </param>
        /// <param name="targetHub">
        ///     The <see cref="ReferenceHub"/> of the player who was being targeted.
        /// </param>
        /// <param name="humeShieldToReward">
        ///     The amount of hume shield cooldown that is given to the player.
        /// </param>
        /// <param name="damageAmount">
        ///     The amount of damage to do to the target.
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IDeniableEvent.IsAllowed"/>
        /// </param>
        public SlappingPlayerEventArgs(Scp3114Slap instance, ReferenceHub targetHub, float damageAmount, float humeShieldToReward, bool isAllowed = true)
        {
            Player = Player.Get(instance.Owner);
            Target = Player.Get(targetHub);
            DamageAmount = damageAmount;
            Scp3114 = Player.Role.As<Scp3114Role>();
            HumeShieldToReward = humeShieldToReward;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        /// <summary>
        ///     The <see cref="Player"/> who is Scp-3114.
        /// </summary>
        public Player Player { get; set; }

        /// <inheritdoc cref="IScp3114Event.Scp3114"/>
        public Scp3114Role Scp3114 { get; }

        /// <summary>
        ///     Gets the <see cref="Player"/> who is being slapped.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        ///     Gets or sets the hume shield amount that will be regenerated for the Scp 3114 <see cref="Player"/>. This will not have any effect if the event is cancelled.
        /// </summary>
        public float HumeShieldToReward { get; set; }

        /// <summary>
        ///     Gets or sets the amount of damage to deal to the <see cref="Target"/>. This will not have any effect if the event is cancelled.
        /// </summary>
        public float DamageAmount { get; set; }

        /// <inheritdoc cref="IDeniableEvent.IsAllowed"/>
        public bool IsAllowed { get; set; }
    }
}
