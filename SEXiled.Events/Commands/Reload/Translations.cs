// -----------------------------------------------------------------------
// <copyright file="Translations.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Commands.Reload
{
    using System;

    using CommandSystem;

    using SEXiled.API.Interfaces;
    using SEXiled.Loader;
    using SEXiled.Permissions.Extensions;

    /// <summary>
    /// The reload configs command.
    /// </summary>
    public class Translations : ICommand
    {
        /// <summary>
        /// Gets static instance of the <see cref="Translations"/> command.
        /// </summary>
        public static Translations Instance { get; } = new Translations();

        /// <inheritdoc/>
        public string Command { get; } = "translations";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string Description { get; } = "Reload plugin translations.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("ee.reloadtranslations"))
            {
                response = "You can't reload translations, you don't have \"ee.reloadtranslations\" permission.";
                return false;
            }

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
