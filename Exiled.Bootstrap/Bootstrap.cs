// -----------------------------------------------------------------------
// <copyright file="Bootstrap.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Boostrap
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// The assembly loader class for Exiled.
    /// </summary>
    public class Bootstrap
    {
        /// <summary>
        /// Gets a value indicating whether exiled has already been loaded or not.
        /// </summary>
        public static bool IsLoaded { get; private set; }

        /// <summary>
        /// Internally called loading method.
        /// </summary>
        public static void Load()
        {
            if (IsLoaded)
            {
                ServerConsole.AddLog("[Exiled.Bootstrap] Exiled has already been loaded!", ConsoleColor.DarkRed);
                return;
            }

            try
            {
                ServerConsole.AddLog("[Exiled.Bootstrap] Exiled is loading...", ConsoleColor.DarkRed);

                string rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EXILED");
                string dependenciesPath = Path.Combine(rootPath, "Plugins", "dependencies");

                if (Environment.CurrentDirectory.ToLower().Contains("testing"))
                    rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EXILED-Testing");

                if (!Directory.Exists(rootPath))
                    Directory.CreateDirectory(rootPath);

                if (File.Exists(Path.Combine(rootPath, "Exiled.Loader.dll")))
                {
                    if (File.Exists(Path.Combine(dependenciesPath, "Exiled.API.dll")))
                    {
                        if (File.Exists(Path.Combine(dependenciesPath, "YamlDotNet.dll")))
                        {
                            Assembly.LoadFrom(Path.Combine(rootPath, "Exiled.Loader.dll"))
                                .GetType("Exiled.Loader.Loader")
                                .GetMethod("Run")
                                ?.Invoke(null, new object[] { new Assembly[] { Assembly.LoadFrom(Path.Combine(dependenciesPath, "Exiled.API.dll")), Assembly.LoadFrom(Path.Combine(dependenciesPath, "YamlDotNet.dll")) } });

                            IsLoaded = true;
                        }
                        else
                        {
                            ServerConsole.AddLog($"[Exiled.Bootstrap] YamlDotNet.dll was not found, Exiled won't be loaded!", ConsoleColor.DarkRed);
                        }
                    }
                    else
                    {
                        ServerConsole.AddLog($"[Exiled.Bootstrap] Exiled.API.dll was not found, Exiled won't be loaded!", ConsoleColor.DarkRed);
                    }
                }
                else
                {
                    ServerConsole.AddLog($"[Exiled.Bootstrap] Exiled.Loader.dll was not found, Exiled won't be loaded!", ConsoleColor.DarkRed);
                }
            }
            catch (Exception exception)
            {
                ServerConsole.AddLog($"[Exiled.Bootstrap] Exiled loading error: {exception}", ConsoleColor.DarkRed);
            }
        }
    }
}
