// -----------------------------------------------------------------------
// <copyright file="FixPickupPreviousOwner.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
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
    /// Patches <see cref="InventoryExtensions.ServerDropAmmo(Inventory, ItemType, ushort, bool)"/> delegate.
    /// Fix than NW don't set the field <see cref="ItemPickupBase.PreviousOwner"/>.
    /// </summary>
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerDropAmmo))]
    internal class FixPickupPreviousOwner
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = 1;
            int index = newInstructions.FindLastIndex(instruction => instruction.Calls(Method(typeof(List<AmmoPickup>), nameof(List<AmmoPickup>.Add)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldloc_S, 8),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(Footprint))[0]),
                    new(OpCodes.Stfld, Field(typeof(ItemPickupBase), nameof(ItemPickupBase.PreviousOwner))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
