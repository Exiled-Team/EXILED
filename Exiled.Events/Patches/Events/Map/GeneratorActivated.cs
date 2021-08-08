// -----------------------------------------------------------------------
// <copyright file="GeneratorActivated.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Generator079.CheckFinish"/>.
    /// Adds the <see cref="Map.GeneratorActivated"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Generator079), nameof(Generator079.CheckFinish))]
    internal static class GeneratorActivated
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            const int offset = 1;

            // Search for the third "ldarg.0".
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            // Get the first label of the first "ret".
            Label returnLabel = newInstructions[index - offset].labels[0];

            // var ev = new GeneratorActivatedEventArgs(this, true);
            //
            // Map.OnGeneratorActivated(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(GeneratorActivatedEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Map), nameof(Map.OnGeneratorActivated))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(GeneratorActivatedEventArgs), nameof(GeneratorActivatedEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
