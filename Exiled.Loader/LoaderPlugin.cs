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
    using System.Linq;
    using System.Reflection;

    using NorthwoodLib;
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
                ServerConsole.AddLog("EXILED is disabled on this server via config.", ConsoleColor.DarkRed);
                return;
            }

            if (!Config.ShouldLoadOutdatedExiled &&
                GameCore.Version.CompatibilityCheck(
                    (byte)AutoUpdateFiles.RequiredSCPSLVersion.Major,
                    (byte)AutoUpdateFiles.RequiredSCPSLVersion.Minor,
                    (byte)AutoUpdateFiles.RequiredSCPSLVersion.Revision,
                    GameCore.Version.Major,
                    GameCore.Version.Minor,
                    GameCore.Version.Revision,
                    GameCore.Version.BackwardCompatibility,
                    GameCore.Version.BackwardRevision))
            {
                ServerConsole.AddLog("Exiled is outdated, please update to the latest version. Wait for release if still shows after update.", ConsoleColor.DarkRed);
                return;
            }

            try
            {
                ServerConsole.AddLog("[Exiled.Loader] Exiled is loading...", ConsoleColor.DarkRed);

                string rootPath = Path.Combine(Config.ExiledDirectoryPath, "EXILED");

                if (Environment.CurrentDirectory.Contains("testing", StringComparison.OrdinalIgnoreCase))
                    rootPath = Path.Combine(Config.ExiledDirectoryPath, "EXILED-Testing");

                ServerConsole.AddLog($"[Exiled.Loader] Exiled root path set to: {rootPath}", ConsoleColor.Cyan);

                string dependenciesPath = Path.Combine(rootPath, "Plugins", "dependencies");

                if (!Directory.Exists(rootPath))
                    Directory.CreateDirectory(rootPath);

                if (!File.Exists(Path.Combine(dependenciesPath, "Exiled.API.dll")))
                {
                    ServerConsole.AddLog($"[Exiled.Loader] Exiled.API.dll was not found, Exiled won't be loaded!", ConsoleColor.DarkRed);
                    return;
                }

                new Loader().Run(new[]
                            {
                                Assembly.Load(File.ReadAllBytes(Path.Combine(dependenciesPath, "Exiled.API.dll"))),
                            });
            }
            catch (Exception exception)
            {
                ServerConsole.AddLog($"[Exiled.Loader] Exiled loading error: {exception}", ConsoleColor.DarkRed);
            }
        }
    }
}