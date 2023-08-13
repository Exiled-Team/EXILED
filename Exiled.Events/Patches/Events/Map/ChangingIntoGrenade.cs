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

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;

    using Handlers;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.ThrowableProjectiles;

    using Mirror;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="TimedGrenadePickup.Update" />.
    ///     Adds the <see cref="Map.ChangingIntoGrenade" /> and <see cref="Map.ChangedIntoGrenade" /> events.
    /// </summary>
    [EventPatch(typeof(Map), nameof(Map.ChangingIntoGrenade))]
    [EventPatch(typeof(Map), nameof(Map.ChangedIntoGrenade))]
    [HarmonyPatch(typeof(TimedGrenadePickup), nameof(TimedGrenadePickup.Update))]
    internal static class ChangingIntoGrenade
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // Extract the existing label we will be removing.
            Label returnLabel = generator.DefineLabel();
            Label skipFuse = generator.DefineLabel();
            Label dontResetLabel = generator.DefineLabel();

            LocalBuilder changingIntoGrenade = generator.DeclareLocal(typeof(ChangingIntoGrenadeEventArgs));
            LocalBuilder changedIntoGrenade = generator.DeclareLocal(typeof(ChangedIntoGrenadeEventArgs));
            LocalBuilder timeGrenade = generator.DeclareLocal(typeof(TimeGrenade));
            LocalBuilder thrownProjectile = generator.DeclareLocal(typeof(ThrownProjectile));

            int offset = 1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ret) + offset;

            Label enterLabel = newInstructions[index].labels[0];

            // Remove the existing instructions that get the itemBase to spawn, we will be doing this ourselves.
            int instructionsToRemove = 14;

            newInstructions.RemoveRange(index, instructionsToRemove);

            // Setup EventArgs, call event, check ev.IsAllowed and implement ev.Type changing
            newInstructions.InsertRange(index, new[]
                {
                    // this
                    new CodeInstruction(OpCodes.Ldarg_0).WithLabels(enterLabel),

                    // ChangingIntoGrenadeEventArgs ev = new(ItemPickupBase);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingIntoGrenadeEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, changingIntoGrenade.LocalIndex),

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
                    new(OpCodes.Ldloc_S, changingIntoGrenade.LocalIndex),
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

            offset = -2;
            index = newInstructions.FindIndex(i => i.Calls(Method(typeof(NetworkServer), nameof(NetworkServer.Spawn), new[] { typeof(GameObject), typeof(NetworkConnection) }))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // ¯\_(ツ)_/¯
                new(OpCodes.Dup),

                // store thrownProjectile into local var
                new(OpCodes.Stloc_S, thrownProjectile.LocalIndex),

                // this
                new CodeInstruction(OpCodes.Ldarg_0),

                // thrownProjectile
                new(OpCodes.Ldloc_S, thrownProjectile.LocalIndex),

                // var ev = new ChangedIntoGrenadeEventArgs(timedGrenadePickup, thrownProjectile);
                // Map.OnChangingIntoGrenade(ev);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangedIntoGrenadeEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Map), nameof(Map.OnChangedIntoGrenade))),
                new(OpCodes.Stloc_S, changedIntoGrenade.LocalIndex),

                // if (thrownProjectile is TimeGrenade timeGrenade)
                //    timeGrenade._fuseTime = ev.FuseTime;
                new(OpCodes.Ldloc_S, thrownProjectile.LocalIndex),
                new(OpCodes.Isinst, typeof(TimeGrenade)),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, timeGrenade.LocalIndex),
                new(OpCodes.Brfalse_S, skipFuse),

                new(OpCodes.Ldloc_S, timeGrenade.LocalIndex),
                new(OpCodes.Ldloc_S, changedIntoGrenade.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangedIntoGrenadeEventArgs), nameof(ChangedIntoGrenadeEventArgs.FuseTime))),
                new(OpCodes.Stfld, Field(typeof(TimeGrenade), nameof(TimeGrenade._fuseTime))),
                new CodeInstruction(OpCodes.Nop).WithLabels(skipFuse),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}