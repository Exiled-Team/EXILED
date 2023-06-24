// -----------------------------------------------------------------------
// <copyright file="Scp330.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using System.Collections.Generic;

    using Exiled.API.Features.Pickups;
    using Exiled.API.Interfaces;

    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.Usables.Scp330;

    using UnityEngine;

    using Object = UnityEngine.Object;
    using Scp330Pickup = Exiled.API.Features.Pickups.Scp330Pickup;

    /// <summary>
    /// Candy enumeration status.
    /// </summary>
    public enum CandyAddStatus
    {
        /// <summary>
        /// If no candy was able to be added.
        /// </summary>
        NoCandyAdded,

        /// <summary>
        /// If at least one candy was added.
        /// </summary>
        SomeCandyAdded,

        /// <summary>
        /// If all candies provided were added.
        /// </summary>
        AllCandyAdded,
    }

    /// <summary>
    /// A wrapper class for SCP-330 bags.
    /// </summary>
    public partial class Scp330 : Usable, IWrapper<Scp330Bag>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp330"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="Scp330Bag"/> class.</param>
        public Scp330(Scp330Bag itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp330"/> class, as well as a new SCP-330 bag item.
        /// </summary>
        internal Scp330()
            : this((Scp330Bag)Server.Host.Inventory.CreateItemInstance(new(ItemType.SCP330, 0), false))
        {
        }

        /// <summary>
        /// Gets the <see cref="Scp330Bag"/> that this class is encapsulating.
        /// </summary>
        public new Scp330Bag Base { get; }

        /// <summary>
        /// Gets the <see cref="CandyKindID"/>s held in this bag.
        /// </summary>
        public IReadOnlyCollection<CandyKindID> Candies => Base.Candies.AsReadOnly();

        /// <summary>
        /// Gets or sets the exposed type. When set to a candy color, the bag will appear as that candy when dropped with the <see cref="Spawn"/> method. Setting it to <see cref="CandyKindID.None"/> results in it looking like a bag.
        /// </summary>
        public CandyKindID ExposedType { get; set; } = CandyKindID.None;

        /// <summary>
        /// Adds a specific candy to the bag.
        /// </summary>
        /// <param name="type">The <see cref="CandyKindID"/> to add.</param>
        /// <returns><see langword="true"/> if the candy was successfully added to the bag; otherwise, <see langword="false"/>.</returns>
        public bool AddCandy(CandyKindID type)
        {
            if (Base.TryAddSpecific(type))
            {
                Base.ServerRefreshBag();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a collection of candy's to a bag.
        /// </summary>
        /// <param name="candies">The <see cref="CandyKindID"/>'s to add.</param>
        /// <param name="status">The <see cref="CandyAddStatus"/>'s insertion status.</param>
        /// <returns> based on number of candy added. </returns>
        public int AddCandy(IEnumerable<CandyKindID> candies, out CandyAddStatus status)
        {
            int validCandy = 0;
            foreach (CandyKindID candy in candies)
            {
                if (!Base.TryAddSpecific(candy))
                {
                    status = validCandy is 0 ? CandyAddStatus.NoCandyAdded : CandyAddStatus.SomeCandyAdded;
                    return validCandy;
                }

                validCandy++;
            }

            status = CandyAddStatus.AllCandyAdded;
            return validCandy;
        }

        /// <summary>
        /// Removes a specific candy from the bag.
        /// </summary>
        /// <param name="type">The <see cref="CandyKindID"/> to be removed.</param>
        /// <param name="removeAll">Whether or not to only remove all matching candy. (If <see langword="true"/>, all candies of the given type are removed).</param>
        /// <returns>The total amount of candies that were removed from the bag.</returns>
        public int RemoveCandy(CandyKindID type, bool removeAll = false)
        {
            int amount = 0;

            while (Base.Candies.Contains(type))
            {
                Base.TryRemove(Base.Candies.IndexOf(type));

                amount++;

                if (!removeAll)
                    break;
            }

            return amount;
        }

        /// <summary>
        /// Removes all candy from the bag.
        /// </summary>
        /// <returns>The total amount of candies that were removed from the bag.</returns>
        public int RemoveAllCandy()
        {
            int amount = 0;
            for (int i = Base.Candies.Count; i > 0; i--)
            {
                Base.TryRemove(0);
                amount++;
            }

            return amount;
        }

        /// <summary>
        /// Drops candies from the bag.
        /// </summary>
        /// <param name="type">The <see cref="CandyKindID"/> of candies to drop.</param>
        /// <param name="dropAll">Whether or not to drop all candies matching the given type, or just one.</param>
        /// <param name="dropIndividual">Whether or not to drop all candies individually, or as a bag, when dropping more than one candy.</param>
        /// <param name="exposedType">The <see cref="ExposedType"/> to use, if the override is set to true.</param>
        /// <returns>a <see cref="IEnumerable{T}"/> of <see cref="Pickup"/>s generated by this method. *Can be empty!*.</returns>
        public IEnumerable<Scp330Pickup> DropCandy(CandyKindID type, bool dropAll = false, bool dropIndividual = false, CandyKindID exposedType = CandyKindID.None)
        {
            int count = 0;

            for (int i = 0; i < Base.Candies.Count; i++)
            {
                if (Base.Candies[i] == type)
                {
                    count++;

                    if (!dropAll)
                        break;
                }
            }

            List<Scp330Pickup> pickups = new();

            if (count > 1 && !dropIndividual)
            {
                ItemPickupBase ipb = Object.Instantiate(Base.PickupDropModel, Owner.Position, default);

                ipb.Info = new(Type, Weight, ItemSerialGenerator.GenerateNext());

                Scp330Pickup pickup = (Scp330Pickup)Pickup.Get(ipb);

                if (exposedType is not CandyKindID.None)
                    pickup.ExposedCandy = exposedType;
                for (int i = 0; i < count; i++)
                    pickup.Candies.Add(type);

                pickup.Base.InfoReceivedHook(default, pickup.Info);
                pickup.Scale = Scale;
                pickup.Spawn();
                pickups.Add(pickup);

                return pickups;
            }

            for (int i = 0; i < count; i++)
            {
                ItemPickupBase ipb = Object.Instantiate(Base.PickupDropModel, Owner.Position, default);

                ipb.Info = new(Type, Weight, ItemSerialGenerator.GenerateNext());

                Scp330Pickup pickup = (Scp330Pickup)Pickup.Get(ipb);

                if (exposedType is not CandyKindID.None)
                    pickup.ExposedCandy = exposedType;

                pickup.Candies.Add(type);
                pickup.Base.InfoReceivedHook(default, pickup.Info);
                pickup.Scale = Scale;
                pickup.Spawn();
                pickups.Add(pickup);
            }

            return pickups;
        }

        /// <summary>
        /// Creates the <see cref="Pickup"/> that based on this <see cref="Item"/>.
        /// </summary>
        /// <param name="position">The <see cref="Vector3"/> location to spawn it.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> rotation to give the item.</param>
        /// <param name="spawn">Whether the <see cref="Scp330Pickup"/> should be initially spawned.</param>
        /// <returns>The created <see cref="Pickup"/>.</returns>
        public override Pickup CreatePickup(Vector3 position, Quaternion rotation = default, bool spawn = true)
        {
            Scp330Pickup pickup = (Scp330Pickup)Pickup.Get(Object.Instantiate(Base.PickupDropModel, position, rotation));

            pickup.Info = new(Type, Weight, ItemSerialGenerator.GenerateNext());
            pickup.Candies = new(Base.Candies);
            pickup.ExposedCandy = ExposedType;
            pickup.Scale = Scale;

            if (spawn)
                pickup.Spawn();

            return pickup;
        }

        /// <summary>
        /// Clones current <see cref="Scp330"/> object.
        /// </summary>
        /// <returns> New <see cref="Scp330"/> object. </returns>
        public override Item Clone()
        {
            Scp330 cloneableItem = new()
            {
                ExposedType = ExposedType,
            };

            cloneableItem.AddCandy(Candies, out _);

            return cloneableItem;
        }

        /// <summary>
        /// Returns the SCP-330 in a human readable format.
        /// </summary>
        /// <returns>A string containing SCP-330 related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Candies}|";

        /// <summary>
        /// Clones current <see cref="Scp330"/> object.
        /// </summary>
        /// <param name="oldOwner">old <see cref="Item"/> owner.</param>
        /// <param name="newOwner">new <see cref="Item"/> owner.</param>
        internal override void ChangeOwner(Player oldOwner, Player newOwner)
        {
            Base.Owner = newOwner.ReferenceHub;
            Base.ServerRefreshBag();
        }
    }
}
