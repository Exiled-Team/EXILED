// -----------------------------------------------------------------------
// <copyright file="Scp914Handler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Sexiled.API.Features;
using Sexiled.Events.EventArgs;

namespace Sexiled.Example.Events
{
    using Sexiled.API.Extensions;
    using Sexiled.API.Features;
    using Sexiled.Events.EventArgs;

    /// <summary>
    /// Handles SCP-914 events.
    /// </summary>
    internal sealed class Scp914Handler
    {
        /// <inheritdoc cref="Sexiled.Events.Handlers.Scp914.OnUpgradingItems(UpgradingItemsEventArgs)"/>
        public void OnUpgradingItems(UpgradingItemsEventArgs ev)
        {
            Log.Info($"Items ({ev.Items.Count}):\n{ev.Items.ToString(false)}\nand players ({ev.Players.Count}):\n{ev.Players.ToString(false)}\nare being processed inside SCP-914, which is set on {ev.KnobSetting}.");
        }
    }
}
