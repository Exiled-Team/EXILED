// -----------------------------------------------------------------------
// <copyright file="Scp914Handler.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Example.Events
{
    using SEXiled.API.Features;
    using SEXiled.Events.EventArgs;

    /// <summary>
    /// Handles SCP-914 events.
    /// </summary>
    internal sealed class Scp914Handler
    {
        /// <inheritdoc cref="SEXiled.Events.Handlers.Scp914.OnUpgradingItem(UpgradingItemEventArgs)"/>
        public void OnUpgradingItem(UpgradingItemEventArgs ev)
        {
            Log.Info($"Item being upgraded\n[Type]: {ev.Item.Type}\n[Weight]: {ev.Item.Weight}\n[Output Position]: {ev.OutputPosition}\n[Knob Setting]: {ev.KnobSetting}");
        }
    }
}
