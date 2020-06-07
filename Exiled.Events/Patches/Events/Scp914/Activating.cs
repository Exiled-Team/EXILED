// -----------------------------------------------------------------------
// <copyright file="Activating.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp914
{
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using global::Scp914;
    using HarmonyLib;
    using Mirror;

    /// <summary>
    /// Patches <see cref="PlayerInteract.CallCmdUse914"/>.
    /// Adds the <see cref="Scp914.Activating"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdUse914))]
    public class Activating
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

            var ev = new ActivatingEventArgs(API.Features.Player.Get(__instance.gameObject), 0);

            Scp914.OnActivating(ev);

            if (ev.IsAllowed)
            {
                Scp914Machine.singleton.RpcActivate(NetworkTime.time + ev.Duration);
                __instance.OnInteract();
            }

            return false;
        }
    }
}
