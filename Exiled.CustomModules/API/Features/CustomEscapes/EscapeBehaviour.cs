// -----------------------------------------------------------------------
// <copyright file="EscapeBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomEscapes
{
    using System.Collections.Generic;
    using System.Reflection;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.API.Features.DynamicEvents;
    using Exiled.CustomModules.API.Enums;
    using Exiled.CustomModules.API.Features.CustomRoles;
    using Exiled.CustomModules.API.Features.Generic;
    using PlayerRoles;

    /// <summary>
    /// Represents the base class for custom escape behaviors.
    /// </summary>
    /// <remarks>
    /// This class extends <see cref="ModuleBehaviour{TEntity}"/> and implements <see cref="IAdditiveSettingsCollection{T}"/>.
    /// <br/>It serves as the foundation for creating custom escape behaviors associated with in-game player actions.
    /// </remarks>
    public abstract class EscapeBehaviour : ModuleBehaviour<Player>, IAdditiveSettingsCollection<EscapeSettings>
    {
        private List<EscapeSettings> settings;

        /// <summary>
        /// Gets the relative <see cref="CustomEscapes.CustomEscape"/>.
        /// </summary>
        public CustomEscape CustomEscape { get; private set; }

        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of <see cref="EscapeSettings"/> containing all escape's settings.
        /// </summary>
        public virtual List<EscapeSettings> Settings
        {
            get => settings ??= CustomEscape.Settings;
            set => settings = value;
        }

        /// <inheritdoc/>
        public override ModulePointer Config
        {
            get => config ??= CustomEscape.Config;
            protected set => config = value;
        }

        /// <summary>
        /// Gets the current escape scenario.
        /// </summary>
        protected virtual byte CurrentScenario => CalculateEscapeScenario();

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> handling all bound delegates to be fired before escaping.
        /// </summary>
        [DynamicEventDispatcher]
        protected TDynamicEventDispatcher<Events.EventArgs.CustomEscapes.EscapingEventArgs> EscapingDispatcher { get; set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> handling all bound delegates to be fired after escaping.
        /// </summary>
        [DynamicEventDispatcher]
        protected TDynamicEventDispatcher<Player> EscapedDispatcher { get; set; } = new();

        /// <inheritdoc/>
        public virtual void AdjustAdditivePipe()
        {
            CustomRole customRole = Owner.Cast<Pawn>().CustomRole;

            if (CustomEscape.TryGet(GetType(), out CustomEscape customEscape))
            {
                CustomEscape = customEscape;
                Settings = customRole is not null && !customRole.EscapeSettings.IsEmpty() ?
                    customRole.EscapeSettings : CustomEscape.Settings;
            }

            if (CustomEscape is null || Settings is null || Config is null)
            {
                Log.Error($"Custom escape ({GetType().Name}) has invalid configuration.");
                Destroy();
            }

            ImplementConfigs();
        }

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            AdjustAdditivePipe();

            FixedTickRate = 0.5f;
        }

        /// <inheritdoc/>
        protected override void Tick()
        {
            base.Tick();

            foreach (EscapeSettings settings in Settings)
            {
                if (!settings.IsAllowed || MathExtensions.DistanceSquared(Owner.Position, settings.Position) > settings.DistanceThreshold * settings.DistanceThreshold)
                    continue;

                Events.EventArgs.CustomEscapes.EscapingEventArgs ev = new(Owner.Cast<Pawn>(), settings.Role, settings.CustomRole, CurrentScenario, CustomEscape.AllScenarios[CurrentScenario]);
                EscapingDispatcher.InvokeAll(ev);

                if (!ev.IsAllowed)
                    continue;

                ev.Player.Cast<Pawn>().SetRole(ev.NewRole != RoleTypeId.None ? ev.NewRole : ev.NewCustomRole);
                ev.Player.ShowHint(ev.Hint);

                EscapedDispatcher.InvokeAll(ev.Player);

                CanEverTick = false;
                Destroy();

                break;
            }
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            CustomEscape.Detach(Owner);
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            EscapingDispatcher.Bind(this, OnEscaping);
            EscapedDispatcher.Bind(this, OnEscaped);
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
        /// <param name="ev">The <see cref="Events.EventArgs.CustomEscapes.EscapingEventArgs"/> instance.</param>
        protected virtual void OnEscaping(Events.EventArgs.CustomEscapes.EscapingEventArgs ev)
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