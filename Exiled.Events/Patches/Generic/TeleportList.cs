// -----------------------------------------------------------------------
// <copyright file="TeleportList.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
    using API.Features;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="PocketDimensionGenerator.PrepTeleports"/>.
    /// </summary>
    [HarmonyPatch(typeof(PocketDimensionGenerator), nameof(PocketDimensionGenerator.PrepTeleports))]
    internal class TeleportList
    {
        private static void Postfix(ref PocketDimensionTeleport[] __result)
        {
            Map.TeleportsValue.Clear();
            Map.TeleportsValue.AddRange(__result);
        }
    }
}