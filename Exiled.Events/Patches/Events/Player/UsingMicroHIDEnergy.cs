// -----------------------------------------------------------------------
// <copyright file="UsingMicroHIDEnergy.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313

    using HarmonyLib;

    using InventorySystem.Items.MicroHID;

    /// <summary>
    /// Patches <see cref="MicroHIDItem.ExecuteServerside"/>.
    /// Adds the <see cref="Handlers.Player.OnUsingMicroHIDEnergy"/> event.
    /// </summary>
    [HarmonyPatch(typeof(MicroHIDItem), nameof(MicroHIDItem.ExecuteServerside))]
    internal static class UsingMicroHIDEnergy
    {
        // TODO: This
    }
}
