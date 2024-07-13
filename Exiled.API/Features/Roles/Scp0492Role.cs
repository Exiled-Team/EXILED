// -----------------------------------------------------------------------
// <copyright file="Scp0492Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using Exiled.API.Features.Core.Attributes;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.HumeShield;
    using PlayerRoles.PlayableScps.Scp049;
    using PlayerRoles.PlayableScps.Scp049.Zombies;
    using PlayerRoles.Ragdolls;
    using PlayerRoles.Subroutines;
    using UnityEngine;

    /// <summary>
    /// Defines a role that represents SCP-049-2.
    /// </summary>
    public class Scp0492Role : FpcRole, ISubroutinedScpRole, IHumeShieldRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp0492Role"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="ZombieRole"/>.</param>
        internal Scp0492Role(ZombieRole baseRole)
            : base(baseRole)
        {
            SubroutineModule = baseRole.SubroutineModule;
            HumeShieldModule = baseRole.HumeShieldModule;

            if (!SubroutineModule.TryGetSubroutine(out ZombieAttackAbility zombieAttackAbility))
                Log.Error("ZombieAttackAbility subroutine not found in Scp0492Role::ctor");

            AttackAbility = zombieAttackAbility;

            if (!SubroutineModule.TryGetSubroutine(out ZombieBloodlustAbility zombieBloodlustAbility))
                Log.Error("ZombieBloodlustAbility subroutine not found in Scp0492Role::ctor");

            BloodlustAbility = zombieBloodlustAbility;

            if (!SubroutineModule.TryGetSubroutine(out ZombieConsumeAbility zombieConsumeAbility492))
                Log.Error("ZombieConsumeAbility subroutine not found in Scp0492Role::ctor");

            ConsumeAbility = zombieConsumeAbility492;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp0492Role"/> class.
        /// </summary>
        /// <param name="gameObject">The <see cref="GameObject"/>.</param>
        protected internal Scp0492Role(GameObject gameObject)
            : base(gameObject)
        {
        }

        /// <inheritdoc/>
        [EProperty(readOnly: true, category: nameof(Role))]
        public override RoleTypeId Type { get; } = RoleTypeId.Scp0492;

        /// <inheritdoc/>
        public SubroutineManagerModule SubroutineModule { get; }

        /// <inheritdoc/>
        public HumeShieldModuleBase HumeShieldModule { get; }

        /// <summary>
        /// Gets SCP-049-2's <see cref="ZombieAttackAbility"/>.
        /// </summary>
        public ZombieAttackAbility AttackAbility { get; }

        /// <summary>
        /// Gets SCP-049-2's <see cref="ZombieBloodlustAbility"/>.
        /// </summary>
        public ZombieBloodlustAbility BloodlustAbility { get; }

        /// <summary>
        /// Gets SCP-049-2's <see cref="ZombieConsumeAbility"/>.
        /// </summary>
        public ZombieConsumeAbility ConsumeAbility { get; }

        /// <summary>
        /// Gets or sets the amount of times this SCP-049-2 has been resurrected.
        /// </summary>
        [EProperty(category: nameof(Scp0492Role))]
        public int ResurrectNumber
        {
            get => Scp049ResurrectAbility.GetResurrectionsNumber(Owner.ReferenceHub);
            set => Scp049ResurrectAbility.ResurrectedPlayers[Owner.ReferenceHub.netId] = value;
        }

        /// <summary>
        /// Gets the SCP-049-2 attack damage.
        /// </summary>
        [EProperty(category: nameof(Scp0492Role))]
        public float AttackDamage => AttackAbility.DamageAmount;

        /// <summary>
        /// Gets or sets a value indicating the amount of time to simulate SCP-049-2's Bloodlust ability.
        /// </summary>
        [EProperty(category: nameof(Scp0492Role))]
        public float SimulatedStare
        {
            get => BloodlustAbility.SimulatedStare;
            set => BloodlustAbility.SimulatedStare = value;
        }

        /// <summary>
        /// Gets a value indicating whether or not SCP-049-2 is currently pursuing a target (Bloodlust ability).
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Scp0492Role))]
        public bool BloodlustActive => BloodlustAbility.LookingAtTarget;

        /// <summary>
        /// Gets a value indicating whether or not SCP-049-2 is consuming a ragdoll.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Scp0492Role))]
        public bool IsConsuming => ConsumeAbility.IsInProgress;

        /// <summary>
        /// Gets the <see cref="Ragdoll"/> that SCP-049-2 is currently consuming. Will be <see langword="null"/> if <see cref="IsConsuming"/> is <see langword="false"/>.
        /// </summary>
        public Ragdoll RagdollConsuming => Ragdoll.Get(ConsumeAbility.CurRagdoll);

        /// <summary>
        /// Gets the amount of time in between SCP-049-2 attacks.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Scp0492Role))]
        public float AttackCooldown => AttackAbility.BaseCooldown;

        /// <summary>
        /// Returns a <see langword="bool"/> indicating whether or not SCP-049-2 is close enough to a ragdoll to consume it.
        /// </summary>
        /// <remarks>This method only returns whether or not SCP-049-2 is close enough to the body to consume it; the body may have been consumed previously. Make sure to check <see cref="Ragdoll.IsConsumed"/> to ensure the body can be consumed.</remarks>
        /// <param name="ragdoll">The ragdoll to check.</param>
        /// <returns><see langword="true"/> if close enough to consume the body; otherwise, <see langword="false"/>.</returns>
        public bool IsInConsumeRange(BasicRagdoll ragdoll) => ragdoll != null && ConsumeAbility.IsCloseEnough(Owner.Position, ragdoll.transform.position);

        /// <summary>
        /// Returns a <see langword="bool"/> indicating whether or not SCP-049-2 is close enough to a ragdoll to consume it.
        /// </summary>
        /// <remarks>This method only returns whether or not SCP-049-2 is close enough to the body to consume it; the body may have been consumed previously. Make sure to check <see cref="Ragdoll.IsConsumed"/> to ensure the body can be consumed.</remarks>
        /// <param name="ragdoll">The ragdoll to check.</param>
        /// <returns><see langword="true"/> if close enough to consume the body; otherwise, <see langword="false"/>.</returns>
        public bool IsInConsumeRange(Ragdoll ragdoll) => ragdoll is not null && IsInConsumeRange(ragdoll.Base);
    }
}