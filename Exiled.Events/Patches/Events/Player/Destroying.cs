// -----------------------------------------------------------------------
// <copyright file="Destroying.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using PlayerAPI = Exiled.API.Features.Player;
    using PlayerEvents = Exiled.Events.Handlers.Player;

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    [HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.OnDestroy))]
    internal static class Destroying
    {
        private static void Prefix(ReferenceHub __instance)
        {
            try
            {
                var player = PlayerAPI.Get(__instance);

                // Means it's the server
                if (player == null)
                    return;

                PlayerEvents.OnDestroying(new DestroyingEventArgs(player));

                PlayerAPI.Dictionary.Remove(player.GameObject);
                PlayerAPI.IdsCache.Remove(player.Id);

                if (player.UserId != null)
                    PlayerAPI.UserIdsCache.Remove(player.UserId);
            }
            catch (Exception ex)
            {
                Log.Error($"{typeof(Destroying).FullName}.{nameof(Prefix)}:\n{ex}");
            }
        }
    }
}
