// -----------------------------------------------------------------------
// <copyright file="ChangingDurability.cs" company="SEXiled Team">
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

    using InventorySystem.Items.Firearms;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Firearm = InventorySystem.Items.Firearms.Firearm;

    /// <summary>
    /// Patches <see cref="Firearm.Status"/>.
    /// Adds the <see cref="Handlers.Item.ChangingDurability"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Firearm), nameof(Firearm.Status), MethodType.Setter)]
    internal static class ChangingDurability
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Brfalse_S) + offset;

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingDurabilityEventArgs));
            LocalBuilder mem_0x01 = generator.DeclareLocal(typeof(byte));

            Label cdc = generator.DefineLabel();
            Label jmp = generator.DefineLabel();
            Label jcc = generator.DefineLabel();

            newInstructions[index].labels.Add(cdc);

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(FirearmStatus), nameof(FirearmStatus.Ammo))),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Firearm), nameof(Firearm._status))),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(FirearmStatus), nameof(FirearmStatus.Ammo))),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brtrue_S, cdc),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Firearm), nameof(Firearm.Owner))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Firearm), nameof(Firearm._status))),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(FirearmStatus), nameof(FirearmStatus.Ammo))),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(FirearmStatus), nameof(FirearmStatus.Ammo))),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingDurabilityEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnChangingDurability))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingDurabilityEventArgs), nameof(ChangingDurabilityEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, jmp),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingDurabilityEventArgs), nameof(ChangingDurabilityEventArgs.NewDurability))),
                new CodeInstruction(OpCodes.Br_S, jcc),
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(jmp),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Firearm), nameof(Firearm._status))),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(FirearmStatus), nameof(FirearmStatus.Ammo))),
                new CodeInstruction(OpCodes.Stloc_S, mem_0x01.LocalIndex).WithLabels(jcc),
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(FirearmStatus), nameof(FirearmStatus.Flags))),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(FirearmStatus), nameof(FirearmStatus.Attachments))),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(FirearmStatus))[0]),
                new CodeInstruction(OpCodes.Starg_S, 1),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
