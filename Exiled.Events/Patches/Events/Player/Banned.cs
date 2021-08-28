// -----------------------------------------------------------------------
// <copyright file="Banned.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using Server = Exiled.API.Features.Server;

    /// <summary>
    /// Patches <see cref="BanHandler.IssueBan(BanDetails, BanHandler.BanType)"/>.
    /// Adds the <see cref="Player.Banned"/> event.
    /// </summary>
    [HarmonyPatch(typeof(BanHandler), nameof(BanHandler.IssueBan))]
    internal static class Banned
    {
        private static void Prefix(BanDetails ban, BanHandler.BanType banType, out API.Features.Player __state)
        {
            API.Features.Player issuerPlayer;
            if (ban.Issuer.Contains("("))
            {
                issuerPlayer = API.Features.Player.Get(ban.Issuer.Substring(ban.Issuer.LastIndexOf('(') + 1).TrimEnd(')')) ?? Server.Host;
            }
            else
            {
                issuerPlayer = Server.Host;
            }

            __state = issuerPlayer;
        }

        private static void Postfix(BanDetails ban, BanHandler.BanType banType, API.Features.Player __state)
        {
            BannedEventArgs ev = new BannedEventArgs(string.IsNullOrEmpty(ban.Id) ? null : API.Features.Player.Get(ban.Id), __state, ban, banType);

            Player.OnBanned(ev);
        }
    }
}
