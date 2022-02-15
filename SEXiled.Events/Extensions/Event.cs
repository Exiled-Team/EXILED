// -----------------------------------------------------------------------
// <copyright file="Event.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Extensions
{
    using System;

    using SEXiled.API.Features;

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
        public static void InvokeSafely<T>(this Events.CustomEventHandler<T> ev, T arg)
            where T : EventArgs
        {
            if (ev == null)
                return;

            string eventName = ev.GetType().FullName;
            foreach (Events.CustomEventHandler<T> handler in ev.GetInvocationList())
            {
                try
                {
                    handler(arg);
                }
                catch (Exception ex)
                {
                    LogException(ex, handler.Method.Name, handler.Method.ReflectedType.FullName, eventName);
                }
            }
        }

        /// <summary>
        /// Executes all <see cref="Events.CustomEventHandler"/> listeners safely.
        /// </summary>
        /// <param name="ev">Source event.</param>
        /// <exception cref="ArgumentNullException">Event is <see langword="null"/>.</exception>
        public static void InvokeSafely(this Events.CustomEventHandler ev)
        {
            if (ev == null)
                return;

            string eventName = ev.GetType().FullName;
            foreach (Events.CustomEventHandler handler in ev.GetInvocationList())
            {
                try
                {
                    handler();
                }
                catch (Exception ex)
                {
                    LogException(ex, handler.Method.Name, handler.Method.ReflectedType?.FullName, eventName);
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
