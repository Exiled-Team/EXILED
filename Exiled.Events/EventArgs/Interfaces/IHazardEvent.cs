// -----------------------------------------------------------------------
// <copyright file="IHazardEvent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces
{
    using Exiled.API.Features.Hazards;

    /// <summary>
    /// Event args for all <see cref="Hazard"/> related events.
    /// </summary>
    public interface IHazardEvent : IExiledEvent
    {

        /// <summary>
        /// Gets the environmental hazard that the player is interacting with.
        /// </summary>
        public Hazard Hazard { get; }
    }
}