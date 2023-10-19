// -----------------------------------------------------------------------
// <copyright file="LoaderPlugin.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader
{
    using System;
    using System.IO;
    using System.Reflection;

    using MEC;

    using PluginAPI.Core.Attributes;

    /// <summary>
    /// The PluginAPI Plugin class for the EXILED Loader.
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
        public void Enable()
        {
            if (!Config.IsEnabled)
            {
                ServerConsole.AddLog("EXILED is disabled on this server via config.", ConsoleColor.Cyan);
                return;
            }

            ServerConsole.AddLog($"Loading EXILED Version: {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}", ConsoleColor.Cyan);

            string dependenciesPath = Path.Combine(Config.ExiledDirectoryPath, "Plugins", "dependencies");

            ServerConsole.AddLog($"Exiled root path set to: {Config.ExiledDirectoryPath}", ConsoleColor.Cyan);

            Timing.RunCoroutine(new Loader().Run(new Assembly[]
                {
                    Assembly.LoadFrom(Path.Combine(dependenciesPath, "Exiled.API.dll")),
                    Assembly.LoadFrom(Path.Combine(dependenciesPath, "SemanticVersioning.dll")),
                }));
        }
    }
}