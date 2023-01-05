// -----------------------------------------------------------------------
// <copyright file="IRoomEvent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces
{
    using API.Features;

    /// <summary>
    ///     Event args used for all <see cref="API.Features.Room" /> related events.
    /// </summary>
    public interface IRoomEvent : IExiledEvent
    {
        /// <summary>
        ///     Gets the <see cref="API.Features.Room" /> that is a part of the event.
        /// </summary>
        public Room Room { get; }
    }
}