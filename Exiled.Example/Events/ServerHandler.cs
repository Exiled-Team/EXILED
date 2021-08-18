// -----------------------------------------------------------------------
// <copyright file="ServerHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.Events
{
    using System;

    using CameraShaking;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs;

    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Coin;
    using InventorySystem.Items.Firearms;

    using MEC;

    using UnityEngine;

    using Firearm = Exiled.API.Features.Items.Firearm;
    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

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

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRoundStarted"/>
        public void OnRoundStarted()
        {
            Log.Info("A round has started!");
            Timing.CallDelayed(2f, () =>
            {
                try
                {
                    for (int i = 0; i < Enum.GetValues(typeof(ItemType)).GetUpperBound(0); i++)
                    {
                        Item item = new Item((ItemType)i);
                        Log.Warn($"{nameof(OnRoundStarted)}: {(ItemType)i} -- {item.Type}");
                        Pickup pickup = item.Spawn(new Vector3(53f, 1020f, -44f), default);
                        Log.Info($"Spawned {pickup.Type} ({pickup.Serial}) at {pickup.Position}.  Weight: {pickup.Weight} Is Locked: {pickup.Locked}");
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"{nameof(OnRoundStarted)}: {e}");
                }
            });
        }
    }
}
