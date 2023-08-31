// -----------------------------------------------------------------------
// <copyright file="LockerListAdd.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using HarmonyLib;
    using MapGeneration.Distributors;

    using Lockers = API.Features.Lockers;

    /// <summary>
    /// A patch for adding lockers in the list.
    /// </summary>
    [HarmonyPatch(typeof(Locker), nameof(Locker.Start))]
    internal class LockerListAdd
    {
        private static void Postfix(Locker __instance)
        {
            _ = __instance switch
            {
                PedestalScpLocker psl => new Lockers.PedestalLocker(psl),
                _ => new Lockers.Locker(__instance)
            };
        }
    }
}