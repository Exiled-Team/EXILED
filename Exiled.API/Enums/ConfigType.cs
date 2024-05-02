// -----------------------------------------------------------------------
// <copyright file="ConfigType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using System;

    /// <summary>
    /// The different types of configuration files distribution.
    /// </summary>
    public enum ConfigType
    {
        /// <summary>
        /// Separated distribution, each plugin will have an individual config file.
        /// </summary>
        Separated,

        /// <summary>
        /// Merged configs, all plugins share a config file.
        /// </summary>
        Merged,

        /// <summary>
        /// Default. Removed in the future.
        /// </summary>
        [Obsolete("Separated & Merged exist now.")]
        Default = Separated,
    }
}