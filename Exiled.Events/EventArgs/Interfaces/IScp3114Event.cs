// -----------------------------------------------------------------------
// <copyright file="IScp3114Event.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces
{
    using Exiled.API.Features.Roles;

    /// <summary>
    /// Event args used for all <see cref="Scp3114Role" /> related events.
    /// </summary>
    public interface IScp3114Event : IPlayerEvent
    {
        /// <summary>
        /// Gets the <see cref="Scp3114Role" /> triggering the event.
        /// </summary>
        public Scp3114Role Scp3114 { get; }
    }
}