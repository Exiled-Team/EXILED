// -----------------------------------------------------------------------
// <copyright file="AddingTarget.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp096
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Scp096;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using PlayableScps;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Scp096 = PlayableScps.Scp096;

    /// <summary>
    ///     Patches <see cref="PlayableScps.Scp096.AddTarget" />.
    ///     Adds the <see cref="Handlers.Scp096.AddingTarget" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp096), nameof(Scp096.AddTarget))]
    internal static class AddingTarget
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            int offset = 1;

            // Search for the third "ldarg.0".
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            // Get the return label from the "ret" instruction.
            Label returnLabel = newInstructions[index - offset].labels[0];

            // Declare AddingTargetEventArgs, to be able to store its instance with "stloc.s".
            LocalBuilder ev = generator.DeclareLocal(typeof(AddingTargetEventArgs));

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
                new(OpCodes.Ldfld, Field(typeof(Scp096), nameof(Scp096.Hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Player.Get(target)
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),

                // AddingTarget.GetRageTime(this)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(AddingTarget), nameof(GetRageTime))),

                // true
                new(OpCodes.Ldc_I4_1),

                // var ev = new AddingTargetEventArgs(...)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AddingTargetEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Scp096.OnAddingTarget(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Scp096), nameof(Handlers.Scp096.OnAddingTarget))),

                // if (!ev.IsAllowed)
                //   return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(AddingTargetEventArgs), nameof(AddingTargetEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
            });

            offset = -1;
            int instructionsToRemove = 2;

            // Search for the sixth "ldarg.0".
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Call &&
                                                                 (MethodInfo) instruction.operand == Method(typeof(Scp096), nameof(Scp096.AddReset))) + offset;

            // Extract all labels from it.
            List<Label> addResetLabels = newInstructions[index].ExtractLabels();

            // Get the return label to next instruction in "this.AddReset()".
            Label exitLabel = newInstructions[index + 2].labels[0];

            // Declare timeToAdd, to be able to temp its float with "stloc".
            LocalBuilder timeToAdd = generator.DeclareLocal(typeof(float));

            // Remove "this.AddReset()"
            newInstructions.RemoveRange(index, instructionsToRemove);

            newInstructions.InsertRange(index, new[]
            {
                // timeToAdd = this.AddedTimeThisRage + ev.EnrageTimeToAdd
                // if (timeToadd > this.MaximumAddedEnrageTime)
                //     return;
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(addResetLabels),
                new(OpCodes.Call, PropertyGetter(typeof(Scp096), nameof(Scp096.AddedTimeThisRage))),
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AddingTargetEventArgs), nameof(AddingTargetEventArgs.EnrageTimeToAdd))),
                new(OpCodes.Add),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, timeToAdd.LocalIndex),
                new(OpCodes.Ldc_R4, Scp096.MaximumAddedEnrageTime),
                new(OpCodes.Bgt_Un_S, exitLabel),

                // this.EnrageTimeLeft += ev.EnrageTimeToAdd;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Dup),
                new(OpCodes.Call, PropertyGetter(typeof(Scp096), nameof(Scp096.EnrageTimeLeft))),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AddingTargetEventArgs), nameof(AddingTargetEventArgs.EnrageTimeToAdd))),
                new(OpCodes.Add),
                new(OpCodes.Call, PropertySetter(typeof(Scp096), nameof(Scp096.EnrageTimeLeft))),

                // this.AddedTimeThisRage += ev.EnrageTimeToAdd;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Dup),
                new(OpCodes.Call, PropertyGetter(typeof(Scp096), nameof(Scp096.AddedTimeThisRage))),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AddingTargetEventArgs), nameof(AddingTargetEventArgs.EnrageTimeToAdd))),
                new(OpCodes.Add),
                new(OpCodes.Call, PropertySetter(typeof(Scp096), nameof(Scp096.AddedTimeThisRage))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static float GetRageTime(Scp096 scp096)
        {
            bool isValidTimeAddition = scp096.PlayerState == Scp096PlayerState.Docile ||
                                       scp096.PlayerState == Scp096PlayerState.TryNotToCry ||
                                       scp096.PlayerState == Scp096PlayerState.Enraging;

            return isValidTimeAddition ? scp096.EnrageTimePerReset : 0f;
        }
    }
}
