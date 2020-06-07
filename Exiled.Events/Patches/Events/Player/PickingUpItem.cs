// -----------------------------------------------------------------------
// <copyright file="PickingUpItem.cs" company="Exiled Team">
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
    using Searching;

    /// <summary>
    /// Patches <see cref="ItemSearchCompletor.Complete"/>.
    /// Adds the <see cref="Player.PickingUpItem"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.Complete))]
    public class PickingUpItem
    {
        /// <summary>
        /// Prefix of <see cref="ItemSearchCompletor.Complete"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="ItemSearchCompletor"/> instance.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(ItemSearchCompletor __instance)
        {
            var ev = new PickingUpItemEventArgs(API.Features.Player.Get(__instance.Hub.gameObject), __instance.TargetPickup);

            Player.OnPickingUpItem(ev);

            return ev.IsAllowed;
        }
    }
}
