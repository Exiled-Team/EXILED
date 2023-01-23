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
    using PluginAPI.Core.Attributes;
    using PluginAPI.Enums;

    /// <summary>
    /// The assembly loader class for Exiled.
    /// </summary>
    public sealed class Bootstrap
    {
#pragma warning disable SA1401
        /// <summary>
        /// The config for the EXILED Loader.
        /// </summary>
        [PluginConfig("Exiled.Bootstrap.yml")]
        public static Config Config;

#pragma warning restore SA1401

        /// <summary>
        /// Internally called loading method.
        /// </summary>
        [PluginPriority((LoadPriority)0)]
        [PluginEntryPoint("Exiled Loader", null, "Loads the EXILED Plugin Framework.", "Exiled-Team")]
        public void Enable()
        {
            try
            {
                ServerConsole.AddLog("[Exiled.Bootstrap] Exiled is loading...", ConsoleColor.DarkRed);

                string rootPath = Config.ExiledDirectoryPath;

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
            }
            catch (Exception exception)
            {
                ServerConsole.AddLog($"[Exiled.Bootstrap] Exiled loading error: {exception}", ConsoleColor.DarkRed);
            }
        }
    }
}
