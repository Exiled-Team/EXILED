// -----------------------------------------------------------------------
// <copyright file="ServerHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.Events {
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
    internal sealed class ServerHandler {
        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnWaitingForPlayers"/>
        public void OnWaitingForPlayers() {
            Log.Info("I'm waiting for players!"); // This is an example of information messages sent to your console!
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRoundStarted"/>
        public void OnRoundStarted() {
            Log.Info($"A round has started with {Player.Dictionary.Count} players!");
        }
    }
}
