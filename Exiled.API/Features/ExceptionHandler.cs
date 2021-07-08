// -----------------------------------------------------------------------
// <copyright file="ExceptionHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.IO;

    using YamlDotNet.Core;

    /// <summary>
    /// Handles exceptions that may occur while using EXILED or plugins.
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

            switch (exception)
            {
                case MissingMethodException _:
                    message = "This error usually means EXILED or the plugin causing it are outdated, try updating.";
                    break;

                case FileNotFoundException fileNotFound:
                    message = $"The \"{fileNotFound.FileName.Split(new char[] { ',' })[0]}.dll\" file is either missing or is outdated.";
                    break;

                case YamlException _:
                    message = $"Check the line stated above. If you don't know what is the issue parse the .yml file through a YAML Validator.";
                    break;

                case BadImageFormatException _:
                    message = "The dll file stated above is likely corrupted, this can usually be fixed by re-downloading it.";
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
