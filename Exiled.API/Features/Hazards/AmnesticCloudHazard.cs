// -----------------------------------------------------------------------
// <copyright file="AmnesticCloudHazard.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Hazards
{
    using Exiled.API.Extensions;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp939;

    using Scp939GameRole = PlayerRoles.PlayableScps.Scp939.Scp939Role;

    /// <summary>
    /// A wrapper for SCP-939's amnestic cloud.
    /// </summary>
    public class AmnesticCloudHazard : TemporaryHazard
    {
        private static Scp939AmnesticCloudInstance amnesticCloudPrefab;

        /// <summary>
        /// Initializes a new instance of the <see cref="AmnesticCloudHazard"/> class.
        /// </summary>
        /// <param name="hazard">The <see cref="Scp939AmnesticCloudInstance"/> instance.</param>
        public AmnesticCloudHazard(Scp939AmnesticCloudInstance hazard)
            : base(hazard)
        {
            Base = hazard;
            Ability = Base._cloud;
            Owner = Player.Get(Ability.Owner);
        }

        /// <summary>
        /// Gets the amnestic cloud prefab.
        /// </summary>
        public static Scp939AmnesticCloudInstance AmnesticCloudPrefab
        {
            get
            {
                if (amnesticCloudPrefab == null)
                {
                    Scp939GameRole scp939Role = (Scp939GameRole)RoleTypeId.Scp939.GetRoleBase();

                    if (scp939Role.SubroutineModule.TryGetSubroutine(out Scp939AmnesticCloudAbility ability))
                        amnesticCloudPrefab = ability._instancePrefab;
                }

                return amnesticCloudPrefab;
            }
        }

        /// <inheritdoc cref="Hazard.Base"/>
        public new Scp939AmnesticCloudInstance Base { get; }

        /// <summary>
        /// Gets the <see cref="Scp939AmnesticCloudAbility"/> for this instance.
        /// </summary>
        public Scp939AmnesticCloudAbility Ability { get; }

        /// <summary>
        /// Gets the player who controls SCP-939.
        /// </summary>
        public Player Owner { get; }

        /// <summary>
        /// Gets or sets current state of cloud.
        /// </summary>
        public Scp939AmnesticCloudInstance.CloudState State
        {
            get => Base.State;
            set => Base.State = value;
        }

        /// <summary>
        /// Gets or sets duration for effects.
        /// </summary>
        public float EffectDuration
        {
            get => Base._amnesiaDuration;
            set => Base._amnesiaDuration = value;
        }

        /// <summary>
        /// Gets or sets minimum time to press key to spawn cloud.
        /// </summary>
        public float MinHoldTime
        {
            get => Base._minHoldTime;
            set => Base._minHoldTime = value;
        }

        /// <summary>
        /// Gets or sets maximum time to press key to spawn cloud.
        /// </summary>
        public float MaxHoldTime
        {
            get => Base._maxHoldTime;
            set => Base._maxHoldTime = value;
        }

        /// <summary>
        /// Gets or sets total duration before hazard will get destroyed.
        /// </summary>
        public new float TotalDuration
        {
            get => Base._targetDuration;
            set => Base._targetDuration = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not hazard is active or not.
        /// </summary>
        public bool TargetState
        {
            get => Ability.TargetState;
            set => Ability.TargetState = value;
        }
    }
}