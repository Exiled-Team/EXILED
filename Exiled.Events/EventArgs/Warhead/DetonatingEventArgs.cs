// -----------------------------------------------------------------------
// <copyright file="DetonatingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Warhead
{
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before detonating the warhead.
    /// </summary>
    public class DetonatingEventArgs : IDeniableEvent
    {
        /// <inheritdoc/>
        public bool IsAllowed { get; set; } = true;
    }
}
