// -----------------------------------------------------------------------
// <copyright file="LockerFixes.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Core.Generic.Pools;

    using HarmonyLib;

    using InventorySystem.Items.Pickups;

    using MapGeneration.Distributors;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Fix for chamber lists weren't cleared.
    /// </summary>
    [HarmonyPatch(typeof(LockerChamber), nameof(LockerChamber.SetDoor))]
    internal class LockerFixes
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = -1;
            int index = newInstructions.FindIndex(i => i.LoadsField(Field(typeof(LockerChamber), nameof(LockerChamber._spawnOnFirstChamberOpening)))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Call, Method(typeof(LockerFixes), nameof(Helper))),
                new(OpCodes.Ret),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void Helper(LockerChamber chamber)
        {
            chamber._content.Clear();

            if (!chamber._spawnOnFirstChamberOpening)
                return;

            foreach (ItemPickupBase ipb in chamber._toBeSpawned)
            {
                if (ipb != null)
                    ItemDistributor.SpawnPickup(ipb);
            }

            chamber._toBeSpawned.Clear();
        }
    }
}