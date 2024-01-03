// -----------------------------------------------------------------------
// <copyright file="RemovingObserverEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp173
{
    using API.Features;
    using Interfaces;

    /// <summary>
    /// Contains all information before a players just saw scp 173 (useful for an UTR for example).
    /// </summary>
    public class RemovingObserverEventArgs : IExiledEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemovingObserverEventArgs"/> class.
        /// </summary>
        /// <param name="scp173">
        /// <inheritdoc cref="Scp173" />
        /// </param>
        /// <param name="target">
        /// <inheritdoc cref="Target" />
        /// </param>
        public RemovingObserverEventArgs(Player scp173, Player target)
        {
            Scp173 = scp173;
            Target = target;
        }

        /// <summary>
        /// Gets the scp 173 as a <see cref="Player"/>.
        /// </summary>
        public Player Scp173 { get; }

        /// <summary>
        /// Gets the target who no longer see the scp 173, as a <see cref="Player"/>.
        /// </summary>
        public Player Target { get; }
    }
}