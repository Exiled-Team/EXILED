// -----------------------------------------------------------------------
// <copyright file="GrenadeFuseTimeFix.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Patches.Fixes
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using SEXiled.API.Features.Items;

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
                // if (!thrownProjectils is TimeGrenade timeGrenade)
                //    goto SKIP_LABEL
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Isinst, typeof(TimeGrenade)),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, timeGrenade.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse, skipLabel),

                // item = Item.Get(this);
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, Method(typeof(Item), nameof(Item.Get))),
                new CodeInstruction(OpCodes.Stloc, item.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc, item.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse, skipLabel),

                // if (item is ExplosiveGrenade explosive)
                //    goto NOT_EXPLOSIVE_LABEL
                new CodeInstruction(OpCodes.Ldloc, item.LocalIndex),
                new CodeInstruction(OpCodes.Isinst, typeof(ExplosiveGrenade)),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, explosive.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse, notExplosiveLabel),

                // timeGrenade._fuseTime = explosive.FuseTime
                new CodeInstruction(OpCodes.Ldloc, timeGrenade.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc, explosive.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.FuseTime))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(TimeGrenade), nameof(TimeGrenade._fuseTime))),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.GrenadeToItem))),
                new CodeInstruction(OpCodes.Ldloc, timeGrenade.LocalIndex),
                new CodeInstruction(OpCodes.Isinst, typeof(ExplosionGrenade)),
                new CodeInstruction(OpCodes.Ldloc, explosive.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Dictionary<ExplosionGrenade, ExplosiveGrenade>), nameof(Dictionary<ExplosiveGrenade, ExplosionGrenade>.Add))),

                // timeGrenade.ServerActivate()
                new CodeInstruction(OpCodes.Ldloc, timeGrenade.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(TimeGrenade), nameof(TimeGrenade.ServerActivate))),
                new CodeInstruction(OpCodes.Ret),

                // if (item is FlashGrenade flash)
                //    goto SKIP_LABEL
                new CodeInstruction(OpCodes.Ldloc, item.LocalIndex).WithLabels(notExplosiveLabel),
                new CodeInstruction(OpCodes.Isinst, typeof(FlashGrenade)),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, flash.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse, skipLabel),

                // timeGrenade._fuseTime = flash.FuseTime
                new CodeInstruction(OpCodes.Ldloc, timeGrenade.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc, flash.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(FlashGrenade), nameof(FlashGrenade.FuseTime))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(TimeGrenade), nameof(TimeGrenade._fuseTime))),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(FlashGrenade), nameof(FlashGrenade.GrenadeToItem))),
                new CodeInstruction(OpCodes.Ldloc, timeGrenade.LocalIndex),
                new CodeInstruction(OpCodes.Isinst, typeof(FlashbangGrenade)),
                new CodeInstruction(OpCodes.Ldloc, flash.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Dictionary<FlashbangGrenade, FlashGrenade>), nameof(Dictionary<FlashbangGrenade, FlashGrenade>.Add))),

                // timeGrenade.ServerActivate();
                new CodeInstruction(OpCodes.Ldloc, timeGrenade.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(TimeGrenade), nameof(TimeGrenade.ServerActivate))),
                new CodeInstruction(OpCodes.Ret),

                // skips all of the above code, and runs base-game serverActivate instead.
                new CodeInstruction(OpCodes.Nop).WithLabels(skipLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
