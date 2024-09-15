// -----------------------------------------------------------------------
// <copyright file="ModuleBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.Generic
{
    using System;
    using System.Reflection;

    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Generic;

    /// <summary>
    /// Represents a marker class for a module's pointer.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to which the module behaviour is applied.</typeparam>
    /// <remarks>
    /// This abstract class serves as a foundation for user-defined module behaviours that can be applied to entities (such as playable characters)
    /// to extend and customize their functionality through custom modules. It provides a modular and extensible architecture for enhancing gameplay elements.
    /// </remarks>
    public abstract class ModuleBehaviour<TEntity> : EBehaviour<TEntity>
        where TEntity : GameEntity
    {
#pragma warning disable SA1310
#pragma warning disable SA1401
        /// <summary>
        /// The binding flags in use to implement configs through reflection.
        /// </summary>
        internal const BindingFlags CONFIG_IMPLEMENTATION_BINDING_FLAGS =
            BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.DeclaredOnly;

        /// <summary>
        /// The behaviour's config.
        /// </summary>
        protected ModulePointer config;
#pragma warning restore SA1401
#pragma warning restore SA1310

        /// <summary>
        /// Gets or sets the behaviour's config.
        /// </summary>
        public virtual ModulePointer Config
        {
            get => config;
            protected set => config = value;
        }

        /// <summary>
        /// Implements the behaviour's configs by copying properties from the config object to the current instance.
        /// </summary>
        /// <param name="instance">The instance of the object to implement the configs to.</param>
        /// <param name="config">The <see cref="ModulePointer"/> or any config object to be implemented.</param>
        public static void ImplementConfigs_DefaultImplementation(object instance, object config)
        {
            Type inType = instance.GetType();

            foreach (PropertyInfo propertyInfo in config.GetType().GetProperties(CONFIG_IMPLEMENTATION_BINDING_FLAGS))
            {
                PropertyInfo targetProperty = inType.GetProperty(propertyInfo.Name, CONFIG_IMPLEMENTATION_BINDING_FLAGS);

                if (targetProperty is null || targetProperty.PropertyType != propertyInfo.PropertyType)
                    continue;

                if (targetProperty.PropertyType.IsClass && targetProperty.PropertyType != typeof(string))
                {
                    object targetInstance = targetProperty.GetValue(instance);

                    if (targetInstance is not null)
                        ApplyConfig_DefaultImplementation(propertyInfo.GetValue(config), targetInstance);
                }
                else
                {
                    if (targetProperty.CanWrite)
                        targetProperty.SetValue(instance, propertyInfo.GetValue(config));
                }
            }
        }

        /// <summary>
        /// Applies a configuration property value from the source property to the target property.
        /// </summary>
        /// <param name="source">The source property value from the config object.</param>
        /// <param name="target">The target instance where the config should be applied.</param>
        public static void ApplyConfig_DefaultImplementation(object source, object target)
        {
            if (source is null || target is null)
                return;

            Type sourceType = source.GetType();
            Type targetType = target.GetType();

            foreach (PropertyInfo sourceProperty in sourceType.GetProperties(CONFIG_IMPLEMENTATION_BINDING_FLAGS))
            {
                PropertyInfo targetProperty = targetType.GetProperty(sourceProperty.Name);

                if (targetProperty is null || !targetProperty.CanWrite || targetProperty.PropertyType != sourceProperty.PropertyType)
                    continue;

                object value = sourceProperty.GetValue(source);

                if (value is not null && targetProperty.PropertyType.IsClass && targetProperty.PropertyType != typeof(string))
                {
                    object targetInstance = targetProperty.GetValue(target);

                    if (targetInstance is null)
                    {
                        targetInstance = Activator.CreateInstance(targetProperty.PropertyType);
                        targetProperty.SetValue(target, targetInstance);
                    }

                    ApplyConfig_DefaultImplementation(value, targetInstance);
                }
                else
                {
                    targetProperty.SetValue(target, value);
                }
            }
        }

        /// <summary>
        /// Implements the behaviour's configs by copying properties from the config object to the current instance,
        /// stopping at the current class type (including its base classes up to the current type).
        /// </summary>
        protected virtual void ImplementConfigs()
        {
            if (Config is null)
                return;

            ImplementConfigs_DefaultImplementation(this, Config);
        }

        /// <summary>
        /// Applies the configuration from the source property to the target property by copying the value.
        /// Handles nested objects and primitive types appropriately.
        /// </summary>
        /// <param name="source">The source property from which to copy the value.</param>
        /// <param name="target">The target property to which the value will be copied.</param>
        protected virtual void ApplyConfig(object source, object target) => ApplyConfig_DefaultImplementation(source, target);
    }
}
