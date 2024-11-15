// -----------------------------------------------------------------------
// <copyright file="VirtualPlugin.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.VirtualAssemblies.Generics
{
    /// <inheritdoc/>
    /// <typeparam name="TConfig">The type of the plugin's config.</typeparam>
    public abstract class VirtualPlugin<TConfig> : VirtualPlugin
        where TConfig : class
    {
        /// <inheritdoc/>
        public override ConfigSubsystem Config { get; protected set; } = ConfigSubsystem.Get<TConfig>(true);
    }
}
