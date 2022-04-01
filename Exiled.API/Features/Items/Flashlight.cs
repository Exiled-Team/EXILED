// -----------------------------------------------------------------------
// <copyright file="Flashlight.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items {
    using System;

    using Exiled.API.Enums;

    using InventorySystem.Items;
    using InventorySystem.Items.Flashlight;

    /// <summary>
    /// A wrapped class for <see cref="FlashlightItem"/>.
    /// </summary>
    public class Flashlight : Item {
        /// <summary>
        /// Initializes a new instance of the <see cref="Flashlight"/> class.
        /// </summary>
        /// <param name="itemBase"><inheritdoc cref="Base"/></param>
        public Flashlight(ItemBase itemBase)
            : base(itemBase) {
            Base = (FlashlightItem)itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Flashlight"/> class.
        /// </summary>
        /// <param name="type"><inheritdoc cref="Type"/></param>
        public Flashlight(ItemType type)
            : this((FlashlightItem)Server.Host.Inventory.CreateItemInstance(type, false)) {
        }

        /// <inheritdoc cref="Item.Base"/>
        public new FlashlightItem Base { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the flashlight is turned on.
        /// </summary>
        public bool Active {
            get => Base.IsEmittingLight;
            set => Base.IsEmittingLight = value;
        }

        /// <inheritdoc/>
        public override string ToString() {
            return $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Active}|";
        }
    }
}
