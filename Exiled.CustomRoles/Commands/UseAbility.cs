// -----------------------------------------------------------------------
// <copyright file="UseAbility.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.Commands
{
    using System;
    using System.Linq;

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
            string abilityName = string.Empty;
            ActiveAbility? ability;

            if (arguments.Count > 0)
            {
                foreach (string s in arguments.Skip(1))
                    abilityName += s;

                if (!CustomAbility.TryGet(abilityName, out CustomAbility? customAbility) || customAbility is null)
                {
                    response = $"Ability {abilityName} does not exist.";
                    return false;
                }

                if (customAbility is not ActiveAbility activeAbility)
                {
                    response = $"{abilityName} is not an active ability.";
                    return false;
                }

                ability = activeAbility;
            }
            else
            {
                ability = player.GetSelectedAbility();
            }

            if (ability is null)
            {
                response = "No selected ability.";
                return false;
            }

            if (!ability.CanUseAbility(player, out response, CustomRoles.Instance.Config.ActivateOnlySelected))
                return false;
            response = $"{ability.Name} has been used.";
            ability.UseAbility(player);
            return true;
        }
    }
}