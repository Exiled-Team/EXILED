// -----------------------------------------------------------------------
// <copyright file="ChangingRoleAndSpawned.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Core.Generic.Pools;

    using API.Features.Roles;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items.Armor;
    using InventorySystem.Items.Pickups;

    using PlayerRoles;

    using static HarmonyLib.AccessTools;

    using Player = Handlers.Player;

    /// <summary>
    /// Patches <see cref="PlayerRoleManager.InitializeNewRole(RoleTypeId, RoleChangeReason, RoleSpawnFlags, Mirror.NetworkReader)" />
    /// Adds the <see cref="Player.ChangingRole" /> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerRoleManager), nameof(PlayerRoleManager.InitializeNewRole))]
    internal static class ChangingRoleAndSpawned
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();
            Label continueLabel1 = generator.DefineLabel();
            Label continueLabel2 = generator.DefineLabel();
            Label jmp = generator.DefineLabel();

            LocalBuilder changingRoleEventArgs = generator.DeclareLocal(typeof(ChangingRoleEventArgs));
            LocalBuilder player = generator.DeclareLocal(typeof(API.Features.Player));

            newInstructions.InsertRange(
                0,
                new[]
                {
                    // player = Player.Get(this._hub)
                    //
                    // if (player == null)
                    //    goto continueLabel;
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(PlayerRoleManager), nameof(PlayerRoleManager.Hub))),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, player.LocalIndex),
                    new(OpCodes.Brfalse_S, continueLabel),

                    // if (Player.IsVerified)
                    //  goto jmp
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.IsVerified))),
                    new(OpCodes.Brtrue_S, jmp),

                    // if (!Player.IsNpc)
                    //  goto continueLabel;
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.IsNPC))),
                    new(OpCodes.Brfalse_S, continueLabel),

                    // jmp
                    // player
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex).WithLabels(jmp),

                    // newRole
                    new(OpCodes.Ldarg_1),

                    // reason
                    new(OpCodes.Ldarg_2),

                    // spawnFlags
                    new(OpCodes.Ldarg_3),

                    // ChangingRoleEventArgs changingRoleEventArgs = new(Player, RoleTypeId, RoleChangeReason, SpawnFlags)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingRoleEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, changingRoleEventArgs.LocalIndex),

                    // Handlers.Player.OnChangingRole(changingRoleEventArgs)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnChangingRole))),

                    // if (!changingRoleEventArgs.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // newRole = changingRoleEventArgs.NewRole;
                    new(OpCodes.Ldloc_S, changingRoleEventArgs.LocalIndex),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.NewRole))),
                    new(OpCodes.Starg_S, 1),

                    // reason = changingRoleEventArgs.Reason
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.Reason))),
                    new(OpCodes.Starg_S, 2),

                    // spawnFlags = changingRoleEventArgs.SpawnFlags
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.SpawnFlags))),
                    new(OpCodes.Starg_S, 3),

                    // UpdatePlayerRole(changingRoleEventArgs.NewRole, changingRoleEventArgs.Player)
                    new(OpCodes.Ldloc_S, changingRoleEventArgs.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.NewRole))),
                    new(OpCodes.Ldloc_S, changingRoleEventArgs.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.Player))),
                    new(OpCodes.Call, Method(typeof(ChangingRoleAndSpawned), nameof(UpdatePlayerRole))),

                    new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
                });

            int offset = 1;
            int index = newInstructions.FindIndex(
                instruction => instruction.Calls(Method(typeof(GameObjectPools.PoolObject), nameof(GameObjectPools.PoolObject.SetupPoolObject)))) + offset;

            newInstructions[index].WithLabels(continueLabel1);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // if (player == null)
                    //     continue
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Brfalse_S, continueLabel1),

                    // player.Role = Role.Create(roleBase);
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Ldloc_2),
                    new(OpCodes.Call, Method(typeof(Role), nameof(Role.Create))),
                    new(OpCodes.Callvirt, PropertySetter(typeof(API.Features.Player), nameof(API.Features.Player.Role))),
                });

            offset = 1;
            index = newInstructions.FindIndex(i => i.Calls(Method(typeof(PlayerRoleManager.RoleChanged), nameof(PlayerRoleManager.RoleChanged.Invoke)))) + offset;

            newInstructions[index].labels.Add(continueLabel2);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // if (player == null)
                    //     continue
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Brfalse_S, continueLabel2),

                    // if (changingRoleEventArgs == null)
                    //     continue
                    new CodeInstruction(OpCodes.Ldloc_S, changingRoleEventArgs.LocalIndex),
                    new(OpCodes.Brfalse_S, continueLabel2),

                    // changingRoleEventArgs
                    new(OpCodes.Ldloc_S, changingRoleEventArgs.LocalIndex),

                    // ChangingRole.ChangeInventory(changingRoleEventArgs, oldRoleType);
                    new(OpCodes.Call, Method(typeof(ChangingRoleAndSpawned), nameof(ChangeInventory))),
                });

            newInstructions.InsertRange(
                newInstructions.Count - 1,
                new[]
                {
                    // if (player == null)
                    //     continue
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // player
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),

                    // OldRole
                    new(OpCodes.Ldloc_0),

                    // SpawnedEventArgs spawnedEventArgs = new(Player, OldRole)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawnedEventArgs))[0]),

                    // Handlers.Player.OnSpawned(spawnedEventArgs)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnSpawned))),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void UpdatePlayerRole(RoleTypeId newRole, API.Features.Player player)
        {
            if (newRole is RoleTypeId.Scp173)
                Scp173Role.TurnedPlayers.Remove(player);

            player.MaxHealth = default;
        }

        private static void ChangeInventory(ChangingRoleEventArgs ev)
        {
            try
            {
                if (ev.ShouldPreserveInventory || ev.Reason == API.Enums.SpawnReason.Destroyed)
                    return;

                Inventory inventory = ev.Player.Inventory;

                if (ev.Reason == API.Enums.SpawnReason.Escaped)
                {
                    List<ItemPickupBase> list = new();
                    if (inventory.TryGetBodyArmor(out BodyArmor bodyArmor))
                        bodyArmor.DontRemoveExcessOnDrop = true;

                    while (inventory.UserInventory.Items.Count > 0)
                    {
                        int startCount = inventory.UserInventory.Items.Count;
                        ushort key = inventory.UserInventory.Items.ElementAt(0).Key;
                        ItemPickupBase item = inventory.ServerDropItem(key);

                        // If the list wasn't changed, we need to manually remove the item to avoid a softlock.
                        if (startCount == inventory.UserInventory.Items.Count)
                            inventory.UserInventory.Items.Remove(key);
                        else
                            list.Add(item);
                    }

                    InventoryItemProvider.PreviousInventoryPickups[ev.Player.ReferenceHub] = list;
                }
                else
                {
                    while (inventory.UserInventory.Items.Count > 0)
                    {
                        int startCount = inventory.UserInventory.Items.Count;
                        ushort key = inventory.UserInventory.Items.ElementAt(0).Key;
                        inventory.ServerRemoveItem(key, null);

                        // If the list wasn't changed, we need to manually remove the item to avoid a softlock.
                        if (startCount == inventory.UserInventory.Items.Count)
                            inventory.UserInventory.Items.Remove(key);
                    }

                    inventory.UserInventory.ReserveAmmo.Clear();
                    inventory.SendAmmoNextFrame = true;
                }

                foreach (ItemType item in ev.Items)
                    inventory.ServerAddItem(item);

                foreach (KeyValuePair<ItemType, ushort> keyValuePair in ev.Ammo)
                    inventory.ServerAddAmmo(keyValuePair.Key, keyValuePair.Value);

                foreach (KeyValuePair<ushort, InventorySystem.Items.ItemBase> item in inventory.UserInventory.Items)
                    InventoryItemProvider.OnItemProvided?.Invoke(ev.Player.ReferenceHub, item.Value);

                InventoryItemProvider.SpawnPreviousInventoryPickups(ev.Player.ReferenceHub);
            }
            catch (Exception exception)
            {
                Log.Error($"{nameof(ChangingRoleAndSpawned)}.{nameof(ChangeInventory)}: {exception}");
            }
        }
    }
}