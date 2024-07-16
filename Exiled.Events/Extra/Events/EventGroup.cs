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
        /// Gets a value indicating whether events have been registered.
        /// </summary>
        public bool IsHandlerAdded { get; private set; }

        /// <summary>
        /// Registers all methods in the specified instances marked with <see cref="ListenerAttribute"/>.
        /// </summary>
        /// <param name="classes">The instances of classes containing listener methods to be registered.</param>
        public void AddEvents(params object[] classes)
        {
            foreach (object @class in classes)
                AddEventHandlers(@class);
        }

        /// <summary>
        /// Unregisters all currently active event handlers in this event group.
        /// </summary>
        public void RemoveEvents()
        {
            RemoveEventHandlers();
        }

        /// <summary>
        /// Registers all methods in the specified instance that have the [Listener] attribute.
        /// </summary>
        /// <param name="instance">The instance containing listener methods to be registered.</param>
        public void AddEventHandlers(object instance)
        {
            IEnumerable<MethodInfo> methods = instance.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.GetCustomAttributes(typeof(ListenerAttribute), false).Length > 0);

            List<MethodInfo> methodInfos = methods.ToList();
            Log.Warn($"Beginning event handlers, total of {methodInfos.Count} found");
            
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

        /// <summary>
        /// Unregisters all event handlers that were previously registered.
        /// </summary>
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
