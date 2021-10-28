// -----------------------------------------------------------------------
// <copyright file="Modifiers.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.Features
{
    using Exiled.API.Structs;

    /// <summary>
    /// Weapon modifiers.
    /// </summary>
    public struct Modifiers
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Modifiers"/> struct.
        /// </summary>
        /// <param name="attachmentIdentifiers"><inheritdoc cref="Attachments"/></param>
        public Modifiers(AttachmentIdentifier[] attachmentIdentifiers)
        {
            Attachments = attachmentIdentifiers;
        }

        /// <summary>
        /// Gets a value indicating what <see cref="AttachmentIdentifier"/>s the weapon will have.
        /// </summary>
        public AttachmentIdentifier[] Attachments { get; private set; }
    }
}
