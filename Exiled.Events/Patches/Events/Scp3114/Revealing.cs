// -----------------------------------------------------------------------
// <copyright file="Revealing.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp3114
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Scp244;
    using Exiled.Events.EventArgs.Scp3114;

    using HarmonyLib;

    using Mirror;

    using PlayerRoles.PlayableScps.Scp3114;
    using PlayerRoles.PlayableScps.Subroutines;
    using PlayerRoles.Ragdolls;

    using static HarmonyLib.AccessTools;
    using static PlayerRoles.PlayableScps.Scp3114.Scp3114Identity;

    /// <summary>
    ///     Patches <see cref="Scp3114Identity.Update" /> setter.
    ///     Adds the <see cref="Handlers.Scp3114.Revealed" /> and <see cref="Handlers.Scp3114.Revealing" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp3114Identity), nameof(Scp3114Identity.Update), MethodType.Setter)]
    internal class Revealing
    {
        private static bool Prefix(Scp3114Identity __instance)
        {
            __instance.UpdateWarningAudio();
            if (NetworkServer.active && __instance.CurIdentity.Status == DisguiseStatus.Active && __instance.RemainingDuration.IsReady)
            {
                RevealingEventArgs revealing = new(__instance.Owner);
                Handlers.Scp3114.OnRevealing(revealing);
                if (!revealing.IsAllowed)
                {
                    __instance.RemainingDuration.Trigger(__instance._disguiseDurationSeconds);
                    __instance.ServerResendIdentity();
                    return false;
                }

                __instance.CurIdentity.Status = DisguiseStatus.None;
                __instance.ServerResendIdentity();

                RevealedEventArgs revealed = new(__instance.Owner);
                Handlers.Scp3114.OnRevealed(revealed);
            }

            return false;
        }
    }
}
