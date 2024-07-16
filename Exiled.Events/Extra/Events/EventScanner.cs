// -----------------------------------------------------------------------
// <copyright file="EventScanner.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Extra.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.Events.EventArgs.Interfaces;
    using Exiled.Events.Features;
    using Exiled.Events.Handlers;

    /// <summary>
    /// A utility class that scans for all available events.
    /// </summary>
    public static class EventScanner
    {
        private static Dictionary<Type, object> EventTypes { get; set; }

        /// <summary>
        /// Retrieve event types.
        /// </summary>
        /// <returns>Returns all event types.</returns>
        public static Dictionary<Type, object> Get()
        {
            if (EventTypes != null)
                return EventTypes;
            EventTypes = new Dictionary<Type, object>();
            ScanEvents();

            return EventTypes;
        }

        private static void ScanEvents()
        {
            Assembly exiledHandlersAssembly = typeof(Player).Assembly;

            foreach (Type type in exiledHandlersAssembly.GetTypes())
            {
                if (type.Namespace == null || !type.Namespace.StartsWith("Exiled.Events.Handlers"))
                    continue;

                FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
                PropertyInfo[] properties = type.GetProperties(BindingFlags.Static | BindingFlags.Public);

                foreach (FieldInfo field in fields)
                {
                    if (!field.FieldType.IsGenericType ||
                        field.FieldType.GetGenericTypeDefinition() != typeof(Event<>))
                        continue;
                    Type genericType = field.FieldType.GetGenericArguments().FirstOrDefault();
                    if (genericType != null && typeof(System.EventArgs).IsAssignableFrom(genericType))
                        EventTypes[genericType] = field.GetValue(null);
                }

                foreach (PropertyInfo property in properties)
                {
                    if (!property.PropertyType.IsGenericType ||
                        property.PropertyType.GetGenericTypeDefinition() != typeof(Event<>))
                        continue;
                    Type genericType = property.PropertyType.GetGenericArguments().FirstOrDefault();
                    if (genericType != null && typeof(IExiledEvent).IsAssignableFrom(genericType))
                        EventTypes[genericType] = property.GetValue(null);
                }
            }
        }
    }
}