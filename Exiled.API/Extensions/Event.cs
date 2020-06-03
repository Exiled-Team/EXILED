// -----------------------------------------------------------------------
// <copyright file="Event.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;
    using System.Reflection;
    using Exiled.API.Features;

    /// <summary>
    /// A set of tools to execute events safely and without breaking other plugins.
    /// </summary>
    public static class Event
    {
        /// <summary>
        /// Executes all <see cref="MulticastDelegate"/> listeners safely.
        /// </summary>
        /// <param name="ev">Delegates to be invoked.</param>
        /// <param name="args">Delegates arguments.</param>
        public static void InvokeSafely(this MulticastDelegate ev, params object[] args)
        {
            // Just return, this will isAllowed you not to use null-conditional operator at each event
            if (ev == null)
                return;

            foreach (var handler in ev.GetInvocationList())
                HandleSafely(ev.GetType().FullName, handler.Method, handler.Target, args);
        }

        /// <summary>
        /// Executes a <see cref="MethodInfo"/> safely.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="action">The method to invoke.</param>
        /// <param name="instance">The instance that invoked the method.</param>
        /// <param name="args">The method arguments.</param>
        private static void HandleSafely(string eventName, MethodInfo action, object instance, params object[] args)
        {
            try
            {
                action.Invoke(instance, args);
            }
            catch (Exception ex)
            {
                Log.Error($"Method \"{action.Name}\" of the class \"{action.ReflectedType.FullName}\" caused an error when processing the event \"{eventName}\": {ex.InnerException} {ex.Message}");
                Log.Error(ex.StackTrace);
            }
        }
    }
}
