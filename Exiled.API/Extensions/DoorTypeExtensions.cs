// -----------------------------------------------------------------------
// <copyright file="DoorTypeExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using Exiled.API.Enums;

    /// <summary>
    /// A set of extensions for <see cref="DoorType"/>.
    /// </summary>
    public static class DoorTypeExtensions
    {
        /// <summary>
        /// Checks if a <see cref="DoorType">door type</see> is a gate.
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns whether the <see cref="DoorType"/> is a gate or not.</returns>
        public static bool IsGate(this DoorType door) => door is DoorType.GateA or DoorType.GateB or DoorType.Scp914Gate or
            DoorType.Scp049Gate or DoorType.GR18Gate or DoorType.SurfaceGate or DoorType.Scp173Gate;

        /// <summary>
        /// Checks if a <see cref="DoorType">door type</see> is a checkpoint.
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns whether the <see cref="DoorType"/> is a checkpoint or not.</returns>
        public static bool IsCheckpoint(this DoorType door) => door is DoorType.CheckpointLczA or DoorType.CheckpointLczB or DoorType.CheckpointEzHczA or DoorType.CheckpointEzHczB;

        /// <summary>
        /// Checks if a <see cref="DoorType">door type</see> is an elevator.
        /// </summary>
        /// <param name="door">The door to be checked.</param>
        /// <returns>Returns whether the <see cref="DoorType"/> is an elevator or not.</returns>
        public static bool IsElevator(this DoorType door) => door is DoorType.ElevatorGateA or DoorType.ElevatorGateB
            or DoorType.ElevatorLczA or DoorType.ElevatorLczB or DoorType.ElevatorNuke or DoorType.ElevatorScp049;
    }
}