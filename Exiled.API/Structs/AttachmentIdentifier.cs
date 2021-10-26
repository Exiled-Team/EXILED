// -----------------------------------------------------------------------
// <copyright file="AttachmentIdentifier.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Structs
{
    using InventorySystem.Items.Firearms.Attachments;

    /// <summary>
    /// A tool to identify attachments.
    /// </summary>
    public struct AttachmentIdentifier
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentIdentifier"/> struct.
        /// </summary>
        /// <param name="code"><inheritdoc cref="Code"/></param>
        /// <param name="name"><inheritdoc cref="Name"/></param>
        /// <param name="slot"><inheritdoc cref="Slot"/></param>
        public AttachmentIdentifier(uint code, AttachmentNameTranslation name, AttachmentSlot slot)
        {
            Code = code;
            Name = name;
            Slot = slot;
        }

        /// <summary>
        /// Gets the attachment code.
        /// </summary>
        public uint Code { get; }

        /// <summary>
        /// Gets the attachment name.
        /// </summary>
        public AttachmentNameTranslation Name { get; }

        /// <summary>
        /// Gets the attachment slot.
        /// </summary>
        public AttachmentSlot Slot { get; }
    }
}
