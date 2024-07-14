// -----------------------------------------------------------------------
// <copyright file="PreAssigningHumanRolesEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using Interfaces;

    using PlayerRoles;

    /// <summary>
    /// Contains all information before setting up the environment for the assignment of human roles.
    /// </summary>
    public class PreAssigningHumanRolesEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreAssigningHumanRolesEventArgs" /> class.
        /// </summary>
        /// <param name="queue"><inheritdoc cref="Queue"/></param>
        /// <param name="queueLength"><inheritdoc cref="QueueLength"/></param>
        public PreAssigningHumanRolesEventArgs(Team[] queue, int queueLength)
        {
            Queue = queue;
            QueueLength = queueLength;
        }

        /// <summary>
        /// Gets or sets the human queue.
        /// </summary>
        public Team[] Queue { get; set; }

        /// <summary>
        /// Gets or sets the human queue length.
        /// </summary>
        public int QueueLength { get; set; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; } = true;
    }
}