// -----------------------------------------------------------------------
// <copyright file="TogglingNoClipEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a player toggles the NoClip mode.
    /// </summary>
    public class TogglingNoClipEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TogglingNoClipEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public TogglingNoClipEventArgs(Player player, bool isAllowed = true)
        {
            Player = player;
            IsEnabled = player.ReferenceHub.characterClassManager.NoclipEnabled;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's toggling the NoClip mode.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets a value indicating whether or not the NoClip mode is enabled.
        /// </summary>
        public bool IsEnabled { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can toggle the NoClip mode.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
