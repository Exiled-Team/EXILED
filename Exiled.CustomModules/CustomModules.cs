// -----------------------------------------------------------------------
// <copyright file="CustomModules.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.DynamicEvents;
    using Exiled.CustomModules.API.Enums;
    using Exiled.CustomModules.API.Features;
    using Exiled.CustomModules.EventHandlers;
    using MEC;

    /// <summary>
    /// Handles all custom role API functions.
    /// </summary>
    public class CustomModules : Plugin<Config>
    {
        /// <summary>
        /// The delay to be applied in order to execute any dispatch operation.
        /// </summary>
#pragma warning disable SA1310 // Field names should not contain underscore
        private const float DISPATCH_OPERATION_DELAY = 0.5f;
#pragma warning restore SA1310 // Field names should not contain underscore

        /// <summary>
        /// Gets a static reference to the plugin's instance.
        /// </summary>
        public static CustomModules Instance { get; private set; }

        /// <summary>
        /// Gets a value indicating whether <see cref="CustomModules"/> assembly is loaded.
        /// </summary>
        public static bool IsLoaded => Instance is not null;

        /// <inheritdoc/>
        public override PluginPriority Priority => PluginPriority.Last;

        /// <summary>
        /// Gets the <see cref="EventHandlers.PlayerHandler"/>.
        /// </summary>
        internal PlayerHandler PlayerHandler { get; private set; }

        /// <summary>
        /// Gets the <see cref="EventHandlers.ServerHandler"/>.
        /// </summary>
        internal ServerHandler ServerHandler { get; private set; }

        /// <summary>
        /// Gets the <see cref="EventHandlers.RegistrationHandler"/>.
        /// </summary>
        internal RegistrationHandler RegistrationHandler { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the specified module is loaded.
        /// </summary>
        /// <param name="module">The module to check.</param>
        /// <returns><see langword="true"/> if the module is loaded; otherwise, <see langword="false"/>.</returns>
        public static bool IsModuleLoaded(UUModuleType module) => Instance.Config.Modules.Contains(module.Name);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;

            base.OnEnabled();

            Timing.CallDelayed(DISPATCH_OPERATION_DELAY, CustomModule.LoadAll);
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            base.OnDisabled();

            CustomModule.UnloadAll();

            Instance = null;
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            PlayerHandler = new();
            ServerHandler = new();
            RegistrationHandler = new(Config);

            Exiled.Events.Handlers.Player.ChangingItem += PlayerHandler.OnChangingItem;
            Exiled.Events.Handlers.Server.RoundStarted += ServerHandler.OnRoundStarted;
            CustomModule.OnEnabled += RegistrationHandler.OnModuleEnabled;
            CustomModule.OnDisabled += RegistrationHandler.OnModuleDisabled;

            DynamicEventManager.CreateFromTypeInstance(RegistrationHandler);
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangingItem -= PlayerHandler.OnChangingItem;
            Exiled.Events.Handlers.Server.RoundStarted -= ServerHandler.OnRoundStarted;
            CustomModule.OnEnabled -= RegistrationHandler.OnModuleEnabled;
            CustomModule.OnDisabled -= RegistrationHandler.OnModuleDisabled;

            DynamicEventManager.DestroyFromTypeInstance(RegistrationHandler);

            PlayerHandler = null;
            ServerHandler = null;
            RegistrationHandler = null;
        }
    }
}