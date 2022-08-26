// -----------------------------------------------------------------------
// <copyright file="ChangingRadioPreset.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Radio.NetworkcurRangeId" />.
    ///     Adds the <see cref="Handlers.Player.ChangingRadioPreset" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ChangingRadioPreset))]
    [HarmonyPatch(typeof(Radio), nameof(Radio.NetworkcurRangeId), MethodType.Setter)]
    internal static class ChangingRadioPreset
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingRadioPresetEventArgs));

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // Player.Get(this.gameObject)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, PropertyGetter(typeof(Radio), nameof(Radio.gameObject))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),

                // this.NetworkcurRangeId
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, PropertyGetter(typeof(Radio), nameof(Radio.NetworkcurRangeId))),

                // newValue
                new(OpCodes.Ldarg_1),

                // true
                new(OpCodes.Ldc_I4_1),

                // var ev = ChangingRadioPresetEventArgs(...)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingRadioPresetEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Player.OnChangingRadioPreset(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangingRadioPreset))),

                // if (!ev.IsAllowed)
                //     return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRadioPresetEventArgs), nameof(ChangingRadioPresetEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),

                // newValue = ev.NewValue
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRadioPresetEventArgs), nameof(ChangingRadioPresetEventArgs.NewValue))),
                new(OpCodes.Stloc_0),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
