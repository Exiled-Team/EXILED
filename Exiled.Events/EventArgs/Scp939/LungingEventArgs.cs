// -----------------------------------------------------------------------
// <copyright file="LungingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp939
{
    using System;

    using API.Features;
    using Interfaces;

    /// <summary>
    ///     Contains all information before SCP-939 uses its lunge ability.
    /// </summary>
    public class LungingEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LungingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        public LungingEventArgs(ReferenceHub player, bool isAllowed = true)
        {
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the player who's controlling SCP-939.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not SCP-939 can lunge.
        /// </summary>
        [Obsolete("Deprecated.")]
        public bool IsAllowed { get; set; }
    }
}
