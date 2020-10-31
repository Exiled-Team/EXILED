// -----------------------------------------------------------------------
// <copyright file="SpawningItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
#pragma warning disable SA1313

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using MEC;

    /// <summary>
    /// Patches <see cref="RandomItemSpawner.SpawnerItemToSpawn.DoorTrigger"/>.
    /// Adds the <see cref="Handlers.Map.SpawningItem"/> event.
    /// </summary>
    [HarmonyPatch(typeof(RandomItemSpawner.SpawnerItemToSpawn), nameof(RandomItemSpawner.SpawnerItemToSpawn.DoorTrigger))]
    internal static class SpawningItem
    {
        public static bool Prefix(RandomItemSpawner.SpawnerItemToSpawn __instance)
        {
            SpawningItemEventArgs ev = new SpawningItemEventArgs(__instance._id, __instance._pos, __instance._rot, true);

            Handlers.Map.OnSpawningItem(ev);

            if (!ev.IsAllowed)
                return false;
            if (ev.Id == __instance._id && ev.Position == __instance._pos && ev.Rotation == __instance._rot)
                return true;
            RandomItemSpawner.SpawnerItemToSpawn newItem = new RandomItemSpawner.SpawnerItemToSpawn(ev.Id, ev.Position, ev.Rotation, __instance._locked);
            Timing.RunCoroutine(newItem.Spawn(), Segment.FixedUpdate);
            return false;
        }
    }
}
