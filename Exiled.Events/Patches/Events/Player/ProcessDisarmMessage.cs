// -----------------------------------------------------------------------
// <copyright file="ProcessDisarmMessage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
#pragma warning disable SA1600 // Elements should be documented
    using System;

    using Achievements;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using global::Utils.Networking;

    using HarmonyLib;

    using InventorySystem.Disarming;
    using InventorySystem.Items;

    using Mirror;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="DisarmingHandlers.ServerProcessDisarmMessage"/>.
    /// Adds the <see cref="Player.Handcuffing"/> and <see cref="Player.RemovingHandcuffs"/> events.
    /// </summary>
    [HarmonyPatch(typeof(DisarmingHandlers), nameof(DisarmingHandlers.ServerProcessDisarmMessage))]
    internal static class ProcessDisarmMessage
    {
        public static bool Prefix(NetworkConnection conn, DisarmMessage msg)
        {
            try
            {
                if (!ReferenceHub.TryGetHub(conn.identity.gameObject, out ReferenceHub hub))
                {
                    return false;
                }

                if (!msg.PlayerIsNull)
                {
                    Vector3 vector3 = msg.PlayerToDisarm.transform.position - hub.transform.position;
                    if (vector3.sqrMagnitude > 20.0 || (msg.PlayerToDisarm.inventory.CurInstance != null && msg.PlayerToDisarm.inventory.CurInstance.TierFlags != ItemTierFlags.Common))
                    {
                        return false;
                    }
                }

                bool flag1 = !msg.PlayerIsNull && msg.PlayerToDisarm.inventory.IsDisarmed();
                bool flag2 = !msg.PlayerIsNull && hub.CanDisarm(msg.PlayerToDisarm);

                if (flag1 && !msg.Disarm)
                {
                    if (!hub.inventory.IsDisarmed())
                    {
                        var ev = new RemovingHandcuffsEventArgs(API.Features.Player.Get(hub), API.Features.Player.Get(msg.PlayerToDisarm));

                        Player.OnRemovingHandcuffs(ev);

                        if (!ev.IsAllowed)
                        {
                            return false;
                        }

                        msg.PlayerToDisarm.inventory.SetDisarmedStatus(null);
                    }
                }
                else if (!flag1 & flag2 && msg.Disarm)
                {
                    if (msg.PlayerToDisarm.inventory.CurInstance == null || msg.PlayerToDisarm.inventory.CurInstance.CanHolster())
                    {
                        if (msg.PlayerToDisarm.characterClassManager.CurRole.team == Team.MTF && hub.characterClassManager.CurClass == RoleType.ClassD)
                        {
                            AchievementHandlerBase.ServerAchieve(hub.networkIdentity.connectionToClient, AchievementName.TablesHaveTurned);
                        }

                        var ev = new HandcuffingEventArgs(API.Features.Player.Get(hub), API.Features.Player.Get(msg.PlayerToDisarm));

                        Player.OnHandcuffing(ev);

                        if (!ev.IsAllowed)
                        {
                            return false;
                        }

                        msg.PlayerToDisarm.inventory.SetDisarmedStatus(hub.inventory);
                    }
                }
                else
                {
                    hub.networkIdentity.connectionToClient.Send<DisarmedPlayersListMessage>(DisarmingHandlers.NewDisarmedList, 0);
                    return false;
                }

                DisarmingHandlers.NewDisarmedList.SendToAuthenticated<DisarmedPlayersListMessage>();

                return false;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.RemovingHandcuffs: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
