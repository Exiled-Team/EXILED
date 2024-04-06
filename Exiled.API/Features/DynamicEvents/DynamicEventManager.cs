// -----------------------------------------------------------------------
// <copyright file="DynamicEventManager.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.DynamicEvents
{
    using System;
    using System.Reflection;

    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core;

    /// <summary>
    /// The class which handles all the multicast delegates.
    /// </summary>
    public class DynamicEventManager : StaticActor
    {
        /// <summary>
        /// The custom <see cref="EventHandler"/> delegate.
        /// </summary>
        /// <typeparam name="T">The <see cref="EventHandler{T}"/> type.</typeparam>
        /// <param name="ev">The <see cref="EventHandler{T}"/> instance.</param>
        public delegate void FDelegate<T>(T ev)
            where T : EventArgs;

        /// <summary>
        /// The custom <see cref="EventHandler"/> delegate, with empty parameters.
        /// </summary>
        public delegate void FDelegate();

        /// <inheritdoc cref="BindAllFromTypeInstance(object)"/>
        public static void CreateFromTypeInstance(object obj) => Get<DynamicEventManager>().BindAllFromTypeInstance(obj);

        /// <inheritdoc cref="UnbindAllFromTypeInstance(object)"/>
        public static void DestroyFromTypeInstance(object obj) => Get<DynamicEventManager>().UnbindAllFromTypeInstance(obj);

        /// <summary>
        /// Executes all <see cref="FDelegate{T}"/> listeners safely.
        /// </summary>
        /// <typeparam name="T">Event arg type.</typeparam>
        /// <param name="ev">Source event.</param>
        /// <param name="arg">Event arg.</param>
        /// <exception cref="ArgumentNullException">Event or its arg is null.</exception>
        public void InvokeSafely<T>(FDelegate<T> ev, T arg)
            where T : EventArgs
        {
            if (ev is null)
                return;

            string eventName = ev.GetType().FullName;

            foreach (Delegate @delegate in ev.GetInvocationList())
            {
                try
                {
                    FDelegate<T> handler = (FDelegate<T>)@delegate;
                    handler(arg);
                }
                catch (Exception ex)
                {
                    Log.Error($"{ex}, {@delegate.Method.Name}, {@delegate.Method.ReflectedType?.FullName}, {eventName}");
                }
            }
        }

        /// <summary>
        /// Executes all <see cref="FDelegate{T}"/> listeners safely.
        /// </summary>
        /// <param name="ev">Source event.</param>
        /// <exception cref="ArgumentNullException">Event is null.</exception>
        public void InvokeSafely(FDelegate ev)
        {
            if (ev is null)
                return;

            string eventName = ev.GetType().FullName;

            foreach (Delegate @delegate in ev.GetInvocationList())
            {
                try
                {
                    FDelegate handler = (FDelegate)@delegate;
                    handler();
                }
                catch (Exception ex)
                {
                    Log.Error($"{ex}, {@delegate.Method.Name}, {@delegate.Method.ReflectedType?.FullName}, {eventName}");
                }
            }
        }

        /// <summary>
        /// Initializes all the dynamic handlers in the specified type instance.
        /// </summary>
        /// <param name="obj">The type instance.</param>
        public void BindAllFromTypeInstance(object obj)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (FieldInfo field in obj.GetType().GetFields(flags))
            {
                if (field.GetCustomAttribute<DynamicEventDispatcherAttribute>() is not null)
                    field.SetValue(obj, Activator.CreateInstance(field.FieldType, flags, null, null, null), flags, null, null);
            }

            foreach (PropertyInfo property in obj.GetType().GetProperties(flags))
            {
                if (property.GetCustomAttribute<DynamicEventDispatcherAttribute>() is not null)
                    property.SetValue(obj, Activator.CreateInstance(property.PropertyType, flags, null, null, null), flags, null, null, null);
            }
        }

        /// <summary>
        /// Unbinds all the dynamic handlers in the specified type instance.
        /// </summary>
        /// <param name="obj">The type instance.</param>
        public void UnbindAllFromTypeInstance(object obj)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (FieldInfo field in obj.GetType().GetFields(flags))
            {
                if (field.GetValue(obj) is IDynamicEventDispatcher ev)
                    ev.UnbindAll();
            }

            foreach (PropertyInfo property in obj.GetType().GetProperties(flags))
            {
                try
                {
                    if (property.GetValue(obj) is IDynamicEventDispatcher ev)
                        ev.UnbindAll();
                }
                catch
                {
                    continue;
                }
            }
        }
    }
}