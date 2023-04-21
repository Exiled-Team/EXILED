// -----------------------------------------------------------------------
// <copyright file="ShowHitIndicator.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using InventorySystem.Items.Firearms.Modules;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="StandardHitregBase.ShowHitIndicator"/> to prevent sending Hit Indicators to NPCs (Null Connections).
    /// </summary>
    [HarmonyPatch(typeof(StandardHitregBase), nameof(StandardHitregBase.ShowHitIndicator))]
    internal static class ShowHitIndicator
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> OnShowHitIndicator(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();
            var index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldloc_0);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.IsNpc))),
                new(OpCodes.Brtrue_S, retLabel),
            });
            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            foreach (var instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}