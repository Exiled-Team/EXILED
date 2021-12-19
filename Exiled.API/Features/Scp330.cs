// -----------------------------------------------------------------------
// <copyright file="Scp330.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.Collections.Generic;

    using InventorySystem;
    using InventorySystem.Items.Usables.Scp330;

    using UnityEngine;

    /// <summary>
    /// A Wrapper class for <see cref="Scp330Bag"/>.
    /// </summary>
    public class Scp330
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp330"/> class.
        /// </summary>
        /// <param name="scp330Bag"><inheritdoc cref="Base"/></param>
        public Scp330(Scp330Bag scp330Bag)
        {
            Base = scp330Bag;
            Room = scp330Bag.GetComponentInParent<Room>();
        }

        /// <summary>
        /// Gets the base-game <see cref="Scp330Bag"/>.
        /// </summary>
        public Scp330Bag Base { get; }

        /// <summary>
        /// Gets the <see cref="Component.transform"/>.
        /// </summary>
        public Transform Transform => Base.transform;

        /// <summary>
        /// Gets the <see cref="Transform.position"/>.
        /// </summary>
        public Vector3 Position => Base.transform.position;

        /// <summary>
        /// Gets the <see cref="Transform.rotation"/>.
        /// </summary>
        public Quaternion Rotation => Base.transform.rotation;

        /// <summary>
        /// Gets the room the bag is in.
        /// </summary>
        public Room Room { get; }

        /// <summary>
        /// Gets a value indicating whether the bag can be equipped.
        /// </summary>
        public bool CanBeEquipped => Base.AllowEquip;

        /// <summary>
        /// Gets a value indicating whether the bag can be holstered.
        /// </summary>
        public bool CanBeHolstered => Base.AllowHolster;

        /// <summary>
        /// Gets all candies inside the bag.
        /// </summary>
        public List<CandyKindID> Candies => Base.Candies;

        /// <summary>
        /// Gets the <see cref="Player"/> who owns the bag.
        /// </summary>
        public Player Owner => Player.Get(Base.Owner);

        /// <summary>
        /// Gets or sets the unique serial number for the item.
        /// </summary>
        public ushort Serial { get => Base.ItemSerial; set => Base.ItemSerial = value; }

        /// <summary>
        /// Gets the <see cref="Scp330.Owner"/>s <see cref="Inventory"/>.
        /// </summary>
        public Inventory OwnerInventory => Base.OwnerInventory;

        /// <summary>
        /// Gets the remaining cooldown of the bag.
        /// </summary>
        public float RemainingCooldown => Base.RemainingCooldown;

        /// <summary>
        /// Gets the selected candy id.
        /// </summary>
        public int SelectedCandyId => Base.SelectedCandyId;

        /// <summary>
        /// Gets the weight of the bag.
        /// </summary>
        public float Weight => Base.Weight;

        /// <summary>
        /// Gets a value indicating whether an candy is selected.
        /// </summary>
        public bool IsCandySelected => Base.IsCandySelected;

        /// <summary>
        /// Refreshes the bag.
        /// </summary>
        public void ServerRefreshBag() => Base.ServerRefreshBag();

        /// <summary>
        /// Give a random Candy to the player.
        /// </summary>
        /// <param name="player">Player to give a random candy to.</param>
        public void GiveRandomCandy(Player player)
        {
            int randomCandyType = UnityEngine.Random.Range(1, 8);
            if (!player.HasItem(ItemType.SCP330))
                return;

            Base.TryAddSpecific((CandyKindID)randomCandyType);
            Base.ServerRefreshBag();
        }

        /// <summary>
        /// Gives a specific Candy to the player.
        /// </summary>
        /// <param name="player">Player to give the candy to.</param>
        /// <param name="candy"><see cref="CandyKindID"/>.</param>
        public void GiveCandy(Player player, CandyKindID candy)
        {
            if (!player.HasItem(ItemType.SCP330))
                return;

            Base.TryAddSpecific(candy);
            Base.ServerRefreshBag();
        }

        /// <summary>
        /// Gives an Scp-330 Bag to the player.
        /// </summary>
        /// <param name="player">Player to give a bag to.</param>
        public void GiveBag(Player player)
        {
            if (player.HasItem(ItemType.SCP330))
                return;
            player.AddItem(ItemType.SCP330);
        }

        /// <summary>
        /// Removes the Scp-330 Bag from the player.
        /// </summary>
        /// <param name="player">Player to remove the bag.</param>
        public void RemoveBag(Player player)
        {
            if (!player.HasItem(ItemType.SCP330))
                return;
            player.RemoveItem(new Items.Item(ItemType.SCP330), true);
        }
    }
}
