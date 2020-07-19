// -----------------------------------------------------------------------
// <copyright file="Starting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Warhead
{
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using UnityEngine;

    /// <summary>
    /// Patch the <see cref="PlayerInteract.CallCmdDetonateWarhead"/>.
    /// Adds the <see cref="Warhead.Starting"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdDetonateWarhead))]
    internal static class Starting
    {
        private static bool Prefix(PlayerInteract __instance)
        {
            if (!__instance._playerInteractRateLimit.CanExecute(true) || (__instance._hc.CufferId > 0 && !PlayerInteract.CanDisarmedInteract))
                return false;

            GameObject gameObject = GameObject.Find("OutsitePanelScript");

            if (!__instance.ChckDis(gameObject.transform.position) || !AlphaWarheadOutsitePanel.nukeside.enabled)
                return false;

            if (!gameObject.GetComponent<AlphaWarheadOutsitePanel>().keycardEntered || Recontainer079.isLocked)
                return false;

            AlphaWarheadController.Host.doorsOpen = false;

            ServerLogs.AddLog(ServerLogs.Modules.Warhead, "Countdown started.", ServerLogs.ServerLogType.GameEvent);
            if ((AlphaWarheadController._resumeScenario == -1 && AlphaWarheadController.Host.scenarios_start[AlphaWarheadController._startScenario].SumTime() == AlphaWarheadController.Host.timeToDetonation) ||
                (AlphaWarheadController._resumeScenario != -1 && AlphaWarheadController.Host.scenarios_resume[AlphaWarheadController._resumeScenario].SumTime() == AlphaWarheadController.Host.timeToDetonation))
            {
                var ev = new StartingEventArgs(API.Features.Player.Get(__instance.gameObject) ?? API.Features.Server.Host);

                Warhead.OnStarting(ev);

                if (!ev.IsAllowed)
                    return false;

                AlphaWarheadController.Host.NetworkinProgress = true;
            }

            ServerLogs.AddLog(ServerLogs.Modules.Warhead, __instance.GetComponent<NicknameSync>().MyNick + " (" + __instance.GetComponent<CharacterClassManager>().UserId + ") started the Alpha Warhead detonation.", ServerLogs.ServerLogType.GameEvent);

            __instance.OnInteract();

            return false;
        }
    }
}
