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

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using Searching;

    /// <summary>
    /// Patches <see cref="ItemSearchCompletor.Complete"/>.
    /// Adds the <see cref="Handlers.Player.PickingUpItem"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.Complete))]
    internal static class PickingUpItem
    {
        private static bool Prefix(ItemSearchCompletor __instance)
        {
            try
            {
                var ev = new PickingUpItemEventArgs(Player.Get(__instance.Hub), __instance.TargetPickup);

                Handlers.Player.OnPickingUpItem(ev);

                // Allow future pick up of this item
                if (!ev.IsAllowed)
                    __instance.TargetPickup.InUse = false;

                return ev.IsAllowed;
            }
            catch (Exception exception)
            {
                Log.Error($"{typeof(PickingUpItem).FullName}\n{exception}");

                return true;
            }
        }
    }
}
