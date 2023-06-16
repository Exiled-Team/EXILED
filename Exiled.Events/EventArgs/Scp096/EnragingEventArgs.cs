// -----------------------------------------------------------------------
// <copyright file="EnragingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp096
{
    using API.Features;

    using Interfaces;

    using PlayerRoles.PlayableScps.Scp096;

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
        /// <param name="initialDuration">
        ///     <inheritdoc cref="InitialDuration" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public EnragingEventArgs(Scp096Role scp096, Player player, float initialDuration, bool isAllowed = true)
        {
            Scp096 = scp096;
            Player = player;
            InitialDuration = initialDuration;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the SCP-096 instance.
        /// </summary>
        public Scp096Role Scp096 { get; }

        /// <inheritdoc />
        public Player Player { get; }

        /// <summary>
        ///     Gets or sets the SCP-096 rage initial duration.
        /// </summary>
        public float InitialDuration { get; set; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; }
    }
}