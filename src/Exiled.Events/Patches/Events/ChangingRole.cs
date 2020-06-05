// -----------------------------------------------------------------------
// <copyright file="ChangingRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Patches
{
    #pragma warning disable SA1313
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.Events.Handlers;
    using Exiled.Events.Handlers.EventArgs;
    using HarmonyLib;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="CharacterClassManager.SetPlayersClass(RoleType, GameObject, bool, bool)"/>.
    /// Adds the <see cref="Player.ChangingRole"/> and <see cref="Player.Escaping"/> events.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.SetPlayersClass))]
    public class ChangingRole
    {
        /// <summary>
        /// Prefix of <see cref="CharacterClassManager.SetPlayersClass(RoleType, GameObject, bool, bool)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="CharacterClassManager"/> instance.</param>
        /// <param name="classid"><inheritdoc cref="ChangingRoleEventArgs.NewRole"/></param>
        /// <param name="ply">The player's <see cref="GameObject"/>.</param>
        /// <param name="lite"><inheritdoc cref="ChangingRoleEventArgs.ShouldPreservePosition"/></param>
        /// <param name="escape"><inheritdoc cref="ChangingRoleEventArgs.IsEscaped"/></param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(CharacterClassManager __instance, RoleType classid, GameObject ply, bool lite = false, bool escape = false)
        {
            if (!NetworkServer.active)
            {
                return false;
            }

            if (!ply.GetComponent<CharacterClassManager>().IsVerified)
            {
                return false;
            }

            var changingRoleEventArgs = new ChangingRoleEventArgs(API.Features.Player.Get(ply), classid, __instance.Classes.SafeGet(classid).startItems.ToList(), lite, escape);

            Player.OnChangingRole(changingRoleEventArgs);

            if (lite)
            {
                var escapingEventArgs = new EscapingEventArgs(API.Features.Player.Get(ply), classid);

                Player.OnEscaping(escapingEventArgs);

                classid = escapingEventArgs.NewRole;
            }

            ply.GetComponent<CharacterClassManager>().SetClassIDAdv(classid, lite, escape);
            ply.GetComponent<PlayerStats>().SetHPAmount(__instance.Classes.SafeGet(classid).maxHP);

            if (lite)
            {
                return false;
            }

            Inventory component = ply.GetComponent<Inventory>();
            List<Inventory.SyncItemInfo> list = new List<Inventory.SyncItemInfo>();
            if (escape && __instance.KeepItemsAfterEscaping)
            {
                foreach (Inventory.SyncItemInfo item in component.items)
                {
                    list.Add(item);
                }
            }

            component.items.Clear();
            foreach (ItemType id in changingRoleEventArgs.Items)
            {
                component.AddNewItem(id, -4.65664672E+11f, 0, 0, 0);
            }

            if (escape && __instance.KeepItemsAfterEscaping)
            {
                foreach (Inventory.SyncItemInfo syncItemInfo in list)
                {
                    if (__instance.PutItemsInInvAfterEscaping)
                    {
                        Item itemByID = component.GetItemByID(syncItemInfo.id);
                        bool flag = false;
                        InventoryCategory[] categories = __instance._search.categories;
                        int i = 0;
                        while (i < categories.Length)
                        {
                            InventoryCategory inventoryCategory = categories[i];
                            if (inventoryCategory.itemType == itemByID.itemCategory && (itemByID.itemCategory != ItemCategory.None || itemByID.itemCategory != ItemCategory.None))
                            {
                                int num = 0;
                                foreach (Inventory.SyncItemInfo syncItemInfo2 in component.items)
                                {
                                    if (component.GetItemByID(syncItemInfo2.id).itemCategory == itemByID.itemCategory)
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

            return false;
        }
    }
}
