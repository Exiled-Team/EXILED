// -----------------------------------------------------------------------
// <copyright file="PlacingAmnesticCloud.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp939
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using Exiled.Events.EventArgs.Scp939;
    using Exiled.Events.Handlers;

    using HarmonyLib;
    using Mirror;

    using PlayerRoles.PlayableScps.Scp939;

    /// <summary>
    ///     Patches <see cref="Scp939AmnesticCloudAbility.ServerProcessCmd(NetworkReader)" />
    ///     to add the <see cref="Scp939.PlacingAmnesticCloud" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp939AmnesticCloudAbility), nameof(Scp939AmnesticCloudAbility.ServerProcessCmd))]
    internal static class PlacingAmnesticCloud
    {
        private static bool Prefix(Scp939AmnesticCloudAbility __instance, NetworkReader reader)
        {
            bool flag = reader.ReadBoolean();
            bool toAll = flag != __instance.TargetState;

            PlacingAmnesticCloudEventArgs ev = new(__instance.Owner, flag, __instance.Cooldown.IsReady, __instance._failedCooldown);
            Scp939.OnPlacingAmnesticCloud(ev);

            if (!ev.IsAllowed)
            {
                __instance.TargetState = false;
                return false;
            }

            if (flag)
            {
                if (ev.IsReady)
                {
                    __instance.TargetState = flag;
                    __instance.Cooldown.Trigger(ev.Cooldown);
                }
            }
            else
            {
                __instance.TargetState = false;
            }

            __instance.ServerSendRpc(toAll);
            return false;
        }
    }
}
