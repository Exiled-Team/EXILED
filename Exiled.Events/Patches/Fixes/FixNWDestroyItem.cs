// -----------------------------------------------------------------------
// <copyright file="FixNWDestroyItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Footprinting;
    using HarmonyLib;
    using InventorySystem;
    using InventorySystem.Items.Firearms.Ammo;
    using InventorySystem.Items.Pickups;
    using VoiceChat;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Inventory.DestroyItemInstance(ushort, ItemPickupBase, out InventorySystem.Items.ItemBase)"/> delegate.
    /// Fix than NW give a NRE because of GameObject being null.
    /// </summary>
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.DestroyItemInstance))]
    internal class FixNWDestroyItem
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            const int offset = -4;
            int index = newInstructions.FindLastIndex(instruction => instruction.Calls(Method(typeof(UnityEngine.Object), nameof(UnityEngine.Object.Destroy), new System.Type[] { typeof(UnityEngine.Object), }))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // if (pickup == null) return false;
                    new CodeInstruction(OpCodes.Ldarg_3).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Ldind_Ref),
                    new(OpCodes.Ldnull),
                    new(OpCodes.Call, Method(typeof(UnityEngine.Object), "op_Equality", new System.Type[] { typeof(UnityEngine.Object), typeof(UnityEngine.Object), })),
                    new(OpCodes.Brtrue_S, retLabel),
                });

            newInstructions[newInstructions.FindIndex(x => x.opcode == OpCodes.Ret) - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
