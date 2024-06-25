// -----------------------------------------------------------------------
// <copyright file="EntityAsset.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Core.Attributes;
    using Exiled.API.Features.Core.Generic;
    using Exiled.API.Features.Core.Interfaces;

    /// <summary>
    /// Represents an entity asset that can be manipulated externally and within subobjects.
    /// </summary>
    public sealed class EntityAsset : EObject
    {
        private Dictionary<string, List<EProperty>> registry;
        private EProperty[] properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAsset"/> class with the default name.
        /// </summary>
        private EntityAsset() => Name = AssetRegistry.DEFAULT_ASSET_NAME;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityAsset"/> class with the specified identifier.
        /// </summary>
        /// <param name="identifier">The identifier for the entity asset.</param>
        private EntityAsset(string identifier) => Name = identifier;

        /// <summary>
        /// Gets the class of the entity asset.
        /// </summary>
        public EClass Class { get; private set; }

        /// <summary>
        /// Gets the properties of the entity asset.
        /// </summary>
        public IReadOnlyCollection<EProperty> Properties => properties;

        /// <summary>
        /// Gets the registry of the entity asset.
        /// </summary>
        public IReadOnlyDictionary<string, List<EProperty>> Registry => registry;

        /// <summary>
        /// Acquires an entity asset from the specified target object.
        /// </summary>
        /// <param name="target">The target object to acquire the entity asset from.</param>
        /// <param name="identifier">The optional identifier for the entity asset.</param>
        /// <returns>The acquired entity asset.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the target is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the target type is not decorated with <see cref="EClassAttribute"/>.</exception>
        public static EntityAsset Acquire(object target, string identifier = "")
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target), "The target to acquire the EntityAsset from must not be null");

            EClassAttribute classAttribute = target.GetType().GetCustomAttribute<EClassAttribute>();

            if (classAttribute is null)
                throw new InvalidOperationException("The target type must be decorated with EClassAttribute to acquire an EntityAsset.");

            if (!Singleton<AssetRegistry>.TryGet(out AssetRegistry _))
                Singleton<AssetRegistry>.Create(new());

            identifier = string.IsNullOrEmpty(identifier) ?
                string.IsNullOrEmpty(classAttribute.ExactName) ?
                    AssetRegistry.DEFAULT_ASSET_NAME : classAttribute.ExactName : identifier;

            if (classAttribute.AllowOnce && !string.IsNullOrEmpty(identifier))
                Singleton<AssetRegistry>.Instance.RemoveAll(identifier);

            EntityAsset entityAsset = new(identifier)
            {
                Class = new(target, classAttribute),
            };

            if (classAttribute.AssetRegistrySearchable)
                Singleton<AssetRegistry>.Instance.Add(entityAsset);

            List<EProperty> propertiesToSave = new();
            Dictionary<string, List<EProperty>> propertiesToSaveIntoRegistry = new();

            void ProcessMember(MemberInfo member)
            {
                if (member.GetCustomAttribute<EPropertyAttribute>() is not EPropertyAttribute propertyAttribute)
                    return;

                object value = member is PropertyInfo propertyInfo ? propertyInfo.GetValue(target) :
                    member is FieldInfo fieldInfo ? fieldInfo.GetValue(target) : null;

                if (value is IAssetFragment)
                    value = AssetFragment.GenerateAsset(value, member);

                EProperty property =
                    string.IsNullOrEmpty(propertyAttribute.ExactName) ||
                    propertyAttribute.ExactName == AssetRegistry.DEFAULT_EXACT_NAME
                        ? new EProperty(
                            target,
                            value,
                            member,
                            propertyAttribute.RegistrySearchable,
                            propertyAttribute.ReadOnly,
                            propertyAttribute.Serializable,
                            propertyAttribute.Category,
                            member.Name)
                        : new EProperty(target, value, member, propertyAttribute);

                propertiesToSave.Add(property);

                if (!propertyAttribute.RegistrySearchable)
                    return;

                if (propertiesToSaveIntoRegistry.ContainsKey(property.Category))
                    propertiesToSaveIntoRegistry[property.Category].Add(property);
                else
                    propertiesToSaveIntoRegistry[property.Category] = new() { property };
            }

            entityAsset.Class.Type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m is PropertyInfo or FieldInfo)
                .ForEach(ProcessMember);

            entityAsset.properties = propertiesToSave.ToArray();
            entityAsset.registry = propertiesToSaveIntoRegistry;

            return entityAsset;
        }

        /// <summary>
        /// Acquires an entity asset from the specified target object.
        /// </summary>
        /// <param name="target">The target object to acquire the entity asset from.</param>
        /// <param name="members">The members to include in the acquired entity asset.</param>
        /// <param name="identifier">The optional identifier for the entity asset.</param>
        /// <returns>The acquired entity asset.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the target is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the target type is not decorated with <see cref="EClassAttribute"/>.</exception>
        public static EntityAsset Acquire(object target, IEnumerable<string> members, string identifier = "")
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target), "The target to acquire the EntityAsset from must not be null");

            EClassAttribute classAttribute = target.GetType().GetCustomAttribute<EClassAttribute>();

            if (classAttribute is null)
                throw new InvalidOperationException("The target type must be decorated with EClassAttribute to acquire an EntityAsset.");

            if (!Singleton<AssetRegistry>.TryGet(out AssetRegistry _))
                Singleton<AssetRegistry>.Create(new());

            identifier = string.IsNullOrEmpty(identifier) ?
                string.IsNullOrEmpty(classAttribute.ExactName) ?
                    AssetRegistry.DEFAULT_ASSET_NAME : classAttribute.ExactName : identifier;

            if (classAttribute.AllowOnce && !string.IsNullOrEmpty(identifier))
                Singleton<AssetRegistry>.Instance.RemoveAll(identifier);

            EntityAsset entityAsset = new(identifier)
            {
                Class = new(target, classAttribute),
            };

            if (classAttribute.AssetRegistrySearchable)
                Singleton<AssetRegistry>.Instance.Add(entityAsset);

            List<EProperty> propertiesToSave = new();
            Dictionary<string, List<EProperty>> propertiesToSaveIntoRegistry = new();

            void ProcessMember(MemberInfo member)
            {
                if (member.GetCustomAttribute<EPropertyAttribute>() is not EPropertyAttribute propertyAttribute)
                    return;

                object value = member is PropertyInfo propertyInfo ? propertyInfo.GetValue(target) :
                    member is FieldInfo fieldInfo ? fieldInfo.GetValue(target) : null;

                if (value is IAssetFragment)
                    value = AssetFragment.GenerateAsset(value, member, members);

                EProperty property =
                    string.IsNullOrEmpty(propertyAttribute.ExactName)
                        ? new EProperty(
                            target,
                            value,
                            member,
                            propertyAttribute.RegistrySearchable,
                            propertyAttribute.ReadOnly,
                            propertyAttribute.Serializable,
                            propertyAttribute.Category,
                            member.Name)
                        : new EProperty(target, value, member, propertyAttribute);

                propertiesToSave.Add(property);

                if (!propertyAttribute.RegistrySearchable)
                    return;

                if (propertiesToSaveIntoRegistry.ContainsKey(property.Category))
                    propertiesToSaveIntoRegistry[property.Category].Add(property);
                else
                    propertiesToSaveIntoRegistry[property.Category] = new() { property };
            }

            entityAsset.Class.Type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m is PropertyInfo or FieldInfo && members.
                    Any(obj => obj == m.Name || obj == m.GetCustomAttribute<EPropertyAttribute>().ExactName))
                .ForEach(ProcessMember);

            entityAsset.properties = propertiesToSave.ToArray();
            entityAsset.registry = propertiesToSaveIntoRegistry;

            return entityAsset;
        }

        /// <summary>
        /// Revokes the entity asset associated with the specified target object within the specified category.
        /// </summary>
        /// <param name="target">The target object associated with the entity asset to revoke.</param>
        /// <param name="exactName">The exact name of the entity asset to revoke.</param>
        /// <param name="category">The category from which to revoke the entity asset.</param>
        public void Revoke(object target, string exactName = AssetRegistry.DEFAULT_EXACT_NAME, string category = AssetRegistry.DEFAULT_CATEGORY)
        {
            if (target is null || target.GetType() != Class.Entity.GetType())
                throw new ArgumentException("The target for revoking an entity asset must not be null and must match the type of the first owner entity.");

            if (exactName != AssetRegistry.DEFAULT_EXACT_NAME)
            {
                EProperty exactProperty = GetExactFromRegistry(exactName);

                if (exactProperty.ReadOnly)
                    return;

                if (exactProperty.IsField)
                {
                    Class.Type.GetField(exactProperty.MemberName)?.SetValue(target, exactProperty.Value);
                    return;
                }

                Class.Type.GetProperty(exactProperty.MemberName)?.SetValue(target, exactProperty.Value);
                return;
            }

            IEnumerable<EProperty> properties = category != AssetRegistry.DEFAULT_CATEGORY ? GetAllFromRegistry<EProperty>(category) : GetAllFromRegistry<EProperty>();

            foreach (EProperty property in properties)
            {
                if (property.ReadOnly)
                    continue;

                if (property.Value is AssetFragment assetFragment)
                {
                    AssetFragment.ExtractAsset(target, assetFragment, property.IsField ?
                        Class.Type.GetField(property.MemberName) :
                        Class.Type.GetProperty(property.MemberName));

                    continue;
                }

                if (property.IsField)
                {
                    Class.Type.GetField(property.MemberName)?.SetValue(target, property.Value);
                    continue;
                }

                Class.Type.GetProperty(property.MemberName)?.SetValue(target, property.Value);
            }
        }

        /// <summary>
        /// Revokes the entity assets associated with the specified target object and exact names.
        /// </summary>
        /// <param name="target">The target object associated with the entity assets to revoke.</param>
        /// <param name="exactNames">The exact names of the entity assets to revoke.</param>
        /// <exception cref="ArgumentException">Thrown when the target is null or does not match the type of the first owner entity.</exception>
        public void Revoke(object target, IEnumerable<string> exactNames)
        {
            if (target is null || target.GetType() != Class.Entity.GetType())
                throw new ArgumentException("The target for revoking an entity asset must not be null and must match the type of the first owner entity.");

            IEnumerable<EProperty> properties = GetAllFromRegistry<EProperty>();

            IEnumerable<string> exactNamesArr = exactNames.ToArray();

            foreach (EProperty property in properties)
            {
                if (property.ReadOnly || !exactNamesArr.Contains(property.ExactName))
                    continue;

                if (property.IsField)
                {
                    Class.Type.GetField(property.MemberName)?.SetValue(target, property.Value);
                    continue;
                }

                Class.Type.GetProperty(property.MemberName)?.SetValue(target, property.Value);
            }
        }

        /// <summary>
        /// Retrieves all entities from the registry of the specified type and category.
        /// </summary>
        /// <typeparam name="T">The type of entities to retrieve.</typeparam>
        /// <param name="category">The category of entities to retrieve.</param>
        /// <returns>An enumerable collection of entities from the registry.</returns>
        public IEnumerable<T> GetAllFromRegistry<T>(string category = AssetRegistry.DEFAULT_CATEGORY)
        {
            foreach (KeyValuePair<string, List<EProperty>> kvp in Registry)
            {
                if (kvp.Key != category)
                    continue;

                foreach (EProperty property in kvp.Value)
                {
                    if (typeof(T) == typeof(EProperty))
                        yield return (T)(object)kvp.Value;
                    else if (property.Entity is T comp)
                        yield return comp;
                }

                break;
            }
        }

        /// <summary>
        /// Retrieves the entity with the specified name from the registry of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the entity to retrieve.</typeparam>
        /// <param name="exactName">The exact name of the entity to retrieve.</param>
        /// <returns>The entity with the specified name from the registry.</returns>
        public T GetExactFromRegistry<T>(string exactName)
        {
            EProperty property = Registry.Values
                .SelectMany(value => value)
                .FirstOrDefault(property =>
                    property.ExactName == exactName &&
                    (typeof(T) == typeof(EProperty) || property.Entity is T));

            return typeof(T) == typeof(EProperty) ? (T)property.Value : (T)property.Entity;
        }

        /// <summary>
        /// Retrieves the entity property with the specified name from the registry.
        /// </summary>
        /// <param name="exactName">The exact name of the entity property to retrieve.</param>
        /// <returns>The entity property with the specified name from the registry.</returns>
        public EProperty GetExactFromRegistry(string exactName) => Registry.Values
            .SelectMany(value => value)
            .FirstOrDefault(property => property.ExactName == exactName);

        /// <summary>
        /// Retrieves the entity property and its index with the specified name from the registry.
        /// </summary>
        /// <param name="exactName">The exact name of the entity property to retrieve.</param>
        /// <returns>A tuple containing the entity property and its index in the registry.</returns>
        public (EProperty property, int index) GetPropertyWithIndexFromRegistry(string exactName)
        {
            int index = 0;
            foreach (List<EProperty> properties in Registry.Values)
            {
                foreach (EProperty property in properties)
                {
                    if (property.ExactName == exactName)
                        return (property, index);

                    index++;
                }
            }

            return (default, -1);
        }

        /// <summary>
        /// Edits the entity property with the specified name in the registry.
        /// </summary>
        /// <param name="exactName">The exact name of the entity property to edit.</param>
        /// <param name="regEditType">The type of edit to perform on the entity property.</param>
        /// <param name="value">The value to apply during the edit operation.</param>
        /// <exception cref="ArgumentException">Thrown when the value cannot be converted to the required type.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an invalid registry edit type is provided.</exception>
        public void EditProperty(string exactName, RegistryEditType regEditType, object value = null)
        {
            (EProperty property, int index) result = GetPropertyWithIndexFromRegistry(exactName);
            EProperty property = result.property;
            int index = result.index;

            switch (regEditType)
            {
                case RegistryEditType.Value:
                    property = property.Edit(value);
                    break;
                case RegistryEditType.ReadOnly:
                    property = property.MakeReadOnly();
                    break;
                case RegistryEditType.Category:
                    if (value is not string category)
                        throw new ArgumentException("Failed to convert the object to a string.", nameof(value));

                    string previousCategory = property.Category;
                    property = property.SetCategory(category);
                    registry[previousCategory].RemoveAt(index);

                    if (registry.ContainsKey(property.Category))
                        registry[property.Category].Add(property);
                    else
                        registry[property.Category] = new() { property };

                    return;
                case RegistryEditType.ExactName:
                    if (value is not string locExactName)
                        throw new ArgumentException("Failed to convert the object to a string.", nameof(value));

                    property = property.SetExactName(locExactName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(regEditType), regEditType, null);
            }

            registry[property.Category][index] = property;
        }
    }
}