// -----------------------------------------------------------------------
// <copyright file="EventHandlers.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems
{
    using System.Collections.Generic;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;

    /// <summary>
    /// Event Handlers for the CustomItem API.
    /// </summary>
    public class EventHandlers
    {
        private readonly CustomItems plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlers"/> class.
        /// </summary>
        /// <param name="plugin">The <see cref="CustomItems"/> class instantiating the class.</param>
        public EventHandlers(CustomItems plugin) => this.plugin = plugin;

        /// <summary>
        /// OnRoundStart handlers.
        /// </summary>
        public void OnRoundStart()
        {
            foreach (CustomItem item in plugin.ItemManagers)
            {
                if (item.SpawnProperties == null)
                    continue;

                int count = 0;

                foreach (CustomItemSpawn spawn in item.SpawnProperties.SpawnLocations)
                {
                    Log.Debug($"Attempting to spawn {item.Name} at {spawn.Name}", plugin.Config.Debug);
                    if (plugin.Rng.Next(100) >= spawn.Chance || (item.SpawnProperties.Limit > 0 && count >= item.SpawnProperties.Limit))
                        continue;

                    count++;
                    item.Spawn(spawn.Position);
                    Log.Debug($"Spawned {item.Name} at {spawn.Name} - {spawn.Position}", plugin.Config.Debug);
                }
            }
        }
    }
}
