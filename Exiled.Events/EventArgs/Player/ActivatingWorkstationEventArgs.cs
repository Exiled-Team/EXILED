// -----------------------------------------------------------------------
// <copyright file="ActivatingWorkstationEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Exiled.API.Enums;
    using Interfaces;
    using InventorySystem.Items.Firearms.Attachments;

    /// <summary>
    /// Contains all information before a player activates a workstation.
    /// </summary>
    public class ActivatingWorkstationEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivatingWorkstationEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="controller">
        /// <inheritdoc cref="WorkstationController" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public ActivatingWorkstationEventArgs(Player player, WorkstationController controller, bool isAllowed = true)
        {
            Player = player;
            Workstation = Workstation.Get(controller);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the workstation.
        /// </summary>
        public Workstation Workstation { get; }

        /// <summary>
        /// Gets or sets the workstation status.
        /// </summary>
        public WorkstationStatus NewStatus { get; set; } = WorkstationStatus.PoweringUp;

        /// <summary>
        /// Gets or sets a value indicating whether the workstation can be activated.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the player who's trying to activate the workstation.
        /// </summary>
        public Player Player { get; }
    }
}