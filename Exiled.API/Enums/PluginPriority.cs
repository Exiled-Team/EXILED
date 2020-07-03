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
    public enum PluginPriority
    {
        /// <summary>
        /// Execute the plugin first, before other ones.
        /// </summary>
        First = int.MinValue,

        /// <inheritdoc cref="First"/>
        Lowest = First,

        /// <inheritdoc cref="Lower"/>
        Default = Lower,

        /// <summary>
        /// Default plugin priority.
        /// </summary>
        Lower = 0,

        /// <summary>
        /// Low plugin priority.
        /// </summary>
        Low = 250,

        /// <summary>
        /// Medium plugin priority.
        /// </summary>
        Medium = 500,

        /// <summary>
        /// Higher plugin priority.
        /// </summary>
        High = 750,

        /// <summary>
        /// Higher plugin priority.
        /// </summary>
        Higher = 1000,

        /// <inheritdoc cref="Last"/>
        Highest = Last,

        /// <summary>
        /// Execute the plugin last, after other ones.
        /// </summary>
        Last = int.MaxValue,
    }
}
