// -----------------------------------------------------------------------
// <copyright file="ActivatingWarheadPanel.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="PlayerInteract.UserCode_CmdSwitchAWButton"/>.
    /// Adds the <see cref="Handlers.Player.ActivatingWarheadPanel"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.UserCode_CmdSwitchAWButton))]
    internal static class ActivatingWarheadPanel
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder isAllowed = generator.DeclareLocal(typeof(bool));
            LocalBuilder cmp_0x01 = generator.DeclareLocal(typeof(bool));

            Label jne = generator.DefineLabel();
            Label ceq = generator.DefineLabel();
            Label je = generator.DefineLabel();
            Label jmp = generator.DefineLabel();
            Label cmp = generator.DefineLabel();
            Label ret = generator.DefineLabel();

            int offset = 0;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Brtrue_S) + offset;
            newInstructions.RemoveAt(index);

            offset = -3;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Isinst) + offset;

            newInstructions[index].labels.Add(jne);
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Stloc_S, isAllowed.LocalIndex),
                new(OpCodes.Ldloc_S, isAllowed.LocalIndex),
                new(OpCodes.Brfalse_S, jne),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Stloc_S, isAllowed.LocalIndex),
                new(OpCodes.Br_S, jmp),
            });

            offset = 1;
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_1) + offset;
            newInstructions.RemoveAt(index);

            offset = 1;
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_1) + offset;
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Brfalse_S, ceq),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Stloc_S, cmp_0x01.LocalIndex),
                new(OpCodes.Br_S, je),
                new CodeInstruction(OpCodes.Ldc_I4_0).WithLabels(ceq),
                new(OpCodes.Stloc_S, cmp_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, cmp_0x01.LocalIndex).WithLabels(je),
                new(OpCodes.Brfalse_S, jmp),
            });

            offset = 0;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Brfalse_S) + offset;
            newInstructions.RemoveAt(index);

            offset = 0;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldloc_0) + offset;
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloc_S, cmp_0x01.LocalIndex),
                new(OpCodes.Ceq),
                new(OpCodes.Stloc_S, isAllowed.LocalIndex),
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(jmp),
                new(OpCodes.Ldfld, Field(typeof(PlayerInteract), nameof(PlayerInteract._hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldloc_S, isAllowed.LocalIndex),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ActivatingWarheadPanelEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnActivatingWarheadPanel))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingWarheadPanelEventArgs), nameof(ActivatingWarheadPanelEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, ret),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
