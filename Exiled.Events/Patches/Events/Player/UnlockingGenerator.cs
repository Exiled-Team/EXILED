// -----------------------------------------------------------------------
// <copyright file="UnlockingGenerator.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Generator079.OpenClose(GameObject)"/>.
    /// Adds the <see cref="Handlers.Player.UnlockingGenerator"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Generator079), nameof(Generator079.OpenClose))]
    internal static class UnlockingGenerator
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            const int offset = -4;

            // Find the starting index by searching for "call" of "set_NetworkisDoorUnlocked".
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Call &&
                                                                 (MethodInfo)instruction.operand == PropertySetter(typeof(Generator079), nameof(Generator079.NetworkisDoorUnlocked))) + offset;

            // Get the count to find the previous index
            int oldCount = newInstructions.Count;

            // var ev = new UnlockingGeneratorEventArgs(Player.Get(person), this, flag);
            //
            // Handlers.Player.OnUnlockingGenerator(ev);
            //
            // flag = ev.IsAllowed
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(UnlockingGeneratorEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnUnlockingGenerator))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(UnlockingGeneratorEventArgs), nameof(UnlockingGeneratorEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Stloc_1),
            });

            // Add the starting labels to the first injected instruction.
            // Calculate the difference and get the valid index - is better and easy than using a list
            newInstructions[index].MoveLabelsFrom(newInstructions[newInstructions.Count - oldCount + index]);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
