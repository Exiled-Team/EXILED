// -----------------------------------------------------------------------
// <copyright file="TeleportingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Mirror;
using PlayerRoles.PlayableScps.Scp106;
using PlayerRoles.PlayableScps.Subroutines;

namespace Exiled.Events.EventArgs.Scp106
{
    using API.Features;
    using Interfaces;

    using UnityEngine;

    /// <summary>
    ///     Contains all information before SCP-106 teleports using a portal.
    /// </summary>
    public class PlayerTryEnterStalkEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PlayerTryEnterStalkEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="position">
        ///     <inheritdoc cref="Position" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public PlayerTryEnterStalkEventArgs(Player player, Scp106StalkAbility stalkAbilityInstance, AbilityCooldown cooldown, bool isAllowed = true)
        {
            Player = player;
            Scp106StalkAbility = stalkAbilityInstance;
            Vigor = Scp106StalkAbility.Vigor.VigorAmount;
            Cooldown = cooldown;
            IsAllowed = isAllowed;
            BypassChecks = false;
            ValidateNewVigor = false;
            MinimumVigor = .25f;
            MustUseAllVigor = false;

        }

        /// <summary>
        /// Vigor value
        /// </summary>
        public float Vigor { get; set; }

        /// <summary>
        /// Scp106 cooldown, if you want to overwrite it, do <see cref="Scp106StalkAbility.ServerWriteRpc"/> with <see cref="NetworkWriterPool.GetWriter"/>
        /// </summary>
        public AbilityCooldown Cooldown { get; set; }

        /// <summary>
        /// Force the Scp106 to use all their current vigor before leaving state.
        /// </summary>
        public bool MustUseAllVigor { get; set; }

        /// <summary>
        /// Minimum vigor required
        /// </summary>
        public float MinimumVigor { get; set; }

        /// <summary>
        /// Should be used to change how much vigor SCP106 needs
        /// </summary>
        public bool ValidateNewVigor { get; set; }

        /// <summary>
        /// Bypass all current NW checks
        /// </summary>
        public bool BypassChecks { get; set; }

        /// <summary>
        ///     Gets or sets the stalk ability instance.
        /// </summary>
        public Scp106StalkAbility Scp106StalkAbility { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not SCP-106 can teleport using a portal.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who's controlling SCP-106.
        /// </summary>
        public Player Player { get; }
    }
}