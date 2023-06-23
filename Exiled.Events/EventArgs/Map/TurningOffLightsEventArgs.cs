// -----------------------------------------------------------------------
// <copyright file="TurningOffLightsEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using Interfaces;

    /// <summary>
    /// Contains all information before turning off lights.
    /// </summary>
    public class TurningOffLightsEventArgs : IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TurningOffLightsEventArgs"/> class.
        /// </summary>
        /// <param name="duration"><inheritdoc cref="Duration"/></param>
        /// <param name="flickerableLightControllerHandler"><inheritdoc cref="RoomLightController"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public TurningOffLightsEventArgs(RoomLightController flickerableLightControllerHandler, float duration, bool isAllowed = true)
        {
            RoomLightController = flickerableLightControllerHandler;
            Duration = duration;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="RoomLightController"/>.
        /// </summary>
        public RoomLightController RoomLightController { get; }

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