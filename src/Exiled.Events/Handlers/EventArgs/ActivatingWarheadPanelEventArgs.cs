// -----------------------------------------------------------------------
// <copyright file="ActivatingWarheadPanelEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.EventArgs
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before a player activates the warhead panel.
    /// </summary>
    public class ActivatingWarheadPanelEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivatingWarheadPanelEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="permissions"><inheritdoc cref="Permissions"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ActivatingWarheadPanelEventArgs(Player player, List<string> permissions, bool isAllowed = true)
        {
            Player = player;
            Permissions = permissions;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's trying to activate the warhead panel.
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// Gets or sets a list of permissions, required to activate the warhead panel.
        /// </summary>
        public List<string> Permissions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}