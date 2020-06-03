// -----------------------------------------------------------------------
// <copyright file="ContainingScp106.cs" company="Exiled Team">
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
    /// Patches <see cref="PlayerInteract.CallCmdContain106"/>.
    /// Adds the <see cref="Player.ContainingScp106"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdContain106))]
    public class ContainingScp106
    {
        /// <summary>
        /// Prefix of <see cref="PlayerInteract.CallCmdContain106"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="CharacterClassManager"/> instance.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(CharacterClassManager __instance)
        {
            var ev = new ContainingScp106EventArgs(API.Features.Player.Get(__instance.gameObject));

            Player.OnContainingScp106(ev);

            return ev.IsAllowed;
        }
    }
}
