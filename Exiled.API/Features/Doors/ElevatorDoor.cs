// -----------------------------------------------------------------------
// <copyright file="ElevatorDoor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Doors
{
    using UnityEngine;

    using BaseDoor = Interactables.Interobjects.ElevatorDoor;

    public class ElevatorDoor : Door
    {
        public ElevatorDoor(BaseDoor door, Room room)
            : base(door, room)
        {
            Base = door;
        }

        public new BaseDoor Base { get; }

        public Vector3 TopPosition => Base.TopPosition;

        public Vector3 BottomPosition => Base.BottomPosition;

        public Vector3 TargetPosition => Base.TargetPosition;
    }
}