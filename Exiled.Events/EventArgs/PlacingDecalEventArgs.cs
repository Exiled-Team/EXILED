// -----------------------------------------------------------------------
// <copyright file="PlacingDecalEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
#pragma warning disable SA1401
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    using UnityEngine;

    /// <summary>
    /// Contains all informations before placing a decal.
    /// </summary>
    public class PlacingDecalEventArgs : EventArgs
    {
        /// <summary>
        /// The Type Object used by the GunShoot to determine decal type.
        /// </summary>
        internal GameObject TypeObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlacingDecalEventArgs"/> class.
        /// </summary>
        /// <param name="owner"><inheritdoc cref="Owner"/></param>
        /// <param name="hit"><inheritdoc cref="RaycastHit"/></param>
        /// <param name="type"><inheritdoc cref="Type"/></param>
        /// <param name="shoot"><inheritdoc cref="GunShoot"/></param>
        public PlacingDecalEventArgs(Player owner, RaycastHit hit, GameObject type, GunShoot shoot)
        {
            Owner = owner;
            Position = hit.point;
            Rotation = Quaternion.LookRotation(hit.normal);
            GunShoot = shoot;
            TypeObject = type;
        }

        /// <summary>
        /// Gets the decal owner.
        /// </summary>
        public Player Owner { get; }

        /// <summary>
        /// Gets the <see cref="GunShoot"/> that is placing the decal.
        /// </summary>
        public GunShoot GunShoot { get; }

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
        public DecalType Type
        {
            get => DecalTypes.GetDecalType(TypeObject, GunShoot);
            set => TypeObject = DecalTypes.GetDecalObject(value, GunShoot);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the decal can be placed.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
