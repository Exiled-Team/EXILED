// -----------------------------------------------------------------------
// <copyright file="Kicking.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;

    using API.Features;

    using CommandSystem;

    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;
    using PluginAPI.Events;

    using Log = API.Features.Log;

    /// <summary>
    ///     Patches <see cref="BanPlayer.KickUser(ReferenceHub, ICommandSender , string)" />.
    ///     Adds the <see cref="Handlers.Player.Kicking" /> event.
    /// </summary>
    [HarmonyPatch(typeof(BanPlayer), nameof(BanPlayer.KickUser), typeof(ReferenceHub), typeof(ICommandSender), typeof(string))]
    internal static class Kicking
    {
        private static bool Prefix(ReferenceHub target, ICommandSender issuer, string reason, ref bool __result)
        {
            try
            {
                string message = $"You have been kicked. {(!string.IsNullOrEmpty(reason) ? "Reason: " + reason : string.Empty)}";

                KickingEventArgs ev = new(Player.Get(target), Player.Get(issuer), reason, message);

                Handlers.Player.OnKicking(ev);

                if (!ev.IsAllowed)
                {
                    __result = false;
                    return false;
                }

                reason = ev.Reason;
                message = ev.FullMessage;

                if (!EventManager.ExecuteEvent(new PlayerKickedEvent(target, issuer, reason)))
                {
                    __result = false;
                    return false;
                }

                ServerConsole.Disconnect(target.gameObject, message);

                __result = true;
                return false;
            }
            catch (Exception exception)
            {
                Log.Error($"Exiled.Events.Patches.Events.Player.Kicking: {exception}\n{exception.StackTrace}");
                return true;
            }
        }
    }
}