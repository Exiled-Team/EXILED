// -----------------------------------------------------------------------
// <copyright file="ExtendedObjectFactory.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader.Features.Configs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Exiled.API.Interfaces;

    using YamlDotNet.Serialization;

    /// <inheritdoc />
    internal sealed class ExtendedObjectFactory : IObjectFactory
    {
        private static readonly Dictionary<Type, Type> DefaultGenericInterfaceImplementations = new Dictionary<Type, Type>
        {
            { typeof(IEnumerable<>), typeof(List<>) },
            { typeof(ICollection<>), typeof(List<>) },
            { typeof(IList<>), typeof(List<>) },
            { typeof(IDictionary<,>), typeof(Dictionary<,>) },
        };

        private static readonly Dictionary<Type, Type> DefaultNonGenericInterfaceImplementations = new Dictionary<Type, Type>
        {
            { typeof(IEnumerable), typeof(List<object>) },
            { typeof(ICollection), typeof(List<object>) },
            { typeof(IList), typeof(List<object>) },
            { typeof(IDictionary), typeof(Dictionary<object, object>) },
            { typeof(IConfig), typeof(DefaultConfigImplementation) },
        };

        /// <inheritdoc />
        public object Create(Type type)
        {
            if (type.IsInterface)
            {
                if (type.IsGenericType)
                {
                    if (DefaultGenericInterfaceImplementations.TryGetValue(type.GetGenericTypeDefinition(), out var implementationType))
                    {
                        type = implementationType.MakeGenericType(type.GetGenericArguments());
                    }
                }
                else
                {
                    if (DefaultNonGenericInterfaceImplementations.TryGetValue(type, out var implementationType))
                    {
                        type = implementationType;
                    }
                }
            }

            try
            {
                return Activator.CreateInstance(type);
            }
            catch (Exception err)
            {
                var message = $"Failed to create an instance of type '{type.FullName}'.";
                throw new InvalidOperationException(message, err);
            }
        }
    }

    /// <inheritdoc />
#pragma warning disable SA1201 // Elements should appear in the correct order
    internal struct DefaultConfigImplementation : IConfig
#pragma warning restore SA1201 // Elements should appear in the correct order
    {
        /// <inheritdoc />
        public bool IsEnabled { get; set; }
    }
}
