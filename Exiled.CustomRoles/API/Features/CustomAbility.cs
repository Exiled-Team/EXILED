// -----------------------------------------------------------------------
// <copyright file="CustomAbility.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Interfaces;

    using YamlDotNet.Serialization;

    /// <summary>
    /// The custom ability base class.
    /// </summary>
    public abstract class CustomAbility
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAbility"/> class.
        /// </summary>
        public CustomAbility()
        {
            AbilityType = GetType().Name;
        }

        /// <summary>
        /// Gets a list of all registered custom abilities.
        /// </summary>
        public static HashSet<CustomAbility> Registered { get; } = new();

        /// <summary>
        /// Gets or sets the name of the ability.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the ability.
        /// </summary>
        public abstract string Description { get; set; }

        /// <summary>
        /// Gets all players who have this ability.
        /// </summary>
        [YamlIgnore]
        public HashSet<Player> Players { get; } = new();

        /// <summary>
        /// Gets the <see cref="Type"/> for this ability.
        /// </summary>
        [Description("Changing this will likely break your config.")]
        public string AbilityType { get; }

        /// <summary>
        /// Gets a <see cref="CustomAbility"/> by name.
        /// </summary>
        /// <param name="name">The name of the ability to get.</param>
        /// <returns>The ability, or <see langword="null"/> if it doesn't exist.</returns>
        public static CustomAbility? Get(string name) => Registered?.FirstOrDefault(r => r.Name.ToLower() == name);

        /// <summary>
        /// Gets a <see cref="CustomAbility"/> by type.
        /// </summary>
        /// <param name="type">The type of the ability to get.</param>
        /// <returns>The type, or <see langword="null"/> if it doesn't exist.</returns>
        public static CustomAbility? Get(Type type) => Registered?.FirstOrDefault(r => r.GetType() == type);

        /// <summary>
        /// Tries to get a <see cref="CustomAbility"/> by type.
        /// </summary>
        /// <param name="type">The type of the ability to get.</param>
        /// <param name="customAbility">The custom ability.</param>
        /// <returns>True if the ability exists, otherwise false.</returns>
        public static bool TryGet(Type type, out CustomAbility? customAbility)
        {
            customAbility = Get(type);

            return customAbility is not null;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomAbility"/> by name.
        /// </summary>
        /// <param name="name">The name of the ability to get.</param>
        /// <param name="customAbility">The custom ability.</param>
        /// <returns>True if the ability exists.</returns>
        /// <exception cref="ArgumentNullException">If the name is <see langword="null"/> or an empty string.</exception>
        public static bool TryGet(string name, out CustomAbility? customAbility)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            customAbility = Get(name);

            return customAbility is not null;
        }

        /// <summary>
        /// Registers all the <see cref="CustomAbility"/>'s present in the current assembly.
        /// </summary>
        /// <param name="skipReflection">Whether or not reflection is skipped (more efficient if you are not using your custom item classes as config objects).</param>
        /// <param name="overrideClass">The class to search properties for, if different from the plugin's config class.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomAbility"/> which contains all registered <see cref="CustomAbility"/>'s.</returns>
        public static IEnumerable<CustomAbility> RegisterAbilities(bool skipReflection = false, object? overrideClass = null)
        {
            List<CustomAbility> abilities = new();
            Assembly assembly = Assembly.GetCallingAssembly();
            foreach (Type type in assembly.GetTypes())
            {
                if (type.BaseType != typeof(CustomAbility) || type.GetCustomAttribute(typeof(CustomAbilityAttribute)) is null)
                    continue;

                CustomAbility? customAbility = null;

                if (!skipReflection && Server.PluginAssemblies.ContainsKey(assembly))
                {
                    IPlugin<IConfig> plugin = Server.PluginAssemblies[assembly];

                    foreach (PropertyInfo property in overrideClass?.GetType().GetProperties() ??
                                                      plugin.Config.GetType().GetProperties())
                    {
                        if (property.PropertyType != type)
                            continue;

                        customAbility = property.GetValue(overrideClass ?? plugin.Config) as CustomAbility;
                        break;
                    }
                }

                if (customAbility is null)
                    customAbility = (CustomAbility)Activator.CreateInstance(type);

                if (customAbility.TryRegister())
                    abilities.Add(customAbility);
            }

            return abilities;
        }

        /// <summary>
        /// Registers all the <see cref="CustomAbility"/>'s present in the current assembly.
        /// </summary>
        /// <param name="targetTypes">The <see cref="IEnumerable{T}"/> of <see cref="Type"/> containing the target types.</param>
        /// <param name="isIgnored">A value indicating whether the target types should be ignored.</param>
        /// <param name="skipReflection">Whether or not reflection is skipped (more efficient if you are not using your custom item classes as config objects).</param>
        /// <param name="overrideClass">The class to search properties for, if different from the plugin's config class.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomAbility"/> which contains all registered <see cref="CustomAbility"/>'s.</returns>
        public static IEnumerable<CustomAbility> RegisterAbilities(IEnumerable<Type> targetTypes, bool isIgnored = false, bool skipReflection = false, object? overrideClass = null)
        {
            List<CustomAbility> abilities = new();
            Assembly assembly = Assembly.GetCallingAssembly();
            foreach (Type type in assembly.GetTypes())
            {
                if (((type.BaseType != typeof(CustomAbility)) && !type.IsSubclassOf(typeof(CustomAbility))) || type.GetCustomAttribute(typeof(CustomAbilityAttribute)) is null ||
                    (isIgnored && targetTypes.Contains(type)) || (!isIgnored && !targetTypes.Contains(type)))
                    continue;

                CustomAbility? customAbility = null;

                if (!skipReflection && Server.PluginAssemblies.ContainsKey(assembly))
                {
                    IPlugin<IConfig> plugin = Server.PluginAssemblies[assembly];

                    foreach (PropertyInfo property in overrideClass?.GetType().GetProperties() ?? plugin.Config.GetType().GetProperties())
                    {
                        if (property.PropertyType != type)
                            continue;

                        customAbility = property.GetValue(overrideClass ?? plugin.Config) as CustomAbility;
                    }
                }

                if (customAbility is null)
                    customAbility = (CustomAbility)Activator.CreateInstance(type);

                if (customAbility.TryRegister())
                    abilities.Add(customAbility);
            }

            return abilities;
        }

        /// <summary>
        /// Unregisters all the <see cref="CustomAbility"/>'s present in the current assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomAbility"/> which contains all unregistered <see cref="CustomAbility"/>'s.</returns>
        public static IEnumerable<CustomAbility> UnregisterAbilities()
        {
            List<CustomAbility> unregisteredAbilities = new();

            foreach (CustomAbility customAbility in Registered)
            {
                customAbility.TryUnregister();
                unregisteredAbilities.Add(customAbility);
            }

            return unregisteredAbilities;
        }

        /// <summary>
        /// Unregisters all the <see cref="CustomAbility"/>'s present in the current assembly.
        /// </summary>
        /// <param name="targetTypes">The <see cref="IEnumerable{T}"/> of <see cref="Type"/> containing the target types.</param>
        /// <param name="isIgnored">A value indicating whether the target types should be ignored.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomAbility"/> which contains all unregistered <see cref="CustomAbility"/>'s.</returns>
        public static IEnumerable<CustomAbility> UnregisterAbilities(IEnumerable<Type> targetTypes, bool isIgnored = false)
        {
            List<CustomAbility> unregisteredAbilities = new();

            foreach (CustomAbility customAbility in Registered)
            {
                if ((targetTypes.Contains(customAbility.GetType()) && isIgnored) || (!targetTypes.Contains(customAbility.GetType()) && !isIgnored))
                    continue;

                customAbility.TryUnregister();
                unregisteredAbilities.Add(customAbility);
            }

            return unregisteredAbilities;
        }

        /// <summary>
        /// Unregisters all the <see cref="CustomAbility"/>'s present in the current assembly.
        /// </summary>
        /// <param name="targetAbilities">The <see cref="IEnumerable{T}"/> of <see cref="CustomAbility"/> containing the target roles.</param>
        /// <param name="isIgnored">A value indicating whether the target abilities should be ignored.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomAbility"/> which contains all unregistered <see cref="CustomAbility"/>'s.</returns>
        public static IEnumerable<CustomAbility> UnregisterAbilities(IEnumerable<CustomAbility> targetAbilities, bool isIgnored = false) => UnregisterAbilities(targetAbilities.Select(x => x.GetType()), isIgnored);

        /// <summary>
        /// Checks to see if the specified player has this ability.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns>True if the player has this ability.</returns>
        public virtual bool Check(Player player) => Players.Contains(player);

        /// <summary>
        /// Adds this ability to the player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to give the ability to.</param>
        public void AddAbility(Player player)
        {
            Log.Debug($"Added {Name} to {player.Nickname}");
            Players.Add(player);
            AbilityAdded(player);
        }

        /// <summary>
        /// Removes this ability from the player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to remove this ability from.</param>
        public void RemoveAbility(Player player)
        {
            Log.Debug($"Removed {Name} from {player.Nickname}");
            Players.Remove(player);
            AbilityRemoved(player);
        }

        /// <summary>
        /// Initializes this ability.
        /// </summary>
        public void Init() => SubscribeEvents();

        /// <summary>
        /// Destroys this ability.
        /// </summary>
        public void Destroy() => UnsubscribeEvents();

        /// <summary>
        /// Tries to register this ability.
        /// </summary>
        /// <returns>True if the ability registered properly.</returns>
        internal bool TryRegister()
        {
            if (!CustomRoles.Instance!.Config.IsEnabled)
                return false;

            if (!Registered.Contains(this))
            {
                Registered.Add(this);
                Init();

                Log.Debug($"{Name} has been successfully registered.");

                return true;
            }

            Log.Warn($"Couldn't register {Name} as it already exists.");

            return false;
        }

        /// <summary>
        /// Tries to unregister this ability.
        /// </summary>
        /// <returns>True if the ability is unregistered properly.</returns>
        internal bool TryUnregister()
        {
            Destroy();

            if (!Registered.Remove(this))
            {
                Log.Warn($"Cannot unregister {Name}, it hasn't been registered yet.");

                return false;
            }

            return true;
        }

        /// <summary>
        /// Loads the internal event handlers for the ability.
        /// </summary>
        protected virtual void SubscribeEvents()
        {
        }

        /// <summary>
        /// Unloads the internal event handlers for the ability.
        /// </summary>
        protected virtual void UnsubscribeEvents()
        {
        }

        /// <summary>
        /// Called when the ability is first added to the player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> using the ability.</param>
        protected virtual void AbilityAdded(Player player)
        {
        }

        /// <summary>
        /// Called when the ability is being removed.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> using the ability.</param>
        protected virtual void AbilityRemoved(Player player)
        {
        }
    }
}