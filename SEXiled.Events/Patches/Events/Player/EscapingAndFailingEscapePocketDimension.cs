// -----------------------------------------------------------------------
// <copyright file="EscapingAndFailingEscapePocketDimension.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using SEXiled.API.Features;
    using SEXiled.Events.EventArgs;

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
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder exiledPlayerLocal = generator.DeclareLocal(typeof(Player));

            // --------- Player check ---------
            // The check is a check that this is a player, if it isn't a player, then we simply call return
            // if we don't, we'll get a NullReferenceException which is also thrown when we try to call the event.

            // Find the first null check of the NetworkIdentity component
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Call);

            // Get the return label from the instruction at the index.
            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
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

            // ----------- FailingEscapePocketDimension-------------
            int offset = 2;
            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldsfld && (FieldInfo)i.operand ==
                Field(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.DebugBool))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, exiledPlayerLocal.LocalIndex).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(FailingEscapePocketDimensionEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnFailingEscapePocketDimension))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(FailingEscapePocketDimensionEventArgs), nameof(FailingEscapePocketDimensionEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),
            });

            // --------- EscapingPocketDimension ---------

            // The index offset.
            offset = -1;

            // Find the starting index by searching for "callvirt" of "Component.GetComponent<PlayerMovementSync>()".
            index = newInstructions.FindLastIndex(i =>
                i.opcode == OpCodes.Ldfld && (FieldInfo)i.operand ==
                Field(typeof(ReferenceHub), nameof(ReferenceHub.playerMovementSync))) + offset;

            // Declare EscapingPocketDimensionEventArgs local variable.
            LocalBuilder ev = generator.DeclareLocal(typeof(EscapingPocketDimensionEventArgs));

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
                new CodeInstruction(OpCodes.Ldloc_S, 10),
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
                new CodeInstruction(OpCodes.Stloc_S, 10),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
