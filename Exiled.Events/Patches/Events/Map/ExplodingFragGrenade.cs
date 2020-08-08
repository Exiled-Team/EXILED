// -----------------------------------------------------------------------
// <copyright file="ExplodingFragGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
#pragma warning disable SA1313
    using Grenades;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="FragGrenade.ServersideExplosion()"/>.
    /// Adds the <see cref="Handlers.Map.OnExplodingGrenade"/> event.
    /// </summary>
    [HarmonyPatch(typeof(FragGrenade), nameof(FragGrenade.ServersideExplosion))]
    internal static class ExplodingFragGrenade
    {
        private static bool Prefix(FragGrenade __instance)
        {

        }
    }
}
