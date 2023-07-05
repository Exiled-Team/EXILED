// -----------------------------------------------------------------------
// <copyright file="Shooting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Items;
    using API.Features.Pools;

    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items.Firearms.BasicMessages;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="FirearmBasicMessagesHandler.ServerShotReceived" />.
    ///     Adds the <see cref="Handlers.Player.Shooting" /> events.
    /// </summary>
    [HarmonyPatch(typeof(FirearmBasicMessagesHandler), nameof(FirearmBasicMessagesHandler.ServerShotReceived))]
    internal static class Shooting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabelModAmmo = generator.DefineLabel();
            Label isAllowedLabel = generator.DefineLabel();
            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ShootingEventArgs));
            LocalBuilder isAllowed = generator.DeclareLocal(typeof(bool));
            LocalBuilder firearm = generator.DeclareLocal(typeof(Item));
            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            int offset = 2;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Pop) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Pop ldloc.1 in the stack
                    new(OpCodes.Pop),

                    // Player.Get(referenceHub)
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, player.LocalIndex),

                    // Check to make sure the player is not null. (Yes, this happens)
                    new(OpCodes.Brfalse_S, returnLabel),
                    new(OpCodes.Ldloc_S, player.LocalIndex),

                    // msg
                    new(OpCodes.Ldarg_1),

                    // ShootingEventArgs ev = new(Player, ShotMessage)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ShootingEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Handlers.Player.OnShooting(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnShooting))),

                    // isAllowed = ev.IsAllowed
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ShootingEventArgs), nameof(ShootingEventArgs.IsAllowed))),
                    new(OpCodes.Stloc_S, isAllowed.LocalIndex),

                    // firearm = ev.Player.CurrentItem
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ShootingEventArgs), nameof(ShootingEventArgs.Player))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.CurrentItem))),
                    new(OpCodes.Stloc_S, firearm.LocalIndex),

                    // if (isAllowed)
                    //    goto isAllowedLabel;
                    new(OpCodes.Ldloc_S, isAllowed.LocalIndex),
                    new(OpCodes.Brtrue_S, isAllowedLabel),

                    // if (firearm is null)
                    //    return;
                    new(OpCodes.Ldloc_S, firearm.LocalIndex),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // goto returnLabelModAmmo
                    new(OpCodes.Br_S, returnLabelModAmmo),

                    // isAllowedLabel:
                    //
                    // msg = ev.ShotMessage
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(isAllowedLabel),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ShootingEventArgs), nameof(ShootingEventArgs.ShotMessage))),
                    new(OpCodes.Starg_S, 1),

                    // Reload the popped ldloc.1
                    new(OpCodes.Ldloc_1),
                });

            newInstructions.InsertRange(
                newInstructions.Count - 1,
                new[]
                {
                    // returnLabelModAmmo:
                    //
                    // if (isAllowed)
                    //    return;
                    new CodeInstruction(OpCodes.Ldloc_S, isAllowed.LocalIndex).WithLabels(returnLabelModAmmo),
                    new(OpCodes.Brtrue_S, returnLabel),

                    // firearm.Ammo += 1
                    new(OpCodes.Ldloc_S, firearm.LocalIndex),
                    new(OpCodes.Dup),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Firearm), nameof(Firearm.Ammo))),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Add),
                    new(OpCodes.Callvirt, PropertySetter(typeof(Firearm), nameof(Firearm.Ammo))),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}