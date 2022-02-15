// -----------------------------------------------------------------------
// <copyright file="ReceivingPreference.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Patches.Events.Item
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using SEXiled.Events.EventArgs;

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

            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldloc_1);

            LocalBuilder ev = generator.DeclareLocal(typeof(ReceivingPreferenceEventArgs));

            LocalBuilder curCode = generator.DeclareLocal(typeof(uint));

            Label cdc = generator.DefineLabel();
            Label ret = generator.DefineLabel();

            newInstructions[index].labels.Add(cdc);

            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldloc_1);

            newInstructions.InsertRange(index, new[]
            {
                // dictionary::TryGetValue(AttachmentsSetupPreference::Weapon, *mem_0x02)
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(AttachmentsSetupPreference), nameof(AttachmentsSetupPreference.Weapon))),
                new CodeInstruction(OpCodes.Ldloca_S, curCode.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Dictionary<ItemType, uint>), nameof(Dictionary<ItemType, uint>.TryGetValue))),
                new CodeInstruction(OpCodes.Brfalse_S, cdc),

                // API::Features::Player::Get(referenceHub)
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // AttachmentsSetupPreference::Weapon
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(AttachmentsSetupPreference), nameof(AttachmentsSetupPreference.Weapon))),

                // currentCode
                new CodeInstruction(OpCodes.Ldloc_S, curCode.LocalIndex),

                // AttachmentsSetupPreference::AttachmentsCode
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(AttachmentsSetupPreference), nameof(AttachmentsSetupPreference.AttachmentsCode))),

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

                // **AttachmentSetupPreference::AttachmentsCode = ev::NewCode
                new CodeInstruction(OpCodes.Ldarga_S, 1),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingPreferenceEventArgs), nameof(ReceivingPreferenceEventArgs.NewCode))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(AttachmentsSetupPreference), nameof(AttachmentsSetupPreference.AttachmentsCode))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
