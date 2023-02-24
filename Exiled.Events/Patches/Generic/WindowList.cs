// -----------------------------------------------------------------------
// <copyright file="WindowList.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
    using API.Features;

    using HarmonyLib;

    using MapGeneration.Distributors;

    /// <summary>
    /// Patches <see cref="BreakableWindow.EnableColliders"/>.
    /// </summary>
    [HarmonyPatch(typeof(BreakableWindow), nameof(BreakableWindow.EnableColliders))]
    internal class WindowList
    {
        private static void Postfix(BreakableWindow __instance)
        {
            _ = new Window(__instance, __instance.gameObject.GetComponentInParent<Room>());
        }
    }
}