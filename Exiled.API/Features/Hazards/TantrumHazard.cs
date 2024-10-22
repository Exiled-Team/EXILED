// -----------------------------------------------------------------------
// <copyright file="TantrumHazard.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Hazards
{
    using global::Hazards;
    using PlayerRoles;
    using RelativePositioning;
    using UnityEngine;

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
        /// Gets the <see cref="TantrumEnvironmentalHazard"/>.
        /// </summary>
        public new TantrumEnvironmentalHazard Base { get; }

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
        /// Gets or sets the teams that will be affected by this Tantrum.
        /// </summary>
        public Team[] TargetedTeams
        {
            get => Base._targetedTeams;
            set => Base._targetedTeams = value;
        }
    }
}