// -----------------------------------------------------------------------
// <copyright file="MainLoader.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader
{
    using System;
    using System.IO;
    using Exiled.API.Features;
    using MEC;

    /// <summary>
    /// Entry point class.
    /// </summary>
    public class MainLoader
    {
        /// <summary>
        /// Called by the assembly's Loader class when the server starts.
        /// </summary>
        public static void EntryPointForLoader()
        {
            Log.Info($"Initializing Exiled at {Environment.CurrentDirectory}");

            if (Environment.CurrentDirectory.ToLower().Contains("testing"))
            {
                Paths.Exiled = Path.Combine(Paths.AppData, "EXILED-Testing");
                Paths.Plugins = Path.Combine(Paths.Exiled, "Plugins");
                Paths.Config = Path.Combine(Path.Combine(Paths.Exiled, "Configs"), $"{Server.Port}-config.yml");
                Paths.Log = Path.Combine(Paths.Exiled, $"{Server.Port}-RA_log.txt");
                Paths.Dependencies = Path.Combine(Paths.Plugins, "dependencies");
            }

            if (!Directory.Exists(Path.Combine(Paths.Exiled, "Configs")))
                Directory.CreateDirectory(Path.Combine(Paths.Exiled, "Configs"));

            if (!File.Exists(Paths.Config))
                File.Create(Paths.Config).Close();

            CustomNetworkManager.Modded = true;

            ServerConsole.AddLog($"Exiled - Version {PluginManager.Version.Major}.{PluginManager.Version.Minor}.{PluginManager.Version.Build}", ConsoleColor.DarkRed);

            PluginManager.Config.Reload();
            Timing.CallDelayed(0.25f, PluginManager.LoadAll);
        }
    }
}