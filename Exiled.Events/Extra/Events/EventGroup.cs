// -----------------------------------------------------------------------
// <copyright file="EventGroup.cs" company="Exiled Team">
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

    using Exiled.API.Features;
    using Features;

    /// <summary>
    /// A versatile tool for dynamically and bulk registering/unregistering events.
    /// </summary>
    public class EventGroup
    {
        private readonly List<Tuple<Delegate, object, MethodInfo>> dynamicHandlers = new();

        /// <summary>
        /// Gets a value indicating whether events added have been registered or not.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public bool IsHandlerAdded { get; private set; }

        /// <summary>
        /// Register all methods in each class with [Listener] attribute.
        /// </summary>
        /// <param name="classes">The instances of classes with Listeners that should be registered.</param>
        public void AddEvents(params object[] classes)
        {
            foreach (object @class in classes)
                AddEventHandlers(@class);
        }

        /// <summary>
        /// Remove all active event handlers in this event group.
        /// </summary>
        public void RemoveEvents()
        {
            RemoveEventHandlers();
        }

        /// <summary>
        /// Register all methods in class with [Listener] attribute.
        /// </summary>
        /// <param name="instance">The instance of a class with Listeners that should be registered.</param>
        public void AddEventHandlers(object instance)
        {
            IEnumerable<MethodInfo> methods = instance.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.GetCustomAttributes(typeof(ListenerAttribute), false).Length > 0);
            List<MethodInfo> methodInfos = methods.ToList();
            Log.Warn($"Beginning event handlers, total of {methodInfos.Count()} found");
            foreach (MethodInfo method in methodInfos)
            {
                Type eventType = method.GetParameters().First().ParameterType;
                object eventInstance = EventScanner.Get()[eventType];
                Type delegateType = typeof(CustomEventHandler<>).MakeGenericType(eventType);
                Delegate handler = Delegate.CreateDelegate(delegateType, instance, method);
                MethodInfo[] eventMethods = eventInstance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
                eventMethods.First(e => e.Name == "Subscribe").Invoke(eventInstance, new object[] { handler });

                dynamicHandlers.Add(Tuple.Create(handler, instance, method));
            }

            IsHandlerAdded = true;
        }

        private void RemoveEventHandlers()
        {
            if (!IsHandlerAdded)
                return;

            foreach (Tuple<Delegate, object, MethodInfo> handler in dynamicHandlers)
            {
                Type eventType = handler.Item3.GetParameters().First().ParameterType;
                object eventInstance = EventScanner.Get()[eventType];
                MethodInfo[] eventMethods = eventInstance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
                eventMethods.First(e => e.Name == "Unsubscribe").Invoke(eventInstance, new object[] { handler.Item1 });
            }

            dynamicHandlers.Clear();
            IsHandlerAdded = false;
        }
    }
}