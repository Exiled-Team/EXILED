// -----------------------------------------------------------------------
// <copyright file="Flashlight.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Exiled.API.Interfaces;
    using InventorySystem.Items.ToggleableLights;
    using InventorySystem.Items.ToggleableLights.Flashlight;
    using InventorySystem.Items.ToggleableLights.Lantern;
    using Utils.Networking;

    /// <summary>
    /// A wrapped class for <see cref="ToggleableLightItemBase"/>.
    /// </summary>
    public class Flashlight : Item, IWrapper<ToggleableLightItemBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Flashlight"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="ToggleableLightItemBase"/> class.</param>
        public Flashlight(ToggleableLightItemBase itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Flashlight"/> class, as well as a new Flashlight item.
        /// </summary>
        internal Flashlight()
            : this((ToggleableLightItemBase)Server.Host.Inventory.CreateItemInstance(new(ItemType.Flashlight, 0), false))
        {
        }

        /// <summary>
        /// Gets the <see cref="ToggleableLightItemBase"/> that this class is encapsulating.
        /// </summary>
        /// <remarks>Can be <see cref="FlashlightItem"/> or <see cref="LanternItem"/>.</remarks>
        public new ToggleableLightItemBase Base { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the flashlight is turned on.
        /// </summary>
        public bool IsEmittingLight
        {
            get => Base.IsEmittingLight;
            set
            {
                Base.IsEmittingLight = value;
                new FlashlightNetworkHandler.FlashlightMessage(Serial, value).SendToAuthenticated(0);
            }
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
        public override Item Clone() => new Flashlight
        {
            IsEmittingLight = IsEmittingLight,
            NextAllowedTime = NextAllowedTime
        };

        /// <summary>
        /// Returns the Lantern in a human readable format.
        /// </summary>
        /// <returns>A string containing item-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{IsEmittingLight}| /{NextAllowedTime}/";
    }
}
