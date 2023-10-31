// -----------------------------------------------------------------------
// <copyright file="PrismaticCloud.cs" company="Exiled Team">
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

    using PrismaticCloudHazard = global::Hazards.PrismaticCloud;

    /// <summary>
    /// A wrapper for <see cref="PrismaticCloudHazard"/>.
    /// </summary>
    public class PrismaticCloud : TemporaryHazard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrismaticCloud"/> class.
        /// </summary>
        /// <param name="hazard">The <see cref="PrismaticCloudHazard"/> instance.</param>
        public PrismaticCloud(PrismaticCloudHazard hazard)
            : base(hazard)
        {
            Base = hazard;
        }

        /// <summary>
        /// Gets the <see cref="PrismaticCloudHazard"/>.
        /// </summary>
        public new PrismaticCloudHazard Base { get; }

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
