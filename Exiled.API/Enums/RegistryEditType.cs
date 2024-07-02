// -----------------------------------------------------------------------
// <copyright file="RegistryEditType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Specifies the type of edit to perform on a registry property.
    /// </summary>
    public enum RegistryEditType
    {
        /// <summary>
        /// Indicates that the value of the property should be edited.
        /// </summary>
        Value,

        /// <summary>
        /// Indicates that the exact name of the property should be edited.
        /// </summary>
        ExactName,

        /// <summary>
        /// Indicates that the category of the property should be edited.
        /// </summary>
        Category,

        /// <summary>
        /// Indicates that the property should be set as read-only.
        /// </summary>
        ReadOnly,
    }
}