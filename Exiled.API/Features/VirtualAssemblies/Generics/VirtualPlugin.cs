// -----------------------------------------------------------------------
// <copyright file="VirtualPlugin.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.VirtualAssemblies
{
    using Exiled.API.Features;
    using Exiled.API.Interfaces;

    /// <inheritdoc/>
    /// <typeparam name="TConfig">The type of the plugin's config.</typeparam>
    public abstract class VirtualPlugin<TConfig> : VirtualPlugin
        where TConfig : class
    {
        /// <inheritdoc/>
        public override EConfig Config { get; protected set; } = EConfig.Get<TConfig>(true);
    }
}
