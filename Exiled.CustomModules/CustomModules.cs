// -----------------------------------------------------------------------
// <copyright file="CustomModules.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules
{
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Generic;
    using Exiled.API.Interfaces;
    using Exiled.CustomModules.API.Features;
    using Exiled.CustomModules.API.Features.CustomAbilities;
    using Exiled.CustomModules.API.Features.CustomEscapes;
    using Exiled.CustomModules.API.Features.CustomGameModes;
    using Exiled.CustomModules.API.Features.CustomItems;
    using Exiled.CustomModules.API.Features.CustomRoles;
    using Exiled.CustomModules.EventHandlers;

    /// <summary>
    /// Handles all custom role API functions.
    /// </summary>
    public class CustomModules : Plugin<Config>
    {
        /// <summary>
        /// Gets a static reference to the plugin's instance.
        /// </summary>
        public static CustomModules Instance { get; private set; }

        /// <summary>
        /// Gets the <see cref="EventHandlers.PlayerHandler"/>.
        /// </summary>
        internal PlayerHandler PlayerHandler { get; private set; }

        /// <summary>
        /// Gets the <see cref="EventHandlers.ServerHandler"/>.
        /// </summary>
        internal ServerHandler ServerHandler { get; private set; }

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;

            if (Config.UseAutomaticModulesLoader)
            {
                foreach (IPlugin<IConfig> plugin in Loader.Loader.Plugins)
                {
                    CustomItem.EnableAll(plugin.Assembly);
                    CustomRole.EnableAll(plugin.Assembly);
                    CustomAbility<GameEntity>.EnableAll(plugin.Assembly);
                    CustomTeam.EnableAll(plugin.Assembly);
                    CustomEscape.EnableAll(plugin.Assembly);
                    CustomGameMode.EnableAll(plugin.Assembly);
                }
            }

            if (Config.UseDefaultRoleAssigner)
                StaticActor.CreateNewInstance<RoleAssigner>();

            if (Config.UseDefaultRespawnManager)
                StaticActor.CreateNewInstance<RespawnManager>();

            SubscribeEvents();

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            StaticActor.Get<RoleAssigner>()?.Destroy();
            StaticActor.Get<RespawnManager>()?.Destroy();

            CustomItem.DisableAll();
            CustomRole.DisableAll();
            CustomAbility<GameEntity>.DisableAll();
            CustomTeam.DisableAll();
            CustomEscape.DisableAll();
            CustomGameMode.DisableAll();

            UnsubscribeEvents();

            base.OnDisabled();
        }

        private void SubscribeEvents()
        {
            PlayerHandler = new();
            ServerHandler = new();

            Exiled.Events.Handlers.Player.ChangingItem += PlayerHandler.OnChangingItem;
            Exiled.Events.Handlers.Server.RoundStarted += ServerHandler.OnRoundStarted;
        }

        private void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangingItem -= PlayerHandler.OnChangingItem;
            Exiled.Events.Handlers.Server.RoundStarted -= ServerHandler.OnRoundStarted;

            PlayerHandler = null;
            ServerHandler = null;
        }
    }
}