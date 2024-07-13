// -----------------------------------------------------------------------
// <copyright file="EProperty.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core
{
    using System;
    using System.Reflection;

    using Exiled.API.Features.Core.Attributes;

    /// <summary>
    /// Represents a property of an <see cref="EntityAsset"/>.
    /// </summary>
    public readonly struct EProperty
    {
        private readonly MemberInfo originMember;

        /// <summary>
        /// Initializes a new instance of the <see cref="EProperty"/> struct.
        /// </summary>
        /// <param name="entity">The entity belonging to the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <param name="memberInfo">The origin member.</param>
        /// <param name="registrySearchable">Indicates whether the property is searchable in the registry.</param>
        /// <param name="readOnly">Indicates whether the property is read-only.</param>
        /// <param name="serializable">Indicates whether the property is serializable.</param>
        /// <param name="category">The category of the property.</param>
        /// <param name="exactName">The exact name of the property.</param>
        internal EProperty(
            object entity,
            object value,
            MemberInfo memberInfo,
            bool registrySearchable = true,
            bool readOnly = false,
            bool serializable = true,
            string category = AssetRegistry.DEFAULT_CATEGORY,
            string exactName = AssetRegistry.DEFAULT_EXACT_NAME)
        {
            if (memberInfo is not PropertyInfo and MemberInfo)
                throw new ArgumentException("An EProperty requires a member that can be either a property or a field, and nothing else.");

            Entity = entity;
            Value = value;
            RegistrySearchable = registrySearchable;
            ReadOnly = readOnly;
            Serializable = serializable;
            Category = category;
            ExactName = exactName;
            originMember = memberInfo;
            MemberName = originMember.Name;
            IsField = originMember is FieldInfo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EProperty"/> struct.
        /// </summary>
        /// <param name="entity">The entity belonging to the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <param name="memberInfo">The origin member.</param>
        /// <param name="propertyAttribute">The <see cref="EPropertyAttribute"/> containing all information to initialize the property.</param>
        /// <exception cref="ArgumentException">Thrown when the member is not either a property or a field.</exception>
        internal EProperty(object entity, object value, MemberInfo memberInfo, EPropertyAttribute propertyAttribute)
        {
            if (memberInfo is not PropertyInfo and MemberInfo)
                throw new ArgumentException("An EProperty requires a member that can be either a property or a field, and nothing else.");

            Entity = entity;
            Value = value;
            RegistrySearchable = propertyAttribute.RegistrySearchable;
            ReadOnly = propertyAttribute.ReadOnly;
            Serializable = propertyAttribute.Serializable;
            Category = propertyAttribute.Category;
            ExactName = propertyAttribute.ExactName;
            originMember = memberInfo;
            MemberName = originMember.Name;
            IsField = originMember is FieldInfo;
        }

        /// <summary>
        /// Gets the entity instance belonging to the property.
        /// </summary>
        public object Entity { get; }

        /// <summary>
        /// Gets the value of the property.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Gets a value indicating whether the property is searchable in the registry.
        /// </summary>
        public bool RegistrySearchable { get; }

        /// <summary>
        /// Gets a value indicating whether the property is read-only.
        /// </summary>
        public bool ReadOnly { get; }

        /// <summary>
        /// Gets a value indicating whether the property is serializable.
        /// </summary>
        public bool Serializable { get; }

        /// <summary>
        /// Gets the category of the property.
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Gets the exact name of the property.
        /// </summary>
        public string ExactName { get; }

        /// <summary>
        /// Gets the initial member name.
        /// </summary>
        public string MemberName { get; }

        /// <summary>
        /// Gets a value indicating whether the origin member is a field.
        /// </summary>
        public bool IsField { get; }

        /// <summary>
        /// Creates a new instance of <see cref="EProperty"/> with the specified value.
        /// </summary>
        /// <param name="value">The new value for the property.</param>
        /// <returns>A new instance of <see cref="EProperty"/> with the specified value.</returns>
        /// <exception cref="Exception">Thrown when attempting to edit a ReadOnly property.</exception>
        public EProperty Edit(object value)
        {
            if (ReadOnly)
                throw new Exception("Cannot edit a ReadOnly EProperty.");

            return new(Entity, value, originMember, RegistrySearchable, ReadOnly, Serializable, Category, ExactName);
        }

        /// <summary>
        /// Makes the property read-only.
        /// </summary>
        /// <returns>A new instance of <see cref="EProperty"/> that is read-only.</returns>
        /// <exception cref="Exception">Thrown when attempting to edit a ReadOnly property.</exception>
        public EProperty MakeReadOnly()
        {
            if (ReadOnly)
                throw new Exception("Cannot edit a ReadOnly EProperty.");

            return new(Entity, Value, originMember, RegistrySearchable, true, Serializable, Category, ExactName);
        }

        /// <summary>
        /// Sets the category of the property to the specified value.
        /// </summary>
        /// <param name="newCategory">The new category for the property.</param>
        /// <returns>A new instance of <see cref="EProperty"/> with the updated category.</returns>
        /// <exception cref="Exception">Thrown when attempting to edit a ReadOnly property.</exception>
        public EProperty SetCategory(string newCategory)
        {
            if (ReadOnly)
                throw new Exception("Cannot edit a ReadOnly EProperty.");

            return new(Entity, Value, originMember, RegistrySearchable, ReadOnly, Serializable, newCategory, ExactName);
        }

        /// <summary>
        /// Sets the exact name of the property to the specified value.
        /// </summary>
        /// <param name="newExactName">The new exact name for the property.</param>
        /// <returns>A new instance of <see cref="EProperty"/> with the updated exact name.</returns>
        /// <exception cref="Exception">Thrown when attempting to edit a ReadOnly property.</exception>
        public EProperty SetExactName(string newExactName)
        {
            if (ReadOnly)
                throw new Exception("Cannot edit a ReadOnly EProperty.");

            return new(Entity, Value, originMember, RegistrySearchable, ReadOnly, Serializable, Category, newExactName);
        }
    }
}