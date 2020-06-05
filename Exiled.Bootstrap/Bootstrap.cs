// -----------------------------------------------------------------------
// <copyright file="Bootstrap.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Loader
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// The assembly loader class for Exiled.
    /// </summary>
    public class Bootstrap
    {
        /// <summary>
        /// Gets or sets a value indicating whether Exiled has already loaded or not.
        /// </summary>
        public static bool IsLoaded { get; set; }

        /// <summary>
        /// Internally called loading method.
        /// </summary>
        public static void Load()
        {
            if (IsLoaded)
                return;

            ServerConsole.AddLog("Exiled is loading...", ConsoleColor.DarkRed);

            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EXILED-PTB");

                if (Environment.CurrentDirectory.ToLower().Contains("testing"))
                    path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EXILED-Testing");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                if (File.Exists(Path.Combine(path, "Exiled.API.dll")))
                    Assembly.LoadFile(Path.Combine(path, "Exiled.API.dll"));

                if (!File.Exists(Path.Combine(path, "Exiled.Loader.dll")))
                    return;

                MethodInfo methodInfo = Assembly.LoadFrom(Path.Combine(path, "Exiled.Loader.dll")).GetTypes().SelectMany(type => type.GetMethods()).FirstOrDefault(method => method.Name == "EntryPointForLoader");

                methodInfo?.Invoke(null, null);

                IsLoaded = true;
            }
            catch (Exception exception)
            {
                ServerConsole.AddLog($"Exiled load error: {exception}");
            }
        }
    }
}