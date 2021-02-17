// -----------------------------------------------------------------------
// <copyright file="DeactivatingWorkstation.cs" company="Exiled Team">
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

    using HarmonyLib;

    using UnityEngine;

    /// <summary>
    /// Patch the <see cref="WorkStation.UnconnectTablet(UnityEngine.GameObject)"/>.
    /// Adds the <see cref="Player.ActivatingWorkstation"/> event.
    /// </summary>
    [HarmonyPatch(typeof(WorkStation), nameof(WorkStation.UnconnectTablet))]
    internal static class DeactivatingWorkstation
    {
        private static bool Prefix(WorkStation __instance, GameObject taker)
        {
            try
            {
                if (!__instance.CanTake(taker) || __instance._animationCooldown > 0f)
                {
                    return false;
                }

                DeactivatingWorkstationEventArgs ev = new DeactivatingWorkstationEventArgs(Exiled.API.Features.Player.Get(taker), __instance);
                Player.OnDeactivatingWorkstation(ev);

                if (ev.IsAllowed)
                {
                    taker.GetComponent<Inventory>().AddNewItem(ItemType.WeaponManagerTablet, -4.65664672E+11f, 0, 0, 0);
                    __instance.Network_playerConnected = null;
                    __instance.NetworkisTabletConnected = false;
                    __instance._animationCooldown = 3.5f;
                }

                return false;
            }
            catch (Exception exception)
            {
                API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.DeactivatingWorkstation: {exception}\n{exception.StackTrace}");
                return true;
            }
        }
    }
}
