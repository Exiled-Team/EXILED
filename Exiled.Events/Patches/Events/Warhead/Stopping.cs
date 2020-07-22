// -----------------------------------------------------------------------
// <copyright file="Stopping.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Warhead
{
#pragma warning disable SA1118
#pragma warning disable SA1313
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="AlphaWarheadController.CancelDetonation(GameObject)"/>.
    /// Adds the <see cref="Warhead.Stopping"/> event.
    /// </summary>
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.CancelDetonation), new Type[] { typeof(GameObject) })]
    internal static class Stopping
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = new List<CodeInstruction>(instructions);

            // Search for "br.s" and then subtract 2 to get the index of the third "ldc.i4.0".
            var index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Br_S) - 2;

            // Generate the return label.
            var returnLabel = generator.DefineLabel();

            // Copy [Label3, Label4] from "ldc.i4.0" and then clear them.
            var startLabels = new List<Label>(newInstructions[index].labels);
            newInstructions[index].labels.Clear();

            // var ev = new StoppingEventArgs(API.Features.Player.Get(disabler), true);
            //
            // Warhead.OnStopping(ev);
            //
            // if(!ev.IsAllowed || API.Features.Warhead.IsWarheadLocked)
            //   return;
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(GameObject) })),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(StoppingEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Warhead), nameof(Warhead.OnStopping))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(StoppingEventArgs), nameof(StoppingEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(API.Features.Warhead), nameof(API.Features.Warhead.IsWarheadLocked))),
                new CodeInstruction(OpCodes.Brtrue_S, returnLabel),
            });

            // Add [Label3, Label4] to "ldc.i4.0".
            newInstructions[index].labels.AddRange(startLabels);

            // Add the label to the last "ret".
            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            return newInstructions;
        }
    }
}
