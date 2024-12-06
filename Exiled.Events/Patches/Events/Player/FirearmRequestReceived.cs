// -----------------------------------------------------------------------
// <copyright file="FirearmRequestReceived.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using API.Features.Core.Generic.Pools;
    using Exiled.API.Features.Items;

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using Handlers;

    using HarmonyLib;

    using InventorySystem.Items;
    using InventorySystem.Items.Firearms.BasicMessages;
    using InventorySystem.Items.Firearms.Modules;
    using PluginAPI.Events;

    using static HarmonyLib.AccessTools;

    /*
    /// <summary>
    /// Patches <see cref="FirearmBasicMessagesHandler.ServerRequestReceived" />.
    /// Adds <see cref="Player.ReloadingWeapon" />, <see cref="Player.ReloadedWeapon" />,
    /// <see cref="Player.UnloadingWeapon" />, <see cref="Player.UnloadedWeapon" /> and
    /// <see cref="Player.DryfiringWeapon" />, <see cref="Player.AimingDownSight" /> and
    /// <see cref="Player.TogglingWeaponFlashlight" />, <see cref="Player.ToggledWeaponFlashlight" /> events.
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.ReloadingWeapon))]
    [EventPatch(typeof(Player), nameof(Player.ReloadedWeapon))]
    [EventPatch(typeof(Player), nameof(Player.UnloadingWeapon))]
    [EventPatch(typeof(Player), nameof(Player.UnloadedWeapon))]
    [EventPatch(typeof(Player), nameof(Player.DryfiringWeapon))]
    [EventPatch(typeof(Player), nameof(Player.AimingDownSight))]
    [EventPatch(typeof(Player), nameof(Player.TogglingWeaponFlashlight))]
    [EventPatch(typeof(Player), nameof(Player.ToggledWeaponFlashlight))]
    [HarmonyPatch(typeof(FirearmBasicMessagesHandler), nameof(FirearmBasicMessagesHandler.ServerRequestReceived))]
    internal static class FirearmRequestReceived
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder player = generator.DeclareLocal(typeof(API.Features.Player));
            LocalBuilder firearm = generator.DeclareLocal(typeof(Firearm));

            Label returnLabel = generator.DefineLabel();

            int offset = -1;
            int index = newInstructions.FindLastIndex(instruction => instruction.LoadsField(Field(typeof(RequestMessage), nameof(RequestMessage.Request)))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // Player player = Player.Get(hub);
                // if (player == null)
                //     return;
                new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, player.LocalIndex),
                new(OpCodes.Brfalse_S, returnLabel),

                // Firearm firearm = (Firearm)Item.Get(hub);
                // if (Firearm == null)
                //     return;
                new CodeInstruction(OpCodes.Ldloc_1),
                new(OpCodes.Call, Method(typeof(API.Features.Items.Item), nameof(API.Features.Items.Item.Get), new[] { typeof(ItemBase) })),
                new(OpCodes.Isinst, typeof(Firearm)),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, firearm.LocalIndex),
                new(OpCodes.Brfalse_S, returnLabel),
            });

            offset = -2;
            index = newInstructions.FindIndex(
                instruction => instruction.opcode == OpCodes.Newobj && (ConstructorInfo)instruction.operand == GetDeclaredConstructors(typeof(PlayerReloadWeaponEvent))[0]) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // player
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex).MoveLabelsFrom(newInstructions[index]),

                    // firearm
                    new(OpCodes.Ldloc_S, firearm.LocalIndex),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ReloadingWeaponEventArgs ev = new(Player, firearm, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ReloadingWeaponEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Player.OnReloadingWeapon(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnReloadingWeapon))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ReloadingWeaponEventArgs), nameof(ReloadingWeaponEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),
                });

            offset = 2;
            index = newInstructions.FindIndex(
                instruction => instruction.opcode == OpCodes.Callvirt && instruction.operand == (object)Method(typeof(IAmmoManagerModule), nameof(IAmmoManagerModule.ServerTryReload))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // player
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex).MoveLabelsFrom(newInstructions[index]),

                    // firearm
                    new(OpCodes.Ldloc_S, firearm.LocalIndex),

                    // ReloadedWeapon ev = new(Player, firearm, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ReloadedWeaponEventArgs))[0]),

                    // Player.OnReloadedWeapon(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnReloadedWeapon))),
                });

            offset = -2;
            index = newInstructions.FindIndex(
                instruction => instruction.opcode == OpCodes.Newobj && (ConstructorInfo)instruction.operand == GetDeclaredConstructors(typeof(PlayerUnloadWeaponEvent))[0]) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // player
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex).MoveLabelsFrom(newInstructions[index]),

                    // firearm
                    new(OpCodes.Ldloc_S, firearm.LocalIndex),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // UnloadingWeaponEventArgs ev = new(Player, firearm, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UnloadingWeaponEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Player.OnUnloadingWeapon(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnUnloadingWeapon))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UnloadingWeaponEventArgs), nameof(UnloadingWeaponEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),
                });

            offset = 2;
            index = newInstructions.FindIndex(
                instruction => instruction.opcode == OpCodes.Callvirt && instruction.operand == (object)Method(typeof(IAmmoManagerModule), nameof(IAmmoManagerModule.ServerTryUnload))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // player
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex).MoveLabelsFrom(newInstructions[index]),

                    // firearm
                    new(OpCodes.Ldloc_S, firearm.LocalIndex),

                    // UnloadedWeaponEventArgs ev = new(Player, firearm, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UnloadedWeaponEventArgs))[0]),

                    // Player.OnUnloadedWeapon(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnUnloadedWeapon))),
                });

            offset = -2;
            index = newInstructions.FindIndex(
                instruction => instruction.opcode == OpCodes.Newobj && (ConstructorInfo)instruction.operand == GetDeclaredConstructors(typeof(PlayerDryfireWeaponEvent))[0]) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // player
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex).MoveLabelsFrom(newInstructions[index]),

                    // firearm
                    new(OpCodes.Ldloc_S, firearm.LocalIndex),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // DryfiringWeaponEventArgs ev = new(Player, firearm, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DryfiringWeaponEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Player.OnDryfiringWeapon(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnDryfiringWeapon))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DryfiringWeaponEventArgs), nameof(DryfiringWeaponEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),
                });

            offset = -3;
            index = newInstructions.FindIndex(
                instruction => instruction.opcode == OpCodes.Newobj && (ConstructorInfo)instruction.operand == GetDeclaredConstructors(typeof(PlayerAimWeaponEvent))[0]) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // player
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex).MoveLabelsFrom(newInstructions[index]),

                    // firearm
                    new(OpCodes.Ldloc_S, firearm.LocalIndex),

                    // true (adsIn)
                    new(OpCodes.Ldc_I4_1),

                    // AimingDownSightEventArgs ev = new(Player, firearm, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AimingDownSightEventArgs))[0]),

                    // Player.OnAimingDownSight(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnAimingDownSight))),
                });

            offset = -3;
            index = newInstructions.FindLastIndex(
                instruction => instruction.opcode == OpCodes.Newobj && (ConstructorInfo)instruction.operand == GetDeclaredConstructors(typeof(PlayerAimWeaponEvent))[0]) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // player
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex).MoveLabelsFrom(newInstructions[index]),

                    // firearm
                    new(OpCodes.Ldloc_S, firearm.LocalIndex),

                    // false (adsIn)
                    new(OpCodes.Ldc_I4_0),

                    // AimingDownSightEventArgs ev = new(Player, firearm, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AimingDownSightEventArgs))[0]),

                    // Player.OnAimingDownSight(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnAimingDownSight))),
                });

            offset = -5;
            index = newInstructions.FindLastIndex(
                instruction => instruction.opcode == OpCodes.Newobj && (ConstructorInfo)instruction.operand == GetDeclaredConstructors(typeof(PlayerToggleFlashlightEvent))[0]) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // player
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),

                    // firearm
                    new(OpCodes.Ldloc_S, firearm.LocalIndex),

                    // !flag
                    new(OpCodes.Ldloc_S, 8),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ceq),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // TogglingWeaponFlashlightEventArgs ev = new(Player, firearm, bool, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(TogglingWeaponFlashlightEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Player.OnTogglingWeaponFlashlight(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnTogglingWeaponFlashlight))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TogglingWeaponFlashlightEventArgs), nameof(TogglingWeaponFlashlightEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            offset = 1;
            index = newInstructions.FindIndex(
                instruction => instruction.opcode == OpCodes.Callvirt && instruction.operand == (object)PropertySetter(typeof(InventorySystem.Items.Firearms.Firearm), nameof(InventorySystem.Items.Firearms.Firearm.Status))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // player
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),

                    // firearm
                    new(OpCodes.Ldloc_S, firearm.LocalIndex),

                    // !flag
                    new(OpCodes.Ldloc_S, 8),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ceq),

                    // TogglingWeaponFlashlightEventArgs ev = new(Player, firearm, bool, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ToggledWeaponFlashlightEventArgs))[0]),

                    // Player.OnToggledWeaponFlashlight(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnToggledWeaponFlashlight))),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
    */
}