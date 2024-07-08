// -----------------------------------------------------------------------
// <copyright file="BreakingJailbirdEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Item
{
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Interfaces;
    using InventorySystem.Items.Jailbird;

    /// <summary>
    /// Contains all information before a jailbird breaks.
    /// </summary>
    public class BreakingJailbirdEventArgs : IItemEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BreakingJailbirdEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public BreakingJailbirdEventArgs(JailbirdItem item, bool isAllowed = true)
        {
            Jailbird = Item.Get(item).As<Jailbird>();
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Item Item => Jailbird;

        /// <summary>
        /// Gets a jailbird that is going to break.
        /// </summary>
        public Jailbird Jailbird { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}