// -----------------------------------------------------------------------
// <copyright file="UseAbility.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.Commands
{
    using System;
    using System.Collections.ObjectModel;

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
            int abilityNumber = 0;
            if (arguments.Count > 0)
                int.TryParse(arguments.At(0), out abilityNumber);

            ReadOnlyCollection<CustomRole> roles = player.GetCustomRoles();
            if (roles.Count == 0)
            {
                response = "You do not have any custom roles.";

                return false;
            }

            if (arguments.Count > 1)
            {
                CustomRole? role = CustomRole.Get(arguments.At(1));
                if (role is null)
                {
                    response = $"The specified role {arguments.At(1)} does not exist.";

                    return false;
                }

                if (role.CustomAbilities is null)
                {
                    response = "You do not have any custom abilities for this role.";

                    return false;
                }

                if (role.CustomAbilities.Count >= abilityNumber + 1)
                {
                    if (role.CustomAbilities[abilityNumber] is ActiveAbility active)
                    {
                        if (!active.CanUseAbility(player, out response))
                        {
                            return false;
                        }

                        active.UseAbility(player);
                        response = $"Ability {active.Name} used.";
                        return true;
                    }
                }
            }

            response = "Could not find an ability that was able to be used.";

            foreach (CustomRole customRole in roles)
            {
                if (customRole.CustomAbilities is null || customRole.CustomAbilities.Count < abilityNumber + 1 || !(customRole.CustomAbilities[abilityNumber] is ActiveAbility activeAbility) || !activeAbility.CanUseAbility(player, out response))
                    continue;

                activeAbility.UseAbility(player);
                response = $"Ability {activeAbility.Name} used.";
                return true;
            }

            return false;
        }
    }
}