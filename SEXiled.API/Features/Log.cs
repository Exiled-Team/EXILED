// -----------------------------------------------------------------------
// <copyright file="Log.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.API.Features
{
    using System.Reflection;

    using Discord;

    /// <summary>
    /// A set of tools to print messages on the server console.
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Sends a <see cref="LogLevel.Info"/> level messages to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Info(object message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", LogLevel.Info, System.ConsoleColor.Cyan);

        /// <summary>
        /// Sends a <see cref="LogLevel.Info"/> level messages to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Info(string message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", LogLevel.Info, System.ConsoleColor.Cyan);

        /// <summary>
        /// Sends a <see cref="LogLevel.Debug"/> level messages to the game console.
        /// Server must have exiled_debug config enabled.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="canBeSent">Indicates whether the log can be sent or not.</param>
        public static void Debug(object message, bool canBeSent = true)
        {
            if (canBeSent)
                Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", LogLevel.Debug, System.ConsoleColor.Green);
        }

        /// <summary>
        /// Sends a <see cref="LogLevel.Debug"/> level messages to the game console.
        /// Server must have exiled_debug config enabled.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="canBeSent">Indicates whether the log can be sent or not.</param>
        public static void Debug(string message, bool canBeSent = true)
        {
            if (canBeSent)
                Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", LogLevel.Debug, System.ConsoleColor.Green);
        }

        /// <summary>
        /// Sends a <see cref="LogLevel.Debug"/> level messages to the game console.
        /// Server must have exiled_debug config enabled.
        /// </summary>
        /// <typeparam name="T">The inputted object's type.</typeparam>
        /// <param name="object">The object to be logged and returned.</param>
        /// <param name="canBeSent">Indicates whether the log can be sent or not.</param>
        /// <returns>Returns the <typeparamref name="T"/> object inputted in <paramref name="object"/>.</returns>
        public static T DebugObject<T>(T @object, bool canBeSent = true)
        {
            if (canBeSent)
                Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {@object}", LogLevel.Debug, System.ConsoleColor.Green);

            return @object;
        }

        /// <summary>
        /// Sends a <see cref="LogLevel.Warn"/> level messages to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Warn(object message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", LogLevel.Warn, System.ConsoleColor.Magenta);

        /// <summary>
        /// Sends a <see cref="LogLevel.Warn"/> level messages to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Warn(string message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", LogLevel.Warn, System.ConsoleColor.Magenta);

        /// <summary>
        /// Sends a <see cref="LogLevel.Error"/> level messages to the game console.
        /// This should be used to send errors only.
        /// It's recommended to send any messages in the catch block of a try/catch as errors with the exception string.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Error(object message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", LogLevel.Error, System.ConsoleColor.DarkRed);

        /// <summary>
        /// Sends a <see cref="LogLevel.Error"/> level messages to the game console.
        /// This should be used to send errors only.
        /// It's recommended to send any messages in the catch block of a try/catch as errors with the exception string.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public static void Error(string message) => Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", LogLevel.Error, System.ConsoleColor.DarkRed);

        /// <summary>
        /// Sends a log message to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="level">The message level of importance.</param>
        /// <param name="color">The message color.</param>
        public static void Send(object message, LogLevel level, System.ConsoleColor color = System.ConsoleColor.Gray)
        {
            SendRaw($"[{level.ToString().ToUpper()}] {message}", color);
        }

        /// <summary>
        /// Sends a log message to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="level">The message level of importance.</param>
        /// <param name="color">The message color.</param>
        public static void Send(string message, LogLevel level, System.ConsoleColor color = System.ConsoleColor.Gray)
        {
            SendRaw($"[{level.ToString().ToUpper()}] {message}", color);
        }

        /// <summary>
        /// Sends a raw log message to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="color">The <see cref="System.ConsoleColor"/> of the message.</param>
        public static void SendRaw(object message, System.ConsoleColor color) => ServerConsole.AddLog(message.ToString(), color);

        /// <summary>
        /// Sends a raw log message to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="color">The <see cref="System.ConsoleColor"/> of the message.</param>
        public static void SendRaw(string message, System.ConsoleColor color) => ServerConsole.AddLog(message, color);

        /// <summary>
        /// Sends an <see cref="Error(object)"/> with the provided message if the condition is false and stops the execution.
        /// <example> For example:
        /// <code>
        ///     Player ply = Player.Get(2);
        ///     Log.Assert(ply != null, "The player with the id 2 is null");
        /// </code>
        /// results in it logging an error if the player is null and not continuing.
        /// </example>
        /// </summary>
        /// <param name="condition">The conditional expression to evaluate. If the condition is true it will continue.</param>
        /// <param name="message">The information message. The error and exception will show this message.</param>
        /// <exception cref="System.Exception">If the condition is false. It throws an exception stopping the execution.</exception>
        public static void Assert(bool condition, object message)
        {
            if (condition)
                return;

            Error(message);
            throw new System.Exception(message.ToString());
        }
    }
}
