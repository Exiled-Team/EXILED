// -----------------------------------------------------------------------
// <copyright file="CustomRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Dissonance.Integrations.MirrorIgnorance;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.CustomItems;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs;
    using Exiled.Loader;

    using MEC;

    using UnityEngine;

    using YamlDotNet.Serialization;

    using Ragdoll = Ragdoll;

    /// <summary>
    /// The custom role base class.
    /// </summary>
    public abstract class CustomRole : MonoBehaviour
    {
        /// <summary>
        /// Gets a list of all registered custom roles.
        /// </summary>
        [YamlIgnore]
        public static List<CustomRole> RegisteredRoles { get; } = new List<CustomRole>();

        /// <summary>
        /// Gets the <see cref="Exiled.API.Features.Player"/> who is this role.
        /// </summary>
        [YamlIgnore]
        public Player Player { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="RoleType"/> to spawn this role as.
        /// </summary>
        public abstract RoleType Role { get; set; }

        /// <summary>
        /// Gets or sets the max <see cref="Exiled.API.Features.Player.Health"/> for the role.
        /// </summary>
        public abstract int MaxHealth { get; set; }

        /// <summary>
        /// Gets or sets the name of this role.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of this role.
        /// </summary>
        public abstract string Description { get; set; }

        /// <summary>
        /// Gets or sets the cooldown time of this roles abilities.
        /// </summary>
        public virtual int AbilityCooldown { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> they last used their ability.
        /// </summary>
        [YamlIgnore]
        public DateTime UsedAbility { get; set; }

        /// <summary>
        /// Gets or sets the starting inventory for the role.
        /// </summary>
        protected virtual List<string> Inventory { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the possible spawn locations for this role.
        /// </summary>
        protected virtual Dictionary<Vector3, float> SpawnLocations { get; set; } = new Dictionary<Vector3, float>();

        /// <summary>
        /// Gets or sets a value indicating whether players keep their current inventory when gaining this role.
        /// </summary>
        protected virtual bool KeepInventoryOnSpawn { get; set; }

        /// <summary>
        /// Gets or sets a list of the roles custom abilities.
        /// </summary>
        protected List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>();

        /// <summary>
        /// Determines if the role's ability can be used.
        /// </summary>
        /// <param name="usableTime">The <see cref="DateTime"/> the ability is usable at.</param>
        /// <returns>Whether the ability can be used or not.</returns>
        public virtual bool CanUseAbility(out DateTime usableTime)
        {
            usableTime = UsedAbility + TimeSpan.FromSeconds(AbilityCooldown);
            return DateTime.Now > usableTime;
        }

        /// <summary>
        /// Uses the roles custom ability.
        /// </summary>
        /// <returns>A string to send as a response to the user.</returns>
        public virtual string UseAbility() => "This role does not implement any abilities.";

        /// <summary>
        /// Called when the component is first added to the player.
        /// </summary>
        protected void Start()
        {
            Player = Player.Get(this.gameObject);
            if (Player == null)
            {
                Log.Error($"{Name}: Attempted to give this role to a player who doesn't exist.");
                Destroy(this);
            }

            foreach (CustomRole role in Player.GetCustomRoles())
            {
                if (role.Name != this.Name)
                    Destroy(role);
            }

            Timing.CallDelayed(0.25f, () =>
            {
                LoadEvents();
                AddRole();
            });
        }

        /// <summary>
        /// Removes the role from the player.
        /// </summary>
        protected void Remove() => Destroy(this);

        /// <summary>
        /// Tries to add an item to the player's inventory by name.
        /// </summary>
        /// <param name="itemName">The name of the item to try adding.</param>
        protected void TryAddItem(string itemName)
        {
            if (CustomItem.TryGet(itemName, out CustomItem customItem))
            {
                customItem.Give(Player);
            }
            else if (Enum.TryParse(itemName, out ItemType type))
            {
                if (type.IsAmmo())
                    Player.Ammo[type] = 100;
                else
                    Player.AddItem(type);
            }
            else
            {
                Log.Warn($"{Name}: {nameof(TryAddItem)}: {itemName} is not a valid ItemType or Custom Item name.");
            }
        }

        /// <summary>
        /// Gets a random <see cref="SpawnLocations"/>.
        /// </summary>
        /// <returns>The chosen spawn location.</returns>
        protected Vector3 GetSpawnPosition()
        {
            if (SpawnLocations.Count == 0)
                return Vector3.zero;

            foreach (KeyValuePair<Vector3, float> kvp in SpawnLocations)
            {
                if (Loader.Random.Next(100) <= kvp.Value)
                    return kvp.Key;
            }

            return SpawnLocations.ElementAt(Loader.Random.Next(SpawnLocations.Count)).Key;
        }

        /// <summary>
        /// Handles setup of the role, including spawn location, inventory and registering event handlers.
        /// </summary>
        protected virtual void AddRole()
        {
            Log.Debug($"{Name}: Adding role to {Player.Nickname}.", CustomRoles.Instance.Config.Debug);
            if (Role != RoleType.None)
            {
                Player.SetRole(Role, SpawnReason.ForceClass, true);
            }

            Timing.CallDelayed(1.5f, () =>
            {
                Vector3 pos = GetSpawnPosition();
                if (pos != Vector3.zero)
                    Player.Position = pos + (Vector3.up * 1.5f);

                if (!KeepInventoryOnSpawn)
                {
                    Log.Debug($"{Name}: Clearing {Player.Nickname}'s inventory.", CustomRoles.Instance.Config.Debug);
                    Player.ClearInventory();
                }

                foreach (string itemName in Inventory)
                {
                    Log.Debug($"{Name}: Adding {itemName} to inventory.", CustomRoles.Instance.Config.Debug);
                    TryAddItem(itemName);
                }

                Log.Debug($"{Name}: Setting health values.", CustomRoles.Instance.Config.Debug);
                Player.Health = MaxHealth;
                Player.MaxHealth = MaxHealth;
            });

            Log.Debug($"{Name}: Setting player info", CustomRoles.Instance.Config.Debug);
            Player.CustomInfo = $"{Name} (Custom Role)";
            ShowMessage();
            RoleAdded();
        }

        /// <summary>
        /// Called when removing a player's role.
        /// </summary>
        protected void OnDestroy()
        {
            Player.CustomInfo = string.Empty;
            UnloadEvents();
            RoleRemoved();
        }

        /// <summary>
        /// Called when the role is added to setup internal events.
        /// </summary>
        protected virtual void LoadEvents()
        {
            Log.Debug($"{Name}: Loading events for {Player.Nickname}.", CustomRoles.Instance.Config.Debug);
            Exiled.Events.Handlers.Player.ChangingRole += OnInternalChangingRole;
            Exiled.Events.Handlers.Player.Dying += OnInternalDying;
        }

        /// <summary>
        /// Called when the role is removed to unsubscribe internal event handlers.
        /// </summary>
        protected virtual void UnloadEvents()
        {
            Log.Debug($"{Name}: Unloading events for {Player?.Nickname}.", CustomRoles.Instance.Config.Debug);
            Exiled.Events.Handlers.Player.ChangingRole -= OnInternalChangingRole;
            Exiled.Events.Handlers.Player.Dying -= OnInternalDying;
        }

        /// <summary>
        /// Shows the spawn message to the player.
        /// </summary>
        protected virtual void ShowMessage() => Player.ShowHint($"You have spawned as a {Name}\n{Description}", 30f);

        /// <summary>
        /// Called after the role has been added to the player.
        /// </summary>
        protected virtual void RoleAdded()
        {
        }

        /// <summary>
        /// Called 1 frame before the role is removed from the player.
        /// </summary>
        protected virtual void RoleRemoved()
        {
        }

        private void OnInternalChangingRole(ChangingRoleEventArgs ev)
        {
            if (ev.Player == Player && ev.NewRole != Role)
                Remove();
        }

        private void OnInternalDying(DyingEventArgs ev)
        {
            if (ev.Target == Player)
            {
                CustomRoles.Instance.StopRagdollPlayers.Add(Player);
                Role role = CharacterClassManager._staticClasses.SafeGet(Role);
                Ragdoll.Info info = new Ragdoll.Info
                {
                    ClassColor = role.classColor,
                    DeathCause = ev.HitInformation,
                    FullName = Name,
                    Nick = Player.Nickname,
                    ownerHLAPI_id = Player.GameObject.GetComponent<MirrorIgnorancePlayer>().PlayerId,
                    PlayerId = Player.Id,
                };
                Exiled.API.Features.Ragdoll.Spawn(role, info, Player.Position, Quaternion.Euler(Player.Rotation));

                Remove();
            }
        }
    }
}
