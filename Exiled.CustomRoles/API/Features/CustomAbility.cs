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

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using UnityEngine;

    /// <summary>
    /// The custom ability base class.
    /// </summary>
    public abstract class CustomAbility : MonoBehaviour
    {
        /// <summary>
        /// Gets a list of all registered custom abilities.
        /// </summary>
        public static List<CustomAbility> RegisteredAbilities { get; } = new List<CustomAbility>();

        /// <summary>
        /// Gets or sets the name of the ability.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the ability.
        /// </summary>
        public abstract string Description { get; set; }

        /// <summary>
        /// Gets or sets how long the ability lasts.
        /// </summary>
        public virtual float Duration { get; set; }

        /// <summary>
        /// Gets the <see cref="Exiled.API.Features.Player"/> using the ability.
        /// </summary>
        protected Player Player { get; private set; }

        /// <summary>
        /// Loads the internal event handlers for the ability.
        /// </summary>
        protected virtual void LoadEvents()
        {
            Exiled.Events.Handlers.Player.ChangingRole += OnInternalChangingRole;
        }

        /// <summary>
        /// Unloads the internal event handlers for the ability.
        /// </summary>
        protected virtual void UnloadEvents()
        {
            Exiled.Events.Handlers.Player.ChangingRole -= OnInternalChangingRole;
        }

        /// <summary>
        /// Called when the ability is first added to the player.
        /// </summary>
        protected virtual void AbilityAdded()
        {
        }

        /// <summary>
        /// Called when the ability is being removed.
        /// </summary>
        protected virtual void AbilityRemoved()
        {
        }

        /// <summary>
        /// Called when the ability is successfully used.
        /// </summary>
        protected virtual void ShowMessage() =>
            Player.ShowHint($"Ability {Name} has been activated.\n{Description}", 10f);

        private void Start()
        {
            Player = Player.Get(this.gameObject);

            if (Player == null)
            {
                Log.Warn($"{Name}: tried to add ability component to a player that doesn't exist!");
                Destroy(this);
            }

            LoadEvents();
            AbilityAdded();
        }

        private void OnInternalChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.Player == Player)
                Destroy(this);
        }

        private void OnDestroy()
        {
            UnloadEvents();
            AbilityRemoved();
        }
    }
}
