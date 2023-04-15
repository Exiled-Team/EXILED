// -----------------------------------------------------------------------
// <copyright file="HurtingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp0492
{
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before zombie attacks.
    /// </summary>
    public class HurtingEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HurtingEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public HurtingEventArgs(ReferenceHub player, ReferenceHub target, bool isAllowed = true)
        {
            Player = API.Features.Player.Get(player);
            Target = API.Features.Player.Get(target);
            IsAllowed = isAllowed;
        }

        /// <inheritdoc />
        public API.Features.Player Player { get; }

        /// <summary>
        /// Gets the player who was attacked.
        /// </summary>
        public API.Features.Player Target { get; set; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; } = true;
    }
}
