// -----------------------------------------------------------------------
// <copyright file="Flashlight.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Exiled.API.Interfaces;
    using InventorySystem.Items.SwitchableLightSources;
    using InventorySystem.Items.SwitchableLightSources.Flashlight;
    using Utils.Networking;

    /// <summary>
    /// A wrapped class for <see cref="FlashlightItem"/>.
    /// </summary>
    public class Flashlight : Item, IWrapper<FlashlightItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Flashlight"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="FlashlightItem"/> class.</param>
        public Flashlight(FlashlightItem itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Flashlight"/> class, as well as a new Flashlight item.
        /// </summary>
        internal Flashlight()
            : this((FlashlightItem)Server.Host.Inventory.CreateItemInstance(new(ItemType.Flashlight, 0), false))
        {
        }

        /// <summary>
        /// Gets the <see cref="FlashlightItem"/> that this class is encapsulating.
        /// </summary>
        public new FlashlightItem Base { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the flashlight is turned on.
        /// </summary>
        public bool Active
        {
            get => Base.IsEmittingLight;
            set
            {
                Base.IsEmittingLight = value;
                new FlashlightNetworkHandler.FlashlightMessage(Serial, value).SendToAuthenticated(0);
            }
        }

        /// <summary>
        /// Clones current <see cref="Flashlight"/> object.
        /// </summary>
        /// <returns> New <see cref="Flashlight"/> object. </returns>
        public override Item Clone() => new Flashlight()
        {
            Active = Active,
        };

        /// <summary>
        /// Returns the Flashlight in a human readable format.
        /// </summary>
        /// <returns>A string containing Flashlight-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Active}|";
    }
}