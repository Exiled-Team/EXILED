// -----------------------------------------------------------------------
// <copyright file="TimeGrenadePickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using InventorySystem.Items.ThrowableProjectiles;

    /// <summary>
    /// A wrapper class for TimeGrenade.
    /// </summary>
    public class TimeGrenadePickup : ProjectilePickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeGrenadePickup"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="TimeGrenade"/> class.</param>
        public TimeGrenadePickup(TimeGrenade itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Gets the <see cref="TimeGrenade"/> that this class is encapsulating.
        /// </summary>
        public new TimeGrenade Base { get; }

        /// <summary>
        /// Gets or sets a value indicating whether if the grenade have already explode.
        /// </summary>
        public bool IsAlreadyDetonated
        {
            get => Base._alreadyDetonated;
            set => Base._alreadyDetonated = value;
        }

        /// <summary>
        /// Gets or sets the time to explode before it's start.
        /// </summary>
        public float FuseTime
        {
            get => Base._fuseTime;
            set => Base._fuseTime = value;
        }

        /// <summary>
        /// Gets or sets how long it going to takes to explode.
        /// </summary>
        public float TargetTime
        {
            get => Base.TargetTime;
            set => Base.TargetTime = value;
        }
    }
}
