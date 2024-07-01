// -----------------------------------------------------------------------
// <copyright file="LoaderPlugin.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader
{
    using System.IO;
    using System.Reflection;

    using MEC;

    using PluginAPI.Core.Attributes;

    using Log = API.Features.Log;
    using Paths = API.Features.Paths;

    /// <summary>
    /// The Northwood PluginAPI Plugin class for the EXILED Loader.
    /// </summary>
    public class LoaderPlugin
    {
#pragma warning disable SA1401
        /// <summary>
        /// The config for the EXILED Loader.
        /// </summary>
        [PluginConfig]
        public static Config Config;
#pragma warning restore SA1401

        /// <summary>
        /// Called by PluginAPI when the plugin is enabled.
        /// </summary>
        [PluginEntryPoint("Exiled Loader", null, "Loads the EXILED Plugin Framework.", "Exiled-Team")]
        [PluginPriority(byte.MinValue)]
        public void Enable()
        {
            if (Config == null)
            {
                Log.Error("Loader's config is null. EXILED won't be loaded. Please check your config.");
                return;
            }

            if (!Config.IsEnabled)
            {
                Log.Info("EXILED is disabled on this server via config.");
                return;
            }

            Log.Info($"Loading EXILED Version: {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}");

            Paths.Reload(Config.ExiledDirectoryPath);

            Log.Info($"Exiled root path set to: {Paths.Exiled}");

            Directory.CreateDirectory(Paths.Exiled);
            Directory.CreateDirectory(Paths.Configs);
            Directory.CreateDirectory(Paths.Plugins);
            Directory.CreateDirectory(Paths.Dependencies);

            Timing.RunCoroutine(new Loader().Run());
        }
    }
}