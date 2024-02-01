// -----------------------------------------------------------------------
// <copyright file="MapGenerated.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using API.Features;
    using API.Features.Items;
    using API.Features.Pools;
    using API.Structs;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;

    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Firearms.Attachments.Components;

    using MEC;

    using Utils.NonAllocLINQ;

    /// <summary>
    /// Handles <see cref="Handlers.Map.Generated"/> event.
    /// </summary>
    internal static class MapGenerated
    {
        /// <summary>
        /// Called once the map is generated.
        /// </summary>
        /// <remarks>
        /// This fixes an issue where
        /// all those extensions that
        /// require calling the central
        /// property of the Map class in
        /// the API were corrupted due to
        /// a missed call, such as before
        /// getting the elevator type.
        /// </remarks>
        public static void OnMapGenerated()
        {
            Map.ClearCache();

            // TODO: Fix For (https://trello.com/c/cUwpZDLs/5003-config-teamrespawnqueue-in-configgameplay-is-not-working-as-expected)
            PlayerRoles.RoleAssign.HumanSpawner.Handlers[PlayerRoles.Team.ChaosInsurgency] = new PlayerRoles.RoleAssign.OneRoleHumanSpawner(PlayerRoles.RoleTypeId.ChaosRepressor);
            PlayerRoles.RoleAssign.HumanSpawner.Handlers[PlayerRoles.Team.OtherAlive] = new PlayerRoles.RoleAssign.OneRoleHumanSpawner(PlayerRoles.RoleTypeId.Tutorial);
            PlayerRoles.RoleAssign.HumanSpawner.Handlers[PlayerRoles.Team.Dead] = new PlayerRoles.RoleAssign.OneRoleHumanSpawner(PlayerRoles.RoleTypeId.Spectator);

            GenerateAttachments();
            Timing.CallDelayed(1, GenerateCache);
        }

        private static void GenerateCache()
        {
            Handlers.Map.OnGenerated();

            Timing.CallDelayed(0.1f, Handlers.Server.OnWaitingForPlayers);
        }

        private static void GenerateAttachments()
        {
            foreach (FirearmType firearmType in Enum.GetValues(typeof(FirearmType)))
            {
                if (firearmType == FirearmType.None)
                    continue;

                if (Item.Create(firearmType.GetItemType()) is not Firearm firearm)
                    continue;

                Firearm.ItemTypeToFirearmInstance.Add(firearmType, firearm);

                List<AttachmentIdentifier> attachmentIdentifiers = ListPool<AttachmentIdentifier>.Pool.Get();
                HashSet<AttachmentSlot> attachmentsSlots = HashSetPool<AttachmentSlot>.Pool.Get();

                uint code = 1;

                foreach (Attachment attachment in firearm.Attachments)
                {
                    attachmentsSlots.Add(attachment.Slot);
                    attachmentIdentifiers.Add(new(code, attachment.Name, attachment.Slot));
                    code *= 2U;
                }

                uint baseCode = 0;

                attachmentsSlots
                    .ForEach(slot => baseCode += attachmentIdentifiers
                        .Where(attachment => attachment.Slot == slot)
                        .Min(slot => slot.Code));

                Firearm.BaseCodesValue.Add(firearmType, baseCode);
                Firearm.AvailableAttachmentsValue.Add(firearmType, attachmentIdentifiers.ToArray());

                ListPool<AttachmentIdentifier>.Pool.Return(attachmentIdentifiers);
                HashSetPool<AttachmentSlot>.Pool.Return(attachmentsSlots);
            }
        }
    }
}