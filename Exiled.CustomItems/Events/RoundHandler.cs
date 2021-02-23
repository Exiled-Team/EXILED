// -----------------------------------------------------------------------
// <copyright file="RoundHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems
{
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;

    using UnityEngine;

    using static CustomItems;

    /// <summary>
    /// Event Handlers for the CustomItem API.
    /// </summary>
    internal sealed class RoundHandler
    {
        /// <inheritdoc cref="Events.Handlers.Server.OnRoundStarted"/>
        public void OnRoundStarted()
        {
            foreach (CustomItem customItem in Instance.ItemManagers)
            {
                if (customItem.SpawnProperties == null)
                    continue;

                int spawned = 0;

                foreach (DynamicSpawnPoint spawnPoint in customItem.SpawnProperties.DynamicSpawnPoints)
                {
                    Log.Debug($"Attempting to spawn {customItem.Name} at {spawnPoint.Location}", Instance.Config.Debug);

                    if (Random.Range(1, 101) >= spawnPoint.Chance || (customItem.SpawnProperties.Limit > 0 && spawned >= customItem.SpawnProperties.Limit))
                        continue;

                    spawned++;

                    customItem.Spawn(spawnPoint.Position.ToVector3);

                    Log.Debug($"Spawned {customItem.Name} at {spawnPoint.Name} - {spawnPoint.Position}", Instance.Config.Debug);
                }

                // This code should be put in a method, because it's literally the same as above
                foreach (StaticSpawnPoint spawnPoint in customItem.SpawnProperties.StaticSpawnPoints)
                {
                    Log.Debug($"Attempting to spawn {customItem.Name} at {spawnPoint.Name}", Instance.Config.Debug);

                    if (Random.Range(1, 101) >= spawnPoint.Chance || (customItem.SpawnProperties.Limit > 0 && spawned >= customItem.SpawnProperties.Limit))
                        continue;

                    spawned++;

                    customItem.Spawn(spawnPoint.Position.ToVector3);

                    Log.Debug($"Spawned {customItem.Name} at {spawnPoint.Name} - {spawnPoint.Position}", Instance.Config.Debug);
                }
            }
        }
    }
}
