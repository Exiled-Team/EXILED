// -----------------------------------------------------------------------
// <copyright file="Revealing.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp3114
{
    using Exiled.Events.Attributes;
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
#pragma warning disable SA1402 // File may only contain a single type
    using Exiled.Events.EventArgs.Scp3114;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using Mirror;

    using PlayerRoles.PlayableScps.Scp3114;

    using static PlayerRoles.PlayableScps.Scp3114.Scp3114Identity;

    /// <summary>
    ///     Patches <see cref="Scp3114Identity.Update" /> setter.
    ///     Adds the <see cref="Handlers.Scp3114.Revealed" /> and <see cref="Handlers.Scp3114.Revealing" /> event.
    /// </summary>
    [EventPatch(typeof(Scp3114), nameof(Scp3114.Revealed))]
    [EventPatch(typeof(Scp3114), nameof(Scp3114.Revealing))]
    [HarmonyPatch(typeof(Scp3114Identity), nameof(Scp3114Identity.Update))]
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

    /// <summary>
    ///     Patches <see cref="Scp3114Reveal.ServerProcessCmd" />.
    ///     Adds the <see cref="Handlers.Scp3114.Revealed" /> and <see cref="Handlers.Scp3114.Revealing" /> event.
    /// </summary>
    [EventPatch(typeof(Scp3114), nameof(Scp3114.Revealed))]
    [EventPatch(typeof(Scp3114), nameof(Scp3114.Revealing))]
    [HarmonyPatch(typeof(Scp3114Reveal), nameof(Scp3114Reveal.ServerProcessCmd))]
    internal class RevealingKey
    {
        private static bool Prefix(Scp3114Reveal __instance, NetworkReader reader)
        {
            RevealingEventArgs revealing = new(__instance.Owner);
            Handlers.Scp3114.OnRevealing(revealing);

            if (!revealing.IsAllowed)
            {
                return false;
            }

            __instance.ServerProcessCmd(reader);
            __instance.ScpRole.Disguised = false;

            RevealedEventArgs revealed = new(__instance.Owner);
            Handlers.Scp3114.OnRevealed(revealed);

            return false;
        }
    }
}
