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

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs;

    using MEC;

    using UnityEngine;

    /// <summary>
    /// The base <see cref="CustomRole"/> component class.
    /// </summary>
    public abstract class CustomRole : EActor
    {
        private CustomRoleBlueprint blueprint;
        private bool forceRemove;

        /// <summary>
        /// Gets the owner of this <see cref="CustomRole"/> instance.
        /// </summary>
        public Player Owner { get; private set; }

        /// <summary>
        /// Gets the <see cref="CustomRoleBlueprint"/>.
        /// </summary>
        public CustomRoleBlueprint Blueprint => blueprint ??= CustomRoleBlueprint.Get(GetType());

        /// <summary>
        /// Checks if the given player has this role.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns>True if the player has this role.</returns>
        public virtual bool Check(Player player) => player is not null && Owner is null && player == Owner;

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            if (Blueprint is null)
                Destroy();

            Log.Debug($"{Name}: Adding role to {Owner.Nickname}.", CustomRoles.Instance.Config.Debug);
            if (blueprint.Role is not RoleType.None)
            {
                foreach (EActor component in Owner.Components)
                {
                    if (component.Cast(out CustomRole customRole) && customRole.blueprint.IsShared)
                        continue;

                    customRole.Destroy();
                }
            }

            Vector3 oldPos = Owner.Position;
            Timing.CallDelayed(1.5f, () =>
            {
                Vector3 pos = GetSpawnPosition();

                Log.Debug($"{nameof(OnBeginPlay)}: Found {pos} to spawn {Owner.Nickname}", CustomRoles.Instance.Config.Debug);

                // If the spawn pos isn't 0,0,0, We add vector3.up * 1.5 here to ensure they do not spawn inside the ground and get stuck.
                Owner.Position = oldPos;
                if (pos != Vector3.zero)
                {
                    Log.Debug($"{nameof(OnBeginPlay)}: Setting {Owner.Nickname} position..", CustomRoles.Instance.Config.Debug);
                    Owner.Position = pos + (Vector3.up * 1.5f);
                }

                if (!blueprint.KeepInventoryOnSpawn)
                {
                    Log.Debug($"{Name}: Clearing {Owner.Nickname}'s inventory.", CustomRoles.Instance.Config.Debug);
                    Owner.ClearInventory();
                }

                foreach (KeyValuePair<string, ushort> item in blueprint.Inventory)
                {
                    Log.Debug($"{Name}: Adding {item.Key} to inventory.", CustomRoles.Instance.Config.Debug);
                    TryAddItem(Owner, item.Key, item.Value);
                }

                Log.Debug($"{Name}: Setting health values.", CustomRoles.Instance.Config.Debug);
                Owner.MaxHealth = blueprint.MaxHealth;
                Owner.Health = blueprint.StartingHealth <= 0f ? Owner.MaxHealth : blueprint.StartingHealth;
                Owner.MaxArtificialHealth = blueprint.MaxArtificialHealth;
                Owner.ArtificialHealth = blueprint.StartingHealth;
                Owner.Scale = blueprint.Scale;
                Owner.EnableEffects(blueprint.GivenEffects);

                if (blueprint.FakeAppearance is not RoleType.None)
                    Owner.ChangeAppearance(blueprint.FakeAppearance);
            });

            Log.Debug($"{Name}: Setting player info", CustomRoles.Instance.Config.Debug);

            if (blueprint.IgnoreScp173)
                Scp173.TurnedPlayers.Add(Owner);

            Owner.CustomInfo = blueprint.CustomInfo;
            if (blueprint.HideInfoArea)
            {
                Owner.InfoArea &= ~PlayerInfoArea.Role;
                Owner.InfoArea &= ~PlayerInfoArea.UnitName;
            }

            if (blueprint.CustomAbilities is not null)
            {
                foreach (CustomAbility ability in blueprint.CustomAbilities)
                    ability.AddAbility(Owner);
            }

            if (blueprint.DisplayBroadcast is not null)
                Owner.Broadcast(blueprint.DisplayBroadcast);

            ShowMessage();
        }

        /// <inheritdoc/>
        protected override void OnBeginDestroy()
        {
            base.OnBeginDestroy();

            if (blueprint is null)
                return;

            Log.Debug($"{Name}: Removing role from {Owner.Nickname}", CustomRoles.Instance.Config.Debug);
            Scp173.TurnedPlayers.Remove(Owner);
            Owner.Scale = Vector3.one;
            Owner.CustomInfo = string.Empty;
            if (blueprint.HideInfoArea)
            {
                Owner.InfoArea |= PlayerInfoArea.Role;
                Owner.InfoArea |= PlayerInfoArea.UnitName;
            }

            if (blueprint.KillOwnerOnDestroy)
            {
                forceRemove = true;
                Owner.SetRole(RoleType.Spectator);
            }

            foreach (CustomAbility ability in blueprint.CustomAbilities)
                ability.RemoveAbility(Owner);

            Owner.DisableEffects(blueprint.GivenEffects);
        }

        /// <summary>
        /// Tries to add an item to the player's inventory by name.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to try giving the item to.</param>
        /// <param name="itemName">The name of the item to try adding.</param>
        /// <param name="amount">The amount of items to add.</param>
        /// <returns>Whether or not the item was able to be added.</returns>
        protected bool TryAddItem(Player player, string itemName, ushort amount)
        {
            if (CustomItem.TryGet(itemName, out CustomItem customItem))
            {
                for (int i = 0; i < amount; i++)
                {
                    if (player.IsInventoryFull)
                        break;

                    customItem.Give(player);
                }

                return true;
            }

            if (Enum.TryParse(itemName, out ItemType type))
            {
                if (type.IsAmmo())
                    player.Ammo[type] = amount;
                else
                    player.AddItem(type, amount);

                return true;
            }

            Log.Warn($"{Name}: {nameof(TryAddItem)}: {itemName} is not a valid ItemType or Custom Item name.");

            return false;
        }

        /// <summary>
        /// Gets a random <see cref="Vector3"/> from <see cref="SpawnProperties"/>.
        /// </summary>
        /// <returns>The chosen spawn location.</returns>
        protected Vector3 GetSpawnPosition()
        {
            if (blueprint.SpawnProperties is null || blueprint.SpawnProperties.Count() == 0)
                return Vector3.zero;

            if (blueprint.SpawnProperties.StaticSpawnPoints.Count > 0)
            {
                foreach ((float chance, Vector3 pos) in blueprint.SpawnProperties.StaticSpawnPoints)
                {
                    if (UnityEngine.Random.Range(0, 101f) <= chance)
                        return pos;
                }
            }

            if (blueprint.SpawnProperties.DynamicSpawnPoints.Count > 0)
            {
                foreach ((float chance, Vector3 pos) in blueprint.SpawnProperties.DynamicSpawnPoints)
                {
                    if (UnityEngine.Random.Range(0, 101f) <= chance)
                        return pos;
                }
            }

            if (blueprint.SpawnProperties.RoleSpawnPoints.Count > 0)
            {
                foreach ((float chance, Vector3 pos) in blueprint.SpawnProperties.RoleSpawnPoints)
                {
                    if (UnityEngine.Random.Range(0, 101f) <= chance)
                        return pos;
                }
            }

            return Vector3.zero;
        }

        /// <summary>
        /// Called when the role is initialized to setup internal events.
        /// </summary>
        protected virtual void SubscribeEvents()
        {
            Log.Debug($"{Name}: Loading events.", CustomRoles.Instance.Config.Debug);
            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole_Internal;
            Exiled.Events.Handlers.Player.Dying += OnDying_Internal;
            Exiled.Events.Handlers.Player.Hurting += OnBeingHurted_Internal;
            Exiled.Events.Handlers.Player.Hurting += OnHurting_Internal;
            Exiled.Events.Handlers.Player.Escaping += OnEscaping_Internal;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor_Internal;
            Exiled.Events.Handlers.Scp096.AddingTarget += OnAddingTarget_Internal;
        }

        /// <summary>
        /// Called when the role is destroyed to unsubscribe internal event handlers.
        /// </summary>
        protected virtual void UnsubscribeEvents()
        {
            Log.Debug($"{Name}: Unloading events.", CustomRoles.Instance.Config.Debug);
            Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole_Internal;
            Exiled.Events.Handlers.Player.Dying -= OnDying_Internal;
            Exiled.Events.Handlers.Player.Hurting -= OnBeingHurted_Internal;
            Exiled.Events.Handlers.Player.Hurting -= OnHurting_Internal;
            Exiled.Events.Handlers.Player.Escaping -= OnEscaping_Internal;
            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor_Internal;
            Exiled.Events.Handlers.Scp096.AddingTarget -= OnAddingTarget_Internal;
        }

        /// <summary>
        /// Shows the spawn message to the player.
        /// </summary>
        protected virtual void ShowMessage() => Owner.ShowHint(string.Format(CustomRoles.Instance.Config.GotRoleHint.Content, Name, blueprint.Description), CustomRoles.Instance.Config.GotRoleHint.Duration);

        private void OnBeingHurted_Internal(HurtingEventArgs ev)
        {
            if (!Check(ev.Target))
                return;

            if (ev.Attacker is null)
            {
                if ((!blueprint.AllowedDamageTypes.IsEmpty() && !blueprint.AllowedDamageTypes.Contains(ev.Handler.Type)) ||
                    (!blueprint.IgnoredDamageTypes.IsEmpty() && blueprint.IgnoredDamageTypes.Contains(ev.Handler.Type)))
                    ev.IsAllowed = false;

                return;
            }

            if ((!blueprint.DamageableByTeams.IsEmpty() && !blueprint.DamageableByTeams.Contains(ev.Attacker.Role.Team)) ||
                (!blueprint.NotDamageableByTeams.IsEmpty() && blueprint.NotDamageableByTeams.Contains(ev.Attacker.Role.Team)) ||
                (!blueprint.DamageableByRoles.IsEmpty() && !blueprint.DamageableByRoles.Contains(ev.Attacker.Role.Type)) ||
                (!blueprint.NotDamageableByRoles.IsEmpty() && blueprint.NotDamageableByRoles.Contains(ev.Attacker.Role.Type)))
            {
                ev.IsAllowed = false;
                return;
            }
        }

        private void OnHurting_Internal(HurtingEventArgs ev)
        {
            if (!Check(ev.Attacker))
                return;

            if ((!blueprint.DamageableTeams.IsEmpty() && !blueprint.DamageableTeams.Contains(ev.Attacker.Role.Team)) ||
                (!blueprint.NotDamageableTeams.IsEmpty() && blueprint.NotDamageableTeams.Contains(ev.Attacker.Role.Team)) ||
                (!blueprint.DamageableRoles.IsEmpty() && !blueprint.DamageableRoles.Contains(ev.Attacker.Role.Type)) ||
                (!blueprint.NotDamageableRoles.IsEmpty() && blueprint.NotDamageableRoles.Contains(ev.Attacker.Role.Type)))
            {
                ev.IsAllowed = false;
                return;
            }
        }

        private void OnChangingRole_Internal(ChangingRoleEventArgs ev)
        {
            if (!Check(ev.Player))
                return;

            if (!blueprint.AllowedRoles.Contains(ev.NewRole) || forceRemove)
            {
                Destroy();
                return;
            }

            if (blueprint.FakeAppearance is not RoleType.None && blueprint.KeepFakeAppearanceOnChangingRole)
                Timing.CallDelayed(0.25f, () => ev.Player.ChangeAppearance(blueprint.FakeAppearance));
        }

        private void OnEscaping_Internal(EscapingEventArgs ev)
        {
            if (!Check(ev.Player))
                return;

            if (!blueprint.CanEscape)
            {
                ev.IsAllowed = false;
                return;
            }

            if (blueprint.OverrideDefaultEscapeRole is not RoleType.None)
                ev.NewRole = blueprint.OverrideDefaultEscapeRole;
        }

        private void OnInteractingDoor_Internal(InteractingDoorEventArgs ev)
        {
            if (Check(ev.Player) && !ev.IsAllowed && blueprint.BypassableDoors.Contains(ev.Door.Type))
                ev.IsAllowed = true;
        }

        private void OnAddingTarget_Internal(AddingTargetEventArgs ev)
        {
            if (!Check(ev.Target) && blueprint.IgnoreScp096)
                ev.IsAllowed = false;
        }

        private void OnDying_Internal(DyingEventArgs ev)
        {
            if (Check(ev.Target))
            {
                CustomRoles.Instance.StopRagdollPlayers.Add(ev.Target);

                // TODO: This
                /*
                Role role = CharacterClassManager._staticClasses.SafeGet(Role);

                Ragdoll.Info info = new Ragdoll.Info
                {
                    ClassColor = role.classColor,
                    DeathCause = ev.HitInformation,
                    FullName = Name,
                    Nick = ev.Target.Nickname,
                    ownerHLAPI_id = ev.Target.GameObject.GetComponent<MirrorIgnorancePlayer>().PlayerId,
                    PlayerId = ev.Target.Id,
                };
                Exiled.API.Features.Ragdoll.Spawn(role, info, ev.Target.Position, Quaternion.Euler(ev.Target.Rotation), default, false, false);
                */
            }
        }
    }
}
