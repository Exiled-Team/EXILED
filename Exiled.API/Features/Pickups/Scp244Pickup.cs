// -----------------------------------------------------------------------
// <copyright file="Scp244Pickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using System;

    using Exiled.API.Extensions;
    using Exiled.API.Features.Core.Attributes;
    using Exiled.API.Features.DamageHandlers;
    using Exiled.API.Features.Items;
    using Exiled.API.Interfaces;
    using InventorySystem;
    using InventorySystem.Items.Usables.Scp244;
    using UnityEngine;

    /// <summary>
    /// A wrapper class for a SCP-244 pickup.
    /// </summary>
    public class Scp244Pickup : UsablePickup, IWrapper<Scp244DeployablePickup>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp244Pickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="Scp244DeployablePickup"/> class.</param>
        internal Scp244Pickup(Scp244DeployablePickup pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp244Pickup"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        internal Scp244Pickup(ItemType type)
            : this((Scp244DeployablePickup)type.GetItemBase().ServerDropItem())
        {
        }

        /// <summary>
        /// Gets the <see cref="Scp244DeployablePickup"/> that this class is encapsulating.
        /// </summary>
        public new Scp244DeployablePickup Base { get; }

        /// <summary>
        /// Gets the amount of time this Scp244 has been on the ground.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Scp244Pickup))]
        public TimeSpan Lifetime => Base._lifeTime.Elapsed;

        /// <summary>
        /// Gets the speed of <see cref="Scp244Pickup"/>'s too grow.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Scp244Pickup))]
        public float GrowSpeed => Base.GrowSpeed;

        /// <summary>
        /// Gets the time for the sphere to finish their expansion.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Scp244Pickup))]
        public float TimeToGrow => Base.TimeToGrow;

        /// <summary>
        /// Gets the current size effect of the Scp244's Hypothermia.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Scp244Pickup))]
        public float CurrentDiameter => Base.CurrentDiameter;

        /// <summary>
        /// Gets or sets the current size percent of the Scp244's Hypothermia.
        /// </summary>
        [EProperty(category: nameof(Scp244Pickup))]
        public float CurrentSizePercent
        {
            get => Base.CurrentSizePercent;
            set => Base.CurrentSizePercent = value;
        }

        /// <summary>
        /// Gets or sets the maximum diameter within which SCP-244's hypothermia effect is dealt.
        /// </summary>
        /// <remarks>This does not prevent visual effects.</remarks>
        [EProperty(category: nameof(Scp244Pickup))]
        public float MaxDiameter
        {
            get => Base.MaxDiameter;
            set => Base.MaxDiameter = value;
        }

        /// <summary>
        /// Gets or sets the Scp244's remaining health.
        /// </summary>
        [EProperty(category: nameof(Scp244Pickup))]
        public float Health
        {
            get => Base._health;
            set => Base._health = value;
        }

        /// <summary>
        /// Gets a value indicating whether or not this Scp244 is breakable.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Scp244Pickup))]
        public bool IsBreakable => Base.State is Scp244State.Idle or Scp244State.Active;

        /// <summary>
        /// Gets a value indicating whether or not this Scp244 is broken.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Scp244Pickup))]
        public bool IsBroken => Base.State is Scp244State.Destroyed;

        /// <summary>
        /// Gets or sets the <see cref="Scp244State"/>.
        /// </summary>
        [EProperty(category: nameof(Scp244Pickup))]
        public Scp244State State
        {
            get => Base.State;
            set => Base.State = value;
        }

        /// <summary>
        /// Gets or sets the activation angle, where 1 is a minimum, and -1 it's a maximum activation angle.
        /// </summary>
        [EProperty(category: nameof(Scp244Pickup))]
        public float ActivationDot
        {
            get => Base._activationDot;
            set => Base._activationDot = value;
        }

        /// <summary>
        /// Damages the <see cref="Scp244Pickup"/>.
        /// </summary>
        /// <param name="handler">The <see cref="DamageHandler"/> used to deal damage.</param>
        /// <returns><see langword="true"/> if the the damage has been dealt; otherwise, <see langword="false"/>.</returns>
        public bool Damage(DamageHandler handler) => Base.Damage(handler.Damage, handler, Vector3.zero);

        /// <summary>
        /// Returns the Scp244Pickup in a human readable format.
        /// </summary>
        /// <returns>A string containing Scp244Pickup related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Health}| -{State}- ={CurrentSizePercent}=";

        /// <inheritdoc/>
        internal override void ReadItemInfo(Item item)
        {
            base.ReadItemInfo(item);
            if (item is Scp244 scp244)
            {
                ActivationDot = scp244.ActivationDot;
                MaxDiameter = scp244.MaxDiameter;
                Health = scp244.Health;
            }
        }
    }
}
