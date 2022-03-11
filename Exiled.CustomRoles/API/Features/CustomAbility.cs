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

    using YamlDotNet.Serialization;

    /// <summary>
    /// The custom ability base class.
    /// </summary>
    public abstract class CustomAbility
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAbility"/> class.
        /// </summary>
        public CustomAbility() => AbilityType = GetType().Name;

        /// <summary>
        /// Gets a list of all registered custom abilities.
        /// </summary>
        public static HashSet<CustomAbility> Registered { get; } = new HashSet<CustomAbility>();

        /// <summary>
        /// Gets all players who have this ability.
        /// </summary>
        [YamlIgnore]
        public IEnumerable<Player> Players => Player.Get(x =>
            x.GetCustomRoles().Any(y => y.CustomAbilities.Any(z => z.AbilityType == AbilityType)));

        /// <summary>
        /// Gets or sets the name of the ability.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the ability.
        /// </summary>
        public abstract string Description { get; set; }

        /// <summary>
        /// Gets the <see cref="Type"/> for this ability.
        /// </summary>
        [Description("Changing this will likely break your config.")]
        public string AbilityType { get; }

        /// <summary>
        /// Gets a <see cref="CustomRole"/> by name.
        /// </summary>
        /// <param name="name">The name of the role to get.</param>
        /// <returns>The role, or <see langword="null"/> if it doesn't exist.</returns>
        public static CustomAbility Get(string name) => Registered?.FirstOrDefault(r => r.Name == name);

        /// <summary>
        /// Tries to get a <see cref="CustomRole"/> by name.
        /// </summary>
        /// <param name="name">The name of the role to get.</param>
        /// <param name="customAbility">The custom role.</param>
        /// <returns>True if the role exists.</returns>
        /// <exception cref="ArgumentNullException">If the name is <see langword="null"/> or an empty string.</exception>
        public static bool TryGet(string name, out CustomAbility customAbility)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            customAbility = Get(name);

            return customAbility != null;
        }

        /// <summary>
        /// Registers all the <see cref="CustomAbility"/>'s present in the current assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomAbility"/> which contains all registered <see cref="CustomAbility"/>'s.</returns>
        public static IEnumerable<CustomAbility> RegisterAbilities()
        {
            List<CustomAbility> registeredAbilities = new List<CustomAbility>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.BaseType != typeof(CustomAbility) || type.GetCustomAttribute(typeof(CustomAbilityAttribute)) is null)
                    continue;

                CustomAbility customAbility = (CustomAbility)Activator.CreateInstance(type);
                customAbility.TryRegister();
                registeredAbilities.Add(customAbility);
            }

            return registeredAbilities;
        }

        /// <summary>
        /// Registers all the <see cref="CustomAbility"/>'s present in the current assembly.
        /// </summary>
        /// <param name="targetTypes">The <see cref="IEnumerable{T}"/> of <see cref="Type"/> containing the target types.</param>
        /// <param name="isIgnored">A value indicating whether the target types should be ignored.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomAbility"/> which contains all registered <see cref="CustomAbility"/>'s.</returns>
        public static IEnumerable<CustomAbility> RegisterAbilities(IEnumerable<Type> targetTypes, bool isIgnored = false)
        {
            List<CustomAbility> registeredAbilities = new List<CustomAbility>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.BaseType != typeof(CustomAbility) || type.GetCustomAttribute(typeof(CustomAbilityAttribute)) is null ||
                    (isIgnored && targetTypes.Contains(type)) || (!isIgnored && !targetTypes.Contains(type)))
                    continue;

                CustomAbility customAbility = (CustomAbility)Activator.CreateInstance(type);
                customAbility.TryRegister();
                registeredAbilities.Add(customAbility);
            }

            return registeredAbilities;
        }

        /// <summary>
        /// Unregisters all the <see cref="CustomAbility"/>'s present in the current assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomAbility"/> which contains all unregistered <see cref="CustomAbility"/>'s.</returns>
        public static IEnumerable<CustomAbility> UnregisterAbilities()
        {
            List<CustomAbility> unregisteredAbilities = new List<CustomAbility>();

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
            List<CustomAbility> unregisteredAbilities = new List<CustomAbility>();

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
            AbilityAdded(player);
        }

        /// <summary>
        /// Removes this ability from the player.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to remove this ability from.</param>
        public void RemoveAbility(Player player)
        {
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
            if (!Registered.Contains(this))
            {
                Registered.Add(this);
                Init();

                Log.Debug($"{Name} has been successfully registered.", CustomRoles.Instance.Config.Debug);

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
