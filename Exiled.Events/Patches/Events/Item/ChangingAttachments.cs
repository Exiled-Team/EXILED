// -----------------------------------------------------------------------
// <copyright file="ChangingAttachments.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Item
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Structs;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem.Items.Firearms.Attachments;

    using Mirror;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Firearm = InventorySystem.Items.Firearms.Firearm;

    /// <summary>
    /// Patches <see cref="AttachmentsServerHandler.ServerReceiveChangeRequest(NetworkConnection, AttachmentsChangeRequest)"/>.
    /// Adds the <see cref="Handlers.Item.ChangingAttachments"/> event.
    /// </summary>
    [HarmonyPatch(typeof(AttachmentsServerHandler), nameof(AttachmentsServerHandler.ServerReceiveChangeRequest))]
    internal static class ChangingAttachments
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = -3;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_1) + offset;

            LocalBuilder wCode_0x01 = generator.DeclareLocal(typeof(uint));
            LocalBuilder wCode_0x02 = generator.DeclareLocal(typeof(uint));
            LocalBuilder wCode_0x03 = generator.DeclareLocal(typeof(uint));
            LocalBuilder wCode_0x04 = generator.DeclareLocal(typeof(uint));
            LocalBuilder attachment_0x01 = generator.DeclareLocal(typeof(FirearmAttachment));
            LocalBuilder attachment_0x02 = generator.DeclareLocal(typeof(FirearmAttachment));
            LocalBuilder repIter_1 = generator.DeclareLocal(typeof(int));
            LocalBuilder repIter_Id = generator.DeclareLocal(typeof(FirearmAttachment));
            LocalBuilder identifier_0x01 = generator.DeclareLocal(typeof(AttachmentIdentifier));
            LocalBuilder identifier_0x02 = generator.DeclareLocal(typeof(AttachmentIdentifier));
            LocalBuilder un_M_arr = generator.DeclareLocal(typeof(AttachmentIdentifier[]));
            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingAttachmentsEventArgs));
            LocalBuilder repIter_2 = generator.DeclareLocal(typeof(int));
            LocalBuilder store_data_0x01 = generator.DeclareLocal(typeof(uint));
            LocalBuilder store_data_0x02 = generator.DeclareLocal(typeof(AttachmentIdentifier));
            LocalBuilder cmp_0x01 = generator.DeclareLocal(typeof(bool));
            LocalBuilder cmp_0x02 = generator.DeclareLocal(typeof(bool));
            LocalBuilder cmp_0x03 = generator.DeclareLocal(typeof(bool));
            LocalBuilder cmp_0x04 = generator.DeclareLocal(typeof(bool));
            LocalBuilder cmp_0x05 = generator.DeclareLocal(typeof(bool));
            LocalBuilder cmp_0x06 = generator.DeclareLocal(typeof(bool));
            LocalBuilder tmp_0x01 = generator.DeclareLocal(typeof(uint));
            LocalBuilder tmp_0x02 = generator.DeclareLocal(typeof(AttachmentIdentifier));

            Label ret = generator.DefineLabel();
            Label rep = generator.DefineLabel();
            Label al = generator.DefineLabel();
            Label jmp = generator.DefineLabel();
            Label jcc = generator.DefineLabel();
            Label blMul = generator.DefineLabel();
            Label cmovge = generator.DefineLabel();
            Label cmovgt = generator.DefineLabel();
            Label cdcRep = generator.DefineLabel();
            Label jmpRep = generator.DefineLabel();
            Label lpHd = generator.DefineLabel();
            Label fndRep = generator.DefineLabel();
            Label fndJmp = generator.DefineLabel();
            Label fndJcc = generator.DefineLabel();
            Label fndLpHd = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(AttachmentsChangeRequest), nameof(AttachmentsChangeRequest.AttachmentsCode))),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(AttachmentsUtils), nameof(AttachmentsUtils.GetCurrentAttachmentsCode))),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brtrue_S, ret),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Stloc_S, wCode_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Stloc_S, wCode_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(AttachmentsChangeRequest), nameof(AttachmentsChangeRequest.AttachmentsCode))),
                new CodeInstruction(OpCodes.Stloc_S, wCode_0x03.LocalIndex),
                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Stloc_S, attachment_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Stloc_S, attachment_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Stloc_S, tmp_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Stloc_S, repIter_1.LocalIndex),
                new CodeInstruction(OpCodes.Br_S, rep),
                new CodeInstruction(OpCodes.Nop).WithLabels(lpHd),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Firearm), nameof(Firearm.Attachments))),
                new CodeInstruction(OpCodes.Ldloc_S, repIter_1.LocalIndex),
                new CodeInstruction(OpCodes.Ldelem_Ref),
                new CodeInstruction(OpCodes.Stloc_S, repIter_Id.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, repIter_Id.LocalIndex),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(FirearmAttachment), nameof(FirearmAttachment.IsEnabled))),
                new CodeInstruction(OpCodes.Brfalse_S, al),
                new CodeInstruction(OpCodes.Ldloc_S, wCode_0x03.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, tmp_0x01.LocalIndex),
                new CodeInstruction(OpCodes.And),
                new CodeInstruction(OpCodes.Ldloc_S, tmp_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Br_S, jmp),
                new CodeInstruction(OpCodes.Ldc_I4_0).WithLabels(al),
                new CodeInstruction(OpCodes.Stloc_S, cmp_0x01.LocalIndex).WithLabels(jmp),
                new CodeInstruction(OpCodes.Ldloc_S, cmp_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse_S, jcc),
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Ldloc_S, repIter_Id.LocalIndex),
                new CodeInstruction(OpCodes.Stloc_S, attachment_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, tmp_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Stloc_S, wCode_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Br_S, blMul),
                new CodeInstruction(OpCodes.Ldloc_S, repIter_Id.LocalIndex).WithLabels(jcc),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(FirearmAttachment), nameof(FirearmAttachment.IsEnabled))),
                new CodeInstruction(OpCodes.Brtrue_S, cmovge),
                new CodeInstruction(OpCodes.Ldloc_S, wCode_0x03.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, tmp_0x01.LocalIndex),
                new CodeInstruction(OpCodes.And),
                new CodeInstruction(OpCodes.Ldloc_S, tmp_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Br_S, cmovgt),
                new CodeInstruction(OpCodes.Ldc_I4_0).WithLabels(cmovge),
                new CodeInstruction(OpCodes.Stloc_S, cmp_0x02.LocalIndex).WithLabels(cmovgt),
                new CodeInstruction(OpCodes.Ldloc_S, cmp_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse_S, blMul),
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Ldloc_S, repIter_Id.LocalIndex),
                new CodeInstruction(OpCodes.Stloc_S, attachment_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, tmp_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Stloc_S, wCode_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Ldloc_S, tmp_0x01.LocalIndex).WithLabels(blMul),
                new CodeInstruction(OpCodes.Ldc_I4_2),
                new CodeInstruction(OpCodes.Mul),
                new CodeInstruction(OpCodes.Stloc_S, tmp_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Nop),
                new CodeInstruction(OpCodes.Ldloc_S, repIter_1.LocalIndex),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Add),
                new CodeInstruction(OpCodes.Stloc_S, repIter_1.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, repIter_1.LocalIndex).WithLabels(rep),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Firearm), nameof(Firearm.Attachments))),
                new CodeInstruction(OpCodes.Ldlen),
                new CodeInstruction(OpCodes.Conv_I4),
                new CodeInstruction(OpCodes.Clt),
                new CodeInstruction(OpCodes.Stloc_S, cmp_0x03.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, cmp_0x03.LocalIndex),
                new CodeInstruction(OpCodes.Brtrue_S, lpHd),
                new CodeInstruction(OpCodes.Ldloca_S, identifier_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Initobj, typeof(AttachmentIdentifier)),
                new CodeInstruction(OpCodes.Ldloca_S, identifier_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Initobj, typeof(AttachmentIdentifier)),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(API.Features.Items.Firearm), nameof(API.Features.Items.Firearm.AvailableAttachments))),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Firearm), nameof(Firearm.ItemTypeId))),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Dictionary<ItemType, AttachmentIdentifier[]>), "get_Item", new[] { typeof(ItemType) })),
                new CodeInstruction(OpCodes.Stloc_S, un_M_arr.LocalIndex),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Stloc_S, repIter_2),
                new CodeInstruction(OpCodes.Br_S, fndRep),
                new CodeInstruction(OpCodes.Nop).WithLabels(fndLpHd),
                new CodeInstruction(OpCodes.Ldloc_S, un_M_arr.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, repIter_2.LocalIndex),
                new CodeInstruction(OpCodes.Ldelem, typeof(AttachmentIdentifier)),
                new CodeInstruction(OpCodes.Stloc_S, tmp_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Ldloca_S, tmp_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(AttachmentIdentifier), nameof(AttachmentIdentifier.Code))),
                new CodeInstruction(OpCodes.Ldloc_S, wCode_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Stloc_S, cmp_0x04.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, cmp_0x04.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse_S, fndJcc),
                new CodeInstruction(OpCodes.Ldloc_S, tmp_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Stloc_S, identifier_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Br_S, fndJmp),
                new CodeInstruction(OpCodes.Ldloca_S, tmp_0x02.LocalIndex).WithLabels(fndJcc),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(AttachmentIdentifier), nameof(AttachmentIdentifier.Code))),
                new CodeInstruction(OpCodes.Ldloc_S, wCode_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Stloc_S, cmp_0x05.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, cmp_0x05.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse_S, fndJmp),
                new CodeInstruction(OpCodes.Ldloc_S, tmp_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Stloc_S, identifier_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Nop).WithLabels(fndJmp),
                new CodeInstruction(OpCodes.Ldloc_S, repIter_2.LocalIndex),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Add),
                new CodeInstruction(OpCodes.Stloc_S, repIter_2.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, repIter_2.LocalIndex).WithLabels(fndRep),
                new CodeInstruction(OpCodes.Ldloc_S, un_M_arr.LocalIndex),
                new CodeInstruction(OpCodes.Ldlen),
                new CodeInstruction(OpCodes.Conv_I4),
                new CodeInstruction(OpCodes.Clt),
                new CodeInstruction(OpCodes.Stloc_S, cmp_0x06.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, cmp_0x06.LocalIndex),
                new CodeInstruction(OpCodes.Brtrue_S, fndLpHd),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(NetworkConnection), nameof(NetworkConnection.identity))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(NetworkIdentity), nameof(NetworkIdentity.netId))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(uint) })),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldloc_S, identifier_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, identifier_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingAttachmentsEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnChangingAttachments))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingAttachmentsEventArgs), nameof(ChangingAttachmentsEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, ret),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingAttachmentsEventArgs), nameof(ChangingAttachmentsEventArgs.NewAttachmentIdentifier))),
                new CodeInstruction(OpCodes.Stloc_S, store_data_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Ldloca_S, store_data_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(AttachmentIdentifier), nameof(AttachmentIdentifier.Code))),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(AttachmentsUtils), nameof(AttachmentsUtils.GetCurrentAttachmentsCode))),
                new CodeInstruction(OpCodes.Add),
                new CodeInstruction(OpCodes.Ldloca_S, identifier_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(AttachmentIdentifier), nameof(AttachmentIdentifier.Code))),
                new CodeInstruction(OpCodes.Sub),
                new CodeInstruction(OpCodes.Stloc_S, wCode_0x04.LocalIndex),
                new CodeInstruction(OpCodes.Ldarga_S, 1),
                new CodeInstruction(OpCodes.Ldloc_S, wCode_0x04.LocalIndex),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(AttachmentsChangeRequest), nameof(AttachmentsChangeRequest.AttachmentsCode))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
