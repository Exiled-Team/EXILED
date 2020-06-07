// -----------------------------------------------------------------------
// <copyright file="CalmingDown.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp096
{
    #pragma warning disable SA1313
    using Exiled.Events.EventArgs;
    using HarmonyLib;
    using PlayableScps;

    /// <summary>
    /// Patches <see cref="Scp096.EndEnrage"/>.
    /// Adds the <see cref="Handlers.Scp096.CalmingDown"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp096), nameof(Scp096.EndEnrage))]
    public class CalmingDown
    {
        /// <summary>
        /// Prefix of <see cref="Scp096.EndEnrage"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="Scp096"/> instance.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(Scp096 __instance)
        {
            var ev = new CalmingDownEventArgs(__instance, API.Features.Player.Get(__instance.Hub.gameObject));

            Handlers.Scp096.OnCalmingDown(ev);

            return ev.IsAllowed;
        }
    }
}
