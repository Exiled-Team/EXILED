// -----------------------------------------------------------------------
// <copyright file="PrismaticCloudHazard.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Hazards
{
    using global::Hazards;
    using RelativePositioning;

    /// <summary>
    /// A wrapper for <see cref="PrismaticCloudHazard"/>.
    /// </summary>
    public class PrismaticCloudHazard : TemporaryHazard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrismaticCloudHazard"/> class.
        /// </summary>
        /// <param name="hazard">The <see cref="PrismaticCloudHazard"/> instance.</param>
        public PrismaticCloudHazard(PrismaticCloud hazard)
            : base(hazard)
        {
            Base = hazard;
        }

        /// <summary>
        /// Gets the <see cref="PrismaticCloudHazard"/>.
        /// </summary>
        public new PrismaticCloud Base { get; }

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