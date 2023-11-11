// -----------------------------------------------------------------------
// <copyright file="Scp3114Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features.Hazards;
    using Exiled.API.Interfaces;
    using Mirror;
    using PlayerRoles;
    using PlayerRoles.PlayableScps;
    using PlayerRoles.PlayableScps.HumeShield;
    using PlayerRoles.PlayableScps.Scp3114;
    using PlayerRoles.PlayableScps.Subroutines;
    using PlayerRoles.Ragdolls;
    using UnityEngine;

    using static PlayerRoles.PlayableScps.Scp3114.Scp3114Identity;

    using Scp3114GameRole = PlayerRoles.PlayableScps.Scp3114.Scp3114Role;

    /// <summary>
    /// Defines a role that represents SCP-3114.
    /// </summary>
    public class Scp3114Role : FpcRole, ISubroutinedScpRole, IHumeShieldRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp3114Role"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="Scp3114GameRole"/>.</param>
        internal Scp3114Role(Scp3114GameRole baseRole)
            : base(baseRole)
        {
            Base = baseRole;
            SubroutineModule = baseRole.SubroutineModule;
            HumeShieldModule = baseRole.HumeShieldModule;

            if (!SubroutineModule.TryGetSubroutine(out Scp3114Slap scp3114Slap))
                Log.Error("Scp3114Slap not found in Scp3114Role::ctor");

            Slap = scp3114Slap;

            if (!SubroutineModule.TryGetSubroutine(out Scp3114Dance scp3114Dance))
                Log.Error("Scp3114Dance not found in Scp3114Role::ctor");

            Dance = scp3114Dance;

            if (!SubroutineModule.TryGetSubroutine(out Scp3114Reveal scp3114Reveal))
                Log.Error("Scp3114Reveal not found in Scp3114Role::ctor");

            Reveal = scp3114Reveal;

            if (!SubroutineModule.TryGetSubroutine(out Scp3114Identity scp3114Identity))
                Log.Error("Scp3114Identity not found in Scp3114Role::ctor");

            Identity = scp3114Identity;

            if (!SubroutineModule.TryGetSubroutine(out Scp3114History scp3114History))
                Log.Error("Scp3114History not found in Scp3114Role::ctor");

            History = scp3114History;

            if (!SubroutineModule.TryGetSubroutine(out Scp3114FakeModelManager scp3114FakeModelManager))
                Log.Error("Scp3114FakeModelManager not found in Scp3114Role::ctor");

            FakeModelManager = scp3114FakeModelManager;

            if (!SubroutineModule.TryGetSubroutine(out Scp3114Disguise scp3114Disguise))
                Log.Error("Scp3114Disguise not found in Scp3114Role::ctor");

            Disguise = scp3114Disguise;

            if (!SubroutineModule.TryGetSubroutine(out Scp3114VoiceLines scp3114VoiceLines))
                Log.Error("Scp3114VoiceLines not found in Scp3114Role::ctor");

            VoiceLines = scp3114VoiceLines;
        }

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Scp3114;

        /// <inheritdoc/>
        public SubroutineManagerModule SubroutineModule { get; }

        /// <inheritdoc/>
        public HumeShieldModuleBase HumeShieldModule { get; }

        /// <summary>
        /// Gets Scp3114's <see cref="Scp3114Slap"/>.
        /// </summary>
        public Scp3114Slap Slap { get; }

        /// <summary>
        /// Gets Scp3114's <see cref="Scp3114Dance"/>.
        /// </summary>
        public Scp3114Dance Dance { get; }

        /// <summary>
        /// Gets Scp3114's <see cref="Scp3114Reveal"/>.
        /// </summary>
        public Scp3114Reveal Reveal { get; }

        /// <summary>
        /// Gets Scp3114's <see cref="Scp3114Identity"/>.
        /// </summary>
        public Scp3114Identity Identity { get; }

        /// <summary>
        /// Gets Scp3114's <see cref="Scp3114History"/>.
        /// </summary>
        public Scp3114History History { get; }

        /// <summary>
        /// Gets Scp3114's <see cref="Scp3114FakeModelManager"/>.
        /// </summary>
        public Scp3114FakeModelManager FakeModelManager { get; }

        /// <summary>
        /// Gets Scp3114's <see cref="Scp3114Disguise"/>.
        /// </summary>
        public Scp3114Disguise Disguise { get; }

        /// <summary>
        /// Gets Scp3114's <see cref="Scp3114VoiceLines"/>.
        /// </summary>
        public Scp3114VoiceLines VoiceLines { get; }

        /// <summary>
        /// Gets the <see cref="Scp3114GameRole"/> instance.
        /// </summary>
        public new Scp3114GameRole Base { get; }

        /// <summary>
        /// Gets the damage amount of SCP-3114's slap ability.
        /// </summary>
        public float SlapDamage => Slap.DamageAmount;

        /// <summary>
        /// Gets the current target of SCP-3114's strangle ability. Can be <see langword="null"/>.
        /// </summary>
        public Player StrangleTarget => Player.Get(Slap._strangle.SyncTarget?.Target);

        /// <summary>
        /// Gets or sets the SCP-3114's Stolen Role.
        /// </summary>
        public RoleTypeId StolenRole
        {
            get => Identity.CurIdentity.StolenRole;
            set
            {
                if (Ragdoll is null)
                    return;

                Ragdoll.Role = value;
                Identity.ServerResendIdentity();
            }
        }

        /// <summary>
        /// Gets or sets the SCP-3114's Ragdoll used for it's FakeIdentity.
        /// </summary>
        public Ragdoll Ragdoll
        {
            get => Ragdoll.Get(Identity.CurIdentity.Ragdoll);
            set
            {
                Identity.CurIdentity.Ragdoll = value?.Base;
                Identity.ServerResendIdentity();
            }
        }

        /// <summary>
        /// Gets or sets the SCP-3114's UnitId used for it's FakeIdentity.
        /// </summary>
        public byte UnitId
        {
            get => Identity.CurIdentity.UnitNameId;
            set
            {
                Identity.CurIdentity.UnitNameId = value;
                Identity.ServerResendIdentity();
            }
        }

        /// <summary>
        /// Gets or sets current state of SCP-3114's disguise ability.
        /// </summary>
        public DisguiseStatus DisguiseStatus
        {
            get => Identity.CurIdentity.Status;
            set
            {
                Identity.CurIdentity.Status = value;
                Identity.ServerResendIdentity();
            }
        }

        /// <summary>
        /// Gets or sets the SCP-3114's Disguise duration.
        /// </summary>
        public float DisguiseDuration
        {
            get => Identity._disguiseDurationSeconds;
            set => Identity._disguiseDurationSeconds = value;
        }

        /// <summary>
        /// Gets or sets the warning time seconds.
        /// </summary>
        public float WarningTime
        {
            get => Identity._warningTimeSeconds;
            set => Identity._warningTimeSeconds = value;
        }

        /// <summary>
        /// Reset Scp3114 FakeIdentity.
        /// </summary>
        public void ResetIdentity()
        {
            Identity.CurIdentity.Reset();
            Identity.ServerResendIdentity();
        }

        /// <summary>
        /// Plays a random Scp3114 voice line.
        /// </summary>
        /// <param name="voiceLine">The type of voice line to play.</param>
        public void PlaySound(Scp3114VoiceLines.VoiceLinesName voiceLine = Scp3114VoiceLines.VoiceLinesName.RandomIdle)
            => VoiceLines.ServerPlayConditionally(voiceLine);

        /// <summary>
        /// Gets the Spawn Chance of SCP-3114.
        /// </summary>
        /// <param name="alreadySpawned">The List of Roles already spawned.</param>
        /// <returns>The Spawn Chance.</returns>
        public float GetSpawnChance(List<RoleTypeId> alreadySpawned) => Base is ISpawnableScp spawnableScp ? spawnableScp.GetSpawnChance(alreadySpawned) : 0;
    }
}