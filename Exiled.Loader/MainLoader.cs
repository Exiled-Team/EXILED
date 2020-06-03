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

            if (!File.Exists(Paths.Config))
                File.Create(Paths.Config).Close();

            CustomNetworkManager.Modded = true;

            ServerConsole.AddLog($"Exiled - Version {PluginManager.Version.Major}.{PluginManager.Version.Minor}.{PluginManager.Version.Build}", ConsoleColor.DarkRed);

            PluginManager.Config.Reload();
            PluginManager.LoadAll(0.5f);
        }
    }
}