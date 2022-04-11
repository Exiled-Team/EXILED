// -----------------------------------------------------------------------
// <copyright file="ExplosiveGrenadeFieldsFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Items;

    using HarmonyLib;

    using InventorySystem.Items.ThrowableProjectiles;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ExplosionGrenade.PlayExplosionEffects"/> to sync <see cref="ExplosiveGrenade"/> property values.
    /// </summary>
    [HarmonyPatch(typeof(ExplosionGrenade), nameof(ExplosionGrenade.PlayExplosionEffects))]
    internal static class ExplosiveGrenadeFieldsFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            const int index = 0;
            Label skipLabel = generator.DefineLabel();
            LocalBuilder explosive = generator.DeclareLocal(typeof(ExplosiveGrenade));

            newInstructions.InsertRange(index, new[]
            {
                // if (!ExplosiveGrenade.GrenadeToItem.TryGetValue(this, out ExplosiveGrenade explosive)
                //     goto SKIP_LABEL
                new(OpCodes.Call, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.GrenadeToItem))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloca_S, explosive.LocalIndex),
                new(OpCodes.Callvirt, Method(typeof(Dictionary<ExplosionGrenade, ExplosiveGrenade>), nameof(Dictionary<ExplosionGrenade, ExplosiveGrenade>.TryGetValue))),
                new(OpCodes.Brfalse, skipLabel),

                // this._burnedDuration = explosive.BurnDuration;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc, explosive.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.BurnDuration))),
                new(OpCodes.Stfld, Field(typeof(ExplosionGrenade), nameof(ExplosionGrenade._burnedDuration))),

                // this._deafenedDuration = explosive.DeafenDuration
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc, explosive.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.DeafenDuration))),
                new(OpCodes.Stfld, Field(typeof(ExplosionGrenade), nameof(ExplosionGrenade._deafenedDuration))),

                // this._concussedDuration = explosive.ConcussDuration
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc, explosive.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.ConcussDuration))),
                new(OpCodes.Stfld, Field(typeof(ExplosionGrenade), nameof(ExplosionGrenade._concussedDuration))),

                // this._scpDamageMultiplier = explosive.ScpMultiplier
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc, explosive.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.ScpMultiplier))),
                new(OpCodes.Stfld, Field(typeof(ExplosionGrenade), nameof(ExplosionGrenade._scpDamageMultiplier))),

                // this._maxRadius = explosive.MaxRadius
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc, explosive.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.MaxRadius))),
                new(OpCodes.Stfld, Field(typeof(ExplosionGrenade), nameof(ExplosionGrenade._maxRadius))),

                // SKIP_LABEL
                new CodeInstruction(OpCodes.Nop).WithLabels(skipLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
