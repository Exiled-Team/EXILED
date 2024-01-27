// -----------------------------------------------------------------------
// <copyright file="Property{T}.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Features
{
    using System;
#pragma warning disable SA1611 // Element parameters should be documented

    using System.Collections.Generic;
    using System.Reflection;

    using Exiled.API.Interfaces;

    /// <summary>
    /// An implementation of the <see cref="IProperty"/> interface that encapsulates a property with arguments.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    public class Property<T> : IProperty
    {
        private static bool? useDynamicPatching;

        private static MethodInfo patchMethod;

        private static object patcherInstance;
        private bool patched;
        private T propertyValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="Property{T}"/> class.
        /// <param name="value"><inheritdoc cref="propertyValue"/></param>
        /// </summary>
        public Property(T value) => propertyValue = value;

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public T Value
        {
            get => propertyValue;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(propertyValue, value))
                {
                    if (useDynamicPatching is null)
                        AccessPatcherUsingReflection();

                    if (useDynamicPatching is true && !patched)
                    {
                        patchMethod.Invoke(patcherInstance, new object[] { this });
                        patched = true;
                    }

                    propertyValue = value;
                }
            }
        }

        private static Property<string> Example { get; set; } = new("Hello World");

        /// <summary>
        /// Implicitly converts the Property to its value type.
        /// </summary>
        /// <param name="property">The Property to convert.</param>
        public static implicit operator T(Property<T> property) => property.Value;

        private static void UseExample()
        {
            Example.Value = "Test";

            string value = Example;
        }

        private static void AccessPatcherUsingReflection()
        {
            if (patchMethod == null || patcherInstance == null)
            {
                foreach (Type type in Assembly.Load("Exiled.Events").GetTypes())
                {
                    if (type.Name == "Patcher")
                    {
                        // Access the Patcher instance property using reflection
                        patcherInstance = type.GetField("Instance", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);

                        if (patcherInstance != null)
                        {
                            // Call the Patch method on the patcherInstance
                            patchMethod = type.GetMethod("Patch");

                            // Instance.Config.UseDynamicPatching
                            // Call the config
                            useDynamicPatching = (bool?)type.GetProperty("Config", BindingFlags.Public | BindingFlags.Static)?.GetValue(null)?
                                .GetType()?.GetProperty("UseDynamicPatching", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
                        }
                    }
                }
            }
        }
    }
}