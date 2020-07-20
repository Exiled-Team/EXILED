// -----------------------------------------------------------------------
// <copyright file="PickingUpItem.cs" company="Exiled Team">
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

    using Searching;

    /// <summary>
    /// Patches <see cref="ItemSearchCompletor.Complete"/>.
    /// Adds the <see cref="Player.PickingUpItem"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.Complete))]
    internal static class PickingUpItem
    {
        private static bool Prefix(ItemSearchCompletor __instance)
        {
            try
            {
                var ev = new PickingUpItemEventArgs(API.Features.Player.Get(__instance.Hub.gameObject), __instance.TargetPickup);

                Player.OnPickingUpItem(ev);

                return ev.IsAllowed;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.PickingUpItem: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
