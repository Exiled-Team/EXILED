// -----------------------------------------------------------------------
// <copyright file="SnowpileListAdd.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using HarmonyLib;

#pragma warning disable SA1313

    /// <summary>
    /// Patches <see cref="Snowpile.Awake"/> to control list.
    /// </summary>
    [HarmonyPatch(typeof(Snowpile), nameof(Snowpile.Awake))]
    internal class SnowpileListAdd
    {
        private static void Postfix(Snowpile __instance)
        {
            _ = new Exiled.API.Features.Snowpile(__instance);
        }
    }
}