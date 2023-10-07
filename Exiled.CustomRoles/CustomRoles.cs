// -----------------------------------------------------------------------
// <copyright file="CustomRoles.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles
{
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.CustomRoles.API.Features;
    using Exiled.CustomRoles.API.Features.Parsers;
    using Exiled.CustomRoles.Events;
    using Exiled.Loader;
    using Exiled.Loader.Features.Configs.CustomConverters;

    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;
    using YamlDotNet.Serialization.NodeDeserializers;

    /// <summary>
    /// Handles all custom role API functions.
    /// </summary>
    public class CustomRoles : Plugin<Config>
    {
        private PlayerHandlers? playerHandlers;
        private KeypressActivator? keypressActivator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomRoles"/> class.
        /// </summary>
        public CustomRoles()
        {
            Loader.Deserializer = new DeserializerBuilder()
                .WithTypeConverter(new VectorsConverter())
                .WithTypeConverter(new ColorConverter())
                .WithTypeConverter(new AttachmentIdentifiersConverter())
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .WithNodeDeserializer(inner => new AbstractClassNodeTypeResolver(inner, new AggregateExpectationTypeResolver<CustomAbility>(UnderscoredNamingConvention.Instance)), s => s.InsteadOf<ObjectNodeDeserializer>())
                .IgnoreFields()
                .IgnoreUnmatchedProperties()
                .Build();
        }

        /// <summary>
        /// Gets a static reference to the plugin's instance.
        /// </summary>
        public static CustomRoles Instance { get; private set; } = null!;

        /// <summary>
        /// Gets or sets the <see cref="KeypressActivator"/> used for abilities.
        /// </summary>
        public static KeypressActivator? KeypressActivator
        {
            get => Instance.keypressActivator;
            set
            {
                Instance.keypressActivator?.Dispose();
                Instance.keypressActivator = value;
            }
        }

        /// <summary>
        /// Gets a list of players to stop spawning ragdolls for.
        /// </summary>
        internal List<Player> StopRagdollPlayers { get; } = new();

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;
            playerHandlers = new PlayerHandlers(this);

            if (Config.UseKeypressActivation)
                keypressActivator = new();
            Exiled.Events.Handlers.Player.SpawningRagdoll += playerHandlers.OnSpawningRagdoll;
            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.SpawningRagdoll -= playerHandlers!.OnSpawningRagdoll;
            keypressActivator = null;
            base.OnDisabled();
        }
    }
}