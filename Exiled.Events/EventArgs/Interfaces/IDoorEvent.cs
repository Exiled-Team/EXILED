// -----------------------------------------------------------------------
// <copyright file="IDoorEvent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces
{
    using Exiled.API.Features;

    /// <summary>
    ///     Event args used for all <see cref="Exiled.API.Features.Door" /> related events.
    /// </summary>
    public interface IDoorEvent : IExiledEvent
    {
        /// <summary>
        ///     Gets the <see cref="Exiled.API.Features.Door" /> triggering the event.
        /// </summary>
        public Door Door { get; }
    }
}
