// -----------------------------------------------------------------------
// <copyright file="IScp559Event.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces
{
    /// <summary>
    /// Defines the base contract for all <see cref="Scp559Cake"/> related events.
    /// </summary>
    public interface IScp559Event : IExiledEvent
    {
        /// <summary>
        /// Gets the <see cref="API.Features.Scp559"/>.
        /// </summary>
        public API.Features.Scp559 Scp559 { get; }
    }
}