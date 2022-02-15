// -----------------------------------------------------------------------
// <copyright file="Bootstrap.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Bootstrap
{
#pragma warning disable SA1118
    using System;
    using System.IO;
    using System.Reflection;

    using NorthwoodLib;

    /// <summary>
    /// The assembly loader class for SEXiled.
    /// </summary>
    public sealed class Bootstrap
    {
        /// <summary>
        /// Gets a value indicating whether sexiled has already been loaded or not.
        /// </summary>
        public static bool IsLoaded { get; private set; }

        /// <summary>
        /// Internally called loading method.
        /// </summary>
        public static void Load()
        {
            if (IsLoaded)
            {
                ServerConsole.AddLog("[SEXiled.Bootstrap] SEXiled has already been loaded!", ConsoleColor.DarkRed);
                return;
            }

            try
            {
                ServerConsole.AddLog("[SEXiled.Bootstrap] SEXiled is loading...", ConsoleColor.DarkRed);

                string rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EXILED");

                if (Environment.CurrentDirectory.Contains("testing", StringComparison.OrdinalIgnoreCase))
                    rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EXILED-Testing");

                string dependenciesPath = Path.Combine(rootPath, "Plugins", "dependencies");

                if (!Directory.Exists(rootPath))
                    Directory.CreateDirectory(rootPath);

                if (!File.Exists(Path.Combine(rootPath, "SEXiled.Loader.dll")))
                {
                    ServerConsole.AddLog($"[SEXiled.Bootstrap] SEXiled.Loader.dll was not found, SEXiled won't be loaded!", ConsoleColor.DarkRed);
                    return;
                }

                if (!File.Exists(Path.Combine(dependenciesPath, "SEXiled.API.dll")))
                {
                    ServerConsole.AddLog($"[SEXiled.Bootstrap] SEXiled.API.dll was not found, SEXiled won't be loaded!", ConsoleColor.DarkRed);
                    return;
                }

                if (!File.Exists(Path.Combine(dependenciesPath, "YamlDotNet.dll")))
                {
                    ServerConsole.AddLog($"[SEXiled.Bootstrap] YamlDotNet.dll was not found, SEXiled won't be loaded!", ConsoleColor.DarkRed);
                    return;
                }

                Assembly.Load(File.ReadAllBytes(Path.Combine(rootPath, "SEXiled.Loader.dll")))
                    .GetType("SEXiled.Loader.Loader")
                    .GetMethod("Run")
                    ?.Invoke(
                        null,
                        new object[]
                        {
                            new[]
                            {
                                Assembly.Load(File.ReadAllBytes(Path.Combine(dependenciesPath, "SEXiled.API.dll"))),
                                Assembly.Load(File.ReadAllBytes(Path.Combine(dependenciesPath, "YamlDotNet.dll"))),
                            },
                        });

                IsLoaded = true;
            }
            catch (Exception exception)
            {
                ServerConsole.AddLog($"[SEXiled.Bootstrap] SEXiled loading error: {exception}", ConsoleColor.DarkRed);
            }
        }
    }
}
