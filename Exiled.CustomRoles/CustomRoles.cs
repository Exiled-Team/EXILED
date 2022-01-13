// -----------------------------------------------------------------------
// <copyright file="CustomRoles.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Exiled.API.Features;
    using Exiled.CustomRoles.API.Features;
    using Exiled.CustomRoles.Deserialization;
    using Exiled.CustomRoles.Events;
    using Exiled.Loader;
    using Exiled.Loader.Features.Configs;
    using Exiled.Loader.Features.Configs.CustomConverters;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;
    using YamlDotNet.Serialization.NodeDeserializers;

    /// <summary>
    /// Handles all custom role API functions.
    /// </summary>
    public class CustomRoles : Plugin<Config>
    {
        private PlayerHandlers playerHandlers;

        /// <summary>
        /// Gets a static reference to the plugin's instance.
        /// </summary>
        public static CustomRoles Instance { get; private set; }

        /// <summary>
        /// Gets a list of players to stop spawning ragdolls for.
        /// </summary>
        internal List<Player> StopRagdollPlayers { get; } = new List<Player>();

        private static CustomAbilityTypeResolver TypeResolver { get; } =
            new CustomAbilityTypeResolver(UnderscoredNamingConvention.Instance);

        private static IDeserializer Deserializer { get; } = new DeserializerBuilder()
            .WithTypeConverter(new VectorsConverter())
            .WithNodeDeserializer(inner => new ValidatingNodeDeserializer(inner), deserializer => deserializer.InsteadOf<ObjectNodeDeserializer>())
            .WithNodeDeserializer(
                inner => new AbstractNodeNodeTypeResolver(
                    inner,
                    TypeResolver),
                s => s.InsteadOf<ValidatingNodeDeserializer>())
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreFields()
            .IgnoreUnmatchedProperties()
            .Build();

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;
            playerHandlers = new PlayerHandlers(this);

            Exiled.Events.Handlers.Player.SpawningRagdoll += playerHandlers.OnSpawningRagdoll;
            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.SpawningRagdoll -= playerHandlers.OnSpawningRagdoll;
            playerHandlers = null;
            Instance = null;
            base.OnDisabled();
        }

        private static void InjectDeserializer()
        {
            foreach (Type t in Loader.Locations.Keys.SelectMany(asm => asm
                .GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(CustomAbility)))))
            {
                TypeResolver.TypeLookup.Add(t.Name, t);
            }

            typeof(Loader).GetProperty("Deserializer", BindingFlags.Static)?.SetValue(null, Deserializer);
        }
    }
}
