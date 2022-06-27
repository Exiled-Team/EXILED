// -----------------------------------------------------------------------
// <copyright file="Bootstrap.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Bootstrap
{
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
                ServerConsole.AddLog("[Exiled.Bootstrap] Exiled has already been loaded!", ConsoleColor.DarkRed);
                return;
            }

            try
            {
                ServerConsole.AddLog("[Exiled.Bootstrap] Exiled is loading...", ConsoleColor.DarkRed);

                string rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EXILED");

                if (Environment.CurrentDirectory.Contains("testing", StringComparison.OrdinalIgnoreCase))
                    rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EXILED-Testing");

                string dependenciesPath = Path.Combine(rootPath, "Plugins", "dependencies");

                if (!Directory.Exists(rootPath))
                    Directory.CreateDirectory(rootPath);

                if (!File.Exists(Path.Combine(rootPath, "Exiled.Loader.dll")))
                {
                    ServerConsole.AddLog($"[Exiled.Bootstrap] Exiled.Loader.dll was not found, Exiled won't be loaded!", ConsoleColor.DarkRed);
                    return;
                }

                if (!File.Exists(Path.Combine(dependenciesPath, "Exiled.API.dll")))
                {
                    ServerConsole.AddLog($"[Exiled.Bootstrap] Exiled.API.dll was not found, Exiled won't be loaded!", ConsoleColor.DarkRed);
                    return;
                }

                if (!File.Exists(Path.Combine(dependenciesPath, "YamlDotNet.dll")))
                {
                    ServerConsole.AddLog($"[Exiled.Bootstrap] YamlDotNet.dll was not found, Exiled won't be loaded!", ConsoleColor.DarkRed);
                    return;
                }

                Assembly.Load(File.ReadAllBytes(Path.Combine(rootPath, "Exiled.Loader.dll")))
                    .GetType("Exiled.Loader.Loader")
                    .GetMethod("Run")
                    ?.Invoke(
                        null,
                        new object[]
                        {
                            new[]
                            {
                                Assembly.Load(File.ReadAllBytes(Path.Combine(dependenciesPath, "Exiled.API.dll"))),
                                Assembly.Load(File.ReadAllBytes(Path.Combine(dependenciesPath, "YamlDotNet.dll"))),
                            },
                        });

                IsLoaded = true;
            }
            catch (Exception exception)
            {
                ServerConsole.AddLog($"[Exiled.Bootstrap] Exiled loading error: {exception}", ConsoleColor.DarkRed);
            }
        }
    }
}
