// -----------------------------------------------------------------------
// <copyright file="SpawnLocationData.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API
{
    using System.Collections.Generic;

    /// <summary>
    /// All of the data related to spawn locations.
    /// </summary>
    public struct SpawnLocationData
    {
        /// <summary>
        /// The names of the doors attached to each spawn location.
        /// </summary>
        public static Dictionary<SpawnLocation, string> DoorNames = new Dictionary<SpawnLocation, string>
        {
            { SpawnLocation.Inside012, "012" },
            { SpawnLocation.Inside096, "096" },
            { SpawnLocation.Inside914, "914" },
            { SpawnLocation.InsideHid, "HID" },
            { SpawnLocation.InsideGr18, "GR18" },
            { SpawnLocation.InsideGateA, "GATE_A" },
            { SpawnLocation.InsideGateB, "GATE_B" },
            { SpawnLocation.InsideLczWc, "LCZ_WC" },
            { SpawnLocation.InsideHidLeft, "HID_LEFT" },
            { SpawnLocation.InsideLczCafe, "LCZ_CAFE" },
            { SpawnLocation.Inside173Gate, "173_GATE" },
            { SpawnLocation.InsideIntercom, "INTERCOM" },
            { SpawnLocation.InsideHidRight, "HID_RIGHT" },
            { SpawnLocation.Inside079First, "079_FIRST" },
            { SpawnLocation.Inside012Bottom, "012_BOTTOM" },
            { SpawnLocation.Inside012Locker, "012_LOCKER" },
            { SpawnLocation.Inside049Armory, "049_ARMORY" },
            { SpawnLocation.Inside173Armory, "173_ARMORY" },
            { SpawnLocation.Inside173Bottom, "173_BOTTOM" },
            { SpawnLocation.InsideLczArmory, "LCZ_ARMORY" },
            { SpawnLocation.InsideHczArmory, "HCZ_ARMORY" },
            { SpawnLocation.InsideNukeArmory, "NUKE_ARMORY" },
            { SpawnLocation.InsideSurfaceNuke, "SURFACE_NUKE" },
            { SpawnLocation.Inside079Secondary, "079_SECONDARY" },
            { SpawnLocation.Inside173Connector, "173_CONNECTOR" },
            { SpawnLocation.InsideServersBottom, "SERVERS_BOTTOM" },
            { SpawnLocation.InsideEscapePrimary, "ESCAPE_PRIMARY" },
            { SpawnLocation.InsideEscapeSecondary, "ESCAPE_SECONDARY" },
        };

        /// <summary>
        /// The names of spawn locations who's positions are on the opposite side of their door, and must be corrected.
        /// </summary>
        public static List<SpawnLocation> ReversedLocations = new List<SpawnLocation>
        {
            SpawnLocation.InsideServersBottom,
            SpawnLocation.InsideHczArmory,
            SpawnLocation.Inside079First,
            SpawnLocation.InsideHidRight,
            SpawnLocation.Inside173Gate,
            SpawnLocation.InsideHidLeft,
            SpawnLocation.InsideGateA,
            SpawnLocation.InsideGateB,
            SpawnLocation.InsideLczWc,
            SpawnLocation.InsideGr18,
            SpawnLocation.Inside914,
            SpawnLocation.InsideHid,
        };
    }
}
