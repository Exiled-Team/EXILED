// -----------------------------------------------------------------------
// <copyright file="Stopping.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Warhead
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Warhead;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Warhead = Exiled.Events.Handlers.Warhead;

    /// <summary>
    ///     Patches <see cref="AlphaWarheadController.CancelDetonation(GameObject)" />.
    ///     Adds the <see cref="Handlers.Warhead.Stopping" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Warhead), nameof(Handlers.Warhead.Stopping))]
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.CancelDetonation), typeof(GameObject))]
    internal static class Stopping
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // Search for "call" with method "ServerLogs::AddLog" and then add 1 to insert method after "ServerLogs::AddLog".
            int index = newInstructions.FindIndex(instruction => (instruction.opcode == OpCodes.Call) && ((MethodInfo)instruction.operand == Method(typeof(ServerLogs), nameof(ServerLogs.AddLog)))) + 1;

            // Get the count to find the previous index
            int oldCount = newInstructions.Count;

            // Generate the return label.
            Label returnLabel = generator.DefineLabel();

            // if(!this.inProgress)
            //   return;
            //
            // var ev = new StoppingEventArgs(Player.Get(disabler), true);
            //
            // Handlers.Warhead.OnStopping(ev);
            //
            // if(!ev.IsAllowed || Warhead.IsWarheadLocked)
            //   return;
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(AlphaWarheadController), nameof(AlphaWarheadController.inProgress))),
                    new(OpCodes.Brfalse_S, returnLabel),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StoppingEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Call, Method(typeof(Warhead), nameof(Warhead.OnStopping))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(StoppingEventArgs), nameof(StoppingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                    new(OpCodes.Call, PropertyGetter(typeof(API.Features.Warhead), nameof(API.Features.Warhead.IsLocked))),
                    new(OpCodes.Brtrue_S, returnLabel),
                });

            // Add the starting labels to the first injected instruction.
            // Calculate the difference and get the valid index - is better and easy than using a list
            newInstructions[index].MoveLabelsFrom(newInstructions[(newInstructions.Count - oldCount) + index]);

            // Add the label to the last instruction.
            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}