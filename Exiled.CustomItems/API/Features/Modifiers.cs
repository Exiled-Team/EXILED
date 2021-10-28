// -----------------------------------------------------------------------
// <copyright file="Modifiers.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.Features
{
    using InventorySystem.Items.Firearms.Attachments;

    /// <summary>
    /// Weapon modifiers.
    /// </summary>
    public struct Modifiers
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Modifiers"/> struct.
        /// </summary>
        /// <param name="attachmentNames"><inheritdoc cref="Attachments"/></param>
        public Modifiers(AttachmentNameTranslation[] attachmentNames)
        {
            Attachments = attachmentNames;
        }

        /// <summary>
        /// Gets a value indicating what <see cref="FirearmAttachment"/>s the weapon will have.
        /// </summary>
        public AttachmentNameTranslation[] Attachments { get; private set; }
    }
}
