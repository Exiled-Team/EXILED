// -----------------------------------------------------------------------
// <copyright file="Scp106Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System.Collections.Generic;

    using Exiled.API.Enums;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Attributes;
    using PlayerRoles;
    using PlayerRoles.PlayableScps;
    using PlayerRoles.PlayableScps.HumeShield;
    using PlayerRoles.PlayableScps.Scp106;
    using PlayerRoles.Subroutines;
    using PlayerStatsSystem;
    using UnityEngine;

    using Scp106GameRole = PlayerRoles.PlayableScps.Scp106.Scp106Role;

    /// <summary>
    /// Defines a role that represents SCP-106.
    /// </summary>
    public class Scp106Role : FpcRole, ISubroutinedScpRole, IHumeShieldRole, ISpawnableScp
    {
        private readonly ConstProperty<float> vigorStalkCostStationary = new(Scp106StalkAbility.VigorStalkCostStationary, new[] { typeof(Scp106StalkAbility) });
        private readonly ConstProperty<float> vigorStalkCostMoving = new(Scp106StalkAbility.VigorStalkCostMoving, new[] { typeof(Scp106StalkAbility) });
        private readonly ConstProperty<float> vigorRegeneration = new(Scp106StalkAbility.VigorRegeneration, new[] { typeof(Scp106StalkAbility) });
        private readonly ConstProperty<float> corrodingTime = new(Scp106Attack.CorrodingTime, new[] { typeof(Scp106Attack) });
        private readonly ConstProperty<float> vigorCaptureReward = new(Scp106Attack.VigorCaptureReward, new[] { typeof(Scp106Attack) });
        private readonly ConstProperty<float> cooldownReductionReward = new(Scp106Attack.CooldownReductionReward, new[] { typeof(Scp106Attack) });
        private readonly ConstProperty<double> sinkholeCooldownDuration = new(Scp106SinkholeController.CooldownDuration, new[] { typeof(Scp106SinkholeController) });
        private readonly ConstProperty<float> huntersAtlasCostPerMeter = new(Scp106HuntersAtlasAbility.CostPerMeter, new[] { typeof(Scp106HuntersAtlasAbility) });

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp106Role"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="Scp106GameRole"/>.</param>
        internal Scp106Role(Scp106GameRole baseRole)
            : base(baseRole)
        {
            SubroutineModule = baseRole.SubroutineModule;
            HumeShieldModule = baseRole.HumeShieldModule;
            Base = baseRole;
            MovementModule = FirstPersonController.FpcModule as Scp106MovementModule;

            if (!SubroutineModule.TryGetSubroutine(out Scp106VigorAbilityBase scp106VigorAbilityBase))
                Log.Error("Scp106VigorAbilityBase subroutine not found in Scp106Role::ctor");

            VigorAbility = scp106VigorAbilityBase;

            if (!SubroutineModule.TryGetSubroutine(out Scp106Attack scp106Attack))
                Log.Error("Scp106Attack subroutine not found in Scp106Role::ctor");

            Attack = scp106Attack;

            if (!SubroutineModule.TryGetSubroutine(out Scp106StalkAbility scp106StalkAbility))
                Log.Error("Scp106StalkAbility not found in Scp106Role::ctor");

            StalkAbility = scp106StalkAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp106HuntersAtlasAbility scp106HuntersAtlasAbility))
                Log.Error("Scp106HuntersAtlasAbility not found in Scp106Role::ctor");

            HuntersAtlasAbility = scp106HuntersAtlasAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp106SinkholeController scp106SinkholeController))
                Log.Error("Scp106SinkholeController not found in Scp106Role::ctor");

            SinkholeController = scp106SinkholeController;
        }

        /// <inheritdoc/>
        [EProperty(readOnly: true, category: nameof(Role))]
        public override RoleTypeId Type { get; } = RoleTypeId.Scp106;

        /// <inheritdoc/>
        public SubroutineManagerModule SubroutineModule { get; }

        /// <summary>
        /// Gets the <see cref="HumeShieldModuleBase"/>.
        /// </summary>
        public HumeShieldModuleBase HumeShieldModule { get; }

        /// <summary>
        /// Gets the <see cref="Scp106VigorAbilityBase"/>.
        /// </summary>
        public Scp106VigorAbilityBase VigorAbility { get; }

        /// <summary>
        /// Gets the <see cref="VigorStat"/>.
        /// </summary>
        public VigorStat VigorComponent => VigorAbility.Vigor;

        /// <summary>
        /// Gets the <see cref="Scp106Attack"/>.
        /// </summary>
        public Scp106Attack Attack { get; }

        /// <summary>
        /// Gets the <see cref="Scp106Attack"/>.
        /// </summary>
        public Scp106StalkAbility StalkAbility { get; }

        /// <summary>
        /// Gets the <see cref="Scp106HuntersAtlasAbility"/>.
        /// </summary>
        public Scp106HuntersAtlasAbility HuntersAtlasAbility { get; }

        /// <summary>
        /// Gets the <see cref="Scp106SinkholeController"/>.
        /// </summary>
        public Scp106SinkholeController SinkholeController { get; }

        /// <summary>
        /// Gets the <see cref="Scp106MovementModule"/>.
        /// </summary>
        public new Scp106MovementModule MovementModule { get; }

        /// <summary>
        /// Gets or sets SCP-106's Vigor Level.
        /// </summary>
        [EProperty(category: nameof(Scp106Role))]
        public float Vigor
        {
            get => VigorAbility.VigorAmount;
            set => VigorAbility.VigorAmount = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-106 is currently submerged.
        /// </summary>
        [EProperty(category: nameof(Scp106Role))]
        public bool IsSubmerged
        {
            get => Base.IsSubmerged;
            set => HuntersAtlasAbility.SetSubmerged(value);
        }

        /// <summary>
        /// Gets a value indicating whether or not SCP-106 can activate teslas.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Scp106Role))]
        public bool CanActivateTesla => Base.CanActivateShock;

        /// <summary>
        /// Gets a value indicating whether if SCP-106 <see cref="Scp106StalkAbility"/> can be cleared.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Scp106Role))]
        public bool CanStopStalk => StalkAbility.CanBeCleared;

        /// <summary>
        /// Gets a value indicating whether or not SCP-106 is currently slow down by a door.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Scp106Role))]
        public bool IsSlowdown => MovementModule._slowndownTarget is < 1;

        /// <summary>
        /// Gets a value indicating the current time of the sinkhole.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Scp106Role))]
        public float SinkholeCurrentTime => SinkholeController.ElapsedToggle;

        /// <summary>
        /// Gets a value indicating the normalized state of the sinkhole.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Scp106Role))]
        public float SinkholeNormalizedState => SinkholeController.NormalizedState;

        /// <summary>
        /// Gets a value indicating whether or not SCP-106 is currently in the middle of an animation.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Scp106Role))]
        public bool IsDuringAnimation => SinkholeController.IsDuringAnimation;

        /// <summary>
        /// Gets a value indicating whether or not SCP-106 sinkhole is hidden.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Scp106Role))]
        public bool IsSinkholeHidden => SinkholeController.IsHidden;

        /// <summary>
        /// Gets or sets a value indicating whether the current sinkhole state.
        /// </summary>
        [EProperty(category: nameof(Scp106Role))]
        public bool SinkholeState
        {
            get => SinkholeController.State;
            set => SinkholeController.State = value;
        }

        /// <summary>
        /// Gets the sinkhole target duration.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Scp106Role))]
        public float SinkholeTargetDuration => SinkholeController.TargetDuration;

        /// <summary>
        /// Gets the speed multiplier of the sinkhole.
        /// </summary>
        public float SinkholeSpeedMultiplier => SinkholeController.SpeedMultiplier;

        /// <summary>
        /// Gets or sets how mush cost the Ability Stalk will cost per tick when being stationary.
        /// </summary>
        public float VigorStalkCostStationary
        {
            get => vigorStalkCostStationary;
            set => vigorStalkCostStationary.Value = value;
        }

        /// <summary>
        /// Gets or sets how mush cost the Ability Stalk will cost per tick when moving.
        /// </summary>
        public float VigorStalkCostMoving
        {
            get => vigorStalkCostMoving;
            set => vigorStalkCostMoving.Value = value;
        }

        /// <summary>
        /// Gets or sets how mush vigor will be regenerate while moving per seconds.
        /// </summary>
        public float VigorRegeneration
        {
            get => vigorRegeneration;
            set => vigorRegeneration.Value = value;
        }

        /// <summary>
        /// Gets or sets the duration of Corroding effect.
        /// </summary>
        public float CorrodingTime
        {
            get => corrodingTime;
            set => corrodingTime.Value = value;
        }

        /// <summary>
        /// Gets or sets how mush vigor Scp106 will gain when being reward for having caught a player.
        /// </summary>
        public float VigorCaptureReward
        {
            get => vigorCaptureReward;
            set => vigorCaptureReward.Value = value;
        }

        /// <summary>
        /// Gets or sets how mush reduction cooldown Scp106 will gain when being reward for having caught a player.
        /// </summary>
        public float CooldownReductionReward
        {
            get => cooldownReductionReward;
            set => cooldownReductionReward.Value = value;
        }

        /// <summary>
        /// Gets or sets the cooldown duration of it's Sinkhole ability's.
        /// </summary>
        public double SinkholeCooldownDuration
        {
            get => sinkholeCooldownDuration;
            set => sinkholeCooldownDuration.Value = value;
        }

        /// <summary>
        /// Gets or sets how mush vigor it's ability Hunter Atlas will cost per meter.
        /// </summary>
        public float HuntersAtlasCostPerMeter
        {
            get => huntersAtlasCostPerMeter;
            set => huntersAtlasCostPerMeter.Value = value;
        }

        /// <summary>
        /// Gets or sets how mush damage Scp106 will dealt when attacking a player.
        /// </summary>
        [EProperty(category: nameof(Scp106Role))]
        public int AttackDamage
        {
            get => Attack._damage;
            set => Attack._damage = value;
        }

        /// <summary>
        /// Gets or sets the amount of time in between player captures.
        /// </summary>
        [EProperty(category: nameof(Scp106Role))]
        public float CaptureCooldown
        {
            get => Attack._hitCooldown;
            set
            {
                Attack._hitCooldown = value;
                Attack.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets or sets the Sinkhole cooldown.
        /// </summary>
        [EProperty(category: nameof(Scp106Role))]
        public float RemainingSinkholeCooldown
        {
            get => SinkholeController.Cooldown.Remaining;
            set
            {
                SinkholeController.Cooldown.Remaining = value;
                SinkholeController.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-106 will enter his stalking mode.
        /// </summary>
        [EProperty(category: nameof(Scp106Role))]
        public bool IsStalking
        {
            get => StalkAbility.IsActive;
            set => StalkAbility.IsActive = value;
        }

        /// <summary>
        /// Gets the <see cref="Scp106GameRole"/>.
        /// </summary>
        public new Scp106GameRole Base { get; }

        /// <summary>
        /// Forces SCP-106 to use its portal, and Teleport to position.
        /// </summary>
        /// <param name="position">Where the player will be teleported.</param>
        /// <param name="cost">The amount of vigor that is required and will be consumed.</param>
        /// <returns>If the player will be teleport.</returns>
        public bool UsePortal(Vector3 position, float cost = 0f)
        {
            if (Room.Get(position) is not Room room)
                return false;

            HuntersAtlasAbility._syncRoom = room.Identifier;
            HuntersAtlasAbility._syncPos = position;

            if (Vigor < cost)
                return false;

            HuntersAtlasAbility._estimatedCost = cost;
            HuntersAtlasAbility.SetSubmerged(true);

            return true;
        }

        /// <summary>
        /// Send a player to the pocket dimension.
        /// </summary>
        /// <param name="player">The <see cref="Player"/>to send.</param>
        /// <returns>If the player has been capture in PocketDimension.</returns>
        public bool CapturePlayer(Player player)
        {
            if (player is null)
                return false;
            Attack._targetHub = player.ReferenceHub;
            DamageHandlerBase handler = new ScpDamageHandler(Attack.Owner, AttackDamage, DeathTranslations.PocketDecay);

            if (!Attack._targetHub.playerStats.DealDamage(handler))
                return false;

            Attack.SendCooldown(Attack._hitCooldown);
            Vigor += VigorCaptureReward;
            Attack.ReduceSinkholeCooldown();
            Hitmarker.SendHitmarkerDirectly(Attack.Owner, 1f);

            player.EnableEffect(EffectType.PocketCorroding);
            return true;
        }

        /// <summary>
        /// Gets the Spawn Chance of SCP-106.
        /// </summary>
        /// <param name="alreadySpawned">The List of Roles already spawned.</param>
        /// <returns>The Spawn Chance.</returns>
        public float GetSpawnChance(List<RoleTypeId> alreadySpawned) => Base.GetSpawnChance(alreadySpawned);
    }
}