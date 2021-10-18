// -----------------------------------------------------------------------
// <copyright file="Hurting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using global::Utils.Networking;

    using HarmonyLib;

    using InventorySystem.Disarming;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="PlayerStats.HurtPlayer(PlayerStats.HitInfo, GameObject, bool, bool)"/>.
    /// Adds the <see cref="Player.Hurting"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.HurtPlayer))]
    internal static class Hurting
    {
        private static bool Prefix(PlayerStats __instance, ref PlayerStats.HitInfo info, GameObject go)
        {
            try
            {
                if (go == null)
                    return true;

                API.Features.Player attacker = API.Features.Player.Get(info.IsPlayer ? info.RHub.gameObject : __instance.gameObject);
                API.Features.Player target = API.Features.Player.Get(go);

                if (target == null || target.IsHost || target.Role == RoleType.Spectator)
                    return true;

                if (info.Tool.Equals(DamageTypes.Recontainment) && target.Role == RoleType.Scp079)
                {
                    Scp079.OnRecontained(new RecontainedEventArgs(target));
                    DiedEventArgs eventArgs = new DiedEventArgs(null, target, info);
                    Player.OnDied(eventArgs);
                }

                if (attacker == null || attacker.IsHost)
                    return true;

                HurtingEventArgs ev = new HurtingEventArgs(attacker, target, info);

                if (ev.Target.IsHost)
                    return true;

                Player.OnHurting(ev);

                info = ev.HitInformation;

                if (!ev.IsAllowed)
                    return false;

                if (!ev.Target.IsGodModeEnabled && (ev.Amount == -1 || ev.Amount >= ev.Target.Health + ev.Target.ArtificialHealth))
                {
                    DyingEventArgs dyingEventArgs = new DyingEventArgs(ev.Attacker, ev.Target, ev.HitInformation);

                    Player.OnDying(dyingEventArgs);

                    if (!dyingEventArgs.IsAllowed)
                        return false;

                    dyingEventArgs.Target.Inventory.SetDisarmedStatus(null);
                    new DisarmedPlayersListMessage(DisarmedPlayers.Entries).SendToAuthenticated();

                    if (dyingEventArgs.ItemsToDrop != null)
                    {
                        dyingEventArgs.Target.ResetInventory(dyingEventArgs.ItemsToDrop);
                        dyingEventArgs.Target.DropItems();
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.Hurting: {e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}
