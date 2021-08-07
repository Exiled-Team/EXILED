// -----------------------------------------------------------------------
// <copyright file="ServerHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.Events
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs;

    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Coin;

    using MEC;

    using UnityEngine;

    using Object = UnityEngine.Object;

    /// <summary>
    /// Handles server-related events.
    /// </summary>
    internal sealed class ServerHandler
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnWaitingForPlayers"/>
        public void OnWaitingForPlayers()
        {
            Log.Info("I'm waiting for players!"); // This is an example of information messages sent to your console!
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnEndingRound(EndingRoundEventArgs)"/>
        public void OnEndingRound(EndingRoundEventArgs ev)
        {
            Log.Debug($"The round is ending, fetching leading team...");

            if (ev.LeadingTeam == LeadingTeam.Draw)
                Log.Error($"The round has ended in a draw!");
            else
                Log.Info($"The leading team is actually: {ev.LeadingTeam}.");
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRoundStarted"/>
        public void OnRoundStarted()
        {
            Log.Info("A round has started!");
            Timing.CallDelayed(2f, () =>
            {
                Item item = new Item(ItemType.Flashlight);
                Pickup pickup = item.Spawn(new Vector3(53f, 1020f, -44f), default);
                Log.Info($"Spawned {pickup.Type} ({pickup.Serial}) at {pickup.Position}.  Weight: {pickup.Weight} Is Locked: {pickup.Locked}");
            });
        }
    }
}
