// -----------------------------------------------------------------------
// <copyright file="PickupListAdd.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Items;

    using HarmonyLib;

    using InventorySystem.Items.Pickups;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ItemPickupBase.Awake"/>.
    /// </summary>
    [HarmonyPatch(typeof(ItemPickupBase), nameof(ItemPickupBase.Awake))]
    public static class PickupListAdd
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(codeInstructions);

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Ldsfld, Field(typeof(Pickup), nameof(Pickup.BaseToItem))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(Pickup))[0]),
                new(OpCodes.Callvirt, Method(typeof(List<Pickup>), nameof(List<Pickup>.Add))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
