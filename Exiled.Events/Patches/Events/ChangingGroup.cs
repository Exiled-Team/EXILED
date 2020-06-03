// -----------------------------------------------------------------------
// <copyright file="ChangingGroup.cs" company="Exiled Team">
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
    /// Patches <see cref="ServerRoles.SetGroup(UserGroup, bool, bool, bool)"/>.
    /// Adds the <see cref="Player.ChangingGroup"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.SetGroup))]
    public class ChangingGroup
    {
        /// <summary>
        /// Prefix of <see cref="ServerRoles.SetGroup(UserGroup, bool, bool, bool)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="ServerRoles"/> instance.</param>
        /// <param name="group">The group to be set.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(ServerRoles __instance, UserGroup group)
        {
            var ev = new ChangingGroupEventArgs(API.Features.Player.Get(__instance.gameObject), group);

            Player.OnChangingGroup(ev);

            return ev.IsAllowed;
        }
    }
}
