// -----------------------------------------------------------------------
// <copyright file="TryUseBody.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp3114
{
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp3114;
    using Exiled.Events.Handlers;
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp3114;

    /// <summary>
    ///     Patches <see cref="Scp3114Disguise.OnProgressSet()" />.
    ///     Adds the <see cref="Handlers.Scp3114.TryUseBody" /> event.
    /// </summary>
    [EventPatch(typeof(Scp3114), nameof(Scp3114.TryUseBody))]
    [HarmonyPatch(typeof(Scp3114Disguise), nameof(Scp3114Disguise.OnProgressSet))]
    internal class TryUseBody
    {
        private static bool Prefix(Scp3114Disguise __instance)
        {
            Scp3114Identity.StolenIdentity curIdentity = __instance.CastRole.CurIdentity;
            if (__instance.IsInProgress)
            {
                TryUseBodyEventArgs ev = new(__instance.Owner, __instance.CurRagdoll, true);
                Handlers.Scp3114.OnTryUseBody(ev);

                if (!ev.IsAllowed)
                    return false;

                __instance._equipSkinSound.Play();
                curIdentity.Ragdoll = __instance.CurRagdoll;
                curIdentity.UnitNameId = __instance._prevUnitIds.TryGetValue(__instance.CurRagdoll, out byte b) ? b : byte.MinValue;
                curIdentity.Status = Scp3114Identity.DisguiseStatus.Equipping;
                return false;
            }

            if (curIdentity.Status == Scp3114Identity.DisguiseStatus.Equipping)
            {
                __instance._equipSkinSound.Stop();
                curIdentity.Status = Scp3114Identity.DisguiseStatus.None;
                __instance.Cooldown.Trigger((double)__instance.Duration);
            }

            return false;
        }
    }
}
