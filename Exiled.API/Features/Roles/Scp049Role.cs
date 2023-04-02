// -----------------------------------------------------------------------
// <copyright file="Scp049Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System.Collections.Generic;

    using CustomPlayerEffects;
    using Exiled.API.Enums;

    using PlayerRoles;
    using PlayerRoles.PlayableScps.HumeShield;
    using PlayerRoles.PlayableScps.Scp049;
    using PlayerRoles.PlayableScps.Subroutines;
    using PlayerStatsSystem;

    using UnityEngine;

    using Scp049GameRole = PlayerRoles.PlayableScps.Scp049.Scp049Role;

    /// <summary>
    /// Defines a role that represents SCP-049.
    /// </summary>
    public class Scp049Role : FpcRole, ISubroutinedScpRole, IHumeShieldRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp049Role"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="Scp049GameRole"/>.</param>
        internal Scp049Role(Scp049GameRole baseRole)
            : base(baseRole)
        {
            SubroutineModule = baseRole.SubroutineModule;
            HumeShieldModule = baseRole.HumeShieldModule;

            if (!SubroutineModule.TryGetSubroutine(out Scp049ResurrectAbility scp049ResurrectAbility))
                Log.Error("Scp049ResurrectAbility subroutine not found in Scp049Role::ctor");

            ResurrectAbility = scp049ResurrectAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp049CallAbility scp049CallAbility))
                Log.Error("Scp049CallAbility subroutine not found in Scp049Role::ctor");

            CallAbility = scp049CallAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp049SenseAbility scp049SenseAbility))
                Log.Error("Scp049SenseAbility subroutine not found in Scp049Role::ctor");

            SenseAbility = scp049SenseAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp049AttackAbility scp049AttackAbility))
                Log.Error("Scp049AttackAbility subroutine not found in Scp049Role::ctor");

            AttackAbility = scp049AttackAbility;
        }

        /// <summary>
        /// Gets a list of players who will be turned away from SCP-049 Sense Ability.
        /// </summary>
        public static HashSet<Player> TurnedPlayers { get; } = new(20);

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Scp049;

        /// <inheritdoc/>
        public SubroutineManagerModule SubroutineModule { get; }

        /// <inheritdoc/>
        public HumeShieldModuleBase HumeShieldModule { get; }

        /// <summary>
        /// Gets SCP-049's <see cref="Scp049ResurrectAbility"/>.
        /// </summary>
        public Scp049ResurrectAbility ResurrectAbility { get; }

        /// <summary>
        /// Gets SCP-049's <see cref="Scp049AttackAbility"/>.
        /// </summary>
        public Scp049AttackAbility AttackAbility { get; }

        /// <summary>
        /// Gets SCP-049's <see cref="Scp049CallAbility"/>.
        /// </summary>
        public Scp049CallAbility CallAbility { get; }

        /// <summary>
        /// Gets SCP-049's <see cref="Scp049SenseAbility"/>.
        /// </summary>
        public Scp049SenseAbility SenseAbility { get; }

        /// <summary>
        /// Gets a value indicating whether or not SCP-049 is currently recalling a player.
        /// </summary>
        public bool IsRecalling => ResurrectAbility.IsInProgress;

        /// <summary>
        /// Gets a value indicating whether or not SCP-049's "Doctor's Call" ability is currently active.
        /// </summary>
        public bool IsCallActive => CallAbility.IsMarkerShown;

        /// <summary>
        /// Gets the player that is currently being revived by SCP-049. Will be <see langword="null"/> if <see cref="IsRecalling"/> is <see langword="false"/>.
        /// </summary>
        public Player RecallingPlayer => ResurrectAbility.CurRagdoll == null ? null : Player.Get(ResurrectAbility.CurRagdoll.Info.OwnerHub);

        /// <summary>
        /// Gets the ragdoll that is currently being revived by SCP-049. Will be <see langword="null"/> if <see cref="IsRecalling"/> is <see langword="false"/>.
        /// </summary>
        public Ragdoll RecallingRagdoll => Ragdoll.Get(ResurrectAbility.CurRagdoll);

        /// <summary>
        /// Gets all the dead zombies.
        /// </summary>
        public HashSet<uint> DeadZombies => Scp049ResurrectAbility.DeadZombies;

        /// <summary>
        /// Gets all the resurrected players.
        /// </summary>
        public Dictionary<uint, int> ResurrectedPlayers => Scp049ResurrectAbility.ResurrectedPlayers;

        /// <summary>
        /// Gets or sets the amount of time before SCP-049 can use its Doctor's Call ability again.
        /// </summary>
        public float CallCooldown
        {
            get => CallAbility.Cooldown.Remaining;
            set
            {
                CallAbility.Cooldown.Remaining = value;
                CallAbility.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-049 can use its Good Sense ability again.
        /// </summary>
        public float GoodSenseCooldown
        {
            get => SenseAbility.Cooldown.Remaining;
            set
            {
                SenseAbility.Cooldown.Remaining = value;
                SenseAbility.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets or sets the amount of time before SCP-049 can attack again.
        /// </summary>
        public float AttackCooldown
        {
            get => AttackAbility.Cooldown.Remaining;
            set
            {
                AttackAbility.Cooldown.Remaining = value;
                AttackAbility.ServerSendRpc(true);
            }
        }

        /// <summary>
        /// Gets or sets the distance of the Sense Ability.
        /// </summary>
        public float SenseDistance
        {
            get => SenseAbility._distanceThreshold;
            set => SenseAbility._distanceThreshold = value;
        }

        /// <summary>
        /// Lose the actual target of the SCP-049 Sense Ability.
        /// </summary>
        public void LoseSenseTarget() => SenseAbility.ServerLoseTarget();

        /// <summary>
        /// Resurrects a player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/>to resurrect.</param>
        public void Resurrect(Player player)
        {
            player.ReferenceHub.transform.position = ResurrectAbility.ScpRole.FpcModule.Position;

            HumeShieldModuleBase humeShield = ResurrectAbility.ScpRole.HumeShieldModule;
            humeShield.HsCurrent = Mathf.Min(humeShield.HsCurrent + 100f, humeShield.HsMax);
            Set(RoleTypeId.Scp0492, Enums.SpawnReason.Revived);
        }

        /// <summary>
        /// Resurrects a <see cref="Ragdoll"/> owner.
        /// </summary>
        /// <param name="ragdoll">The Ragdoll to resurrect.</param>
        public void Resurrect(Ragdoll ragdoll)
        {
            Resurrect(ragdoll.Owner);
            ragdoll.Destroy();
        }

        /// <summary>
        /// Attacks a Player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/>to attack.</param>
        public void Attack(Player player)
        {
            AttackAbility._target = player.ReferenceHub;

            if (AttackAbility._target is null || !AttackAbility.IsTargetValid(AttackAbility._target))
                return;

            AttackAbility.Cooldown.Trigger(Scp049AttackAbility.CooldownTime);
            CardiacArrest cardiacArrest = AttackAbility._target.playerEffectsController.GetEffect<CardiacArrest>();

            if (cardiacArrest.IsEnabled)
            {
                AttackAbility._target.playerStats.DealDamage(new Scp049DamageHandler(AttackAbility.Owner, -1f, Scp049DamageHandler.AttackType.Instakill));
            }
            else
            {
                cardiacArrest.SetAttacker(AttackAbility.Owner);
                cardiacArrest.Intensity = 1;
                cardiacArrest.ServerChangeDuration(AttackAbility._statusEffectDuration, false);
            }

            SenseAbility.HasTarget = false;
            SenseAbility.Cooldown.Trigger(20f);
            SenseAbility.ServerSendRpc(true);

            AttackAbility.ServerSendRpc(true);
            Hitmarker.SendHitmarker(AttackAbility.Owner, 1f);
        }

        /// <summary>
        /// Refresh the <see cref="Scp049CallAbility"/> duration.
        /// </summary>
        public void RefreshCallDuration() => CallAbility.ServerRefreshDuration();

        /// <summary>
        /// Returns a <see langword="bool"/> indicating whether or not the ragdoll can be resurrected by SCP-049.
        /// </summary>
        /// <param name="ragdoll">The ragdoll to check.</param>
        /// <returns><see langword="true"/> if the body can be revived; otherwise, <see langword="false"/>.</returns>
        public bool CanResurrect(BasicRagdoll ragdoll) => ResurrectAbility.CheckRagdoll(ragdoll);

        /// <summary>
        /// Returns a <see langword="bool"/> indicating whether or not the ragdoll can be resurrected by SCP-049.
        /// </summary>
        /// <param name="ragdoll">The ragdoll to check.</param>
        /// <returns><see langword="true"/> if the body can be revived; otherwise, <see langword="false"/>.</returns>
        public bool CanResurrect(Ragdoll ragdoll) => ResurrectAbility.CheckRagdoll(ragdoll.Base);

        /// <summary>
        /// Returns a <see langword="bool"/> indicating whether or not SCP-049 is close enough to a ragdoll to revive it.
        /// </summary>
        /// <remarks>This method only returns whether or not SCP-049 is close enough to the body to revive it; the body may have expired. Make sure to check <see cref="CanResurrect(BasicRagdoll)"/> to ensure the body can be revived.</remarks>
        /// <param name="ragdoll">The ragdoll to check.</param>
        /// <returns><see langword="true"/> if close enough to revive the body; otherwise, <see langword="false"/>.</returns>
        public bool IsInRecallRange(BasicRagdoll ragdoll) => ResurrectAbility.IsCloseEnough(Owner.Position, ragdoll.transform.position);

        /// <summary>
        /// Returns a <see langword="bool"/> indicating whether or not SCP-049 is close enough to a ragdoll to revive it.
        /// </summary>
        /// <remarks>This method only returns whether or not SCP-049 is close enough to the body to revive it; the body may have expired. Make sure to check <see cref="CanResurrect(Ragdoll)"/> to ensure the body can be revived.</remarks>
        /// <param name="ragdoll">The ragdoll to check.</param>
        /// <returns><see langword="true"/> if close enough to revive the body; otherwise, <see langword="false"/>.</returns>
        public bool IsInRecallRange(Ragdoll ragdoll) => IsInRecallRange(ragdoll.Base);
    }
}