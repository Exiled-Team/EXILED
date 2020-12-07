// -----------------------------------------------------------------------
// <copyright file="ChangingCameraEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a player's intercom mute status is changed.
    /// </summary>
    public class ChangingCameraEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingCameraEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="newCamera"><inheritdoc cref="Camera"/></param>
        /// <param name="manaCost"><inheritdoc cref="APCost"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ChangingCameraEventArgs(Player player, Camera079 newCamera, float manaCost, bool isAllowed = true)
        {
            Player = player;
            Camera = newCamera;
            APCost = manaCost;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's being intercom muted/unmuted.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the camera SCP-079 will be moved to.
        /// </summary>
        public Camera079 Camera { get; set; }

        /// <summary>
        /// Gets or sets the amount of AP that will be required to switch cameras.
        /// </summary>
        public float APCost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-079 can switch cameras.
        /// Defaults to a value describing whether or not SCP-079 has enough AP to switch.
        /// Can be set to true to allow a switch regardless of SCP-079's AP amount.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
