// -----------------------------------------------------------------------
// <copyright file="RoleBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomRoles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Generic;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.API.Features.DynamicEvents;
    using Exiled.API.Features.Pools;
    using Exiled.API.Features.Roles;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomModules.API.Enums;
    using Exiled.CustomModules.API.Features.CustomEscapes;
    using Exiled.CustomModules.API.Features.Inventory;
    using Exiled.CustomModules.Events.EventArgs.CustomEscapes;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.EventArgs.Player;

    using PlayerRoles;

    using UnityEngine;

    using static Exiled.API.Extensions.MirrorExtensions;

    /// <summary>
    /// A tool to easily handle the custom role's logic.
    /// </summary>
    public abstract class RoleBehaviour : EPlayerBehaviour, IAdditiveSettings<RoleSettings>
    {
        private Vector3 lastPosition;
        private RoleTypeId fakeAppearance;
        private bool isHuman;
        private bool wasNoClipPermitted;
        private bool useCustomEscape;
        private bool wasEscaped;

        /// <summary>
        /// Gets a <see cref="HashSet{T}"/> of <see cref="Player"/> containing all players to be spawned without affecting their current position (static).
        /// </summary>
        public static List<Player> StaticPlayers { get; } = new();

        /// <summary>
        /// Gets or sets the <see cref="RoleSettings"/>.
        /// </summary>
        public virtual RoleSettings Settings { get; set; }

        /// <summary>
        /// Gets a random spawn point based on existing settings.
        /// </summary>
        public Vector3 SpawnPoint
        {
            get
            {
                if (Settings.SpawnProperties is null || Settings.SpawnProperties.IsEmpty)
                    return RoleExtensions.GetRandomSpawnLocation(Role).Position;

                static bool EvalSpawnPoint(IEnumerable<SpawnPoint> spawnpoints, out Vector3 outPos)
                {
                    outPos = default;

                    foreach ((float chance, Vector3 pos) in spawnpoints)
                    {
                        if (UnityEngine.Random.Range(0f, 101f) <= chance)
                        {
                            outPos = pos;
                            return true;
                        }
                    }

                    return false;
                }

                if (Settings.SpawnProperties.StaticSpawnPoints.Count > 0 && EvalSpawnPoint(Settings.SpawnProperties.StaticSpawnPoints, out Vector3 staticPos))
                    return staticPos;
                else if (Settings.SpawnProperties.DynamicSpawnPoints.Count > 0 && EvalSpawnPoint(Settings.SpawnProperties.DynamicSpawnPoints, out Vector3 dynamicPos))
                    return dynamicPos;
                else if (Settings.SpawnProperties.RoleSpawnPoints.Count > 0 && EvalSpawnPoint(Settings.SpawnProperties.RoleSpawnPoints, out Vector3 rolePos))
                    return rolePos;

                return Vector3.zero;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="RoleTypeId"/> of the fake appearance applied by this <see cref="RoleBehaviour"/> component.
        /// </summary>
        protected virtual RoleTypeId FakeAppearance
        {
            get => fakeAppearance;
            set
            {
                fakeAppearance = value;
                Owner.ChangeAppearance(value, false, 0);
            }
        }

        /// <summary>
        /// Gets the <see cref="InventoryManager"/>.
        /// </summary>
        protected virtual InventoryManager Inventory { get; }

        /// <summary>
        /// Gets or sets a <see cref="IEnumerable{T}"/> of <see cref="EffectType"/> which should be permanently given to the player.
        /// </summary>
        protected virtual IEnumerable<EffectType> PermanentEffects { get; set; }

        /// <summary>
        /// Gets a value indicating whether <see cref="FakeAppearance"/> should be used.
        /// </summary>
        protected virtual bool UseFakeAppearance { get; }

        /// <summary>
        /// Gets a value indicating whether an existing spawnpoint should be used.
        /// </summary>
        protected virtual bool UseCustomSpawnpoint => true;

        /// <summary>
        /// Gets or sets a value indicating whether the effects should persistently remain active.
        /// </summary>
        protected virtual bool SustainEffects { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="RoleTypeId"/> of this <see cref="RoleBehaviour"/> component.
        /// </summary>
        protected RoleTypeId Role { get; set; }

        /// <summary>
        /// Gets the relative <see cref="CustomRoles.CustomRole"/>.
        /// </summary>
        protected CustomRole CustomRole { get; private set; }

        /// <summary>
        /// Gets the current speed of the  <see cref="EBehaviour{T}.Owner"/>.
        /// </summary>
        protected float CurrentSpeed { get; private set; }

        /// <summary>
        /// Gets the role's configs.
        /// </summary>
        protected virtual object ConfigRaw { get; private set; }

        /// <summary>
        /// Gets or sets the the escape settings.
        /// </summary>
        protected virtual List<EscapeSettings> EscapeSettings { get; set; } = new();

        /// <summary>
        /// Gets the <see cref="TDynamicEventDispatcher{T}"/> handling all bound delegates to be fired before escaping.
        /// </summary>
        protected TDynamicEventDispatcher<Events.EventArgs.CustomEscapes.EscapingEventArgs> EscapingEventDispatcher { get; private set; }

        /// <summary>
        /// Gets the <see cref="TDynamicEventDispatcher{T}"/> handling all bound delegates to be fired after escaping.
        /// </summary>
        protected TDynamicEventDispatcher<Player> EscapedEventDispatcher { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the specified <see cref="DamageType"/> is allowed.
        /// </summary>
        /// <param name="damageType">The <see cref="DamageType"/> to check.</param>
        /// <returns><see langword="true"/> if the specified <see cref="DamageType"/> is allowed; otherwise, <see langword="false"/>.</returns>
        public bool IsDamageAllowed(DamageType damageType) => Settings.AllowedDamageTypes.Contains(damageType);

        /// <summary>
        /// Gets a value indicating whether the specified <see cref="DamageType"/> is ignored.
        /// </summary>
        /// <param name="damageType">The <see cref="DamageType"/> to check.</param>
        /// <returns><see langword="true"/> if the specified <see cref="DamageType"/> is ignored; otherwise, <see langword="false"/>.</returns>
        public bool IsDamageIgnored(DamageType damageType) => Settings.IgnoredDamageTypes.Contains(damageType);

        /// <inheritdoc/>
        public abstract void AdjustAddittivePipe();

        /// <summary>
        /// Loads the given config.
        /// </summary>
        /// <param name="config">The config load.</param>
        protected virtual void LoadConfigs(object config)
        {
            if (config is null)
                return;

            foreach (PropertyInfo propertyInfo in config.GetType().GetProperties())
            {
                PropertyInfo targetInfo = typeof(RoleSettings).GetProperty(propertyInfo.Name) ?? typeof(InventoryManager).GetProperty(propertyInfo.Name);
                if (targetInfo is null)
                    continue;

                targetInfo.SetValue(targetInfo.DeclaringType == typeof(RoleSettings) ? Settings : Inventory, propertyInfo.GetValue(config, null));
            }
        }

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            LoadConfigs(ConfigRaw);

            if (CustomRole.TryGet(this, out CustomRole customRole))
            {
                CustomRole = customRole;
                Settings = CustomRole.Settings;

                Owner.UniqueRole = CustomRole.Name;
                Owner.TryAddCustomRoleFriendlyFire(Name, Settings.FriendlyFireMultiplier);

                if (CustomRole.EscapeBehaviourComponent is not null)
                {
                    Owner.AddComponent(CustomRole.EscapeBehaviourComponent);
                    useCustomEscape = true;
                }
            }

            AdjustAddittivePipe();

            wasNoClipPermitted = Owner.IsNoclipPermitted;
            isHuman = !CustomRole.IsScp;

            if (!Settings.IsRoleDynamic)
            {
                Role = CustomRole.Role;

                if (Role is RoleTypeId.None)
                {
                    Destroy();
                    return;
                }
            }

            if (Owner.Role != Role)
            {
                switch (Settings.PreservePosition)
                {
                    case true when Settings.PreserveInventory:
                        Owner.Role.Set(Role, SpawnReason.ForceClass, RoleSpawnFlags.None);
                        break;
                    case true:
                        Owner.Role.Set(Role, SpawnReason.ForceClass, RoleSpawnFlags.AssignInventory);
                        break;
                    default:
                    {
                        if (Settings.PreserveInventory && Owner.IsAlive)
                            Owner.Role.Set(Role, SpawnReason.ForceClass, RoleSpawnFlags.UseSpawnpoint);
                        else
                            Owner.Role.Set(Role, SpawnReason.ForceClass, RoleSpawnFlags.All);
                        break;
                    }
                }
            }

            Owner.Scale *= Settings.Scale;
            Owner.Health = Settings.Health;
            Owner.MaxHealth = Settings.MaxHealth;
            Owner.MaxArtificialHealth = Settings.MaxArtificialHealth;
            Owner.CustomInfo = $"{Owner.CustomName}\n{Settings.CustomInfo}";

            if (!Settings.DoesLookingAffectScp173)
                Scp173Role.TurnedPlayers.Add(Owner);

            if (!Settings.DoesLookingAffectScp096)
                Scp096Role.TurnedPlayers.Add(Owner);

            if (Settings.CustomInfo != string.Empty)
                Owner.CustomInfo += Settings.CustomInfo;

            if (Settings.HideInfoArea)
            {
                Owner.InfoArea &= ~PlayerInfoArea.UnitName;
                Owner.InfoArea &= ~PlayerInfoArea.Role;
            }

            if (isHuman && !Settings.PreserveInventory)
            {
                Owner.ClearInventory();

                if (Inventory.Items is not null)
                    Owner.AddItem(Inventory.Items);

                if (Inventory.CustomItems is not null)
                    Owner.Cast<Pawn>().AddItem(Inventory.CustomItems);

                if (Inventory.AmmoBox is not null)
                {
                    foreach (KeyValuePair<AmmoType, ushort> kvp in Inventory.AmmoBox)
                        Owner.AddAmmo(kvp.Key, kvp.Value);
                }
            }

            if (Settings.InitialBroadcast is not null)
                Owner.Broadcast(Settings.InitialBroadcast, true);

            if (!string.IsNullOrEmpty(Settings.ConsoleMessage.Replace("{role}", CustomRole.Name)))
            {
                StringBuilder builder = StringBuilderPool.Pool.Get();

                builder.AppendLine(CustomRole.Name);
                builder.AppendLine(CustomRole.Description);
                builder.AppendLine();
                builder.AppendLine(Settings.ConsoleMessage);

                Owner.SendConsoleMessage(StringBuilderPool.Pool.ToStringReturn(builder), "green");
            }
        }

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            if (!Owner)
            {
                Destroy();
                return;
            }

            if (Owner.Role.Cast(out FpcRole fpcRole))
                fpcRole.IsNoclipEnabled = false;

            if (Settings.ArtificialHealth > 0f)
                Owner.AddAhp(Settings.ArtificialHealth, Owner.MaxArtificialHealth, 0, 1, 0);

            if (UseFakeAppearance)
                Owner.ChangeAppearance(FakeAppearance, false);

            if (PermanentEffects is not null)
                Owner.EnableEffects(PermanentEffects);
        }

        /// <inheritdoc/>
        protected override void Tick()
        {
            base.Tick();

            // Must be refactored (performance issues)
            if ((Settings.UseDefaultRoleOnly && (Owner.Role != Role)) || (!Settings.DynamicRoles.IsEmpty() && !Settings.DynamicRoles.Contains(Owner.Role)))
            {
                Destroy();
                return;
            }

            if (SustainEffects)
            {
                foreach (EffectType effect in PermanentEffects)
                {
                    if (!Owner.GetEffect(effect).IsEnabled)
                        Owner.EnableEffect(effect);
                }
            }

            if (!useCustomEscape && !wasEscaped)
            {
                foreach (EscapeSettings settings in EscapeSettings)
                {
                    if (!settings.IsAllowed || Vector3.Distance(Owner.Position, settings.Position) > settings.DistanceThreshold)
                        continue;

                    Events.EventArgs.CustomEscapes.EscapingEventArgs ev = new(Owner.Cast<Pawn>(), settings.Role, settings.CustomRole, UUEscapeScenarioType.None, default);
                    EscapingEventDispatcher.InvokeAll(ev);

                    if (!ev.IsAllowed)
                        continue;

                    ev.Player.SetRole(ev.NewRole != RoleTypeId.None ? ev.NewRole : ev.NewCustomRole);
                    ev.Player.ShowHint(ev.Hint);

                    EscapedEventDispatcher.InvokeAll(ev.Player);
                    wasEscaped = true;

                    break;
                }
            }

            CurrentSpeed = (Owner.Position - lastPosition).magnitude;
            lastPosition = Owner.Position;
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            CustomRole.PlayersValue.Remove(Owner.Cast<Pawn>());

            if (!Settings.DoesLookingAffectScp173)
                Scp173Role.TurnedPlayers.Remove(Owner);

            if (!Settings.DoesLookingAffectScp096)
                Scp096Role.TurnedPlayers.Remove(Owner);

            if (!string.IsNullOrEmpty(Settings.CustomInfo))
                Owner.CustomInfo = string.Empty;

            if (Settings.HideInfoArea)
            {
                Owner.InfoArea |= PlayerInfoArea.UnitName;
                Owner.InfoArea |= PlayerInfoArea.Role;
            }

            Owner.Scale = Vector3.one;
            Owner.IsNoclipPermitted = wasNoClipPermitted;
            Owner.IsUsingStamina = true;
            Owner.DisableAllEffects();
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            EscapingEventDispatcher.Bind(this, OnEscaping);
            EscapedEventDispatcher.Bind(this, OnEscaped);

            Exiled.Events.Handlers.Player.ChangingItem += ChangingItemBehaviour;
            Exiled.Events.Handlers.Player.Destroying += DestroyOnLeave;
            Exiled.Events.Handlers.Player.ChangingRole += DestroyOnChangingRole;
            Exiled.Events.Handlers.Player.Escaping += PreventPlayerFromEscaping;
            Exiled.Events.Handlers.Player.SearchingPickup += PickingUpItemBehavior;
            Exiled.Events.Handlers.Player.Died += AnnounceOwnerDeath;
            Exiled.Events.Handlers.Player.Hurting += PreventDealingDamageToScps;
            Exiled.Events.Handlers.Player.Hurting += PreventTakingDamageFromScps;
            Exiled.Events.Handlers.Player.Hurting += IgnoreDamage;
            Exiled.Events.Handlers.Player.InteractingDoor += CheckpointsBehavior;
            Exiled.Events.Handlers.Player.InteractingDoor += InteractingDoorBehavior;
            Exiled.Events.Handlers.Player.IntercomSpeaking += IntercomSpeakingBehavior;
            Exiled.Events.Handlers.Player.VoiceChatting += VoiceChattingBehavior;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel += ActivatingWarheadBehavior;
            Exiled.Events.Handlers.Player.ActivatingWorkstation += ActivatingWorkstationBehavior;
            Exiled.Events.Handlers.Player.ActivatingGenerator += ActivatingGeneratorBehavior;
            Exiled.Events.Handlers.Player.InteractingElevator += InteractingElevatorBehavior;
            Exiled.Events.Handlers.Player.DroppingItem += DroppingItemBehavior;
            Exiled.Events.Handlers.Player.Handcuffing += HandcuffingBehavior;
            Exiled.Events.Handlers.Map.PlacingBlood += PlacingBloodBehavior;
            Exiled.Events.Handlers.Player.ChangingNickname += OnInternalChangingNickname;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Player.ChangingItem -= ChangingItemBehaviour;
            Exiled.Events.Handlers.Player.Destroying -= DestroyOnLeave;
            Exiled.Events.Handlers.Player.ChangingRole -= DestroyOnChangingRole;
            Exiled.Events.Handlers.Player.Escaping -= PreventPlayerFromEscaping;
            Exiled.Events.Handlers.Player.SearchingPickup -= PickingUpItemBehavior;
            Exiled.Events.Handlers.Player.Died -= AnnounceOwnerDeath;
            Exiled.Events.Handlers.Player.Hurting -= PreventDealingDamageToScps;
            Exiled.Events.Handlers.Player.Hurting -= PreventTakingDamageFromScps;
            Exiled.Events.Handlers.Player.Hurting -= IgnoreDamage;
            Exiled.Events.Handlers.Player.InteractingDoor -= CheckpointsBehavior;
            Exiled.Events.Handlers.Player.InteractingDoor -= InteractingDoorBehavior;
            Exiled.Events.Handlers.Player.IntercomSpeaking -= IntercomSpeakingBehavior;
            Exiled.Events.Handlers.Player.VoiceChatting -= VoiceChattingBehavior;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel -= ActivatingWarheadBehavior;
            Exiled.Events.Handlers.Player.ActivatingWorkstation -= ActivatingWorkstationBehavior;
            Exiled.Events.Handlers.Player.ActivatingGenerator -= ActivatingGeneratorBehavior;
            Exiled.Events.Handlers.Player.InteractingElevator -= InteractingElevatorBehavior;
            Exiled.Events.Handlers.Player.DroppingItem -= DroppingItemBehavior;
            Exiled.Events.Handlers.Player.Handcuffing -= HandcuffingBehavior;
            Exiled.Events.Handlers.Map.PlacingBlood -= PlacingBloodBehavior;
            Exiled.Events.Handlers.Player.ChangingNickname -= OnInternalChangingNickname;
        }

        /// <summary>
        /// Fired before the player escapes.
        /// </summary>
        /// <param name="ev">The <see cref="Events.EventArgs.CustomEscapes.EscapingEventArgs"/> instance.</param>
        protected virtual void OnEscaping(Events.EventArgs.CustomEscapes.EscapingEventArgs ev)
        {
        }

        /// <summary>
        /// Fired after the player escapes.
        /// </summary>
        /// <param name="player">The player who escaped.</param>
        protected virtual void OnEscaped(Player player)
        {
        }

        /// <summary>
        /// Tries to get a valid target based on a specified condition.
        /// </summary>
        /// <param name="predicate">The condition.</param>
        /// <param name="distance">The maximum distance to reach.</param>
        /// <param name="target">The valid target.</param>
        /// <returns><see langword="true"/> if the target was found; otherwise, <see langword="false"/>.</returns>
        protected virtual bool TryGetValidTarget(Func<Player, bool> predicate, float distance, out Player target)
        {
            List<Player> targets = new();
            foreach (Player pl in Player.Get(predicate))
            {
                if (Vector3.Distance(pl.Position, Owner.Position) <= distance)
                    targets.Add(pl);
            }

            target = targets.FirstOrDefault();
            return target is not null;
        }

        /// <summary>
        /// Tries to get a valid target based on a specified condition.
        /// </summary>
        /// <param name="predicate">The condition.</param>
        /// <param name="distance">The maximum distance to reach.</param>
        /// <param name="players">The valid targets.</param>
        /// <returns><see langword="true"/> if targets were found; otherwise, <see langword="false"/>.</returns>
        protected virtual bool TryGetValidTargets(Func<Player, bool> predicate, float distance, out List<Player> players)
        {
            players = new();
            foreach (Player pl in Player.Get(predicate))
            {
                if (Vector3.Distance(pl.Position, Owner.Position) <= distance)
                    players.Add(pl);
            }

            return players.Any();
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnHurting(HurtingEventArgs)"/>
        protected virtual void PreventTakingDamageFromScps(HurtingEventArgs ev)
        {
            if (!Check(ev.Player) || ev.Attacker is null ||
                !ev.Attacker.IsScp || Settings.CanBeHurtByScps)
                return;

            ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnHurting(HurtingEventArgs)"/>
        protected virtual void IgnoreDamage(HurtingEventArgs ev)
        {
            if (!Check(ev.Player) || !Settings.AllowedDamageTypes.IsEmpty() ||
                !Settings.IgnoredDamageTypes.Contains(ev.DamageHandler.Type))
                return;

            ev.Amount = 0;
            ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnChangingItem(ChangingItemEventArgs)"/>
        protected virtual void ChangingItemBehaviour(ChangingItemEventArgs ev)
        {
            if (!Check(ev.Player) || Settings.CanSelectItems)
                return;

            ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnHurting(HurtingEventArgs)"/>
        protected virtual void AllowDamage(HurtingEventArgs ev)
        {
            if (!Check(ev.Player) || !Settings.IgnoredDamageTypes.IsEmpty() ||
                !Settings.AllowedDamageTypes.Contains(ev.DamageHandler.Type))
                return;

            ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnHurting(HurtingEventArgs)"/>
        protected virtual void PreventDealingDamageToScps(HurtingEventArgs ev)
        {
            if (!Check(ev.Attacker) || Settings.CanHurtScps)
                return;

            ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnDestroying(DestroyingEventArgs)"/>
        protected virtual void DestroyOnLeave(DestroyingEventArgs ev)
        {
            if (!Check(ev.Player))
                return;

            Destroy();
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnHandcuffing(HandcuffingEventArgs)"/>
        protected virtual void HandcuffingBehavior(HandcuffingEventArgs ev)
        {
            if (!Check(ev.Target) || Settings.CanBeHandcuffed)
                return;

            ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnChangingGroup(ChangingGroupEventArgs)"/>
        protected virtual void DestroyOnChangingRole(ChangingRoleEventArgs ev)
        {
            if (!Check(ev.Player) || (Settings.UniqueRole is not RoleTypeId.None && ev.NewRole == Settings.UniqueRole) ||
                (Settings.IsRoleDynamic && Settings.DynamicRoles.Contains(ev.NewRole)))
                return;

            Destroy();
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnSpawning(SpawningEventArgs)"/>
        protected virtual void OverrideSpawnPoint(SpawningEventArgs ev)
        {
            if (!Check(ev.Player) || !UseCustomSpawnpoint)
                return;

            ev.Position = SpawnPoint + (Vector3.up * 1.5f);
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnSearchPickupRequest(SearchingPickupEventArgs)"/>
        protected virtual void PickingUpItemBehavior(SearchingPickupEventArgs ev)
        {
            if (!Check(ev.Player) || Settings.CanPickupItems)
                return;

            ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnEscaping(Exiled.Events.EventArgs.Player.EscapingEventArgs)"/>
        protected virtual void PreventPlayerFromEscaping(Exiled.Events.EventArgs.Player.EscapingEventArgs ev)
        {
            if (!Check(ev.Player) || useCustomEscape || wasEscaped || !EscapeSettings.IsEmpty())
                return;

            ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnDied(DiedEventArgs)"/>
        protected virtual void AnnounceOwnerDeath(DiedEventArgs ev)
        {
            if (!Check(ev.Player) || Check(ev.Attacker) || Check(Server.Host) || !Settings.IsDeathAnnouncementEnabled)
                return;

            void DoAnnounce(string announcement)
            {
                if (string.IsNullOrEmpty(announcement))
                    announcement = Settings.UnknownTerminationCauseAnnouncement;

                if (!string.IsNullOrEmpty(announcement))
                    Cassie.Message(announcement);
            }

            string announcement = string.Empty;
            if (ev.Attacker is null)
            {
                DoAnnounce(announcement);
                return;
            }

            if (CustomRole.TryGet(ev.Attacker, out CustomRole customRole))
            {
                if (CustomTeam.TryGet<CustomTeam>(customRole, out CustomTeam customTeam) &&
                    Settings.KilledByCustomTeamAnnouncements.TryGetValue(customTeam.Id, out announcement))
                {
                    DoAnnounce(announcement);
                    return;
                }
                else if (Settings.KilledByCustomRoleAnnouncements.TryGetValue(customRole.Id, out announcement))
                {
                    DoAnnounce(announcement);
                    return;
                }
            }
            else
            {
                if (Settings.KilledByRoleAnnouncements.TryGetValue(ev.Attacker.Role, out announcement))
                {
                    DoAnnounce(announcement);
                    return;
                }
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnIntercomSpeaking(IntercomSpeakingEventArgs)"/>
        protected virtual void IntercomSpeakingBehavior(IntercomSpeakingEventArgs ev)
        {
            if (!Check(ev.Player) || Settings.CanUseIntercom)
                return;

            ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnEnteringPocketDimension(EnteringPocketDimensionEventArgs)"/>
        protected virtual void EnteringPocketDimensionBehavior(EnteringPocketDimensionEventArgs ev)
        {
            if (!Check(ev.Player) || Settings.CanEnterPocketDimension)
                return;

            ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnVoiceChatting(VoiceChattingEventArgs)"/>
        protected virtual void VoiceChattingBehavior(VoiceChattingEventArgs ev)
        {
            if (!Check(ev.Player) || Settings.CanUseVoiceChat)
                return;

            ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Map.OnPlacingBlood(PlacingBloodEventArgs)"/>
        protected virtual void PlacingBloodBehavior(PlacingBloodEventArgs ev)
        {
            if (!Check(ev.Player) || Settings.CanPlaceBlood)
                return;

            ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnEnteringPocketDimension(EnteringPocketDimensionEventArgs)"/>
        protected virtual void DroppingItemBehavior(DroppingItemEventArgs ev)
        {
            if (!Check(ev.Player) || Settings.CanDropItems)
                return;

            ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnActivatingWarheadPanel(ActivatingWarheadPanelEventArgs)"/>
        protected virtual void ActivatingWarheadBehavior(ActivatingWarheadPanelEventArgs ev)
        {
            if (!Check(ev.Player) || Settings.CanActivateWarhead)
                return;

            ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnActivatingGenerator(ActivatingGeneratorEventArgs)"/>
        protected virtual void ActivatingGeneratorBehavior(ActivatingGeneratorEventArgs ev)
        {
            if (!Check(ev.Player) || Settings.CanActivateGenerators)
                return;

            ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnActivatingWorkstation(ActivatingWorkstationEventArgs)"/>
        protected virtual void ActivatingWorkstationBehavior(ActivatingWorkstationEventArgs ev)
        {
            if (!Check(ev.Player) || Settings.CanActivateWorkstations)
                return;

            ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnInteractingElevator(InteractingElevatorEventArgs)"/>
        protected virtual void InteractingElevatorBehavior(InteractingElevatorEventArgs ev)
        {
            if (!Check(ev.Player) || Settings.CanUseElevators)
                return;

            ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnInteractingDoor(InteractingDoorEventArgs)"/>
        protected virtual void CheckpointsBehavior(InteractingDoorEventArgs ev)
        {
            if (!Check(ev.Player) || !Settings.CanBypassCheckpoints || !ev.Door.IsCheckpoint)
                return;

            ev.IsAllowed = true;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnInteractingDoor(InteractingDoorEventArgs)"/>
        protected virtual void InteractingDoorBehavior(InteractingDoorEventArgs ev)
        {
            if (!Check(ev.Player) || !Settings.BypassableDoors.Contains(ev.Door.Type))
                return;

            ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnChangingNickname(ChangingNicknameEventArgs)"/>
        protected void OnInternalChangingNickname(ChangingNicknameEventArgs ev)
        {
            if (!Check(ev.Player))
                return;

            ev.Player.CustomInfo = $"{ev.NewName}\n{Settings.CustomInfo}";
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnSpawningRagdoll(SpawningRagdollEventArgs)"/>
        protected void OnSpawningRagdoll(SpawningRagdollEventArgs ev)
        {
            if (!Check(ev.Player))
                return;

            ev.Role = Role;
        }
    }
}