// -----------------------------------------------------------------------
// <copyright file="Enraging.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp096
{
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using PlayableScps;

    /// <summary>
    /// Patches <see cref="Scp096.Enrage"/>.
    /// Adds the <see cref="Handlers.Scp096.Enraging"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp096), nameof(Scp096.Enrage))]
    internal class Enraging
    {
        private static bool Prefix(Scp096 __instance)
        {
            var ev = new EnragingEventArgs(__instance, API.Features.Player.Get(__instance.Hub.gameObject));

            Handlers.Scp096.OnEnraging(ev);

            if (!ev.IsAllowed)
                return false;

            if (__instance.Enraged)
            {
                __instance.AddReset();
            }
            else
            {
                __instance.SetMovementSpeed(12f);
                __instance.SetJumpHeight(10f);
                __instance.PlayerState = Scp096PlayerState.Enraged;
                __instance.EnrageTimeLeft = __instance.EnrageTime;
            }

            return false;
        }
    }
}
