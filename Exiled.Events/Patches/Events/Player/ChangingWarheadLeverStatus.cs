// -----------------------------------------------------------------------
// <copyright file="ChangingWarheadLeverStatus.cs" company="Exiled Team">
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
    /// Patches <see cref="PlayerInteract.CallCmdUsePanel"/>.
    /// Adds the <see cref="Handlers.Player.ChangingWarheadLeverStatus"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdUsePanel))]
    internal class ChangingWarheadLeverStatus
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 2; // offset variable

            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Brtrue_S) + offset; // Find last brtrue.s

            var startLabels = ListPool<Label>.Shared.Rent(newInstructions[index].labels);
            newInstructions[index].labels.Clear(); // Remove old labels

            var returnLabel = generator.DefineLabel();
            newInstructions[index - 1].labels.Add(returnLabel);

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
                 new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingWarheadLeverStatusEventArgs))[0]),
                 new CodeInstruction(OpCodes.Dup),
                 new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangingWarheadLeverStatus))),
                 new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(ChangingWarheadLeverStatusEventArgs), nameof(ChangingWarheadLeverStatusEventArgs.IsAllowed))),
                 new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            newInstructions[index].labels.AddRange(startLabels); // Restore labels on the first injected instructions

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
            ListPool<Label>.Shared.Return(startLabels);
        }
    }
}
