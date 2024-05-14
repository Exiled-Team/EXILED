// -----------------------------------------------------------------------
// <copyright file="HazardType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using Exiled.API.Features.Hazards;

    /// <summary>
    /// Unique identifier for a <see cref="Hazard"/>.
    /// </summary>
    public enum HazardType
    {
        /// <summary>
        /// SCP-939 amnestic cloud.
        /// </summary>
        AmnesticCloud,

        /// <summary>
        /// Sinkhole spawned at start of round.
        /// </summary>
        Sinkhole,

        /// <summary>
        /// SCP-173 tantrum.
        /// </summary>
        Tantrum,

        /// <summary>
        /// Should never happen
        /// </summary>
        Unknown,
    }
}