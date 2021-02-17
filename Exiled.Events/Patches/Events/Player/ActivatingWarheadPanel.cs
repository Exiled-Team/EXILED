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
    using System.Linq;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    /// <summary>
    /// Patch the <see cref="PlayerInteract.CallCmdSwitchAWButton"/>.
    /// Adds the <see cref="Player.ActivatingWarheadPanel"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdSwitchAWButton))]
    internal static class ActivatingWarheadPanel
    {
        private static bool Prefix(PlayerInteract __instance)
        {
            try
            {
                if (!__instance._playerInteractRateLimit.CanExecute()
                    || (__instance._hc.CufferId > 0 && !PlayerInteract.CanDisarmedInteract))
                {
                    return false;
                }

                GameObject gameObject = GameObject.Find("OutsitePanelScript");
                if (!__instance.ChckDis(gameObject.transform.position))
                    return false;

                var itemById = __instance._inv.GetItemByID(__instance._inv.curItem);
                if (!__instance._sr.BypassMode && itemById == null)
                    return false;

                const string PANEL_PERM = "CONT_LVL_3";

                // Deprecated
                var list = ListPool<string>.Shared.Rent();
                list.Add(PANEL_PERM);

                var ev = new ActivatingWarheadPanelEventArgs(
                    API.Features.Player.Get(__instance.gameObject),
                    list,
                    __instance._sr.BypassMode || itemById.permissions.Contains(PANEL_PERM));

                Player.OnActivatingWarheadPanel(ev);
                ListPool<string>.Shared.Return(list);

                if (ev.IsAllowed)
                {
                    gameObject.GetComponentInParent<AlphaWarheadOutsitePanel>().NetworkkeycardEntered = true;
                    __instance.OnInteract();
                }

                return false;
            }
            catch (Exception exception)
            {
                API.Features.Log.Error($"{typeof(ActivatingWarheadPanel).FullName}.{nameof(Prefix)}:\n{exception}");

                return true;
            }
        }
    }
}
