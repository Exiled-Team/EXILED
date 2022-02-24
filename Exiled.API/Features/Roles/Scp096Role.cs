// -----------------------------------------------------------------------
// <copyright file="Scp096Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System.Collections.Generic;
    using System.Linq;

    using PlayableScps;

    /// <summary>
    /// Defines a role that represents SCP-096.
    /// </summary>
    public class Scp096Role : Role
    {
        private Scp096 script;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp096Role"/> class.
        /// </summary>
        /// <param name="player">The encapsulated player.</param>
        internal Scp096Role(Player player) => Owner = player;

        /// <summary>
        /// Gets the actual script of the Scp.
        /// </summary>
        public Scp096 Scp096 => script ?? (script = Owner.ReferenceHub.scpsController.CurrentScp as Scp096);

        /// <inheritdoc/>
        public override Player Owner { get; }

        /// <summary>
        /// Gets a value indicating SCP-096's state.
        /// </summary>
        public Scp096PlayerState State => Scp096.PlayerState;

        /// <summary>
        /// Gets a value indicating whether or not SCP-096 can receive targets.
        /// </summary>
        public bool CanReceiveTargets => Scp096.CanReceiveTargets;

        /// <summary>
        /// Gets a value indicating whether or not SCP-096 is currently enraged.
        /// </summary>
        public bool IsEnraged => Scp096.Enraged;

        /// <summary>
        /// Gets a value indicating whether or not SCP-096 is currently docile.
        /// </summary>
        public bool IsDocile => !IsEnraged;

        /// <summary>
        /// Gets a value indicating whether or not SCP-096 is currently trying not to cry behind a door.
        /// </summary>
        public bool TryingNotToCry => State == Scp096PlayerState.TryNotToCry;

        /// <summary>
        /// Gets a value indicating whether or not SCP-096 is currently prying a gate.
        /// </summary>
        public bool IsPryingGate => State == Scp096PlayerState.PryGate;

        /// <summary>
        /// Gets a value indicating whether or not SCP-096 is currently charging.
        /// </summary>
        public bool IsCharging => Scp096.Charging;

        /// <summary>
        /// Gets or sets the amount of time in between SCP-096 charges.
        /// </summary>
        public float ChargeCooldown
        {
            get => Scp096._chargeCooldown;
            set => Scp096._chargeCooldown = value;
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-096 can be enraged again.
        /// </summary>
        public float EnrageCooldown
        {
            get => Scp096.RemainingEnrageCooldown;
            set => Scp096.RemainingEnrageCooldown = value;
        }

        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}"/> of Players that are currently targeted by SCP-096.
        /// </summary>
        public IReadOnlyCollection<Player> Targets => Scp096._targets.Select(hub => Player.Get(hub)).ToList().AsReadOnly();

        /// <inheritdoc/>
        internal override RoleType RoleType => RoleType.Scp096;
    }
}
