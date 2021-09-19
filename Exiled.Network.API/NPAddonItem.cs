// -----------------------------------------------------------------------
// <copyright file="NPAddonItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API
{
    using Exiled.Network.API.Attributes;
    using Exiled.Network.API.Interfaces;

    /// <summary>
    /// Addon info.
    /// </summary>
    public class NPAddonItem
    {
        /// <summary>
        /// Gets or sets addon interface.
        /// </summary>
        public IAddon<IConfig> Addon { get; set; }

        /// <summary>
        /// Gets or sets addon info.
        /// </summary>
        public NPAddonInfo Info { get; set; }
    }
}
