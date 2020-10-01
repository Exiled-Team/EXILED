// -----------------------------------------------------------------------
// <copyright file="ChangingKnobSetting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp914
{
#pragma warning disable SA1118
    using System;

    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayerInteract.CallCmdChange914Knob"/>.
    /// Adds the <see cref="Handlers.Scp914.ChangingKnobSetting"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdChange914Knob))]
    internal static class ChangingKnobSetting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            const int offset = 0;

            // Search for the last "ldsfld".
            var index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldsfld) + offset;

            // Declare ChangingKnobSettingEventArgs local variable.
            var ev = generator.DeclareLocal(typeof(ChangingKnobSettingEventArgs));

            // Get the starting labels and remove all of them from the original instruction.
            var startingLabels = ListPool<Label>.Shared.Rent(newInstructions[index].labels);
            newInstructions[index].labels.Clear();

            // Get the return label from the last instruction.
            var returnLabel = newInstructions[index - 1].labels[0];

            // Remove "Scp914.Scp914Machine.singleton.ChangeKnobStatus()".
            newInstructions.RemoveRange(index, 2);

            // if (Math.Abs(Scp914.Scp914Machine.singleton.curKnobCooldown) > 0.001f)
            //   return;
            //
            //  var ev = new ChangingKnobSettingEventArgs(Player.Get(this.gameObject), Scp914Machine.singleton.knobState + 1);
            //
            // Handlers.Scp914.OnChangingKnobSetting(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            //
            //  Scp914Machine.singleton.curKnobCooldown = Scp914Machine.singleton.knobCooldown;
            //  Scp914Machine.singleton.NetworkknobState = ev.KnobSetting;
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(global::Scp914.Scp914Machine), nameof(global::Scp914.Scp914Machine.singleton))),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(global::Scp914.Scp914Machine), nameof(global::Scp914.Scp914Machine.curKnobCooldown))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Math), nameof(Math.Abs), new[] { typeof(float) })),
                new CodeInstruction(OpCodes.Ldc_R4, 0.001f),
                new CodeInstruction(OpCodes.Bgt_Un_S, returnLabel),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(global::Scp914.Scp914Machine), nameof(global::Scp914.Scp914Machine.singleton))),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(global::Scp914.Scp914Machine), nameof(global::Scp914.Scp914Machine.knobState))),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Add),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingKnobSettingEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp914), nameof(Handlers.Scp914.OnChangingKnobSetting))),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(ChangingKnobSettingEventArgs), nameof(ChangingKnobSettingEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(global::Scp914.Scp914Machine), nameof(global::Scp914.Scp914Machine.singleton))),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(global::Scp914.Scp914Machine), nameof(global::Scp914.Scp914Machine.knobCooldown))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(global::Scp914.Scp914Machine), nameof(global::Scp914.Scp914Machine.curKnobCooldown))),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(ChangingKnobSettingEventArgs), nameof(ChangingKnobSettingEventArgs.KnobSetting))),
                new CodeInstruction(OpCodes.Call, PropertySetter(typeof(global::Scp914.Scp914Machine), nameof(global::Scp914.Scp914Machine.NetworkknobState))),
            });

            // Add the starting labels to the first injected instruction.
            newInstructions[index].labels.AddRange(startingLabels);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
            ListPool<Label>.Shared.Return(startingLabels);
        }
    }
}
