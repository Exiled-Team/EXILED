// -----------------------------------------------------------------------
// <copyright file="CustomModules.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Interfaces;
    using Exiled.CustomModules.API.Enums;
    using Exiled.CustomModules.API.Features;
    using Exiled.CustomModules.API.Features.CustomAbilities;
    using Exiled.CustomModules.API.Features.CustomEscapes;
    using Exiled.CustomModules.API.Features.CustomGameModes;
    using Exiled.CustomModules.API.Features.CustomItems;
    using Exiled.CustomModules.API.Features.CustomItems.Items;
    using Exiled.CustomModules.API.Features.CustomItems.Pickups;
    using Exiled.CustomModules.API.Features.CustomRoles;
    using Exiled.CustomModules.EventHandlers;

    /// <summary>
    /// Handles all custom role API functions.
    /// </summary>
    public class CustomModules : Plugin<Config>
    {
        private static readonly Dictionary<ModuleType, Action<Assembly>> ModulesLoader = new()
        {
            { ModuleType.CustomItems, asm => CustomItem.EnableAll(asm) },
            { ModuleType.CustomRoles, asm => CustomRole.EnableAll(asm) },
            { ModuleType.CustomAbilities, asm => CustomAbility<GameEntity>.EnableAll(asm) },
            { ModuleType.CustomTeams, asm => CustomTeam.EnableAll(asm) },
            { ModuleType.CustomEscapes, asm => CustomEscape.EnableAll(asm) },
            { ModuleType.CustomGameModes, asm => CustomGameMode.EnableAll(asm) },
        };

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

        /// <summary>
        /// Gets a value indicating whether the specified module is loaded.
        /// </summary>
        /// <param name="module">The module to check.</param>
        /// <returns><see langword="true"/> if the module is loaded; otherwise, <see langword="false"/>.</returns>
        public static bool IsModuleLoaded(ModuleType module) => Instance.Config.Modules.Contains(module);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;

            if (Config.UseAutomaticModulesLoader)
            {
                foreach (IPlugin<IConfig> plugin in Exiled.Loader.Loader.Plugins)
                    Config.Modules.ForEach(module => ModulesLoader[module](plugin.Assembly));
            }

            if (Config.Modules.Contains(ModuleType.CustomRoles) && Config.UseDefaultRoleAssigner)
                StaticActor.Get<RoleAssigner>();

            if (Config.Modules.Contains(ModuleType.CustomTeams) && Config.UseDefaultRespawnManager)
                StaticActor.Get<RespawnManager>();

            if (Config.Modules.Contains(ModuleType.CustomGameModes))
                World.Get();

            if (Config.Modules.Contains(ModuleType.CustomAbilities))
                StaticActor.Get<AbilityTracker>();

            if (Config.Modules.Contains(ModuleType.CustomItems))
            {
                StaticActor.Get<ItemTracker>();
                StaticActor.Get<PickupTracker>();
            }

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            World.Get().Destroy();

            StaticActor.Get<RoleAssigner>()?.Destroy();
            StaticActor.Get<RespawnManager>()?.Destroy();
            StaticActor.Get<AbilityTracker>()?.Destroy();
            StaticActor.Get<ItemTracker>()?.Destroy();
            StaticActor.Get<PickupTracker>()?.Destroy();

            CustomItem.DisableAll();
            CustomRole.DisableAll();
            CustomAbility<GameEntity>.DisableAll();
            CustomTeam.DisableAll();
            CustomEscape.DisableAll();
            CustomGameMode.DisableAll();

            base.OnDisabled();
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            PlayerHandler = new();
            ServerHandler = new();

            Exiled.Events.Handlers.Player.ChangingItem += PlayerHandler.OnChangingItem;
            Exiled.Events.Handlers.Server.RoundStarted += ServerHandler.OnRoundStarted;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangingItem -= PlayerHandler.OnChangingItem;
            Exiled.Events.Handlers.Server.RoundStarted -= ServerHandler.OnRoundStarted;

            PlayerHandler = null;
            ServerHandler = null;
        }
    }
}