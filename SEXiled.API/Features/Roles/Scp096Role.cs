// -----------------------------------------------------------------------
// <copyright file="Scp096Role.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.API.Features.Roles
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
        internal Scp096Role(Player player)
        {
            Owner = player;
            script = player.ReferenceHub.scpsController.CurrentScp as Scp096;
        }

        /// <inheritdoc/>
        public override Player Owner { get; }

        /// <summary>
        /// Gets a value indicating SCP-096's state.
        /// </summary>
        public Scp096PlayerState State => script.PlayerState;

        /// <summary>
        /// Gets a value indicating whether or not SCP-096 can receive targets.
        /// </summary>
        public bool CanReceiveTargets => script.CanReceiveTargets;

        /// <summary>
        /// Gets a value indicating whether or not SCP-096 is currently enraged.
        /// </summary>
        public bool IsEnraged => script.Enraged;

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
        public bool IsCharging => script.Charging;

        /// <summary>
        /// Gets or sets the amount of time in between SCP-096 charges.
        /// </summary>
        public float ChargeCooldown
        {
            get => script._chargeCooldown;
            set => script._chargeCooldown = value;
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-096 can be enraged again.
        /// </summary>
        public float EnrageCooldown
        {
            get => script.RemainingEnrageCooldown;
            set => script.RemainingEnrageCooldown = value;
        }

        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}"/> of Players that are currently targeted by SCP-096.
        /// </summary>
        public IReadOnlyCollection<Player> Targets => script._targets.Select(hub => Player.Get(hub)).ToList().AsReadOnly();

        /// <inheritdoc/>
        internal override RoleType RoleType => RoleType.Scp096;
    }
}
