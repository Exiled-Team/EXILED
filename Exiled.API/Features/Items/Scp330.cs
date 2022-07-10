// -----------------------------------------------------------------------
// <copyright file="Scp330.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using System.Collections.Generic;

    using InventorySystem.Items.Usables.Scp330;

    using MEC;

    using Mirror;

    using UnityEngine;

    using Object = UnityEngine.Object;

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
    public partial class Scp330 : Usable
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
            : this((Scp330Bag)Server.Host.Inventory.CreateItemInstance(ItemType.SCP330, false))
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
        /// <returns> <see cref="CandyAddStatus"/> based on insertion status. </returns>
        public CandyAddStatus AddCandy(IEnumerable<CandyKindID> candies)
        {
            bool addedCandy = false;
            foreach(CandyKindID candy in candies)
            {
                if (!Base.TryAddSpecific(candy))
                {
                    return addedCandy ? CandyAddStatus.SomeCandyAdded : CandyAddStatus.NoCandyAdded;
                }

                addedCandy = true;
            }

            return CandyAddStatus.AllCandyAdded;
        }

        /// <summary>
        /// Removes a specific candy from the bag.
        /// </summary>
        /// <param name="type">The <see cref="CandyKindID"/> to be removed.</param>
        /// <param name="removeAll">Whether or not to only remove all matching candy. (If <see langword="true"/>, all candies of the given type are removed).</param>
        /// <returns>The total amount of candies that were dropped from the bag.</returns>
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
        /// Drops candies from the bag.
        /// </summary>
        /// <param name="type">The <see cref="CandyKindID"/> of candies to drop.</param>
        /// <param name="dropAll">Whether or not to drop all candies matching the given type, or just one.</param>
        /// <param name="dropIndividual">Whether or not to drop all candies individually, or as a bag, when dropping more than one candy.</param>
        /// <param name="overrideExposedType">Whether or not to override the exposed type of the candy dropped.</param>
        /// <param name="exposedType">The <see cref="ExposedType"/> to use, if the override is set to true.</param>
        /// <returns>a <see cref="IEnumerable{T}"/> of <see cref="Pickup"/>s generated by this method. *Can be empty!*.</returns>
        public IEnumerable<Pickup> DropCandy(CandyKindID type, bool dropAll = false, bool dropIndividual = false, bool overrideExposedType = false, CandyKindID exposedType = CandyKindID.None)
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

            List<Pickup> pickups = new();

            if (count > 1 && !dropIndividual)
            {
                Scp330Pickup ipb = (Scp330Pickup)Object.Instantiate(Base.PickupDropModel, Owner.Position, default);
                ipb.NetworkExposedCandy = overrideExposedType ? exposedType : CandyKindID.None;
                for (int i = 0; i < count; i++)
                    ipb.StoredCandies.Add(type);
                NetworkServer.Spawn(ipb.gameObject);
                ipb.InfoReceived(default, Base.PickupDropModel.NetworkInfo);
                Pickup pickup = Pickup.Get(ipb);
                pickup.Scale = Scale;
                pickups.Add(pickup);
                return pickups;
            }

            for (int i = 0; i < count; i++)
            {
                Scp330Pickup ipb = (Scp330Pickup)Object.Instantiate(Base.PickupDropModel, Owner.Position, default);
                ipb.NetworkExposedCandy = overrideExposedType ? exposedType : CandyKindID.None;
                NetworkServer.Spawn(ipb.gameObject);
                ipb.InfoReceived(default, Base.PickupDropModel.NetworkInfo);
                Pickup pickup = Pickup.Get(ipb);
                pickup.Scale = Scale;
                pickups.Add(pickup);
            }

            return pickups;
        }

        /// <summary>
        /// Spawns the bag, allowing to override the exposed type.
        /// </summary>
        /// <param name="position">The <see cref="Vector3"/> location to spawn it.</param>
        /// <param name="rotation">The <see cref="Quaternion"/> rotation to give the item.</param>
        /// <param name="overrideExposedType">Whether or not to use the <see cref="ExposedType"/> value or the default value.</param>
        /// <returns>The <see cref="Pickup"/> spawned.</returns>
        public Pickup Spawn(Vector3 position, Quaternion rotation = default, bool overrideExposedType = false)
        {
            Base.PickupDropModel.Info.ItemId = Type;
            Base.PickupDropModel.Info.Position = position;
            Base.PickupDropModel.Info.Weight = Weight;
            Base.PickupDropModel.Info.Rotation = new LowPrecisionQuaternion(rotation);
            Base.PickupDropModel.NetworkInfo = Base.PickupDropModel.Info;

            Scp330Pickup ipb = (Scp330Pickup)Object.Instantiate(Base.PickupDropModel, position, rotation);

            if (overrideExposedType)
                ipb.NetworkExposedCandy = ExposedType;

            NetworkServer.Spawn(ipb.gameObject);
            ipb.InfoReceived(default, Base.PickupDropModel.NetworkInfo);
            Pickup pickup = Pickup.Get(ipb);
            pickup.Scale = Scale;
            return pickup;
        }

        /// <summary>
        /// Returns the SCP-330 in a human readable format.
        /// </summary>
        /// <returns>A string containing SCP-330 related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Candies}|";

        /// <summary>
        /// Clones current <see cref="Scp330"/> object.
        /// </summary>
        /// <returns> New <see cref="Scp330"/> object. </returns>
        public override Item Clone()
        {
            Scp330 cloneableItem = new();
            cloneableItem.ExposedType = ExposedType;
            cloneableItem.AddCandy(Candies);
            return cloneableItem;
        }
    }
}
