// -----------------------------------------------------------------------
// <copyright file="NPLogger.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API
{
    /// <summary>
    /// Network logger.
    /// </summary>
    public abstract class NPLogger
    {
        /// <summary>
        /// Sends a info level messages to the game console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public abstract void Info(string message);

        /// <summary>
        /// Sends a Error level messages to the game console.
        /// This should be used to send errors only.
        /// It's recommended to send any messages in the catch block of a try/catch as errors with the exception string.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public abstract void Error(string message);

        /// <summary>
        /// Sends a Debug level messages to the game console.
        /// Server must have exiled_debug config enabled.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public abstract void Debug(string message);
    }
}
