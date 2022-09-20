// -----------------------------------------------------------------------
// <copyright file="TurningOffLightsEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before turning off lights.
    /// </summary>
    public class TurningOffLightsEventArgs : IDeniableEvent, IRoomEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TurningOffLightsEventArgs"/> class.
        /// </summary>
        /// <param name="duration"><inheritdoc cref="Duration"/></param>
        /// <param name="flickerableLightControllerHandler"><inheritdoc cref="FlickerableLightControllerHandler"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public TurningOffLightsEventArgs(FlickerableLightController flickerableLightControllerHandler, float duration, bool isAllowed = true)
        {
            FlickerableLightControllerHandler = flickerableLightControllerHandler;
            Duration = duration;
            IsAllowed = isAllowed;
            Room = Room.Get(flickerableLightControllerHandler);
        }

        /// <summary>
        /// Gets the <see cref="API.Features.Room" /> triggering the event.
        /// </summary>
        public Room Room { get; }

        /// <summary>
        /// Gets the <see cref="FlickerableLightController"/>.
        /// </summary>
        public FlickerableLightController FlickerableLightControllerHandler { get; }

        /// <summary>
        /// Gets or sets the blackout duration.
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the lights can be turned off.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}