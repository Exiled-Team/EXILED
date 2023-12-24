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
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomModules.API.Features.CustomAbilities;
    using Exiled.CustomModules.API.Features.CustomEscapes;
    using Exiled.CustomModules.API.Features.CustomRoles;
    using PlayerRoles;
    using UnityEngine;

    /// <summary>
    /// Represents the in-game player, by encapsulating a ReferenceHub.
    /// <para>
    /// <see cref="Pawn"/> implements more features in addition to <see cref="Player"/>'s existing ones.
    /// <br>This is treated as a <see cref="Player"/>, which means it can be used along with existing methods asking for a <see cref="Player"/> as parameter.</br>
    /// <para>Nullable context is enabled in order to prevent users to pass or interact with <see langword="null"/> references.</para>
    /// </para>
    /// </summary>
    public class Pawn : Player
    {
        private RoleBehaviour roleBehaviour;
        private EscapeBehaviour escapeBehaviour;
        private CustomRole customRole;
        private CustomTeam customTeam;
        private CustomEscape customEscape;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pawn"/> class.
        /// </summary>
        /// <param name="referenceHub">The <see cref="ReferenceHub"/> of the player to be encapsulated.</param>
        public Pawn(ReferenceHub referenceHub)
            : base(referenceHub)
        {
            foreach (KeyValuePair<Player, HashSet<CustomPlayerAbility>> kvp in CustomPlayerAbility.Manager)
            {
                if (kvp.Key != this)
                    continue;

                foreach (CustomPlayerAbility ability in kvp.Value)
                    AbilityManager.Add(ability, GetComponent(ability.BehaviourComponent).Cast<PlayerAbilityBehaviour>());
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pawn"/> class.
        /// </summary>
        /// <param name="gameObject">The <see cref="GameObject"/> of the player.</param>
        public Pawn(GameObject gameObject)
            : base(gameObject)
        {
            foreach (KeyValuePair<Player, HashSet<CustomPlayerAbility>> kvp in CustomPlayerAbility.Manager)
            {
                if (kvp.Key != this)
                    continue;

                foreach (CustomPlayerAbility ability in kvp.Value)
                    AbilityManager.Add(ability, GetComponent(ability.BehaviourComponent).Cast<PlayerAbilityBehaviour>());
            }
        }

        /// <summary>
        /// Gets the pawn's ability manager.
        /// </summary>
        public Dictionary<CustomPlayerAbility, PlayerAbilityBehaviour> AbilityManager { get; private set; } = new();

        /// <summary>
        /// Gets all pawn's <see cref="EPlayerBehaviour"/>'s.
        /// </summary>
        public IEnumerable<EPlayerBehaviour> Behaviours => ComponentsInChildren.Where(cmp => cmp is EPlayerBehaviour).Cast<EPlayerBehaviour>();

        /// <summary>
        /// Gets the pawn's <see cref="CustomRoles.CustomRole"/>.
        /// </summary>
        public CustomRole CustomRole => customRole ??= CustomRole.Get(this);

        /// <summary>
        /// Gets the pawn's <see cref="CustomRoles.CustomTeam"/>.
        /// </summary>
        public CustomTeam CustomTeam => customTeam ??= CustomTeam.Get(this);

        /// <summary>
        /// Gets the pawn's <see cref="CustomEscapes.CustomEscape"/>.
        /// </summary>
        public CustomEscape CustomEscape => customEscape ??= CustomEscape.Get(this);

        /// <summary>
        /// Gets the pawn's <see cref="CustomRoles.RoleBehaviour"/>.
        /// </summary>
        public RoleBehaviour RoleBehaviour => roleBehaviour ??= GetComponent<RoleBehaviour>();

        /// <summary>
        /// Gets the pawn's <see cref="CustomEscapes.EscapeBehaviour"/>.
        /// </summary>
        public EscapeBehaviour EscapeBehaviour => escapeBehaviour ??= GetComponent<EscapeBehaviour>();

        /// <summary>
        /// Gets the pawn's custom abilities.
        /// </summary>
        public IEnumerable<CustomAbility<Player>> CustomAbilities => AbilityManager.Keys;

        /// <summary>
        /// Gets the pawn's <see cref="PlayerAbilityBehaviour"/>.
        /// </summary>
        public IEnumerable<PlayerAbilityBehaviour> AbilityBehaviours => AbilityManager.Values;

        /// <summary>
        /// Gets a value indicating whether the pawn has a <see cref="CustomRoles.CustomRole"/>.
        /// </summary>
        public bool HasCustomRole => CustomRole.Players.Contains(this);

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
                if (customItem is CustomItem instance)
                    return CustomItem.TryGive(this, instance.Id);

                return false;
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
        /// Tries to get a <see cref="CustomItem"/> from the given <see cref="Item"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="CustomItem"/> to look for.</typeparam>
        /// <param name="customItem">The <see cref="CustomItem"/> result.</param>
        /// <returns><see langword="true"/> if pawn owns the specified <see cref="CustomItem"/>; otherwise, <see langword="false"/>.</returns>
        public bool TryGetCustomItem<T>(out T customItem)
            where T : CustomItem
        {
            customItem = null;
            foreach (Item item in Items)
            {
                if (!CustomItem.TryGet(item, out CustomItem tmp) || tmp is null || tmp.GetType() != typeof(T))
                    continue;

                customItem = (T)tmp;
            }

            return customItem is not null;
        }

        /// <summary>
        /// Gets the pawn's <see cref="CustomRoles.CustomRole"/>.
        /// </summary>
        /// <param name="customRole">The <see cref="CustomRoles.CustomRole"/> result.</param>
        /// <returns>The found <see cref="CustomRoles.CustomRole"/>, or <see langword="null"/> if not found.</returns>
        public bool TryGetCustomRole(out CustomRole customRole) => (customRole = CustomRole) is not null;

        /// <summary>
        /// Sets the pawn's role.
        /// </summary>
        /// <param name="role">The role to be set.</param>
        /// <param name="preservePlayerPosition">A value indicating whether the <see cref="Pawn"/> should be spawned in the same position.</param>
        public void SetRole(object role, bool preservePlayerPosition = false)
        {
            if (role is RoleTypeId id)
            {
                Role.Set(id);
                return;
            }

            CustomRole.Spawn(this, role, preservePlayerPosition);
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
    }
}