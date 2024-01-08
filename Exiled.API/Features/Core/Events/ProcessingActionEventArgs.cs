// -----------------------------------------------------------------------
// <copyright file="ProcessingActionEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Events
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before processing an action.
    /// </summary>
    public class ProcessingActionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessingActionEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="input"><inheritdoc cref="Input"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ProcessingActionEventArgs(Player player, InputBinding input, bool isAllowed = true)
        {
            Player = player;
            IsAllowed = isAllowed;
            Input = input;
        }

        /// <summary>
        /// Gets the player who's processing the action.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the processing input.
        /// </summary>
        public InputBinding Input { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the action can be processed.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}