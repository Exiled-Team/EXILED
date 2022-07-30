// -----------------------------------------------------------------------
// <copyright file="IItemFirearmEvent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces.Item
{
    using Exiled.API.Features.Items;

    /// <summary>
    ///     Event args used for all <see cref="API.Features.Items.Firearm" /> related events.
    /// </summary>
    public interface IItemFirearmEvent : IExiledEvent
    {
        /// <summary>
        ///     Gets the <see cref="API.Features.Items.Firearm" /> triggering the event.
        /// </summary>
        public Firearm Firearm { get; }
    }
}
