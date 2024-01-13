// -----------------------------------------------------------------------
// <copyright file="GrantingGiftEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp2536
{
    using Christmas.Scp2536;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before player receives a gift from SCP-2536.
    /// </summary>
    public class GrantingGiftEventArgs : IDeniableEvent, IPlayerEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrantingGiftEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="gift"><inheritdoc cref="Gift"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public GrantingGiftEventArgs(Player player, Scp2536GiftBase gift, bool isAllowed = true)
        {
            Player = player;
            Gift = gift;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a gift that will be granted to a <see cref="Player"/>.
        /// </summary>
        public Scp2536GiftBase Gift { get; set; }
    }
}