// -----------------------------------------------------------------------
// <copyright file="ZoneType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using System;

    /// <summary>
    /// Facility zone types.
    /// </summary>
    [Flags]
    public enum ZoneType
    {
        /// <summary>
        /// The Surface Zone.
        /// </summary>
        Surface,

        /// <summary>
        /// The Entrance Zone.
        /// </summary>
        Entrance,

        /// <summary>
        /// The Heavy Containment Zone.
        /// </summary>
        HeavyContainment,

        /// <summary>
        /// The Light Containment Zone.
        /// </summary>
        LightContainment,

        /// <summary>
        /// An unspecified zone.
        /// </summary>
        Unspecified,
    }
}
