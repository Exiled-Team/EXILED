// -----------------------------------------------------------------------
// <copyright file="Joined.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
#pragma warning disable SA1600 // Elements should be documented
    using System;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using Exiled.Loader.Features;

    using HarmonyLib;

    using MEC;

    using PlayerAPI = Exiled.API.Features.Player;
    using PlayerEvents = Exiled.Events.Handlers.Player;

    /// <summary>
    /// Patches <see cref="ReferenceHub.Awake"/>.
    /// Adds the <see cref="PlayerEvents.Joined"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.Awake))]
    internal static class Joined
    {
        internal static void CallEvent(ReferenceHub hub, out Player player)
        {
            player = new PlayerAPI(hub);

            var p = player;
            Timing.CallDelayed(0.25f, () =>
            {
                if (p.IsMuted)
                    p.ReferenceHub.characterClassManager.SetDirtyBit(2UL);
            });

            PlayerEvents.OnJoined(new JoinedEventArgs(player));
        }

        private static void Postfix(ReferenceHub __instance)
        {
            try
            {
                // ReferenceHub is a component that is loaded first
                if (__instance.isDedicatedServer || ReferenceHub.HostHub == null || PlayerManager.localPlayer == null)
                    return;

                if (PlayerManager.players.Count >= CustomNetworkManager.slots)
                    MultiAdminFeatures.CallEvent(MultiAdminFeatures.EventType.SERVER_FULL);

                CallEvent(__instance, out _);
            }
            catch (Exception exception)
            {
                Log.Error($"{typeof(Joined).FullName}:\n{exception}");
            }
        }
    }
}
