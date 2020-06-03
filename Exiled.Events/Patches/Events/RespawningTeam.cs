// -----------------------------------------------------------------------
// <copyright file="RespawningTeam.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Patches
{
    #pragma warning disable SA1313
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using Exiled.Events;
    using Exiled.Events.Handlers.EventArgs;
    using HarmonyLib;

    /// <summary>
    /// Patch the <see cref="MTFRespawn.RespawnDeadPlayers"/>.
    /// Adds the <see cref="Exiled.Events.Handlers.Server.RespawningTeam"/> event.
    /// </summary>
    [HarmonyPatch(typeof(MTFRespawn), nameof(MTFRespawn.RespawnDeadPlayers))]
    public class RespawningTeam
    {
        /// <summary>
        /// Prefix of <see cref="PlayerInteract.CallCmdSwitchAWButton"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="PlayerInteract"/> instance.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(MTFRespawn __instance)
        {
            int num = 0;
            List<Player> deadPlayers = Player.List.Values.Where(player => player.IsDead || player.IsOverwatchEnabled).ToList();
            __instance.playersToNTF.Clear();

            Log.Debug($"Respawn: Got players: {deadPlayers.Count}", Exiled.Loader.PluginManager.ShouldDebugBeShown);

            if (Config.IsRespawnRandom)
                deadPlayers.ShuffleList();

            int maxRespawn = __instance.nextWaveIsCI ? __instance.maxCIRespawnAmount : __instance.maxMTFRespawnAmount;

            var ev = new RespawningTeamEventArgs(deadPlayers.Take(maxRespawn).ToList(), maxRespawn, __instance.nextWaveIsCI);

            Exiled.Events.Handlers.Server.OnRespawningTeam(ev);

            if (maxRespawn <= 0 || ev.Players == null || ev.Players.Count == 0)
                return false;

            foreach (Player player in ev.Players)
            {
                if (num >= maxRespawn)
                    break;

                if (player != null)
                {
                    ++num;
                    if (ev.IsChaos)
                    {
                        __instance.GetComponent<CharacterClassManager>().SetPlayersClass(
                            RoleType.ChaosInsurgency,
                            player.GameObject);

                        ServerLogs.AddLog(
                            ServerLogs.Modules.ClassChange,
                            player.Nickname + " (" + player.UserId + ") respawned as Chaos Insurgency agent.",
                            ServerLogs.ServerLogType.GameEvent);
                    }
                    else
                    {
                        __instance.playersToNTF.Add(player.GameObject);
                    }
                }
            }

            if (num > 0)
            {
                ServerLogs.AddLog(
                    ServerLogs.Modules.ClassChange,
                    (__instance.nextWaveIsCI ? "Chaos Insurgency" : "MTF") + " respawned!",
                    ServerLogs.ServerLogType.GameEvent);

                if (__instance.nextWaveIsCI)
                    __instance.Invoke("CmdDelayCIAnnounc", 1f);
            }

            __instance.SummonNTF();
            return false;
        }
    }
}