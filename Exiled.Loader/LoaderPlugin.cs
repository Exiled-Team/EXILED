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

    using PluginAPI.Core.Attributes;

    using Log = API.Features.Log;
    using Paths = API.Features.Paths;

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
                Log.Info("EXILED is disabled on this server via config.");
                return;
            }

            Log.Info($"Loading EXILED Version: {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}");
            if (!Config.ShouldLoadOutdatedExiled &&
                !GameCore.Version.CompatibilityCheck(
                    (byte)AutoUpdateFiles.RequiredSCPSLVersion.Major,
                    (byte)AutoUpdateFiles.RequiredSCPSLVersion.Minor,
                    (byte)AutoUpdateFiles.RequiredSCPSLVersion.Revision,
                    GameCore.Version.Major,
                    GameCore.Version.Minor,
                    GameCore.Version.Revision,
                    GameCore.Version.BackwardCompatibility,
                    GameCore.Version.BackwardRevision))
            {
                ServerConsole.AddLog($"Exiled is outdated, please update to the latest version. Wait for release if still shows after update.\nSCP:SL: {GameCore.Version.VersionString} Exiled Supported Version: {AutoUpdateFiles.RequiredSCPSLVersion}", ConsoleColor.DarkRed);
                return;
            }

            Paths.Reload(Config.ExiledDirectoryPath);

            Log.Info($"Exiled root path set to: {Paths.Exiled}");

            Directory.CreateDirectory(Paths.Exiled);
            Directory.CreateDirectory(Paths.Configs);
            Directory.CreateDirectory(Paths.Plugins);
            Directory.CreateDirectory(Paths.Dependencies);

            if (!File.Exists(Path.Combine(Paths.Dependencies, "Exiled.API.dll")))
            {
                Log.Error($"Exiled.API.dll was not found at {Path.Combine(Paths.Dependencies, "Exiled.API.dll")}, Exiled won't be loaded!");
                return;
            }

            new Loader().Run();
        }
    }
}