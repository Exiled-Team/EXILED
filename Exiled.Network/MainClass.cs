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
        private NPClient client;
        private NPHost host;

        /// <summary>
        /// Gets or Sets singleton of main plugin class.
        /// </summary>
        public static MainClass Singleton { get; set; }

        /// <inheritdoc/>
        public override string Name { get; } = "Exiled.Network";

        /// <inheritdoc/>
        public override string Prefix { get; } = "exiled_network";

        /// <inheritdoc/>
        public override string Author { get; } = "Exiled Team";

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
            if (client != null)
            {
                client.Unload();
                client = null;
            }

            if (host != null)
            {
                host.Unload();
                host = null;
            }
        }
    }
}
