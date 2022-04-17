// -----------------------------------------------------------------------
// <copyright file="DroppingCandy.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp330
{
#pragma warning disable SA1313

    using System;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using Footprinting;

    using global::Utils.Networking;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.Usables;
    using InventorySystem.Items.Usables.Scp330;

    using Mirror;

    using UnityEngine;

    /// <summary>
    /// Patches the <see cref="Scp330NetworkHandler.ServerSelectMessageReceived"/> method to add the <see cref="Handlers.Scp330.DroppingUpScp330"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp330NetworkHandler), nameof(Scp330NetworkHandler.ServerSelectMessageReceived))]
    internal static class DroppingCandy
    {
        private static bool Prefix(NetworkConnection conn, SelectScp330Message msg)
        {
            try
            {
                if (!ReferenceHub.TryGetHubNetID(conn.identity.netId, out ReferenceHub referenceHub)
                    || referenceHub.inventory.CurInstance is not Scp330Bag scp330Bag || scp330Bag is null
                    || scp330Bag.ItemSerial != msg.Serial || msg.CandyID >= scp330Bag.Candies.Count)
                {
                    return false;
                }

                if (!msg.Drop)
                {
                    scp330Bag.SelectedCandyId = msg.CandyID;
                    PlayerHandler handler = UsableItemsController.GetHandler(referenceHub);
                    handler.CurrentUsable = new(scp330Bag, msg.Serial, Time.timeSinceLevelLoad);
                    handler.CurrentUsable.Item.OnUsingStarted();
                    new StatusMessage(StatusMessage.StatusType.Start, msg.Serial).SendToAuthenticated();
                    return false;
                }

                PickupSyncInfo psi = new()
                {
                    ItemId = scp330Bag.ItemTypeId,
                    Serial = ItemSerialGenerator.GenerateNext(),
                    Weight = scp330Bag.Weight,
                    Position = referenceHub.PlayerCameraReference.transform.position,
                };
                DroppingUpScp330EventArgs ev = new(Player.Get(referenceHub), scp330Bag, scp330Bag.TryRemove(msg.CandyID));

                if (referenceHub.inventory.ServerCreatePickup(scp330Bag, psi, true) is not Scp330Pickup scp330Pickup)
                    return false;

                scp330Pickup.PreviousOwner = new(referenceHub);
                CandyKindID candyKindID = ev.Candy;
                if (candyKindID == CandyKindID.None)
                    return false;

                scp330Pickup.NetworkExposedCandy = candyKindID;
                scp330Pickup.StoredCandies.Add(candyKindID);
                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"{typeof(DroppingCandy).FullName}.{nameof(Prefix)}:\n{ex}");
                return true;
            }
        }
    }
}
