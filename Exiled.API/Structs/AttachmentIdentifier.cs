// -----------------------------------------------------------------------
// <copyright file="AttachmentIdentifier.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Structs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Exiled.API.Enums;

    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Firearms.Attachments.Components;

    /// <summary>
    /// A tool to identify attachments.
    /// </summary>
    [DebuggerDisplay("AmmoType = {AmmoType} Limit = {Limit}")]
    public readonly struct AttachmentIdentifier
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentIdentifier"/> struct.
        /// </summary>
        /// <param name="code">The code of the attachment.</param>
        /// <param name="name">The name of the attachment.</param>
        /// <param name="slot">The slot of the attachment.</param>
        internal AttachmentIdentifier(uint code, AttachmentName name, AttachmentSlot slot)
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
        public AttachmentName Name { get; }

        /// <summary>
        /// Gets the attachment slot.
        /// </summary>
        public AttachmentSlot Slot { get; }

        /// <summary>
        /// Compares two operands: <see cref="AttachmentIdentifier"/> and <see cref="AttachmentIdentifier"/>.
        /// </summary>
        /// <param name="left">The left-hand <see cref="AttachmentIdentifier"/> operand to compare.</param>
        /// <param name="right">The right-hand <see cref="AttachmentIdentifier"/> operand to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(AttachmentIdentifier left, AttachmentIdentifier right) => (left.Name == right.Name) && (left.Code == right.Code) && (left.Slot == right.Slot);

        /// <summary>
        /// Compares two operands: <see cref="AttachmentIdentifier"/> and <see cref="AttachmentIdentifier"/>.
        /// </summary>
        /// <param name="left">The left-hand <see cref="AttachmentIdentifier"/> operand to compare.</param>
        /// <param name="right">The right-hand <see cref="AttachmentIdentifier"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator !=(AttachmentIdentifier left, AttachmentIdentifier right) => (left.Name != right.Name) && (left.Code != right.Code) && (left.Slot != right.Slot);

        /// <summary>
        /// Compares two operands: <see cref="AttachmentIdentifier"/> and <see cref="Attachment"/>.
        /// </summary>
        /// <param name="left">The <see cref="AttachmentIdentifier"/> to compare.</param>
        /// <param name="right">The <see cref="Attachment"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(AttachmentIdentifier left, Attachment right) => (left.Name == right.Name) && (left.Slot == right.Slot);

        /// <summary>
        /// Compares two operands: <see cref="AttachmentIdentifier"/> and <see cref="Attachment"/>.
        /// </summary>
        /// <param name="left">The <see cref="AttachmentIdentifier"/> to compare.</param>
        /// <param name="right">The <see cref="Attachment"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(AttachmentIdentifier left, Attachment right) => left.Name != right.Name || left.Slot != right.Slot;

        /// <summary>
        /// Compares two operands: <see cref="AttachmentIdentifier"/> and <see cref="Attachment"/>.
        /// </summary>
        /// <param name="left">The <see cref="Attachment"/> to compare.</param>
        /// <param name="right">The <see cref="AttachmentIdentifier"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(Attachment left, AttachmentIdentifier right) => right == left;

        /// <summary>
        /// Compares two operands: <see cref="AttachmentIdentifier"/> and <see cref="Attachment"/>.
        /// </summary>
        /// <param name="left">The <see cref="Attachment"/> to compare.</param>
        /// <param name="right">The <see cref="AttachmentIdentifier"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(Attachment left, AttachmentIdentifier right) => right != left;

        /// <summary>
        /// Computes the sum of its right-hand <see cref="AttachmentIdentifier"/> operand and its left-hand <see cref="uint"/> operand.
        /// </summary>
        /// <param name="left">The <see cref="AttachmentIdentifier"/> to be added up.</param>
        /// <param name="right">The <see cref="uint"/> to be added up.</param>
        /// <returns>A <see cref="uint"/> value that represents the sum of the two operands.</returns>
        public static uint operator +(AttachmentIdentifier left, uint right) => left.Code + right;

        /// <summary>
        /// Subtracts its right-hand <see cref="AttachmentIdentifier"/> operand from its left-hand <see cref="uint"/> operand.
        /// </summary>
        /// <param name="left">The <see cref="AttachmentIdentifier"/> to be subtracted.</param>
        /// <param name="right">The <see cref="uint"/> to be subtracted.</param>
        /// <returns>A <see cref="uint"/> value representing the subtraction between the two operands.</returns>
        public static uint operator -(AttachmentIdentifier left, uint right) => left.Code - right;

        /// <summary>
        /// Computes the sum of its right-hand <see cref="uint"/> operand and its left-hand <see cref="AttachmentIdentifier"/> operand.
        /// </summary>
        /// <param name="left">The <see cref="uint"/> to be added up.</param>
        /// <param name="right">The <see cref="AttachmentIdentifier"/> to be added up.</param>
        /// <returns>A <see cref="uint"/> value that represents the sum of the two operands.</returns>
        public static uint operator +(uint left, AttachmentIdentifier right) => right + left;

        /// <summary>
        /// Subtracts its right-hand <see cref="uint"/> operand from its left-hand <see cref="AttachmentIdentifier"/> operand.
        /// </summary>
        /// <param name="left">The <see cref="uint"/> to be subtracted.</param>
        /// <param name="right">The <see cref="AttachmentIdentifier"/> to be subtracted.</param>
        /// <returns>A <see cref="uint"/> value representing the subtraction between the two operands.</returns>
        public static uint operator -(uint left, AttachmentIdentifier right) => left - right.Code;

        /// <summary>
        /// Converts the string representation of a <see cref="AttachmentIdentifier"/> to its <see cref="AttachmentIdentifier"/> equivalent.
        /// A return value indicates whether the conversion is succeeded or failed.
        /// </summary>
        /// <param name="s">The <see cref="string"/> to convert.</param>
        /// <param name="identifier">The converted <see cref="string"/>.</param>
        /// <returns><see langword="true"/> if <see cref="string"/> was converted successfully; otherwise, <see langword="false"/>.</returns>
        public static bool TryParse(string s, out AttachmentIdentifier identifier)
        {
            identifier = default;
            foreach (AttachmentIdentifier attId in Features.Items.Firearm.AvailableAttachments.Values.SelectMany(kvp => kvp.Where(kvp2 => kvp2.Name.ToString() == s)))
            {
                identifier = attId;
                return true;
            }

            identifier = default;
            return false;
        }

        /// <summary>
        /// Gets a <see cref="AttachmentIdentifier"/> by name.
        /// </summary>
        /// <param name="type">Weapons <see cref="FirearmType"/>.</param>
        /// <param name="name">Attachment name.</param>
        /// <returns><see cref="AttachmentIdentifier"/> instance.</returns>
        public static AttachmentIdentifier Get(FirearmType type, AttachmentName name) => Features.Items.Firearm.AvailableAttachments[type].FirstOrDefault(identifier => identifier.Name == name);

        /// <summary>
        /// Gets the all <see cref="AttachmentIdentifier"/>'s for type, by slot.
        /// </summary>
        /// <param name="type">Weapons <see cref="FirearmType"/>.</param>
        /// <param name="slot">Attachment slot.</param>
        /// <returns><see cref="AttachmentIdentifier"/> instance.</returns>
        public static IEnumerable<AttachmentIdentifier> Get(FirearmType type, AttachmentSlot slot) => Features.Items.Firearm.AvailableAttachments[type].Where(identifier => identifier.Slot == slot);

        /// <summary>
        /// Converts the string representation of a <see cref="AttachmentName"/> to its <see cref="AttachmentName"/> equivalent.
        /// A return value indicates whether the conversion is succeeded or failed.
        /// </summary>
        /// <param name="s">The <see cref="string"/> to convert.</param>
        /// <param name="name">The converted <see cref="string"/>.</param>
        /// <returns><see langword="true"/> if <see cref="string"/> was converted successfully; otherwise, <see langword="false"/>.</returns>
        public static bool TryParse(string s, out AttachmentName name)
        {
            name = default;

            if (string.IsNullOrEmpty(s))
                return false;

            return Enum.TryParse(s, true, out name);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => base.Equals(obj);

        /// <inheritdoc/>
        public override string ToString() => Name.ToString();

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// Indicates whether this instance and a <see cref="Attachment"/> are equal.
        /// </summary>
        /// <param name="firearmAttachment">The <see cref="Attachment"/> to compare with the current instance.</param>
        /// <returns><see langword="true"/> if <see cref="Attachment"/> and this instance represent the same value; otherwise, <see langword="false"/>.</returns>
        public bool Equals(Attachment firearmAttachment) => this == firearmAttachment;

        /// <summary>
        /// Indicates whether this instance and a <see cref="AttachmentIdentifier"/> are equal.
        /// </summary>
        /// <param name="attachmentIdentifier">The <see cref="AttachmentIdentifier"/> to compare with the current instance.</param>
        /// <returns><see langword="true"/> if <see cref="AttachmentIdentifier"/> and this instance represent the same value; otherwise, <see langword="false"/>.</returns>
        public bool Equals(AttachmentIdentifier attachmentIdentifier) => this == attachmentIdentifier;
    }
}