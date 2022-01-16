// -----------------------------------------------------------------------
// <copyright file="TeslaGateInstantBurst.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="global::TeslaGate._PlayAnimation"/>.
    /// </summary>
    [HarmonyPatch(typeof(global::TeslaGate), nameof(global::TeslaGate._PlayAnimation))]
    internal static class TeslaGateInstantBurst
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = -1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stfld &&
            (FieldInfo)instruction.operand == Field(typeof(global::TeslaGate), nameof(global::TeslaGate.next079burst))) + offset;

            Label cdc = generator.DefineLabel();
            Label jcc = generator.DefineLabel();

            newInstructions[index].labels.Add(cdc);
            newInstructions[index + 2].labels.Add(jcc);

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, Method(typeof(TeslaGate), nameof(TeslaGate.Get))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(TeslaGate), nameof(TeslaGate.IsUsingInstantBursts))),
                new CodeInstruction(OpCodes.Brfalse_S, cdc),
                new CodeInstruction(OpCodes.Br_S, jcc),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
