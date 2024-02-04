// -----------------------------------------------------------------------
// <copyright file="ICameraEvent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces
{
    using API.Features;

    /// <summary>
    /// Event args used for all <see cref="API.Features.Camera" /> related events.
    /// </summary>
    public interface ICameraEvent : IExiledEvent
    {
        /// <summary>
        /// Gets or sets the <see cref="API.Features.Camera" /> triggering the event.
        /// </summary>
        public Camera Camera { get; set; }
    }
}