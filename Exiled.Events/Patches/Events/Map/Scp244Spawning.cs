// -----------------------------------------------------------------------
// <copyright file="Scp244Spawning.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using Exiled.API.Features;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using HarmonyLib;
    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.Usables.Scp244;
    using MapGeneration;
    using Mirror;
    using UnityEngine;

    /// <summary>
    ///     Patches <see cref="Scp244Spawner.SpawnScp244" />.
    ///     Adds the <see cref="Handlers.Map.Scp244Spawning" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.Scp244Spawning))]
    [HarmonyPatch(typeof(Scp244Spawner), nameof(Scp244Spawner.SpawnScp244))]
    internal static class Scp244Spawning
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

            Scp244SpawningEventArgs ev = new(Room.Get(Scp244Spawner.CompatibleRooms[index]), Pickup.Get(itemPickupBase), Pickup.Get(itemPickupBase).As<Scp244Pickup>());
            Handlers.Map.OnScp244Spawning(ev);

            if (!ev.IsAllowed)
            {
                NetworkServer.Destroy(itemPickupBase.gameObject);
                return false;
            }

            NetworkServer.Spawn(itemPickupBase.gameObject);
            Scp244Spawner.CompatibleRooms.RemoveAt(index);
            return false;
        }
    }
}