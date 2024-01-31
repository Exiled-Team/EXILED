// -----------------------------------------------------------------------
// <copyright file="CustomModule.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features
{
    using System;

    using Exiled.API.Features.Core;

    /// <summary>
    /// Represents a marker class for custom modules.
    /// </summary>
    public abstract class CustomModule : TypeCastObject<CustomModule>, IEquatable<CustomModule>, IEquatable<uint>
    {
        /// <summary>
        /// Gets the <see cref="CustomModule"/>'s name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets or sets the <see cref="CustomModule"/>'s id.
        /// </summary>
        public abstract uint Id { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomModule"/> is enabled.
        /// </summary>
        public abstract bool IsEnabled { get; }

        /// <summary>
        /// Compares two operands: <see cref="CustomModule"/> and <see cref="object"/>.
        /// </summary>
        /// <param name="left">The <see cref="CustomModule"/> to compare.</param>
        /// <param name="right">The <see cref="object"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(CustomModule left, object right) => left is null ? right is null : left.Equals(right);

        /// <summary>
        /// Compares two operands: <see cref="object"/> and <see cref="CustomModule"/>.
        /// </summary>
        /// <param name="left">The <see cref="object"/> to compare.</param>
        /// <param name="right">The <see cref="CustomModule"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator ==(object left, CustomModule right) => right == left;

        /// <summary>
        /// Compares two operands: <see cref="CustomModule"/> and <see cref="object"/>.
        /// </summary>
        /// <param name="left">The <see cref="object"/> to compare.</param>
        /// <param name="right">The <see cref="CustomModule"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(CustomModule left, object right) => !(left == right);

        /// <summary>
        /// Compares two operands: <see cref="object"/> and <see cref="CustomModule"/>.
        /// </summary>
        /// <param name="left">The left <see cref="object"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomModule"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(object left, CustomModule right) => !(left == right);

        /// <summary>
        /// Compares two operands: <see cref="CustomModule"/> and <see cref="CustomModule"/>.
        /// </summary>
        /// <param name="left">The left <see cref="CustomModule"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomModule"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(CustomModule left, CustomModule right) => left is null ? right is null : left.Equals(right);

        /// <summary>
        /// Compares two operands: <see cref="CustomModule"/> and <see cref="CustomModule"/>.
        /// </summary>
        /// <param name="left">The left <see cref="CustomModule"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomModule"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(CustomModule left, CustomModule right) => !(left.Id == right.Id);

        /// <summary>
        /// Determines whether the provided id is equal to the current object.
        /// </summary>
        /// <param name="id">The id to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public bool Equals(uint id) => Id == id;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="cr">The custom module to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public bool Equals(CustomModule cr) => cr && cr.GetType().IsAssignableFrom(GetType()) && (ReferenceEquals(this, cr) || Id == cr.Id);

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is CustomModule other)
                return Equals(other);

            try
            {
                return Equals((uint)obj);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a the 32-bit signed hash code of the current object instance.
        /// </summary>
        /// <returns>The 32-bit signed hash code of the current object instance.</returns>
        public override int GetHashCode() => base.GetHashCode();
    }
}