// -----------------------------------------------------------------------
// <copyright file="ActivatingWarheadPanel.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;
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
    internal class ActivatingWarheadPanel
    {
        private static bool Prefix(PlayerInteract __instance)
        {
            try
            {
                if (!__instance._playerInteractRateLimit.CanExecute() ||
                    (__instance._hc.CufferId > 0 && !PlayerInteract.CanDisarmedInteract))
                    return false;

                GameObject gameObject = GameObject.Find("OutsitePanelScript");

                if (!__instance.ChckDis(gameObject.transform.position))
                    return false;

                Item itemById = __instance._inv.GetItemByID(__instance._inv.curItem);

                if (!__instance._sr.BypassMode &&
                    (itemById == null || !Enumerable.Contains(itemById.permissions, "CONT_LVL_3")))
                    return false;

                var ev = new ActivatingWarheadPanelEventArgs(API.Features.Player.Get(__instance.gameObject), new List<string> { "CONT_LVL_3" });

                Handlers.Player.OnActivatingWarheadPanel(ev);

                gameObject.GetComponentInParent<AlphaWarheadOutsitePanel>().NetworkkeycardEntered = true;
                __instance.OnInteract();

                return false;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.ActivatingWarheadPanel: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
