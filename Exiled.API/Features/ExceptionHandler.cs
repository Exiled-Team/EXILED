// -----------------------------------------------------------------------
// <copyright file="ExceptionHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;

    /// <summary>
    /// .
    /// </summary>
    public static class ExceptionHandler
    {
        /// <summary>
        /// Tries to send a helpful message when the certain <see cref="Exception"/> occurs.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> for which the solution should be found.</param>
        /// <returns><see langword="true"/> if the <see cref="Exception"/> has help info built-in and a help message was send, otherwise <see langword="false"/>.</returns>
        public static bool TrySendingHelpMessage(Exception exception)
        {
            string message = null;

            switch (exception.GetType().Name)
            {
                case "MissingMethodException":
                    message = "This error can be fixed via updating Exiled or plugin to the latest version.";
                    break;

                default:
                    break;
            }

            if (message != null)
            {
                Log.Send(message, Discord.LogLevel.Warn, ConsoleColor.DarkYellow);
                return true;
            }
            else
            {
                #if DEBUG
                Log.Send(exception.GetType().Name, Discord.LogLevel.Warn, ConsoleColor.DarkYellow);
                #endif

                return false;
            }
        }
    }
}
