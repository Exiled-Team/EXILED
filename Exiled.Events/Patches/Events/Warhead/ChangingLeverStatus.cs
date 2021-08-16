// -----------------------------------------------------------------------
// <copyright file="ChangingLeverStatus.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Warhead
{
#pragma warning disable SA1118
#pragma warning disable SA1313

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="PlayerInteract.UserCode_CmdSwitchAWButton"/>.
    /// Adds the <see cref="Handlers.Warhead.ChangingLeverStatus"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.UserCode_CmdUsePanel))]
    internal static class ChangingLeverStatus
    {
        private static bool Prefix(PlayerInteract __instance, PlayerInteract.AlphaPanelOperations n)
        {
            if (!__instance.CanInteract)
                return false;
            ReferenceHub component = __instance._hub;
            AlphaWarheadNukesitePanel nukeside = AlphaWarheadOutsitePanel.nukeside;
            if (!__instance.ChckDis(nukeside.transform.position))
                return false;
            switch (n)
            {
                case PlayerInteract.AlphaPanelOperations.Cancel:
                    __instance.OnInteract();
                    AlphaWarheadController.Host.CancelDetonation(__instance.gameObject);
                    ServerLogs.AddLog(ServerLogs.Modules.Warhead, component.LoggedNameFromRefHub() + " cancelled the Alpha Warhead detonation.", ServerLogs.ServerLogType.GameEvent);
                    break;
                case PlayerInteract.AlphaPanelOperations.Lever:
                    __instance.OnInteract();
                    if (!nukeside.AllowChangeLevelState())
                        break;
                    var ev = new ChangingLeverStatusEventArgs(Player.Get(component), nukeside.Networkenabled, true);
                    Handlers.Warhead.OnChangingLeverStatus(ev);
                    if (!ev.IsAllowed)
                        return false;
                    nukeside.Networkenabled = !nukeside.enabled;
                    __instance.RpcLeverSound();
                    ServerLogs.AddLog(ServerLogs.Modules.Warhead, component.LoggedNameFromRefHub() + " set the Alpha Warhead status to " + nukeside.enabled.ToString() + ".", ServerLogs.ServerLogType.GameEvent);
                    break;
            }

            return false;
        }
    }
}
