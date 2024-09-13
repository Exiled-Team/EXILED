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
        /// <summary>
        /// The behaviour's config.
        /// </summary>
#pragma warning disable SA1401
        protected ModulePointer config;
#pragma warning restore SA1401

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
            foreach (PropertyInfo propertyInfo in config.GetType().GetProperties())
            {
                PropertyInfo targetProperty = inType.GetProperty(propertyInfo.Name);
                if (targetProperty?.PropertyType.IsClass == true && targetProperty.PropertyType != typeof(string))
                {
                    object targetInstance = targetProperty.GetValue(instance);
                    if (targetInstance != null)
                        ApplyConfig_DefaultImplementation(propertyInfo.GetValue(config), targetInstance);
                }
                else
                {
                    ApplyConfig_DefaultImplementation(propertyInfo, targetProperty);
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
            if (source == null || target == null)
                return;

            Type sourceType = source.GetType();
            Type targetType = target.GetType();

            foreach (PropertyInfo sourceProperty in sourceType.GetProperties())
            {
                PropertyInfo targetProperty = targetType.GetProperty(sourceProperty.Name);
                targetProperty?.SetValue(target, sourceProperty.GetValue(source, null));
            }
        }

        /// <summary>
        /// Implements the behaviour's configs by copying properties from the config object to the current instance.
        /// </summary>
        protected virtual void ImplementConfigs()
        {
            if (Config is null)
                return;

            Type inType = GetType();
            foreach (PropertyInfo propertyInfo in Config.GetType().GetProperties())
            {
                PropertyInfo targetProperty = inType.GetProperty(propertyInfo.Name);
                if (targetProperty?.PropertyType.IsClass == true && targetProperty.PropertyType != typeof(string))
                {
                    object targetInstance = targetProperty.GetValue(this);
                    if (targetInstance != null)
                        ApplyConfig(propertyInfo.GetValue(Config), targetInstance);
                }
                else
                {
                    ApplyConfig(propertyInfo, targetProperty);
                }
            }
        }

        /// <summary>
        /// Applies a configuration property value from the source property to the target property.
        /// </summary>
        /// <param name="source">The source property value from the config object.</param>
        /// <param name="target">The target instance where the config should be applied.</param>
        protected virtual void ApplyConfig(object source, object target)
        {
            if (source == null || target == null)
                return;

            Type sourceType = source.GetType();
            Type targetType = target.GetType();

            foreach (PropertyInfo sourceProperty in sourceType.GetProperties())
            {
                PropertyInfo targetProperty = targetType.GetProperty(sourceProperty.Name);
                targetProperty?.SetValue(target, sourceProperty.GetValue(source, null));
            }
        }
    }
}
