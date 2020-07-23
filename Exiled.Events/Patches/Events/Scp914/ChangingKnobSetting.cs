// -----------------------------------------------------------------------
// <copyright file="ChangingKnobSetting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp914
{
    using System;
#pragma warning disable SA1118
#pragma warning disable SA1313
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayerInteract.CallCmdChange914Knob"/>.
    /// Adds the <see cref="Scp914.ChangingKnobSetting"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdChange914Knob))]
    internal static class ChangingKnobSetting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = new List<CodeInstruction>(instructions);

            // Search for the last "ldsfld".
            var index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldsfld);

            // Get the start label [Label4] and remove it.
            var startLabel = newInstructions[index].labels[0];
            newInstructions[index].labels.Clear();

            // Remove "Scp914.Scp914Machine.singleton.ChangeKnobStatus()".
            newInstructions.RemoveRange(index, 2);

            // Declare ChangingKnobSettingEventArgs, to be able to store its instance with "stloc.0".
            generator.DeclareLocal(typeof(ChangingKnobSettingEventArgs));

            // if (Math.Abs(Scp914.Scp914Machine.singleton.curKnobCooldown) > 0.001f)
            //   return;
            //
            //  var ev = new ChangingKnobSettingEventArgs(API.Features.Player.Get(this.gameObject), Scp914Machine.singleton.knobState + 1);
            //
            // Scp914.OnChangingKnobSetting(ev);
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
                new CodeInstruction(OpCodes.Bgt_Un_S, newInstructions[index - 1].labels[0]),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(GameObject) })),
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(global::Scp914.Scp914Machine), nameof(global::Scp914.Scp914Machine.singleton))),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(global::Scp914.Scp914Machine), nameof(global::Scp914.Scp914Machine.knobState))),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Add),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingKnobSettingEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_0),
                new CodeInstruction(OpCodes.Call, Method(typeof(Scp914), nameof(Scp914.OnChangingKnobSetting))),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(ChangingKnobSettingEventArgs), nameof(ChangingKnobSettingEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, newInstructions[index - 1].labels[0]),
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(global::Scp914.Scp914Machine), nameof(global::Scp914.Scp914Machine.singleton))),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(global::Scp914.Scp914Machine), nameof(global::Scp914.Scp914Machine.knobCooldown))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(global::Scp914.Scp914Machine), nameof(global::Scp914.Scp914Machine.curKnobCooldown))),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(ChangingKnobSettingEventArgs), nameof(ChangingKnobSettingEventArgs.KnobSetting))),
                new CodeInstruction(OpCodes.Call, PropertySetter(typeof(global::Scp914.Scp914Machine), nameof(global::Scp914.Scp914Machine.NetworkknobState))),
            });

            // Add the start label [Label4].
            newInstructions[index].labels.Add(startLabel);

            return newInstructions;
        }
    }
}
