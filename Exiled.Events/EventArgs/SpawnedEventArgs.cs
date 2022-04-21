// -----------------------------------------------------------------------
// <copyright file="SpawnedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all information after spawning a player.
    /// </summary>
    public class SpawnedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnedEventArgs"/> class.
        /// </summary>
        /// <param name="referenceHub"><see cref="ReferenceHub"/> of the spawned player.</param>
        public SpawnedEventArgs(ReferenceHub referenceHub) => Player = Player.Get(referenceHub);

        /// <summary>
        /// Gets the <see cref="API.Features.Player"/> who spawned.
        /// </summary>
        public Player Player { get; }
    }
}
