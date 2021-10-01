// -----------------------------------------------------------------------
// <copyright file="DummyShowHitIndicatorFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using HarmonyLib;

    using InventorySystem.Items.Firearms.Modules;

    using Mirror;

    using NorthwoodLib.Pools;

    /// <summary>
    /// Patches <see cref="StandardHitregBase.ShowHitIndicator"/> to fix crash when shooting near dummy.
    /// </summary>
    [HarmonyPatch(typeof(StandardHitregBase), nameof(StandardHitregBase.ShowHitIndicator))]
    internal static class DummyShowHitIndicatorFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 1;

            int index = newInstructions.FindIndex(inst => inst.opcode == OpCodes.Ldloc_0) + offset;

            Label okLabel = generator.DefineLabel();

            // if(referenceHub.GetComponent<NetworkConnection>() == null)
            // {
            //   return;
            // }
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(ReferenceHub), nameof(ReferenceHub.GetComponent), null, new System.Type[] { typeof(NetworkConnection) })),
                new CodeInstruction(OpCodes.Brtrue_S, okLabel),
                new CodeInstruction(OpCodes.Ret),
                new CodeInstruction(OpCodes.Ldloc_0).WithLabels(okLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
