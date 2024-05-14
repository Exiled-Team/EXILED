// -----------------------------------------------------------------------
// <copyright file="TimeGrenadeProjectile.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups.Projectiles
{
    using Exiled.API.Extensions;
    using Exiled.API.Interfaces;
    using InventorySystem;
    using InventorySystem.Items.ThrowableProjectiles;

    using Mirror;
    using UnityEngine;

    /// <summary>
    /// A wrapper class for TimeGrenade.
    /// </summary>
    public class TimeGrenadeProjectile : Projectile, IWrapper<TimeGrenade>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeGrenadeProjectile"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="TimeGrenade"/> class.</param>
        internal TimeGrenadeProjectile(TimeGrenade pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeGrenadeProjectile"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        internal TimeGrenadeProjectile(ItemType type)
            : this((TimeGrenade)type.GetItemBase().ServerDropItem())
        {
        }

        /// <summary>
        /// Gets the <see cref="TimeGrenade"/> that this class is encapsulating.
        /// </summary>
        public new TimeGrenade Base { get; }

        /// <summary>
        /// Gets a value indicating whether the grenade has already exploded.
        /// </summary>
        public bool IsAlreadyDetonated => Base._alreadyDetonated;

        /// <summary>
        /// Gets or sets FuseTime.
        /// </summary>
        public float FuseTime
        {
            get => Base._fuseTime;
            set
            {
                Base._fuseTime = value;
                if (IsActive)
                    Base.TargetTime = NetworkTime.time + value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the greande is active.
        /// </summary>
        public bool IsActive
        {
            get => Base.TargetTime != 0.0;
            set
            {
                if (value && Base.TargetTime == 0.0)
                    Base.TargetTime = FuseTime;
                else if (!value && Base.TargetTime != 0.0)
                    Base.TargetTime = 0.0;
            }
        }

        /// <summary>
        /// Immediately exploding the <see cref="TimeGrenadeProjectile"/>.
        /// </summary>
        public void Explode()
        {
            Base.ServerFuseEnd();
            Base._alreadyDetonated = true;
        }

        /// <summary>
        /// Returns the TimeGrenadePickup in a human readable format.
        /// </summary>
        /// <returns>A string containing TimeGrenadePickup related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{FuseTime}| ={IsAlreadyDetonated}=";
    }
}
