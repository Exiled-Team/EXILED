// -----------------------------------------------------------------------
// <copyright file="StartingWarheadByServer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events
{
    #pragma warning disable SA1313
    using Exiled.Events.Handlers;
    using Exiled.Events.Handlers.EventArgs;
    using HarmonyLib;

    /// <summary>
    /// Patch the <see cref="AlphaWarheadController.StartDetonation"/>.
    /// Adds the <see cref="Map.StartingWarhead"/> event.
    /// </summary>
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.StartDetonation))]
    public class StartingWarheadByServer
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

            var ev = new StartingWarheadEventArgs(null);

            Map.OnStartingWarhead(ev);

            if (!ev.IsAllowed)
                return false;

            __instance.NetworkinProgress = true;

            return false;
        }
    }
}
