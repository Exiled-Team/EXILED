// -----------------------------------------------------------------------
// <copyright file="ThrowingItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;

    using Exiled.API.Enums;
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using InventorySystem.Items.ThrowableProjectiles;

    using Mirror;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="ThrowableNetworkHandler.ServerProcessMessages"/>.
    /// Adds the <see cref="Player.ThrowingItem"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ThrowableNetworkHandler), nameof(ThrowableNetworkHandler.ServerProcessMessages))]
    internal static class ThrowingItem
    {
        private static bool Prefix(NetworkConnection conn, ThrowableNetworkHandler.ThrowableItemMessage msg)
        {
            ReferenceHub hub;
            if (!ReferenceHub.TryGetHubNetID(conn.identity.netId, out hub) ||
                hub.inventory.CurItem.SerialNumber != msg.Serial || !(hub.inventory.CurInstance is ThrowableItem))
                return false;
            var ev = new ThrowingItemEventArgs(API.Features.Player.Get(hub), msg.Request);
            Player.OnThrowingItem(ev);
            msg = new ThrowableNetworkHandler.ThrowableItemMessage(msg.Serial, (ThrowableNetworkHandler.RequestType)ev.RequestType, msg.CameraRotation, msg.CameraPosition);

            return ev.IsAllowed;
        }
    }
}
