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
    /// A client command to show an EXILED credit tag.
    /// </summary>
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ShowCreditTag : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "exiledtag";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "crtag", "et", "ct" };

        /// <inheritdoc/>
        public string Description { get; } = "Shows your EXILED Credits tag, if available.";

        /// <inheritdoc />
        public bool SanitizeResponse { get; }

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            CommandSender cmdSender = (CommandSender)sender;

            if (!Player.TryGet(sender, out Player player))
            {
                response = "You cannot use this command while still authenticating.";
                return false;
            }

            void ErrorHandler() => cmdSender.RaReply("An error has occurred.", false, true, string.Empty);
            void SuccessHandler() => cmdSender.RaReply("Enjoy your credit tag!", true, true, string.Empty);

            bool cached = CreditTags.Instance.ShowCreditTag(player, ErrorHandler, SuccessHandler, true);

            response = cached ? "Your credit tag has been shown." : "Hold on...";
            return true;
        }
    }
}