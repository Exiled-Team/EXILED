// -----------------------------------------------------------------------
// <copyright file="MainClass.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network
{
    using Exiled.API.Features;

    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    /// <inheritdoc/>
    public class MainClass : Plugin<PluginConfig>
    {
        /// <inheritdoc/>
        public override string Name { get; } = "Exiled.Network";

        /// <inheritdoc/>
        public override string Prefix { get; } = "exiled.network";

        /// <inheritdoc/>
        public override string Author { get; } = "Killers0992";

        private NPClient client;
        private NPHost host;

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Log.Info("Enabling NetworkedPlugins...");
            if (Config.IsHost)
                host = new NPHost(this);
            else
                client = new NPClient(this);
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            client = null;
            host = null;
        }
    }
}
