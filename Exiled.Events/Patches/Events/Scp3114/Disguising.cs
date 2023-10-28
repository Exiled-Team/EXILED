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
    [HarmonyPatch(typeof(Scp3114Disguise), nameof(Scp3114Disguise.ServerComplete))]
    internal class Disguising
    {
        private static bool Prefix(Scp3114Disguise __instance)
        {
            if (__instance.CurRagdoll != null && __instance.CurRagdoll is DynamicRagdoll ragdoll)
            {
                DisguisingEventArgs disguising = new(__instance.Owner, ragdoll);
                Handlers.Scp3114.OnDisguising(disguising);

                if (!disguising.IsAllowed)
                    return false;

                __instance.ScpRole.Disguised = true;
                Scp3114RagdollToBonesConverter.ServerConvertNew(__instance.ScpRole, ragdoll);

                DisguisedEventArgs disguised = new(__instance.Owner, ragdoll);
                Handlers.Scp3114.OnDisguised(disguised);
            }

            return false;
        }
    }
}
