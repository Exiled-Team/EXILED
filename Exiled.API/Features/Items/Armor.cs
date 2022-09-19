// -----------------------------------------------------------------------
// <copyright file="Armor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Structs;

    using InventorySystem.Items.Armor;

    /// <summary>
    /// A wrapper class for <see cref="BodyArmor"/>.
    /// </summary>
    public class Armor : Item
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Armor"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="BodyArmor"/> class.</param>
        public Armor(BodyArmor itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Armor"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the armor.</param>
        internal Armor(ItemType type)
            : this((BodyArmor)Server.Host.Inventory.CreateItemInstance(type, false))
        {
        }

        /// <summary>
        /// Gets the <see cref="BodyArmor"/> that this class is encapsulating.
        /// </summary>
        public new BodyArmor Base { get; }

        /// <summary>
        /// Gets a value indicating whether this item is equippable.
        /// </summary>
        public bool Equippable
        {
            get => Base.AllowEquip;
        }

        /// <summary>
        /// Gets a value indicating whether this item is holsterable.
        /// </summary>
        public bool Holsterable
        {
            get => Base.AllowHolster;
        }

        /// <summary>
        /// Gets a value indicating whether or not this is a worn item.
        /// </summary>
        public bool IsWorn
        {
            get => Base.IsWorn;
        }

        /// <summary>
        /// Gets or sets the Weight of the armor.
        /// </summary>
        public new float Weight
        {
            get => Base.Weight;
            set => Base._weight = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not excess ammo should be removed when the armor is dropped.
        /// </summary>
        public bool RemoveExcessOnDrop
        {
            get => !Base.DontRemoveExcessOnDrop;
            set => Base.DontRemoveExcessOnDrop = !value;
        }

        /// <summary>
        /// Gets or sets how strong the helmet on the armor is.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">When trying to set the value below 0 or above 100.</exception>
        public int HelmetEfficacy
        {
            get => Base.HelmetEfficacy;
            set
            {
                if (value is <= 101 and >= 0)
                    Base.HelmetEfficacy = value;
                else
                    throw new ArgumentOutOfRangeException(nameof(HelmetEfficacy), "You can only set the efficacy value of armor to a value between 0 and 100.");
            }
        }

        /// <summary>
        /// Gets or sets how strong the vest on the armor is.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">When trying to set the value below 0 or above 100.</exception>
        public int VestEfficacy
        {
            get => Base.VestEfficacy;
            set
            {
                if (value is <= 101 and >= 0)
                    Base.VestEfficacy = value;
                else
                    throw new ArgumentOutOfRangeException(nameof(VestEfficacy), "You can only set the efficacy value of armor to a value between 0 and 100.");
            }
        }

        /// <summary>
        /// Gets or sets how much faster stamina will drain when wearing this armor.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">When attempting to set the value below 1 or above 2.</exception>
        public float StaminaUseMultiplier
        {
            get => Base.StaminaUseMultiplier;
            set
            {
                if (value is > 2f or < 1f)
                    throw new ArgumentOutOfRangeException(nameof(StaminaUseMultiplier), "You can only set the stamina use multiplier to a value between 1f and 2f.");
                Base.StaminaUseMultiplier = value;
            }
        }

        /// <summary>
        /// Gets or sets how much the users movement speed should be affected when wearing this armor. (higher values = slower movement).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">When attempting to set the value below 0 or above 1.</exception>
        public float MovementSpeedMultiplier
        {
            get => Base.MovementSpeedMultiplier;
            [Obsolete("The client would be desynchronized", true)]
            set
            {
                if (value is < 0.0f or > 1f)
                    throw new ArgumentOutOfRangeException(nameof(MovementSpeedMultiplier), "You can only set the movement speed multiplier to a value between 0 and 1.");
                Base.MovementSpeedMultiplier = value;
            }
        }

        /// <summary>
        /// Gets or sets how much worse <see cref="RoleType.ClassD"/> and <see cref="RoleType.Scientist"/>s are affected by wearing this armor.
        /// </summary>
        public float CivilianDownsideMultiplier
        {
            get => Base.CivilianClassDownsidesMultiplier;
            [Obsolete("The client would be desynchronized", true)]
            set => Base.CivilianClassDownsidesMultiplier = value;
        }

        /// <summary>
        /// Gets or sets the ammo limit of the wearer when using this armor.
        /// </summary>
        public IEnumerable<ArmorAmmoLimit> AmmoLimits
        {
            get => Base.AmmoLimits.Select(limit => (ArmorAmmoLimit)limit);

            set => Base.AmmoLimits = value.Select(limit => (BodyArmor.ArmorAmmoLimit)limit).ToArray();
        }

        /// <summary>
        /// Gets or sets the item caterory limit of the wearer when using this armor.
        /// </summary>
        public IEnumerable<BodyArmor.ArmorCategoryLimitModifier> CategoryLimits
        {
            get => Base.CategoryLimits;

            set => Base.CategoryLimits = value.ToArray();
        }

        /// <summary>
        /// Clones current <see cref="Armor"/> object.
        /// </summary>
        /// <returns> New <see cref="Armor"/> object. </returns>
        public override Item Clone()
        {
            Armor cloneableItem = new(Type)
            {
                Weight = Weight,
                StaminaUseMultiplier = StaminaUseMultiplier,
                RemoveExcessOnDrop = RemoveExcessOnDrop,
                CategoryLimits = CategoryLimits,
                AmmoLimits = AmmoLimits,
                VestEfficacy = VestEfficacy,
                HelmetEfficacy = HelmetEfficacy,
            };

            return cloneableItem;
        }
    }
}