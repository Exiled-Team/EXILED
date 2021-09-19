// -----------------------------------------------------------------------
// <copyright file="IConfig.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API.Interfaces
{
    /// <summary>
    /// Config for addon.
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether gets or sets.
        /// </summary>
        bool IsEnabled { get; set; }
    }
}
