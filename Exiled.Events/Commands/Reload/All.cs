// -----------------------------------------------------------------------
// <copyright file="All.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.Reload
{
    using System;

    using CommandSystem;

    /// <summary>
    /// The reload all command.
    /// </summary>
    public class All : ICommand
    {
        /// <summary>
        /// Gets static instance of the <see cref="All"/> command.
        /// </summary>
        public static All Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "all";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "a" };

        /// <inheritdoc/>
        public string Description { get; } = "Reload all configs and plugins.";

        /// <inheritdoc cref="SanitizeResponse" />
        public bool SanitizeResponse { get; }

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool success = true;

            if (!Configs.Instance.Execute(arguments, sender, out string responsetemp))
                success = false;
            sender.Respond(responsetemp);

            if (!Translations.Instance.Execute(arguments, sender, out responsetemp))
                success = false;
            sender.Respond(responsetemp);

            if (!GamePlay.Instance.Execute(arguments, sender, out responsetemp))
                success = false;
            sender.Respond(responsetemp);

            if (!RemoteAdmin.Instance.Execute(arguments, sender, out responsetemp))
                success = false;
            sender.Respond(responsetemp);

            if (!Plugins.Instance.Execute(arguments, sender, out responsetemp))
                success = false;
            sender.Respond(responsetemp);

            if (!Permissions.Instance.Execute(arguments, sender, out responsetemp))
                success = false;
            sender.Respond(responsetemp);

            response = success ? "Reloaded all configs and plugins successfully!" : "Failed to reload all configs and plugins. Read above.";

            return success;
        }
    }
}