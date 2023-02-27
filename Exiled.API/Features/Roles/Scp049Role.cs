// -----------------------------------------------------------------------
// <copyright file="Scp049Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using PlayerRoles;
    using PlayerRoles.PlayableScps.HumeShield;
    using PlayerRoles.PlayableScps.Scp049;
    using PlayerRoles.PlayableScps.Subroutines;

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
        }

        /// <inheritdoc/>
        public override RoleTypeId Type
        {
            get => RoleTypeId.Scp049;
            set => Set(value);
        }

        /// <inheritdoc/>
        public SubroutineManagerModule SubroutineModule { get; }

        /// <inheritdoc/>
        public HumeShieldModuleBase HumeShieldModule { get; }

        /// <summary>
        /// Gets SCP-049's <see cref="Scp049ResurrectAbility"/>.
        /// </summary>
        public Scp049ResurrectAbility ResurrectAbility { get; }

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