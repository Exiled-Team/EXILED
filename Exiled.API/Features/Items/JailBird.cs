// -----------------------------------------------------------------------
// <copyright file="JailBird.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using InventorySystem.Items;
    using InventorySystem.Items.Flashlight;
    using InventorySystem.Items.Jailbird;
    using Utils.Networking;

    /// <summary>
    /// A wrapped class for <see cref="JailbirdItem"/>.
    /// </summary>
    public class JailBird : Item
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JailBird"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="JailbirdItem"/> class.</param>
        public JailBird(JailbirdItem itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JailBird"/> class, as well as a new Flashlight item.
        /// </summary>
        internal JailBird()
            : this((JailbirdItem)Server.Host.Inventory.CreateItemInstance(new(ItemType.Jailbird, 0), false))
        {
        }

        /// <summary>
        /// Gets the <see cref="FlashlightItem"/> that this class is encapsulating.
        /// </summary>
        public new JailbirdItem Base { get; }

        /// <summary>
        /// Gets or Sets the saving value of the total damage dealt.
        /// </summary>
        public float TotalMelee
        {
            get => Base._hitreg.TotalMeleeDamageDealt;
            set => Base._hitreg.TotalMeleeDamageDealt = value;
        }

        /// <summary>
        /// Gets or Sets the number of charge remaining in the item.
        /// </summary>
        public int TotalCharges
        {
            get => Base.TotalChargesPerformed;
            set => Base.TotalChargesPerformed = value;
        }

        /// <summary>
        /// Clones current <see cref="JailBird"/> object.
        /// </summary>
        /// <returns> New <see cref="JailBird"/> object. </returns>
        public override Item Clone() => new JailBird()
        {
            TotalMelee = TotalMelee,
            TotalCharges = TotalCharges,
        };

        /// <summary>
        /// Returns the Flashlight in a human readable format.
        /// </summary>
        /// <returns>A string containing Flashlight-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Active}|";
    }
}