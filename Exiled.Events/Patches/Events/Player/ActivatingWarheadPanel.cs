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
                if (!__instance._playerInteractRateLimit.CanExecute() ||
                    (__instance._hc.CufferId > 0 && !PlayerInteract.CanDisarmedInteract))
                    return false;

                GameObject gameObject = GameObject.Find("OutsitePanelScript");

                if (!__instance.ChckDis(gameObject.transform.position))
                    return false;

                Item itemById = __instance._inv.GetItemByID(__instance._inv.curItem);

                if (!__instance._sr.BypassMode && itemById == null)
                    return false;

                var list = ListPool<string>.Shared.Rent();
                list.Add("CONT_LVL_3");
                var ev = new ActivatingWarheadPanelEventArgs(API.Features.Player.Get(__instance.gameObject), list);

                Player.OnActivatingWarheadPanel(ev);

                if (ev.IsAllowed && itemById.permissions.Intersect(ev.Permissions).Any())
                {
                    gameObject.GetComponentInParent<AlphaWarheadOutsitePanel>().NetworkkeycardEntered = true;
                    __instance.OnInteract();
                }

                ListPool<string>.Shared.Return(list);
                return false;
            }
            catch (Exception exception)
            {
                API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.ActivatingWarheadPanel: {exception}\n{exception.StackTrace}");

                return true;
            }
        }
    }
}
