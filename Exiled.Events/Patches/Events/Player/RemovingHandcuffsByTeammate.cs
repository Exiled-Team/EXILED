// -----------------------------------------------------------------------
// <copyright file="RemovingHandcuffsByTeammate.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    #pragma warning disable SA1313
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="Handcuffs.CallCmdFreeTeammate(GameObject)"/>.
    /// Adds the <see cref="Player.RemovingHandcuffs"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Handcuffs), nameof(Handcuffs.CallCmdFreeTeammate))]
    public class RemovingHandcuffsByTeammate
    {
        /// <summary>
        /// Prefix of <see cref="Handcuffs.CallCmdFreeTeammate(GameObject)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="Handcuffs"/> instance.</param>
        /// <param name="target"><inheritdoc cref="HandcuffingEventArgs.Target"/></param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(Handcuffs __instance, GameObject target)
        {
            if (!__instance._interactRateLimit.CanExecute(true) || target == null ||
                (Vector3.Distance(target.transform.position, __instance.transform.position) >
                 __instance.raycastDistance * 1.10000002384186 || __instance.MyReferenceHub.characterClassManager
                     .Classes.SafeGet(__instance.MyReferenceHub.characterClassManager.CurClass).team == Team.SCP))
                return false;

            var targetPlayer = API.Features.Player.Get(target);
            var ev = new RemovingHandcuffsEventArgs(API.Features.Player.Get(__instance.gameObject), targetPlayer);

            Player.OnRemovingHandcuffs(ev);

            if (!ev.IsAllowed)
                return false;

            targetPlayer.CufferId = -1;

            return false;
        }
    }
}
