// -----------------------------------------------------------------------
// <copyright file="TeleportList.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using API.Features;
    using HarmonyLib;
    using MapGeneration;

    /// <summary>
    /// Patches <see cref="ImageGenerator.GenerateMap(int, string, out string)"/>.
    /// </summary>
    [HarmonyPatch(typeof(ImageGenerator), nameof(ImageGenerator.GenerateMap))]
    internal class TeleportList
    {
        private static void Prefix()
        {
            Map.TeleportsValue.Clear();
            Map.TeleportsValue.AddRange(UnityEngine.Object.FindObjectsOfType<PocketDimensionTeleport>());
        }
    }
}