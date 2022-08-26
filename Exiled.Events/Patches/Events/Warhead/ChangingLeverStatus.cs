// -----------------------------------------------------------------------
// <copyright file="ChangingLeverStatus.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Warhead
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Warhead;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Warhead = Exiled.Events.Handlers.Warhead;

    /// <summary>
    ///     Patches <see cref="PlayerInteract.UserCode_CmdUsePanel" />.
    ///     Adds the <see cref="Handlers.Warhead.ChangingLeverStatus" /> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.UserCode_CmdUsePanel))]
    internal static class ChangingLeverStatus
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            int offset = 2;

            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Brtrue_S) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Call, PropertyGetter(typeof(AlphaWarheadNukesitePanel), nameof(AlphaWarheadNukesitePanel.Networkenabled))),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingLeverStatusEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Call, Method(typeof(Warhead), nameof(Warhead.OnChangingLeverStatus))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingLeverStatusEventArgs), nameof(ChangingLeverStatusEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            offset = 10;

            index += offset;

            int moveIndex = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Brtrue_S) + 2;

            newInstructions[index].MoveLabelsTo(newInstructions[moveIndex]);

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}