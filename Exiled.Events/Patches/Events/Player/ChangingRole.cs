// -----------------------------------------------------------------------
// <copyright file="ChangingRole.cs" company="Exiled Team">
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

    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items.Armor;
    using InventorySystem.Items.Pickups;

    using NorthwoodLib.Pools;

    using PlayerRoles;

    using static HarmonyLib.AccessTools;

    using Player = Handlers.Player;

    /// <summary>
    ///     Patches <see cref="PlayerRoleManager.InitializeNewRole(RoleTypeId, RoleChangeReason, RoleSpawnFlags, Mirror.NetworkReader)" />
    ///     Adds the <see cref="Player.ChangingRole" /> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerRoleManager), nameof(PlayerRoleManager.InitializeNewRole))]
    internal static class ChangingRole
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();
            Label continueLabel1 = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingRoleEventArgs));
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

                    // if (this.CurrentRole.RoleTypeId == newRole)
                    //    return;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(PlayerRoleManager), nameof(PlayerRoleManager.CurrentRole))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PlayerRoleBase), nameof(PlayerRoleBase.RoleTypeId))),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ceq),
                    new(OpCodes.Brtrue_S, continueLabel),

                    // player
                    new(OpCodes.Ldloc_S, player.LocalIndex),

                    // newRole
                    new(OpCodes.Ldarg_1),

                    // reason
                    new(OpCodes.Ldarg_2),

                    // spawnFlags
                    new(OpCodes.Ldarg_3),

                    // ChangingRoleEventArgs ev = new(Player, RoleTypeId, RoleChangeReason, SpawnFlags)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingRoleEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnChangingRole(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnChangingRole))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // newRole = ev.NewRole;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.NewRole))),
                    new(OpCodes.Starg_S, 1),

                    // reason = ev.Reason
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.Reason))),
                    new(OpCodes.Starg_S, 2),

                    // spawnFlags = ev.SpawnFlags
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.SpawnFlags))),
                    new(OpCodes.Starg_S, 3),

                    // UpdatePlayerRole(ev.NewRole, ev.Player)
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.NewRole))),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.Player))),
                    new(OpCodes.Call, Method(typeof(ChangingRole), nameof(UpdatePlayerRole))),

                    // if (ev.ShouldPreserveInventory)
                    //    goto continueLabel;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.ShouldPreserveInventory))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    // ev
                    new(OpCodes.Ldloc_S, ev.LocalIndex),

                    // this.CurrentRole.RoleTypeId
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(PlayerRoleManager), nameof(PlayerRoleManager.CurrentRole))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PlayerRoleBase), nameof(PlayerRoleBase.RoleTypeId))),

                    // ChangingRole.ChangeInventory(ev, oldRoleType);
                    new(OpCodes.Call, Method(typeof(ChangingRole), nameof(ChangeInventory))),

                    new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
                });

            int offset = 1;
            int index = newInstructions.FindIndex(
                instruction => instruction.Calls(Method(typeof(GameObjectPools.PoolObject), nameof(GameObjectPools.PoolObject.SpawnPoolObject)))) + offset;

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

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static void UpdatePlayerRole(RoleTypeId newRole, API.Features.Player player)
        {
            if (newRole is RoleTypeId.Scp173)
                Scp173Role.TurnedPlayers.Remove(player);

            player.MaxHealth = default;
        }

        private static void ChangeInventory(ChangingRoleEventArgs ev, RoleTypeId prevRole)
        {
            try
            {
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

                foreach (KeyValuePair<ItemType, ushort> keyValuePair in ev.Ammo)
                    inventory.ServerAddAmmo(keyValuePair.Key, keyValuePair.Value);

                foreach (ItemType item in ev.Items)
                    InventoryItemProvider.OnItemProvided?.Invoke(ev.Player.ReferenceHub, inventory.ServerAddItem(item));

                InventoryItemProvider.SpawnPreviousInventoryPickups(ev.Player.ReferenceHub);
            }
            catch (Exception exception)
            {
                Log.Error($"{nameof(ChangingRole)}.{nameof(ChangeInventory)}: {exception}");
            }
        }
    }
}