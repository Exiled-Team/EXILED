// -----------------------------------------------------------------------
// <copyright file="Bootstrap.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.Bootstrap
{
#pragma warning disable SA1118
    using System;
    using System.IO;
    using System.Reflection;

    using NorthwoodLib;

    /// <summary>
    /// The assembly loader class for Exiled.
    /// </summary>
    public sealed class Bootstrap
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
                ServerConsole.AddLog("[Sexiled.Bootstrap] Sexiled has already been loaded!", ConsoleColor.DarkRed);
                return;
            }

            try
            {
                ServerConsole.AddLog("[Sexiled.Bootstrap] Sexiled is loading...", ConsoleColor.DarkRed);

                string rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SEXILED");
                string dependenciesPath = Path.Combine(rootPath, "Plugins", "dependencies");

                if (Environment.CurrentDirectory.Contains("testing", StringComparison.OrdinalIgnoreCase))
                    rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SEXILED-Testing");

                if (!Directory.Exists(rootPath))
                    Directory.CreateDirectory(rootPath);

                if (File.Exists(Path.Combine(rootPath, "Sexiled.Loader.dll")))
                {
                    if (File.Exists(Path.Combine(dependenciesPath, "Sexiled.API.dll")))
                    {
                        if (File.Exists(Path.Combine(dependenciesPath, "YamlDotNet.dll")))
                        {
                            Assembly.Load(File.ReadAllBytes(Path.Combine(rootPath, "Sexiled.Loader.dll")))
                                .GetType("Sexiled.Loader.Loader")
                                .GetMethod("Run")
                                ?.Invoke(
                                    null,
                                    new object[]
                                    {
                                        new Assembly[]
                                        {
                                            Assembly.Load(File.ReadAllBytes(Path.Combine(dependenciesPath, "Sexiled.API.dll"))),
                                            Assembly.Load(File.ReadAllBytes(Path.Combine(dependenciesPath, "YamlDotNet.dll"))),
                                        },
                                    });

                            IsLoaded = true;
                        }
                        else
                        {
                            ServerConsole.AddLog($"[Sexiled.Bootstrap] YamlDotNet.dll was not found, Sexiled won't be loaded!", ConsoleColor.DarkRed);
                        }
                    }
                    else
                    {
                        ServerConsole.AddLog($"[Sexiled.Bootstrap] Sexiled.API.dll was not found, Sexiled won't be loaded!", ConsoleColor.DarkRed);
                    }
                }
                else
                {
                    ServerConsole.AddLog($"[Sexiled.Bootstrap] Sexiled.Loader.dll was not found, Sexiled won't be loaded!", ConsoleColor.DarkRed);
                }
            }
            catch (Exception exception)
            {
                ServerConsole.AddLog($"[Sexiled.Bootstrap] Sexiled loading error: {exception}", ConsoleColor.DarkRed);
            }
        }
    }
}
