// -----------------------------------------------------------------------
// <copyright file="UpdateScp244.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp244
{
#pragma warning disable SA1313
    using System;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items.Usables.Scp244;
    using InventorySystem.Searching;

    using Mirror;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="Scp244DeployablePickup.UpdateRange"/>.
    /// Adds the <see cref="Handlers.Scp244.OpeningScp244"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup.UpdateRange))]
    internal static class UpdateScp244
    {
        private static bool Prefix(Scp244DeployablePickup __instance)
        {
            try
            {
                if (__instance.ModelDestroyed && __instance._visibleModel.activeSelf)
                {
                    __instance.Rb.constraints = RigidbodyConstraints.FreezeAll;
                    __instance._visibleModel.SetActive(false);
                }

                if (!NetworkServer.active)
                {
                    __instance.CurrentSizePercent = __instance._syncSizePercent;
                    __instance.CurrentSizePercent /= 255f;
                    return false;
                }

                if (__instance.State == Scp244State.Idle)
                {
                    OpeningScp244EventArgs ev = new(__instance, Vector3.Dot(__instance.transform.up, Vector3.up) < __instance._activationDot);
                    Handlers.Scp244.OnOpeningScp244(ev);
                    if (ev.IsAllowed)
                    {
                        __instance.State = Scp244State.Active;
                        __instance._lifeTime.Restart();
                    }
                }

                float num = (__instance.State == Scp244State.Active) ? __instance.TimeToGrow : (-__instance._timeToDecay);
                __instance.CurrentSizePercent = Mathf.Clamp01(__instance.CurrentSizePercent + (Time.deltaTime / num));
                __instance.Network_syncSizePercent = (byte)Mathf.RoundToInt(__instance.CurrentSizePercent * 255f);
                if (!__instance.ModelDestroyed || __instance.CurrentSizePercent > 0f)
                {
                    return false;
                }

                __instance._timeToDecay -= Time.deltaTime;
                if (__instance._timeToDecay <= 0f)
                {
                    NetworkServer.Destroy(__instance.gameObject);
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"{typeof(UsingScp244).FullName}.{nameof(Prefix)}:\n{ex}");
                return true;
            }
        }
    }
}
