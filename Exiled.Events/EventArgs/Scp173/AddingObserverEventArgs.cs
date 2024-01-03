// -----------------------------------------------------------------------
// <copyright file="AddingObserverEventArgs.cs" company="Exiled Team">
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
    public class AddingObserverEventArgs : IExiledEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddingObserverEventArgs"/> class.
        /// </summary>
        /// <param name="scp173">
        /// <inheritdoc cref="Scp173" />
        /// </param>
        /// <param name="target">
        /// <inheritdoc cref="Target" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public AddingObserverEventArgs(Player scp173, Player target, bool isAllowed = true)
        {
            Scp173 = scp173;
            Target = target;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the scp 173 as a <see cref="Player"/>.
        /// </summary>
        public Player Scp173 { get; }

        /// <summary>
        /// Gets the target who saw the scp 173, as a <see cref="Player"/>.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets or Sets a value indicating whether or not the player trigger scp 173 (for exemple, if scp173 is alone with the Target, it's can keep moving).
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}