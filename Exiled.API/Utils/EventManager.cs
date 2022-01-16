// -----------------------------------------------------------------------
// <copyright file="EventManager.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Creates a list of [Subscribe]'d methods alongside their first argument,
    /// Making it easy to register events and invoke them.
    /// </summary>
    public class EventManager
    {
        /// <summary>
        /// The instance of EventManager in use by EXILED.
        /// </summary>
        public static readonly EventManager Instance = new EventManager();

        /// <summary>
        /// A list of subscriptions attached to their event arg type.
        /// </summary>
        private readonly Dictionary<Assembly, Dictionary<Type, List<object>>> subscriptions = new Dictionary<Assembly, Dictionary<Type, List<object>>>();

        /// <summary>
        /// Invoke an event's subscribers by searching for the event argument type.
        /// </summary>
        /// <param name="ev">The specific instance of an event argument to invoke.</param>
        /// <typeparam name="T">The type of the event argument, eg PlayerDiedArgs.</typeparam>
        /// <returns>The amount of events successfully invoked.</returns>
        public int Invoke<T>(T ev)
        {
            var t = typeof(T);

            var invoked = 0;
            foreach (KeyValuePair<Assembly, Dictionary<Type, List<object>>> pair in subscriptions)
            {
                if (!pair.Value.ContainsKey(t))
                    pair.Value[t] = new List<object>();
                foreach (object sub in pair.Value[t])
                {
                    var other = sub as Action<T>;
                    if (other != null)
                    {
                        other(ev);

                        invoked += 1;
                    }
                }
            }

            return invoked;
        }

        /// <summary>
        /// Build this EventManager by compiling Subscription attributes for a specific class.
        /// </summary>
        /// <param name="t">The class to discover subscriptions under.</param>
        /// <returns>the number of subscriptions collected.</returns>
        public int Build(Type t)
        {
            var registered = 0;

            Type act = typeof(Action).Assembly.GetType("System.Action`1");
            Assembly parent = t.Assembly;

            foreach (MethodInfo m in t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreReturn | BindingFlags.InvokeMethod))
            {
                var attr = m.GetCustomAttribute<SubscribeAttribute>();
                if (attr != null)
                {
                    var arg = m.GetParameters().First().ParameterType;

                    if (!subscriptions.ContainsKey(parent))
                        subscriptions[parent] = new Dictionary<Type, List<object>>();

                    if (!subscriptions[parent].ContainsKey(arg))
                        subscriptions[parent][arg] = new List<object>(4);

                    var gen = act.MakeGenericType(arg);


                    subscriptions[parent][arg].Add(m.CreateDelegate(gen));
                    registered += 1;
                }
            }
            return registered;
        }

        /// <summary>
        /// Given the assembly, iterate over all types and create subscriptions for every SubscribeAttribute
        /// </summary>
        /// <param name="a">The assembly to scan.</param>
        /// <returns>The number of events subscribed.</returns>
        public int Build(Assembly a)
        {
            var registered = 0;
            foreach (Type t in a.GetTypes())
                registered += Build(t);
            return registered;
        }

        /// <summary>
        /// Remove all subscriptions under Assembly a.
        /// </summary>
        /// <param name="a">The assembly to destroy subscriptions under.</param>
        /// <returns>Whether or not any subscriptions were removed.</returns>
        public bool Destroy(Assembly a)
        {
            if (subscriptions.ContainsKey(a))
            {
                return subscriptions.Remove(a);
            }
            return false;
        }
    }
}
