// -----------------------------------------------------------------------
// <copyright file="AddingTarget.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp096
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

    using Scp096 = PlayableScps.Scp096;

    /// <summary>
    /// Patches <see cref="Scp096.AddTarget"/>.
    /// Adds the <see cref="Handlers.Scp096.AddingTarget"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp096), nameof(Scp096.AddTarget))]
    internal static class AddingTarget
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            var offset = 1;

            // Search for the third "ldarg.0".
            var index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            // Get the return label from the "ret" instruction.
            var returnLabel = newInstructions[index - offset].labels[0];

            // Declare AddingTargetEventArgs, to be able to store its instance with "stloc.s".
            var ev = generator.DeclareLocal(typeof(AddingTargetEventArgs));

            // var ev = new AddingTargetEventArgs(Player.Get(this.Hub), Player.Get(target), 70, this.EnrageTimePerReset);
            //
            // Handlers.Scp096.OnAddingTarget(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(this.Hub)
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Scp096), nameof(Scp096.Hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Player.Get(target)
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),

                // 70
                new CodeInstruction(OpCodes.Ldc_I4_S, 70),

                // this.EnrageTimePerReset;
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Scp096), nameof(Scp096.EnrageTimePerReset))),

                // true
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // var ev = new AddingTargetEventArgs(...)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(AddingTargetEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Scp096.OnAddingTarget(ev)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp096), nameof(Handlers.Scp096.OnAddingTarget))),

                // if (!ev.IsAllowed)
                //   return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(AddingTargetEventArgs), nameof(AddingTargetEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            offset = -1;
            var instructionsToRemove = 2;

            // Search for the sixth "ldarg.0".
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Call &&
            (MethodInfo)instruction.operand == Method(typeof(Scp096), nameof(Scp096.AddReset))) + offset;

            // Extract all labels from it.
            var addResetLabels = newInstructions[index].ExtractLabels();

            // Remove "this.AddReset()"
            newInstructions.RemoveRange(index, instructionsToRemove);

            newInstructions.InsertRange(index, new[]
            {
                // this.EnrageTimeLeft += ev.EnrageTimeToAdd
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(addResetLabels),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Scp096), nameof(Scp096.EnrageTimeLeft))),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(AddingTargetEventArgs), nameof(AddingTargetEventArgs.EnrageTimeToAdd))),
                new CodeInstruction(OpCodes.Add),
                new CodeInstruction(OpCodes.Call, PropertySetter(typeof(Scp096), nameof(Scp096.EnrageTimeLeft))),

                // this.AddedTimeThisRage += ev.EnrageTimeToAdd
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Scp096), nameof(Scp096.AddedTimeThisRage))),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(AddingTargetEventArgs), nameof(AddingTargetEventArgs.EnrageTimeToAdd))),
                new CodeInstruction(OpCodes.Add),
                new CodeInstruction(OpCodes.Call, PropertySetter(typeof(Scp096), nameof(Scp096.AddedTimeThisRage))),
            });

            // Get the index of the last "ldc.i4.s".
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_S);

            // Remove "ldc.i4.s" instruction.
            newInstructions.RemoveAt(index);

            // Load "ev.AhpToAdd" instead of 70.
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(AddingTargetEventArgs), nameof(AddingTargetEventArgs.AhpToAdd))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
