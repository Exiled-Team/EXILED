// -----------------------------------------------------------------------
// -----------------------------------------------------------------------
// <copyright file="Scp1507Ragdoll.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using Exiled.API.Extensions;
    using Exiled.API.Features.Roles;
    using Exiled.API.Interfaces;
    using PlayerRoles;

    using Scp1507BaseRagdoll = PlayerRoles.PlayableScps.Scp1507.Scp1507Ragdoll;

    /// <summary>
    /// A wrapper for <see cref="Scp1507BaseRagdoll"/>.
    /// </summary>
    public class Scp1507Ragdoll : Ragdoll, IWrapper<Scp1507BaseRagdoll>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp1507Ragdoll"/> class.
        /// </summary>
        /// <param name="ragdoll">The encapsulated <see cref="Scp1507BaseRagdoll"/>.</param>
        internal Scp1507Ragdoll(Scp1507BaseRagdoll ragdoll)
            : base(ragdoll)
        {
            Base = ragdoll;
        }

        /// <inheritdoc/>
        public new Scp1507BaseRagdoll Base { get; }

        /// <summary>
        /// Gets or sets current progress of revival process.
        /// </summary>
        public float RevivalProgress
        {
            get => Base._revivalProgress;
            set => Base.Network_revivalProgress = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this ragdoll has been revived.
        /// </summary>
        public bool IsRevived
        {
            get => Base._hasAlreadyRevived;
            set => Base.Network_hasAlreadyRevived = value;
        }

        /// <summary>
        /// Gets or sets amount of time when ragdoll was reset last time.
        /// </summary>
        public double ResetTime
        {
            get => Base._lastResetTime;
            set => Base.Network_lastResetTime = value;
        }

        /// <summary>
        /// Spawns a variant from available ragdolls for chosen role.
        /// </summary>
        /// <param name="role">Role. Can be <see cref="RoleTypeId.Flamingo"/>, <see cref="RoleTypeId.AlphaFlamingo"/> or <see cref="RoleTypeId.ZombieFlamingo"/>.</param>
        public void SpawnVariant(RoleTypeId role) => Base.SpawnVariant(role);

        /// <summary>
        /// Vocalizes ragdoll.
        /// </summary>
        /// <param name="player">Player who vocalizes. If <see langword="null"/>, will be chosen random.</param>
        public void Vocalize(Player player = null) => Base.OnVocalize((player ?? Player.Get(x => x.Role.Is(out Scp1507Role _)).Random()).ReferenceHub);
    }
}