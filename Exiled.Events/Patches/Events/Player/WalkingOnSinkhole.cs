// -----------------------------------------------------------------------
// <copyright file="WalkingOnSinkhole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
#pragma warning disable SA1118
#pragma warning disable SA1600

    using System;

    using CustomPlayerEffects;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using Mirror;

    [HarmonyPatch(typeof(SinkholeEnvironmentalHazard), nameof(SinkholeEnvironmentalHazard.DistanceChanged))]
    internal static class WalkingOnSinkhole
    {
        private static bool Prefix(SinkholeEnvironmentalHazard __instance, ReferenceHub player)
        {
            try
            {
                if (!NetworkServer.active)
                    return false;
                if (player.playerEffectsController == null)
                    return false;

                var effectsController = player.playerEffectsController;
                if ((player.playerMovementSync.RealModelPosition - __instance.transform.position).sqrMagnitude <= __instance.DistanceToBeAffected * __instance.DistanceToBeAffected)
                {
                    var ev = new WalkingOnSinkholeEventArgs(Player.Get(player), __instance);
                    Handlers.Player.OnWalkingOnSinkhole(ev);

                    if (!ev.IsAllowed)
                        return false;

                    if (__instance.SCPImmune)
                    {
                        var characterClassManager = player.characterClassManager;
                        if (characterClassManager == null || characterClassManager.IsAnyScp())
                        {
                            return false;
                        }
                    }

                    effectsController.EnableEffect<SinkHole>();
                    return false;
                }

                effectsController.DisableEffect<SinkHole>();
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"{typeof(WalkingOnSinkhole).FullName}.{nameof(Prefix)}:\n{e}");
                return true;
            }
        }
    }
}
