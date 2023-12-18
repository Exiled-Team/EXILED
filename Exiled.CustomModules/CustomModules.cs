// -----------------------------------------------------------------------
// <copyright file="CustomModules.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules
{
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.CustomModules.API.Features;
    using Exiled.CustomModules.Events;

    /// <summary>
    /// Handles all custom role API functions.
    /// </summary>
    public class CustomModules : Plugin<Config>
    {
        private PlayerHandlers playerHandlers;
        private KeypressActivator keypressActivator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomModules"/> class.
        /// </summary>
        public CustomModules()
        {
            //Loader.Deserializer = new DeserializerBuilder()
            //    .WithTypeConverter(new VectorsConverter())
            //    .WithTypeConverter(new ColorConverter())
            //    .WithTypeConverter(new AttachmentIdentifiersConverter())
            //    .WithNamingConvention(UnderscoredNamingConvention.Instance)
            //    .WithNodeDeserializer(inner => new AbstractClassNodeTypeResolver(inner, new AggregateExpectationTypeResolver<CustomAbility>(UnderscoredNamingConvention.Instance)), s => s.InsteadOf<ObjectNodeDeserializer>())
            //    .IgnoreFields()
            //    .IgnoreUnmatchedProperties()
            //    .Build();
        }

        /// <summary>
        /// Gets a static reference to the plugin's instance.
        /// </summary>
        public static CustomModules Instance { get; private set; } = null!;

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