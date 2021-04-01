// -----------------------------------------------------------------------
// <copyright file="IntercomSpeaking.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Sexiled.API.Features;
using Sexiled.Events.EventArgs;

namespace Sexiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;

    using Sexiled.Events.EventArgs;
    using Sexiled.Events.Handlers;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="Intercom.CallCmdSetTransmit(bool)"/>.
    /// Adds the <see cref="Handlers.Player.IntercomSpeaking"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Intercom), nameof(Intercom.CallCmdSetTransmit))]
    internal static class IntercomSpeaking
    {
        private static bool Prefix(Intercom __instance, bool player)
        {
            try
            {
                if (!__instance._interactRateLimit.CanExecute(true) || Intercom.AdminSpeaking)
                    return false;

                var ev = new IntercomSpeakingEventArgs(player ? API.Features.Player.Get(__instance.gameObject) : null);

                if (player)
                {
                    if (!__instance.ServerAllowToSpeak())
                        return false;

                    Handlers.Player.OnIntercomSpeaking(ev);

                    if (ev.IsAllowed)
                        Intercom.host.RequestTransmission(__instance.gameObject);
                }
                else
                {
                    if (!(Intercom.host.Networkspeaker == __instance.gameObject))
                        return false;

                    Handlers.Player.OnIntercomSpeaking(ev);

                    if (ev.IsAllowed)
                        Intercom.host.RequestTransmission(null);
                }

                return false;
            }
            catch (Exception e)
            {
                Log.Error($"Sexiled.Events.Patches.Events.Player.IntercomSpeaking: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
