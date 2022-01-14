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

            // Values to keep track of both the original codes and the results obtained from IL calculations
            LocalBuilder wCode_0x01 = generator.DeclareLocal(typeof(uint));
            LocalBuilder wCode_0x02 = generator.DeclareLocal(typeof(uint));
            LocalBuilder wCode_0x03 = generator.DeclareLocal(typeof(uint));
            LocalBuilder wCode_0x04 = generator.DeclareLocal(typeof(uint));

            // Values to keep track of both the old and new attachment
            LocalBuilder attachment_0x01 = generator.DeclareLocal(typeof(FirearmAttachment));
            LocalBuilder attachment_0x02 = generator.DeclareLocal(typeof(FirearmAttachment));

            // Values to use for iterations
            LocalBuilder repIter_1 = generator.DeclareLocal(typeof(int));
            LocalBuilder repIter_Id = generator.DeclareLocal(typeof(FirearmAttachment));

            // Values to keep track of both the old and new identifier
            LocalBuilder identifier_0x01 = generator.DeclareLocal(typeof(AttachmentIdentifier));
            LocalBuilder identifier_0x02 = generator.DeclareLocal(typeof(AttachmentIdentifier));

            // Value to keep track of the iteration results
            LocalBuilder un_M_arr = generator.DeclareLocal(typeof(AttachmentIdentifier[]));

            // The event to implement
            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingAttachmentsEventArgs));

            // Value to use for iterations
            LocalBuilder repIter_2 = generator.DeclareLocal(typeof(int));

            // Values to extend results obtained from iterations involving memory addresses
            LocalBuilder store_data_0x01 = generator.DeclareLocal(typeof(uint));
            LocalBuilder store_data_0x02 = generator.DeclareLocal(typeof(AttachmentIdentifier));

            // Values to store results given by IL temp calculations
            LocalBuilder cmp_0x01 = generator.DeclareLocal(typeof(bool));
            LocalBuilder cmp_0x02 = generator.DeclareLocal(typeof(bool));
            LocalBuilder cmp_0x03 = generator.DeclareLocal(typeof(bool));
            LocalBuilder cmp_0x04 = generator.DeclareLocal(typeof(bool));
            LocalBuilder cmp_0x05 = generator.DeclareLocal(typeof(bool));
            LocalBuilder cmp_0x06 = generator.DeclareLocal(typeof(bool));

            // Values to store results temporarily
            LocalBuilder tmp_0x01 = generator.DeclareLocal(typeof(uint));
            LocalBuilder tmp_0x02 = generator.DeclareLocal(typeof(AttachmentIdentifier));

            // The return label
            Label ret = generator.DefineLabel();

            // Labels to use for loops
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
                // If the Firearm::GetCurrentAttachmentsCode isn't changed, prevents the method from being executed
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(AttachmentsChangeRequest), nameof(AttachmentsChangeRequest.AttachmentsCode))),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(AttachmentsUtils), nameof(AttachmentsUtils.GetCurrentAttachmentsCode))),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brtrue_S, ret),

                // wCode_0x01 = 0
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Stloc_S, wCode_0x01.LocalIndex),

                // wCode_0x02 = 0
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Stloc_S, wCode_0x02.LocalIndex),

                // wCode_0x03 = AttachmentsChangeRequest::AttachmentsCode
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(AttachmentsChangeRequest), nameof(AttachmentsChangeRequest.AttachmentsCode))),
                new CodeInstruction(OpCodes.Stloc_S, wCode_0x03.LocalIndex),

                // attachment_0x01 = null
                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Stloc_S, attachment_0x01.LocalIndex),

                // attachment_0x02 = null
                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Stloc_S, attachment_0x02.LocalIndex),

                // tmp_0x01 = 1
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Stloc_S, tmp_0x01.LocalIndex),

                // repIter_1 = 0
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Stloc_S, repIter_1.LocalIndex),

                // This is a for loop
                new CodeInstruction(OpCodes.Br_S, rep),

                // Marking the Nop instruction with the loopHead label which determines the head of the loop
                new CodeInstruction(OpCodes.Nop).WithLabels(lpHd),

                // repIter_Id = Firearm::Attachments[repIter_1]
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Firearm), nameof(Firearm.Attachments))),
                new CodeInstruction(OpCodes.Ldloc_S, repIter_1.LocalIndex),
                new CodeInstruction(OpCodes.Ldelem_Ref),
                new CodeInstruction(OpCodes.Stloc_S, repIter_Id.LocalIndex),

                // repIter_Id::IsEnabled && (wCode_0x01 & tmp_0x01) != tmp_0x01
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

                // Conditions fulfilled
                new CodeInstruction(OpCodes.Nop),

                // attachment_0x02 = repIter_Id
                new CodeInstruction(OpCodes.Ldloc_S, repIter_Id.LocalIndex),
                new CodeInstruction(OpCodes.Stloc_S, attachment_0x02.LocalIndex),

                // wCode_0x01 = tmp_0x01
                new CodeInstruction(OpCodes.Ldloc_S, tmp_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Stloc_S, wCode_0x01.LocalIndex),

                new CodeInstruction(OpCodes.Nop),

                // Goes to *= tmp_0x01 which keeps track of the current attachment value
                new CodeInstruction(OpCodes.Br_S, blMul),

                // repIter_Id::IsEnabled
                new CodeInstruction(OpCodes.Ldloc_S, repIter_Id.LocalIndex).WithLabels(jcc),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(FirearmAttachment), nameof(FirearmAttachment.IsEnabled))),
                new CodeInstruction(OpCodes.Brtrue_S, cmovge),

                // !repIter_Id::IsEnabled && (wCode_0x03 & tmp_0x01) == tmp_0x01
                new CodeInstruction(OpCodes.Ldloc_S, wCode_0x03.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, tmp_0x01.LocalIndex),
                new CodeInstruction(OpCodes.And),
                new CodeInstruction(OpCodes.Ldloc_S, tmp_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Br_S, cmovgt),
                new CodeInstruction(OpCodes.Ldc_I4_0).WithLabels(cmovge),
                new CodeInstruction(OpCodes.Stloc_S, cmp_0x02.LocalIndex).WithLabels(cmovgt),
                new CodeInstruction(OpCodes.Ldloc_S, cmp_0x02.LocalIndex),

                // Goes to *= tmp_0x01 which keeps track of the current attachment value
                new CodeInstruction(OpCodes.Brfalse_S, blMul),
                new CodeInstruction(OpCodes.Nop),

                // attachment_0x01 = repIter_Id
                new CodeInstruction(OpCodes.Ldloc_S, repIter_Id.LocalIndex),
                new CodeInstruction(OpCodes.Stloc_S, attachment_0x01.LocalIndex),

                // wCode_0x02 = tmp_0x01
                new CodeInstruction(OpCodes.Ldloc_S, tmp_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Stloc_S, wCode_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Nop),

                // tmp_0x01 *= 2
                new CodeInstruction(OpCodes.Ldloc_S, tmp_0x01.LocalIndex).WithLabels(blMul),
                new CodeInstruction(OpCodes.Ldc_I4_2),
                new CodeInstruction(OpCodes.Mul),
                new CodeInstruction(OpCodes.Stloc_S, tmp_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Nop),

                // repIter_1++
                new CodeInstruction(OpCodes.Ldloc_S, repIter_1.LocalIndex),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Add),
                new CodeInstruction(OpCodes.Stloc_S, repIter_1.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, repIter_1.LocalIndex).WithLabels(rep),

                // i < Firearm::Attachments::Length
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Firearm), nameof(Firearm.Attachments))),
                new CodeInstruction(OpCodes.Ldlen),
                new CodeInstruction(OpCodes.Conv_I4),
                new CodeInstruction(OpCodes.Clt),

                // Passes the result to cmp_0x03
                new CodeInstruction(OpCodes.Stloc_S, cmp_0x03.LocalIndex),

                // cmp_0x03 ?
                new CodeInstruction(OpCodes.Ldloc_S, cmp_0x03.LocalIndex),
                new CodeInstruction(OpCodes.Brtrue_S, lpHd),

                // **identifier_0x01 = default
                new CodeInstruction(OpCodes.Ldloca_S, identifier_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Initobj, typeof(AttachmentIdentifier)),

                // **identifier_0x02 = default
                new CodeInstruction(OpCodes.Ldloca_S, identifier_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Initobj, typeof(AttachmentIdentifier)),

                // un_M_arr = API::Features::Items::Firearm::AvailableAttachments::get_Item(ItemType)
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(API.Features.Items.Firearm), nameof(API.Features.Items.Firearm.AvailableAttachments))),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Firearm), nameof(Firearm.ItemTypeId))),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Dictionary<ItemType, AttachmentIdentifier[]>), "get_Item", new[] { typeof(ItemType) })),
                new CodeInstruction(OpCodes.Stloc_S, un_M_arr.LocalIndex),

                // i = 0
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Stloc_S, repIter_2),

                // This is a for loop
                new CodeInstruction(OpCodes.Br_S, fndRep),

                // Marking the Nop instruction with the loopHead label which determines the head of the loop
                new CodeInstruction(OpCodes.Nop).WithLabels(fndLpHd),

                // tmp_0x02 = un_M_arr[repIter_2]
                new CodeInstruction(OpCodes.Ldloc_S, un_M_arr.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, repIter_2.LocalIndex),
                new CodeInstruction(OpCodes.Ldelem, typeof(AttachmentIdentifier)),
                new CodeInstruction(OpCodes.Stloc_S, tmp_0x02.LocalIndex),

                // **tmp_0x02::Code == wCode_0x02
                new CodeInstruction(OpCodes.Ldloca_S, tmp_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(AttachmentIdentifier), nameof(AttachmentIdentifier.Code))),
                new CodeInstruction(OpCodes.Ldloc_S, wCode_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Ceq),

                // Passes the result to cmp_0x04
                new CodeInstruction(OpCodes.Stloc_S, cmp_0x04.LocalIndex),

                // cmp_0x04 ?
                new CodeInstruction(OpCodes.Ldloc_S, cmp_0x04.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse_S, fndJcc),

                // identifier_0x01 = tmp_0x02
                new CodeInstruction(OpCodes.Ldloc_S, tmp_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Stloc_S, identifier_0x01.LocalIndex),

                // jumps to to fndJmp
                new CodeInstruction(OpCodes.Br_S, fndJmp),

                // **tmp_0x02::Code == wCode_0x01
                new CodeInstruction(OpCodes.Ldloca_S, tmp_0x02.LocalIndex).WithLabels(fndJcc),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(AttachmentIdentifier), nameof(AttachmentIdentifier.Code))),
                new CodeInstruction(OpCodes.Ldloc_S, wCode_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Ceq),

                // Passes the result to cmp_0x05
                new CodeInstruction(OpCodes.Stloc_S, cmp_0x05.LocalIndex),

                // cmp_0x05 ?
                new CodeInstruction(OpCodes.Ldloc_S, cmp_0x05.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse_S, fndJmp),

                // identifier_0x02 = tmp_0x02
                new CodeInstruction(OpCodes.Ldloc_S, tmp_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Stloc_S, identifier_0x02.LocalIndex),

                new CodeInstruction(OpCodes.Nop).WithLabels(fndJmp),

                // repIter_2++
                new CodeInstruction(OpCodes.Ldloc_S, repIter_2.LocalIndex),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Add),
                new CodeInstruction(OpCodes.Stloc_S, repIter_2.LocalIndex),

                // repIter_2 < un_M_arr::Length
                new CodeInstruction(OpCodes.Ldloc_S, repIter_2.LocalIndex).WithLabels(fndRep),
                new CodeInstruction(OpCodes.Ldloc_S, un_M_arr.LocalIndex),
                new CodeInstruction(OpCodes.Ldlen),
                new CodeInstruction(OpCodes.Conv_I4),
                new CodeInstruction(OpCodes.Clt),

                // Passes the result to cmp_0x06
                new CodeInstruction(OpCodes.Stloc_S, cmp_0x06.LocalIndex),

                // cmp_0x06 ?
                new CodeInstruction(OpCodes.Ldloc_S, cmp_0x06.LocalIndex),
                new CodeInstruction(OpCodes.Brtrue_S, fndLpHd),

                // API::Features::Player::Get(NetowrkConnection.identity.netId)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(NetworkConnection), nameof(NetworkConnection.identity))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(NetworkIdentity), nameof(NetworkIdentity.netId))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(uint) })),

                // firearm
                new CodeInstruction(OpCodes.Ldloc_1),

                // old identifier
                new CodeInstruction(OpCodes.Ldloc_S, identifier_0x02.LocalIndex),

                // new identifier
                new CodeInstruction(OpCodes.Ldloc_S, identifier_0x01.LocalIndex),

                // true
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // ChangingAttachmentsEventArgs ev = new ChangingAttachmentsEventArgs(__ARGS__)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingAttachmentsEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers::Item.OnChangingAttachments(ev)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnChangingAttachments))),

                // ev.IsAllowed
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingAttachmentsEventArgs), nameof(ChangingAttachmentsEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, ret),

                // store_data_0x02 = *ev.NewAttachmentIdentifier
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingAttachmentsEventArgs), nameof(ChangingAttachmentsEventArgs.NewAttachmentIdentifier))),
                new CodeInstruction(OpCodes.Stloc_S, store_data_0x02.LocalIndex),

                // wCode_0x04 = *store_data_0x02::Code + &firearm::GetCurrentAttachmentsCode - **identifier_0x02
                new CodeInstruction(OpCodes.Ldloca_S, store_data_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(AttachmentIdentifier), nameof(AttachmentIdentifier.Code))),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(AttachmentsUtils), nameof(AttachmentsUtils.GetCurrentAttachmentsCode))),
                new CodeInstruction(OpCodes.Add),
                new CodeInstruction(OpCodes.Ldloca_S, identifier_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(AttachmentIdentifier), nameof(AttachmentIdentifier.Code))),
                new CodeInstruction(OpCodes.Sub),
                new CodeInstruction(OpCodes.Stloc_S, wCode_0x04.LocalIndex),

                // **AttachmentsChangeRequest = wCode_0x04
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
