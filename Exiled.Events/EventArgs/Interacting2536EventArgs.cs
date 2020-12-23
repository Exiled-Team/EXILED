// -----------------------------------------------------------------------
// <copyright file="Interacting2536EventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;

    /// <summary>
    /// Contains all information before a player interacts with a <see cref="SCP2536_Present"/>.
    /// </summary>
    public class Interacting2536EventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Interacting2536EventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="present"><inheritdoc cref="Present"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public Interacting2536EventArgs(Player player, SCP2536_Present present, bool isAllowed = true)
        {
            Present = present;
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's interacting with the present.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="SCP2536_Present"/> instance.
        /// </summary>
        public SCP2536_Present Present { get; }

        /// <summary>
        /// Gets or sets the 2536 scenario (which determines the item outcome).
        /// </summary>
        public SCP_2536_Controller.Valid2536Scenario Scenario
        {
            get => Present.ThisPresentsScenario;
            set => Present.ThisPresentsScenario = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can open the present.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
