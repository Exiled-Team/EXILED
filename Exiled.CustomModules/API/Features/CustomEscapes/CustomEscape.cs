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
    using Exiled.CustomModules.API.Features.CustomRoles;
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
    public abstract class CustomEscape : TypeCastObject<CustomEscape>, IAdditiveBehaviour, IEquatable<CustomEscape>, IEquatable<uint>
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
        public abstract string Name { get; }

        /// <summary>
        /// Gets or sets the <see cref="CustomEscape"/>'s id.
        /// </summary>
        public virtual uint Id { get; protected set; }

        /// <summary>
        /// Gets the <see cref="CustomEscape"/>'s <see cref="Type"/>.
        /// </summary>
        public virtual Type BehaviourComponent { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomEscape"/> is enabled.
        /// </summary>
        public virtual bool IsEnabled { get; }

        /// <summary>
        /// Gets all <see cref="Hint"/>'s to be displayed based on the relative <see cref="UUEscapeScenarioType"/>.
        /// </summary>
        public virtual Dictionary<byte, Hint> Scenarios { get; } = new();

        /// <summary>
        /// Gets a <see cref="List{T}"/> of <see cref="EscapeSettings"/> containing all escape settings.
        /// </summary>
        public virtual List<EscapeSettings> Settings { get; }

        /// <summary>
        /// Compares two operands: <see cref="CustomEscape"/> and <see cref="object"/>.
        /// </summary>
        /// <param name="left">The <see cref="CustomEscape"/> to compare.</param>
        /// <param name="right">The <see cref="object"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(CustomEscape left, object right) => left is null ? right is null : left.Equals(right);

        /// <summary>
        /// Compares two operands: <see cref="object"/> and <see cref="CustomEscape"/>.
        /// </summary>
        /// <param name="left">The <see cref="object"/> to compare.</param>
        /// <param name="right">The <see cref="CustomEscape"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator ==(object left, CustomEscape right) => right == left;

        /// <summary>
        /// Compares two operands: <see cref="CustomEscape"/> and <see cref="object"/>.
        /// </summary>
        /// <param name="left">The <see cref="object"/> to compare.</param>
        /// <param name="right">The <see cref="CustomEscape"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(CustomEscape left, object right) => !(left == right);

        /// <summary>
        /// Compares two operands: <see cref="object"/> and <see cref="CustomRole"/>.
        /// </summary>
        /// <param name="left">The left <see cref="object"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomRole"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(object left, CustomEscape right) => !(left == right);

        /// <summary>
        /// Compares two operands: <see cref="CustomEscape"/> and <see cref="CustomEscape"/>.
        /// </summary>
        /// <param name="left">The left <see cref="CustomEscape"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomEscape"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(CustomEscape left, CustomEscape right) => left is null ? right is null : left.Equals(right);

        /// <summary>
        /// Compares two operands: <see cref="CustomEscape"/> and <see cref="CustomEscape"/>.
        /// </summary>
        /// <param name="left">The left <see cref="CustomEscape"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomEscape"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(CustomEscape left, CustomEscape right) => !(left.Id == right.Id);

        /// <summary>
        /// Enables all the custom roles present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomEscape"/> which contains all the enabled custom roles.</returns>
        public static List<CustomEscape> RegisterAll()
        {
            List<CustomEscape> customEscapes = new();
            foreach (Type type in Assembly.GetCallingAssembly().GetTypes())
            {
                CustomEscapeAttribute attribute = type.GetCustomAttribute<CustomEscapeAttribute>();
                if ((type.BaseType != typeof(CustomEscape) && !type.IsSubclassOf(typeof(CustomEscape))) || attribute is null)
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
        /// Disables all the custom roles present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomEscape"/> which contains all the disabled custom roles.</returns>
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
        /// Determines whether the provided id is equal to the current object.
        /// </summary>
        /// <param name="id">The id to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public bool Equals(uint id) => Id == id;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="ce">The custom escape to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public bool Equals(CustomEscape ce) => ce && (ReferenceEquals(this, ce) || Id == ce.Id);

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            if (Equals(obj as CustomEscape))
                return true;

            try
            {
                return Equals((uint)obj);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a the 32-bit signed hash code of the current object instance.
        /// </summary>
        /// <returns>The 32-bit signed hash code of the current object instance.</returns>
        public override int GetHashCode() => base.GetHashCode();

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
            player.GetComponent(BehaviourComponent).Destroy();
        }

        /// <summary>
        /// Tries to register a <see cref="CustomEscape"/>.
        /// </summary>
        /// <param name="attribute">The specified <see cref="CustomEscapeAttribute"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomEscape"/> was registered; otherwise, <see langword="false"/>.</returns>
        internal bool TryRegister(CustomEscapeAttribute attribute = null)
        {
            if (!List.Contains(this))
            {
                if (attribute is not null && Id == 0)
                {
                    if (attribute.Id != 0)
                        Id = attribute.Id;
                    else
                        throw new ArgumentException($"Unable to register {Name}. The ID 0 is reserved for special use.");
                }

                if (List.Any(x => x.Id == Id))
                {
                    Log.Debug($"Unable to register {Name}. Another escape has been registered with the same Id: {List.FirstOrDefault(x => x.Id == Id)}");

                    return false;
                }

                AllScenariosInternal.AddRange(Scenarios);
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
        /// Tries to unregister a <see cref="CustomRole"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="CustomRole"/> was unregistered; otherwise, <see langword="false"/>.</returns>
        internal bool TryUnregister()
        {
            if (!List.Contains(this))
            {
                Log.Debug($"Unable to unregister {Name}. Escape is not yet registered.");

                return false;
            }

            foreach (UUEscapeScenarioType scenario in Scenarios.Keys)
                AllScenariosInternal.Remove(scenario);

            Registered.Remove(this);
            TypeLookupTable.Remove(GetType());
            BehaviourLookupTable.Remove(BehaviourComponent);
            IdLookupTable.Remove(Id);
            NameLookupTable.Remove(Name);

            return true;
        }
    }
}