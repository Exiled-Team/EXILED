// -----------------------------------------------------------------------
// <copyright file="Scp914Handler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.Events
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Scp914;

    /// <summary>
    /// Handles SCP-914 events.
    /// </summary>
    internal sealed class Scp914Handler
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Scp914.OnUpgradingPickup(UpgradingPickupEventArgs)"/>
        public void OnUpgradingItem(UpgradingPickupEventArgs ev)
        {
            Log.Info($"Item being upgraded\n[Type]: {ev.Pickup.Type}\n[Weight]: {ev.Pickup.Weight}\n[Output Position]: {ev.OutputPosition}\n[Knob Setting]: {ev.KnobSetting}");
        }
    }
}