// -----------------------------------------------------------------------
// <copyright file="UsingRadioBattery.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118

    using HarmonyLib;

    using InventorySystem.Items.Radio;

    /// <summary>
    /// Patches <see cref="RadioItem.Update"/>.
    /// Adds the <see cref="Handlers.Player.UsingRadioBattery"/> event.
    /// </summary>
    [HarmonyPatch(typeof(RadioItem), nameof(RadioItem.Update))]
    internal static class UsingRadioBattery
    {
        // TODO: This
    }
}
