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
    /// Patches <see cref="AttachmentsServerHandler.ServerReceivePreference(NetworkConnection, AttachmentsSetupPreference)"/>.
    /// Adds the <see cref="Handlers.Item.ReceivingPreference"/> event.
    /// </summary>
    [HarmonyPatch(typeof(AttachmentsServerHandler), nameof(AttachmentsServerHandler.ServerReceivePreference))]
    internal static class ReceivingPreference
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldsfld);

            LocalBuilder ev = generator.DeclareLocal(typeof(ReceivingPreferenceEventArgs));

            LocalBuilder mem_0x01 = generator.DeclareLocal(typeof(AttachmentIdentifier[]));
            LocalBuilder mem_0x02 = generator.DeclareLocal(typeof(uint));
            LocalBuilder mem_0x03 = generator.DeclareLocal(typeof(List<AttachmentIdentifier>));
            LocalBuilder mem_0x04 = generator.DeclareLocal(typeof(uint));

            Label cdc = generator.DefineLabel();
            Label ret = generator.DefineLabel();
            Label std_ovf = generator.DefineLabel();

            newInstructions[index].labels.Add(cdc);

            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldloc_1);

            newInstructions.InsertRange(index, new[]
            {
                // referenceHub ?
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Brfalse_S, ret),

                // referenceHub::characterClassManager::get_NetworkCurClass == 2
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.characterClassManager))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkCurClass))),
                new CodeInstruction(OpCodes.Ldc_I4_2),
                new CodeInstruction(OpCodes.Ceq),

                // Runs the original code
                new CodeInstruction(OpCodes.Brfalse_S, cdc),

                // dictionary::TryGetValue(AttachmentsSetupPreference::Weapon, *mem_0x02)
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(AttachmentsSetupPreference), nameof(AttachmentsSetupPreference.Weapon))),
                new CodeInstruction(OpCodes.Ldloca_S, mem_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Dictionary<ItemType, uint>), nameof(Dictionary<ItemType, uint>.TryGetValue))),

                // Runs the original code
                new CodeInstruction(OpCodes.Brfalse_S, cdc),

                // AttachmentsSetupPreference::AttachmentsCode == mem_0x02
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(AttachmentsSetupPreference), nameof(AttachmentsSetupPreference.AttachmentsCode))),
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Ceq),

                // Runs the original code
                new CodeInstruction(OpCodes.Brtrue_S, cdc),

                // mem_0x01 = null
                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Stloc_S, mem_0x01.LocalIndex),

                // mem_0x03 = new List<AttachmentIdentifier>()
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(List<AttachmentIdentifier>))[0]),
                new CodeInstruction(OpCodes.Stloc_S, mem_0x03.LocalIndex),

                // mem_0x01 = ItemExtensions::GetAttachments(AttachmentsSetupPreference::Weapon, mem_0x02)::ToList()
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(AttachmentsSetupPreference), nameof(AttachmentsSetupPreference.Weapon))),
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(ItemExtensions), nameof(ItemExtensions.GetAttachmentIdentifiers), new[] { typeof(ItemType), typeof(uint) })),
                new CodeInstruction(OpCodes.Call, Method(typeof(Enumerable), nameof(Enumerable.ToArray)).MakeGenericMethod(typeof(AttachmentIdentifier))),
                new CodeInstruction(OpCodes.Stloc_S, mem_0x01.LocalIndex),

                // mem_0x03 = ItemExtensions::GetAttachments(AttachmentsSetupPreference::Weapon, AttachmentsSetupPreference::AttachmentsCode)::ToList()
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(AttachmentsSetupPreference), nameof(AttachmentsSetupPreference.Weapon))),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(AttachmentsSetupPreference), nameof(AttachmentsSetupPreference.AttachmentsCode))),
                new CodeInstruction(OpCodes.Call, Method(typeof(ItemExtensions), nameof(ItemExtensions.GetAttachmentIdentifiers), new[] { typeof(ItemType), typeof(uint) })),
                new CodeInstruction(OpCodes.Call, Method(typeof(Enumerable), nameof(Enumerable.ToList)).MakeGenericMethod(typeof(AttachmentIdentifier))),
                new CodeInstruction(OpCodes.Stloc_S, mem_0x03.LocalIndex),

                // API::Features::Player::Get(referenceHub)
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // AttachmentsSetupPreference::Weapon
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(AttachmentsSetupPreference), nameof(AttachmentsSetupPreference.Weapon))),

                // mem_0x01
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x01.LocalIndex),

                // mem_0x03
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x03.LocalIndex),

                // true
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // ReceivingPreferenceEventArgs ev = new ReceivingPreferenceEventArgs(__ARGS__)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ReceivingPreferenceEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers::Item::OnReceivingPreference(ev)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnReceivingPreference))),

                // ev.IsAllowed
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingPreferenceEventArgs), nameof(ReceivingPreferenceEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, ret),

                // mem_0x03 = ev::NewAttachmentIdentifiers
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingPreferenceEventArgs), nameof(ReceivingPreferenceEventArgs.NewAttachmentIdentifiers))),
                new CodeInstruction(OpCodes.Stloc_S, mem_0x03.LocalIndex),

                // mem_0x03::Count()
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x03.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(List<AttachmentIdentifier>), nameof(List<AttachmentIdentifier>.Count))),
                new CodeInstruction(OpCodes.Brfalse_S, std_ovf),

                // mem_0x04 = ItemExtensions.GetBaseCode(ev::Item)
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingPreferenceEventArgs), nameof(ReceivingPreferenceEventArgs.Item))),
                new CodeInstruction(OpCodes.Call, Method(typeof(ItemExtensions), nameof(ItemExtensions.GetBaseCode))),
                new CodeInstruction(OpCodes.Conv_I4),
                new CodeInstruction(OpCodes.Stloc_S, mem_0x04.LocalIndex),

                // ItemExtensions::GetAttachmentsCode(mem_0x03)
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x03.LocalIndex).WithLabels(std_ovf),
                new CodeInstruction(OpCodes.Call, Method(typeof(ItemExtensions), nameof(ItemExtensions.GetAttachmentsCode))),
                new CodeInstruction(OpCodes.Conv_I4),
                new CodeInstruction(OpCodes.Stloc_S, mem_0x04.LocalIndex),

                // ItemExtensions::IsWeapon(ev::Item, false)
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingPreferenceEventArgs), nameof(ReceivingPreferenceEventArgs.Item))),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Call, Method(typeof(ItemExtensions), nameof(ItemExtensions.IsWeapon))),

                // Runs the original code
                new CodeInstruction(OpCodes.Brfalse_S, cdc),

                // **AttachmentSetupPreference::Weapon = ev::Item
                new CodeInstruction(OpCodes.Ldarga_S, 1),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingPreferenceEventArgs), nameof(ReceivingPreferenceEventArgs.Item))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(AttachmentsSetupPreference), nameof(AttachmentsSetupPreference.Weapon))),

                // **AttachmentSetupPreference::AttachmentsCode = ItemExtensions::GetBaseCode(ev::Item)
                new CodeInstruction(OpCodes.Ldarga_S, 1),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingPreferenceEventArgs), nameof(ReceivingPreferenceEventArgs.Item))),
                new CodeInstruction(OpCodes.Call, Method(typeof(ItemExtensions), nameof(ItemExtensions.GetBaseCode))),
                new CodeInstruction(OpCodes.Conv_I4),
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x04.LocalIndex),
                new CodeInstruction(OpCodes.Add),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(AttachmentsSetupPreference), nameof(AttachmentsSetupPreference.AttachmentsCode))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
