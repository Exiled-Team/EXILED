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

    using Exiled.Loader;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// The reload configs command.
    /// </summary>
    public class All : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "all";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new string[] { "a" };

        /// <inheritdoc/>
        public string Description { get; } = "Reload all configs and plugins.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool success = true;

            if (!new Configs().Execute(arguments, sender, out string responsetemp))
                success = false;
            sender.Respond(responsetemp);

            if (!new GamePlay().Execute(arguments, sender, out responsetemp))
                success = false;
            sender.Respond(responsetemp);

            if (!new RemoteAdmin().Execute(arguments, sender, out responsetemp))
                success = false;
            sender.Respond(responsetemp);

            if (!new Plugins().Execute(arguments, sender, out responsetemp))
                success = false;
            sender.Respond(responsetemp);

            if (success)
                response = "Reloaded all configs and plugins successfully!";
            else
                response = "Failed to reload all configs and plugins. Read above.";

            return success;
        }
    }
}
