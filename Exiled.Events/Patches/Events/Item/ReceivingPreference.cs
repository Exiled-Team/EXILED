// -----------------------------------------------------------------------
// <copyright file="ReceivingPreference.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Item
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using Exiled.API.Extensions;
    using Exiled.API.Structs;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem.Items.Firearms.Attachments;

    using Mirror;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="AttachmentsServerHandler.ServerReceiveChangeRequest(NetworkConnection, AttachmentsChangeRequest)"/>.
    /// Adds the <see cref="Handlers.Item.ReceivingPreference"/> event.
    /// </summary>
    [HarmonyPatch(typeof(AttachmentsServerHandler), nameof(AttachmentsServerHandler.ServerReceivePreference))]
    internal static class ReceivingPreference
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldloc_1);

            LocalBuilder ev = generator.DeclareLocal(typeof(ReceivingPreferenceEventArgs));
            LocalBuilder mem_0x01 = generator.DeclareLocal(typeof(AttachmentIdentifier[]));
            LocalBuilder mem_0x02 = generator.DeclareLocal(typeof(uint));
            LocalBuilder mem_0x03 = generator.DeclareLocal(typeof(List<AttachmentIdentifier>));
            LocalBuilder cmp_0x01 = generator.DeclareLocal(typeof(bool));

            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Stloc_S, mem_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(AttachmentsSetupPreference), nameof(AttachmentsSetupPreference.Weapon))),
                new CodeInstruction(OpCodes.Ldloca_S, mem_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Dictionary<ItemType, uint>), "TryGetValue", new[] { typeof(ItemType), typeof(uint) })),
                new CodeInstruction(OpCodes.Stloc_S, cmp_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, cmp_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse_S, ret),
                new CodeInstruction(OpCodes.Ldarga_S, 1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(AttachmentsSetupPreference), nameof(AttachmentsSetupPreference.Weapon))),
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(ItemExtensions), nameof(ItemExtensions.GetAttachments))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Enumerable), nameof(Enumerable.ToArray))),
                new CodeInstruction(OpCodes.Stloc_S, mem_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Ldarga_S, 1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(AttachmentsSetupPreference), nameof(AttachmentsSetupPreference.Weapon))),
                new CodeInstruction(OpCodes.Ldarga_S, 1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(AttachmentsSetupPreference), nameof(AttachmentsSetupPreference.AttachmentsCode))),
                new CodeInstruction(OpCodes.Call, Method(typeof(ItemExtensions), nameof(ItemExtensions.GetAttachments))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Enumerable), nameof(Enumerable.ToList))),
                new CodeInstruction(OpCodes.Stloc_S, mem_0x03.LocalIndex),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(NetworkConnection), nameof(NetworkConnection.identity))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(NetworkIdentity), nameof(NetworkIdentity.netId))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(uint) })),
                new CodeInstruction(OpCodes.Ldarga_S, 1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(AttachmentsSetupPreference), nameof(AttachmentsSetupPreference.Weapon))),
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x03.LocalIndex),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ReceivingPreferenceEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnReceivingPreference))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingPreferenceEventArgs), nameof(ReceivingPreferenceEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, ret),
                new CodeInstruction(OpCodes.Ldarga_S, 1),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingPreferenceEventArgs), nameof(ReceivingPreferenceEventArgs.Item))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(AttachmentsSetupPreference), nameof(AttachmentsSetupPreference.Weapon))),
                new CodeInstruction(OpCodes.Ldarga_S, 1),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingPreferenceEventArgs), nameof(ReceivingPreferenceEventArgs.NewAttachmentIdentifiers))),
                new CodeInstruction(OpCodes.Call, Method(typeof(ItemExtensions), nameof(ItemExtensions.GetAttachmentsCode))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(AttachmentsSetupPreference), nameof(AttachmentsSetupPreference.AttachmentsCode))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
