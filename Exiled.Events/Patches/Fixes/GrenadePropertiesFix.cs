// -----------------------------------------------------------------------
// <copyright file="GrenadePropertiesFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Pickups.Projectiles;

    using HarmonyLib;

    using InventorySystem.Items;
    using InventorySystem.Items.ThrowableProjectiles;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ThrowableItem.ServerThrow(float, float, Vector3, Vector3)"/> to fix all grenade properties.
    /// </summary>
    [HarmonyPatch(typeof(ThrowableItem), nameof(ThrowableItem.ServerThrow), typeof(float), typeof(float), typeof(Vector3), typeof(Vector3))]
    internal static class GrenadePropertiesFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder throwable = generator.DeclareLocal(typeof(ThrowableItem));
            LocalBuilder projectile = generator.DeclareLocal(typeof(Projectile));
            LocalBuilder playerCamera = generator.DeclareLocal(typeof(Transform));

            Label cnt = generator.DefineLabel();

            const int offset = -11;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Stloc_0) + offset;

            newInstructions.RemoveRange(index, 11);

            // if (Item.Get(this) is not Throwable throwable)
            // {
            //     Log.Error("Item is not Throwable, should never happen");
            //     return;
            // }
            // Projectile projectile = throwable.Projectile;
            // ThrownProjectile baseProjectile = projectile.Base;
            // baseProjectile.transform.position = this.Owner.PlayerCameraReference.position;
            // baseProjectile.transform.rotation = this.Owner.PlayerCameraReference.rotation;
            // baseProjectile.gameObject.SetActive(true);
            // projectile.Spawned = true;
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(Item), nameof(Item.Get), new[] { typeof(ItemBase) })),
                new(OpCodes.Isinst, typeof(Throwable)),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, throwable.LocalIndex),
                new(OpCodes.Brtrue_S, cnt),

                new(OpCodes.Ldstr, "Item is not Throwable, should never happen"),
                new(OpCodes.Call, Method(typeof(API.Features.Log), nameof(API.Features.Log.Error), new[] { typeof(string) })),
                new(OpCodes.Ret),

                new CodeInstruction(OpCodes.Ldloc_S, throwable.LocalIndex).WithLabels(cnt),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Throwable), nameof(Throwable.Projectile))),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, projectile.LocalIndex),
                new(OpCodes.Callvirt, FirstProperty(typeof(Projectile), prop => prop.Name == nameof(Projectile.Base) && prop.PropertyType == typeof(ThrownProjectile)).GetMethod),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Dup),

                new(OpCodes.Callvirt, PropertyGetter(typeof(ThrownProjectile), nameof(ThrownProjectile.transform))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ThrowableItem), nameof(ThrowableItem.Owner))),
                new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.PlayerCameraReference))),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, playerCamera.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Transform), nameof(Transform.position))),
                new(OpCodes.Callvirt, PropertySetter(typeof(Transform), nameof(Transform.position))),

                new(OpCodes.Callvirt, PropertyGetter(typeof(ThrownProjectile), nameof(ThrownProjectile.transform))),
                new(OpCodes.Ldloc_S, playerCamera.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Transform), nameof(Transform.rotation))),
                new(OpCodes.Callvirt, PropertySetter(typeof(Transform), nameof(Transform.rotation))),

                new(OpCodes.Callvirt, PropertyGetter(typeof(ThrownProjectile), nameof(ThrownProjectile.gameObject))),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Callvirt, Method(typeof(GameObject), nameof(GameObject.SetActive))),

                new(OpCodes.Ldloc_S, projectile.LocalIndex),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Callvirt, PropertySetter(typeof(Projectile), nameof(Projectile.Spawned))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
