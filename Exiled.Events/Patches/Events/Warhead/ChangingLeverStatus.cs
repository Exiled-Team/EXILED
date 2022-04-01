// -----------------------------------------------------------------------
// <copyright file="ChangingLeverStatus.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Warhead {
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayerInteract.UserCode_CmdUsePanel"/>.
    /// Adds the <see cref="Handlers.Warhead.ChangingLeverStatus"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.UserCode_CmdUsePanel))]
    internal static class ChangingLeverStatus {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            int offset = 2;

            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Brtrue_S) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(AlphaWarheadNukesitePanel), nameof(AlphaWarheadNukesitePanel.Networkenabled))),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingLeverStatusEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Warhead), nameof(Handlers.Warhead.OnChangingLeverStatus))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingLeverStatusEventArgs), nameof(ChangingLeverStatusEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            offset = 10;

            index += offset;

            int moveIndex = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Brtrue_S) + 2;

            newInstructions[index].MoveLabelsTo(newInstructions[moveIndex]);

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
