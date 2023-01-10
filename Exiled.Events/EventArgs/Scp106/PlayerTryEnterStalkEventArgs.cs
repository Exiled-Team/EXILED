// -----------------------------------------------------------------------
// <copyright file="PlayerTryEnterStalkEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp106
{
    using API.Features;
    using Interfaces;
    using Mirror;
    using PlayerRoles.PlayableScps.Scp106;
    using PlayerRoles.PlayableScps.Subroutines;
    using UnityEngine;

    /// <summary>
    ///     Contains all information before SCP-106 client tries to enter stalk.
    /// </summary>
    public class PlayerTryEnterStalkEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerTryEnterStalkEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="PlayerTryEnterStalkEventArgs.Player" /></param>
        /// <param name="stalkAbilityInstance"><inheritdoc cref="PlayerTryEnterStalkEventArgs.Scp106StalkAbility" /></param>
        /// <param name="cooldown"><inheritdoc cref="PlayerTryEnterStalkEventArgs.Cooldown" /></param>
        /// <param name="isAllowed"> <inheritdoc cref="PlayerTryEnterStalkEventArgs.IsAllowed" /></param>
        public PlayerTryEnterStalkEventArgs(Player player, Scp106StalkAbility stalkAbilityInstance, AbilityCooldown cooldown, bool isAllowed = true)
        {
            Player = player;
            Scp106StalkAbility = stalkAbilityInstance;
            Cooldown = cooldown;
            IsAllowed = isAllowed;
            Vigor = Scp106StalkAbility.Vigor.VigorAmount;
            BypassChecks = false;
            ValidateNewVigor = false;
            MinimumVigor = .25f;
            MustUseAllVigor = false;
        }

        /// <summary>
        /// Gets or sets vigor value.
        /// </summary>
        public float Vigor { get; set; }

        /// <summary>
        /// Gets or sets scp106 cooldown.
        /// <remarks> If you want to overwrite it, do Scp106StalkAbility.ServerWriteRpc with <see cref="NetworkWriterPool.GetWriter"/></remarks>
        /// </summary>
        public AbilityCooldown Cooldown { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether force the Scp106 to use all their current vigor before leaving state.
        /// </summary>
        public bool MustUseAllVigor { get; set; }

        /// <summary>
        /// Gets or sets minimum vigor required.
        /// </summary>
        public float MinimumVigor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether should be used to change how much vigor SCP106 needs.
        /// </summary>
        public bool ValidateNewVigor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether bypass all current NW checks.
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