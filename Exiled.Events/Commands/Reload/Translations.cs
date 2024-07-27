// -----------------------------------------------------------------------
// <copyright file="Translations.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.Reload
{
    using System;

    using API.Interfaces;

    using CommandSystem;

    using Loader;

    /// <summary>
    /// The reload translations command.
    /// </summary>
    public class Translations : ICommand, IPermissioned
    {
        /// <summary>
        /// Gets static instance of the <see cref="Translations"/> command.
        /// </summary>
        public static Translations Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "translations";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string Description { get; } = "Reload plugin translations.";

        /// <inheritdoc />
        public string Permission { get; } = "ee.reloadtranslations";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool haveBeenReloaded = TranslationManager.Reload();

            Handlers.Server.OnReloadedTranslations();

            foreach (IPlugin<IConfig> plugin in Loader.Plugins)
            {
                plugin.OnUnregisteringCommands();
                plugin.OnRegisteringCommands();
            }

            response = "Plugin translations have been reloaded successfully!";
            return haveBeenReloaded;
        }
    }
}