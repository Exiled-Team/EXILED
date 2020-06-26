// -----------------------------------------------------------------------
// <copyright file="ExplodingFlashGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
#pragma warning disable SA1313
    using System.Collections.Generic;
    using CustomPlayerEffects;
    using Exiled.Events.EventArgs;
    using Grenades;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="FlashGrenade.ServersideExplosion()"/>.
    /// Adds the <see cref="Handlers.Map.OnExplodingGrenade"/> event.
    /// </summary>
    [HarmonyPatch(typeof(FlashGrenade), nameof(FlashGrenade.ServersideExplosion))]
    public class ExplodingFlashGrenade
    {
        /// <summary>
        /// Prefix of <see cref="FlashGrenade.ServersideExplosion()"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="FlashGrenade"/> instance.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(FlashGrenade __instance)
        {
            List<Exiled.API.Features.Player> players = new List<Exiled.API.Features.Player>();
            foreach (GameObject gameObject in PlayerManager.players)
            {
                Vector3 position = __instance.transform.position;
                ReferenceHub hub = ReferenceHub.GetHub(gameObject);
                Flashed effect = hub.playerEffectsController.GetEffect<Flashed>();
                Deafened effect2 = hub.playerEffectsController.GetEffect<Deafened>();
                if (effect == null || __instance.thrower == null || (!__instance.Network_friendlyFlash && !effect.Flashable(ReferenceHub.GetHub(__instance.thrower.gameObject), position, __instance.viewLayerMask)))
                    continue;

                float num = __instance.powerOverDistance.Evaluate(Vector3.Distance(gameObject.transform.position, position) / ((position.y > 900f) ? __instance.distanceMultiplierSurface : __instance.distanceMultiplierFacility)) * __instance.powerOverDot.Evaluate(Vector3.Dot(hub.PlayerCameraReference.forward, (hub.PlayerCameraReference.position - position).normalized));
                byte b = (byte)Mathf.Clamp(Mathf.RoundToInt(num * 10f * __instance.maximumDuration), 1, 255);
                if (b >= effect.Intensity && num > 0f)
                {
                    players.Add(Exiled.API.Features.Player.Get(gameObject));
                }
            }

            ExplodingGrenadeEventArgs ev = new ExplodingGrenadeEventArgs(players, false, __instance.gameObject);
            Exiled.Events.Handlers.Map.OnExplodingGrenade(ev);
            return ev.IsAllowed;
        }
    }
}
