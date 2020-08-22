// -----------------------------------------------------------------------
// <copyright file="Scp096MaxShield.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1118
#pragma warning disable SA1313
    using HarmonyLib;

    using Scp096 = PlayableScps.Scp096;

    /// <summary>
    /// Patches the <see cref="Scp096.MaxShield"/> property.
    /// </summary>
    [HarmonyPatch(typeof(Scp096), nameof(Scp096.MaxShield), MethodType.Getter)]
    internal static class Scp096MaxShield
    {
        private static bool Prefix(Scp096 __instance, ref float __result)
        {
            __result = Exiled.API.Features.Scp096.MaxShield;
            return false;
        }
    }
}
