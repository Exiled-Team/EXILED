// -----------------------------------------------------------------------
// <copyright file="Log.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// A set of tools to print messages to the server console.
    /// </summary>
    public static class Log
    {
#pragma warning disable SA1310
        /// <summary>
        /// Represents critical context, used for critical errors and issues.
        /// </summary>
        public const string CONTEXT_CRITICAL = "CRITICAL";

        /// <summary>
        /// Represents database context, used for database-related operations and issues.
        /// </summary>
        public const string CONTEXT_DATABASE = "DATABASE";

        /// <summary>
        /// Represents network context, used for network-related operations and issues.
        /// </summary>
        public const string CONTEXT_NETWORK = "NETWORK";

        /// <summary>
        /// Represents authentication context, used for authentication and authorization-related operations.
        /// </summary>
        public const string CONTEXT_AUTHENTICATION = "AUTHENTICATION";

        /// <summary>
        /// Represents configuration context, used for configuration-related operations and issues.
        /// </summary>
        public const string CONTEXT_CONFIGURATION = "CONFIGURATION";

        /// <summary>
        /// Represents performance context, used for performance-related monitoring and issues.
        /// </summary>
        public const string CONTEXT_PERFORMANCE = "PERFORMANCE";

        /// <summary>
        /// Represents security context, used for security-related operations and issues.
        /// </summary>
        public const string CONTEXT_SECURITY = "SECURITY";

        /// <summary>
        /// Represents API context, used for API-related operations and issues.
        /// </summary>
        public const string CONTEXT_API = "API";

        /// <summary>
        /// Represents logic context, used for core application logic operations and issues.
        /// </summary>
        public const string CONTEXT_LOGIC = "LOGIC";

        /// <summary>
        /// Represents I/O context, used for input/output operations and issues.
        /// </summary>
        public const string CONTEXT_IO = "IO";

        /// <summary>
        /// Represents serialization context, used for de/serialize operations and issues.
        /// </summary>
        public const string CONTEXT_SERIALIZATION = "SERIALIZATION";
#pragma warning restore

        /// <summary>
        /// Gets a <see cref="HashSet{T}"/> of plugin assemblies that have debug logs enabled.
        /// </summary>
        public static HashSet<Assembly> DebugEnabled { get; } = new();

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Info"/> level messages to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Info(object message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", Discord.LogLevel.Info, ConsoleColor.Cyan);

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Info"/> level messages to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Info(string message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", Discord.LogLevel.Info, ConsoleColor.Cyan);

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Debug"/> level messages to the game console.
        /// Server must have exiled_debug config enabled.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="traceMethod">A value indicating whether the method invoking the debug function should be traced.</param>
        public static void Debug(object message, bool traceMethod = false)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();

#if DEBUG
            if (callingAssembly.GetName().Name is "Exiled.API")
            {
                Send($"[{callingAssembly.GetName().Name}{(traceMethod ? $"::{new StackFrame(1, false).GetMethod()}] {message}" : $"] {message}")}", Discord.LogLevel.Debug, ConsoleColor.Green);
                return;
            }
#endif

            if (DebugEnabled.Contains(callingAssembly))
                Send($"[{callingAssembly.GetName().Name}{(traceMethod ? $"::{new StackFrame(1, false).GetMethod()}] {message}" : $"] {message}")}", Discord.LogLevel.Debug, ConsoleColor.Green);
        }

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Debug"/> level messages to the game console.
        /// Server must have exiled_debug config enabled.
        /// </summary>
        /// <typeparam name="T">The inputted object's type.</typeparam>
        /// <param name="object">The object to be logged and returned.</param>
        /// <returns>Returns the <typeparamref name="T"/> object inputted in <paramref name="object"/>.</returns>
        public static T DebugObject<T>(T @object)
        {
            Debug(@object);

            return @object;
        }

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Debug"/> level messages to the game console.
        /// Server must have exiled_debug config enabled.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="traceMethod">A value indicating whether the method invoking the debug function should be traced.</param>
        public static void Debug(string message, bool traceMethod = false)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
#if DEBUG
            if (callingAssembly.GetName().Name is "Exiled.API")
            {
                Send($"[{callingAssembly.GetName().Name}{(traceMethod ? $"::{new StackFrame(1, false).GetMethod()}] {message}" : $"] {message}")}", Discord.LogLevel.Debug, ConsoleColor.Green);
                return;
            }
#endif

            if (DebugEnabled.Contains(callingAssembly))
                Send($"[{callingAssembly.GetName().Name}{(traceMethod ? $"::{new StackFrame(1, false).GetMethod()}] {message}" : $"] {message}")}", Discord.LogLevel.Debug, ConsoleColor.Green);
        }

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Warn"/> level messages to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Warn(object message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", Discord.LogLevel.Warn, ConsoleColor.Magenta);

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Warn"/> level messages to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Warn(string message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", Discord.LogLevel.Warn, ConsoleColor.Magenta);

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Error"/> level messages to the game console.
        /// This should be used to send errors only.
        /// It's recommended to send any messages in the catch block of a try/catch as errors with the exception string.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Error(object message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", Discord.LogLevel.Error, ConsoleColor.DarkRed);

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Error"/> level messages to the game console.
        /// This should be used to send errors only.
        /// It's recommended to send any messages in the catch block of a try/catch as errors with the exception string.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Error(string message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", Discord.LogLevel.Error, ConsoleColor.DarkRed);

        /// <summary>
        /// Sends a log message to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="level">The message level of importance.</param>
        /// <param name="color">The message color.</param>
        public static void Send(object message, Discord.LogLevel level, ConsoleColor color = ConsoleColor.Gray)
        {
            SendRaw($"[{level.ToString().ToUpper()}] {message}", color);
        }

        /// <summary>
        /// Sends a log message to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="level">The message level of importance.</param>
        /// <param name="color">The message color.</param>
        public static void Send(string message, Discord.LogLevel level, ConsoleColor color = ConsoleColor.Gray)
        {
            SendRaw($"[{level.ToString().ToUpper()}] {message}", color);
        }

        /// <summary>
        /// Sends a raw log message to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="color">The <see cref="ConsoleColor"/> of the message.</param>
        public static void SendRaw(object message, ConsoleColor color) => ServerConsole.AddLog(message.ToString(), color);

        /// <summary>
        /// Sends a raw log message to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="color">The <see cref="ConsoleColor"/> of the message.</param>
        public static void SendRaw(string message, ConsoleColor color) => ServerConsole.AddLog(message, color);

        /// <summary>
        /// Sends an <see cref="Error(object)"/> with the provided message if the condition is false and stops the execution.
        /// <example> For example:
        /// <code>
        /// Player ply = Player.Get(2);
        /// Log.Assert(ply is not null, "The player with the id 2 is null");
        /// </code>
        /// results in it logging an error if the player is null and not continuing.
        /// </example>
        /// </summary>
        /// <param name="condition">The conditional expression to evaluate. If the condition is true it will continue.</param>
        /// <param name="message">The information message. The error and exception will show this message.</param>
        /// <exception cref="Exception">If the condition is false. It throws an exception stopping the execution.</exception>
        public static void Assert(bool condition, object message)
        {
            if (condition)
                return;

            Error(message);

            throw new Exception(message.ToString());
        }

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Info"/> level message to the game console with context.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="context">Additional context to include in the log.</param>
        public static void InfoWithContext(object message, string context = null)
        {
            context ??= GetContextInfo();
            Send($"[{Assembly.GetCallingAssembly().GetName().Name}] [{context}] {message}", Discord.LogLevel.Info, ConsoleColor.Cyan);
        }

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Info"/> level message to the game console with context.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="context">Additional context to include in the log.</param>
        public static void InfoWithContext(string message, string context = null)
        {
            context ??= GetContextInfo();
            Send($"[{Assembly.GetCallingAssembly().GetName().Name}] [{context}] {message}", Discord.LogLevel.Info, ConsoleColor.Cyan);
        }

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Debug"/> level message to the game console with context.
        /// Server must have exiled_debug config enabled.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="context">Additional context to include in the log.</param>
        public static void DebugWithContext(object message, string context = null)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            context ??= GetContextInfo();
#if DEBUG
            if (callingAssembly.GetName().Name is "Exiled.API")
            {
                Send($"[{callingAssembly.GetName().Name}] [{context}] {message}", Discord.LogLevel.Debug, ConsoleColor.Green);
                return;
            }
#endif

            if (DebugEnabled.Contains(callingAssembly))
                Send($"[{callingAssembly.GetName().Name}] [{context}] {message})", Discord.LogLevel.Debug, ConsoleColor.Green);
        }

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Debug"/> level message to the game console with context.
        /// Server must have exiled_debug config enabled.
        /// </summary>
        /// <typeparam name="T">The inputted object's type.</typeparam>
        /// <param name="object">The object to be logged and returned.</param>
        /// <param name="context">Additional context to include in the log.</param>
        /// <returns>Returns the <typeparamref name="T"/> object inputted in <paramref name="object"/>.</returns>
        public static T DebugObjectWithContext<T>(T @object, string context = null)
        {
            DebugWithContext(@object, context);

            return @object;
        }

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Debug"/> level message to the game console with context.
        /// Server must have exiled_debug config enabled.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="context">Additional context to include in the log.</param>
        public static void DebugWithContext(string message, string context = null)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            context ??= GetContextInfo();
#if DEBUG
            if (callingAssembly.GetName().Name is "Exiled.API")
            {
                Send($"[{callingAssembly.GetName().Name}] [{context}] {message})", Discord.LogLevel.Debug, ConsoleColor.Green);
                return;
            }
#endif

            if (DebugEnabled.Contains(callingAssembly))
                Send($"[{callingAssembly.GetName().Name}] [{context}] {message})", Discord.LogLevel.Debug, ConsoleColor.Green);
        }

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Warn"/> level message to the game console with context.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="context">Additional context to include in the log.</param>
        public static void WarnWithContext(object message, string context = null)
        {
            context ??= GetContextInfo();
            Send($"[{Assembly.GetCallingAssembly().GetName().Name}] [{context}] {message}", Discord.LogLevel.Warn, ConsoleColor.Magenta);
        }

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Warn"/> level message to the game console with context.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="context">Additional context to include in the log.</param>
        public static void WarnWithContext(string message, string context = null)
        {
            context ??= GetContextInfo();
            Send($"[{Assembly.GetCallingAssembly().GetName().Name}] [{context}] {message}", Discord.LogLevel.Warn, ConsoleColor.Magenta);
        }

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Error"/> level message to the game console with context.
        /// This should be used to send errors only.
        /// It's recommended to send any messages in the catch block of a try/catch as errors with the exception string.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="context">Additional context to include in the log.</param>
        public static void ErrorWithContext(object message, string context = null)
        {
            context ??= GetContextInfo();
            Send($"[{Assembly.GetCallingAssembly().GetName().Name}] [{context}] {message}", Discord.LogLevel.Error, ConsoleColor.DarkRed);
        }

        /// <summary>
        /// Sends a <see cref="Discord.LogLevel.Error"/> level message to the game console with context.
        /// This should be used to send errors only.
        /// It's recommended to send any messages in the catch block of a try/catch as errors with the exception string.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="context">Additional context to include in the log.</param>
        public static void ErrorWithContext(string message, string context = null)
        {
            context ??= GetContextInfo();
            Send($"[{Assembly.GetCallingAssembly().GetName().Name}] [{context}] {message}", Discord.LogLevel.Error, ConsoleColor.DarkRed);
        }

        /// <summary>
        /// Sends an <see cref="Error(object)"/> with the provided message if the condition is false and stops the execution, including context information.
        /// <example> For example:
        /// <code>
        /// Player ply = Player.Get(2);
        /// Log.AssertWithContext(ply is not null, "The player with the id 2 is null", "GameLogic");
        /// </code>
        /// results in it logging an error if the player is null and not continuing.
        /// </example>
        /// </summary>
        /// <param name="condition">The conditional expression to evaluate. If the condition is true it will continue.</param>
        /// <param name="message">The information message. The error and exception will show this message.</param>
        /// <param name="context">Additional context to include in the log.</param>
        /// <exception cref="Exception">If the condition is false. It throws an exception stopping the execution.</exception>
        public static void AssertWithContext(bool condition, object message, string context = null)
        {
            if (condition)
                return;

            ErrorWithContext(message, context);

            throw new Exception(message.ToString());
        }

        private static string GetContextInfo()
        {
            StackFrame stackFrame = new(2, true);
            MethodBase method = stackFrame.GetMethod();
            string methodName = method?.Name ?? "UnknownMethod";
            string className = method?.DeclaringType?.Name ?? "UnknownClass";
            int lineNumber = stackFrame.GetFileLineNumber();

            return $"[{className}::{methodName} at line {lineNumber}]";
        }
    }
}