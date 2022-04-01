// -----------------------------------------------------------------------
// <copyright file="ChangingRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player {
#pragma warning disable SA1118
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items.Armor;
    using InventorySystem.Items.Pickups;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.Events.Handlers.Player;

    /// <summary>
    /// Patches <see cref="CharacterClassManager.SetClassIDAdv(RoleType, bool, CharacterClassManager.SpawnReason, bool)"/>.
    /// Adds the <see cref="Handlers.Player.ChangingRole"/> and <see cref="Handlers.Player.Escaping"/> events.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.SetClassIDAdv))]
    internal static class ChangingRole {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
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
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(CharacterClassManager), nameof(CharacterClassManager._hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, player.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),
                new CodeInstruction(OpCodes.Ldloc, player.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.Role))),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brtrue, returnLabel),
                new CodeInstruction(OpCodes.Ldloc, player.LocalIndex),

                // id
                new CodeInstruction(OpCodes.Ldarg_1),

                // lite
                new CodeInstruction(OpCodes.Ldarg_2),

                // escape
                new CodeInstruction(OpCodes.Ldarg_3),

                // var ev = new ChangingRoleEventArgs(player, id, lite, escape)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingRoleEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, ev.LocalIndex),

                // Handlers.Player.OnChangingRole(ev)
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.OnChangingRole))),

                // if (!ev.IsAllowed)
                //    return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),

                // id = ev.NewRole;
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.NewRole))),
                new CodeInstruction(OpCodes.Starg, 1),

                // lite = ev.Lite
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.Lite))),
                new CodeInstruction(OpCodes.Starg, 2),

                // escape = ev.IsEscaped
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.Reason))),
                new CodeInstruction(OpCodes.Starg, 3),

                // ev.Player.MaxHealth = this.Classes.SafeGet(ev.NewRole)
                new CodeInstruction(OpCodes.Ldloc, player.LocalIndex),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(CharacterClassManager), nameof(CharacterClassManager.Classes))),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(RoleExtensionMethods), nameof(RoleExtensionMethods.SafeGet), new[] { typeof(Role[]), typeof(RoleType) })),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Role), nameof(Role.maxHP))),
                new CodeInstruction(OpCodes.Call, PropertySetter(typeof(API.Features.Player), nameof(API.Features.Player.MaxHealth))),
            });

            offset = 0;
            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Callvirt && i.operand is MethodInfo method && method.DeclaringType == typeof(CharacterClassManager.ClassChangedAdvanced)) + offset;
            newInstructions[index + 1].WithLabels(liteLabel);
            newInstructions.InsertRange(index + 1, new[]
            {
                // if (ev.Lite)
                //    break;
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.Lite))),
                new CodeInstruction(OpCodes.Brtrue, liteLabel),

                // player
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.Player))),

                // items
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.Items))),

                // ammo
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.Ammo))),

                // prevRole
                new CodeInstruction(OpCodes.Ldloc_0),

                // newRole
                new CodeInstruction(OpCodes.Ldarg_1),

                // reason
                new CodeInstruction(OpCodes.Ldarg_3),

                // ChangingRole.ChangeInventory(ev.Player, ev.Items, ev.Ammo, curClass, id, reason);
                new CodeInstruction(OpCodes.Call, Method(typeof(ChangingRole), nameof(ChangeInventory))),
            });
            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static void ChangeInventory(Exiled.API.Features.Player player, List<ItemType> items, Dictionary<ItemType, ushort> ammo, RoleType prevRole, RoleType newRole, CharacterClassManager.SpawnReason reason) {
            try {
                Inventory inventory = player.Inventory;
                if (reason == CharacterClassManager.SpawnReason.Escaped && prevRole != newRole) {
                    List<ItemPickupBase> list = new List<ItemPickupBase>();
                    if (inventory.TryGetBodyArmor(out BodyArmor bodyArmor))
                        bodyArmor.DontRemoveExcessOnDrop = true;
                    while (inventory.UserInventory.Items.Count > 0) {
                        int startCount = inventory.UserInventory.Items.Count;
                        ushort key = inventory.UserInventory.Items.ElementAt(0).Key;
                        ItemPickupBase item = inventory.ServerDropItem(key);

                        // If the list wasn't changed, we need to manually remove the item to avoid a softlock.
                        if (startCount == inventory.UserInventory.Items.Count) {
                            inventory.UserInventory.Items.Remove(key);
                        }
                        else {
                            list.Add(item);
                        }
                    }

                    InventoryItemProvider.PreviousInventoryPickups[player.ReferenceHub] = list;
                }
                else {
                    while (inventory.UserInventory.Items.Count > 0) {
                        int startCount = inventory.UserInventory.Items.Count;
                        ushort key = inventory.UserInventory.Items.ElementAt(0).Key;
                        inventory.ServerRemoveItem(key, null);

                        // If the list wasn't changed, we need to manually remove the item to avoid a softlock.
                        if (startCount == inventory.UserInventory.Items.Count) {
                            inventory.UserInventory.Items.Remove(key);
                        }
                    }

                    inventory.UserInventory.ReserveAmmo.Clear();
                    inventory.SendAmmoNextFrame = true;
                }

                foreach (KeyValuePair<ItemType, ushort> keyValuePair in ammo)
                    inventory.ServerAddAmmo(keyValuePair.Key, keyValuePair.Value);
                foreach (ItemType item in items)
                    InventoryItemProvider.OnItemProvided?.Invoke(player.ReferenceHub, inventory.ServerAddItem(item));
            }
            catch (Exception e) {
                Log.Error($"{nameof(ChangingRole)}.{nameof(ChangeInventory)}: {e}");
            }
        }
    }
}
