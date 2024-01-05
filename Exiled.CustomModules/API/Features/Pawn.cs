// -----------------------------------------------------------------------
// <copyright file="Pawn.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Roles;
    using Exiled.CustomModules.API.Features.CustomAbilities;
    using Exiled.CustomModules.API.Features.CustomEscapes;
    using Exiled.CustomModules.API.Features.CustomItems;
    using Exiled.CustomModules.API.Features.CustomRoles;
    using Exiled.CustomModules.API.Features.PlayerAbilities;
    using Exiled.CustomModules.Events.EventArgs.CustomAbilities;
    using Exiled.Events.EventArgs.Player;
    using PlayerRoles;
    using UnityEngine;

    /// <summary>
    /// Represents an in-game player by encapsulating a <see cref="ReferenceHub"/>, providing an extended feature set through the <see cref="Pawn"/> class.
    /// <para>
    /// The <see cref="Pawn"/> class enhances the functionality of the base <see cref="Player"/> class, introducing additional features and capabilities.
    /// <br>This class is designed to be used seamlessly alongside existing methods that expect a <see cref="Player"/> as a parameter, allowing for compatibility with the existing codebase.</br>
    /// <para>The use of nullable context is enabled to prevent users from inadvertently passing or interacting with <see langword="null"/> references.</para>
    /// </para>
    /// <remarks>
    /// Developers can leverage the enhanced functionality provided by the <see cref="Pawn"/> class while benefiting from the familiar interface of the <see cref="Player"/> class.
    /// <br>It serves as a comprehensive representation of an in-game entity, encapsulating the associated <see cref="ReferenceHub"/> with an expanded set of features.</br>
    /// </remarks>
    /// </summary>
    public class Pawn : Player
    {
        private readonly List<AbilityBehaviour> abilityBehaviours = new();
        private readonly List<PlayerAbility> customAbilities = new();

        private RoleBehaviour roleBehaviour;
        private EscapeBehaviour escapeBehaviour;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pawn"/> class.
        /// </summary>
        /// <param name="referenceHub">The <see cref="ReferenceHub"/> of the player to be encapsulated.</param>
        public Pawn(ReferenceHub referenceHub)
            : base(referenceHub)
        {
            PlayerAbility.AddedAbilityDispatcher.Bind(this, OnAddedAbility);
            PlayerAbility.RemovingAbilityDispatcher.Bind(this, OnRemovingAbility);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pawn"/> class.
        /// </summary>
        /// <param name="gameObject">The <see cref="GameObject"/> of the player.</param>
        public Pawn(GameObject gameObject)
            : base(gameObject)
        {
            PlayerAbility.AddedAbilityDispatcher.Bind(this, OnAddedAbility);
            PlayerAbility.RemovingAbilityDispatcher.Bind(this, OnRemovingAbility);
        }

        /// <summary>
        /// Gets all pawn's <see cref="EPlayerBehaviour"/>'s.
        /// </summary>
        public IEnumerable<EPlayerBehaviour> Behaviours => GetComponents<EPlayerBehaviour>();

        /// <summary>
        /// Gets the pawn's <see cref="CustomRoles.CustomRole"/>.
        /// </summary>
        public CustomRole CustomRole => roleBehaviour.CustomRole;

        /// <summary>
        /// Gets the pawn's <see cref="CustomRoles.CustomTeam"/>.
        /// </summary>
        public CustomTeam CustomTeam => roleBehaviour.CustomTeam;

        /// <summary>
        /// Gets the pawn's <see cref="CustomEscapes.CustomEscape"/>.
        /// </summary>
        public CustomEscape CustomEscape => escapeBehaviour.CustomEscape;

        /// <summary>
        /// Gets the pawn's custom abilities.
        /// </summary>
        public IEnumerable<PlayerAbility> CustomAbilities => customAbilities;

        /// <summary>
        /// Gets the pawn's ability behaviours.
        /// </summary>
        public IEnumerable<AbilityBehaviour> AbilityBehaviours => abilityBehaviours;

        /// <summary>
        /// Gets the pawn's <see cref="CustomRoles.RoleBehaviour"/>.
        /// </summary>
        public RoleBehaviour RoleBehaviour => roleBehaviour ??= GetComponent<RoleBehaviour>();

        /// <summary>
        /// Gets the pawn's <see cref="CustomEscapes.EscapeBehaviour"/>.
        /// </summary>
        public EscapeBehaviour EscapeBehaviour => escapeBehaviour ??= GetComponent<EscapeBehaviour>();

        /// <summary>
        /// Gets the pawn's ability behaviours.
        /// </summary>
        public List<AbilityBehaviour> AbilityBehaviours { get; } = new();

        /// <summary>
        /// Gets a value indicating whether the pawn is any SCP, including custom ones.
        /// </summary>
        public new bool IsScp
        {
            get
            {
                if (IsCustomScp)
                    return true;

                Team? team = Role?.Team;
                return team.HasValue && team.GetValueOrDefault() == Team.SCPs;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the pawn is any custom SCP.
        /// </summary>
        public bool IsCustomScp => CustomRole is not null && CustomRole.IsScp;

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="object"/> containing all custom items in the pawn's inventory.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="object"/> which contains all found custom items.</returns>
        public IEnumerable<CustomItem> CustomItems
        {
            get
            {
                foreach (Item item in Items)
                {
                    if (!CustomItem.TryGet(item, out CustomItem customItem) || customItem is null)
                        continue;

                    yield return customItem;
                }
            }
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="EffectType"/>.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="EffectType"/>.</returns>
        public IEnumerable<EffectType> EffectTypes
        {
            get
            {
                foreach (object effect in Enum.GetValues(typeof(EffectType)))
                {
                    if (!Enum.TryParse(effect.ToString(), out EffectType effectType) || !TryGetEffect(effectType, out _))
                        continue;

                    yield return effectType;
                }
            }
        }

        /// <summary>
        /// Tries to get the first <see cref="CustomItem"/> of the specified type from the collection of custom items.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="CustomItem"/> to retrieve.</typeparam>
        /// <param name="customItem">The output parameter that will contain the retrieved <see cref="CustomItem"/>, if found.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomItem"/> of the specified type was found; otherwise, <see langword="false"/>.</returns>
        public bool TryGetCustomItem<T>(out T customItem)
            where T : CustomItem => customItem = CustomItems.FirstOrDefault(item => item.GetType() == typeof(T)).Cast<T>();

        /// <summary>
        /// Tries to get the <see cref="CustomRoles.CustomRole"/> of the specified type from the <see cref="Pawn"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="CustomRoles.CustomRole"/> to retrieve.</typeparam>
        /// <param name="customRole">The output parameter that will contain the retrieved <see cref="CustomRoles.CustomRole"/>, if found.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomRoles.CustomRole"/> of the specified type was found; otherwise, <see langword="false"/>.</returns>
        public bool TryGetCustomRole<T>(out T customRole)
            where T : CustomRole => CustomRole.Is(out customRole);

        /// <summary>
        /// Tries to get the <see cref="CustomAbility{T}"/> of the specified type from the player's abilities.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="CustomAbility{T}"/> to retrieve.</typeparam>
        /// <param name="customAbility">The output parameter that will contain the retrieved <see cref="CustomAbility{T}"/>, if found.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomAbility{T}"/> of the specified type was found; otherwise, <see langword="false"/>.</returns>
        public bool TryGetCustomAbility<T>(out T customAbility)
            where T : PlayerAbility => CustomAbilities.FirstOrDefault(ability => ability.GetType() == typeof(T)).Is(out customAbility);

        /// <summary>
        /// Add a <see cref="CustomItem"/> of the specified type to the pawn's inventory.
        /// </summary>
        /// <param name="customItem">The item to be added.</param>
        /// <returns><see langword="true"/> if the item has been given to the pawn; otherwise, <see langword="false"/>.</returns>
        public bool AddItem(object customItem)
        {
            if (IsInventoryFull)
                return false;

            try
            {
                uint value = (uint)customItem;
                CustomItem.TryGive(this, value);
                return true;
            }
            catch
            {
                return customItem is CustomItem instance && CustomItem.TryGive(this, instance.Id);
            }
        }

        /// <summary>
        /// Checks if the player has a custom role, optionally verifying a specific role name.
        /// </summary>
        /// <param name="player">The player object to check for a custom role.</param>
        /// <param name="roleName">The optional name of the specific custom role to check for.</param>
        /// <returns>
        /// <see langword="true"/> if the player has any custom role, or if a specific role name is provided, 
        /// returns <see langword="true"/> if the player has the specified custom role;
        /// otherwise, returns <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when the provided player object is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when the provided player object is not of type Pawn.</exception>
        public bool HasCustomRole(Pawn player, string roleName = null)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            if (player == null)
                throw new ArgumentException("The provided player object must be of type Pawn.", nameof(player));

            if (string.IsNullOrEmpty(roleName))
            {
                return CustomRole.TryGet(player, out _);
            }
            else
            {
                return CustomRole.TryGet(player, out _, roleName);
            }
        }

        /// <summary>
        /// Adds a <see cref="IEnumerable{T}"/> of <see cref="object"/> containing all the custom items to the pawn's inventory.
        /// </summary>
        /// <param name="customItems">The custom items to be added.</param>
        public void AddItem(IEnumerable<object> customItems)
        {
            foreach (object customItemType in customItems)
            {
                if (!AddItem(customItemType))
                    break;
            }
        }

        /// <summary>
        /// Sets the pawn's role.
        /// </summary>
        /// <param name="role">The role to be set.</param>
        /// <param name="preservePlayerPosition">A value indicating whether the <see cref="Pawn"/> should be spawned in the same position.</param>
        public void SetRole(object role, bool preservePlayerPosition = false)
        {
            if (role is RoleTypeId roleType)
            {
                Role.Set(roleType);
                return;
            }

            if (role is uint id)
                CustomRole.Spawn(this, id, preservePlayerPosition);

            throw new ArgumentException("The type of the role instance is not compatible with RoleTypeId or uint.");
        }

        /// <summary>
        /// Safely drops an item.
        /// </summary>
        /// <param name="item">The item to be dropped.</param>
        public void SafeDropItem(Item item)
        {
            if (TryGetCustomItem(out CustomItem customItem))
            {
                RemoveItem(item, false);
                customItem?.Spawn(Position, this);
                return;
            }

            DropItem(item);
        }

        private void OnAddedAbility(AddedAbilityEventArgs<Player> ev)
        {
            abilityBehaviours.Add(GetComponent(ev.Ability.BehaviourComponent).Cast<AbilityBehaviour>());
            customAbilities.Add(ev.Ability.Cast<PlayerAbility>());
        }

        private void OnRemovingAbility(RemovingAbilityEventArgs<Player> ev)
        {
            abilityBehaviours.Remove(GetComponent(ev.Ability.BehaviourComponent).Cast<AbilityBehaviour>());
            customAbilities.Remove(ev.Ability.Cast<PlayerAbility>());
        }
    }
}