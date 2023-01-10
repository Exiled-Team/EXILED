// -----------------------------------------------------------------------
// <copyright file="ServerChangingStalk.cs" company="Exiled Team">
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
    ///     Contains all information before SCP-106 server changes stalk.
    /// </summary>
    public class ServerChangingStalk : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerChangingStalk"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="ServerChangingStalk.Player" /></param>
        /// <param name="stalkAbilityInstance"><inheritdoc cref="ServerChangingStalk.Scp106StalkAbility" /></param>
        /// <param name="cooldownAbility"><inheritdoc cref="ServerChangingStalk.CooldownAbility" /></param>
        /// <param name="isActive"><inheritdoc cref="ServerChangingStalk.IsActive" /></param>
        /// <param name="isAllowed"> <inheritdoc cref="ServerChangingStalk.IsAllowed" /></param>
        public ServerChangingStalk(Player player, Scp106StalkAbility stalkAbilityInstance, AbilityCooldown cooldownAbility, bool isActive, bool isAllowed = true)
        {
            Player = player;
            Scp106StalkAbility = stalkAbilityInstance;
            CooldownAbility = cooldownAbility;
            IsActive = isActive;
            Vigor = Scp106StalkAbility.Vigor.VigorAmount;
            Cooldown = 20f;
            IsAllowed = isAllowed;
            BypassChecks = false;
            ValidateNewVigor = false;
            MinimumVigor = .25f;
            MustUseAllVigor = false;
            AllowNwEventHandler = true;
            TargetDuration = 2.5f;
        }

        /// <summary>
        /// Gets or sets target duration if active.
        /// </summary>
        public float TargetDuration { get; set; }

        /// <summary>
        /// Gets or sets how long of a cooldown for ability.
        /// </summary>
        public float Cooldown { get; set; }

        /// <summary>
        /// Gets a value indicating whether whether stalk is currently active.
        /// </summary>
        public bool IsActive { get; }

        /// <summary>
        /// Gets or sets a value indicating whether allows NW api event call handler to occur.
        /// </summary>
        public bool AllowNwEventHandler { get; set; }

        /// <summary>
        /// Gets or sets vigor value.
        /// </summary>
        public float Vigor { get; set; }

        /// <summary>
        /// Gets or sets scp106 cooldown.
        /// <remarks> If you want to overwrite it, do Scp106StalkAbility.ServerWriteRpc with <see cref="NetworkWriterPool.GetWriter"/> </remarks>
        /// </summary>
        public AbilityCooldown CooldownAbility { get; set; }

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
        ///     Gets the stalk ability instance.
        /// </summary>
        public Scp106StalkAbility Scp106StalkAbility { get; }

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