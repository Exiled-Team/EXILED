// -----------------------------------------------------------------------
// <copyright file="LosingSignal.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp079;
    using HarmonyLib;
    using Mirror;
    using PlayerRoles.PlayableScps.Scp079;

    /// <summary>
    /// Patches <see cref="Scp079LostSignalHandler.ServerLoseSignal"/>.
    /// Adds the <see cref="Handlers.Scp079.LosingSignal" /> event for SCP-079.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp079), nameof(Handlers.Scp079.LosingSignal))]
    [HarmonyPatch(typeof(Scp079LostSignalHandler), nameof(Scp079LostSignalHandler.ServerLoseSignal))]
    internal static class LosingSignal
    {
#pragma warning disable SA1313
        private static bool Prefix(Scp079LostSignalHandler __instance, float duration)
#pragma warning restore SA1313
        {
            if (!__instance.Role.TryGetOwner(out ReferenceHub hub))
                return true;

            LosingSignalEventArgs ev = new(hub, duration);
            Handlers.Scp079.OnLosingSignal(ev);

            if (ev.IsAllowed)
            {
                __instance._recoveryTime = NetworkTime.time + duration;
                __instance.ServerSendRpc(true);
            }

            return false;
        }
    }
}