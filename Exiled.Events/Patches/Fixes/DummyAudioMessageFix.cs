// -----------------------------------------------------------------------
// <copyright file="DummyAudioMessageFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using HarmonyLib;

    using InventorySystem.Items;
    using InventorySystem.Items.Firearms;

    using Mirror;

    using NorthwoodLib.Pools;

    /// <summary>
    /// Patches <see cref="FirearmExtensions.ServerSendAudioMessage"/> to fix crash when shooting near dummy.
    /// </summary>
    [HarmonyPatch(typeof(FirearmExtensions), nameof(FirearmExtensions.ServerSendAudioMessage))]
    internal static class DummyAudioMessageFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 3;
            const int continueOffset = 2;

            int baseIndex = newInstructions.FindLastIndex(inst => inst.opcode == OpCodes.Callvirt && ((MethodInfo)inst.operand) == AccessTools.PropertyGetter(typeof(ItemBase), nameof(ItemBase.Owner)));

            Label continueLabel = (Label)newInstructions[baseIndex + continueOffset].operand;

            // if(referenceHub.GetComponent<NetworkConnection>() == null)
            // {
            //   continue;
            // }
            newInstructions.InsertRange(baseIndex + offset, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, 5),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(ReferenceHub), nameof(ReferenceHub.GetComponent), null, new System.Type[] { typeof(NetworkConnection) })),
                new CodeInstruction(OpCodes.Brfalse_S, continueLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
