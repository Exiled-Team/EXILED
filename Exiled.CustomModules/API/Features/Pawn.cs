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
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core.Behaviours;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Roles;
    using Exiled.CustomModules.API.Features.CustomAbilities;
    using Exiled.CustomModules.API.Features.CustomEscapes;
    using Exiled.CustomModules.API.Features.CustomItems;
    using Exiled.CustomModules.API.Features.CustomItems.Items;
    using Exiled.CustomModules.API.Features.CustomItems.Pickups.Ammos;
    using Exiled.CustomModules.API.Features.CustomRoles;
    using Exiled.CustomModules.API.Features.PlayerAbilities;
    using Exiled.CustomModules.Events.EventArgs.CustomAbilities;
    using PlayerRoles;
    using UnityEngine;

    /// <summary>
    /// Represents an in-game player by encapsulating a <see cref="ReferenceHub"/>, providing an extended feature set through the <see cref="Pawn"/> class.
    /// <para>
    /// The <see cref="Pawn"/> class enhances the functionality of the base <see cref="Player"/> class by providing additional features and capabilities related to <see cref="CustomModules"/>.
    /// <br>This class is designed to be used seamlessly alongside existing methods that expect a <see cref="Player"/> as a parameter, allowing for compatibility with the existing codebase.</br>
    /// <para>The use of nullable context is enabled to prevent users from inadvertently passing or interacting with <see langword="null"/> references.</para>
    /// </para>
    /// <remarks>
    /// Developers can leverage the enhanced functionality provided by the <see cref="Pawn"/> class while benefiting from the familiar interface of the <see cref="Player"/> class.
    /// <br>It serves as a comprehensive representation of an in-game entity, encapsulating the associated <see cref="ReferenceHub"/> with an expanded set of features.</br>
    /// </remarks>
    /// </summary>
    [DefaultPlayerClass(enforceAuthority: false)]
    public class Pawn : Player
    {
        private readonly List<ActiveAbilityBehaviour> abilityBehaviours = new();
        private readonly List<PlayerAbility> customAbilities = new();
        private readonly Dictionary<uint, ushort> customAmmoBox = new();

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
        /// <para/>
        /// Can be <see langword="null"/>.
        /// </summary>
        public CustomRole CustomRole => roleBehaviour.CustomRole;

        /// <summary>
        /// Gets the pawn's <see cref="CustomRoles.CustomTeam"/>.
        /// <para/>
        /// Can be <see langword="null"/>.
        /// </summary>
        public CustomTeam CustomTeam => roleBehaviour.CustomTeam;

        /// <summary>
        /// Gets the pawn's <see cref="CustomEscapes.CustomEscape"/>.
        /// <para/>
        /// Can be <see langword="null"/>.
        /// </summary>
        public CustomEscape CustomEscape => escapeBehaviour.CustomEscape;

        /// <summary>
        /// Gets the pawn's current <see cref="CustomItem"/>.
        /// <para/>
        /// Can be <see langword="null"/>.
        /// </summary>
        public CustomItem CurrentCustomItem => CustomItem.TryGet(CurrentItem, out CustomItem customItem) ? customItem : null;

        /// <summary>
        /// Gets the pawn's custom abilities.
        /// </summary>
        public IEnumerable<PlayerAbility> CustomAbilities => customAbilities;

        /// <summary>
        /// Gets the pawn's ability behaviours.
        /// </summary>
        public IEnumerable<ActiveAbilityBehaviour> AbilityBehaviours => abilityBehaviours;

        /// <summary>
        /// Gets the pawn's <see cref="CustomRoles.RoleBehaviour"/>.
        /// <para/>
        /// Can be <see langword="null"/>.
        /// </summary>
        public RoleBehaviour RoleBehaviour => roleBehaviour ??= GetComponent<RoleBehaviour>();

        /// <summary>
        /// Gets the pawn's <see cref="CustomEscapes.EscapeBehaviour"/>.
        /// <para/>
        /// Can be <see langword="null"/>.
        /// </summary>
        public EscapeBehaviour EscapeBehaviour => escapeBehaviour ??= GetComponent<EscapeBehaviour>();

        /// <summary>
        /// Gets the pawn's custom ammo box containing.
        /// </summary>
        public IReadOnlyDictionary<uint, ushort> CustomAmmoBox => customAmmoBox;

        /// <summary>
        /// Gets the selected <see cref="PlayerAbility"/>.
        /// </summary>
        public PlayerAbility SelectedAbility => SelectedAbilityBehaviour.CustomAbility.Cast<PlayerAbility>();

        /// <summary>
        /// Gets or sets the selected <see cref="AbilityBehaviourBase{T}"/>.
        /// </summary>
        public AbilityBehaviourBase<Player> SelectedAbilityBehaviour { get; set; }

        /// <summary>
        /// Gets a value indicating whether the pawn has a <see cref="CustomRoles.CustomRole"/>.
        /// </summary>
        public bool HasCustomRole => CustomRole;

        /// <summary>
        /// Gets a value indicating whether the pawn is any SCP, including custom ones.
        /// </summary>
        public new bool IsScp
        {
            get
            {
                if (IsCustomScp)
                    return true;

                return Role?.Team is Team.SCPs;
            }
        }

        /// <summary>
        /// Gets the <see cref="Role"/> or <see cref="CustomRoles.CustomRole"/> of the pawn.
        /// <para/>
        /// <returns><see cref="CustomRoles.CustomRole"/> if available; otherwise, the standard <see cref="Role"/></returns>
        /// </summary>
        public object GlobalRole => CustomRole ? CustomRole : Role;

        /// <summary>
        /// Gets the <see cref="Item"/>s and <see cref="CustomItem"/>s associated with the pawn.
        /// <para/>
        /// <returns>A combination of <see cref="Item"/>s and <see cref="CustomItem"/>s</returns>
        /// </summary>
        public IEnumerable<object> GlobalItems => Items.Cast<object>().Concat(CustomItems);

        /// <summary>
        /// Gets or sets the global current item of the pawn.
        /// <para/>
        /// If a <see cref="CustomItem"/> is equipped, it returns the <see cref="CustomItem"/>; otherwise, it returns the regular <see cref="Item"/>.
        /// </summary>
        public object GlobalCurrentItem
        {
            get => CurrentCustomItem ? CurrentCustomItem : CurrentItem;
            set
            {
                if (value is null)
                {
                    Inventory.ServerSelectItem(0);
                    return;
                }

                bool isCustomItem = value is CustomItem;
                if (isCustomItem)
                {
                    if (CustomItems.All(customItem => customItem.GetType() != value.GetType()))
                    {
                        if (IsInventoryFull)
                            return;

                        AddItem(value);
                    }

                    Item customItem = Items.LastOrDefault(i => i.TryGetComponent(out ItemBehaviour behaviour) && behaviour.GetType() == (value as CustomItem)?.BehaviourComponent);

                    if (customItem)
                        Inventory.ServerSelectItem(customItem.Serial);

                    return;
                }

                if (value is not Item item)
                    return;

                CurrentItem = item;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the pawn is a custom SCP.
        /// </summary>
        public bool IsCustomScp => CustomRole && CustomRole.IsScp;

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of all <see cref="CustomItem"/>s in the pawn's inventory.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of all <see cref="CustomItem"/>s in the pawn's inventory.</returns>
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
        /// Gets a <see cref="IEnumerable{T}"/> of all <see cref="EffectType"/>s.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of all <see cref="EffectType"/>s.</returns>
        public IEnumerable<EffectType> EffectTypes { get; } = EnumExtensions.QueryValues<EffectType>();

        /// <summary>
        /// Gets a value indicating whether the pawn has a <see cref="CustomItem"/> of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="CustomItem"/>.</typeparam>
        /// <returns><see langword="true"/> if a <see cref="CustomItem"/> of the specified type was found; otherwise, <see langword="false"/>.</returns>
        public bool HasCustomItem<T>()
            where T : CustomItem => CustomItems.Any(item => item.GetType() == typeof(T));

        /// <summary>
        /// Gets a value indicating whether the pawn has a <see cref="PlayerAbility"/> of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="PlayerAbility"/>.</typeparam>
        /// <returns><see langword="true"/> if a <see cref="PlayerAbility"/> of the specified type was found; otherwise, <see langword="false"/>.</returns>
        public bool HasCustomAbility<T>()
            where T : PlayerAbility => CustomAbilities.Any(ability => ability.GetType() == typeof(T));

        /// <summary>
        /// Tries to get the first <see cref="CustomItem"/> of the specified type from the pawn's inventory.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="CustomItem"/> to get.</typeparam>
        /// <param name="customItem">The output parameter that will contain the <see cref="CustomItem"/>, if found.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomItem"/> of the specified type was found; otherwise, <see langword="false"/>.</returns>
        public bool TryGetCustomItem<T>(out T customItem)
            where T : CustomItem => customItem = CustomItems.FirstOrDefault(item => item.GetType() == typeof(T))?.Cast<T>();

        /// <summary>
        /// Tries to get the <see cref="CustomRoles.CustomRole"/> of the specified type from the <see cref="Pawn"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="CustomRoles.CustomRole"/>.</typeparam>
        /// <param name="customRole">The output parameter that will contain the retrieved <see cref="CustomRoles.CustomRole"/>, if found.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomRoles.CustomRole"/> of the specified type was found; otherwise, <see langword="false"/>.</returns>
        public bool TryGetCustomRole<T>(out T customRole)
            where T : CustomRole => CustomRole.Is(out customRole);

        /// <summary>
        /// Tries to get the <see cref="CustomAbility{T}"/> of the specified type from the pawn's abilities.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="CustomAbility{T}"/> to get.</typeparam>
        /// <param name="customAbility">The output parameter that will contain the <see cref="CustomAbility{T}"/>, if found.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomAbility{T}"/> of the specified type was found; otherwise, <see langword="false"/>.</returns>
        public bool TryGetCustomAbility<T>(out T customAbility)
            where T : PlayerAbility => CustomAbilities.FirstOrDefault(ability => ability.GetType() == typeof(T)).Is(out customAbility);

        /// <summary>
        /// Add an <see cref="Item"/> or <see cref="CustomItem"/> of the specified type to the pawn's inventory.
        /// </summary>
        /// <param name="item">The item to add. Can be an <see cref="Item"/>, <see cref="ItemType"/>, <see cref="CustomItem"/>, as well as <see cref="CustomItem"/>'s <see cref="CustomItem.Name"/>, or <see cref="CustomItem.Id"/>.</param>
        /// <returns><see langword="true"/> if the item has been given to the pawn; otherwise, <see langword="false"/>.</returns>
        public bool AddItem(object item)
        {
            if (IsInventoryFull)
                return false;

            switch (item)
            {
                case Item baseItem:
                    AddItem(baseItem);
                    return true;
                case ItemType itemType:
                    AddItem(itemType);
                    return true;
                case string name:
                    CustomItem.TryGive(this, name);
                    return true;
                case CustomItem instance when CustomItem.TryGive(this, instance.Id):
                    return true;
                default:
                    try
                    {
                        uint value = (uint)item;
                        CustomItem.TryGive(this, value);
                        return true;
                    }
                    catch
                    {
                        throw new ArgumentException("Item is not of a supported type.");
                    }
            }
        }

        /// <summary>
        /// Adds a collection of <see cref="Item"/> and <see cref="CustomItem"/> to the pawn's inventory.
        /// </summary>
        /// <param name="customItems">The custom items to add.</param>
        /// <seealso cref="AddItem(object)"/>
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
        /// <param name="role">The role to set. Can be a <see cref="RoleTypeId"/> or a <see cref="uint"/> representing a custom role's <see cref="CustomRole.Id"/>.</param>
        /// <param name="preservePlayerPosition">A value indicating whether the <see cref="Pawn"/> should be spawned in the same position.</param>
        /// <param name="spawnReason">The <see cref="SpawnReason"/> to be set.</param>
        /// <param name="roleSpawnFlags">The <see cref="RoleSpawnFlags"/> to be set.</param>
        public void SetRole(object role, bool preservePlayerPosition = false, SpawnReason spawnReason = null, RoleSpawnFlags roleSpawnFlags = RoleSpawnFlags.All)
        {
            if (role is RoleTypeId roleType)
            {
                Role.Set(roleType, roleSpawnFlags is not RoleSpawnFlags.All ? roleSpawnFlags : preservePlayerPosition ? RoleSpawnFlags.AssignInventory : RoleSpawnFlags.All);
                return;
            }

            if (role is uint id)
                CustomRole.Spawn(this, id, preservePlayerPosition, spawnReason, roleSpawnFlags);

            try
            {
                uint uuId = (uint)role;
                CustomRole.Spawn(this, uuId, preservePlayerPosition, spawnReason, roleSpawnFlags);
            }
            catch
            {
                throw new ArgumentException("The type of the role instance is not compatible with RoleTypeId or uint.");
            }
        }

        /// <inheritdoc cref="Player.DropItem(Item)"/>
        public new void DropItem(Item item)
        {
            if (TryGetCustomItem(out CustomItem customItem))
            {
                RemoveItem(item, false);
                customItem?.Spawn(Position, this);
                return;
            }

            base.DropItem(item);
        }

        /// <summary>
        /// Gets the amount of a specified custom ammo in a pawn's ammo box.
        /// </summary>
        /// <param name="customAmmoType">The custom ammo to be searched for in the pawn's inventory.</param>
        /// <returns>The amount of the specified custom ammo in the pawn's inventory.</returns>
        public ushort GetAmmo(uint customAmmoType) => (ushort)(customAmmoBox.TryGetValue(customAmmoType, out ushort amount) ? amount : 0);

        /// <summary>
        /// Adds custom ammo to the pawn's ammo box.
        /// </summary>
        /// <param name="id">The type of the custom ammo.</param>
        /// <param name="amount">The amount to add.</param>
        /// <returns><see langword="true"/> if the specified amount of ammo was given entirely or partially; otherwise, <see langword="false"/>.</returns>
        public bool AddAmmo(uint id, ushort amount)
        {
            if (!CustomItem.TryGet(id, out CustomItem customItem) || !customItem.Settings.Is(out AmmoSettings settings))
                return false;

            if (customAmmoBox.TryAdd(id, amount))
                return true;

            if (customAmmoBox[id] >= settings.MaxUnits)
                return false;

            ushort amt;
            try
            {
                checked
                {
                    amt = (ushort)(customAmmoBox[id] + amount);
                }
            }
            catch (OverflowException)
            {
                amt = ushort.MaxValue;
            }

            if (amt >= settings.MaxUnits)
                amt = settings.MaxUnits;

            customAmmoBox[id] = amt;
            return true;
        }

        /// <summary>
        /// Removes custom ammo from the pawn's ammo box.
        /// </summary>
        /// <param name="id">The type of the custom ammo.</param>
        /// <param name="amount">The amount to remove.</param>
        /// <returns><see langword="true"/> if the specified amount of ammo was removed entirely or partially; otherwise, <see langword="false"/>.</returns>
        public bool RemoveAmmo(uint id, ushort amount)
        {
            if (!customAmmoBox.TryGetValue(id, out ushort amt))
                return false;

            try
            {
                checked
                {
                    amt -= amount;
                }
            }
            catch (OverflowException)
            {
                amt = ushort.MinValue;
            }

            customAmmoBox[id] = amt;
            return true;
        }

        /// <summary>
        /// Sets the amount of a specified custom ammo in the pawn's ammo box.
        /// </summary>
        /// <param name="id">The type of the custom ammo.</param>
        /// <param name="amount">The amount to set.</param>
        public void SetAmmo(uint id, ushort amount)
        {
            if (customAmmoBox.TryAdd(id, amount))
                return;

            customAmmoBox[id] = amount;
        }

        private void OnAddedAbility(AddedAbilityEventArgs<Player> ev)
        {
            abilityBehaviours.Add(GetComponent(ev.Ability.BehaviourComponent).Cast<ActiveAbilityBehaviour>());
            customAbilities.Add(ev.Ability.Cast<PlayerAbility>());
        }

        private void OnRemovingAbility(RemovingAbilityEventArgs<Player> ev)
        {
            abilityBehaviours.Remove(GetComponent(ev.Ability.BehaviourComponent).Cast<ActiveAbilityBehaviour>());
            customAbilities.Remove(ev.Ability.Cast<PlayerAbility>());
        }
    }
}