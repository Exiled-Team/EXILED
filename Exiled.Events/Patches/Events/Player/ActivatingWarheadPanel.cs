// -----------------------------------------------------------------------
// <copyright file="ActivatingWarheadPanel.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    #pragma warning disable SA1313
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Patch the <see cref="PlayerInteract.CallCmdSwitchAWButton"/>.
    /// Adds the <see cref="Player.ActivatingWarheadPanel"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdSwitchAWButton))]
    public class ActivatingWarheadPanel
    {
        /// <summary>
        /// Prefix of <see cref="PlayerInteract.CallCmdSwitchAWButton"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="PlayerInteract"/> instance.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(PlayerInteract __instance)
        {
            if (!__instance._playerInteractRateLimit.CanExecute(true) || (__instance._hc.CufferId > 0 && !__instance.CanDisarmedInteract))
                return false;

            GameObject gameObject = GameObject.Find("OutsitePanelScript");

            if (!__instance.ChckDis(gameObject.transform.position))
                return false;

            var ev = new ActivatingWarheadPanelEventArgs(API.Features.Player.Get(__instance.gameObject), new List<string>() { "CONT_LVL_3" });

            Player.OnActivatingWarheadPanel(ev);

            if (ev.IsAllowed && __instance._inv.GetItemByID(__instance._inv.curItem).permissions.Intersect(ev.Permissions).Any())
            {
                gameObject.GetComponentInParent<AlphaWarheadOutsitePanel>().NetworkkeycardEntered = true;
                __instance.OnInteract();
            }

            return false;
        }
    }
}
