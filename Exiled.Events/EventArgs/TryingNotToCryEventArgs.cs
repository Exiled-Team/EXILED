// -----------------------------------------------------------------------
// <copyright file="TryingNotToCryEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs {
    using System;

    using Exiled.API.Features;

    using Interactables.Interobjects.DoorUtils;

    using Scp096 = PlayableScps.Scp096;

    /// <summary>
    /// Contains all informations before SCP-096 tries not to cry.
    /// </summary>
    public class TryingNotToCryEventArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="TryingNotToCryEventArgs"/> class.
        /// </summary>
        /// <param name="scp096"><inheritdoc cref="Scp096"/></param>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="door"><inheritdoc cref="Door"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public TryingNotToCryEventArgs(Scp096 scp096, Player player, DoorVariant door, bool isAllowed = true) {
            Scp096 = scp096;
            Player = player;
            Door = Door.Get(door);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the SCP-096 instance.
        /// </summary>
        public Scp096 Scp096 { get; }

        /// <summary>
        /// Gets the player who is controlling SCP-096.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="Exiled.API.Features.Door"/> to be cried on.
        /// </summary>
        public Door Door { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-096 can try not to cry.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
