// -----------------------------------------------------------------------
// <copyright file="SinkholeHazard.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Hazards
{
    using Exiled.API.Enums;
    using global::Hazards;

    /// <summary>
    /// Represents a sinkhole hazard.
    /// </summary>
    public class SinkholeHazard : Hazard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SinkholeHazard"/> class.
        /// </summary>
        /// <param name="hazard">The <see cref="SinkholeEnvironmentalHazard"/> instance.</param>
        public SinkholeHazard(SinkholeEnvironmentalHazard hazard)
            : base(hazard)
        {
            Base = hazard;
        }

        /// <summary>
        /// Gets the <see cref="SinkholeEnvironmentalHazard"/>.
        /// </summary>
        public new SinkholeEnvironmentalHazard Base { get; }

        /// <inheritdoc />
        public override HazardType Type { get; } = HazardType.Sinkhole;
    }
}