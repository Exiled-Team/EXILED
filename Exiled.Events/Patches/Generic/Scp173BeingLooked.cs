// -----------------------------------------------------------------------
// <copyright file="Scp173BeingLooked.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
    using API.Features;

    using HarmonyLib;

    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp173;

    using ExiledEvents = Exiled.Events.Events;
    using Scp173Role = API.Features.Roles.Scp173Role;

    /// <summary>
    /// Patches <see cref="Scp173ObserversTracker.UpdateObserver(ReferenceHub)"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp173ObserversTracker), nameof(Scp173ObserversTracker.UpdateObserver))]
    internal static class Scp173BeingLooked
    {
        private static bool Prefix(Scp173ObserversTracker __instance, ReferenceHub targetHub, ref int __result)
        {
            if (Player.Get(targetHub) is Player player &&
                ((player.Role.Type == RoleTypeId.Tutorial &&
                !ExiledEvents.Instance.Config.CanTutorialBlockScp173) ||
                Scp173Role.TurnedPlayers.Contains(player)) &&
                __instance.IsObservedBy(targetHub, 0.2f))
            {
                __result = __instance.Observers.Remove(targetHub) ? -1 : 0;
                return false;
            }

            return true;
        }
    }
}