// -----------------------------------------------------------------------
// <copyright file="Throwable.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Exiled.API.Features.Core.Attributes;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Pickups.Projectiles;
    using Exiled.API.Interfaces;
    using InventorySystem.Items.ThrowableProjectiles;
    using UnityEngine;

    /// <summary>
    /// A wrapper class for throwable items.
    /// </summary>
    public class Throwable : Item, IWrapper<ThrowableItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Throwable"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="ThrowableItem"/> class.</param>
        public Throwable(ThrowableItem itemBase)
            : base(itemBase)
        {
            Base = itemBase;
            Base.Projectile.gameObject.SetActive(false);
            Projectile = (Projectile)Pickup.Get(Object.Instantiate(Base.Projectile));
            Base.Projectile.gameObject.SetActive(true);
            Projectile.Serial = Serial;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Throwable"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the throwable item.</param>
        /// <param name="player">The owner of the throwable item. Leave <see langword="null"/> for no owner.</param>
        /// <remarks>The player parameter will always need to be defined if this throwable is custom using Exiled.CustomItems.</remarks>
        internal Throwable(ItemType type, Player player = null)
            : this((ThrowableItem)(player ?? Server.Host).Inventory.CreateItemInstance(new(type, 0), true))
        {
        }

        /// <summary>
        /// Gets the <see cref="ThrowableItem"/> base for this item.
        /// </summary>
        public new ThrowableItem Base { get; }

        /// <summary>
        /// Gets a <see cref="Pickups.Projectiles.Projectile"/> to change grenade properties.
        /// </summary>
        public Projectile Projectile { get; }

        /// <summary>
        /// Gets or sets the amount of time it takes to pull the pin.
        /// </summary>
        [EProperty(category: nameof(Throwable))]
        public float PinPullTime
        {
            get => Base._pinPullTime;
            set => Base._pinPullTime = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether players can pickup grenade after throw.
        /// </summary>
        [EProperty(category: nameof(Throwable))]
        public bool Repickable
        {
            get => Base._repickupable;
            set => Base._repickupable = value;
        }

        /// <summary>
        /// Throws the item.
        /// </summary>
        /// <param name="fullForce">Whether to use full or weak force.</param>
        /// this.ServerThrow(projectileSettings.StartVelocity, projectileSettings.UpwardsFactor, projectileSettings.StartTorque, startVel);
        public void Throw(bool fullForce = true)
        {
            ThrowableItem.ProjectileSettings settings = fullForce ? Base.FullThrowSettings : Base.WeakThrowSettings;

            Base.ServerThrow(settings.StartVelocity, settings.UpwardsFactor, settings.StartTorque, ThrowableNetworkHandler.GetLimitedVelocity(Owner?.Velocity ?? Vector3.one));
        }

        /// <summary>
        /// Clones current <see cref="Throwable"/> object.
        /// </summary>
        /// <returns> New <see cref="Throwable"/> object. </returns>
        public override Item Clone() => new Throwable(Type)
        {
            PinPullTime = PinPullTime,
            Repickable = Repickable,
        };

        /// <summary>
        /// Returns the Throwable in a human readable format.
        /// </summary>
        /// <returns>A string containing Throwable-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{PinPullTime}|";
    }
}