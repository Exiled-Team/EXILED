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

    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using MapGeneration.Distributors;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="MapGeneration.Distributors.ItemDistributor.SpawnPickup" />.
    ///     Adds the <see cref="Handlers.Map.SpawningItem" /> and <see cref="Handlers.Map.SpawningItem" /> events.
    /// </summary>
    [HarmonyPatch(typeof(ItemDistributor), nameof(ItemDistributor.SpawnPickup))]
    internal static class SpawningItem
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();

            int offset = 1;

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            // var ev = new SpawningItemEventArgs(ipb, true);
            //
            // if (!ev.IsAllowed)
            //     return;
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningItemEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Map), nameof(Map.OnSpawningItem))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningItemEventArgs), nameof(SpawningItemEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
