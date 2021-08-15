// -----------------------------------------------------------------------
// <copyright file="AddonConfig.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.NetworkExample
{
    using Exiled.Network.API.Interfaces;

    /// <summary>
    /// Config for example addon.
    /// </summary>
    public class AddonConfig : IConfig
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;
    }
}
