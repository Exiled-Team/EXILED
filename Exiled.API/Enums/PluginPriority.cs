// -----------------------------------------------------------------------
// <copyright file="PluginPriority.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Provides simple and readable plugin priority values.
    /// </summary>
    /// <seealso cref="Interfaces.IPlugin{TConfig}.Priority"/>
    public enum PluginPriority
    {
        /// <inheritdoc cref="Medium"/>
        Default = Medium,

        /// <summary>
        /// Execute the plugin last, after other ones.
        /// </summary>
        Last = int.MinValue,

        /// <inheritdoc cref="Last"/>
        Lowest = Last,

        /// <summary>
        /// Default plugin priority.
        /// </summary>
        Lower = -500,

        /// <summary>
        /// Low plugin priority.
        /// </summary>
        Low = -250,

        /// <summary>
        /// Medium plugin priority.
        /// </summary>
        Medium = 0,

        /// <summary>
        /// Higher plugin priority.
        /// </summary>
        High = 250,

        /// <summary>
        /// Higher plugin priority.
        /// </summary>
        Higher = 500,

        /// <inheritdoc cref="First"/>
        Highest = First,

        /// <summary>
        /// Execute the plugin first, before other ones.
        /// </summary>
        First = int.MaxValue,
    }
}