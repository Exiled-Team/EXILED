// -----------------------------------------------------------------------
// <copyright file="ServerHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.Events
{
    using System;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Spawn;
    using Exiled.API.Features.Toys;
    using PlayerRoles;
    using UnityEngine;

    /// <summary>
    /// Handles server-related events.
    /// </summary>
    internal sealed class ServerHandler
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnWaitingForPlayers"/>
        public void OnWaitingForPlayers()
        {
            Log.Info("I'm waiting for players!"); // This is an example of information messages sent to your console!
            Vector3 size = new(0.2f, 0.2f, 0.2f);

            foreach (RoleTypeId roleType in Enum.GetValues(typeof(RoleTypeId)))
            {
                Color rolecolor = roleType.GetColor();
                foreach (SpawnLocation spawnpoint in roleType.GetSpawns())
                {
                    Primitive primitive = Primitive.Create(spawnpoint.Position, null, size);
                    primitive.Color = rolecolor;
                    primitive.Type = PrimitiveType.Cube;
                }
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRoundStarted"/>
        public void OnRoundStarted()
        {
            Log.Info($"A round has started with {Player.Dictionary.Count} players!");
        }
    }
}