// -----------------------------------------------------------------------
// <copyright file="ChangingIntoGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.ThrowableProjectiles;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="TimedGrenadePickup.Update"/>.
    /// Adds the <see cref="Map.ChangingIntoGrenade"/> event.
    /// </summary>
    [HarmonyPatch(typeof(TimedGrenadePickup), nameof(TimedGrenadePickup.Update))]
    internal static class ChangingIntoGrenade
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset
            int offset = 1;

            // Find the last return false call.
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ret) + offset;

            // Extract the existing label we will be removing.
            Label enterLabel = newInstructions[index].labels[0];

            // Generate a return label.
            Label returnLabel = generator.DefineLabel();

            // Generate a label for when we cannot set the fuse time.
            Label skipFuse = generator.DefineLabel();

            Label dontResetLabel = generator.DefineLabel();

            // Declare ChangingIntoGrenadeEventArgs, to be able to store it's instance with "stloc.s".
            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingIntoGrenadeEventArgs));

            // Declare TimeGrenade, so we are able to set it's fusetime.
            LocalBuilder timeGrenade = generator.DeclareLocal(typeof(TimeGrenade));

            // Declare ThrownProjectile because the base method doesn't use it as a local.
            LocalBuilder thrownProjectile = generator.DeclareLocal(typeof(ThrownProjectile));

            // Remove the existing instructions that get the itemBase to spawn, we will be doing this ourselves.
            int instructionsToRemove = 14;

            newInstructions.RemoveRange(index, instructionsToRemove);

            // Setup EventArgs, call event, check ev.IsAllowed and implement ev.Type changing
            newInstructions.InsertRange(index, new[]
            {
                // itemPickupBase
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(enterLabel),

                // var ev = new ChangingIntoGrenadeEventArgs(ItemPickupBase);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingIntoGrenadeEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Map.OnChangingIntoGrenade(ev);
                new(OpCodes.Call, Method(typeof(Map), nameof(Map.OnChangingIntoGrenade))),

                // if (!ev.IsAllowed)
                // {
                //     this._replaceNextFrame = false;
                //     return;
                // }
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingIntoGrenadeEventArgs), nameof(ChangingIntoGrenadeEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, dontResetLabel),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Stfld, Field(typeof(TimedGrenadePickup), nameof(TimedGrenadePickup._replaceNextFrame))),
                new(OpCodes.Ret),

                // if (!InventoryItemLoader.AvailableItems.TryGetValue(ev.Type, out itemBase) || !(itemBase is ThrowableItem throwableItem))
                //    return;
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(InventoryItemLoader), nameof(InventoryItemLoader.AvailableItems))).WithLabels(dontResetLabel),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingIntoGrenadeEventArgs), nameof(ChangingIntoGrenadeEventArgs.Type))),
                new(OpCodes.Ldloca_S, 0),
                new(OpCodes.Callvirt, Method(typeof(Dictionary<ItemType, ItemBase>), nameof(Dictionary<ItemType, ItemBase>.TryGetValue))),
                new(OpCodes.Brfalse_S, returnLabel),
                new(OpCodes.Ldloc_0),
                new(OpCodes.Isinst, typeof(ThrowableItem)),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_1),
                new(OpCodes.Brfalse_S, returnLabel),
            });

            offset = 4;
            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldloc_1) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // if (thrownProjectile is TimeGrenade timeGrenade)
                //    timeGrenade._fuseTime = ev.FuseTime;
                new(OpCodes.Stloc_S, thrownProjectile.LocalIndex),
                new(OpCodes.Ldloc_S, thrownProjectile.LocalIndex),
                new(OpCodes.Isinst, typeof(TimeGrenade)),
                new(OpCodes.Stloc_S, timeGrenade.LocalIndex),
                new(OpCodes.Brfalse_S, skipFuse),

                new(OpCodes.Ldloc_S, timeGrenade.LocalIndex),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingIntoGrenadeEventArgs), nameof(ChangingIntoGrenadeEventArgs.FuseTime))),
                new(OpCodes.Stfld, Field(typeof(TimeGrenade), nameof(TimeGrenade._fuseTime))),
                new CodeInstruction(OpCodes.Nop).WithLabels(skipFuse),
                new(OpCodes.Ldloc_S, thrownProjectile.LocalIndex),
                new(OpCodes.Dup),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
