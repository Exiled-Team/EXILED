// -----------------------------------------------------------------------
// <copyright file="UseAbility.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.Commands
{
    using System;

    using CommandSystem;

    using Exiled.API.Features;
    using Exiled.CustomRoles.API;
    using Exiled.CustomRoles.API.Features;

    /// <summary>
    /// Handles the using of custom role abilities.
    /// </summary>
    [CommandHandler(typeof(ClientCommandHandler))]
    public class UseAbility : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "useability";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "special" };

        /// <inheritdoc/>
        public string Description { get; } = "Use your custom roles special ability, if available.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get((CommandSender)sender);
            CustomRole customRole = null;

            foreach (CustomRole role in player.GetCustomRoles())
            {
                if (!role.CanUseAbility(out DateTime usableTime))
                {
                    response =
                        $"You cannot use your ability for {role.Name} for another {Math.Round((usableTime - DateTime.Now).TotalSeconds, 2)} seconds.";
                    player.ShowHint(response);

                    return false;
                }
                else
                {
                    customRole = role;

                    break;
                }
            }

            if (customRole == null)
            {
                response = "You are not a role capable of using any custom abilities.";
                player.ShowHint(response);

                return false;
            }

            customRole.UsedAbility = DateTime.Now;
            response = customRole.UseAbility();

            return true;
        }
    }
}
