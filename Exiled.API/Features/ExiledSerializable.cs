// -----------------------------------------------------------------------
// <copyright file="ExiledSerializable.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;

    /// <summary>
    /// An attribute to easily manage Exiled features' behavior.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ExiledSerializable : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExiledSerializable"/> class.
        /// </summary>
        public ExiledSerializable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExiledSerializable"/> class.
        /// </summary>
        /// <param name="type"><inheritdoc cref="ItemType"/></param>
        public ExiledSerializable(ItemType type) => ItemType = type;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExiledSerializable"/> class.
        /// </summary>
        /// <param name="role"><inheritdoc cref="RoleType"/></param>
        public ExiledSerializable(RoleType role) => RoleType = role;

        /// <summary>
        /// Gets the attribute's <see cref="ItemType"/>.
        /// </summary>
        public ItemType ItemType { get; }

        /// <summary>
        /// Gets the attribute's <see cref="RoleType"/>.
        /// </summary>
        public RoleType RoleType { get; }
    }
}
