// -----------------------------------------------------------------------
// <copyright file="DroppingCandy.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Items.Candies.Patches
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using InventorySystem.Items;
    using InventorySystem.Items.Usables.Scp330;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp330NetworkHandler.ServerSelectMessageReceived"/> to add custom candies tracking-removing logic.
    /// </summary>
    [HarmonyPatch(typeof(Scp330NetworkHandler), nameof(Scp330NetworkHandler.ServerSelectMessageReceived))]
    internal class DroppingCandy
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldloca_S);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // DropCandy(Item, SelectScp330Message);
                    new CodeInstruction(OpCodes.Ldloc_1).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Item), nameof(Item.Get), new[] { typeof(ItemBase) })),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(DroppingCandy), nameof(DropCandy))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];
        }

        private static void DropCandy(Item item, SelectScp330Message msg)
        {
            if (!item.Is(out Scp330 _) || !CustomItem.TryGet(item, out CustomItem customItem)
                                            || customItem.BehaviourComponent != typeof(CandyBehaviour) || !item.TryGetComponent(out CandyBehaviour candyBehaviour))
                return;

            candyBehaviour.TrackedCandies.Remove(msg.CandyID);
        }
    }
}