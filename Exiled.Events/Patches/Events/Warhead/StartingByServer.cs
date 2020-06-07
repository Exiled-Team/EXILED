// -----------------------------------------------------------------------
// <copyright file="StartingByServer.cs" company="Exiled Team">
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

    /// <summary>
    /// Patch the <see cref="AlphaWarheadController.StartDetonation"/>.
    /// Adds the <see cref="Warhead.Starting"/> event.
    /// </summary>
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.StartDetonation))]
    public class StartingByServer
    {
        /// <summary>
        /// Prefix of <see cref="PlayerInteract.CallCmdDetonateWarhead"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="AlphaWarheadController"/> instance.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(AlphaWarheadController __instance)
        {
            if (Recontainer079.isLocked)
                return false;

            __instance.doorsOpen = false;

            ServerLogs.AddLog(ServerLogs.Modules.Warhead, "Countdown started.", ServerLogs.ServerLogType.GameEvent);

            if ((AlphaWarheadController._resumeScenario != -1 || __instance.scenarios_start[AlphaWarheadController._startScenario].SumTime() != (double)__instance.timeToDetonation) && (AlphaWarheadController._resumeScenario == -1 || __instance.scenarios_resume[AlphaWarheadController._resumeScenario].SumTime() != (double)__instance.timeToDetonation))
                return false;

            var ev = new StartingEventArgs(API.Features.Server.Host);

            Warhead.OnStarting(ev);

            if (!ev.IsAllowed)
                return false;

            __instance.NetworkinProgress = true;

            return false;
        }
    }
}
