// -----------------------------------------------------------------------
// <copyright file="Scp096Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Exiled.API.Features.Core.Attributes;
    using PlayerRoles;
    using PlayerRoles.PlayableScps;
    using PlayerRoles.PlayableScps.HumeShield;
    using PlayerRoles.PlayableScps.Scp096;
    using PlayerRoles.Subroutines;
    using UnityEngine;

    using Scp096GameRole = PlayerRoles.PlayableScps.Scp096.Scp096Role;

    /// <summary>
    /// Defines a role that represents SCP-096.
    /// </summary>
    [DebuggerDisplay("Scp-096")]
    public class Scp096Role : FpcRole, ISubroutinedScpRole, IHumeShieldRole, ISpawnableScp
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp096Role"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="Scp096GameRole"/>.</param>
        internal Scp096Role(Scp096GameRole baseRole)
            : base(baseRole)
        {
            SubroutineModule = baseRole.SubroutineModule;
            HumeShieldModule = baseRole.HumeShieldModule;
            Base = baseRole;

            if (!SubroutineModule.TryGetSubroutine(out Scp096RageCycleAbility scp096RageCycleAbility))
                Log.Error("RageCycleAbility subroutine not found in Scp096Role::ctor");

            RageCycleAbility = scp096RageCycleAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp096RageManager scp096RageManager))
                Log.Error("RageManager subroutine not found in Scp096Role::ctor");

            RageManager = scp096RageManager;

            if (!SubroutineModule.TryGetSubroutine(out Scp096TargetsTracker scp096TargetsTracker))
                Log.Error("TargetsTracker not found in Scp096Role::ctor");

            TargetsTracker = scp096TargetsTracker;

            if (!SubroutineModule.TryGetSubroutine(out Scp096AttackAbility scp096AttackAbility))
                Log.Error("AttackAbility not found in Scp096Role::ctor");

            AttackAbility = scp096AttackAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp096TryNotToCryAbility scp096TryNotToCryAbility))
                Log.Error("TryNotToCryAbility not found in Scp096Role::ctor");

            TryNotToCryAbility = scp096TryNotToCryAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp096ChargeAbility scp096ChargeAbility))
                Log.Error("ChargeAbility not found in Scp096Role::ctor");

            ChargeAbility = scp096ChargeAbility;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp096Role"/> class.
        /// </summary>
        /// <param name="gameObject">The <see cref="GameObject"/>.</param>
        protected internal Scp096Role(GameObject gameObject)
            : base(gameObject)
        {
        }

        /// <summary>
        /// Gets a list of players who will be turned away from SCP-096.
        /// </summary>
        public static HashSet<Player> TurnedPlayers { get; } = new(20);

        /// <inheritdoc/>
        [EProperty(readOnly: true, category: nameof(Role))]
        public override RoleTypeId Type { get; } = RoleTypeId.Scp096;

        /// <inheritdoc/>
        public SubroutineManagerModule SubroutineModule { get; }

        /// <inheritdoc/>
        public HumeShieldModuleBase HumeShieldModule { get; }

        /// <summary>
        /// Gets SCP-096's <see cref="Scp096RageCycleAbility"/>.
        /// </summary>
        public Scp096RageCycleAbility RageCycleAbility { get; }

        /// <summary>
        /// Gets SCP-096's <see cref="Scp096RageManager"/>.
        /// </summary>
        public Scp096RageManager RageManager { get; }

        /// <summary>
        /// Gets SCP-096's <see cref="Scp096TargetsTracker"/>.
        /// </summary>
        public Scp096TargetsTracker TargetsTracker { get; }

        /// <summary>
        /// Gets SCP-096's <see cref="Scp096AttackAbility"/>.
        /// </summary>
        public Scp096AttackAbility AttackAbility { get; }

        /// <summary>
        /// Gets SCP-096's <see cref="Scp096TryNotToCryAbility"/>.
        /// </summary>
        public Scp096TryNotToCryAbility TryNotToCryAbility { get; }

        /// <summary>
        /// Gets SCP-096's <see cref="Scp096ChargeAbility"/>.
        /// </summary>
        public Scp096ChargeAbility ChargeAbility { get; }

        /// <summary>
        /// Gets a value indicating SCP-096's ability state.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Scp096Role))]
        public Scp096AbilityState AbilityState => Base.StateController.AbilityState;

        /// <summary>
        /// Gets a value indicating SCP-096's rage state.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Scp096Role))]
        public Scp096RageState RageState => Base.StateController.RageState;

        /// <summary>
        /// Gets a value indicating whether or not SCP-096 can receive targets.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Scp096Role))]
        public bool CanReceiveTargets => RageCycleAbility._targetsTracker.CanReceiveTargets;

        /// <summary>
        /// Gets a value indicating whether or not SCP-096 can attack.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Scp096Role))]
        public bool AttackPossible => AttackAbility.AttackPossible;

        /// <summary>
        /// Gets or sets the Charge Ability Cooldown.
        /// </summary>
        [EProperty(category: nameof(Scp096Role))]
        public float ChargeCooldown
        {
            get => ChargeAbility.Cooldown.Remaining;
            set
            {
                ChargeAbility.Cooldown.Remaining = value;
                ChargeAbility.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets or sets the Charge Ability duration.
        /// </summary>
        [EProperty(category: nameof(Scp096Role))]
        public float RemainingChargeDuration
        {
            get => ChargeAbility.Duration.Remaining;
            set
            {
                ChargeAbility.Duration.Remaining = value;
                ChargeAbility.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-096 can be enraged again.
        /// </summary>
        [EProperty(category: nameof(Scp096Role))]
        public float EnrageCooldown
        {
            get => RageCycleAbility._activationTime.Remaining;
            set
            {
                RageCycleAbility._activationTime.Remaining = value;
                RageCycleAbility.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets or sets enraged time left.
        /// </summary>
        [EProperty(category: nameof(Scp096Role))]
        public float EnragedTimeLeft
        {
            get => RageManager.EnragedTimeLeft;
            set
            {
                RageManager.EnragedTimeLeft = value;
                RageManager.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets or sets enraged time left.
        /// </summary>
        [EProperty(category: nameof(Scp096Role))]
        public float TotalEnrageTime
        {
            get => RageManager.TotalRageTime;
            set
            {
                RageManager.TotalRageTime = value;
                RageManager.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the TryNotToCry ability is active.
        /// </summary>
        [EProperty(category: nameof(Scp096Role))]
        public bool TryNotToCryActive
        {
            get => TryNotToCryAbility.IsActive;
            set => TryNotToCryAbility.IsActive = value;
        }

        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}"/> of Players that are currently targeted by SCP-096.
        /// </summary>
        public IReadOnlyCollection<Player> Targets => RageCycleAbility._targetsTracker.Targets.Select(Player.Get).ToList().AsReadOnly();

        /// <summary>
        /// Gets the <see cref="Scp096GameRole"/>.
        /// </summary>
        public new Scp096GameRole Base { get; }

        /// <summary>
        /// Adds the specified <paramref name="player"/> as an SCP-096 target.
        /// </summary>
        /// <param name="player">The player to add as a target.</param>
        /// <returns><see langword="true"/> if target was successfully added. Otherwise, <see langword="false"/>.</returns>
        public bool AddTarget(Player player) => player is not null && TargetsTracker.AddTarget(player.ReferenceHub, false);

        /// <summary>
        /// Adds the specified <paramref name="player"/> as an SCP-096 target.
        /// </summary>
        /// <param name="player">The player to add as a target.</param>
        /// <param name="isLooking">Is because player look SCP-096.</param>
        /// <returns><see langword="true"/> if target was successfully added. Otherwise, <see langword="false"/>.</returns>
        public bool AddTarget(Player player, bool isLooking) => player is not null && TargetsTracker.AddTarget(player.ReferenceHub, isLooking);

        /// <summary>
        /// Removes the specified <paramref name="player"/> from SCP-096's targets.
        /// </summary>
        /// <param name="player">The player to remove as a target.</param>
        /// <returns><see langword="true"/> if target was successfully removed. Otherwise, <see langword="false"/>.</returns>
        public bool RemoveTarget(Player player) => player is not null && TargetsTracker.RemoveTarget(player.ReferenceHub);

        /// <summary>
        /// Enrages SCP-096 for the given amount of times.
        /// </summary>
        /// <param name="time">The amount of time to enrage SCP-096.</param>
        public void Enrage(float time = Scp096RageManager.MinimumEnrageTime) => RageManager.ServerEnrage(time);

        /// <summary>
        /// Ends SCP-096's enrage cycle.
        /// </summary>
        /// <param name="clearTime">Whether or not to clear the remaining enrage time.</param>
        public void Calm(bool clearTime = true) => RageManager.ServerEndEnrage(clearTime);

        /// <summary>
        /// Returns whether or not the provided <paramref name="player"/> is a target of SCP-096.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <returns>Whether or not the player is a target of SCP-096.</returns>
        public bool HasTarget(Player player) => player is not null && TargetsTracker.HasTarget(player.ReferenceHub);

        /// <summary>
        /// Returns whether or not the provided <paramref name="player"/> is observed by SCP-096.
        /// </summary>
        /// <param name="player">The player to check.</param>
        /// <returns>Whether or not the player is observed.</returns>
        public bool IsObserved(Player player) => player is not null && TargetsTracker.IsObservedBy(player.ReferenceHub);

        /// <summary>
        /// Removes all targets from SCP-096's target list.
        /// </summary>
        public void ClearTargets() => TargetsTracker.ClearAllTargets();

        /// <summary>
        /// Trigger the attack ability.
        /// </summary>
        public void Attack() => AttackAbility.ServerAttack();

        /// <summary>
        /// Trigger the charge ability.
        /// </summary>
        /// <param name="cooldown">The cooldown time to set before the charge can be executed again.</param>
        public void Charge(float cooldown = 1f)
        {
            ChargeAbility._hitHandler.Clear();
            ChargeAbility.Duration.Trigger(cooldown);
            ChargeAbility.CastRole.StateController.SetAbilityState(Scp096AbilityState.Charging);
            ChargeAbility.ServerSendRpc(true);
        }

        /// <summary>
        /// Shows the input prompt for the RageCycle ability.
        /// </summary>
        /// <param name="duration">The input prompt duration.</param>
        public void ShowRageInput(float duration = Scp096RageCycleAbility.DefaultActivationDuration) => RageCycleAbility.ServerTryEnableInput(duration);

        /// <summary>
        /// Gets the Spawn Chance of SCP-096.
        /// </summary>
        /// <param name="alreadySpawned">The List of Roles already spawned.</param>
        /// <returns>The Spawn Chance.</returns>
        public float GetSpawnChance(List<RoleTypeId> alreadySpawned) => Base.GetSpawnChance(alreadySpawned);
    }
}