// -----------------------------------------------------------------------
// <copyright file="INonInteractableDoor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces
{
    /// <summary>
    /// Represents an interface for all non-interactable doors.
    /// </summary>
    public interface INonInteractableDoor
    {
        /// <summary>
        /// Gets or sets a value indicating whether door should ignore lockdowns.
        /// </summary>
        public bool IgnoreLockdowns { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether door should ignore RA requests.
        /// </summary>
        public bool IgnoreRemoteAdmin { get; set; }
    }
}