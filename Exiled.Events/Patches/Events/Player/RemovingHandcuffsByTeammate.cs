// -----------------------------------------------------------------------
// <copyright file="RemovingHandcuffsByTeammate.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player {/*
#pragma warning disable SA1313
    using System;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="Handcuffs.CallCmdFreeTeammate(GameObject)"/>.
    /// Adds the <see cref="Player.RemovingHandcuffs"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Handcuffs), nameof(Handcuffs.CallCmdFreeTeammate))]
    internal static class RemovingHandcuffsByTeammate
    {
        private static bool Prefix(Handcuffs __instance, GameObject target)
        {
            try
            {
                if (!__instance._interactRateLimit.CanExecute(true) || target == null ||
                    (Vector3.Distance(target.transform.position, __instance.transform.position) >
                        __instance.raycastDistance * 1.10000002384186 || __instance.MyReferenceHub.characterClassManager
                            .Classes.SafeGet(__instance.MyReferenceHub.characterClassManager.CurClass).team ==
                        Team.SCP))
                    return false;

                var targetPlayer = API.Features.Player.Get(target);
                var ev = new RemovingHandcuffsEventArgs(API.Features.Player.Get(__instance.gameObject), targetPlayer);

                Player.OnRemovingHandcuffs(ev);

                if (!ev.IsAllowed)
                    return false;

                targetPlayer.CufferId = -1;

                return false;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.RemovingHandcuffsByTeammate: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }*/
}
