// -----------------------------------------------------------------------
// <copyright file="ICustomItemEvent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.Events.EventArgs.CustomItems
{
    using Exiled.API.Features.Items;
    using Exiled.CustomModules.API.Features.CustomItems;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Represents a marker interface used for all <see cref="API.Features.CustomItems.CustomItem"/> related events.
    /// </summary>
    public interface ICustomItemEvent : IItemEvent
    {
        /// <summary>
        /// Gets the <see cref="API.Features.CustomItems.CustomItem"/> in the player's inventory.
        /// </summary>
        public CustomItem CustomItem { get; }

        /// <summary>
        /// Gets the <see cref="API.Features.CustomItems.CustomItem"/>'s behaviour.
        /// </summary>
        public ItemBehaviour ItemBehaviour { get; }
    }
}