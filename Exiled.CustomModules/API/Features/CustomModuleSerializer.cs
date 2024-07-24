// -----------------------------------------------------------------------
// <copyright file="CustomModuleSerializer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features
{
    using System;
    using System.Reflection;

    using Attributes;
    using Generic;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <summary>
    /// A IYamlTypeConverter for custom modules.
    /// </summary>
    public class CustomModuleSerializer : IYamlTypeConverter
    {
        /// <inheritdoc />
        public bool Accepts(Type type)
        {
            return type == typeof(ModulePointer) || type == typeof(ModulePointer<>);
        }

        /// <inheritdoc />
        public object ReadYaml(IParser parser, Type type)
        {
            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(ModulePointer<>))
                throw new InvalidOperationException($"Unsupported type: {type.FullName}");
            Type genericArgument = type.GetGenericArguments()[0];
            MethodInfo method = GetType().GetMethod(nameof(ReadYamlGeneric), BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo genericMethod = method?.MakeGenericMethod(genericArgument);
            return genericMethod?.Invoke(this, new object[] { parser });
        }

        /// <inheritdoc />
        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(ModulePointer<>))
                throw new ArgumentException("The type must be the generic ModulePointer.");
            Type genericArgument = type.GetGenericArguments()[0];
            MethodInfo method = GetType().GetMethod(nameof(WriteYamlGeneric), BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo genericMethod = method?.MakeGenericMethod(genericArgument);
            genericMethod?.Invoke(this, new[] { emitter, value });
        }

        private void WriteYamlGeneric<TType>(IEmitter emitter, object value)
            where TType : CustomModule
        {
            if (value is not ModulePointer<TType> pointer)
            {
                throw new ArgumentNullException(nameof(pointer), @"Value is not a valid ModulePointer.");
            }

            emitter.Emit(new MappingStart(null, null, false, MappingStyle.Block));

            emitter.Emit(new Scalar(null, "id"));
            emitter.Emit(new Scalar(null, pointer.Id.ToString()));

            emitter.Emit(new Scalar(null, "module"));
            emitter.Emit(new Scalar(null, typeof(TType).Name));

            emitter.Emit(new Scalar(null, "assembly"));
            emitter.Emit(new Scalar(null, pointer.GetType().Assembly.FullName));

            emitter.Emit(new MappingEnd());
        }

        private object ReadYamlGeneric<TType>(IParser parser)
            where TType : CustomModule
        {
            parser.Consume<MappingStart>();

            parser.Consume<Scalar>(); // id key
            string id = parser.Consume<Scalar>().Value;

            parser.Consume<Scalar>(); // module key
            string module = parser.Consume<Scalar>().Value;

            parser.Consume<Scalar>(); // assembly key
            string assemblyName = parser.Consume<Scalar>().Value;

            parser.Consume<MappingEnd>();

            // Load the specified assembly
            Assembly assembly = Assembly.Load(assemblyName);
            if (assembly == null)
            {
                throw new InvalidOperationException($"Assembly '{assemblyName}' could not be loaded.");
            }

            // Create an instance of the appropriate ModulePointer<TType> subclass
            foreach (Type t in assembly.GetTypes())
            {
                ModuleIdentifierAttribute moduleIdentifier = t.GetCustomAttribute<ModuleIdentifierAttribute>();
                if (moduleIdentifier == null || moduleIdentifier.Id != uint.Parse(id) || !typeof(ModulePointer<TType>).IsAssignableFrom(t))
                    continue;
                ModulePointer<TType> instance = Activator.CreateInstance(t) as ModulePointer<TType>;
                if (instance == null)
                    continue;
                instance.Id = Convert.ToUInt32(id);
                return instance;
            }

            throw new InvalidOperationException($"Could not find a suitable ModulePointer<{typeof(TType).Name}> type for id {id}");
        }
    }
}