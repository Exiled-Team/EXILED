// -----------------------------------------------------------------------
// <copyright file="EscapingAndFailingEscapePocketDimension.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
#pragma warning disable SA1313
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PocketDimensionTeleport.OnTriggerEnter(Collider)"/>.
    /// Adds the <see cref="Handlers.Player.EscapingPocketDimension"/> and <see cref="Handlers.Player.FailingEscapePocketDimension"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.OnTriggerEnter))]
    internal static class EscapingAndFailingEscapePocketDimension
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = new List<CodeInstruction>(instructions);

            // --------- FailingEscapePocketDimension ---------

            // The index offset.
            var offset = 2;

            // Find the starting index by searching for "ldfld" of "BlastDoor.isClosed".
            var index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldfld &&
            (FieldInfo)instruction.operand == Field(typeof(BlastDoor), nameof(BlastDoor.isClosed))) + offset;

            // Get the starting label and remove all of them from the original instruction.
            var startingLabel = newInstructions[index].labels[0];
            newInstructions[index].labels.Clear();

            // Get the return label from the last instruction.
            var returnLabel = newInstructions[newInstructions.Count - 1].labels[0];

            // var ev = new FailingEscapePocketDimensionEventArgs(Player.Get(other.gameObject), this);
            //
            // Handlers.Player.OnFailingEscapePocketDimension(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(FailingEscapePocketDimensionEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnFailingEscapePocketDimension))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(FailingEscapePocketDimensionEventArgs), nameof(FailingEscapePocketDimensionEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            // Add the starting label to the first injected instruction.
            newInstructions[index].labels.Add(startingLabel);

            // --------- EscapingPocketDimension ---------

            // The index offset.
            offset = -1;

            // Find the starting index by searching for "callvirt" of "Component.GetComponent<PlayerMovementSync>()".
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Callvirt &&
            (MethodInfo)instruction.operand == Method(typeof(Component), nameof(Component.GetComponent), generics: new[] { typeof(PlayerMovementSync) })) + offset;

            // Declare EscapingPocketDimensionEventArgs local variable.
            var ev = generator.DeclareLocal(typeof(EscapingPocketDimensionEventArgs));

            // var ev = new EscapingPocketDimensionEventArgs(API.Features.Player.Get(other.gameObject), tpPosition);
            //
            // Player.OnEscapingPocketDimension(ev);
            //
            // if (!ev.IsAllowed)
            //  return;
            //
            // tpPosition = ev.TeleportPosition;
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),
                new CodeInstruction(OpCodes.Ldloc_S, 4),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(EscapingPocketDimensionEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnEscapingPocketDimension))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(EscapingPocketDimensionEventArgs), nameof(EscapingPocketDimensionEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(EscapingPocketDimensionEventArgs), nameof(EscapingPocketDimensionEventArgs.TeleportPosition))),
                new CodeInstruction(OpCodes.Stloc_S, 4),
            });

            return newInstructions;
        }
    }
}
