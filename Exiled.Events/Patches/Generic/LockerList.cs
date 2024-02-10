// -----------------------------------------------------------------------
// <copyright file="LockerList.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using HarmonyLib;

    using MapGeneration.Distributors;

    /// <summary>
    /// Patches <see cref="Locker.Start"/>.
    /// </summary>
    [HarmonyPatch(typeof(Locker), nameof(Locker.Start))]
    internal class LockerList
    {
        private static void Postfix(Locker __instance) => API.Features.Lockers.Locker.Get(__instance);
    }
}