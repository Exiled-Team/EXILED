// -----------------------------------------------------------------------
// <copyright file="EventManager.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------


namespace Exiled.API.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features;

    /// <summary>
    /// Creates a list of [Subscribe]'d methods alongside their first argument,
    /// Making it easy to register events and invoke them.
    /// </summary>
    public class EventManager
    {
        private class RegisteredEvent
        {
            public Type Arg;
            public dynamic Method;
            public Type Parent;
        }

        /// <summary>
        /// The instance of EventManager in use by EXILED.
        /// </summary>
        public static readonly EventManager Instance = new EventManager();

        /// <summary>
        /// A list of subscriptions attached to their event arg type.
        /// </summary>
        private readonly Dictionary<Type, dynamic> subscriptions = new Dictionary<Type, dynamic>();
        private readonly List<RegisteredEvent> licenses = new List<RegisteredEvent>();

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

            try
            {
                dynamic entry;
                if (subscriptions.TryGetValue(t, out entry))
                {
                    foreach (var sub in (entry as Delegate).GetInvocationList())
                    {
                        try
                        {
                            var other = sub as Action<T>;
                            if (other != null)
                            {
                                other(ev);

                                invoked += 1;
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error($"Failed to invoke subscriber with type {t.FullName} and exception {e}.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed during invocation of event with type {t.FullName} and exception {e}.");
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

            var act = typeof(Action).Assembly.GetType("System.Action`1");
            var parent = t.Assembly;

            foreach (var m in t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreReturn | BindingFlags.InvokeMethod))
            {
                try
                {
                    var attr = m.GetCustomAttribute<SubscribeAttribute>();
                    if (attr != null)
                    {
                        var arg = m.GetParameters().First().ParameterType;

                        var gen = act.MakeGenericType(arg);

                        var ev = new RegisteredEvent();
                        ev.Method = (Delegate)m.CreateDelegate(gen);
                        ev.Arg = arg;
                        ev.Parent = t;

                        if (!subscriptions.ContainsKey(arg))
                            subscriptions[arg] = ev.Method;
                        else
                            subscriptions[arg] = Delegate.Combine(ev.Method, subscriptions[arg]);
                        licenses.Add(ev);

                        registered += 1;
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"Failed during registration of subscriber: {e}.");
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
            foreach (var t in a.GetTypes())
                registered += Build(t);
            return registered;
        }

        /// <summary>
        /// Remove all subscriptions under Assembly a.
        /// </summary>
        /// <param name="a">The assembly to destroy subscriptions under.</param>
        /// <returns>The amount of subscriptions removed.</returns>
        public int Destroy(Assembly a)
        {
            var destroyed = 0;
            foreach (var registeredEvent in licenses)
            {
                if (registeredEvent.Parent.Assembly == a)
                {
                    if (subscriptions.ContainsKey(registeredEvent.Arg))
                    {
                        subscriptions[registeredEvent.Arg] = Delegate.Remove(subscriptions[registeredEvent.Arg], registeredEvent.Method);
                        if (subscriptions[registeredEvent.Arg] == null)
                            subscriptions.Remove(registeredEvent.Arg);
                        destroyed += 1;
                    }
                }
            }

            return destroyed;
        }

        /// <summary>
        /// Destroys all subscribers under type T.
        /// </summary>
        /// <param name="t">The type to destroy subscribers to.</param>
        /// <returns>The number of destroyed subscribers.</returns>
        public int Destroy(Type t)
        {
            var destroyed = 0;
            foreach (var registeredEvent in licenses)
            {
                if (registeredEvent.Parent == t)
                {
                    if (subscriptions.ContainsKey(registeredEvent.Arg))
                    {
                        subscriptions[registeredEvent.Arg] = Delegate.Remove(subscriptions[registeredEvent.Arg], registeredEvent.Method);
                        if (subscriptions[registeredEvent.Arg] == null)

                            // Do this so we aren't asking for NullReferenceErrors in a hot path
                            subscriptions.Remove(registeredEvent.Arg);
                        destroyed += 1;
                    }
                }
            }

            return destroyed;
        }
    }
}
