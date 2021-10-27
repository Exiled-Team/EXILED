// -----------------------------------------------------------------------
// <copyright file="ChangedSpectatedPlayerEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using Exiled.API.Features;

    /// <summary>
    /// Contains all information about spectated player change.
    /// </summary>
    public class ChangedSpectatedPlayerEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangedSpectatedPlayerEventArgs"/> class.
        /// </summary>
        /// <param name="spectator"><inheritdoc cref="Spectator"/></param>
        /// <param name="oldTarget"><inheritdoc cref="OldTarget"/></param>
        /// <param name="newTarget"><inheritdoc cref="NewTarget"/></param>
        public ChangedSpectatedPlayerEventArgs(Player spectator, Player oldTarget, Player newTarget)
        {
            this.Spectator = spectator;
            this.OldTarget = oldTarget;
            this.NewTarget = newTarget;
        }

        /// <summary>
        /// Gets player that is changing spectated player.
        /// </summary>
        public Player Spectator { get; }

        /// <summary>
        /// Gets player that was spectated.
        /// </summary>
        public Player OldTarget { get; }

        /// <summary>
        /// Gets player that is spectated.
        /// </summary>
        public Player NewTarget { get; }
    }
}
