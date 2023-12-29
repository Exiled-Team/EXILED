// -----------------------------------------------------------------------
// <copyright file="InteractingScp559EventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp559
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before a player interacts with SCP-559.
    /// </summary>
    public class InteractingScp559EventArgs : IScp559Event, IDeniableEvent, IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractingScp559EventArgs"/> class.
        /// </summary>
        /// <param name="scp559"><inheritdoc cref="Scp559"/></param>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public InteractingScp559EventArgs(Scp559 scp559, Player player, bool isAllowed = true)
        {
            Player = player;
            Scp559 = scp559;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Scp559 Scp559 { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }

        /// <inheritdoc/>
        public Player Player { get; }
    }
}