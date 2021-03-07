// -----------------------------------------------------------------------
// <copyright file="Scp914Handler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.Events
{
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    /// <summary>
    /// Handles SCP-914 events.
    /// </summary>
    internal sealed class Scp914Handler
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Scp914.OnUpgradingItems(UpgradingItemsEventArgs)"/>
        public void OnUpgradingItems(UpgradingItemsEventArgs ev)
        {
            Log.Info($"Items ({ev.Items.Count}):\n{ev.Items.ToString(false)}\nand players ({ev.Players.Count}):\n{ev.Players.ToString(false)}\nare being processed inside SCP-914, which is set on {ev.KnobSetting}.");
        }
    }
}
