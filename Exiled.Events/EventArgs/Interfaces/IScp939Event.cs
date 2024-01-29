// -----------------------------------------------------------------------
// <copyright file="IScp939Event.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces
{
    using Exiled.API.Features.Roles;

    /// <summary>
    ///     Event args used for all <see cref="Scp939Role" /> related events.
    /// </summary>
    public interface IScp939Event : IPlayerEvent
    {
        /// <summary>
        ///     Gets the <see cref="Scp939Role" /> triggering the event.
        /// </summary>
        public Scp939Role Scp939 { get; }
    }
}