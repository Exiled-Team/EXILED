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
        /// <param name="name"><inheritdoc cref="Name"/></param>
        /// <param name="slot"><inheritdoc cref="Slot"/></param>
        public AttachmentIdentifier(AttachmentNameTranslation name, AttachmentSlot slot)
        {
            Code = 0;
            Name = name;
            Slot = slot;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentIdentifier"/> struct.
        /// </summary>
        /// <param name="code"><inheritdoc cref="Code"/></param>
        /// <param name="name"><inheritdoc cref="Name"/></param>
        /// <param name="slot"><inheritdoc cref="Slot"/></param>
        internal AttachmentIdentifier(uint code, AttachmentNameTranslation name, AttachmentSlot slot)
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

        /// <summary>
        /// Compares two operands: <see cref="AttachmentIdentifier"/> and <see cref="FirearmAttachment"/>.
        /// </summary>
        /// <param name="identifier">The <see cref="AttachmentIdentifier"/> to compare.</param>
        /// <param name="firearmAttachment">The <see cref="FirearmAttachment"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(AttachmentIdentifier identifier, FirearmAttachment firearmAttachment) =>
            identifier.Name == firearmAttachment.Name && identifier.Slot == firearmAttachment.Slot;

        /// <summary>
        /// Compares two operands: <see cref="AttachmentIdentifier"/> and <see cref="FirearmAttachment"/>.
        /// </summary>
        /// <param name="identifier">The <see cref="AttachmentIdentifier"/> to compare.</param>
        /// <param name="firearmAttachment">The <see cref="FirearmAttachment"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(AttachmentIdentifier identifier, FirearmAttachment firearmAttachment) =>
            identifier.Name != firearmAttachment.Name || identifier.Slot != firearmAttachment.Slot;

        /// <summary>
        /// Compares two operands: <see cref="AttachmentIdentifier"/> and <see cref="FirearmAttachment"/>.
        /// </summary>
        /// <param name="firearmAttachment">The <see cref="FirearmAttachment"/> to compare.</param>
        /// <param name="identifier">The <see cref="AttachmentIdentifier"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(FirearmAttachment firearmAttachment, AttachmentIdentifier identifier) => identifier == firearmAttachment;

        /// <summary>
        /// Compares two operands: <see cref="AttachmentIdentifier"/> and <see cref="FirearmAttachment"/>.
        /// </summary>
        /// <param name="firearmAttachment">The <see cref="FirearmAttachment"/> to compare.</param>
        /// <param name="identifier">The <see cref="AttachmentIdentifier"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(FirearmAttachment firearmAttachment, AttachmentIdentifier identifier) => identifier != firearmAttachment;

        /// <inheritdoc/>
        public override bool Equals(object obj) => Equals(obj as FirearmAttachment);

        /// <inheritdoc/>
        public override string ToString() => $"Code: {Code}\nName: {Name}\nSlot: {Slot}";

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// Indicates whether this instance and a <see cref="FirearmAttachment"/> are equal.
        /// </summary>
        /// <param name="firearmAttachment">The <see cref="FirearmAttachment"/> to compare with the current instance.</param>
        /// <returns><see langword="true"/> if <see cref="FirearmAttachment"/> and this instance represent the same value; otherwise, <see langword="false"/>.</returns>
        public bool Equals(FirearmAttachment firearmAttachment) => this == firearmAttachment;
    }
}
