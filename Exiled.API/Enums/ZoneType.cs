// -----------------------------------------------------------------------
// <copyright file="ZoneType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using System;

    using Exiled.API.Features.Doors;
    using Features;

    /// <summary>
    /// Facility zone types.
    /// </summary>
    /// <seealso cref="Room.Zone"/>
    /// <seealso cref="Camera.Zone"/>
    /// <seealso cref="Player.Zone"/>
    /// <seealso cref="Door.Zone"/>
    /// <seealso cref="Door.Random(ZoneType, bool)"/>
    /// <seealso cref="Room.Random(ZoneType)"/>
    /// <seealso cref="Map.TurnOffAllLights(float, ZoneType)"/>
    /// <seealso cref="Map.TurnOffAllLights(float, System.Collections.Generic.IEnumerable{ZoneType})"/>
    [Flags]
    public enum ZoneType
    {
        /// <summary>
        /// An unspecified zone.
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// The Light Containment Zone.
        /// </summary>
        LightContainment = 1,

        /// <summary>
        /// The Heavy Containment Zone.
        /// </summary>
        HeavyContainment = 2,

        /// <summary>
        /// The Entrance Zone.
        /// </summary>
        Entrance = 4,

        /// <summary>
        /// The Surface Zone.
        /// </summary>
        Surface = 8,

        /// <summary>
        /// The Pocket Dimension.
        /// </summary>
        Pocket = 16,

        /// <summary>
        /// An unknown type of zone.
        /// </summary>
        Other = 32,
    }
}