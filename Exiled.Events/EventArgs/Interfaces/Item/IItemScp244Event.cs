// -----------------------------------------------------------------------
// <copyright file="IItemScp244Event.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces.Item
{
    using Exiled.API.Features.Items;

    /// <summary>
    ///     Event args used for all <see cref="API.Features.Items.Scp244" /> related events.
    /// </summary>
    public interface IItemScp244Event : IExiledEvent
    {
        /// <summary>
        ///     Gets the <see cref="API.Features.Items.Scp244" /> triggering the event.
        /// </summary>
        public Scp244 Scp244 { get; }
    }
}
