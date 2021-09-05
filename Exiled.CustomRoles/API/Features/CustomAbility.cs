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
    using System.Linq;

    using Exiled.API.Features;

    /// <summary>
    /// The custom ability base class.
    /// </summary>
    public abstract class CustomAbility
    {
        /// <summary>
        /// Gets a list of all registered custom abilities.
        /// </summary>
        public static HashSet<CustomAbility> Registered { get; } = new HashSet<CustomAbility>();

        /// <summary>
        /// Gets or sets the name of the ability.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the ability.
        /// </summary>
        public abstract string Description { get; set; }

        /// <summary>
        /// Gets or sets the abilities unique ID number.
        /// </summary>
        public abstract uint Id { get; set; }

        /// <summary>
        /// Gets a <see cref="CustomRole"/> by ID.
        /// </summary>
        /// <param name="id">The ID of the role to get.</param>
        /// <returns>The role, or null if it doesn't exist.</returns>
        public static CustomAbility Get(int id) => Registered?.FirstOrDefault(r => r.Id == id);

        /// <summary>
        /// Gets a <see cref="CustomRole"/> by name.
        /// </summary>
        /// <param name="name">The name of the role to get.</param>
        /// <returns>The role, or null if it doesn't exist.</returns>
        public static CustomAbility Get(string name) => Registered?.FirstOrDefault(r => r.Name == name);

        /// <summary>
        /// Tries to get a <see cref="CustomRole"/> by <inheritdoc cref="Id"/>.
        /// </summary>
        /// <param name="id">The ID of the role to get.</param>
        /// <param name="customAbility">The custom role.</param>
        /// <returns>True if the role exists.</returns>
        public static bool TryGet(int id, out CustomAbility customAbility)
        {
            customAbility = Get(id);

            return customAbility != null;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomRole"/> by name.
        /// </summary>
        /// <param name="name">The name of the role to get.</param>
        /// <param name="customAbility">The custom role.</param>
        /// <returns>True if the role exists.</returns>
        /// <exception cref="ArgumentNullException">If the name is a null or empty string.</exception>
        public static bool TryGet(string name, out CustomAbility customAbility)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            customAbility = int.TryParse(name, out int id) ? Get(id) : Get(name);

            return customAbility != null;
        }

        /// <summary>
        /// Tries to register this ability.
        /// </summary>
        /// <returns>True if the ability registered properly.</returns>
        public bool TryRegister()
        {
            if (!Registered.Contains(this))
            {
                if (Registered.Any(r => r.Id == Id))
                {
                    Log.Warn($"{Name} has tried to register with the same ability ID as another ability: {Id}. It will not be registered!");

                    return false;
                }

                Registered.Add(this);
                Init();

                Log.Debug($"{Name} ({Id}) has been successfully registered.", CustomRoles.Instance.Config.Debug);

                return true;
            }

            Log.Warn($"Couldn't register {Name} ({Id}) as it already exists.");

            return false;
        }

        /// <summary>
        /// Tries to unregister this ability.
        /// </summary>
        /// <returns>True if the ability is unregistered properly.</returns>
        public bool TryUnregister()
        {
            Destroy();

            if (!Registered.Remove(this))
            {
                Log.Warn($"Cannot unregister {Name} ({Id}), it hasn't been registered yet.");

                return false;
            }

            return true;
        }

        /// <summary>
        /// Initializes this ability.
        /// </summary>
        public void Init() => SubscribeEvents();

        /// <summary>
        /// Destroys this ability.
        /// </summary>
        public void Destroy() => UnSubscribeEvents();

        /// <summary>
        /// Uses the ability.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> using the ability.</param>
        public void UseAbility(Player player)
        {
            AbilityUsed(player);
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
        protected virtual void UnSubscribeEvents()
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

        /// <summary>
        /// Called when the ability is used.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> using the ability.</param>
        protected virtual void AbilityUsed(Player player)
        {
        }

        /// <summary>
        /// Called when the ability is successfully used.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> using the ability.</param>
        protected virtual void ShowMessage(Player player) =>
            player.ShowHint($"Ability {Name} has been activated.\n{Description}", 10f);
    }
}
