// -----------------------------------------------------------------------
// <copyright file="IScp0492Event.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces
{
    using Exiled.API.Features.Roles;

    /// <summary>
    ///     Event args used for all <see cref="Scp0492Role" /> related events.
    /// </summary>
    public interface IScp0492Event : IPlayerEvent
    {
        /// <summary>
        ///     Gets the <see cref="Scp0492Role" /> triggering the event.
        /// </summary>
        public Scp0492Role Scp0492 { get; }
    }
}