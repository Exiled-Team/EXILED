// -----------------------------------------------------------------------
// <copyright file="Paths.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.IO;

    /// <summary>
    /// A set of useful paths.
    /// </summary>
    public static class Paths
    {
        /// <summary>
        /// Gets AppData path.
        /// </summary>
        public static string AppData { get; private set; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        /// <summary>
        /// Gets or sets exiled directory path.
        /// </summary>
        public static string Exiled { get; set; } = Path.Combine(AppData, "EXILED-PTB");

        /// <summary>
        /// Gets or sets plugins path.
        /// </summary>
        public static string Plugins { get; set; } = Path.Combine(Exiled, "Plugins");

        /// <summary>
        /// Gets or sets Dependencies directory path.
        /// </summary>
        public static string Dependencies { get; set; } = Path.Combine(Plugins, "dependencies");

        /// <summary>
        /// Gets managed assemblies directory path.
        /// </summary>
        public static string ManagedAssemblies { get; private set; } = Path.Combine(Path.Combine(Environment.CurrentDirectory, "SCPSL_Data"), "Managed");

        /// <summary>
        /// Gets or sets configs path.
        /// </summary>
        public static string Config { get; set; } = Path.Combine(Path.Combine(Exiled, "Configs"), $"{Server.Port}-config.yml");

        /// <summary>
        /// Gets or sets logs path.
        /// </summary>
        public static string Log { get; set; } = Path.Combine(Exiled, $"{Server.Port}-RA_log.txt");
    }
}
