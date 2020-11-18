// -----------------------------------------------------------------------
// <copyright file="SpawningItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
#pragma warning disable SA1313

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="RandomItemSpawner.SpawnerItemToSpawn.DoorTrigger"/>.
    /// Adds the <see cref="Handlers.Map.SpawningItem"/> and <see cref="Handlers.Map.SpawnedItem"/> events.
    /// </summary>
    [HarmonyPatch(typeof(RandomItemSpawner.SpawnerItemToSpawn), nameof(RandomItemSpawner.SpawnerItemToSpawn.Spawn))]
    internal static class SpawningItem
    {
        private static IEnumerator<float> Postfix(IEnumerator<float> values, RandomItemSpawner.SpawnerItemToSpawn __instance)
        {
            SpawningItemEventArgs ev = new SpawningItemEventArgs(__instance._id, __instance._pos, __instance._rot, __instance._locked, true);

            Handlers.Map.OnSpawningItem(ev);

            if (ev.IsAllowed)
            {
                Pickup pickup = ReferenceHub.GetHub(PlayerManager.localPlayer).inventory.SetPickup(ev.Id, 0.0f, Vector3.zero, Quaternion.identity, 0, 0, 0);
                yield return float.NegativeInfinity;
                HostItemSpawner.SetPos(pickup, ev.Position, ev.Id, ev.Rotation.eulerAngles);
                pickup.RefreshDurability(true, true);
                if (ev.Locked)
                    pickup.Locked = true;

                SpawnedItemEventArgs spawned_ev = new SpawnedItemEventArgs(pickup);
                Handlers.Map.OnSpawnedItem(spawned_ev);
            }
        }
    }
}
