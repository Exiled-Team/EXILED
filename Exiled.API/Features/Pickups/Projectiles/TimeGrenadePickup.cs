// -----------------------------------------------------------------------
// <copyright file="TimeGrenadePickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups.Projectiles
{
    using InventorySystem.Items.ThrowableProjectiles;

    /// <summary>
    /// A wrapper class for TimeGrenade.
    /// </summary>
    public class TimeGrenadePickup : ProjectilePickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeGrenadePickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="TimeGrenade"/> class.</param>
        internal TimeGrenadePickup(TimeGrenade pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeGrenadePickup"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        internal TimeGrenadePickup(ItemType type)
            : base(type)
        {
            Base = (TimeGrenade)((Pickup)this).Base;
        }

        /// <summary>
        /// Gets the <see cref="TimeGrenade"/> that this class is encapsulating.
        /// </summary>
        public new TimeGrenade Base { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the grenade have already explode.
        /// </summary>
        public bool IsAlreadyDetonated
        {
            get => Base._alreadyDetonated;
            set => Base._alreadyDetonated = value;
        }

        /// <summary>
        /// Gets or sets FuseTime.
        /// </summary>
        public float FuseTime
        {
            get => Base._fuseTime;
            set => Base._fuseTime = value;
        }

        /// <summary>
        /// Gets or sets time indicating how long it will take to explode.
        /// </summary>
        public float TargetTime
        {
            get => Base.TargetTime;
            set => Base.TargetTime = value;
        }

        /// <summary>
        /// Returns the TimeGrenadePickup in a human readable format.
        /// </summary>
        /// <returns>A string containing TimeGrenadePickup related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{FuseTime}| -{TargetTime}- ={IsAlreadyDetonated}=";
    }
}
