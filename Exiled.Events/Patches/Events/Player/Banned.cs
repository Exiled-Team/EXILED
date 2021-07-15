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

    /// <summary>
    /// Patches <see cref="BanHandler.IssueBan(BanDetails, BanHandler.BanType)"/>.
    /// Adds the <see cref="Player.Banned"/> event.
    /// </summary>
    [HarmonyPatch(typeof(BanHandler), nameof(BanHandler.IssueBan))]
    internal static class Banned
    {
        private static void Prefix(BanDetails ban, BanHandler.BanType banType, out string __state)
        {
            __state = ban.Issuer;
        }

        private static void Postfix(BanDetails ban, BanHandler.BanType banType, string __state)
        {
            var ev = new BannedEventArgs(string.IsNullOrEmpty(ban.Id) ? null : API.Features.Player.Get(ban.Id), API.Features.Player.Get(__state), ban, banType);

            Player.OnBanned(ev);
        }
    }
}
