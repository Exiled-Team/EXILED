// -----------------------------------------------------------------------
// <copyright file="IEnvironmentalHazardEvent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces
{
    using Exiled.API.Features;

    /// <summary>
    ///     Event args used for all <see cref="EnvironmentalHazard" /> related events.
    /// </summary>
    public interface IEnvironmentalHazardEvent : IExiledEvent
    {
        /// <summary>
        ///     Gets the <see cref="EnvironmentalHazard" /> triggering the event.
        /// </summary>
        public EnvironmentalHazard EnvironmentalHazard { get; }
    }
}
