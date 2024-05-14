// -----------------------------------------------------------------------
// <copyright file="Workstation.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.Collections.Generic;
    using System.Diagnostics;

    using Exiled.API.Enums;
    using Exiled.API.Features.Core;
    using Exiled.API.Interfaces;
    using Interactables;
    using InventorySystem.Items.Firearms.Attachments;
    using MapGeneration;

    /// <summary>
    /// A wrapper for workstation.
    /// </summary>
    public class Workstation : GameEntity, IWrapper<WorkstationController>
    {
        /// <summary>
        /// A <see cref="Dictionary{TKey,TValue}"/> with <see cref="WorkstationController"/> and <see cref="Workstation"/>.
        /// </summary>
        internal static readonly Dictionary<WorkstationController, Workstation> BaseToWrapper = new();

        private Room room;

        /// <summary>
        /// Initializes a new instance of the <see cref="Workstation"/> class.
        /// </summary>
        /// <param name="controller">The <see cref="WorkstationController"/> instance.</param>
        public Workstation(WorkstationController controller)
            : base(controller.gameObject)
        {
            Base = controller;

            BaseToWrapper.Add(controller, this);
        }

        /// <summary>
        /// Gets a list with all <see cref="Workstation"/>.
        /// </summary>
        public static new IReadOnlyCollection<Workstation> List => BaseToWrapper.Values;

        /// <inheritdoc/>
        public WorkstationController Base { get; }

        /// <summary>
        /// Gets or sets a current working status.
        /// </summary>
        public WorkstationStatus CurrentStatus
        {
            get => (WorkstationStatus)Base.Status;
            set => Base.NetworkStatus = (byte)value;
        }

        /// <summary>
        /// Gets a current collider with which player need to interact.
        /// </summary>
        public InteractableCollider CurrentCollider => Base._activateCollder;

        /// <summary>
        /// Gets a stopwatch which is used in counting all cooldowns.
        /// </summary>
        public Stopwatch Stopwatch => Base._serverStopwatch;

        /// <summary>
        /// Gets a last interacted with workstation player.
        /// </summary>
        public Player KnownPlayer => Player.Get(Base._knownUser);

        /// <summary>
        /// Gets a room where this workstation is located.
        /// </summary>
        public Room Room => room ??= Room.Get(Transform.GetComponentInParent<RoomIdentifier>());

        /// <summary>
        /// Gets a workstation from it's base-game analog.
        /// </summary>
        /// <param name="workstationController">The <see cref="WorkstationController"/> instance.</param>
        /// <returns>The <see cref="Workstation"/>.</returns>
        public static Workstation Get(WorkstationController workstationController) => BaseToWrapper.TryGetValue(workstationController, out Workstation workstation) ? workstation : new(workstationController);

        /// <summary>
        /// Interacts with workstation.
        /// </summary>
        /// <param name="player">Player who interact.</param>
        public void Interact(Player player) => Base.ServerInteract(player.ReferenceHub, CurrentCollider.ColliderId);
    }
}