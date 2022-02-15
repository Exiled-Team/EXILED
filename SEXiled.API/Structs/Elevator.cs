// -----------------------------------------------------------------------
// <copyright file="Elevator.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.API.Structs
{
    using UnityEngine;

    /// <summary>
    /// The in-game elevator.
    /// </summary>
    public struct Elevator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Elevator"/> struct.
        /// </summary>
        /// <param name="elevator">The base <see cref="Lift.Elevator"/> class.</param>
        public Elevator(Lift.Elevator elevator) => Base = elevator;

        /// <summary>
        /// Gets the base <see cref="Lift.Elevator"/>.
        /// </summary>
        public Lift.Elevator Base { get; }

        /// <summary>
        /// Gets the elevator's target.
        /// </summary>
        public Transform Target => Base.target;

        /// <summary>
        /// Gets or sets the elevator's position.
        /// </summary>
        public Vector3 Position
        {
            get => Base.GetPosition();
            set
            {
                Base.target.transform.position = value;
                Base.SetPosition();
            }
        }

        /// <summary>
        /// Compares two operands: <see cref="Elevator"/> and <see cref="Elevator"/>.
        /// </summary>
        /// <param name="left">The first <see cref="Elevator"/> to compare.</param>
        /// <param name="right">The second <see cref="Elevator"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(Elevator left, Elevator right) => left.Base.Equals(right.Base);

        /// <summary>
        /// Compares two operands: <see cref="Elevator"/> and <see cref="Elevator"/>.
        /// </summary>
        /// <param name="left">The first <see cref="Elevator"/> to compare.</param>
        /// <param name="right">The second <see cref="Elevator"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(Elevator left, Elevator right) => !left.Base.Equals(right.Base);

        /// <inheritdoc/>
        public override bool Equals(object obj) => Base.Equals(obj);

        /// <inheritdoc/>
        public override int GetHashCode() => Base.GetHashCode();

        /// <inheritdoc/>
        public override string ToString() => Base.ToString();
    }
}
