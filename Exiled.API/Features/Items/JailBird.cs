// -----------------------------------------------------------------------
// <copyright file="JailBird.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using InventorySystem.Items.Jailbird;

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
        /// Gets the <see cref="JailbirdItem"/> that this class is encapsulating.
        /// </summary>
        public new JailbirdItem Base { get; }

        /// <summary>
        /// Gets or sets the amount of damage dealt with a Jailbird melee hit.
        /// </summary>
        public float MeleeDamage
        {
            get => Base._hitreg._damageMelee;
            set => Base._hitreg._damageMelee = value;
        }

        /// <summary>
        /// Gets or sets the amount of damage dealt with a Jailbird charge hit.
        /// </summary>
        public float ChargeDamage
        {
            get => Base._hitreg._damageCharge;
            set => Base._hitreg._damageCharge = value;
        }

        /// <summary>
        /// Gets or sets the total amount of damage dealt with the Jailbird.
        /// </summary>
        public float TotalDamageDealt
        {
            get => Base._hitreg.TotalMeleeDamageDealt;
            set => Base._hitreg.TotalMeleeDamageDealt = value;
        }

        /// <summary>
        /// Gets or sets the number of times the item has been charged and used.
        /// </summary>
        public int TotalCharges
        {
            get => Base.TotalChargesPerformed;
            set => Base.TotalChargesPerformed = value;
        }

        /// <summary>
        /// Gets the amount of charges remaining.
        /// </summary>
        /// <seealso cref="TotalCharges"/>
        public int RemainingCharges => 5 - Base.TotalChargesPerformed; // Hard coded

        /// <summary>
        /// Breaks the Jailbird.
        /// </summary>
        public void Break()
        {
            Base._broken = true;
            Base.SendRpc(JailbirdMessageType.Broken);
        }

        /// <summary>
        /// Clones current <see cref="JailBird"/> object.
        /// </summary>
        /// <returns> New <see cref="JailBird"/> object. </returns>
        public override Item Clone() => new JailBird()
        {
            MeleeDamage = MeleeDamage,
            ChargeDamage = ChargeDamage,
            TotalDamageDealt = TotalDamageDealt,
            TotalCharges = TotalCharges,
        };

        /// <summary>
        /// Returns the JailBird in a human readable format.
        /// </summary>
        /// <returns>A string containing JailBird-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}*";
    }
}