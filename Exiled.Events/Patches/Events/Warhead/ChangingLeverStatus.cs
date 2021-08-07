// -----------------------------------------------------------------------
// <copyright file="ChangingLeverStatus.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayerInteract.UserCode_CmdSwitchAWButton"/>.
    /// Adds the <see cref="Handlers.Warhead.ChangingLeverStatus"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.UserCode_CmdSwitchAWButton))]
    internal static class ChangingLeverStatus
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // offset variable
            const int offset = 2;

            // Find last brtrue.s
            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Brtrue_S) + offset;

            // Get the count to find the previous index
            var oldCount = newInstructions.Count;

            // Add the return label
            var returnLabel = generator.DefineLabel();
            newInstructions[index - 1].WithLabels(returnLabel);

            // var ev = new ChangingWarheadLeverStatusEventArgs(Player.Get(component), true);
            //
            // Handlers.Player.OnChangingWarheadLeverStatus(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(index, new[]
            {
                 new CodeInstruction(OpCodes.Ldloc_0),
                 new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                 new CodeInstruction(OpCodes.Ldc_I4_1),
                 new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingLeverStatusEventArgs))[0]),
                 new CodeInstruction(OpCodes.Dup),
                 new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Warhead), nameof(Handlers.Warhead.OnChangingLeverStatus))),
                 new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(ChangingLeverStatusEventArgs), nameof(ChangingLeverStatusEventArgs.IsAllowed))),
                 new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            // Restore labels on the first injected instructions
            // Calculate the difference and get the valid index - is better and easy than using a list
            newInstructions[index].MoveLabelsFrom(newInstructions[newInstructions.Count - oldCount + index]);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
