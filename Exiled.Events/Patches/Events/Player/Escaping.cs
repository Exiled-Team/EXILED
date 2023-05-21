// -----------------------------------------------------------------------
// <copyright file="Escaping.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1402 // File may only contain a single type

    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.API.Enums;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Escape.ServerHandlePlayer(ReferenceHub)"/> for <see cref="Handlers.Player.Escaping"/>.
    /// </summary>
    [HarmonyPatch(typeof(Escape), nameof(Escape.ServerHandlePlayer))]
    internal static class Escaping
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(EscapingEventArgs));

            const int offset = 2;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Call) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // if (escapeScenario == Escape.EscapeScenarioType.None) return;
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Beq_S, returnLabel),

                    // Player.Get(hub)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // roleTypeId
                    new(OpCodes.Ldloc_0),

                    // escapeScenario
                    new(OpCodes.Ldloc_1),

                    // EscapingEventArgs ev = new(Player, RoleTypeId, EscapeScenario)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(EscapingEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Handlers.Player.OnEscaping(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnEscaping))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EscapingEventArgs), nameof(EscapingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),

                    // roleTypeId = ev.NewRole
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EscapingEventArgs), nameof(EscapingEventArgs.NewRole))),
                    new(OpCodes.Stloc_0),

                    // escapeScenarioType = ev.EscapeScenario
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EscapingEventArgs), nameof(EscapingEventArgs.EscapeScenario))),
                    new(OpCodes.Stloc_1),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="Escape.ServerGetScenario(ReferenceHub)"/> for <see cref="Handlers.Player.Escaping"/>.
    /// Replace <see cref="EscapeScenario.None"/> to make <see cref="EscapeScenario.CustomEscape"/>.
    /// </summary>
    [HarmonyPatch(typeof(Escape), nameof(Escape.ServerGetScenario))]
    internal static class GetScenario
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int e = 0;
            for (int i = 0; i < newInstructions.Count; i++)
            {
                CodeInstruction codeInstruction = newInstructions[i];
                if (codeInstruction.opcode == OpCodes.Ldc_I4_0)
                {
                    e++;
                    if (e > 3)
                    {
                        newInstructions[i].opcode = OpCodes.Ldc_I4_5;
                    }
                }
            }

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}