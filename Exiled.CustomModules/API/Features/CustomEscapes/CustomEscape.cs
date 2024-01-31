// -----------------------------------------------------------------------
// <copyright file="CustomEscape.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomEscapes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.CustomModules.API.Enums;
    using Exiled.CustomModules.API.Features.Attributes;
    using MonoMod.Utils;
    using Utils.NonAllocLINQ;

    /// <summary>
    /// Abstract class facilitating the seamless management of escaping behavior within the game environment.
    /// </summary>
    /// <remarks>
    /// The <see cref="CustomEscape"/> class serves as a foundational framework for implementing and controlling various escaping mechanisms.
    /// <para>
    /// As an implementation of <see cref="IAdditiveBehaviour"/>, <see cref="CustomEscape"/> seamlessly integrates into existing systems, allowing developers to extend and enhance escape-related functionalities.
    /// <br/>The class also implements <see cref="IEquatable{CustomEscape}"/> and <see cref="IEquatable{T}"/>, enabling straightforward comparisons for equality checks.
    /// </para>
    /// </remarks>
    public abstract class CustomEscape : CustomModule, IAdditiveBehaviour
    {
        private static readonly Dictionary<Player, CustomEscape> PlayersValue = new();
        private static readonly List<CustomEscape> Registered = new();
        private static readonly Dictionary<byte, Hint> AllScenariosInternal = new();
        private static readonly Dictionary<Type, CustomEscape> TypeLookupTable = new();
        private static readonly Dictionary<Type, CustomEscape> BehaviourLookupTable = new();
        private static readonly Dictionary<uint, CustomEscape> IdLookupTable = new();
        private static readonly Dictionary<string, CustomEscape> NameLookupTable = new();

        /// <summary>
        /// Gets a <see cref="List{T}"/> which contains all registered <see cref="CustomEscape"/>'s.
        /// </summary>
        public static IEnumerable<CustomEscape> List => Registered;

        /// <summary>
        /// Gets all existing <see cref="Hint"/>'s to be displayed based on the relative <see cref="UUEscapeScenarioType"/>.
        /// </summary>
        public static IReadOnlyDictionary<byte, Hint> AllScenarios => AllScenariosInternal;

        /// <summary>
        /// Gets all players and their respective <see cref="CustomEscape"/>.
        /// </summary>
        public static IReadOnlyDictionary<Player, CustomEscape> Manager => PlayersValue;

        /// <summary>
        /// Gets the <see cref="CustomEscape"/>'s name.
        /// </summary>
        public override string Name { get; }

        /// <summary>
        /// Gets or sets the <see cref="CustomEscape"/>'s id.
        /// </summary>
        public override uint Id { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomEscape"/> is enabled.
        /// </summary>
        public override bool IsEnabled { get; }

        /// <summary>
        /// Gets the <see cref="CustomEscape"/>'s <see cref="Type"/>.
        /// </summary>
        public virtual Type BehaviourComponent { get; }

        /// <summary>
        /// Gets all <see cref="Hint"/>'s to be displayed based on the relative <see cref="UUEscapeScenarioType"/>.
        /// </summary>
        public virtual Dictionary<byte, Hint> Scenarios { get; } = new();

        /// <summary>
        /// Gets a <see cref="List{T}"/> of <see cref="EscapeSettings"/> containing all escape settings.
        /// </summary>
        public virtual List<EscapeSettings> Settings { get; } = new() { EscapeSettings.Default, };

        /// <summary>
        /// Enables all the custom escapes present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomEscape"/> containing all enabled custom escapes.</returns>
        public static List<CustomEscape> EnableAll()
        {
            List<CustomEscape> customEscapes = new();
            foreach (Type type in Assembly.GetCallingAssembly().GetTypes())
            {
                CustomEscapeAttribute attribute = type.GetCustomAttribute<CustomEscapeAttribute>();
                if (!typeof(CustomEscape).IsAssignableFrom(type) || attribute is null)
                    continue;

                CustomEscape customEscape = Activator.CreateInstance(type) as CustomEscape;

                if (!customEscape.IsEnabled)
                    continue;

                if (customEscape.TryRegister(attribute))
                    customEscapes.Add(customEscape);
            }

            if (customEscapes.Count() != List.Count())
                Log.Info($"{customEscapes.Count()} custom escapes have been successfully registered!");

            return customEscapes;
        }

        /// <summary>
        /// Disables all the custom escapes present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomEscape"/> containing all disabled custom escapes.</returns>
        public static List<CustomEscape> DisableAll()
        {
            List<CustomEscape> customEscapes = new();
            customEscapes.AddRange(List.Where(customEscape => customEscape.TryUnregister()));

            Log.Info($"{customEscapes.Count()} custom escapes have been successfully unregistered!");

            return customEscapes;
        }

        /// <summary>
        /// Gets a <see cref="CustomEscape"/> given the specified <see cref="Id"/>.
        /// </summary>
        /// <param name="id">The specified id.</param>
        /// <returns>The <see cref="CustomEscape"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomEscape Get(uint id) => IdLookupTable[id];

        /// <summary>
        /// Gets a <see cref="CustomEscape"/> given the specified name.
        /// </summary>
        /// <param name="name">The specified name.</param>
        /// <returns>The <see cref="CustomEscape"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomEscape Get(string name) => NameLookupTable[name];

        /// <summary>
        /// Gets a <see cref="CustomEscape"/> given the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The specified <see cref="Type"/>.</param>
        /// <returns>The <see cref="CustomEscape"/> matching the search or <see langword="null"/> if not found.</returns>
        public static CustomEscape Get(Type type) =>
            typeof(EscapeBehaviour).IsAssignableFrom(type) ? BehaviourLookupTable[type] :
            typeof(CustomEscape).IsAssignableFrom(type) ? TypeLookupTable[type] : null;

        /// <summary>
        /// Gets a <see cref="CustomEscape"/> given the specified <see cref="EscapeBehaviour"/>.
        /// </summary>
        /// <param name="escapeBuilder">The specified <see cref="EscapeBehaviour"/>.</param>
        /// <returns>The <see cref="CustomEscape"/> matching the search or <see langword="null"/> if not found.</returns>
        public static CustomEscape Get(EscapeBehaviour escapeBuilder) => Get(escapeBuilder.GetType());

        /// <summary>
        /// Gets a <see cref="CustomEscape"/> from a <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="CustomEscape"/> owner.</param>
        /// <returns>The <see cref="CustomEscape"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomEscape Get(Player player) => PlayersValue.TryGetValue(player, out CustomEscape customEscape) ? customEscape : default;

        /// <summary>
        /// Tries to get a <see cref="CustomEscape"/> given the specified <see cref="CustomEscape"/>.
        /// </summary>
        /// <param name="id">The id to look for.</param>
        /// <param name="customEscape">The found <see cref="CustomEscape"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomEscape"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(uint id, out CustomEscape customEscape) => customEscape = Get(id);

        /// <summary>
        /// Tries to get a <see cref="CustomEscape"/> given a specified name.
        /// </summary>
        /// <param name="name">The <see cref="CustomEscape"/> name to look for.</param>
        /// <param name="customEscape">The found <see cref="CustomEscape"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomEscape"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(string name, out CustomEscape customEscape) => customEscape = List.FirstOrDefault(cRole => cRole.Name == name);

        /// <summary>
        /// Tries to get the player's current <see cref="CustomEscape"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to search on.</param>
        /// <param name="customEscape">The found <see cref="CustomEscape"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomEscape"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Player player, out CustomEscape customEscape) => customEscape = Get(player);

        /// <summary>
        /// Tries to get the player's current <see cref="CustomEscape"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to search for.</param>
        /// <param name="customEscape">The found <see cref="CustomEscape"/>, <see langword="null"/> if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomEscape"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Type type, out CustomEscape customEscape) => customEscape = Get(type.GetType());

        /// <summary>
        /// Attaches a <see cref="CustomEscape"/> with the specified <paramref name="id"/> to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to attach the escape rules to.</param>
        /// <param name="id">The unique identifier of the <see cref="CustomEscape"/> to attach.</param>
        /// <remarks>
        /// This method attempts to attach a <see cref="CustomEscape"/> to the provided <paramref name="player"/> based on the specified <paramref name="id"/>.
        /// </remarks>
        public static void Attach(Player player, uint id)
        {
            if (TryGet(id, out CustomEscape customEscape))
                customEscape.Attach(player);
        }

        /// <summary>
        /// Attaches a <see cref="CustomEscape"/> with the specified <paramref name="name"/> to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to attach the escape rules to.</param>
        /// <param name="name">The name of the <see cref="CustomEscape"/> to attach.</param>
        /// <remarks>
        /// This method attempts to attach a <see cref="CustomEscape"/> to the provided <paramref name="player"/> based on the specified <paramref name="name"/>.
        /// </remarks>
        public static void Attach(Player player, string name)
        {
            if (TryGet(name, out CustomEscape customEscape))
                customEscape.Attach(player);
        }

        /// <summary>
        /// Attaches a <see cref="CustomEscape"/> with the specified <paramref name="type"/> to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to attach the escape rules to.</param>
        /// <param name="type">The <see cref="Type"/> of the <see cref="CustomEscape"/> to attach.</param>
        /// <remarks>
        /// This method attempts to attach a <see cref="CustomEscape"/> to the provided <paramref name="player"/> based on the specified <paramref name="type"/>.
        /// </remarks>
        public static void Attach(Player player, Type type)
        {
            if (TryGet(type, out CustomEscape customEscape))
                customEscape.Attach(player);
        }

        /// <summary>
        /// Attaches a <typeparamref name="T"/>-derived <see cref="CustomEscape"/> to the specified <see cref="Player"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="CustomEscape"/> to attach.</typeparam>
        /// <param name="player">The <see cref="Player"/> to attach the escape rules to.</param>
        /// <remarks>
        /// This method attempts to attach a <typeparamref name="T"/>-derived <see cref="CustomEscape"/> to the provided <paramref name="player"/>.
        /// </remarks>
        public static void Attach<T>(Player player)
            where T : CustomEscape => Attach(player, typeof(T));

        /// <summary>
        /// Detaches a <see cref="CustomEscape"/> with the specified <paramref name="id"/> from the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to detach the escape rules from.</param>
        /// <param name="id">The unique identifier of the <see cref="CustomEscape"/> to detach.</param>
        /// <remarks>
        /// This method attempts to detach a <see cref="CustomEscape"/> with the provided <paramref name="id"/> from the specified <paramref name="player"/>.
        /// </remarks>
        public static void Detach(Player player, uint id)
        {
            if (TryGet(id, out CustomEscape customEscape))
                customEscape.Detach(player);
        }

        /// <summary>
        /// Detaches a <see cref="CustomEscape"/> with the specified <paramref name="name"/> from the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to detach the escape rules from.</param>
        /// <param name="name">The name of the <see cref="CustomEscape"/> to detach.</param>
        /// <remarks>
        /// This method attempts to detach a <see cref="CustomEscape"/> with the provided <paramref name="name"/> from the specified <paramref name="player"/>.
        /// </remarks>
        public static void Detach(Player player, string name)
        {
            if (TryGet(name, out CustomEscape customEscape))
                customEscape.Detach(player);
        }

        /// <summary>
        /// Detaches a <see cref="CustomEscape"/> with the specified <paramref name="type"/> from the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to detach the escape rules from.</param>
        /// <param name="type">The <see cref="Type"/> of the <see cref="CustomEscape"/> to detach.</param>
        /// <remarks>
        /// This method attempts to detach a <see cref="CustomEscape"/> with the provided <paramref name="type"/> from the specified <paramref name="player"/>.
        /// </remarks>
        public static void Detach(Player player, Type type)
        {
            if (TryGet(type, out CustomEscape customEscape))
                customEscape.Detach(player);
        }

        /// <summary>
        /// Detaches a <typeparamref name="T"/>-derived <see cref="CustomEscape"/> from the specified <see cref="Player"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="CustomEscape"/> to detach.</typeparam>
        /// <param name="player">The <see cref="Player"/> to detach the escape rules from.</param>
        /// <remarks>
        /// This method attempts to detach a <typeparamref name="T"/>-derived <see cref="CustomEscape"/> from the specified <paramref name="player"/>.
        /// </remarks>
        public static void Detach<T>(Player player)
            where T : CustomEscape => Detach(player, typeof(T));

        /// <summary>
        /// Attaches a <see cref="CustomEscape"/> to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to attach the escape rules to.</param>
        public void Attach(Player player)
        {
            PlayersValue.Remove(player);
            PlayersValue.Add(player, this);
            player.AddComponent(BehaviourComponent, $"ECS-{Name}");
        }

        /// <summary>
        /// Detaches a <see cref="CustomEscape"/> to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to detach the escape rules from.</param>
        public void Detach(Player player)
        {
            PlayersValue.Remove(player);

            if (player.TryGetComponent(BehaviourComponent, out EscapeBehaviour eb) && !eb.IsDestroying)
                eb.Destroy();
        }

        /// <summary>
        /// Tries to register a <see cref="CustomEscape"/>.
        /// </summary>
        /// <param name="attribute">The specified <see cref="CustomEscapeAttribute"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomEscape"/> was registered; otherwise, <see langword="false"/>.</returns>
        internal bool TryRegister(CustomEscapeAttribute attribute = null)
        {
            if (!Registered.Contains(this))
            {
                if (attribute is not null && Id == 0)
                {
                    if (attribute.Id != 0)
                        Id = attribute.Id;
                    else
                        throw new ArgumentException($"Unable to register {Name}. The ID 0 is reserved for special use.");
                }

                CustomEscape duplicate = Registered.FirstOrDefault(x => x.Id == Id || x.Name == Name || x.BehaviourComponent == BehaviourComponent);
                if (duplicate)
                {
                    Log.Debug($"Unable to register {Name}. Another escape has been registered with the same ID, Name or Behaviour Component: {duplicate.Name}");

                    return false;
                }

                AllScenariosInternal.AddRange(Scenarios);

                EObject.RegisterObjectType(BehaviourComponent, Name);
                Registered.Add(this);

                TypeLookupTable.TryAdd(GetType(), this);
                BehaviourLookupTable.TryAdd(BehaviourComponent, this);
                IdLookupTable.TryAdd(Id, this);
                NameLookupTable.TryAdd(Name, this);

                return true;
            }

            Log.Debug($"Unable to register {Name}. Another identical escape already exists.");

            return false;
        }

        /// <summary>
        /// Tries to unregister a <see cref="CustomEscape"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="CustomEscape"/> was unregistered; otherwise, <see langword="false"/>.</returns>
        internal bool TryUnregister()
        {
            if (!Registered.Contains(this))
            {
                Log.Debug($"Unable to unregister {Name}. Escape is not yet registered.");

                return false;
            }

            foreach (UUEscapeScenarioType scenario in Scenarios.Keys)
                AllScenariosInternal.Remove(scenario);

            EObject.UnregisterObjectType(BehaviourComponent);
            Registered.Remove(this);

            TypeLookupTable.Remove(GetType());
            BehaviourLookupTable.Remove(BehaviourComponent);
            IdLookupTable.Remove(Id);
            NameLookupTable.Remove(Name);

            return true;
        }
    }
}