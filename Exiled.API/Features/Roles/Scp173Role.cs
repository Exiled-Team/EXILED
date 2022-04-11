// -----------------------------------------------------------------------
// <copyright file="Scp173Role.cs" company="Exiled Team">
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
    /// Defines a role that represents SCP-173.
    /// </summary>
    public class Scp173Role : Role
    {
        private PlayableScps.Scp173 script;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp173Role"/> class.
        /// </summary>
        /// <param name="player">The encapsulated player.</param>
        internal Scp173Role(Player player) => Owner = player;

        /// <inheritdoc/>
        public override Player Owner { get; }

        /// <summary>
        /// Gets the <see cref="Scp173"/> script for the role.
        /// </summary>
        public Scp173 Script => script ?? (script = Owner.CurrentScp as Scp173);

        /// <summary>
        /// Gets a value indicating whether or not SCP-173 is currently being viewed by one or more players.
        /// </summary>
        public bool IsObserved => Script._isObserved;

        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}"/> of players that are currently viewing SCP-173. Can be empty.
        /// </summary>
        public IReadOnlyCollection<Player> ObservingPlayers
        {
            get => Script._observingPlayers.Select(hub => Player.Get(hub)).ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets SCP-173's move speed.
        /// </summary>
        public float MoveSpeed => Script.GetMoveSpeed();

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-173 is able to blink.
        /// </summary>
        public bool BlinkReady
        {
            get => Script.BlinkReady;
            set => Script.BlinkReady = value;
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-173 can blink.
        /// </summary>
        public float BlinkCooldown
        {
            get => Script._blinkCooldownRemaining;
            set => Script._blinkCooldownRemaining = value;
        }

        /// <summary>
        /// Gets a value indicating the max distance that SCP-173 can move in a blink. Factors in <see cref="BreakneckActive"/>.
        /// </summary>
        public float BlinkDistance => Script.EffectiveBlinkDistance();

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-173's breakneck speed is active.
        /// </summary>
        public bool BreakneckActive
        {
            get => Script.BreakneckSpeedsActive;
            set => Script.BreakneckSpeedsActive = value;
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-173 can use breackneck speed again.
        /// </summary>
        public float BreakneckCooldown
        {
            get => Script._breakneckSpeedsCooldownRemaining;
            set => Script._breakneckSpeedsCooldownRemaining = value;
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-173 can place a tantrum.
        /// </summary>
        public float TantrumCooldown
        {
            get => Script._tantrumCooldownRemaining;
            set => Script._tantrumCooldownRemaining = value;
        }

        /// <inheritdoc/>
        internal override RoleType RoleType => RoleType.Scp173;

        /// <summary>
        /// Places a Tantrum (SCP-173's ability) under the player.
        /// </summary>
        /// <param name="failIfObserved">Whether or not to place the tantrum if SCP-173 is currently being viewed.</param>
        /// <returns>The tantrum's <see cref="UnityEngine.GameObject"/>, or <see langword="null"/> if it cannot be placed.</returns>
        public UnityEngine.GameObject Tantrum(bool failIfObserved = false)
        {
            if (failIfObserved && IsObserved)
                return null;

            return Owner.PlaceTantrum();
        }
    }
}
