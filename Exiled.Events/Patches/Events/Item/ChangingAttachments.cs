// -----------------------------------------------------------------------
// <copyright file="ChangingAttachments.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Item
{
#pragma warning disable SA1600
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.API.Structs;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Attachments;

    using Mirror;

    using UnityEngine;

    using Firearm = InventorySystem.Items.Firearms.Firearm;

    /// <summary>
    /// Patches <see cref="AttachmentsServerHandler.ServerReceiveChangeRequest(NetworkConnection, AttachmentsChangeRequest)"/>.
    /// Adds the <see cref="Handlers.Item.ChangingAttachments"/> event.
    /// </summary>
    [HarmonyPatch(typeof(AttachmentsServerHandler), nameof(AttachmentsServerHandler.ServerReceiveChangeRequest))]
    internal static class ChangingAttachments
    {
        internal static bool Prefix(NetworkConnection conn, AttachmentsChangeRequest msg)
        {
            if (!NetworkServer.active || !ReferenceHub.TryGetHub(conn.identity.gameObject, out ReferenceHub referenceHub))
            {
                return false;
            }

            Firearm firearm;
            if ((firearm = referenceHub.inventory.CurInstance as Firearm) == null)
            {
                return false;
            }

            if (referenceHub.inventory.CurItem.SerialNumber != msg.WeaponSerial)
            {
                return false;
            }

            bool flag = referenceHub.characterClassManager.CurClass == RoleType.Spectator;
            if (!flag)
            {
                foreach (WorkstationController workstationController in WorkstationController.AllWorkstations)
                {
                    if (!(workstationController == null) && workstationController.Status == 3 && workstationController.IsInRange(referenceHub))
                    {
                        flag = true;
                        break;
                    }
                }
            }

            if (flag)
            {
                if (msg.AttachmentsCode == firearm.GetCurrentAttachmentsCode())
                    return false;

                uint newAttachments = msg.AttachmentsCode;
                uint oldAttachments = firearm.Status.Attachments;
                FirearmAttachment addedAttachment = default;
                FirearmAttachment removedAttachment = default;
                uint num = 1;
                foreach (FirearmAttachment att in firearm.Attachments)
                {
                    if (att.IsEnabled && (newAttachments & num) != num)
                        removedAttachment = att;
                    else if (!att.IsEnabled && (newAttachments & num) == num)
                        addedAttachment = att;

                    num *= 2U;
                }

                AttachmentIdentifier newIdentifier = API.Features.Items.Firearm.AvailableAttachments[firearm.ItemTypeId].FirstOrDefault(x =>
                x.Name == addedAttachment.Name);

                AttachmentIdentifier oldIdentifier = API.Features.Items.Firearm.AvailableAttachments[firearm.ItemTypeId].FirstOrDefault(x =>
                x.Name == removedAttachment.Name);

                ChangingAttachmentsEventArgs ev = new ChangingAttachmentsEventArgs(
                    API.Features.Player.Get(conn.identity.netId),
                    (API.Features.Items.Firearm)API.Features.Items.Item.Get(firearm),
                    oldIdentifier,
                    newIdentifier,
                    true);

                Handlers.Item.OnChangingAttachments(ev);

                if (!ev.IsAllowed)
                    return false;

                newIdentifier = ev.NewAttachmentIdentifier;

                msg.AttachmentsCode = newIdentifier.Code + firearm.GetCurrentAttachmentsCode() - ev.OldAttachmentIdentifier.Code;

                firearm.ApplyAttachmentsCode(msg.AttachmentsCode, true);
                if (firearm.Status.Ammo > firearm.AmmoManagerModule.MaxAmmo)
                {
                    referenceHub.inventory.ServerAddAmmo(firearm.AmmoType, firearm.Status.Ammo - firearm.AmmoManagerModule.MaxAmmo);
                }

                firearm.Status = new FirearmStatus((byte)Mathf.Min(firearm.Status.Ammo, firearm.AmmoManagerModule.MaxAmmo), firearm.Status.Flags, msg.AttachmentsCode);
            }

            return false;
        }
    }
}
