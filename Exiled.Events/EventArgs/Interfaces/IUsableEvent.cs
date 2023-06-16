// -----------------------------------------------------------------------
// <copyright file="IUsableEvent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces
{
    using API.Features.Items;

    /// <summary>
    ///     Event args used for all <see cref="API.Features.Items.Usable" /> related events.
    /// </summary>
    public interface IUsableEvent : IItemEvent
    {
        /// <summary>
        ///     Gets the <see cref="API.Features.Items.Usable" /> triggering the event.
        /// </summary>
        public Usable Usable { get; }
    }
}