// -----------------------------------------------------------------------
// <copyright file="JailbirdPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Item
{
    using Exiled.Events.EventArgs.Item;
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using Handlers;

    using HarmonyLib;

    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Jailbird;
    using Mirror;

    /// <summary>
    ///     Patches
    ///     <see cref="AttachmentsServerHandler.ServerReceivePreference(NetworkConnection, AttachmentsSetupPreference)" />.
    ///     Adds the <see cref="Item.Swinging" /> event.
    /// </summary>
    [HarmonyPatch(typeof(JailbirdItem), nameof(JailbirdItem.ServerProcessCmd))]
    internal static class JailbirdPatch
    {
        private static bool Prefix(JailbirdItem __instance, NetworkReader reader)
        {
            if (__instance._broken)
            {
                return false;
            }

            JailbirdMessageType jailbirdMessageType = (JailbirdMessageType)reader.ReadByte();
            if (jailbirdMessageType is JailbirdMessageType.AttackTriggered)
            {
                SwingingEventArgs ev = new(__instance.Owner, __instance);
                Item.OnSwinging(ev);
                return ev.IsAllowed;
            }
            else if (jailbirdMessageType is JailbirdMessageType.ChargeStarted)
            {
                ChargingJailbirdEventArgs ev = new(__instance.Owner, __instance);
                Item.OnChargingJailbird(ev);
                return ev.IsAllowed;
            }

            return true;
        }
    }
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
}