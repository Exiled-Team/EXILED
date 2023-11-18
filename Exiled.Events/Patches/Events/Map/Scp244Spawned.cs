// -----------------------------------------------------------------------
// <copyright file="Scp244Spawned.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.EventArgs.Scp3114;
    using Handlers;
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using HarmonyLib;
    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.Usables.Scp244;
    using Mirror;
    using UnityEngine;
    using static PlayerList;

    /// <summary>
    ///     Patches <see cref="Scp244Spawner.SpawnScp244" />.
    ///     Adds the <see cref="Map.Scp244Spawned" /> event.
    /// </summary>
    [EventPatch(typeof(Map), nameof(Map.Scp244Spawned))]
    [HarmonyPatch(typeof(Scp244Spawner), nameof(Scp244Spawner.SpawnScp244))]
    internal static class Scp244Spawned
    {
        private static bool Prefix(ItemBase ib)
        {
            if (Scp244Spawner.CompatibleRooms.Count == 0 && Random.value > 0.35f)
                return false;

            int index = Random.Range(0, Scp244Spawner.CompatibleRooms.Count);
            Vector3 position = Scp244Spawner.CompatibleRooms[index].transform.TransformPoint(Scp244Spawner.NameToPos[Scp244Spawner.CompatibleRooms[index].Name]);
            ItemPickupBase itemPickupBase = Object.Instantiate(ib.PickupDropModel, position, Quaternion.identity);
            itemPickupBase.NetworkInfo = new PickupSyncInfo
            {
                ItemId = ib.ItemTypeId,
                WeightKg = ib.Weight,
                Serial = ItemSerialGenerator.GenerateNext(),
            };
            Scp244DeployablePickup scp244DeployablePickup = itemPickupBase as Scp244DeployablePickup;
            if (scp244DeployablePickup != null)
                scp244DeployablePickup.State = Scp244State.Active;

            Scp244SpawningEventArgs ev = new(__instance.Owner, __instance.CurRagdoll, true);
            Handlers.Scp3114.OnTryUseBody(ev);

            if (!ev.IsAllowed)
                return false;

            NetworkServer.Spawn(itemPickupBase.gameObject);
            Scp244Spawner.CompatibleRooms.RemoveAt(index);
            return false;
        }
    }
}