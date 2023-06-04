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

    using Mirror;

    using PlayerRoles;
    using PlayerRoles.PlayableScps.HumeShield;
    using PlayerRoles.PlayableScps.Scp173;
    using PlayerRoles.PlayableScps.Subroutines;

    using UnityEngine;

    using Scp173GameRole = PlayerRoles.PlayableScps.Scp173.Scp173Role;

    /// <summary>
    /// Defines a role that represents SCP-173.
    /// </summary>
    public class Scp173Role : FpcRole, ISubroutinedScpRole, IHumeShieldRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp173Role"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="Scp173GameRole"/>.</param>
        internal Scp173Role(Scp173GameRole baseRole)
            : base(baseRole)
        {
            Base = baseRole;
            SubroutineModule = baseRole.SubroutineModule;
            HumeShieldModule = baseRole.HumeShieldModule;
            MovementModule = FirstPersonController.FpcModule as Scp173MovementModule;

            if (!SubroutineModule.TryGetSubroutine(out Scp173ObserversTracker scp173ObserversTracker))
                Log.Error("Scp173ObserversTracker not found in Scp173Role::ctor");

            ObserversTracker = scp173ObserversTracker;

            if (!SubroutineModule.TryGetSubroutine(out Scp173BlinkTimer scp173BlinkTimer))
                Log.Error("Scp173BlinkTimer not found in Scp173Role::ctor");

            BlinkTimer = scp173BlinkTimer;

            if (!SubroutineModule.TryGetSubroutine(out Scp173TeleportAbility scp173TeleportAbility))
                Log.Error("Scp173TeleportAbility not found in Scp173Role::ctor");

            TeleportAbility = scp173TeleportAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp173TantrumAbility scp173TantrumAbility))
                Log.Error("Scp173TantrumAbility not found in Scp173Role::ctor");

            TantrumAbility = scp173TantrumAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp173AudioPlayer scp173AudioPlayer))
                Log.Error("Scp173AudioPlayer not found in Scp173Role::ctor");

            AudioPlayer = scp173AudioPlayer;

            if (!SubroutineModule.TryGetSubroutine(out Scp173BreakneckSpeedsAbility scp173BreakneckSpeedsAbility))
                Log.Error("Scp173BreakneckSpeedsAbility not found in Scp173Role::ctor");

            BreakneckSpeedsAbility = scp173BreakneckSpeedsAbility;
        }

        /// <summary>
        /// Gets a list of players who will be turned away from SCP-173.
        /// </summary>
        public static HashSet<Player> TurnedPlayers { get; } = new(20);

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Scp173;

        /// <inheritdoc/>
        public SubroutineManagerModule SubroutineModule { get; }

        /// <inheritdoc/>
        public HumeShieldModuleBase HumeShieldModule { get; }

        /// <summary>
        /// Gets SCP-173's movement module.
        /// </summary>
        public Scp173MovementModule MovementModule { get; }

        /// <summary>
        /// Gets SCP-173's <see cref="Scp173ObserversTracker"/>.
        /// </summary>
        public Scp173ObserversTracker ObserversTracker { get; }

        /// <summary>
        /// Gets SCP-173's <see cref="Scp173BlinkTimer"/>.
        /// </summary>
        public Scp173BlinkTimer BlinkTimer { get; }

        /// <summary>
        /// Gets SCP-173's <see cref="Scp173TeleportAbility"/>.
        /// </summary>
        public Scp173TeleportAbility TeleportAbility { get; }

        /// <summary>
        /// Gets SCP-173's <see cref="Scp173TantrumAbility"/>.
        /// </summary>
        public Scp173TantrumAbility TantrumAbility { get; }

        /// <summary>
        /// Gets SCP-173's <see cref="Scp173BreakneckSpeedsAbility"/>.
        /// </summary>
        public Scp173BreakneckSpeedsAbility BreakneckSpeedsAbility { get; }

        /// <summary>
        /// Gets the SCP-173's <see cref="Scp173AudioPlayer"/>.
        /// </summary>
        public Scp173AudioPlayer AudioPlayer { get; }

        /// <summary>
        /// Gets or sets the amount of time before SCP-173 can use breakneck speed again.
        /// </summary>
        public float RemainingBreakneckCooldown
        {
            get => BreakneckSpeedsAbility.Cooldown.Remaining;
            set
            {
                BreakneckSpeedsAbility.Cooldown.Remaining = value;
                BreakneckSpeedsAbility.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-173 can place a tantrum.
        /// </summary>
        public float RemainingTantrumCooldown
        {
            get => TantrumAbility.Cooldown.Remaining;
            set
            {
                TantrumAbility.Cooldown.Remaining = value;
                TantrumAbility.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not SCP-173 is currently being viewed by one or more players.
        /// </summary>
        public bool IsObserved => ObserversTracker.IsObserved;

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of players that are currently viewing SCP-173. Can be empty.
        /// </summary>
        public IEnumerable<Player> ObservingPlayers => ObserversTracker.Observers.Select(x => Player.Get(x));

        /// <summary>
        /// Gets SCP-173's max move speed.
        /// </summary>
        public float MaxMovementSpeed => MovementModule.MaxMovementSpeed;

        /// <summary>
        /// Gets the SCP-173's movement speed.
        /// </summary>
        public override float MovementSpeed => MovementModule.ServerSpeed;

        /// <summary>
        /// Gets or sets SCP-173's simulated stare. SCP-173 will be treated as though it is being looked at while this value is greater than <c>0</c>.
        /// </summary>
        public float SimulatedStare
        {
            get => ObserversTracker.SimulatedStare;
            set => ObserversTracker.SimulatedStare = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-173 is able to blink.
        /// </summary>
        public bool BlinkReady
        {
            get => BlinkTimer.AbilityReady;
            set
            {
                if (value)
                {
                    BlinkTimer._endSustainTime = -1;
                    BlinkTimer._totalCooldown = 0;
                    BlinkTimer._initialStopTime = NetworkTime.time;
                }
                else
                {
                    BlinkTimer.ResetObject();
                    BlinkTimer._observers.UpdateObservers();
                }

                BlinkTimer.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-173 can blink.
        /// </summary>
        public float BlinkCooldown
        {
            get => BlinkTimer.RemainingBlinkCooldown;
            set
            {
                BlinkTimer._initialStopTime = NetworkTime.time;
                BlinkTimer._totalCooldown = value;
                BlinkTimer.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets a value indicating the max distance that SCP-173 can move in a blink. Factors in <see cref="BreakneckActive"/>.
        /// </summary>
        public float BlinkDistance => TeleportAbility.EffectiveBlinkDistance;

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-173's breakneck speed is active.
        /// </summary>
        public bool BreakneckActive
        {
            get => BreakneckSpeedsAbility.IsActive;
            set => BreakneckSpeedsAbility.IsActive = value;
        }

        /// <summary>
        /// Gets the <see cref="Scp173GameRole"/> instance.
        /// </summary>
        public new Scp173GameRole Base { get; }

        /// <summary>
        /// Places a Tantrum (SCP-173's ability) under the player.
        /// </summary>
        /// <param name="failIfObserved">Whether or not to place the tantrum if SCP-173 is currently being viewed.</param>
        /// <param name="cooldown">The cooldown until SCP-173 can place a tantrum again. Set to <c>0</c> to not affect the cooldown.</param>
        /// <returns>The tantrum's <see cref="GameObject"/>, or <see langword="null"/> if it cannot be placed.</returns>
        public GameObject Tantrum(bool failIfObserved = false, float cooldown = 0)
        {
            if (failIfObserved && IsObserved)
                return null;

            TantrumAbility.Cooldown.Trigger(cooldown);

            return Owner.PlaceTantrum();
        }

        /// <summary>
        /// Plays a SCP-173 Audio Clip (Snap, Hit, Teleport).
        /// </summary>
        /// <param name="soundId">The SoundId to Play.</param>
        public void SendAudio(Scp173AudioPlayer.Scp173SoundId soundId) => AudioPlayer.ServerSendSound(soundId);

        /// <summary>
        /// Teleport SCP-173 using the blink ability to the Target Position.
        /// </summary>
        /// <param name="targetPos">The Target Position.</param>
        public void Blink(Vector3 targetPos) => BlinkTimer.ServerBlink(targetPos);

        /// <summary>
        /// Gets the Spawn Chance of SCP-173.
        /// </summary>
        /// <param name="alreadySpawned">The List of Roles already spawned.</param>
        /// <returns>The Spawn Chance.</returns>
        public float GetSpawnChance(List<RoleTypeId> alreadySpawned) => Base.GetSpawnChance(alreadySpawned);
    }
}