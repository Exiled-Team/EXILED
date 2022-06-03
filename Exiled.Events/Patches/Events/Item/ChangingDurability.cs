// -----------------------------------------------------------------------
// <copyright file="ChangingDurability.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Item
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

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
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(FirearmStatus), nameof(FirearmStatus.Ammo))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Firearm), nameof(Firearm._status))),
                new(OpCodes.Ldfld, Field(typeof(FirearmStatus), nameof(FirearmStatus.Ammo))),
                new(OpCodes.Ceq),
                new(OpCodes.Brtrue_S, cdc),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Firearm), nameof(Firearm.Owner))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Dup),
                new(OpCodes.Ldfld, Field(typeof(Firearm), nameof(Firearm._status))),
                new(OpCodes.Ldfld, Field(typeof(FirearmStatus), nameof(FirearmStatus.Ammo))),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(FirearmStatus), nameof(FirearmStatus.Ammo))),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingDurabilityEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnChangingDurability))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingDurabilityEventArgs), nameof(ChangingDurabilityEventArgs.Firearm))),
                new(OpCodes.Brfalse_S, cdc),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingDurabilityEventArgs), nameof(ChangingDurabilityEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, jmp),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingDurabilityEventArgs), nameof(ChangingDurabilityEventArgs.NewDurability))),
                new(OpCodes.Br_S, jcc),
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(jmp),
                new(OpCodes.Ldfld, Field(typeof(Firearm), nameof(Firearm._status))),
                new(OpCodes.Ldfld, Field(typeof(FirearmStatus), nameof(FirearmStatus.Ammo))),
                new CodeInstruction(OpCodes.Stloc_S, mem_0x01.LocalIndex).WithLabels(jcc),
                new(OpCodes.Ldloc_S, mem_0x01.LocalIndex),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(FirearmStatus), nameof(FirearmStatus.Flags))),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(FirearmStatus), nameof(FirearmStatus.Attachments))),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(FirearmStatus))[0]),
                new(OpCodes.Starg_S, 1),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
