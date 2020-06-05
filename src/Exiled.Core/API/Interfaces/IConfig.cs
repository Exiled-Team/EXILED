// -----------------------------------------------------------------------
// <copyright file="IConfig.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces
{
    /// <summary>
    /// Define the contract for basic config features.
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether the plugin is enabled or not.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Gets the plugin config prefix.
        /// </summary>
        string Prefix { get; }

        /// <summary>
        /// Reload the configs.
        /// </summary>
        void Reload();
    }
}
