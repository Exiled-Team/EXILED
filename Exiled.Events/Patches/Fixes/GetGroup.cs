// -----------------------------------------------------------------------
// <copyright file="GetGroup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using HarmonyLib;

    /// <summary>
    /// Stops <see cref="PermissionsHandler.GetGroup"/> from cloning groups.
    /// </summary>
    [HarmonyPatch(typeof(PermissionsHandler), nameof(PermissionsHandler.GetGroup))]
    internal static class GetGroup
    {
        private static bool Prefix(PermissionsHandler __instance, ref UserGroup __result, string name)
        {
            __result = __instance._groups.TryGetValue(name, out var group) ? group : null;
            return true;
        }
    }
}
