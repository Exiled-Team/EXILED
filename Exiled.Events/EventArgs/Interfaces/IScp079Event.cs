// -----------------------------------------------------------------------
// <copyright file="IScp079Event.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces
{
    using Exiled.API.Features.Roles;

    /// <summary>
    ///     Event args used for all <see cref="Scp079Role" /> related events.
    /// </summary>
    public interface IScp079Event : IPlayerEvent
    {
        /// <summary>
        ///     Gets the <see cref="Scp079Role" /> triggering the event.
        /// </summary>
        public Scp079Role Scp079 { get; }
    }
}