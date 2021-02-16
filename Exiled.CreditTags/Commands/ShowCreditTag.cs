// -----------------------------------------------------------------------
// <copyright file="ShowCreditTag.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CreditTags.Commands
{
    using System;

    using CommandSystem;

    using Exiled.API.Features;

    /// <summary>
    /// A client command to show your credit tag.
    /// </summary>
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ShowCreditTag : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "exiledtag";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "crtag" };

        /// <inheritdoc/>
        public string Description { get; } = "Shows your EXILED Credits tag, if available.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(((CommandSender)sender).SenderId);

            bool success = CreditTags.Singleton.ShowCreditTag(player);
            response = success
                ? "Your credit tag has been shown."
                : "You have no credit tag to show.";
            return success;
        }
    }
}
