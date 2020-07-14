// -----------------------------------------------------------------------
// <copyright file="LogLevel.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Importance levels of log messages.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Print a grey information on the game console.
        /// </summary>
        Info,

        /// <summary>
        /// Print a green debug on the game console.
        /// </summary>
        Debug,

        /// <summary>
        /// Print a yellow warning on the game console.
        /// </summary>
        Warn,

        /// <summary>
        /// Print a red error on the game console.
        /// </summary>
        Error,
    }
}
