// -----------------------------------------------------------------------
// <copyright file="AssetFragment.cs" company="Exiled Team">
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

    using Exiled.API.Features.Core.Attributes;
    using Exiled.API.Features.Core.Interfaces;

    /// <summary>
    /// Represents a fragment of an asset containing information about its members and values.
    /// </summary>
    public readonly struct AssetFragment
    {
        private readonly Dictionary<string, object> gears;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetFragment"/> struct with the specified type and gears.
        /// </summary>
        /// <param name="inType">The type of the asset fragment.</param>
        /// <param name="member">The member information associated with the value.</param>
        /// <param name="inGears">The dictionary containing member-information pairs.</param>
        public AssetFragment(Type inType, MemberInfo member, Dictionary<string, object> inGears)
        {
            Type = inType;
            gears = inGears;
            MemberName = member.Name;
            IsField = member is FieldInfo;
        }

        /// <summary>
        /// Gets the type of the asset fragment.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the name of the member which the asset derives from.
        /// </summary>
        public string MemberName { get; }

        /// <summary>
        /// Gets a value indicating whether the asset derives from a field.
        /// </summary>
        public bool IsField { get; }

        /// <summary>
        /// Gets the dictionary containing member-information pairs.
        /// </summary>
        public IReadOnlyDictionary<string, object> Gears => gears;

        /// <summary>
        /// Generates an <see cref="AssetFragment"/> from the specified entity object by inspecting its members recursively.
        /// </summary>
        /// <param name="entity">The entity object from which to generate the <see cref="AssetFragment"/>.</param>
        /// <param name="member">The member information associated with the value.</param>
        /// <returns>The generated <see cref="AssetFragment"/>.</returns>
        public static AssetFragment GenerateAsset(object entity, MemberInfo member)
        {
            if (entity is not IAssetFragment)
                return default;

            Type type = entity.GetType();

            if (type.GetCustomAttribute<EClassAttribute>() is null)
                return default;

            Dictionary<string, object> gears = new();

            foreach (MemberInfo memberInfo in type.GetMembers())
            {
                if (memberInfo.GetCustomAttribute<EPropertyAttribute>() is null)
                    continue;

                object value;

                switch (memberInfo)
                {
                    case PropertyInfo propertyInfo:
                        value = propertyInfo.GetValue(entity);
                        break;
                    case FieldInfo fieldInfo:
                        value = fieldInfo.GetValue(entity);
                        break;
                    default:
                        continue;
                }

                if (value is IAssetFragment)
                    value = GenerateAsset(value, memberInfo);

                gears[memberInfo.Name] = value;
            }

            return new(type, member, gears);
        }

        /// <summary>
        /// Generates an <see cref="AssetFragment"/> from the specified entity object by inspecting its members recursively.
        /// </summary>
        /// <param name="entity">The entity object from which to generate the <see cref="AssetFragment"/>.</param>
        /// <param name="member">The member information associated with the value.</param>
        /// <param name="members">The members to include in the generated asset fragment.</param>
        /// <returns>The generated <see cref="AssetFragment"/>.</returns>
        public static AssetFragment GenerateAsset(object entity, MemberInfo member, IEnumerable<string> members)
        {
            if (entity is not IAssetFragment)
                return default;

            Type type = entity.GetType();

            if (type.GetCustomAttribute<EClassAttribute>() is null)
                return default;

            Dictionary<string, object> gears = new();

            foreach (MemberInfo memberInfo in type.GetMembers())
            {
                if (memberInfo.GetCustomAttribute<EPropertyAttribute>() is null || members.All(m => m != memberInfo.Name))
                    continue;

                object value;

                switch (memberInfo)
                {
                    case PropertyInfo propertyInfo:
                        value = propertyInfo.GetValue(entity);
                        break;
                    case FieldInfo fieldInfo:
                        value = fieldInfo.GetValue(entity);
                        break;
                    default:
                        continue;
                }

                if (value is IAssetFragment)
                    value = GenerateAsset(value, memberInfo, members);

                gears[memberInfo.Name] = value;
            }

            return new(type, member, gears);
        }

        /// <summary>
        /// Recursively extracts and assigns values from an <see cref="AssetFragment"/> to the target object.
        /// </summary>
        /// <param name="target">The target object to which the values will be assigned.</param>
        /// <param name="nextFragment">The AssetFragment containing the values to be assigned.</param>
        /// <param name="nextMember">The member information of the target object which could be a field or property.</param>
        public static void ExtractAsset(object target, AssetFragment? nextFragment, MemberInfo nextMember)
        {
            FieldInfo targetField = null;
            PropertyInfo targetProperty = null;

            if (nextMember is FieldInfo nextField)
                targetField = nextField;
            else if (nextMember is PropertyInfo nextProperty)
                targetProperty = nextProperty;

            Type subType = targetField is not null ? targetField.FieldType : targetProperty?.PropertyType;
            if (subType is null || !nextFragment.HasValue)
                return;

            object subTarget = targetField is not null ? targetField.GetValue(target) : targetProperty.GetValue(target);

            foreach (MemberInfo memberInfo in subType.GetMembers())
            {
                if (memberInfo.GetCustomAttribute<EPropertyAttribute>() is null)
                    continue;

                if (!nextFragment.Value.Gears.TryGetValue(memberInfo.Name, out object subValue))
                    continue;

                if (subValue is AssetFragment nextAssetFragment)
                {
                    ExtractAsset(subTarget, nextAssetFragment, memberInfo);
                    continue;
                }

                switch (memberInfo)
                {
                    case FieldInfo f when f.FieldType == subValue.GetType():
                        subType.GetField(memberInfo.Name)?.SetValue(subTarget, subValue);
                        continue;
                    case PropertyInfo p when p.PropertyType == subValue.GetType():
                        subType.GetProperty(memberInfo.Name)?.SetValue(subTarget, subValue);
                        continue;
                }
            }
        }

        /// <summary>
        /// Recursively extracts and assigns values from an <see cref="AssetFragment"/> to the target object.
        /// </summary>
        /// <param name="target">The target object to which the values will be assigned.</param>
        /// <param name="nextFragment">The <see cref="AssetFragment"/> containing the values to be assigned.</param>
        /// <param name="nextMember">The member information of the target object which could be a field or property.</param>
        /// <param name="members">The members to include in the extraction of the asset fragment.</param>
        public static void ExtractAsset(object target, AssetFragment? nextFragment, MemberInfo nextMember, IEnumerable<string> members)
        {
            FieldInfo targetField = null;
            PropertyInfo targetProperty = null;

            if (nextMember is FieldInfo nextField)
                targetField = nextField;
            else if (nextMember is PropertyInfo nextProperty)
                targetProperty = nextProperty;

            Type subType = targetField is not null ? targetField.FieldType : targetProperty?.PropertyType;
            if (subType is null || !nextFragment.HasValue)
                return;

            object subTarget = targetField is not null ? targetField.GetValue(target) : targetProperty.GetValue(target);

            foreach (MemberInfo memberInfo in subType.GetMembers())
            {
                if (memberInfo.GetCustomAttribute<EPropertyAttribute>() is null || members.All(m => m != memberInfo.Name))
                    continue;

                if (!nextFragment.Value.Gears.TryGetValue(memberInfo.Name, out object subValue))
                    continue;

                if (subValue is AssetFragment nextAssetFragment)
                {
                    ExtractAsset(subTarget, nextAssetFragment, memberInfo, members);
                    continue;
                }

                switch (memberInfo)
                {
                    case FieldInfo f when f.FieldType == subValue.GetType():
                        subType.GetField(memberInfo.Name)?.SetValue(subTarget, subValue);
                        continue;
                    case PropertyInfo p when p.PropertyType == subValue.GetType():
                        subType.GetProperty(memberInfo.Name)?.SetValue(subTarget, subValue);
                        continue;
                }
            }
        }
    }
}