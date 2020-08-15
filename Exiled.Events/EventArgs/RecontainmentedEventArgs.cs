// -----------------------------------------------------------------------
// <copyright file="RecontainmentedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains informations after SCP-079 recontainming.
    /// </summary>
    public class RecontainmentedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecontainmentedEventArgs"/> class.
        /// </summary>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        public RecontainmentedEventArgs(Player target)
        {
            Target = target;
        }

        /// <summary>
        /// Gets the player that was SCP-079.
        /// </summary>
        public Player Target { get; }
    }
}
