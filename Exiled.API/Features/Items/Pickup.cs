// -----------------------------------------------------------------------
// <copyright file="Pickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;

    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;

    using MapGeneration.Distributors;

    using Mirror;

    using UnityEngine;

    using Object = UnityEngine.Object;

    /// <summary>
    /// A wrapper class for <see cref="ItemPickupBase"/>.
    /// </summary>
    public class Pickup {
        /// <summary>
        /// A dictionary of all <see cref="ItemBase"/>'s that have been converted into <see cref="Item"/>.
        /// </summary>
        internal static readonly Dictionary<ItemPickupBase, Pickup> BaseToItem = new Dictionary<ItemPickupBase, Pickup>();

        private ushort id;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pickup"/> class.
        /// </summary>
        /// <param name="pickupBase"><inheritdoc cref="Base"/></param>
        public Pickup(ItemPickupBase pickupBase) {
            Base = pickupBase;
            Serial = pickupBase.NetworkInfo.Serial == 0 ? ItemSerialGenerator.GenerateNext() : pickupBase.NetworkInfo.Serial;
            BaseToItem.Add(pickupBase, this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pickup"/> class.
        /// </summary>
        /// <param name="type"><inheritdoc cref="Type"/></param>
        public Pickup(ItemType type) {
            if (!InventoryItemLoader.AvailableItems.TryGetValue(type, out ItemBase itemBase))
                return;

            Base = itemBase.PickupDropModel;
            Serial = itemBase.PickupDropModel.NetworkInfo.Serial;
            BaseToItem.Add(itemBase.PickupDropModel, this);
        }

        /// <summary>
        /// Gets the unique serial number for the item.
        /// </summary>
        public ushort Serial {
            get {
                if (id == 0) {
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
        public Vector3 Scale {
            get => Base.gameObject.transform.localScale;
            set {
                GameObject gameObject = Base.gameObject;
                NetworkServer.UnSpawn(gameObject);
                gameObject.transform.localScale = value;
                NetworkServer.Spawn(gameObject);
            }
        }

        /// <summary>
        /// Gets or sets the weight of the item.
        /// </summary>
        public float Weight {
            get => Base.NetworkInfo.Weight;
            set {
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
        public bool Locked {
            get => Base.NetworkInfo.Locked;
            set {
                PickupSyncInfo info = Base.Info;
                info.Locked = value;
                Base.NetworkInfo = info;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pickup is currently in use.
        /// </summary>
        public bool InUse {
            get => Base.NetworkInfo.InUse;
            set {
                PickupSyncInfo info = Base.Info;
                info.InUse = value;
                Base.NetworkInfo = info;
            }
        }

        /// <summary>
        /// Gets or sets the pickup position.
        /// </summary>
        public Vector3 Position {
            get => Base.NetworkInfo.Position;
            set {
                Base.Rb.position = value;
                Base.transform.position = value;
                NetworkServer.UnSpawn(Base.gameObject);
                NetworkServer.Spawn(Base.gameObject);

                Base.RefreshPositionAndRotation();
            }
        }

        /// <summary>
        /// Gets or sets the pickup rotation.
        /// </summary>
        public Quaternion Rotation {
            get => Base.NetworkInfo.Rotation.Value;
            set {
                Base.Rb.rotation = value;
                Base.transform.rotation = value;
                NetworkServer.UnSpawn(Base.gameObject);
                NetworkServer.Spawn(Base.gameObject);

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
            pickupBase == null ? null :
            BaseToItem.ContainsKey(pickupBase) ? BaseToItem[pickupBase] :
            new Pickup(pickupBase);

        /// <summary>
        /// Destroys the pickup.
        /// </summary>
        public void Destroy() => Base.DestroySelf();

        /// <inheritdoc/>
        public override string ToString() {
            return $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Position}| -{Locked}- ={InUse}=";
        }
    }
}
