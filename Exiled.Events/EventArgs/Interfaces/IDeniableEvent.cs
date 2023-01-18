// -----------------------------------------------------------------------
// <copyright file="IDeniableEvent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces
{
    /// <summary>
    ///     Event args for events that can be allowed or denied.
    /// </summary>
    public interface IDeniableEvent : IExiledEvent
    {
        /// <summary>
        ///     Gets or sets a value indicating whether or not the event is allowed to continue.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}