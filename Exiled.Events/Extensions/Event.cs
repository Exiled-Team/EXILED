// -----------------------------------------------------------------------
// <copyright file="Event.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Extensions
{
    using System;

    using API.Features;

    using EventArgs.Interfaces;

    /// <summary>
    /// A set of tools to execute events safely and without breaking other plugins.
    /// </summary>
    public static class Event
    {
        /// <summary>
        /// Executes all <see cref="Events.CustomEventHandler{TEventArgs}"/> listeners safely.
        /// </summary>
        /// <typeparam name="T">Event arg type.</typeparam>
        /// <param name="ev">Source event.</param>
        /// <param name="arg">Event arg.</param>
        /// <exception cref="ArgumentNullException">Event or its arg is <see langword="null"/>.</exception>
        /// <returns>Returns a value indicating whether the game code after the event is allowed to be executed or not.</returns>
        public static bool InvokeSafely<T>(this Events.CustomEventHandler<T> ev, T arg)
            where T : IExiledEvent
        {
            if (ev is null)
                return true;

            string eventName = ev.GetType().FullName;

            foreach (Delegate @delegate in ev.GetInvocationList())
            {
                try
                {
                    Events.CustomEventHandler<T> handler = (Events.CustomEventHandler<T>)@delegate;
                    handler(arg);
                }
                catch (Exception ex)
                {
                    LogException(ex, @delegate.Method.Name, @delegate.Method.ReflectedType?.FullName, eventName);
                }
            }

            return arg is not IDeniableEvent deniableEv || deniableEv.IsAllowed;
        }

        /// <summary>
        /// Executes all <see cref="Events.CustomEventHandler"/> listeners safely.
        /// </summary>
        /// <param name="ev">Source event.</param>
        /// <exception cref="ArgumentNullException">Event is <see langword="null"/>.</exception>
        public static void InvokeSafely(this Events.CustomEventHandler ev)
        {
            if (ev is null)
                return;

            string eventName = ev.GetType().FullName;

            foreach (Delegate @delegate in ev.GetInvocationList())
            {
                try
                {
                    Events.CustomEventHandler handler = (Events.CustomEventHandler)@delegate;
                    handler();
                }
                catch (Exception ex)
                {
                    LogException(ex, @delegate.Method.Name, @delegate.Method.ReflectedType?.FullName, eventName);
                }
            }
        }

        private static void LogException(Exception ex, string methodName, string sourceClassName, string eventName)
        {
            Log.Error($"Method \"{methodName}\" of the class \"{sourceClassName}\" caused an exception when handling the event \"{eventName}\"");
            Log.Error(ex);
        }
    }
}