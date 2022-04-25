// -----------------------------------------------------------------------
// <copyright file="PickingUpScp244.cs" company="Exiled Team">
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
    [HarmonyPatch(typeof(Scp244SearchCompletor), nameof(Scp244SearchCompletor.Complete))]
    internal static class PickingUpScp244
    {
        private static bool Prefix(Scp244SearchCompletor __instance)
        {
            try
            {
                if (__instance.TargetPickup is not Scp244DeployablePickup scp244DeployablePickup)
                {
                    return false;
                }

                PickingUpScp244EventArgs ev = new(Player.Get(__instance.Hub), scp244DeployablePickup);
                Handlers.Scp244.OnPickingUpScp244(ev);
                if (!ev.IsAllowed)
                {
                    return false;
                }

                __instance.Hub.inventory.ServerAddItem(__instance.TargetPickup.Info.ItemId, __instance.TargetPickup.Info.Serial, __instance.TargetPickup);
                scp244DeployablePickup.State = Scp244State.PickedUp;
                __instance.CheckCategoryLimitHint();
                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"{typeof(PickingUpScp244).FullName}.{nameof(Prefix)}:\n{ex}");
                return true;
            }
        }
    }
}
