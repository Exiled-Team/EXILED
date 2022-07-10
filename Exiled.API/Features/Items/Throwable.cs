// -----------------------------------------------------------------------
// <copyright file="Throwable.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using InventorySystem.Items.ThrowableProjectiles;

    using MEC;

    using UnityEngine;

    /// <summary>
    /// A wrapper class for throwable items.
    /// </summary>
    public class Throwable : Item
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Throwable"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="ThrowableItem"/> class.</param>
        public Throwable(ThrowableItem itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Throwable"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the throwable item.</param>
        /// <param name="player">The owner of the throwable item. Leave <see langword="null"/> for no owner.</param>
        /// <remarks>The player parameter will always need to be defined if this throwable is custom using Exiled.CustomItems.</remarks>
        internal Throwable(ItemType type, Player player = null)
            : this(player is null ? (ThrowableItem)Server.Host.Inventory.CreateItemInstance(type, false) : (ThrowableItem)player.Inventory.CreateItemInstance(type, true))
        {
        }

        /// <summary>
        /// Gets the <see cref="ThrowableItem"/> base for this item.
        /// </summary>
        public new ThrowableItem Base { get; internal set; }

        /// <summary>
        /// Gets or sets the amount of time it takes to pull the pin.
        /// </summary>
        public float PinPullTime
        {
            get => Base._pinPullTime;
            set => Base._pinPullTime = value;
        }

        /// <summary>
        /// Throws the item.
        /// </summary>
        /// <param name="fullForce">Whether to use full or half force.</param>
        /// this.ServerThrow(projectileSettings.StartVelocity, projectileSettings.UpwardsFactor, projectileSettings.StartTorque, startVel);
        public void Throw(bool fullForce = true)
        {
            ThrowableItem.ProjectileSettings settings = fullForce ? Base.FullThrowSettings : Base.WeakThrowSettings;
            Base.ServerThrow(settings.StartVelocity, settings.UpwardsFactor, settings.StartTorque, ThrowableNetworkHandler.GetLimitedVelocity(Base.Owner?.playerMovementSync.PlayerVelocity ?? Vector3.one));
        }

        /// <summary>
        /// Returns the Throwable in a human readable format.
        /// </summary>
        /// <returns>A string containing Throwable-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{PinPullTime}|";

        /// <summary>
        /// Clones current <see cref="Throwable"/> object.
        /// </summary>
        /// <returns> New <see cref="Throwable"/> object. </returns>
        public override Item Clone()
        {
            Throwable cloneableItem = new(Type);

            cloneableItem.PinPullTime = PinPullTime;

            return cloneableItem;
        }
    }
}
