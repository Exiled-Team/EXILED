// -----------------------------------------------------------------------
// <copyright file="EnragingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp096
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    using Scp096 = PlayableScps.Scp096;

    /// <summary>
    ///     Contains all information before SCP-096 gets enraged.
    /// </summary>
    public class EnragingEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EnragingEventArgs" /> class.
        /// </summary>
        /// <param name="scp096">
        ///     <inheritdoc cref="Scp096" />
        /// </param>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public EnragingEventArgs(Scp096 scp096, Player player, bool isAllowed = true)
        {
            Scp096 = scp096;
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the SCP-096 instance.
        /// </summary>
        public Scp096 Scp096 { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not SCP-096 can be enraged.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who's controlling SCP-096.
        /// </summary>
        public Player Player { get; }
    }
}