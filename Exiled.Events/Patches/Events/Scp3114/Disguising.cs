// -----------------------------------------------------------------------
// <copyright file="Disguising.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp3114
{
    using Exiled.Events.EventArgs.Scp3114;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    using HarmonyLib;
    using Mirror;
    using PlayerRoles.PlayableScps.Scp3114;
    using PlayerRoles.PlayableScps.Subroutines;
    using PlayerRoles.Ragdolls;

    /// <summary>
    ///     Patches <see cref="Scp3114Disguise.ServerComplete()" /> setter.
    ///     Adds the <see cref="Handlers.Scp3114.Disguising" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp3114Role), nameof(Scp3114Disguise.ServerComplete))]
    internal class Disguising
    {
        private static bool Prefix(Scp3114Disguise __instance)
        {
            __instance.OnProgressSet();
            Scp3114Identity.StolenIdentity curIdentity = __instance.ScpRole.CurIdentity;
            if (__instance.IsInProgress)
            {
                DisguisingEventArgs ev = new(__instance.Owner);
                Handlers.Scp3114.OnDisguising(ev);
                if (!ev.IsAllowed)
                    return false;

                __instance._equipSkinSound.Play();
                curIdentity.Ragdoll = __instance.CurRagdoll;
                curIdentity.UnitNameId = (byte)(__instance._prevUnitIds.TryGetValue(__instance.CurRagdoll, out byte b) ? b : 0);
                curIdentity.Status = Scp3114Identity.DisguiseStatus.Equipping;
                DisguisedEventArgs ev = new(__instance.Owner);
                Handlers.Scp3114.OnDisguised(ev);
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
