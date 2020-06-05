// -----------------------------------------------------------------------
// <copyright file="RemovingHandcuffs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events
{
    #pragma warning disable SA1313
    using Exiled.Events.Handlers;
    using Exiled.Events.Handlers.EventArgs;
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="Handcuffs.ClearTarget"/>.
    /// Adds the <see cref="Player.RemovingHandcuffs"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Handcuffs), nameof(Handcuffs.ClearTarget))]
    public class RemovingHandcuffs
    {
        /// <summary>
        /// Prefix of <see cref="Handcuffs.ClearTarget"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="Handcuffs"/> instance.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(Handcuffs __instance)
        {
            foreach (API.Features.Player target in API.Features.Player.List)
            {
                if (target == null)
                    continue;

                if (target.CufferId == __instance.MyReferenceHub.queryProcessor.PlayerId)
                {
                    var ev = new RemovingHandcuffsEventArgs(API.Features.Player.Get(__instance.gameObject), target);

                    Player.OnRemovingHandcuffs(ev);

                    if (ev.IsAllowed)
                        target.CufferId = -1;

                    break;
                }
            }

            return false;
        }
    }
}