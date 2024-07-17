// -----------------------------------------------------------------------
// <copyright file="ModuleBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features
{
    using System;
    using System.Reflection;

    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Generic;
    using Exiled.CustomModules.API.Features.Attributes;

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
        /// Gets or sets the behaviour's configs.
        /// </summary>
        public virtual ModulePointer Config { get; set; }

        /// <summary>
        /// Implements the behaviour's configs by copying properties from the config object to the current instance.
        /// </summary>
        protected virtual void ImplementConfigs()
        {
            if (Config is null)
                return;

            Type inType = GetType();
            foreach (PropertyInfo propertyInfo in Config.GetType().GetProperties())
                ApplyConfig(propertyInfo, inType.GetProperty(propertyInfo.Name));
        }

        /// <summary>
        /// Applies a configuration property value from the source property to the target property.
        /// </summary>
        /// <param name="propertyInfo">The source property from the config object.</param>
        /// <param name="targetInfo">The target property in the current instance.</param>
        protected virtual void ApplyConfig(PropertyInfo propertyInfo, PropertyInfo targetInfo)
        {
            targetInfo?.SetValue(this, propertyInfo.GetValue(Config, null));
        }
    }
}
