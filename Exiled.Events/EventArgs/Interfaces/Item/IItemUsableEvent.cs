// -----------------------------------------------------------------------
// <copyright file="IItemUsableEvent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces.Item
{
    using Exiled.API.Features.Items;

    /// <summary>
    ///     Event args used for all <see cref="API.Features.Items.Usable" /> related events.
    /// </summary>
    public interface IItemUsableEvent : IExiledEvent
    {
        /// <summary>
        ///     Gets the <see cref="API.Features.Items.Usable" /> triggering the event.
        /// </summary>
        public Usable Usable { get; }
    }
}
