// -----------------------------------------------------------------------
// <copyright file="KeycardInteracting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Item
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;

    using Exiled.Events;
    using Exiled.Events.EventArgs.Item;

    using Footprinting;

    using HarmonyLib;

    using Interactables.Interobjects.DoorUtils;

    using InventorySystem.Items.Keycards;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="KeycardPickup.ProcessCollision(Collision)"/>.
    /// Adds the <see cref="Handlers.Player.InteractingDoor"/> event.
    /// </summary>
    [HarmonyPatch(typeof(KeycardPickup), nameof(KeycardPickup.ProcessCollision))]
    internal static class KeycardInteracting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder isUnlocked = generator.DeclareLocal(typeof(bool));
            LocalBuilder notEmptyPermissions = generator.DeclareLocal(typeof(bool));
            LocalBuilder havePermissions = generator.DeclareLocal(typeof(bool));

            Label skip = generator.DefineLabel();
            Label skip2 = generator.DefineLabel();
            Label ret = generator.DefineLabel();

            int offset = 1;
            int index = newInstructions.FindIndex(i => i.LoadsField(Field(typeof(DoorVariant), nameof(DoorVariant.ActiveLocks)))) + offset;

            newInstructions.RemoveAt(index);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ceq),
                    new CodeInstruction(OpCodes.Stloc_S, isUnlocked.LocalIndex),
                });

            index = newInstructions.FindIndex(i => i.LoadsField(Field(typeof(DoorPermissions), nameof(DoorPermissions.RequiredPermissions)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // checking empty permissions
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Cgt),

                    new(OpCodes.Stloc_S, notEmptyPermissions.LocalIndex),
                    new(OpCodes.Br_S, skip),

                    // save original return
                    new CodeInstruction(OpCodes.Ret).MoveLabelsFrom(newInstructions[index + 1]),
                    new CodeInstruction(OpCodes.Nop).WithLabels(skip),
                });

            newInstructions.RemoveRange(index + 6, 2);

            index = newInstructions.FindIndex(i => i.Calls(Method(typeof(DoorPermissions), nameof(DoorPermissions.CheckPermissions)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    new(OpCodes.Stloc_S, havePermissions.LocalIndex),
                    new(OpCodes.Br_S, skip2),

                    // save original return
                    new CodeInstruction(OpCodes.Ret).MoveLabelsFrom(newInstructions[index + 1]),
                    new CodeInstruction(OpCodes.Nop).WithLabels(skip2),
                });

            newInstructions.RemoveRange(index + 4, 2);

            offset = -5;
            index = newInstructions.FindIndex(i => i.Calls(PropertySetter(typeof(DoorVariant), nameof(DoorVariant.NetworkTargetState)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // pickup
                    new(OpCodes.Ldarg_0),

                    // PreviousOwner.Hub
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Ldflda, Field(typeof(KeycardPickup), nameof(KeycardPickup.PreviousOwner))),
                    new(OpCodes.Ldfld, Field(typeof(Footprint), nameof(Footprint.Hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // door
                    new(OpCodes.Ldloc_1),

                    // allowed calculate
                    new(OpCodes.Ldloc_S, isUnlocked),

                    new(OpCodes.Ldloc_S, havePermissions),

                    new(OpCodes.Ldloc_S, notEmptyPermissions),
                    new(OpCodes.Call, PropertyGetter(typeof(Events), nameof(Events.Instance))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Events), nameof(Events.Config))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.CanKeycardThrowAffectDoors))),
                    new(OpCodes.Or),

                    new(OpCodes.And),
                    new(OpCodes.And),

                    // ThrowKeycardInteractingEventArgs ev = new(pickup, player, door, isAllowed);
                    //
                    // Item.OnThrowKeycardInteracting(ev);
                    //
                    // if (!ev.IsAllowed)
                    //     return;
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(KeycardInteractingEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnKeycardInteracting))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(KeycardInteractingEventArgs), nameof(KeycardInteractingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, ret),
                });

            index = newInstructions.Count - 1;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // call animation sets(we don't want to call the event more than once)
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new(OpCodes.Callvirt, Method(typeof(DoorVariant), nameof(DoorVariant.TargetStateChanged))),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}