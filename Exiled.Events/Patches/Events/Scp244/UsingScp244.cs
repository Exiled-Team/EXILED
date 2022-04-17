// -----------------------------------------------------------------------
// <copyright file="UsingScp244.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp244
{
#pragma warning disable SA1313
    using System;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items.Usables.Scp244;
    using InventorySystem.Searching;

    /// <summary>
    /// Patches <see cref="Scp244SearchCompletor.Complete"/>.
    /// Adds the <see cref="Handlers.Scp244.PickingUpScp244"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp244Item), nameof(Scp244Item.ServerOnUsingCompleted))]
    internal static class UsingScp244
    {
        private static bool Prefix(Scp244Item __instance)
        {
            try
            {
                UsingScp244EventArgs ev = new(__instance, Player.Get(__instance.Owner));
                Handlers.Scp244.OnUsingScp244(ev);
                if (!ev.IsAllowed)
                {
                    return false;
                }

                __instance._primed = true;
                __instance.OwnerInventory.ServerDropItem(__instance.ItemSerial);
                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"{typeof(UsingScp244).FullName}.{nameof(Prefix)}:\n{ex}");
                return true;
            }
        }
    }
}
