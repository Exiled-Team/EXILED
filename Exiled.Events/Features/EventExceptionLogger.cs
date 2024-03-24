// -----------------------------------------------------------------------
// <copyright file="EventExceptionLogger.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Features
{
    using System;
    using System.Reflection;

    using Exiled.API.Features;

    /// <summary>
    /// A class that logs exceptions that occur when handling events.
    /// </summary>
    public static class EventExceptionLogger
    {
        /// <summary>
        /// Logs an exception that occurred when handling an event.
        /// </summary>
        /// <param name="ex">The exception that occurred.</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="method">The method that caused the exception.</param>
        public static void CaptureException(Exception ex, string eventName, MethodInfo method)
        {
            Log.Error($"Method \"{method.Name}\" of the class \"{method.ReflectedType.FullName}\" caused an exception when handling the event \"{eventName}\"\n{ex}");
        }
    }
}