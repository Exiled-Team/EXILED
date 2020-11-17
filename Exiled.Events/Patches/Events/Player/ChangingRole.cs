// -----------------------------------------------------------------------
// <copyright file="ChangingRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;
    using System.Collections.Generic;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="CharacterClassManager.SetPlayersClass(RoleType, GameObject, bool, bool)"/>.
    /// Adds the <see cref="Player.ChangingRole"/> and <see cref="Player.Escaping"/> events.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.SetPlayersClass))]
    internal static class ChangingRole
    {
        private static bool Prefix(CharacterClassManager __instance, ref RoleType classid, GameObject ply, bool lite = false, bool escape = false)
        {
            try
            {
                // Somehow we've seen spam
                // here with a NullReferenceException,
                // so there are more null checks here
                if (ply == null
                || !ply.TryGetComponent<CharacterClassManager>(out var ccm)
                || ccm == null
                || !ccm.IsVerified)
                {
                    return false;
                }

                var startItemsList = ListPool<ItemType>.Shared.Rent(__instance.Classes.SafeGet(classid).startItems);
                var changingRoleEventArgs = new ChangingRoleEventArgs(API.Features.Player.Get(ply), classid, startItemsList, lite, escape);

                Player.OnChangingRole(changingRoleEventArgs);

                lite = changingRoleEventArgs.ShouldPreservePosition;
                escape = changingRoleEventArgs.IsEscaped;

                if (classid != RoleType.Spectator && changingRoleEventArgs.NewRole == RoleType.Spectator)
                {
                    var diedEventArgs = new DiedEventArgs(API.Features.Server.Host, changingRoleEventArgs.Player, new PlayerStats.HitInfo(-1, "Dedicated Server", DamageTypes.None, 0));
                    Player.OnDied(diedEventArgs);
                }

                classid = changingRoleEventArgs.NewRole;

                if (escape)
                {
                    var escapingEventArgs = new EscapingEventArgs(API.Features.Player.Get(ply), classid);

                    Player.OnEscaping(escapingEventArgs);

                    if (!escapingEventArgs.IsAllowed)
                        return false;

                    classid = escapingEventArgs.NewRole;
                }

                ply.GetComponent<CharacterClassManager>().SetClassIDAdv(classid, lite, escape);
                ply.GetComponent<PlayerStats>().SetHPAmount(__instance.Classes.SafeGet(classid).maxHP);
                ply.GetComponent<FirstPersonController>().ModifyStamina(100f);

                if (lite)
                {
                    ListPool<ItemType>.Shared.Return(startItemsList);
                    return false;
                }

                Inventory component = ply.GetComponent<Inventory>();
                List<Inventory.SyncItemInfo> list = ListPool<Inventory.SyncItemInfo>.Shared.Rent();
                if (escape && CharacterClassManager.KeepItemsAfterEscaping)
                {
                    foreach (Inventory.SyncItemInfo item in component.items)
                        list.Add(item);
                }

                component.items.Clear();
                foreach (ItemType id in changingRoleEventArgs.Items)
                {
                    component.AddNewItem(id, -4.65664672E+11f, 0, 0, 0);
                }

                ListPool<ItemType>.Shared.Return(startItemsList);

                if (escape && CharacterClassManager.KeepItemsAfterEscaping)
                {
                    foreach (Inventory.SyncItemInfo syncItemInfo in list)
                    {
                        if (CharacterClassManager.PutItemsInInvAfterEscaping)
                        {
                            var itemByID = component.GetItemByID(syncItemInfo.id);
                            bool flag = false;
                            InventoryCategory[] categories = __instance._search.categories;
                            int i = 0;
                            while (i < categories.Length)
                            {
                                InventoryCategory inventoryCategory = categories[i];
                                if (inventoryCategory.itemType == itemByID.itemCategory &&
                                    (itemByID.itemCategory != ItemCategory.None ||
                                     itemByID.itemCategory != ItemCategory.None))
                                {
                                    int num = 0;
                                    foreach (Inventory.SyncItemInfo syncItemInfo2 in component.items)
                                    {
                                        if (component.GetItemByID(syncItemInfo2.id).itemCategory ==
                                            itemByID.itemCategory)
                                        {
                                            num++;
                                        }
                                    }

                                    if (num >= inventoryCategory.maxItems)
                                    {
                                        flag = true;
                                        break;
                                    }

                                    break;
                                }
                                else
                                {
                                    i++;
                                }
                            }

                            if (component.items.Count >= 8 || flag)
                            {
                                component.SetPickup(syncItemInfo.id, syncItemInfo.durability, __instance._pms.RealModelPosition, Quaternion.Euler(__instance._pms.Rotations.x, __instance._pms.Rotations.y, 0f), syncItemInfo.modSight, syncItemInfo.modBarrel, syncItemInfo.modOther);
                            }
                            else
                            {
                                component.AddNewItem(syncItemInfo.id, syncItemInfo.durability, syncItemInfo.modSight, syncItemInfo.modBarrel, syncItemInfo.modOther);
                            }
                        }
                        else
                        {
                            component.SetPickup(syncItemInfo.id, syncItemInfo.durability, __instance._pms.RealModelPosition, Quaternion.Euler(__instance._pms.Rotations.x, __instance._pms.Rotations.y, 0f), syncItemInfo.modSight, syncItemInfo.modBarrel, syncItemInfo.modOther);
                        }
                    }
                }

                ListPool<Inventory.SyncItemInfo>.Shared.Return(list);
                return false;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.ChangingRole: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
