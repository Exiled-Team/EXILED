// -----------------------------------------------------------------------
// <copyright file="PlacingDecalEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    using UnityEngine;

    /// <summary>
    /// Contains all informations before placing a decal.
    /// </summary>
    public class PlacingDecalEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlacingDecalEventArgs"/> class.
        /// </summary>
        /// <param name="owner"><inheritdoc cref="Owner"/></param>
        /// <param name="position"><inheritdoc cref="Position"/></param>
        /// <param name="rotation"><inheritdoc cref="Rotation"/></param>
        /// <param name="type"><inheritdoc cref="Type"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public PlacingDecalEventArgs(Player owner, Vector3 position, Quaternion rotation, int type, bool isAllowed = true)
        {
            Owner = owner;
            Position = position;
            Rotation = rotation;
            Type = type;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the decal owner.
        /// </summary>
        public Player Owner { get; }

        /// <summary>
        /// Gets or sets the decal position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the decal rotation.
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// Gets or sets the decal type.
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the decal can be placed.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
