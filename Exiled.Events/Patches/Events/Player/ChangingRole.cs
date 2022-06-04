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
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items.Armor;
    using InventorySystem.Items.Pickups;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.Events.Handlers.Player;
    using Role = Role;
    using Scp173 = Exiled.API.Features.Scp173;

    /// <summary>
    ///     Patches <see cref="CharacterClassManager.SetClassIDAdv(RoleType, bool, CharacterClassManager.SpawnReason, bool)" />
    ///     .
    ///     Adds the <see cref="Handlers.Player.ChangingRole" /> and <see cref="Handlers.Player.Escaping" /> events.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.SetClassIDAdv))]
    internal static class ChangingRole
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            int offset = 5;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldfld) + offset;

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingRoleEventArgs));
            LocalBuilder player = generator.DeclareLocal(typeof(API.Features.Player));
            Label returnLabel = generator.DefineLabel();
            Label liteLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(this._hub)
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Ldfld, Field(typeof(CharacterClassManager), nameof(CharacterClassManager._hub))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, player.LocalIndex),
                new(OpCodes.Brfalse, returnLabel),
                new(OpCodes.Ldloc, player.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.Role))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Roles.Role), nameof(API.Features.Player.Role.Type))),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ceq),
                new(OpCodes.Brtrue, returnLabel),
                new(OpCodes.Ldloc, player.LocalIndex),

                // id
                new(OpCodes.Ldarg_1),

                // lite
                new(OpCodes.Ldarg_2),

                // escape
                new(OpCodes.Ldarg_3),

                // var ev = new ChangingRoleEventArgs(player, id, lite, escape)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingRoleEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, ev.LocalIndex),

                // Handlers.Player.OnChangingRole(ev)
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnChangingRole))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.IsAllowed))),
                new(OpCodes.Brfalse, returnLabel),

                // id = ev.NewRole;
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.NewRole))),
                new(OpCodes.Starg, 1),

                // lite = ev.Lite
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.Lite))),
                new(OpCodes.Starg, 2),

                // escape = ev.IsEscaped
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.Reason))),
                new(OpCodes.Starg, 3),

                // ev.Player.MaxHealth = this.Classes.SafeGet(ev.NewRole)
                new(OpCodes.Ldloc, player.LocalIndex),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(CharacterClassManager), nameof(CharacterClassManager.Classes))),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(RoleExtensionMethods), nameof(RoleExtensionMethods.SafeGet), new[] { typeof(Role[]), typeof(RoleType) })),
                new(OpCodes.Ldfld, Field(typeof(Role), nameof(Role.maxHP))),
                new(OpCodes.Call, PropertySetter(typeof(API.Features.Player), nameof(API.Features.Player.MaxHealth))),

                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.NewRole))),
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.Player))),
                new(OpCodes.Call, Method(typeof(ChangingRole), nameof(UpdatePlayerRole))),
            });

            offset = 0;
            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Callvirt && i.operand is MethodInfo method && method.DeclaringType == typeof(CharacterClassManager.ClassChangedAdvanced)) +
                    offset;
            newInstructions[index + 1].labels.Add(liteLabel);
            newInstructions.InsertRange(index + 1, new CodeInstruction[]
            {
                // if (ev.Lite)
                //    break;
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.Lite))),
                new(OpCodes.Brtrue, liteLabel),

                // player
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.Player))),

                // items
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.Items))),

                // ammo
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.Ammo))),

                // prevRole
                new(OpCodes.Ldloc_0),

                // newRole
                new(OpCodes.Ldarg_1),

                // reason
                new(OpCodes.Ldarg_3),

                // ChangingRole.ChangeInventory(ev.Player, ev.Items, ev.Ammo, curClass, id, reason);
                new(OpCodes.Call, Method(typeof(ChangingRole), nameof(ChangeInventory))),
            });
            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static void UpdatePlayerRole(RoleType newRole, API.Features.Player player)
        {
            if (newRole is RoleType.Scp173) Scp173.TurnedPlayers.Remove(player);

            player.Role = API.Features.Roles.Role.Create(newRole, player);
        }

        private static void ChangeInventory(API.Features.Player player, List<ItemType> items, Dictionary<ItemType, ushort> ammo, RoleType prevRole, RoleType newRole,
            CharacterClassManager.SpawnReason reason)
        {
            try
            {
                Inventory inventory = player.Inventory;
                if (reason == CharacterClassManager.SpawnReason.Escaped && prevRole != newRole)
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

                    InventoryItemProvider.PreviousInventoryPickups[player.ReferenceHub] = list;
                }
                else
                {
                    while (inventory.UserInventory.Items.Count > 0)
                    {
                        int startCount = inventory.UserInventory.Items.Count;
                        ushort key = inventory.UserInventory.Items.ElementAt(0).Key;
                        inventory.ServerRemoveItem(key, null);

                        // If the list wasn't changed, we need to manually remove the item to avoid a softlock.
                        if (startCount == inventory.UserInventory.Items.Count) inventory.UserInventory.Items.Remove(key);
                    }

                    inventory.UserInventory.ReserveAmmo.Clear();
                    inventory.SendAmmoNextFrame = true;
                }

                foreach (KeyValuePair<ItemType, ushort> keyValuePair in ammo)
                    inventory.ServerAddAmmo(keyValuePair.Key, keyValuePair.Value);
                foreach (ItemType item in items)
                    InventoryItemProvider.OnItemProvided?.Invoke(player.ReferenceHub, inventory.ServerAddItem(item));
            }
            catch (Exception e)
            {
                Log.Error($"{nameof(ChangingRole)}.{nameof(ChangeInventory)}: {e}");
            }
        }
    }
}
