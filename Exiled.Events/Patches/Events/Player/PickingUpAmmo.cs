// -----------------------------------------------------------------------
// <copyright file="PickingUpAmmo.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;
    using System.Diagnostics;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using Searching;

    /// <summary>
    /// Patches <see cref="AmmoSearchCompletor.Complete"/>.
    /// Adds the <see cref="Handlers.Player.PickingUpAmmo"/> event.
    /// </summary>
    [HarmonyPatch(typeof(AmmoSearchCompletor), nameof(AmmoSearchCompletor.Complete))]
    internal static class PickingUpAmmo
    {
        private static bool Prefix(AmmoSearchCompletor __instance)
        {
            try
            {
                var ev = new PickingUpAmmoEventArgs(Player.Get(__instance.Hub), __instance.TargetPickup);

                Handlers.Player.OnPickingUpAmmo(ev);

                // Allow future pick up of this ammo
                if (!ev.IsAllowed)
                    __instance.TargetPickup.InUse = false;

                return ev.IsAllowed;
            }
            catch (Exception exception)
            {
                Log.Error($"{typeof(PickingUpAmmo).FullName}\n{exception.ToStringDemystified()}");

                return true;
            }
        }
    }
}
