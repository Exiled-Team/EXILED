// -----------------------------------------------------------------------
// <copyright file="TryingNotToCry.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp096 {
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches the <see cref="PlayableScps.Scp096.TryNotToCry"/> method.
    /// Adds the <see cref="Handlers.Scp096.TryingNotToCry"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayableScps.Scp096), nameof(PlayableScps.Scp096.TryNotToCry))]
    internal static class TryingNotToCry {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            const int offset = 0;

            // Search for last "lfsfld".
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldsfld) + offset;

            // Get the return label from the instruction.
            Label returnLabel = newInstructions[index - 1].labels[0];

            // var ev = new TryingNotToCryEventArgs(Scp096 scp096, Player player, door, true);
            //
            // Handlers.Scp096.OnTryingNotToCry(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PlayableScps.PlayableScp), nameof(PlayableScps.PlayableScp.Hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(TryingNotToCryEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp096), nameof(Handlers.Scp096.OnTryingNotToCry))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(TryingNotToCryEventArgs), nameof(TryingNotToCryEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
