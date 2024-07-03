// -----------------------------------------------------------------------
// <copyright file="EventHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;

    /// <summary>
    /// Example of an event handler.
    /// </summary>
    public class EventHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandler"/> class.
        /// </summary>
        public EventHandler()
        {
            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="EventHandler"/> class.
        /// </summary>
        ~EventHandler()
        {
            Exiled.Events.Handlers.Player.Verified -= OnVerified;
        }

        private void OnVerified(VerifiedEventArgs ev)
        {
        }

        private void OnSpawned(SpawnedEventArgs ev)
        {
            ev.Player.HumeShield = 0;
            ev.Player.MaxHumeShield = 0;
        }
    }
}