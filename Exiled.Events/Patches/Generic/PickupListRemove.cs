// -----------------------------------------------------------------------
// <copyright file="PickupListRemove.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1402
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pickups;

    using HarmonyLib;

    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.ThrowableProjectiles;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ItemPickupBase.OnDestroy"/>.
    /// </summary>
    [HarmonyPatch(typeof(ItemPickupBase), nameof(ItemPickupBase.OnDestroy))]
    internal static class PickupListRemove
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            newInstructions.InsertRange(0, new[]
            {
                // _ = Pickup.BaseToPickup.Remove(this);
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(Pickup), nameof(Pickup.BaseToPickup))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, Method(typeof(Dictionary<ItemPickupBase, Pickup>), nameof(Dictionary<ItemPickupBase, Pickup>.Remove), new[] { typeof(ItemPickupBase) })),
                new(OpCodes.Pop),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="EffectGrenade.OnDestroy"/> for fixing cringe NW code :).
    /// </summary>
    [HarmonyPatch(typeof(EffectGrenade), nameof(EffectGrenade.OnDestroy))]
    internal static class EffectGrenadesListRemove
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            // remove all code and save only base.OnDestroy()
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new(OpCodes.Call, Method(typeof(ItemPickupBase), nameof(ItemPickupBase.OnDestroy)));
            yield return new(OpCodes.Ret);
        }
    }

    /// <summary>
    /// Patches <see cref="EffectGrenade.ServerFuseEnd"/> for fixing cringe NW code :).
    /// </summary>
    [HarmonyPatch(typeof(EffectGrenade), nameof(EffectGrenade.ServerFuseEnd))]
    internal static class ServerFuseEndListRemove
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label skip = generator.DefineLabel();

            // PlayExplosionEffects()
            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, Method(typeof(EffectGrenade), nameof(EffectGrenade.PlayExplosionEffects))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];
        }
    }

    /// <summary>
    /// Patches <see cref="Scp2176Projectile.ServerFuseEnd"/> for fixing cringe NW code :).
    /// </summary>
    [HarmonyPatch(typeof(Scp2176Projectile), nameof(Scp2176Projectile.ServerFuseEnd))]
    internal static class Scp2176ListRemove
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = -1;
            int index = newInstructions.FindIndex(i => i.Calls(Method(typeof(Scp2176Projectile), nameof(Scp2176Projectile.ServerShatter)))) + offset;

            offset = -1;
            int index2 = newInstructions.FindIndex(i => i.Calls(Method(typeof(EffectGrenade), nameof(EffectGrenade.ServerFuseEnd)))) + offset;

            // fix labels
            newInstructions[index].MoveLabelsFrom(newInstructions[index2]);

            // remove EffectGrenade::ServerFuseEnd at start
            newInstructions.RemoveRange(index2, 2);

            // insert EffectGrenade::ServerFuseEnd at the ends
            newInstructions.InsertRange(newInstructions.Count - 1, new[]
            {
                new(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, Method(typeof(EffectGrenade), nameof(EffectGrenade.ServerFuseEnd))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="CoalProjectile.FixedUpdate"/> for fixing cringe NW code :).
    /// </summary>
    [HarmonyPatch(typeof(CoalProjectile), nameof(CoalProjectile.FixedUpdate))]
    internal static class CoalProjectileListRemove
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            // remove all code and save only base.FixedUpdate()
            yield return new(OpCodes.Ldarg_0);
            yield return new(OpCodes.Call, Method(typeof(ItemPickupBase), nameof(ItemPickupBase.FixedUpdate)));
            yield return new(OpCodes.Ret);
        }
    }
}
