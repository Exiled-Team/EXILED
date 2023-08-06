// -----------------------------------------------------------------------
// <copyright file="ElevatorDoor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Doors
{
    using System.Linq;

    using Exiled.API.Enums;
    using Interactables.Interobjects;

    /// <summary>
    /// Represents an elevator door.
    /// </summary>
    public class ElevatorDoor : Door
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElevatorDoor"/> class.
        /// </summary>
        /// <param name="door">The base <see cref="Interactables.Interobjects.ElevatorDoor"/> for this door.</param>
        /// <param name="room">The <see cref="Room"/> for this door.</param>
        public ElevatorDoor(Interactables.Interobjects.ElevatorDoor door, Room room)
            : base(door, room)
        {
            Base = door;
            Lift = Lift.Get(x => x.Group == Group).FirstOrDefault();
        }

        /// <summary>
        /// Gets the base <see cref="Interactables.Interobjects.ElevatorDoor"/>.
        /// </summary>
        public new Interactables.Interobjects.ElevatorDoor Base { get; }

        /// <summary>
        /// Gets the group for elevator for this door.
        /// </summary>
        public ElevatorManager.ElevatorGroup Group => Base.Group;

        /// <summary>
        /// Gets the type according to <see cref="Group"/>.
        /// </summary>
        public ElevatorType ElevatorType => Group switch
        {
            ElevatorManager.ElevatorGroup.Scp049 => ElevatorType.Scp049,
            ElevatorManager.ElevatorGroup.GateA => ElevatorType.GateA,
            ElevatorManager.ElevatorGroup.GateB => ElevatorType.GateB,
            ElevatorManager.ElevatorGroup.LczA01 or ElevatorManager.ElevatorGroup.LczA02 => ElevatorType.LczA,
            ElevatorManager.ElevatorGroup.LczB01 or ElevatorManager.ElevatorGroup.LczB02 => ElevatorType.LczB,
            ElevatorManager.ElevatorGroup.Nuke => ElevatorType.Nuke,
            _ => ElevatorType.Unknown,
        };

        /// <summary>
        /// Gets the target panel for this lift.
        /// </summary>
        public ElevatorPanel Panel => Base.TargetPanel;

        /// <summary>
        /// Gets the <see cref="Lift"/> associated with the elevator door.
        /// </summary>
        public Lift Lift { get; }

        /// <summary>
        /// Returns the Door in a human-readable format.
        /// </summary>
        /// <returns>A string containing Door-related data.</returns>
        public override string ToString() => $"{base.ToString()} !{ElevatorType}!";
    }
}