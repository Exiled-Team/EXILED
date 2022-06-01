// -----------------------------------------------------------------------
// <copyright file="Pickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using System.Collections.Generic;
    using System.Linq;

    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;

    using Mirror;

    using UnityEngine;

    /// <summary>
    /// A wrapper class for <see cref="ItemPickupBase"/>.
    /// </summary>
    public class Pickup
    {
        /// <summary>
        /// A dictionary of all <see cref="ItemBase"/>'s that have been converted into <see cref="Items.Item"/>.
        /// </summary>
        internal static readonly Dictionary<ItemPickupBase, Pickup> BaseToItem = new();

        private ushort id;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="ItemPickupBase"/> class.</param>
        public Pickup(ItemPickupBase pickupBase)
        {
            Base = pickupBase;
            Serial = pickupBase.NetworkInfo.Serial == 0 ? ItemSerialGenerator.GenerateNext() : pickupBase.NetworkInfo.Serial;
            BaseToItem.Add(pickupBase, this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pickup"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        public Pickup(ItemType type)
        {
            if (!InventoryItemLoader.AvailableItems.TryGetValue(type, out ItemBase itemBase))
                return;

            Base = itemBase.PickupDropModel;
            Serial = itemBase.PickupDropModel.NetworkInfo.Serial;
            BaseToItem.Add(itemBase.PickupDropModel, this);
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Pickup"/> which contains all the <see cref="Pickup"/> instances.
        /// </summary>
        public static IEnumerable<Pickup> List => BaseToItem.Values;

        /// <summary>
        /// Gets the <see cref="UnityEngine.GameObject"/> of the Pickup.
        /// </summary>
        public GameObject GameObject => Base.gameObject;

        /// <summary>
        /// Gets the unique serial number for the item.
        /// </summary>
        public ushort Serial
        {
            get
            {
                if (id == 0)
                {
                    id = ItemSerialGenerator.GenerateNext();
                    Base.Info.Serial = id;
                    Base.NetworkInfo = Base.Info;
                }

                return id;
            }

            internal set => id = value;
        }

        /// <summary>
        /// Gets or sets the pickup's scale value.
        /// </summary>
        public Vector3 Scale
        {
            get => GameObject.transform.localScale;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                GameObject.transform.localScale = value;
                NetworkServer.Spawn(GameObject);
            }
        }

        /// <summary>
        /// Gets or sets the weight of the item.
        /// </summary>
        public float Weight
        {
            get => Base.NetworkInfo.Weight;
            set
            {
                Base.Info.Weight = value;
                Base.NetworkInfo = Base.Info;
            }
        }

        /// <summary>
        /// Gets the <see cref="ItemBase"/> of the item.
        /// </summary>
        public ItemPickupBase Base { get; }

        /// <summary>
        /// Gets the <see cref="ItemType"/> of the item.
        /// </summary>
        public ItemType Type => Base.NetworkInfo.ItemId;

        /// <summary>
        /// Gets or sets a value indicating whether the pickup is locked (can't be picked up).
        /// </summary>
        public bool Locked
        {
            get => Base.NetworkInfo.Locked;
            set
            {
                PickupSyncInfo info = Base.Info;
                info.Locked = value;
                Base.NetworkInfo = info;
            }
        }

        /// <summary>
        /// Gets or sets the previous owner of this item.
        /// </summary>
        public Player PreviousOwner
        {
            get => Player.Get(Base.PreviousOwner.Hub);
            set => Base.PreviousOwner = value.Footprint;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pickup is currently in use.
        /// </summary>
        public bool InUse
        {
            get => Base.NetworkInfo.InUse;
            set
            {
                PickupSyncInfo info = Base.Info;
                info.InUse = value;
                Base.NetworkInfo = info;
            }
        }

        /// <summary>
        /// Gets or sets the pickup position.
        /// </summary>
        public Vector3 Position
        {
            get => Base.NetworkInfo.Position;
            set
            {
                Base.Rb.position = value;
                Base.transform.position = value;

                Base.RefreshPositionAndRotation();
            }
        }

        /// <summary>
        /// Gets or sets the pickup rotation.
        /// </summary>
        public Quaternion Rotation
        {
            get => Base.NetworkInfo.Rotation.Value;
            set
            {
                Base.Rb.rotation = value;
                Base.transform.rotation = value;

                Base.RefreshPositionAndRotation();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this pickup is spawned.
        /// </summary>
        public bool Spawned { get; private set; }

        /// <summary>
        /// Gets an existing <see cref="Pickup"/> or creates a new instance of one.
        /// </summary>
        /// <param name="pickupBase">The <see cref="ItemPickupBase"/> to convert into a <see cref="Pickup"/>.</param>
        /// <returns>The <see cref="Pickup"/> wrapper for the given <see cref="ItemPickupBase"/>.</returns>
        public static Pickup Get(ItemPickupBase pickupBase) =>
            pickupBase is null ? null :
            BaseToItem.ContainsKey(pickupBase) ? BaseToItem[pickupBase] :
            new Pickup(pickupBase);

        /// <summary>
        /// Destroys the pickup.
        /// </summary>
        public void Destroy() => Base.DestroySelf();

        /// <summary>
        /// Returns the Pickup in a human readable format.
        /// </summary>
        /// <returns>A string containing Pickup-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Position}| -{Locked}- ={InUse}=";
    }
}
