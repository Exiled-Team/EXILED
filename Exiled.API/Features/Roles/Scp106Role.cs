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
    using Exiled.API.Features.DynamicPatch;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.HumeShield;
    using PlayerRoles.PlayableScps.Scp106;
    using PlayerRoles.PlayableScps.Subroutines;
    using PlayerStatsSystem;
    using UnityEngine;

    using Scp106GameRole = PlayerRoles.PlayableScps.Scp106.Scp106Role;

    /// <summary>
    /// Defines a role that represents SCP-106.
    /// </summary>
    public class Scp106Role : FpcRole, ISubroutinedScpRole, IHumeShieldRole
    {
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
        /// Gets the <see cref="Scp106Vigor"/>.
        /// </summary>
        public Scp106Vigor VigorComponent => VigorAbility.Vigor;

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
        public Scp106MovementModule MovementModule { get; }

        /// <summary>
        /// Gets or sets SCP-106's Vigor.
        /// </summary>
        public float Vigor
        {
            get => VigorAbility.VigorAmount;
            set => VigorAbility.VigorAmount = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-106 is currently submerged.
        /// </summary>
        public bool IsSubmerged
        {
            get => Base.IsSubmerged;
            set => HuntersAtlasAbility.SetSubmerged(value);
        }

        /// <summary>
        /// Gets a value indicating whether or not SCP-106 can activate teslas.
        /// </summary>
        public bool CanActivateTesla => Base.CanActivateShock;

        /// <summary>
        /// Gets a value indicating whether if SCP-106 <see cref="Scp106StalkAbility"/> can be cleared.
        /// </summary>
        public bool CanStopStalk => StalkAbility.CanBeCleared;

        /// <summary>
        /// Gets a value indicating whether or not SCP-106 is currently slow down by a door.
        /// </summary>
        public bool IsSlowdown => MovementModule._slowndownTarget is < 1;

        /// <summary>
        /// Gets a value indicating the current time of the sinkhole.
        /// </summary>
        public float SinkholeCurrentTime => SinkholeController.ElapsedToggle;

        /// <summary>
        /// Gets a value indicating the normalized state of the sinkhole.
        /// </summary>
        public float SinkholeNormalizedState => SinkholeController.NormalizedState;

        /// <summary>
        /// Gets a value indicating whether or not SCP-106 is currently in the middle of an animation.
        /// </summary>
        public bool IsDuringAnimation => SinkholeController.IsDuringAnimation;

        /// <summary>
        /// Gets a value indicating whether or not SCP-106 sinkhole is hidden.
        /// </summary>
        public bool IsSinkholeHidden => SinkholeController.IsHidden;

        /// <summary>
        /// Gets or sets a value indicating whether the current sinkhole state.
        /// </summary>
        public bool SinkholeState
        {
            get => SinkholeController.State;
            set => SinkholeController.State = value;
        }

        /// <summary>
        /// Gets the sinkhole target duration.
        /// </summary>
        public float SinkholeTargetDuration => SinkholeController.TargetDuration;

        /// <summary>
        /// Gets the speed multiplier of the sinkhole.
        /// </summary>
        public float SinkholeSpeedMultiplier => SinkholeController.SpeedMultiplier;

        /// <summary>
        /// Gets or sets how mush cost the Ability Stalk will cost per tick when being stationary.
        /// </summary>
        public Property<float> VigorStalkCostStationary { get; set; } = new(Scp106StalkAbility.VigorStalkCostStationary);

        /// <summary>
        /// Gets or sets how mush cost the Ability Stalk will cost per tick when moving.
        /// </summary>
        public Property<float> VigorStalkCostMoving { get; set; } = new(Scp106StalkAbility.VigorStalkCostMoving);

        /// <summary>
        /// Gets or sets how mush vigor will be regenerate while moving per seconds.
        /// </summary>
        public Property<float> VigorRegeneration { get; set; } = new(Scp106StalkAbility.VigorRegeneration);

        /// <summary>
        /// Gets or sets how mush damage Scp106 will dealt when attacking a player.
        /// </summary>
        public Property<float> AttackDamage { get; set; } = new(Scp106Attack.AttackDamage);

        /// <summary>
        /// Gets or sets the duration of Corroding effect.
        /// </summary>
        public Property<float> CorrodingTime { get; set; } = new(Scp106Attack.CorrodingTime);

        /// <summary>
        /// Gets or sets how mush vigor Scp106 will gain when being reward for having caught a player.
        /// </summary>
        public Property<float> VigorCaptureReward { get; set; } = new(Scp106Attack.VigorCaptureReward);

        /// <summary>
        /// Gets or sets how mush reduction cooldown Scp106 will gain when being reward for having caught a player.
        /// </summary>
        public Property<float> CooldownReductionReward { get; set; } = new(Scp106Attack.CooldownReductionReward);

        /// <summary>
        /// Gets or sets the cooldown duration of it's Sinkhole ability's.
        /// </summary>
        public Property<float> SinkholeCooldownDuration { get; set; } = new(Scp106SinkholeController.CooldownDuration);

        /// <summary>
        /// Gets or sets how mush vigor it's ability Hunter Atlas will cost per meter.
        /// </summary>
        public Property<float> HuntersAtlasCostPerMeter { get; set; } = new(Scp106HuntersAtlasAbility.CostPerMeter);

        /// <summary>
        /// Gets or sets the amount of time in between player captures.
        /// </summary>
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
        public void CapturePlayer(Player player) // Convert to bool.
        {
            if (player is null)
                return;
            Attack._targetHub = player.ReferenceHub;
            DamageHandlerBase handler = new ScpDamageHandler(Attack.Owner, AttackDamage, DeathTranslations.PocketDecay);

            if (!Attack._targetHub.playerStats.DealDamage(handler))
                return;

            Attack.SendCooldown(Attack._hitCooldown);
            Vigor += VigorCaptureReward;
            Attack.ReduceSinkholeCooldown();
            Hitmarker.SendHitmarker(Attack.Owner, 1f);

            player.EnableEffect(EffectType.PocketCorroding);
        }

        /// <summary>
        /// Gets the Spawn Chance of SCP-106.
        /// </summary>
        /// <param name="alreadySpawned">The List of Roles already spawned.</param>
        /// <returns>The Spawn Chance.</returns>
        public float GetSpawnChance(List<RoleTypeId> alreadySpawned) => Base.GetSpawnChance(alreadySpawned);
    }
}