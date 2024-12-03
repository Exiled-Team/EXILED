// -----------------------------------------------------------------------
// <copyright file="ElevatorDoor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Doors
{
    using System.Collections.Generic;
    using System.Linq;

    using Enums;
    using Interactables.Interobjects;
    using UnityEngine;

    /// <summary>
    /// Represents an elevator door.
    /// </summary>
    public class ElevatorDoor : BasicDoor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElevatorDoor"/> class.
        /// </summary>
        /// <param name="door">The base <see cref="Interactables.Interobjects.ElevatorDoor"/> for this door.</param>
        /// <param name="room">The <see cref="Room"/> for this door.</param>
        internal ElevatorDoor(Interactables.Interobjects.ElevatorDoor door, List<Room> room)
            : base(door, room)
        {
            Base = door;
            Lift = Lift.Get(x => x.Group == Group).FirstOrDefault();
            Panel = Object.FindObjectsOfType<ElevatorPanel>().FirstOrDefault(x => x._door == door);
        }

        /// <summary>
        /// Gets the base <see cref="Interactables.Interobjects.ElevatorDoor"/>.
        /// </summary>
        public new Interactables.Interobjects.ElevatorDoor Base { get; }

        /// <summary>
        /// Gets the <see cref="ElevatorGroup"/> that this door's <see cref="Lift"/> belongs to.
        /// </summary>
        public ElevatorGroup Group => Base.Group;

        /// <summary>
        /// Gets the type according to <see cref="Group"/>.
        /// </summary>
        public ElevatorType ElevatorType => Group switch
        {
            ElevatorGroup.Scp049 => ElevatorType.Scp049,
            ElevatorGroup.GateA => ElevatorType.GateA,
            ElevatorGroup.GateB => ElevatorType.GateB,
            ElevatorGroup.LczA01 or ElevatorGroup.LczA02 => ElevatorType.LczA,
            ElevatorGroup.LczB01 or ElevatorGroup.LczB02 => ElevatorType.LczB,
            ElevatorGroup.Nuke01 => ElevatorType.Nuke1,
            ElevatorGroup.Nuke02 => ElevatorType.Nuke2,
            _ => ElevatorType.Unknown,
        };

        /// <summary>
        /// Gets the target panel for this lift.
        /// </summary>
        public ElevatorPanel Panel { get; }

        /// <summary>
        /// Gets the <see cref="Lift"/> associated with this elevator door.
        /// </summary>
        public Lift Lift { get; }

        /// <summary>
        /// Returns the Door in a human-readable format.
        /// </summary>
        /// <returns>A string containing Door-related data.</returns>
        public override string ToString() => $"{base.ToString()} !{ElevatorType}!";
    }
}