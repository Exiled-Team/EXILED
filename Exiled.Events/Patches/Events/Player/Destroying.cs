// -----------------------------------------------------------------------
// <copyright file="Destroying.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player {
    using System;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    [HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.OnDestroy))]
    internal static class Destroying {
        private static void Prefix(ReferenceHub __instance) {
            try {
                // Means it's the server
                if (!(Player.Get(__instance) is Player player))
                    return;

                Handlers.Player.OnDestroying(new DestroyingEventArgs(player));

                Player.Dictionary.Remove(player.GameObject);
                Player.UnverifiedPlayers.Remove(__instance);
                Player.IdsCache.Remove(player.Id);

                if (player.UserId != null)
                    Player.UserIdsCache.Remove(player.UserId);
            }
            catch (Exception exception) {
                Log.Error($"{typeof(Destroying).FullName}.{nameof(Prefix)}:\n{exception}");
            }
        }
    }
}
