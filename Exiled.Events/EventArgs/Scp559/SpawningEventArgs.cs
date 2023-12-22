// -----------------------------------------------------------------------
// <copyright file="SpawningEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp559
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using UnityEngine;

    /// <summary>
    /// Contains all information before SCP-559 spawns.
    /// </summary>
    public class SpawningEventArgs : IDeniableEvent, IScp559Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpawningEventArgs"/> class.
        /// </summary>
        /// <param name="scp559"><inheritdoc cref="Scp559"/></param>
        /// <param name="oldPosition"><inheritdoc cref="OldPosition"/></param>
        /// <param name="newPosition"><inheritdoc cref="NewPosition"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public SpawningEventArgs(Scp559 scp559, Vector3 oldPosition, Vector3 newPosition, bool isAllowed = true)
        {
            Scp559 = scp559;
            NewPosition = newPosition;
            OldPosition = oldPosition;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }

        /// <inheritdoc/>
        public Scp559 Scp559 { get; }

        /// <summary>
        /// Gets or sets new position for <see cref="Scp559"/>.
        /// </summary>
        public Vector3 NewPosition { get; set; }

        /// <summary>
        /// Gets the previous position for <see cref="Scp559"/>.
        /// </summary>
        public Vector3 OldPosition { get; }
    }
}