// -----------------------------------------------------------------------
// <copyright file="OpeningRemoteAdminEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;

    /// <summary>
    /// Contains all informations before changing item durability.
    /// </summary>
    public class OpeningRemoteAdminEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpeningRemoteAdminEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public OpeningRemoteAdminEventArgs(Player player, bool isAllowed = true)
        {
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="API.Features.Player"/> who's changing the <see cref="Firearm"/>'s durability.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Player"/> can open the remote admin.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
