// -----------------------------------------------------------------------
// <copyright file="ChangingRadioPreset.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player {
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Radio.NetworkcurRangeId"/>.
    /// Adds the <see cref="Handlers.Player.ChangingRadioPreset"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Radio), nameof(Radio.NetworkcurRangeId), MethodType.Setter)]
    internal static class ChangingRadioPreset {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingRadioPresetEventArgs));

            newInstructions.InsertRange(0, new[]
            {
                // Player.Get(this.gameObject)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Radio), nameof(Radio.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(GameObject) })),

                // this.NetworkcurRangeId
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Radio), nameof(Radio.NetworkcurRangeId))),

                // newValue
                new CodeInstruction(OpCodes.Ldarg_1),

                // true
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // var ev = ChangingRadioPresetEventArgs(...)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingRadioPresetEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Player.OnChangingRadioPreset(ev)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangingRadioPreset))),

                // if (!ev.IsAllowed)
                //     return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRadioPresetEventArgs), nameof(ChangingRadioPresetEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),

                // newValue = ev.NewValue
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRadioPresetEventArgs), nameof(ChangingRadioPresetEventArgs.NewValue))),
                new CodeInstruction(OpCodes.Stloc_0),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
