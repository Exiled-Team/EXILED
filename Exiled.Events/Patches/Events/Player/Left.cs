// -----------------------------------------------------------------------
// <copyright file="Left.cs" company="Exiled Team">
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
    /// Patches <see cref="ReferenceHub.OnDestroy"/>.
    /// Adds the <see cref="Player.Left"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.OnDestroy))]
    public class Left
    {
        /// <summary>
        /// Prefix of <see cref="ReferenceHub.OnDestroy"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="ReferenceHub"/> instance.</param>
        public static void Prefix(ReferenceHub __instance)
        {
            API.Features.Player player = API.Features.Player.Get(__instance.gameObject);

            if (player.IsHost || string.IsNullOrEmpty(player.UserId))
                return;

            var ev = new LeftEventArgs(player);

            API.Features.Log.Debug($"Player {ev.Player?.Nickname} ({ev.Player?.UserId}) disconnected");

            Player.OnLeft(ev);

            if (API.Features.Player.IdsCache.ContainsKey(__instance.queryProcessor.PlayerId))
                API.Features.Player.IdsCache.Remove(__instance.queryProcessor.PlayerId);

            if (API.Features.Player.UserIdsCache.ContainsKey(__instance.characterClassManager.UserId))
                API.Features.Player.UserIdsCache.Remove(__instance.characterClassManager.UserId);

            if (!API.Features.Player.Dictionary.ContainsKey(__instance.gameObject))
                API.Features.Player.Dictionary.Remove(__instance.gameObject);
        }
    }
}
