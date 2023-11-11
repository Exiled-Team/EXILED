// -----------------------------------------------------------------------
// <copyright file="Lantern.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Exiled.API.Interfaces;
    using InventorySystem.Items.ToggleableLights.Lantern;

    /// <summary>
    /// A wrapper class for <see cref="LanternItem"/>.
    /// </summary>
    public class Lantern : Item, IWrapper<LanternItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Lantern"/> class.
        /// </summary>
        /// <param name="itemBase">The <see cref="LanternItem"/> instance.</param>
        public Lantern(LanternItem itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        internal Lantern()
            : this((LanternItem)Server.Host.Inventory.CreateItemInstance(new(ItemType.Lantern, 0), false))
        {
        }

        /// <inheritdoc/>
        public new LanternItem Base { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not light source is active.
        /// </summary>
        public bool IsEmittingLight
        {
            get => Base.IsEmittingLight;
            set => Base.IsEmittingLight = value;
        }

        /// <summary>
        /// Gets or sets time since level loaded when player will be able to change <see cref="IsEmittingLight"/> again.
        /// </summary>
        public float NextAllowedTime
        {
            get => Base.NextAllowedTime;
            set => Base.NextAllowedTime = value;
        }

        /// <inheritdoc/>
        public override Item Clone() => new Lantern
        {
            IsEmittingLight = IsEmittingLight,
            NextAllowedTime = NextAllowedTime
        };

        /// <summary>
        /// Returns the Lantern in a human readable format.
        /// </summary>
        /// <returns>A string containing Lantern-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{IsEmittingLight}| /{NextAllowedTime}/";
    }
}