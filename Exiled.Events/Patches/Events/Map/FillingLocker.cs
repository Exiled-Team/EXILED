// -----------------------------------------------------------------------
// <copyright file="FillingLocker.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;

    using HarmonyLib;

    using InventorySystem.Items.Pickups;

    using MapGeneration.Distributors;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="LockerChamber.SpawnItem" />.
    /// Adds the <see cref="Handlers.Map.FillingLocker" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.FillingLocker))]
    [HarmonyPatch(typeof(LockerChamber), nameof(LockerChamber.SpawnItem))]
    internal static class FillingLocker
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            int offset = -3;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(HashSet<ItemPickupBase>), nameof(HashSet<ItemPickupBase>.Add), new[] { typeof(ItemPickupBase) }))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // FillingLockerEventArgs ev = new(ItemPickupBase, loockerChamber)
                    new(OpCodes.Ldloc_S, 4),
                    new(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(FillingLockerEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Map.OnFillingLocker(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnFillingLocker))),

                    // if (!ev.IsAllowed)
                    //     return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(FillingLockerEventArgs), nameof(FillingLockerEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
