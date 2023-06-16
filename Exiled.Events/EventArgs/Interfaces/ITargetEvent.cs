// -----------------------------------------------------------------------
// <copyright file="IPlayerEvent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces
{
    using API.Features;

    /// <summary>
    ///     Event args used for all <see cref="Player" /> related events.
    /// </summary>
    public interface ITargetEvent : IExiledEvent
    {
        /// <summary>
        ///     Gets the <see cref="Player" /> triggering the event.
        /// </summary>
        public Player Target { get; }
    }
}