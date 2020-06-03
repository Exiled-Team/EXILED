// -----------------------------------------------------------------------
// <copyright file="CalmingDownScp096.cs" company="Exiled Team">
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
    using PlayableScps;

    /// <summary>
    /// Patches <see cref="Scp096.EndEnrage"/>.
    /// Adds the <see cref="Player.CalmingDownScp096"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp096), nameof(Scp096.EndEnrage))]
    public class CalmingDownScp096
    {
        /// <summary>
        /// Prefix of <see cref="Scp096.EndEnrage"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="Scp096"/> instance.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(Scp096 __instance)
        {
            var ev = new CalmingDownScp096EventArgs(__instance, API.Features.Player.Get(__instance.Hub.gameObject));

            Player.OnCalmingDownScp096(ev);

            return ev.IsAllowed;
        }
    }
}