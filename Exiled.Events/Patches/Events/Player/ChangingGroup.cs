// -----------------------------------------------------------------------
// <copyright file="ChangingGroup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="ServerRoles.SetGroup(UserGroup, bool, bool, bool)"/>.
    /// Adds the <see cref="Player.ChangingGroup"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.SetGroup))]
    internal static class ChangingGroup
    {
        private static bool Prefix(ServerRoles __instance, UserGroup group)
        {
            try
            {
                var ev = new ChangingGroupEventArgs(API.Features.Player.Get(__instance.gameObject), group);

                Player.OnChangingGroup(ev);

                return ev.IsAllowed;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.ChangingGrounp: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
