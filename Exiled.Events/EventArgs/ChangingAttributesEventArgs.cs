// -----------------------------------------------------------------------
// <copyright file="ChangingAttributesEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Enums;

    /// <summary>
    /// Contains all informations before changing item attributes.
    /// </summary>
    public class ChangingAttributesEventArgs : EventArgs
    {
        private Inventory.SyncItemInfo newItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingAttributesEventArgs"/> class.
        /// </summary>
        /// <param name="oldItem"><inheritdoc cref="OldItem"/></param>
        /// <param name="newItem"><inheritdoc cref="NewItem"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ChangingAttributesEventArgs(Inventory.SyncItemInfo oldItem, Inventory.SyncItemInfo newItem, bool isAllowed = true)
        {
            OldItem = oldItem;
            NewItem = newItem;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the old item.
        /// </summary>
        public Inventory.SyncItemInfo OldItem { get; }

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        public Inventory.SyncItemInfo NewItem
        {
            get => newItem;
            set => newItem = value;
        }

        /// <summary>
        /// Gets or sets the item type.
        /// </summary>
        public ItemType NewId
        {
            get => newItem.id;
            set => newItem.id = value;
        }

        /// <summary>
        /// Gets or sets the new item durability.
        /// </summary>
        public float NewDurability
        {
            get => newItem.durability;
            set => newItem.durability = value;
        }

        /// <summary>
        /// Gets or sets the new item unique id.
        /// </summary>
        public int NewUniqueId
        {
            get => newItem.uniq;
            set => newItem.uniq = value;
        }

        /// <summary>
        /// Gets or sets the new item sight attachment.
        /// </summary>
        public SightType NewSight
        {
            get => (SightType)newItem.modSight;
            set => newItem.modSight = (int)value;
        }

        /// <summary>
        /// Gets or sets the new item barrel attachment.
        /// </summary>
        public BarrelType NewBarrel
        {
            get => (BarrelType)newItem.modBarrel;
            set => newItem.modBarrel = (int)value;
        }

        /// <summary>
        /// Gets or sets the new item other attachment.
        /// </summary>
        public OtherType NewOther
        {
            get => (OtherType)newItem.modOther;
            set => newItem.modOther = (int)value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
