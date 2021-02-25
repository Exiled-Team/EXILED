// -----------------------------------------------------------------------
// <copyright file="Give.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.Commands
{
    using System;
    using System.Linq;

    using CommandSystem;

    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// The command to give a player an item.
    /// </summary>
    internal sealed class Give : ICommand
    {
        private Give()
        {
        }

        /// <summary>
        /// Gets the <see cref="Give"/> instance.
        /// </summary>
        public static Give Instance { get; } = new Give();

        /// <inheritdoc/>
        public string Command { get; } = "give";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "g" };

        /// <inheritdoc/>
        public string Description { get; } = "Gives a custom item.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("customitems.give"))
            {
                response = "Permission Denied, required: customitems.give";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "give [Custom item name] [Nickname/PlayerID/UserID]";
                return false;
            }

            string identifier = string.Join(" ", arguments.Skip(1));

            if (!(Player.Get(identifier) is Player player))
            {
                response = $"Unable to find player: {identifier}.";
                return false;
            }

            if (int.TryParse(arguments.At(0), out int id) && CustomItem.TryGive(player, id))
            {
                response = $"Custom item given to {player.Nickname} ({player.UserId})";
                return true;
            }
            else if (CustomItem.TryGive(player, arguments.At(0)))
            {
                response = $"Custom item given to {player.Nickname} ({player.UserId})";
                return true;
            }

            response = $"custom item {arguments.At(0)} not found.";
            return false;
        }
    }
}
