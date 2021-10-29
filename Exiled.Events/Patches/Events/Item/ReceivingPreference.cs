// -----------------------------------------------------------------------
// <copyright file="ReceivingPreference.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Item
{
#pragma warning disable SA1600
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Structs;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem.Items.Firearms.Attachments;

    using Mirror;

    /// <summary>
    /// Patches <see cref="AttachmentsServerHandler.ServerReceiveChangeRequest(NetworkConnection, AttachmentsChangeRequest)"/>.
    /// Adds the <see cref="Handlers.Item.ChangingAttachments"/> event.
    /// </summary>
    [HarmonyPatch(typeof(AttachmentsServerHandler), nameof(AttachmentsServerHandler.ServerReceiveChangeRequest))]
    internal static class ReceivingPreference
    {
        internal static bool Prefix(NetworkConnection conn, AttachmentsSetupPreference msg)
        {
            if (!NetworkServer.active || !ReferenceHub.TryGetHub(conn.identity.gameObject, out ReferenceHub referenceHub))
            {
                return false;
            }

            if (AttachmentsServerHandler.PlayerPreferences.TryGetValue(referenceHub, out Dictionary<ItemType, uint> dictionary) && dictionary != null)
            {
                dictionary[msg.Weapon] = msg.AttachmentsCode;
                return false;
            }

            Dictionary<ReferenceHub, Dictionary<global::ItemType, uint>> playerPreferences = AttachmentsServerHandler.PlayerPreferences;
            ReferenceHub referenceHub2 = referenceHub;
            Dictionary<ItemType, uint> dictionary2 = new Dictionary<ItemType, uint>();

            AttachmentIdentifier[] oldAttachments = msg.Weapon.GetAttachments(dictionary2.FirstOrDefault(x => x.Key == msg.Weapon).Value).ToArray();
            List<AttachmentIdentifier> newAttachments = msg.Weapon.GetAttachments(msg.AttachmentsCode).ToList();
            ReceivingPreferenceEventArgs ev = new ReceivingPreferenceEventArgs(Player.Get(conn.identity.netId), msg.Weapon, oldAttachments, newAttachments, true);

            if (!ev.IsAllowed)
                return false;

            ItemType weapon = ev.Item;
            dictionary2[weapon] = (uint)ev.Item.GetBaseCode() + ev.NewAttachmentIdentifiers.GetAttachmentsCode();

            playerPreferences[referenceHub2] = dictionary2;
            return false;
        }
    }
}
