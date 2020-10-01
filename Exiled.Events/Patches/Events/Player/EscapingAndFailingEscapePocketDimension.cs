// -----------------------------------------------------------------------
// <copyright file="EscapingAndFailingEscapePocketDimension.cs" company="Exiled Team">
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
    /// Patches <see cref="PocketDimensionTeleport.OnTriggerEnter(Collider)"/>.
    /// Adds the <see cref="Handlers.Player.EscapingPocketDimension"/> and <see cref="Handlers.Player.FailingEscapePocketDimension"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.OnTriggerEnter))]
    internal static class EscapingAndFailingEscapePocketDimension
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            var exiledPlayerLocal = generator.DeclareLocal(typeof(Player));

            // --------- Player check ---------
            // The check is a check that this is a player, if it isn't a player, then we simply call return
            // if we don't, we'll get a NullReferenceException which is also thrown when we try to call the event.

            // Find the first null check of the NetworkIdentity component
            var index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Brfalse && instruction.operand is Label);

            // Get the return label from the instruction at the index.
            var returnLabel = newInstructions[index].operand;

            newInstructions.InsertRange(index + 1, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, exiledPlayerLocal.LocalIndex),
                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Call, Method(typeof(object), nameof(Equals), new[] { typeof(object), typeof(object) })),
                new CodeInstruction(OpCodes.Brtrue, returnLabel),
            });

            // --------- FailingEscapePocketDimension ---------

            // The index offset.
            var offset = 2;

            // Find the starting index by searching for "ldfld" of "BlastDoor.isClosed".
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldfld &&
            instruction.operand is FieldInfo finfo && finfo == Field(typeof(BlastDoor), nameof(BlastDoor.isClosed))) + offset;

            // Get the starting labels and remove all of them from the original instruction.
            var startingLabels = ListPool<Label>.Shared.Rent(newInstructions[index].labels);
            newInstructions[index].labels.Clear();

            // Get the return label from the last instruction.
            returnLabel = newInstructions[newInstructions.Count - 1].labels[0];

            // var ev = new FailingEscapePocketDimensionEventArgs(Player.Get(other.gameObject), this);
            //
            // Handlers.Player.OnFailingEscapePocketDimension(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, exiledPlayerLocal.LocalIndex),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, Constructor(typeof(FailingEscapePocketDimensionEventArgs), new[] { typeof(Player), typeof(PocketDimensionTeleport), typeof(bool) })),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnFailingEscapePocketDimension))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(FailingEscapePocketDimensionEventArgs), nameof(FailingEscapePocketDimensionEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            // Add the starting labels to the first injected instruction.
            newInstructions[index].labels.AddRange(startingLabels);

            ListPool<Label>.Shared.Return(startingLabels);

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
                new CodeInstruction(OpCodes.Ldloc_S, exiledPlayerLocal.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, 4),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, Constructor(typeof(EscapingPocketDimensionEventArgs), new[] { typeof(Player), typeof(Vector3), typeof(bool) })),
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

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
