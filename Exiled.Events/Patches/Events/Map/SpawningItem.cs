// -----------------------------------------------------------------------
// <copyright file="SpawningItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Core.Generic.Pools;

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;
    using Handlers;
    using HarmonyLib;
    using MapGeneration.Distributors;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ItemDistributor.SpawnPickup" />.
    /// Adds the <see cref="Map.SpawningItem" /> event.
    /// </summary>
    [EventPatch(typeof(Map), nameof(Map.SpawningItem))]
    [HarmonyPatch(typeof(ItemDistributor), nameof(ItemDistributor.CreatePickup))]
    internal static class SpawningItem
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder initiallySpawn = generator.DeclareLocal(typeof(bool));

            Label skip = generator.DefineLabel();
            Label returnLabel = generator.DefineLabel();

            int lastIndex = newInstructions.FindLastIndex(instruction => instruction.IsLdarg(0));

            const int offset = -1;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(ItemDistributor), nameof(ItemDistributor.SpawnPickup)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // initiallySpawn = true
                    new CodeInstruction(OpCodes.Ldc_I4_1).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Stloc_S, initiallySpawn.LocalIndex),

                    // goto skip
                    new(OpCodes.Br_S, skip),

                    // initiallySpawn = false
                    new CodeInstruction(OpCodes.Ldc_I4_0).MoveLabelsFrom(newInstructions[lastIndex]),
                    new(OpCodes.Stloc_S, initiallySpawn.LocalIndex),

                    // ipb
                    new CodeInstruction(OpCodes.Ldloc_1).WithLabels(skip),

                    // initiallySpawn
                    new(OpCodes.Ldloc_S, initiallySpawn.LocalIndex),

                    // SpawningItemEventArgs ev = new(ItemPickupBase, initiallySpawn)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningItemEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Map.OnSpawningItem(ev)
                    new(OpCodes.Call, Method(typeof(Map), nameof(Map.OnSpawningItem))),

                    // if (!ev.IsAllowed)
                    //     return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningItemEventArgs), nameof(SpawningItemEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}