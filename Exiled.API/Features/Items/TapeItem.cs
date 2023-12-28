﻿// -----------------------------------------------------------------------
// <copyright file="TapeItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Exiled.API.Interfaces;

    using BaseTape = InventorySystem.Items.FlamingoTapePlayer.TapeItem;

    /// <summary>
    /// A wrapper for <see cref="BaseTape"/>.
    /// </summary>
    public class TapeItem : Item, IWrapper<BaseTape>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TapeItem"/> class.
        /// </summary>
        /// <param name="itemBase"><inheritdoc cref="Base"/></param>
        public TapeItem(BaseTape itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TapeItem"/> class.
        /// </summary>
        /// <param name="type"><see cref="ItemType.Tape"/>.</param>
        internal TapeItem(ItemType type)
            : base(type)
        {
        }

        /// <inheritdoc/>
        public new BaseTape Base { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not item is in use now.
        /// </summary>
        public bool IsUsing
        {
            get => Base._using;
            set => Base._using = value;
        }

        /// <summary>
        /// Gets or sets a time when item was equipped.
        /// </summary>
        public double EquipTime
        {
            get => Base._equipTime;
            set => Base._equipTime = value;
        }

        /// <summary>
        /// Gets or sets a time before item will be destroyed and effects will be applied.
        /// </summary>
        public float Remaining
        {
            get => Base._remainingDestroy;
            set => Base._remainingDestroy = value;
        }

        /// <summary>
        /// Gets a value indicating whether or nor item can be holstered.
        /// </summary>
        public bool AllowHolster => Base.AllowHolster;

        /// <summary>
        /// Gets a value indicating whether or not item can be equipped.
        /// </summary>
        public bool AllowEquip => Base.AllowEquip;
    }
}