// -----------------------------------------------------------------------
// <copyright file="ActivatingWorkstationEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;

    using Interfaces;

    using InventorySystem.Items.Firearms.Attachments;

    using static InventorySystem.Items.Firearms.Attachments.WorkstationController;

    /// <summary>
    ///     Contains all information before a player activates a workstation.
    /// </summary>
    public class ActivatingWorkstationEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivatingWorkstationEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="controller">
        ///     <inheritdoc cref="WorkstationController" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public ActivatingWorkstationEventArgs(Player player, WorkstationController controller, bool isAllowed = true)
        {
            Player = player;
            WorkstationController = controller;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the workstation.
        /// </summary>
        public WorkstationController WorkstationController { get; }

        /// <summary>
        ///     Gets or sets the workstation status.
        /// </summary>
        public WorkstationStatus NewStatus { get; set; } = WorkstationStatus.PoweringUp;

        /// <inheritdoc />
        public bool IsAllowed { get; set; }

        /// <inheritdoc />
        public Player Player { get; }
    }
}