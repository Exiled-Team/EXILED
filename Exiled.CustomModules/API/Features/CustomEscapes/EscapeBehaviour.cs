// -----------------------------------------------------------------------
// <copyright file="EscapeBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomEscapes
{
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.API.Features.DynamicEvents;
    using Exiled.CustomModules.API.Enums;
    using Exiled.CustomModules.API.Features.CustomRoles;

    using PlayerRoles;
    using UnityEngine;

    using EscapingEventArgs = Events.EscapingEventArgs;

    /// <summary>
    /// A tool to easily manage custom escaping's logic.
    /// </summary>
    public abstract class EscapeBehaviour : EPlayerBehaviour, IAdditiveSettingsCollection<EscapeSettings>
    {
        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of <see cref="EscapeSettings"/> containing all escape settings.
        /// </summary>
        public virtual List<EscapeSettings> Settings { get; set; }

        /// <summary>
        /// Gets the current escape scenario.
        /// </summary>
        protected virtual byte CurrentScenario => CalculateEscapeScenario();

        /// <summary>
        /// Gets the <see cref="TDynamicEventDispatcher{T}"/> handling all bound delegates to be fired before escaping.
        /// </summary>
        [DynamicEventDispatcher]
        protected TDynamicEventDispatcher<EscapingEventArgs> EscapingEventDispatcher { get; private set; }

        /// <summary>
        /// Gets the <see cref="TDynamicEventDispatcher{T}"/> handling all bound delegates to be fired after escaping.
        /// </summary>
        [DynamicEventDispatcher]
        protected TDynamicEventDispatcher<Player> EscapedEventDispatcher { get; private set; }

        /// <inheritdoc/>
        public abstract void AdjustAddittivePipe();

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            if (Owner.Cast<Pawn>().TryGetCustomRole(out CustomRole customRole) && !customRole.EscapeSettings.IsEmpty())
                Settings = customRole.EscapeSettings;

            AdjustAddittivePipe();

            FixedTickRate = 0.5f;
        }

        /// <inheritdoc/>
        protected override void Tick()
        {
            base.Tick();

            foreach (EscapeSettings settings in Settings)
            {
                if (!settings.IsAllowed || Vector3.Distance(Owner.Position, settings.Position) > settings.DistanceThreshold)
                    continue;

                EscapingEventArgs ev = new(Owner.Cast<Pawn>(), settings.Role, settings.CustomRole, CurrentScenario, CustomEscape.AllScenarios[CurrentScenario]);
                EscapingEventDispatcher.InvokeAll(ev);

                if (!ev.IsAllowed)
                    continue;

                ev.Player.SetRole(ev.NewRole != RoleTypeId.None ? ev.NewRole : ev.NewCustomRole);
                ev.Player.ShowHint(ev.Hint);

                EscapedEventDispatcher.InvokeAll(ev.Player);

                CanEverTick = false;
                Destroy();

                break;
            }
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            EscapingEventDispatcher.Bind(this, OnEscaping);
            EscapedEventDispatcher.Bind(this, OnEscaped);
        }

        /// <summary>
        /// Calculates the escape scenario.
        /// <para>An <see langword="override"/> is required in order to make it work.</para>
        /// </summary>
        /// <returns>The corresponding <see cref="UUEscapeScenarioType"/>.</returns>
        protected abstract UUEscapeScenarioType CalculateEscapeScenario();

        /// <summary>
        /// Fired before the player escapes.
        /// </summary>
        /// <param name="ev">The <see cref="EscapingEventArgs"/> instance.</param>
        protected virtual void OnEscaping(EscapingEventArgs ev)
        {
        }

        /// <summary>
        /// Fired after the player escapes.
        /// </summary>
        /// <param name="player">The player who escaped.</param>
        protected virtual void OnEscaped(Player player)
        {
        }
    }
}