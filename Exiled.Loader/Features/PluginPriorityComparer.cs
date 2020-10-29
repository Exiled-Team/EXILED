// -----------------------------------------------------------------------
// <copyright file="PluginPriorityComparer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader.Features
{
    using System.Collections.Generic;

    using Exiled.API.Interfaces;

    /// <summary>
    /// Comparator implementation according to plugin priorities.
    /// </summary>
    public sealed class PluginPriorityComparer : IComparer<IPlugin<IConfig>>
    {
        /// <summary>
        /// Public instance.
        /// </summary>
        public static readonly PluginPriorityComparer Instance = new PluginPriorityComparer();

        /// <inheritdoc/>
        public int Compare(IPlugin<IConfig> x, IPlugin<IConfig> y)
        {
            return x.Priority.CompareTo(y.Priority);
        }
    }
}
