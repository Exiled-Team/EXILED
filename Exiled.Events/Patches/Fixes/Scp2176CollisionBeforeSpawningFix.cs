// -----------------------------------------------------------------------
// <copyright file="Scp2176CollisionBeforeSpawningFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Pickups;

    using HarmonyLib;

    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.ThrowableProjectiles;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="Scp2176Projectile.ProcessCollision(Collision)"/>.
    /// Fix processing collision before spawning.
    /// </summary>
    [HarmonyPatch(typeof(Scp2176Projectile), nameof(Scp2176Projectile.ProcessCollision))]
    internal static class Scp2176CollisionBeforeSpawningFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder pickup = generator.DeclareLocal(typeof(Pickup));

            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(Pickup), nameof(Pickup.Get), new[] { typeof(ItemPickupBase) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, pickup.LocalIndex),
                new(OpCodes.Brfalse_S, ret),
                new(OpCodes.Ldloc_S, pickup.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Pickup), nameof(Pickup.Spawned))),
                new(OpCodes.Brfalse_S, ret),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
