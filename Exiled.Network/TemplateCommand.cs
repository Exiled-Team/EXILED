// -----------------------------------------------------------------------
// <copyright file="TemplateCommand.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Network.API;
    using Exiled.Network.API.Packets;
    using Exiled.Permissions.Extensions;
    using LiteNetLib;

    /// <summary>
    /// Template command.
    /// </summary>
    public class TemplateCommand : ICommand
    {
        /// <summary>
        /// Gets or sets command name.
        /// </summary>
        public string DummyCommand { get; set; } = string.Empty;

        /// <summary>
        /// Gets command name.
        /// </summary>
        public string Command => DummyCommand;

        /// <summary>
        /// Gets command aliases.
        /// </summary>
        public string[] Aliases => new string[0];

        /// <summary>
        /// Gets or sets command description.
        /// </summary>
        public string DummyDescription { get; set; } = string.Empty;

        /// <summary>
        /// Gets command description.
        /// </summary>
        public string Description => DummyDescription;

        /// <summary>
        /// Gets or sets command permission.
        /// </summary>
        public string Permission { get; set; }

        /// <summary>
        /// Gets or sets assigned addon id.
        /// </summary>
        public string AssignedAddonID { get; set; }

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player p = Player.Get((CommandSender)sender);
            if (p == null)
            {
                response = "Player not found";
                return false;
            }

            if (string.IsNullOrEmpty(Permission))
                goto skipPermCheck;

            if (!p.CheckPermission(Permission))
            {
                response = $"Missing required permission \"{Permission}\".";
                return false;
            }

            skipPermCheck:
            NPManager.Singleton.PacketProcessor.Send<ExecuteCommandPacket>(NPManager.Singleton.NetworkListener, new ExecuteCommandPacket() { UserID = p.UserId, AddonID = AssignedAddonID, CommandName = this.Command, Arguments = arguments.Array }, DeliveryMethod.ReliableOrdered);
            response = string.Empty;
            return false;
        }
    }
}
