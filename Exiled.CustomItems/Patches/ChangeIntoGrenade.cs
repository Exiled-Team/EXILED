// -----------------------------------------------------------------------
// <copyright file="ChangeIntoGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.Patches
{
#pragma warning disable SA1313
    using Exiled.CustomItems.API.Features;

    using Grenades;

    using HarmonyLib;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="FragGrenade.ChangeIntoGrenade"/>.
    /// </summary>
    [HarmonyPatch(typeof(FragGrenade), nameof(FragGrenade.ChangeIntoGrenade))]
    internal static class ChangeIntoGrenade
    {
        private static bool Prefix(FragGrenade __instance, Pickup item)
        {
            if (!CustomItem.TryGet(item, out CustomItem customItem) || !(customItem is CustomGrenade customGrenade))
                return true;

            item.Delete();
            customGrenade.Spawn(item.position, Vector3.zero, customGrenade.FuseTime, customGrenade.Type);

            return false;
        }
    }
}
