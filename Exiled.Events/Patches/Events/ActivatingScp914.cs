// -----------------------------------------------------------------------
// <copyright file="ActivatingScp914.cs" company="Exiled Team">
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
    using Mirror;
    using Scp914;

    /// <summary>
    /// Patches <see cref="PlayerInteract.CallCmdUse914"/>.
    /// Adds the <see cref="Player.ActivatingScp914"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdUse914))]
    public class ActivatingScp914
    {
        /// <summary>
        /// Prefix of <see cref="PlayerInteract.CallCmdUse914"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="PlayerInteract"/> instance.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(PlayerInteract __instance)
        {
            if (!__instance._playerInteractRateLimit.CanExecute(true) ||
                    (__instance._hc.CufferId > 0 && !__instance.CanDisarmedInteract) ||
                    (Scp914Machine.singleton.working || !__instance.ChckDis(Scp914Machine.singleton.button.position)))
                return false;

            var ev = new ActivatingScp914EventArgs(API.Features.Player.Get(__instance.gameObject), 0);

            Player.OnActivatingScp914(ev);

            if (ev.IsAllowed)
            {
                Scp914Machine.singleton.RpcActivate(NetworkTime.time + ev.Duration);
                __instance.OnInteract();
            }

            return false;
        }
    }
}