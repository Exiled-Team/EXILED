// -----------------------------------------------------------------------
// <copyright file="ReceivingPreference.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Item
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Item;

    using Handlers;

    using HarmonyLib;

    using InventorySystem.Items.Firearms.Attachments;

    using Mirror;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    /// Patches
    /// <see cref="AttachmentsServerHandler.ServerReceivePreference(NetworkConnection, AttachmentsSetupPreference)" />.
    /// Adds the <see cref="Item.ReceivingPreference" /> event.
    /// </summary>
    [EventPatch(typeof(Item), nameof(Item.ReceivingPreference))]
    [HarmonyPatch(typeof(AttachmentsServerHandler), nameof(AttachmentsServerHandler.ServerReceivePreference))]
    internal static class ReceivingPreference
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldloc_1);

            LocalBuilder ev = generator.DeclareLocal(typeof(ReceivingPreferenceEventArgs));
            LocalBuilder curCode = generator.DeclareLocal(typeof(uint));

            Label cdc = generator.DefineLabel();
            Label ret = generator.DefineLabel();

            newInstructions[index].labels.Add(cdc);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // dictionary::TryGetValue(AttachmentsSetupPreference::Weapon, *mem_0x02)
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(AttachmentsSetupPreference), nameof(AttachmentsSetupPreference.Weapon))),
                    new(OpCodes.Ldloca_S, curCode.LocalIndex),
                    new(OpCodes.Callvirt, Method(typeof(Dictionary<ItemType, uint>), nameof(Dictionary<ItemType, uint>.TryGetValue))),
                    new(OpCodes.Brfalse_S, cdc),

                    // API::Features::Player::Get(referenceHub)
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // AttachmentsSetupPreference::Weapon
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(AttachmentsSetupPreference), nameof(AttachmentsSetupPreference.Weapon))),

                    // currentCode
                    new(OpCodes.Ldloc_S, curCode.LocalIndex),

                    // AttachmentsSetupPreference::AttachmentsCode
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(AttachmentsSetupPreference), nameof(AttachmentsSetupPreference.AttachmentsCode))),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ReceivingPreferenceEventArgs ev = new ReceivingPreferenceEventArgs(__ARGS__)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ReceivingPreferenceEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers::Item::OnReceivingPreference(ev)
                    new(OpCodes.Call, Method(typeof(Item), nameof(Item.OnReceivingPreference))),

                    // ev.IsAllowed
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingPreferenceEventArgs), nameof(ReceivingPreferenceEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, ret),

                    // **AttachmentSetupPreference::AttachmentsCode = ev::NewCode
                    new(OpCodes.Ldarga_S, 1),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ReceivingPreferenceEventArgs), nameof(ReceivingPreferenceEventArgs.NewCode))),
                    new(OpCodes.Stfld, Field(typeof(AttachmentsSetupPreference), nameof(AttachmentsSetupPreference.AttachmentsCode))),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}