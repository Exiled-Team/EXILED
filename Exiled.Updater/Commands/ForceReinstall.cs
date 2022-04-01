// -----------------------------------------------------------------------
// <copyright file="ForceReinstall.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Updater.Commands {
    using System;

    using CommandSystem;

    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public sealed class ForceReinstall : ICommand {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response) {
            bool result = Updater.Instance.CheckUpdate(true);
            response = result ? "The update proccess has started" : "The update process has already started";
            return result;
        }

        public string Command { get; } = nameof(ForceReinstall);

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Reinstalls Exiled forcibly";
    }
}
