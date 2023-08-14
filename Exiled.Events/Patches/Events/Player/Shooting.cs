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

    using CustomPlayerEffects;

    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items;
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

            Label isAllowedLabel = generator.DefineLabel();
            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ShootingEventArgs));
            LocalBuilder firearm = generator.DeclareLocal(typeof(Item));
            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            int offset = -1;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(SpawnProtected), nameof(SpawnProtected.CheckPlayer)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(referenceHub)
                    new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, player.LocalIndex),

                    // if (player == null)
                    //     return;
                    new(OpCodes.Brfalse_S, returnLabel),

                    // firearm = (Firearm)Item.Get(curInstance)
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Call, Method(typeof(Item), nameof(Item.Get), new[] { typeof(ItemBase) })),
                    new(OpCodes.Isinst, typeof(Firearm)),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, firearm.LocalIndex),

                    // if (firearm == null)
                    //     return;
                    new(OpCodes.Brfalse_S, returnLabel),

                    // player
                    new(OpCodes.Ldloc_S, player.LocalIndex),

                    // firearm
                    new(OpCodes.Ldloc_S, firearm.LocalIndex),

                    // msg
                    new(OpCodes.Ldarg_1),

                    // ShootingEventArgs ev = new(Player, firearm, ShotMessage)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ShootingEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Handlers.Player.OnShooting(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnShooting))),

                    // if (ev.IsAllowed)
                    //    goto isAllowedLabel;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ShootingEventArgs), nameof(ShootingEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, isAllowedLabel),

                    // firearm.Ammo += 1;
                    // return;
                    new(OpCodes.Ldloc_S, firearm.LocalIndex),
                    new(OpCodes.Dup),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Firearm), nameof(Firearm.Ammo))),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Add),
                    new(OpCodes.Callvirt, PropertySetter(typeof(Firearm), nameof(Firearm.Ammo))),
                    new(OpCodes.Ret),

                    // isAllowedLabel:
                    // msg = ev.ShotMessage
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(isAllowedLabel),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ShootingEventArgs), nameof(ShootingEventArgs.ShotMessage))),
                    new(OpCodes.Starg_S, 1),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}