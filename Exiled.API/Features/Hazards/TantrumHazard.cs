// -----------------------------------------------------------------------
// <copyright file="TantrumHazard.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Hazards
{
    using CustomPlayerEffects;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using global::Hazards;
    using Mirror;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp173;
    using RelativePositioning;
    using UnityEngine;

    using Object = UnityEngine.Object;
    using Scp173GameRole = PlayerRoles.PlayableScps.Scp173.Scp173Role;

    /// <summary>
    /// A wrapper for <see cref="TantrumEnvironmentalHazard"/>.
    /// </summary>
    public class TantrumHazard : TemporaryHazard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TantrumHazard"/> class.
        /// </summary>
        /// <param name="hazard">The <see cref="TantrumEnvironmentalHazard"/> instance.</param>
        public TantrumHazard(TantrumEnvironmentalHazard hazard)
            : base(hazard)
        {
            Base = hazard;
        }

        /// <summary>
        /// Gets the tantrum prefab's type.
        /// </summary>
        public static PrefabType PrefabType => PrefabType.TantrumObj;

        /// <summary>
        /// Gets the tantrum cloud prefab's object.
        /// </summary>
        public static GameObject PrefabObject => PrefabHelper.PrefabToGameObject[PrefabType];

        /// <summary>
        /// Gets the <see cref="TantrumEnvironmentalHazard"/>.
        /// </summary>
        public new TantrumEnvironmentalHazard Base { get; }

        /// <inheritdoc />
        public override HazardType Type { get; } = HazardType.Tantrum;

        /// <summary>
        /// Gets or sets a value indicating whether or not sizzle should be played.
        /// </summary>
        public bool PlaySizzle
        {
            get => Base.PlaySizzle;
            set => Base.PlaySizzle = value;
        }

        /// <summary>
        /// Gets or sets the synced position.
        /// </summary>
        public RelativePosition SynchronisedPosition
        {
            get => Base.SynchronizedPosition;
            set => Base.SynchronizedPosition = value;
        }

        /// <summary>
        /// Gets or sets the correct position of tantrum hazard.
        /// </summary>
        public Transform CorrectPosition
        {
            get => Base._correctPosition;
            set => Base._correctPosition = value;
        }

        /// <summary>
        /// Places a Tantrum (SCP-173's ability) in the indicated position.
        /// </summary>
        /// <param name="position">The position where you want to spawn the Tantrum.</param>
        /// <param name="isActive">Whether or not the tantrum will apply the <see cref="Stained"/> effect.</param>
        /// <remarks>If <paramref name="isActive"/> is <see langword="true"/>, the tantrum is moved slightly up from its original position. Otherwise, the collision will not be detected and the slowness will not work.</remarks>
        /// <returns>The <see cref="TantrumHazard"/> instance.</returns>
        public static TantrumHazard CreateAndSpawn(Vector3 position, bool isActive = true)
        {
            TantrumEnvironmentalHazard tantrum = PrefabHelper.Spawn<TantrumEnvironmentalHazard>(PrefabType);

            if (!isActive)
                tantrum.SynchronizedPosition = new RelativePosition(position);
            else
                tantrum.SynchronizedPosition = new RelativePosition(position + (Vector3.up * 0.25f));

            tantrum._destroyed = !isActive;

            return Get(tantrum) as TantrumHazard;
        }
    }
}