// -----------------------------------------------------------------------
// <copyright file="ChangingRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items.Firearms.Attachments;

    using MEC;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.Events.Handlers.Player;

    /// <summary>
    /// Patches <see cref="CharacterClassManager.SetClassIDAdv(RoleType, bool, CharacterClassManager.SpawnReason, bool)"/>.
    /// Adds the <see cref="Handlers.Player.ChangingRole"/> and <see cref="Handlers.Player.Escaping"/> events.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.SetClassIDAdv))]
    internal static class ChangingRole
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            int offset = 0;
            int index = 0;

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingRoleEventArgs));
            LocalBuilder player = generator.DeclareLocal(typeof(API.Features.Player));
            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(this._hub)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(CharacterClassManager), nameof(CharacterClassManager._hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, player.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),
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
            });

            offset = 1;
            index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Call) + offset;
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_3),
                new CodeInstruction(OpCodes.Call, Method(typeof(ChangingRole), nameof(ShouldUpdateInv))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.Lite))),
                new CodeInstruction(OpCodes.Brtrue, returnLabel),
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.Player))),
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingRoleEventArgs), nameof(ChangingRoleEventArgs.Items))),
                new CodeInstruction(OpCodes.Call, Method(typeof(ChangingRole), nameof(ChangeInventory))),
            });
            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static bool ShouldUpdateInv(RoleType type, CharacterClassManager.SpawnReason reason) =>
            (reason != CharacterClassManager.SpawnReason.Escaped ||
             !CharacterClassManager.KeepItemsAfterEscaping) && type != RoleType.Spectator;

        private static void ChangeInventory(Exiled.API.Features.Player player, List<ItemType> items)
        {
            player.ClearInventory();
            Timing.CallDelayed(0.25f, () =>
            {
                try
                {
                    items.Reverse();
                    foreach (ItemType type in items)
                    {
                        Item item = player.AddItem(type);
                    }

                    if (InventorySystem.Configs.StartingInventories.DefinedInventories.ContainsKey(player.Role))
                    {
                        foreach (KeyValuePair<ItemType, ushort> kvp in InventorySystem.Configs.StartingInventories.DefinedInventories[player.Role].Ammo)
                        {
                            player.Inventory.ServerSetAmmo(kvp.Key, kvp.Value);
                            player.Inventory.SendAmmoNextFrame = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"{nameof(ChangingRole)}.{nameof(ChangeInventory)}: {e}");
                }
            });
        }
    }
}
