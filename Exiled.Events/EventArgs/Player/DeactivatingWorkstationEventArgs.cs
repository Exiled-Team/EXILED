// -----------------------------------------------------------------------
// <copyright file="DeactivatingWorkstationEventArgs.cs" company="Exiled Team">
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
    ///     Contains all information before deactivating a workstation.
    /// </summary>
    public class DeactivatingWorkstationEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DeactivatingWorkstationEventArgs" /> class.
        /// </summary>
        /// <param name="controller">
        ///     <inheritdoc cref="WorkstationController" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public DeactivatingWorkstationEventArgs(WorkstationController controller, bool isAllowed = true)
        {
            Player = Player.Get(controller._knownUser);
            WorkstationController = controller;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the <see cref="API.Features.Player" /> last user of the workstation.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets the workstation.
        /// </summary>
        public WorkstationController WorkstationController { get; }

        /// <summary>
        ///     Gets or sets the workstation status.
        /// </summary>
        public WorkstationStatus NewStatus { get; set; } = WorkstationStatus.PoweringDown;

        /// <summary>
        ///     Gets or sets a value indicating whether or not the workstation can be deactivated.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}