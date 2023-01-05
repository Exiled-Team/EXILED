// -----------------------------------------------------------------------
// <copyright file="KeycardPickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using BaseKeycard = InventorySystem.Items.Keycards.KeycardPickup;

    /// <summary>
    /// A wrapper class for a Keycard pickup.
    /// </summary>
    public class KeycardPickup : Pickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeycardPickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="BaseKeycard"/> class.</param>
        internal KeycardPickup(BaseKeycard pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeycardPickup"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        internal KeycardPickup(ItemType type)
            : base(type)
        {
            Base = (BaseKeycard)((Pickup)this).Base;
        }

        /// <summary>
        /// Gets the <see cref="BaseKeycard"/> that this class is encapsulating.
        /// </summary>
        public new BaseKeycard Base { get; }
    }
}
