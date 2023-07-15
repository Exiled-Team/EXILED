// -----------------------------------------------------------------------
// <copyright file="TantrumHazard.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Hazards
{
    using System.Collections.Generic;
    using System.Linq;

    using global::Hazards;
    using RelativePositioning;

    /// <summary>
    /// A wrapper for <see cref="TantrumEnvironmentalHazard"/>.
    /// </summary>
    public class TantrumHazard : Hazard
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
        /// Gets the list of all <see cref="TantrumHazard"/>.
        /// </summary>
        public static IEnumerable<TantrumHazard> AllTantrums => TantrumEnvironmentalHazard.AllTantrums.Select(x => Get(x) as TantrumHazard);

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
    }
}