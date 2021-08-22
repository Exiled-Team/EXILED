// -----------------------------------------------------------------------
// <copyright file="IntercomSpeaking.cs" company="Exiled Team">
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

    /// <summary>
    /// Patches <see cref="Intercom.UserCode_CmdSetTransmit(bool)"/>.
    /// Adds the <see cref="Player.IntercomSpeaking"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Intercom), nameof(Intercom.UserCode_CmdSetTransmit))]
    internal static class IntercomSpeaking
    {
        private static bool Prefix(Intercom __instance, bool player)
        {
            try
            {
                if (!__instance._interactRateLimit.CanExecute(true) || Intercom.AdminSpeaking)
                    return false;

                IntercomSpeakingEventArgs ev = new IntercomSpeakingEventArgs(player ? API.Features.Player.Get(__instance.gameObject) : null);

                if (player)
                {
                    if (!__instance.ServerAllowToSpeak())
                        return false;

                    Player.OnIntercomSpeaking(ev);

                    if (ev.IsAllowed)
                        Intercom.host.RequestTransmission(__instance.gameObject);
                }
                else
                {
                    if (!(Intercom.host.Networkspeaker == __instance.gameObject))
                        return false;

                    Player.OnIntercomSpeaking(ev);

                    if (ev.IsAllowed)
                        Intercom.host.RequestTransmission(null);
                }

                return false;
            }
            catch (Exception e)
            {
                API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.IntercomSpeaking: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
