// -----------------------------------------------------------------------
// <copyright file="CoffeeListAdd.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using Exiled.API.Features;
    using HarmonyLib;
#pragma warning disable SA1313

    /// <summary>
    /// Patches <see cref="global::Coffee.Start"/> to control coffee list.
    /// </summary>
    [HarmonyPatch(typeof(global::Coffee), nameof(global::Coffee.Start))]
    internal class CoffeeListAdd
    {
        private static void Postfix(global::Coffee __instance)
        {
            _ = new Coffee(__instance);
        }
    }
}