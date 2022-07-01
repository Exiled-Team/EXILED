// -----------------------------------------------------------------------
// <copyright file="GrenadeFuseTimeFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Items;

    using HarmonyLib;

    using InventorySystem.Items.ThrowableProjectiles;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ThrowableItem"/> to fix fuse times being unchangeable.
    /// </summary>
    [HarmonyPatch(typeof(ThrowableItem), nameof(ThrowableItem.ServerThrow), typeof(float), typeof(float), typeof(Vector3), typeof(Vector3))]
    internal static class GrenadeFuseTimeFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            const int offset = -1;
            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Callvirt) + offset;
            LocalBuilder timeGrenade = generator.DeclareLocal(typeof(TimeGrenade));
            LocalBuilder explosive = generator.DeclareLocal(typeof(ExplosiveGrenade));
            LocalBuilder flash = generator.DeclareLocal(typeof(FlashGrenade));
            LocalBuilder item = generator.DeclareLocal(typeof(Item));
            Label notExplosiveLabel = generator.DefineLabel();
            Label skipLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                // if (thrownProjectils is not TimeGrenade timeGrenade)
                //    goto SKIP_LABEL
                new(OpCodes.Ldloc_0),
                new(OpCodes.Isinst, typeof(TimeGrenade)),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, timeGrenade.LocalIndex),
                new(OpCodes.Brfalse, skipLabel),

                // item = Item.Get(this);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(Item), nameof(Item.Get))),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, item.LocalIndex),
                new(OpCodes.Brfalse, skipLabel),

                // if (item is not ExplosiveGrenade explosive)
                //    goto NOT_EXPLOSIVE_LABEL
                new(OpCodes.Ldloc, item.LocalIndex),
                new(OpCodes.Isinst, typeof(ExplosiveGrenade)),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, explosive.LocalIndex),
                new(OpCodes.Brfalse, notExplosiveLabel),

                // timeGrenade._fuseTime = explosive.FuseTime
                new(OpCodes.Ldloc, timeGrenade.LocalIndex),
                new(OpCodes.Ldloc, explosive.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.FuseTime))),
                new(OpCodes.Stfld, Field(typeof(TimeGrenade), nameof(TimeGrenade._fuseTime))),
                new(OpCodes.Call, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.GrenadeToItem))),
                new(OpCodes.Ldloc, timeGrenade.LocalIndex),
                new(OpCodes.Isinst, typeof(ExplosionGrenade)),
                new(OpCodes.Ldloc, explosive.LocalIndex),
                new(OpCodes.Callvirt, Method(typeof(Dictionary<ExplosionGrenade, ExplosiveGrenade>), nameof(Dictionary<ExplosiveGrenade, ExplosionGrenade>.Add))),

                // timeGrenade.ServerActivate()
                // return;
                new(OpCodes.Ldloc, timeGrenade.LocalIndex),
                new(OpCodes.Callvirt, Method(typeof(TimeGrenade), nameof(TimeGrenade.ServerActivate))),
                new(OpCodes.Ret),

                // if (item is FlashGrenade flash)
                //    goto SKIP_LABEL
                new CodeInstruction(OpCodes.Ldloc, item.LocalIndex).WithLabels(notExplosiveLabel),
                new(OpCodes.Isinst, typeof(FlashGrenade)),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, flash.LocalIndex),
                new(OpCodes.Brfalse, skipLabel),

                // timeGrenade._fuseTime = flash.FuseTime
                new(OpCodes.Ldloc, timeGrenade.LocalIndex),
                new(OpCodes.Ldloc, flash.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(FlashGrenade), nameof(FlashGrenade.FuseTime))),
                new(OpCodes.Stfld, Field(typeof(TimeGrenade), nameof(TimeGrenade._fuseTime))),
                new(OpCodes.Call, PropertyGetter(typeof(FlashGrenade), nameof(FlashGrenade.GrenadeToItem))),
                new(OpCodes.Ldloc, timeGrenade.LocalIndex),
                new(OpCodes.Isinst, typeof(FlashbangGrenade)),
                new(OpCodes.Ldloc, flash.LocalIndex),
                new(OpCodes.Callvirt, Method(typeof(Dictionary<FlashbangGrenade, FlashGrenade>), nameof(Dictionary<FlashbangGrenade, FlashGrenade>.Add))),

                // timeGrenade.ServerActivate();
                // return
                new(OpCodes.Ldloc, timeGrenade.LocalIndex),
                new(OpCodes.Callvirt, Method(typeof(TimeGrenade), nameof(TimeGrenade.ServerActivate))),
                new(OpCodes.Ret),

                // skips all of the above code, and runs base-game serverActivate instead.
                new CodeInstruction(OpCodes.Nop).WithLabels(skipLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
