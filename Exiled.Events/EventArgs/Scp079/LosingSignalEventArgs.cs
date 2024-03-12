// -----------------------------------------------------------------------
// <copyright file="LosingSignalEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp079
{
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before SCP-079 loses its signal.
    /// </summary>
    public class LosingSignalEventArgs : IScp079Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LosingSignalEventArgs" /> class.
        /// </summary>
        /// <param name="hub"><inheritdoc cref="Player" /></param>
        /// <param name="duration">The duration of the signal loss.</param>
        public LosingSignalEventArgs(ReferenceHub hub, double duration)
        {
            Player = Player.Get(hub);
            Scp079 = Player.Role.As<Scp079Role>();
            Duration = duration;
        }

        /// <summary>
        /// Gets the player who's controlling SCP-079.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp079Role Scp079 { get; }

        /// <summary>
        /// Gets or sets the duration that SCP-079 should lose its signal for.
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-079 is allowed to lose its signal here.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}