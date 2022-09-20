// -----------------------------------------------------------------------
// <copyright file="Player.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using CustomPlayerEffects;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.DamageHandlers;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Roles;
    using Exiled.API.Structs;

    using Footprinting;

    using global::Scp914;

    using Hints;

    using InventorySystem;
    using InventorySystem.Disarming;
    using InventorySystem.Items;
    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Firearms.BasicMessages;
    using InventorySystem.Items.Usables.Scp330;

    using MapGeneration.Distributors;

    using MEC;

    using Mirror;
    using Mirror.LiteNetLib4Mirror;

    using NorthwoodLib;
    using NorthwoodLib.Pools;

    using PlayableScps;

    using PlayerStatsSystem;

    using RemoteAdmin;

    using RoundRestarting;

    using UnityEngine;

    using Utils.Networking;

    using static Exiled.API.Features.DamageHandlers.DamageHandlerBase;

    using CustomHandlerBase = Exiled.API.Features.DamageHandlers.DamageHandlerBase;
    using DamageHandlerBase = PlayerStatsSystem.DamageHandlerBase;
    using Firearm = Exiled.API.Features.Items.Firearm;
    using Random = UnityEngine.Random;

    /// <summary>
    /// Represents the in-game player, by encapsulating a <see cref="global::ReferenceHub"/>.
    /// </summary>
    public class Player
    {
#pragma warning disable SA1401
        /// <summary>
        /// A list of the player's items.
        /// </summary>
        internal readonly List<Item> ItemsValue = new(8);
#pragma warning restore SA1401

        private readonly IReadOnlyCollection<Item> readOnlyItems;

        /// <summary>
        /// The running speed of the player.
        /// </summary>
        private float? runningSpeed;

        /// <summary>
        /// The walk speed of the player.
        /// </summary>
        private float? walkingSpeed;

        private ReferenceHub referenceHub;
        private CustomHealthStat healthStat;
        private Role role;
        private HashSet<EActor> components = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="referenceHub">The <see cref="global::ReferenceHub"/> of the player to be encapsulated.</param>
        public Player(ReferenceHub referenceHub)
        {
            readOnlyItems = ItemsValue.AsReadOnly();
            ReferenceHub = referenceHub;
            Timing.CallDelayed(0.05f, () => Role = Role.Create(referenceHub.characterClassManager.NetworkCurClass, this));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="gameObject">The <see cref="UnityEngine.GameObject"/> of the player.</param>
        public Player(GameObject gameObject)
        {
            readOnlyItems = ItemsValue.AsReadOnly();
            ReferenceHub = ReferenceHub.GetHub(gameObject);
            Timing.CallDelayed(0.05f, () => Role = Role.Create(ReferenceHub.characterClassManager.NetworkCurClass, this));
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Player"/> class.
        /// </summary>
        ~Player()
        {
            HashSetPool<int>.Shared.Return(TargetGhostsHashSet);
        }

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing all <see cref="Player"/>'s on the server.
        /// </summary>
        public static Dictionary<GameObject, Player> Dictionary { get; } = new(20, new ReferenceHub.GameObjectComparer());

        /// <summary>
        /// Gets a list of all <see cref="Player"/>'s on the server.
        /// </summary>
        public static IEnumerable<Player> List
        {
            get => Dictionary.Values;
        }

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing cached <see cref="Player"/> and their user ids.
        /// </summary>
        public static Dictionary<string, Player> UserIdsCache { get; } = new(20);

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing cached <see cref="Player"/> and their ids.
        /// </summary>
        public static Dictionary<int, Player> IdsCache { get; } = new(20);

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing cached <see cref="RoleType"/> and their FF multiplier. This is for non-unique roles.
        /// </summary>
        public Dictionary<RoleType, float> FriendlyFireMultiplier { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing cached <see cref="string"/> and their  <see cref="Dictionary{TKey, TValue}"/> which is cached Role with FF multiplier. This is for unique custom roles.
        /// </summary>
        /// <remarks> Consider adding this as object, Dict so that CustomRoles, and Strings can be parsed. </remarks>
        public Dictionary<string, Dictionary<RoleType, float>> CustomRoleFriendlyFireMultiplier { get; set; } = new();

        /// <summary>
        /// Gets or sets a unique custom role that does not adbide to base game for this player. Used in conjunction with <see cref="CustomRoleFriendlyFireMultiplier"/>.
        /// </summary>
        public string UniqueRole { get; set; } = string.Empty;

        /// <summary>
        /// Gets the encapsulated <see cref="global::ReferenceHub"/>.
        /// </summary>
        public ReferenceHub ReferenceHub
        {
            get => referenceHub;
            private set
            {
                referenceHub = value ?? throw new NullReferenceException("Player's ReferenceHub cannot be null!");
                GameObject = value.gameObject;
                HintDisplay = value.hints;
                Inventory = value.inventory;
                CameraTransform = value.PlayerCameraReference;

                value.playerStats.StatModules[0] = healthStat = new CustomHealthStat { Hub = value };
                if (!value.playerStats._dictionarizedTypes.ContainsKey(typeof(HealthStat)))
                    value.playerStats._dictionarizedTypes.Add(typeof(HealthStat), healthStat);
            }
        }

        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}"/> of <see cref="EActor"/> containing all the player's components.
        /// </summary>
        public IReadOnlyCollection<EActor> Components
        {
            get => components;
        }

        /// <summary>
        /// Gets the player's ammo.
        /// </summary>
        public Dictionary<ItemType, ushort> Ammo
        {
            get => Inventory.UserInventory.ReserveAmmo;
        }

        /// <summary>
        /// Gets the encapsulated <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        public GameObject GameObject { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the player is viewing a hint.
        /// </summary>
        public bool HasHint { get; internal set; }

        /// <summary>
        /// Gets the encapsulated <see cref="ReferenceHub"/>'s <see cref="global::Radio"/>.
        /// </summary>
        public global::Radio Radio
        {
            get => ReferenceHub.radio;
        }

        /// <summary>
        /// Gets the <see cref="Hints.HintDisplay"/> of the player.
        /// </summary>
        public HintDisplay HintDisplay { get; private set; }

        /// <summary>
        /// Gets the player's <see cref="InventorySystem.Inventory"/>.
        /// </summary>
        public Inventory Inventory { get; private set; }

        /// <summary>
        /// Gets the encapsulated <see cref="ReferenceHub"/>'s <see cref="Transform">PlayerCameraReference</see>.
        /// </summary>
        public Transform CameraTransform { get; private set; }

        /// <summary>
        /// Gets the player's <see cref="Assets._Scripts.Dissonance.DissonanceUserSetup"/>.
        /// </summary>
        public Assets._Scripts.Dissonance.DissonanceUserSetup DissonanceUserSetup
        {
            get => referenceHub.dissonanceUserSetup;
        }

        /// <summary>
        /// Gets or sets the player's id.
        /// </summary>
        public int Id
        {
            get => ReferenceHub.queryProcessor.NetworkPlayerId;
            set => ReferenceHub.queryProcessor.NetworkPlayerId = value;
        }

        /// <summary>
        /// Gets the player's user id.
        /// </summary>
        public string UserId
        {
            get => referenceHub.characterClassManager.UserId;
        }

        /// <summary>
        /// Gets or sets the player's custom user id.
        /// </summary>
        public string CustomUserId
        {
            get => ReferenceHub.characterClassManager.UserId2;
            set => ReferenceHub.characterClassManager.UserId2 = value;
        }

        /// <summary>
        /// Gets the player's user id without the authentication.
        /// </summary>
        public string RawUserId { get; internal set; }

        /// <summary>
        /// Gets the player's authentication token.
        /// </summary>
        public string AuthenticationToken
        {
            get => ReferenceHub.characterClassManager.AuthToken;
        }

        /// <summary>
        /// Gets the player's authentication type.
        /// </summary>
        public AuthenticationType AuthenticationType
        {
            get
            {
                if (string.IsNullOrEmpty(UserId))
                    return AuthenticationType.Unknown;

                int index = UserId.LastIndexOf('@');

                if (index == -1)
                    return AuthenticationType.Unknown;

                return UserId.Substring(index + 1) switch
                {
                    "steam" => AuthenticationType.Steam,
                    "discord" => AuthenticationType.Discord,
                    "northwood" => AuthenticationType.Northwood,
                    "patreon" => AuthenticationType.Patreon,
                    _ => AuthenticationType.Unknown,
                };
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the player is verified.
        /// </summary>
        /// <remarks>
        /// This is always <see langword="false"/> if <c>online_mode</c> is set to <see langword="false"/>.
        /// </remarks>
        public bool IsVerified { get; internal set; }

        /// <summary>
        /// Gets or sets the player's display nickname.
        /// May be <see langword="null"/>.
        /// </summary>
        public string DisplayNickname
        {
            get => ReferenceHub.nicknameSync.Network_displayName;
            set => ReferenceHub.nicknameSync.Network_displayName = value;
        }

        /// <summary>
        /// Gets the player's nickname.
        /// </summary>
        public string Nickname
        {
            get => ReferenceHub.nicknameSync.Network_myNickSync;
        }

        /// <summary>
        /// Gets or sets the player's player info area bitmask.
        /// This property can be used to hide player name elements, such as the player's name, badges, etc.
        /// </summary>
        public PlayerInfoArea InfoArea
        {
            get => ReferenceHub.nicknameSync.Network_playerInfoToShow;
            set => ReferenceHub.nicknameSync.Network_playerInfoToShow = value;
        }

        /// <summary>
        /// Gets or sets the player's custom player info string. This string is displayed along with the player's <see cref="InfoArea"/>.
        /// </summary>
        public string CustomInfo
        {
            get => ReferenceHub.nicknameSync.Network_customPlayerInfoString;
            set => ReferenceHub.nicknameSync.Network_customPlayerInfoString = value;
        }

        /// <summary>
        /// Gets or sets runningSpeed of the player.
        /// </summary>
        public float RunningSpeed
        {
            get => runningSpeed ??= ServerConfigSynchronizer.Singleton.NetworkHumanSprintSpeedMultiplier;
            set
            {
                runningSpeed = value;
                this.ChangeRunningSpeed(value, false);
            }
        }

        /// <summary>
        /// Gets or sets walkSpeed of the player.
        /// </summary>
        public float WalkingSpeed
        {
            get => walkingSpeed ??= ServerConfigSynchronizer.Singleton.NetworkHumanWalkSpeedMultiplier;
            set
            {
                walkingSpeed = value;
                this.ChangeWalkingSpeed(value, false);
            }
        }

        /// <summary>
        /// Gets the dictionary of the player's session variables.
        /// <para>
        /// Session variables can be used to save temporary data on players. Data is stored in a <see cref="Dictionary{TKey, TValue}"/>.
        /// The key of the data is always a <see cref="string"/>, whereas the value can be any <see cref="object"/>.
        /// The data stored in a player's session variables can be accessed by different assemblies; it is recommended to uniquely identify stored data so that it does not conflict with other plugins that may also be using the same name.
        /// Data saved with session variables is not being saved on player disconnect. If the data must be saved after the player's disconnects, a database must be used instead.
        /// </para>
        /// </summary>
        public Dictionary<string, object> SessionVariables { get; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether or not the player is invisible.
        /// </summary>
        public bool IsInvisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player has the scotopia effect in darkened rooms.
        /// </summary>
        public bool? HasScotopia { get; set; } = null;

        /// <summary>
        /// Gets a value indicating whether or not the player has Do Not Track (DNT) enabled. If this value is <see langword="true"/>, data about the player unrelated to server security shouldn't be stored.
        /// </summary>
        public bool DoNotTrack
        {
            get => ReferenceHub.serverRoles.DoNotTrack;
        }

        /// <summary>
        /// Gets a value indicating whether the player is fully connected to the server.
        /// </summary>
        public bool IsConnected
        {
            get => GameObject != null;
        }

        /// <summary>
        /// Gets a list of player ids who can't see the player.
        /// </summary>
        public HashSet<int> TargetGhostsHashSet { get; } = HashSetPool<int>.Shared.Rent();

        /// <summary>
        /// Gets a value indicating whether or not the player has Remote Admin access.
        /// </summary>
        public bool RemoteAdminAccess
        {
            get => ReferenceHub.serverRoles.RemoteAdmin;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player's overwatch is enabled.
        /// </summary>
        public bool IsOverwatchEnabled
        {
            get => ReferenceHub.serverRoles.OverwatchEnabled;
            set => ReferenceHub.serverRoles.SetOverwatchStatus(value);
        }

        /// <summary>
        /// Gets or sets a value indicating the <see cref="Player"/> that currently has the player cuffed.
        /// <para>
        /// This value will be <see langword="null"/> if the player is not cuffed. Setting this value to <see langword="null"/> will uncuff the player if they are cuffed.
        /// </para>
        /// </summary>
        public Player Cuffer
        {
            get
            {
                foreach (DisarmedPlayers.DisarmedEntry disarmed in DisarmedPlayers.Entries)
                {
                    if (Get(disarmed.DisarmedPlayer) == this)
                        return Get(disarmed.Disarmer);
                }

                return null;
            }

            set
            {
                for (int i = 0; i < DisarmedPlayers.Entries.Count; i++)
                {
                    if (DisarmedPlayers.Entries[i].DisarmedPlayer == Inventory.netId)
                    {
                        DisarmedPlayers.Entries.RemoveAt(i);
                        break;
                    }
                }

                if (value is not null)
                {
                    Inventory.SetDisarmedStatus(value.Inventory);
                    new DisarmedPlayersListMessage(DisarmedPlayers.Entries).SendToAuthenticated();
                }
            }
        }

        /// <summary>
        /// Gets or sets the player's position.
        /// </summary>
        public Vector3 Position
        {
            get => ReferenceHub.playerMovementSync.GetRealPosition();
            set => ReferenceHub.playerMovementSync.ForcePosition(value);
        }

        /// <summary>
        /// Gets or sets the player's rotation.
        /// </summary>
        /// <returns>Returns the direction the player is looking at.</returns>
        public Vector2 Rotation
        {
            get => ReferenceHub.playerMovementSync.RotationSync;
            set
            {
                ReferenceHub.playerMovementSync.NetworkRotationSync = value;
                ReferenceHub.playerMovementSync.ForceRotation(new PlayerMovementSync.PlayerRotation(value.x, value.y));
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="Roles.Role"/> that is unique to this player and this class. This allows modification of various aspects related to the role solely.
        /// <para>
        /// The type of the Role is different based on the <see cref="RoleType"/> of the player, and casting should be used to modify the role.
        /// <br /><see cref="RoleType.Spectator"/> = <see cref="SpectatorRole"/>.
        /// <br /><see cref="RoleType.None"/> = <see cref="NoneRole"/>.
        /// <br /><see cref="RoleType.Scp049"/> = <see cref="Scp049Role"/>.
        /// <br /><see cref="RoleType.Scp0492"/> = <see cref="Scp0492Role"/>.
        /// <br /><see cref="RoleType.Scp079"/> = <see cref="Scp079Role"/>.
        /// <br /><see cref="RoleType.Scp096"/> = <see cref="Scp096Role"/>.
        /// <br /><see cref="RoleType.Scp106"/> = <see cref="Scp106Role"/>.
        /// <br /><see cref="RoleType.Scp173"/> = <see cref="Scp173Role"/>.
        /// <br /><see cref="RoleType.Scp93953"/> / <see cref="RoleType.Scp93989"/> = <see cref="Scp939Role"/>.
        /// <br />If not listed above, the type of Role will be <see cref="HumanRole"/>.
        /// </para>
        /// <para>
        /// If the role object is stored, it may become invalid if the player changes roles. Thus, the <see cref="Features.Roles.Role.IsValid"/> property can be checked. If this property is <see langword="false"/>, the role should be discarded and this property should be used again to get the new Role.
        /// This role is automatically cached until it changes, and it is recommended to use this propertly directly rather than storing the property yourself.
        /// </para>
        /// <para>
        /// Roles and RoleTypes can be compared directly. <c>Player.Role == RoleType.Scp079</c> is valid and will return <see langword="true"/> if the player is SCP-079. To set the player's role, see <see cref="SetRole(RoleType, SpawnReason, bool)"/>.
        /// </para>
        /// </summary>
        /// <seealso cref="SetRole(RoleType, SpawnReason, bool)"/>
        public Role Role
        {
            get => role ??= Role.Create(RoleType.None, this);
            set => role = value;
        }

        /// <summary>
        /// Gets a value indicating whether or not the player is cuffed.
        /// </summary>
        /// <remarks>Players can be cuffed without another player being the cuffer.</remarks>
        public bool IsCuffed
        {
            get => Cuffer is not null;
        }

        /// <summary>
        /// Gets a value indicating whether or not the player is reloading a weapon.
        /// </summary>
        public bool IsReloading
        {
            get => CurrentItem is Firearm firearm && !firearm.Base.AmmoManagerModule.Standby;
        }

        /// <summary>
        /// Gets a value indicating whether or not the player is aiming with a weapon.
        /// </summary>
        public bool IsAimingDownWeapon
        {
            get => CurrentItem is Firearm firearm && firearm.Aiming;
        }

        /// <summary>
        /// Gets a value indicating whether or not the player has enabled weapon's flashlight module.
        /// </summary>
        public bool HasFlashlightModuleEnabled
        {
            get => CurrentItem is Firearm firearm && firearm.FlashlightEnabled;
        }

        /// <summary>
        /// Gets or sets the player's current <see cref="PlayerMovementState"/>.
        /// </summary>
        public PlayerMovementState MoveState
        {
            get => ReferenceHub.animationController.MoveState;
            set => ReferenceHub.animationController.MoveState = value;
        }

        /// <summary>
        /// Gets a value indicating whether or not the player is jumping.
        /// </summary>
        public bool IsJumping { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether or not the player is on the ground.
        /// </summary>
        public bool IsGrounded
        {
            get => ReferenceHub.playerMovementSync.NetworkGrounded;
        }

        /// <summary>
        /// Gets a value indicating whether or not the player is sprinting.
        /// </summary>
        public bool IsSprinting
        {
            get => MoveState == PlayerMovementState.Sprinting;
        }

        /// <summary>
        /// Gets a value indicating whether or not the player is walking.
        /// </summary>
        public bool IsWalking
        {
            get => MoveState == PlayerMovementState.Walking;
        }

        /// <summary>
        /// Gets a value indicating whether or not the player is sneaking.
        /// </summary>
        public bool IsSneaking
        {
            get => MoveState == PlayerMovementState.Sneaking;
        }

        /// <summary>
        /// Gets the player's IP address.
        /// </summary>
        public string IPAddress
        {
            get => ReferenceHub.networkIdentity.connectionToClient.address;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player has No-clip enabled.
        /// </summary>
        /// <returns><see cref="bool"/> indicating status.</returns>
        public bool NoClipEnabled
        {
            get => ReferenceHub.serverRoles.NoclipReady;
            set => ReferenceHub.serverRoles.NoclipReady = value;
        }

        /// <summary>
        /// Gets the player's command sender instance.
        /// </summary>
        public PlayerCommandSender Sender
        {
            get => ReferenceHub.queryProcessor._sender;
        }

        /// <summary>
        /// Gets player's <see cref="NetworkConnection"/>.
        /// </summary>
        public NetworkConnection Connection
        {
            get => ReferenceHub.scp079PlayerScript.connectionToClient;
        }

        /// <summary>
        /// Gets the player's <see cref="Mirror.NetworkIdentity"/>.
        /// </summary>
        public NetworkIdentity NetworkIdentity
        {
            get => ReferenceHub.networkIdentity;
        }

        /// <summary>
        /// Gets a value indicating whether or not the player is the host.
        /// </summary>
        public bool IsHost
        {
            get => ReferenceHub.isDedicatedServer;
        }

        /// <summary>
        /// Gets a value indicating whether or not the player is alive.
        /// </summary>
        public bool IsAlive
        {
            get => !IsDead;
        }

        /// <summary>
        /// Gets a value indicating whether or not the player is dead.
        /// </summary>
        public bool IsDead
        {
            get => Role?.Team is Team.RIP;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player's friendly fire is enabled.
        /// <br>This property only determines if this player can deal damage to players on the same team;</br>
        /// <br>This player can be damaged by other players on their own team even if this property is <see langword="false"/>.</br>
        /// </summary>
        public bool IsFriendlyFireEnabled
        {
            get => FriendlyFireMultiplier.Count > 0 || CustomRoleFriendlyFireMultiplier.Count > 0;
            [Obsolete("Use SetFriendlyFire instead.", true)]
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the player's scale.
        /// </summary>
        public Vector3 Scale
        {
            get => ReferenceHub.transform.localScale;
            set
            {
                try
                {
                    ReferenceHub.transform.localScale = value;

                    foreach (Player target in List)
                        Server.SendSpawnMessage?.Invoke(null, new object[] { ReferenceHub.characterClassManager.netIdentity, target.Connection });
                }
                catch (Exception exception)
                {
                    Log.Error($"{nameof(Scale)} error: {exception}");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player's bypass mode is enabled.
        /// </summary>
        public bool IsBypassModeEnabled
        {
            get => ReferenceHub.serverRoles.BypassMode;
            set => ReferenceHub.serverRoles.BypassMode = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player is muted.
        /// </summary>
        /// <remarks>This property will NOT persistently mute and unmute the player. For persistent mutes, see <see cref="Mute(bool)"/> and <see cref="UnMute(bool)"/>.</remarks>
        public bool IsMuted
        {
            get => ReferenceHub.dissonanceUserSetup.AdministrativelyMuted;
            set => ReferenceHub.dissonanceUserSetup.AdministrativelyMuted = value;
        }

        /// <summary>
        /// Gets or sets the player's <see cref="Assets._Scripts.Dissonance.VoicechatMuteStatus"/>.
        /// </summary>
        public Assets._Scripts.Dissonance.VoicechatMuteStatus MuteStatus
        {
            get => ReferenceHub.dissonanceUserSetup.NetworkmuteStatus;
            set => ReferenceHub.dissonanceUserSetup.NetworkmuteStatus = value;
        }

        /// <summary>
        /// Gets or sets the player's <see cref="Assets._Scripts.Dissonance.SpeakingFlags"/>.
        /// </summary>
        /// <remarks>Voicechat channels are handled by the client, therefore any changes will be ignored.</remarks>
        public Assets._Scripts.Dissonance.SpeakingFlags SpeakingFlags
        {
            get => ReferenceHub.dissonanceUserSetup.NetworkspeakingFlags;
            set => ReferenceHub.dissonanceUserSetup.NetworkspeakingFlags = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player is intercom muted.
        /// </summary>
        /// <remarks>This property will NOT persistently mute and unmute the player. For persistent mutes, see <see cref="Mute(bool)"/> and <see cref="UnMute(bool)"/>.</remarks>
        public bool IsIntercomMuted
        {
            get => ReferenceHub.characterClassManager.NetworkIntercomMuted;
            set => ReferenceHub.characterClassManager.NetworkIntercomMuted = value;
        }

        /// <summary>
        /// Gets a value indicating whether or not the player is voice chatting.
        /// </summary>
        public bool IsVoiceChatting
        {
            get => ReferenceHub.radio.UsingVoiceChat;
        }

        /// <summary>
        /// Gets a value indicating whether or not the player is transmitting on a Radio.
        /// </summary>
        public bool IsTransmitting
        {
            get => ReferenceHub.radio.UsingRadio;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player has godmode enabled.
        /// </summary>
        public bool IsGodModeEnabled
        {
            get => ReferenceHub.characterClassManager.GodMode;
            set => ReferenceHub.characterClassManager.GodMode = value;
        }

        /// <summary>
        /// Gets or sets the player's unit name.
        /// </summary>
        public string UnitName
        {
            get => ReferenceHub.characterClassManager.NetworkCurUnitName;
            set => ReferenceHub.characterClassManager.NetworkCurUnitName = value;
        }

        /// <summary>
        /// Gets or sets the player's health.
        /// If the health is greater than the <see cref="MaxHealth"/>, the MaxHealth will also be changed to match the health.
        /// </summary>
        public float Health
        {
            get => healthStat.CurValue;
            set
            {
                healthStat.CurValue = value;

                if (value > MaxHealth)
                    MaxHealth = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the player's maximum health.
        /// </summary>
        public int MaxHealth
        {
            get => (int)healthStat.MaxValue;
            set => healthStat.CustomMaxValue = value;
        }

        /// <summary>
        /// Gets or sets the player's artificial health.
        /// If the health is greater than the <see cref="MaxArtificialHealth"/>, it will also be changed to match the artificial health.
        /// </summary>
        public float ArtificialHealth
        {
            get => ActiveArtificialHealthProcesses.FirstOrDefault()?.CurrentAmount ?? 0f;

            set
            {
                if (value > MaxArtificialHealth)
                    MaxArtificialHealth = value;

                AhpStat.AhpProcess ahp = ActiveArtificialHealthProcesses.FirstOrDefault();
                if (ahp is not null)
                    ahp.CurrentAmount = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's maximum artificial health.
        /// </summary>
        public float MaxArtificialHealth
        {
            get => ActiveArtificialHealthProcesses.FirstOrDefault()?.Limit ?? 0f;

            set
            {
                if (!ActiveArtificialHealthProcesses.Any())
                    AddAhp(value);

                AhpStat.AhpProcess ahp = ActiveArtificialHealthProcesses.FirstOrDefault();
                if (ahp is not null)
                    ahp.Limit = value;
            }
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of all active Artificial Health processes on the player.
        /// </summary>
        public IEnumerable<AhpStat.AhpProcess> ActiveArtificialHealthProcesses
        {
            get => ((AhpStat)ReferenceHub.playerStats.StatModules[1])._activeProcesses;
        }

        /// <summary>
        /// Gets or sets the player's current SCP.
        /// </summary>
        public PlayableScp CurrentScp
        {
            get => ReferenceHub.scpsController.CurrentScp;
            set => ReferenceHub.scpsController.CurrentScp = value;
        }

        /// <summary>
        /// Gets or sets the item in the player's hand, returns the default value if empty.
        /// </summary>
        public Item CurrentItem
        {
            get => Item.Get(Inventory.CurInstance);

            set
            {
                if (value is null || value.Type == ItemType.None)
                {
                    Inventory.ServerSelectItem(0);
                }
                else
                {
                    if (!Inventory.UserInventory.Items.TryGetValue(value.Serial, out _))
                    {
                        AddItem(value.Base);
                    }

                    Timing.CallDelayed(0.5f, () => Inventory.ServerSelectItem(value.Serial));
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="global::Stamina"/> class.
        /// </summary>
        public Stamina Stamina
        {
            get => ReferenceHub.fpc.staminaController;
        }

        /// <summary>
        /// Gets a value indicating whether or not the staff bypass is enabled.
        /// </summary>
        public bool IsStaffBypassEnabled
        {
            get => ReferenceHub.serverRoles.BypassStaff;
        }

        /// <summary>
        /// Gets or sets the player's group name.
        /// </summary>
        public string GroupName
        {
            get => ServerStatic.PermissionsHandler._members.TryGetValue(UserId, out string groupName)
                ? groupName
                : null;
            set => ServerStatic.PermissionsHandler._members[UserId] = value;
        }

        /// <summary>
        /// Gets the current <see cref="Room"/> the player is in.
        /// </summary>
        public Room CurrentRoom
        {
            get => Map.FindParentRoom(GameObject);
        }

        /// <summary>
        /// Gets the current zone the player is in.
        /// </summary>
        public ZoneType Zone
        {
            get => CurrentRoom?.Zone ?? ZoneType.Unspecified;
        }

        /// <summary>
        /// Gets all currently active <see cref="PlayerEffect">status effects</see>.
        /// </summary>
        public IEnumerable<PlayerEffect> ActiveEffects
        {
            get => referenceHub.playerEffectsController.AllEffects.Values.Where(effect => effect.Intensity > 0);
        }

        /// <summary>
        /// Gets or sets the player's group.
        /// </summary>
        public UserGroup Group
        {
            get => ReferenceHub.serverRoles.Group;
            set => ReferenceHub.serverRoles.SetGroup(value, false);
        }

        /// <summary>
        /// Gets or sets the player's rank color.
        /// </summary>
        public string RankColor
        {
            get => ReferenceHub.serverRoles.Network_myColor;
            set => ReferenceHub.serverRoles.SetColor(value);
        }

        /// <summary>
        /// Gets or sets the player's rank name.
        /// </summary>
        public string RankName
        {
            get => ReferenceHub.serverRoles.Network_myText;
            set => ReferenceHub.serverRoles.SetText(value);
        }

        /// <summary>
        /// Gets the global badge of the player, can be <see langword="null"/> if none.
        /// </summary>
        public Badge? GlobalBadge
        {
            get
            {
                if (string.IsNullOrEmpty(ReferenceHub.serverRoles.NetworkGlobalBadge))
                    return null;

                ServerRoles serverRoles = ReferenceHub.serverRoles;

                return new Badge(serverRoles._bgt, serverRoles._bgc, serverRoles.GlobalBadgeType, true);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player's badge is hidden.
        /// </summary>
        public bool BadgeHidden
        {
            get => !string.IsNullOrEmpty(ReferenceHub.serverRoles.HiddenBadge);
            set
            {
                if (value)
                    ReferenceHub.characterClassManager.UserCode_CmdRequestHideTag();
                else
                    ReferenceHub.characterClassManager.UserCode_CmdRequestShowTag(false);
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not a player is Northwood staff.
        /// </summary>
        public bool IsNorthwoodStaff
        {
            get => ReferenceHub.serverRoles.Staff;
        }

        /// <summary>
        /// Gets a value indicating whether or not a player is a global moderator.
        /// </summary>
        public bool IsGlobalModerator
        {
            get => ReferenceHub.serverRoles.RaEverywhere;
        }

        /// <summary>
        /// Gets a value indicating whether or not the player is in the pocket dimension.
        /// </summary>
        public bool IsInPocketDimension
        {
            get => GetEffectActive<Corroding>() || Map.FindParentRoom(GameObject)?.Type == RoomType.Pocket;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player should use stamina system.
        /// </summary>
        public bool IsUsingStamina { get; set; } = true;

        /// <summary>
        /// Gets the player's ping.
        /// </summary>
        public int Ping
        {
            get => LiteNetLib4MirrorServer.GetPing(Connection.connectionId);
        }

        /// <summary>
        /// Gets the player's items.
        /// </summary>
        public IReadOnlyCollection<Item> Items
        {
            get => readOnlyItems;
        }

        /// <summary>
        /// Gets a value indicating whether or not the player's inventory is empty.
        /// </summary>
        public bool IsInventoryEmpty
        {
            get => Items.Count == 0;
        }

        /// <summary>
        /// Gets a value indicating whether or not the player's inventory is full.
        /// </summary>
        public bool IsInventoryFull
        {
            get => Items.Count >= 8;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can send inputs.
        /// </summary>
        public bool CanSendInputs
        {
            get => !ReferenceHub.fpc.NetworkforceStopInputs;
            set => ReferenceHub.fpc.NetworkforceStopInputs = !value;
        }

        /// <summary>
        /// Gets a <see cref="Player"/> <see cref="IEnumerable{T}"/> of spectators that are currently spectating this <see cref="Player"/>.
        /// </summary>
        public IEnumerable<Player> CurrentSpectatingPlayers
        {
            get
            {
                foreach (ReferenceHub referenceHub in ReferenceHub.spectatorManager.ServerCurrentSpectatingPlayers)
                {
                    if (referenceHub == ReferenceHub)
                        continue;

                    yield return Get(referenceHub);
                }
            }
        }

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> which contains all player's preferences.
        /// </summary>
        public Dictionary<ItemType, AttachmentIdentifier[]> Preferences
        {
            get => Firearm.PlayerPreferences.FirstOrDefault(kvp => kvp.Key == this).Value;
        }

        /// <summary>
        /// Gets the player's <see cref="Footprinting.Footprint"/>.
        /// </summary>
        public Footprint Footprint
        {
            get => new(ReferenceHub);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player is spawn protected.
        /// </summary>
        public bool IsSpawnProtected
        {
            get => ReferenceHub.characterClassManager.SpawnProtected;
            set => ReferenceHub.characterClassManager.SpawnProtected = value;
        }

        /// <summary>
        /// Gets a dictionary for storing player objects of connected but not yet verified players.
        /// </summary>
        internal static ConditionalWeakTable<ReferenceHub, Player> UnverifiedPlayers { get; } = new();

        /// <summary>
        /// Gets a <see cref="Player"/> <see cref="IEnumerable{T}"/> filtered by side. Can be empty.
        /// </summary>
        /// <param name="side">The players' side.</param>
        /// <returns>The filtered <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<Player> Get(Side side) => List.Where(player => player.Role.Side == side);

        /// <summary>
        /// Gets a <see cref="Player"/> <see cref="IEnumerable{T}"/> filtered by team. Can be empty.
        /// </summary>
        /// <param name="team">The players' team.</param>
        /// <returns>The filtered <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<Player> Get(Team team) => List.Where(player => player.Role.Team == team);

        /// <summary>
        /// Gets a <see cref="Player"/> <see cref="IEnumerable{T}"/> filtered by role. Can be empty.
        /// </summary>
        /// <param name="role">The players' role.</param>
        /// <returns>The filtered <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<Player> Get(RoleType role) => List.Where(player => player.Role == role);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Player"/> filtered based on a predicate.
        /// </summary>
        /// <param name="predicate">The condition to satisfy.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Player"/> which contains elements that satify the condition.</returns>
        public static IEnumerable<Player> Get(Func<Player, bool> predicate) => List.Where(predicate);

        /// <summary>
        /// Gets the <see cref="Player"/> belonging to the <see cref="CommandSystem.ICommandSender"/>, if any.
        /// </summary>
        /// <param name="sender">The command sender.</param>
        /// <returns>A <see cref="Player"/> or <see langword="null"/> if not found.</returns>
        public static Player Get(CommandSystem.ICommandSender sender) => Get(sender as CommandSender);

        /// <summary>
        /// Gets the <see cref="Player"/> belonging to the <see cref="CommandSender"/>, if any.
        /// </summary>
        /// <param name="sender">The command sender.</param>
        /// <returns>A <see cref="Player"/> or <see langword="null"/> if not found.</returns>
        public static Player Get(CommandSender sender) => Get(sender.SenderId);

        /// <summary>
        /// Gets the <see cref="Player"/> belonging to the <see cref="global::ReferenceHub"/>, if any.
        /// </summary>
        /// <param name="referenceHub">The player's <see cref="global::ReferenceHub"/>.</param>
        /// <returns>A <see cref="Player"/> or <see langword="null"/> if not found.</returns>
        public static Player Get(ReferenceHub referenceHub)
        {
            try
            {
                return referenceHub == null || referenceHub.gameObject == null ? null : Get(referenceHub.gameObject);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the <see cref="Player"/> belonging to the <see cref="Collider"/>, if any.
        /// </summary>
        /// <param name="collider"><see cref="Collider"/>.</param>
        /// <returns>A <see cref="Player"/> or <see langword="null"/> if not found.</returns>
        public static Player Get(Collider collider) => Get(collider.transform.root.gameObject);

        /// <summary>
        /// Gets the <see cref="Player"/> belonging to a specific netId, if any.
        /// </summary>
        /// <param name="netId">The player's <see cref="Mirror.NetworkIdentity.netId"/>.</param>
        /// <returns>The <see cref="Player"/> owning the netId, or <see langword="null"/> if not found.</returns>
        public static Player Get(uint netId) =>
            ReferenceHub.TryGetHubNetID(netId, out ReferenceHub hub) ? Get(hub) : null;

        /// <summary>
        /// Gets the <see cref="Player"/> belonging to a specific <see cref="Mirror.NetworkIdentity"/>, if any.
        /// </summary>
        /// <param name="netIdentity">The player's <see cref="Mirror.NetworkIdentity"/>.</param>
        /// <returns>The <see cref="Player"/> owning the <see cref="Mirror.NetworkIdentity"/>, or <see langword="null"/> if not found.</returns>
        public static Player Get(NetworkIdentity netIdentity) => Get(netIdentity.netId);

        /// <summary>
        /// Gets the <see cref="Player"/> belonging to a specific <see cref="NetworkConnection"/>, if any.
        /// </summary>
        /// <param name="conn">The player's <see cref="NetworkConnection"/>.</param>
        /// <returns>The <see cref="Player"/> owning the <see cref="NetworkConnection"/>, or <see langword="null"/> if not found.</returns>
        public static Player Get(NetworkConnection conn) => Get(conn.identity);

        /// <summary>
        /// Gets the <see cref="Player"/> belonging to the <see cref="UnityEngine.GameObject"/>, if any.
        /// </summary>
        /// <param name="gameObject">The player's <see cref="UnityEngine.GameObject"/>.</param>
        /// <returns>A <see cref="Player"/> or <see langword="null"/> if not found.</returns>
        public static Player Get(GameObject gameObject)
        {
            if (gameObject == null)
                return null;

            Dictionary.TryGetValue(gameObject, out Player player);

            return player;
        }

        /// <summary>
        /// Gets the player belonging to the specified id.
        /// </summary>
        /// <param name="id">The player id.</param>
        /// <returns>Returns the player found or <see langword="null"/> if not found.</returns>
        public static Player Get(int id)
        {
            if (IdsCache.TryGetValue(id, out Player player) && player?.ReferenceHub is not null)
                return player;

            foreach (Player playerFound in Dictionary.Values)
            {
                if (playerFound.Id != id)
                    continue;

                IdsCache[id] = playerFound;

                return playerFound;
            }

            return null;
        }

        /// <summary>
        /// Gets the player by identifier.
        /// </summary>
        /// <param name="args">The player's nickname, ID, steamID64 or Discord ID.</param>
        /// <returns>Returns the player found or <see langword="null"/> if not found.</returns>
        public static Player Get(string args)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(args))
                    return null;

                if (UserIdsCache.TryGetValue(args, out Player playerFound) && playerFound?.ReferenceHub is not null)
                    return playerFound;

                if (int.TryParse(args, out int id))
                    return Get(id);

                if (args.EndsWith("@steam") || args.EndsWith("@discord") || args.EndsWith("@northwood") ||
                    args.EndsWith("@patreon"))
                {
                    foreach (Player player in Dictionary.Values)
                    {
                        if (player.UserId == args)
                        {
                            playerFound = player;
                            break;
                        }
                    }
                }
                else
                {
                    int lastnameDifference = 31;
                    string firstString = args.ToLower();

                    foreach (Player player in Dictionary.Values)
                    {
                        if (!player.IsVerified || player.Nickname is null)
                            continue;

                        if (!player.Nickname.Contains(args, StringComparison.OrdinalIgnoreCase))
                            continue;

                        string secondString = player.Nickname;

                        int nameDifference = secondString.Length - firstString.Length;
                        if (nameDifference < lastnameDifference)
                        {
                            lastnameDifference = nameDifference;
                            playerFound = player;
                        }
                    }
                }

                if (playerFound is not null)
                    UserIdsCache[args] = playerFound;

                return playerFound;
            }
            catch (Exception exception)
            {
                Log.Error($"{typeof(Player).FullName}.{nameof(Get)} error: {exception}");
                return null;
            }
        }

        /// <summary>
        /// Try-get a player given a <see cref="CommandSystem.ICommandSender"/>.
        /// </summary>
        /// <param name="sender">The <see cref="CommandSystem.ICommandSender"/>.</param>
        /// <param name="player">The player that matches the given <see cref="CommandSystem.ICommandSender"/>, or <see langword="null"/> if no player is found.</param>
        /// <returns>A boolean indicating whether or not a player was found.</returns>
        public static bool TryGet(CommandSystem.ICommandSender sender, out Player player) => (player = Get(sender)) is not null;

        /// <summary>
        /// Try-get a player given a <see cref="CommandSender"/>.
        /// </summary>
        /// <param name="sender">The <see cref="CommandSender"/>.</param>
        /// <param name="player">The player that matches the given <see cref="CommandSender"/>, or <see langword="null"/> if no player is found.</param>
        /// <returns>A boolean indicating whether or not a player was found.</returns>
        public static bool TryGet(CommandSender sender, out Player player) => (player = Get(sender)) is not null;

        /// <summary>
        /// Try-get a player given a <see cref="global::ReferenceHub"/>.
        /// </summary>
        /// <param name="referenceHub">The <see cref="global::ReferenceHub"/>.</param>
        /// <param name="player">The player that matches the given <see cref="global::ReferenceHub"/>, or <see langword="null"/> if no player is found.</param>
        /// <returns>A boolean indicating whether or not a player was found.</returns>
        public static bool TryGet(ReferenceHub referenceHub, out Player player) => (player = Get(referenceHub)) is not null;

        /// <summary>
        /// Try-get a player given a network ID.
        /// </summary>
        /// <param name="netId">The network ID.</param>
        /// <param name="player">The player that matches the given net ID, or <see langword="null"/> if no player is found.</param>
        /// <returns>A boolean indicating whether or not a player was found.</returns>
        public static bool TryGet(uint netId, out Player player) => (player = Get(netId)) is not null;

        /// <summary>
        /// Try-get a player given a <see cref="Mirror.NetworkIdentity"/>.
        /// </summary>
        /// <param name="netIdentity">The <see cref="Mirror.NetworkIdentity"/>.</param>
        /// <param name="player">The player that matches the given <see cref="Mirror.NetworkIdentity"/>, or <see langword="null"/> if no player is found.</param>
        /// <returns>A boolean indicating whether or not a player was found.</returns>
        public static bool TryGet(NetworkIdentity netIdentity, out Player player) => (player = Get(netIdentity)) is not null;

        /// <summary>
        /// Try-get a player given a <see cref="Mirror.NetworkConnection"/>.
        /// </summary>
        /// <param name="conn">The <see cref="Mirror.NetworkConnection"/>.</param>
        /// <param name="player">The player that matches the given <see cref="Mirror.NetworkConnection"/>, or <see langword="null"/> if no player is found.</param>
        /// <returns>A boolean indicating whether or not a player was found.</returns>
        public static bool TryGet(NetworkConnection conn, out Player player) => (player = Get(conn)) is not null;

        /// <summary>
        /// Try-get a player given a <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        /// <param name="gameObject">The <see cref="UnityEngine.GameObject"/>.</param>
        /// <param name="player">The player that matches the given <see cref="UnityEngine.GameObject"/>, or <see langword="null"/> if no player is found.</param>
        /// <returns>A boolean indicating whether or not a player was found.</returns>
        public static bool TryGet(GameObject gameObject, out Player player) => (player = Get(gameObject)) is not null;

        /// <summary>
        /// Try-get a player given an ID.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="player">The player that matches the given ID, or <see langword="null"/> if no player is found.</param>
        /// <returns>A boolean indicating whether or not a player was found.</returns>
        public static bool TryGet(int id, out Player player) => (player = Get(id)) is not null;

        /// <summary>
        /// Try-get a player by identifier.
        /// </summary>
        /// <param name="args">The player's nickname, ID, steamID64 or Discord ID.</param>
        /// <param name="player">The player found or <see langword="null"/> if not found.</param>
        /// <returns>A boolean indicating whether or not a player was found.</returns>
        public static bool TryGet(string args, out Player player) => (player = Get(args)) is not null;

        /// <summary>
        /// Tries to add <see cref="RoleType"/> to FriendlyFire rules.
        /// </summary>
        /// <param name="roleToAdd"> Role to add. </param>
        /// <param name="ffMult"> Friendly fire multiplier. </param>
        public void SetFriendlyFire(RoleType roleToAdd, float ffMult)
        {
            if (FriendlyFireMultiplier.ContainsKey(roleToAdd))
            {
                FriendlyFireMultiplier[roleToAdd] = ffMult;
            }
            else
            {
                FriendlyFireMultiplier.Add(roleToAdd, ffMult);
            }
        }

        /// <summary>
        /// Wrapper to call <see cref="SetFriendlyFire(RoleType, float)"/>.
        /// </summary>
        /// <param name="roleFF"> Role with FF to add even if it exists. </param>
        public void SetFriendlyFire(KeyValuePair<RoleType, float> roleFF)
        {
            SetFriendlyFire(roleFF.Key, roleFF.Value);
        }

        /// <summary>
        /// Tries to add <see cref="RoleType"/> to FriendlyFire rules.
        /// </summary>
        /// <param name="roleToAdd"> Role to add. </param>
        /// <param name="ffMult"> Friendly fire multiplier. </param>
        /// <returns> Whether or not the item was able to be added. </returns>
        public bool TryAddFriendlyFire(RoleType roleToAdd, float ffMult)
        {
            if (FriendlyFireMultiplier.ContainsKey(roleToAdd))
            {
                return false;
            }

            FriendlyFireMultiplier.Add(roleToAdd, ffMult);
            return true;
        }

        /// <summary>
        /// Tries to add <see cref="RoleType"/> to FriendlyFire rules.
        /// </summary>
        /// <param name="pairedRoleFF"> Role FF multiplier to add. </param>
        /// <returns> Whether or not the item was able to be added. </returns>
        public bool TryAddFriendlyFire(KeyValuePair<RoleType, float> pairedRoleFF) => TryAddFriendlyFire(pairedRoleFF.Key, pairedRoleFF.Value);

        /// <summary>
        /// Tries to add <see cref="RoleType"/> to FriendlyFire rules.
        /// </summary>
        /// <param name="ffRules"> Roles to add with friendly fire values. </param>
        /// <param name="overwrite"> Whether or not to overwrite current values if they exist. </param>
        /// <returns> Whether or not the item was able to be added. </returns>
        public bool TryAddFriendlyFire(Dictionary<RoleType, float> ffRules, bool overwrite = false)
        {
            Dictionary<RoleType, float> temporaryFriendlyFireRules = new();
            foreach (KeyValuePair<RoleType, float> roleFF in ffRules)
            {
                if (overwrite)
                {
                    SetFriendlyFire(roleFF);
                }
                else
                {
                    if (!FriendlyFireMultiplier.ContainsKey(roleFF.Key))
                    {
                        temporaryFriendlyFireRules.Add(roleFF.Key, roleFF.Value);
                    }
                    else
                    {
                        // Contained Key but overwrite set to false so we do not add any.
                        return false;
                    }
                }
            }

            if (!overwrite)
            {
                foreach (KeyValuePair<RoleType, float> roleFF in temporaryFriendlyFireRules)
                {
                    TryAddFriendlyFire(roleFF);
                }
            }

            return true;
        }

        /// <summary>
        /// Tries to add <see cref="RoleType"/> to FriendlyFire rules.
        /// </summary>
        /// <param name="roleType"> Role associated for CustomFF. </param>
        /// <param name="roleToAdd"> Role to add. </param>
        /// <param name="ffMult"> Friendly fire multiplier. </param>
        public void SetCustomRoleFriendlyFire(string roleType, RoleType roleToAdd, float ffMult)
        {
            if (CustomRoleFriendlyFireMultiplier.TryGetValue(roleType, out Dictionary<RoleType, float> currentPairedData))
            {
                if (currentPairedData.ContainsKey(roleToAdd))
                {
                    currentPairedData[roleToAdd] = ffMult;
                }
                else
                {
                    currentPairedData.Add(roleToAdd, ffMult);
                }
            }
            else
            {
                Dictionary<RoleType, float> newFFRules = new();
                newFFRules.Add(roleToAdd, ffMult);
                CustomRoleFriendlyFireMultiplier.Add(roleType, newFFRules);
            }
        }

        /// <summary>
        /// Wrapper to call <see cref="SetCustomRoleFriendlyFire(string, RoleType, float)"/>.
        /// </summary>
        /// <param name="roleType"> Role associated for CustomFF. </param>
        /// <param name="roleFF"> Role with FF to add even if it exists. </param>
        public void SetCustomRoleFriendlyFire(string roleType, KeyValuePair<RoleType, float> roleFF)
        {
            SetCustomRoleFriendlyFire(roleType, roleFF.Key, roleFF.Value);
        }

        /// <summary>
        /// Tries to add <see cref="RoleType"/> to FriendlyFire rules for CustomRole.
        /// </summary>
        /// <param name="roleType"> Role associated for CustomFF. </param>
        /// <param name="roleFF"> Role to add and FF multiplier. </param>
        /// <returns> Whether or not the item was able to be added. </returns>
        public bool TryAddCustomRoleFriendlyFire(string roleType, KeyValuePair<RoleType, float> roleFF) => TryAddCustomRoleFriendlyFire(roleType, roleFF.Key, roleFF.Value);

        /// <summary>
        /// Tries to add <see cref="RoleType"/> to FriendlyFire rules for CustomRole.
        /// </summary>
        /// <param name="roleType"> Role associated for CustomFF. </param>
        /// <param name="roleToAdd"> Role to add. </param>
        /// <param name="ffMult"> Friendly fire multiplier. </param>
        /// <returns> Whether or not the item was able to be added. </returns>
        public bool TryAddCustomRoleFriendlyFire(string roleType, RoleType roleToAdd, float ffMult)
        {
            if (CustomRoleFriendlyFireMultiplier.TryGetValue(roleType, out Dictionary<RoleType, float> currentPairedData))
            {
                if (currentPairedData.ContainsKey(roleToAdd))
                {
                    return false;
                }

                currentPairedData.Add(roleToAdd, ffMult);
            }
            else
            {
                SetCustomRoleFriendlyFire(roleType, roleToAdd, ffMult);
            }

            return true;
        }

        /// <summary>
        /// Tries to add <see cref="RoleType"/> to FriendlyFire rules.
        /// </summary>
        /// <param name="customRoleName"> Role associated for CustomFF. </param>
        /// <param name="ffRules"> Roles to add with friendly fire values. </param>
        /// <param name="overwrite"> Whether to overwrite current values if they exist - does NOT delete previous entries if they are not in provided rules. </param>
        /// <returns> Whether or not the item was able to be added. </returns>
        public bool TryAddCustomRoleFriendlyFire(string customRoleName, Dictionary<RoleType, float> ffRules, bool overwrite = false)
        {
            Dictionary<RoleType, float> temporaryFriendlyFireRules = new();
            if (CustomRoleFriendlyFireMultiplier.TryGetValue(customRoleName, out Dictionary<RoleType, float> pairedRoleFF))
            {
                foreach (KeyValuePair<RoleType, float> roleFF in ffRules)
                {
                    if (overwrite)
                    {
                        SetCustomRoleFriendlyFire(customRoleName, roleFF);
                    }
                    else
                    {
                        if (!pairedRoleFF.ContainsKey(roleFF.Key))
                        {
                            temporaryFriendlyFireRules.Add(roleFF.Key, roleFF.Value);
                        }
                        else
                        {
                            // Contained Key but overwrite set to false so we do not add any.
                            return false;
                        }
                    }
                }

                if (!overwrite)
                {
                    foreach (KeyValuePair<RoleType, float> roleFF in temporaryFriendlyFireRules)
                    {
                        TryAddCustomRoleFriendlyFire(customRoleName, roleFF);
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<RoleType, float> roleFF in ffRules)
                {
                    SetCustomRoleFriendlyFire(customRoleName, roleFF);
                }
            }

            return true;
        }

        /// <summary>
        /// Adds the Custom role to the <see cref="CustomRoleFriendlyFireMultiplier"/> if they did not already exist.
        /// </summary>
        /// <param name="customRoleFriendlyFireMultiplier"> Custom role with FF role rules. </param>
        public void TryAddCustomRoleFriendlyFire(Dictionary<string, Dictionary<RoleType, float>> customRoleFriendlyFireMultiplier)
        {
            foreach (KeyValuePair<string, Dictionary<RoleType, float>> newRolesWithFF in customRoleFriendlyFireMultiplier)
            {
                TryAddCustomRoleFriendlyFire(newRolesWithFF.Key, newRolesWithFF.Value);
            }
        }

        /// <summary>
        /// Sets the <see cref="CustomRoleFriendlyFireMultiplier"/>.
        /// </summary>
        /// <param name="customRoleFriendlyFireMultiplier"> New rules for CustomeRoleFriendlyFireMultiplier to set to. </param>
        public void TrySetCustomRoleFriendlyFire(Dictionary<string, Dictionary<RoleType, float>> customRoleFriendlyFireMultiplier)
        {
            CustomRoleFriendlyFireMultiplier = customRoleFriendlyFireMultiplier;
        }

        /// <summary>
        /// Sets the <see cref="CustomRoleFriendlyFireMultiplier"/>.
        /// </summary>
        /// <param name="roleType"> Role to associate FF rules to. </param>
        /// <param name="customRoleFriendlyFireMultiplier"> New rules for CustomeRoleFriendlyFireMultiplier to set to. </param>
        public void TrySetCustomRoleFriendlyFire(string roleType, Dictionary<RoleType, float> customRoleFriendlyFireMultiplier)
        {
            CustomRoleFriendlyFireMultiplier[roleType] = customRoleFriendlyFireMultiplier;
        }

        /// <summary>
        /// Tries to remove <see cref="RoleType"/> from FriendlyFire rules.
        /// </summary>
        /// <param name="role"> Role to add. </param>
        /// <returns> Whether or not the item was able to be added. </returns>
        public bool TryRemoveFriendlyFire(RoleType role) => FriendlyFireMultiplier.Remove(role);

        /// <summary>
        /// Tries to remove <see cref="RoleType"/> from FriendlyFire rules.
        /// </summary>
        /// <param name="role"> Role to add. </param>
        /// <returns> Whether or not the item was able to be added. </returns>
        public bool TryRemoveCustomeRoleFriendlyFire(string role) => CustomRoleFriendlyFireMultiplier.Remove(role);

        /// <summary>
        /// Forces the player to reload their current weapon.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the item is not a firearm.</exception>
        public void ReloadWeapon()
        {
            if (CurrentItem is Firearm firearm)
            {
                firearm.Base.AmmoManagerModule.ServerTryReload();
                Connection.Send(new RequestMessage(firearm.Serial, RequestType.Reload));
            }
            else
            {
                throw new InvalidOperationException("You may only reload weapons.");
            }
        }

        /// <summary>
        /// Tries to get an item from a player's inventory.
        /// </summary>
        /// <param name="serial">The unique identifier of the item.</param>
        /// <param name="item">The <see cref="ItemBase"/> found. <see langword="null"/> if it doesn't exist.</param>
        /// <returns><see langword="true"/> if the item is found, <see langword="false"/> otherwise.</returns>
        [Obsolete("Use TryGetItem(ushort, Item) instead.", true)]
        public bool TryGetItem(ushort serial, out ItemBase item) =>
            Inventory.UserInventory.Items.TryGetValue(serial, out item);

        /// <summary>
        /// Tries to get an item from a player's inventory.
        /// </summary>
        /// <param name="serial">The unique identifier of the item.</param>
        /// <param name="item">The <see cref="Item"/> found. <see langword="null"/> if it doesn't exist.</param>
        /// <returns><see langword="true"/> if the item is found, <see langword="false"/> otherwise.</returns>
        public bool TryGetItem(ushort serial, out Item item)
        {
            item = Inventory.UserInventory.Items.TryGetValue(serial, out ItemBase itemBase) ? Item.Get(itemBase) : null;

            return item != null;
        }

        /// <summary>
        /// Sets the player's rank.
        /// </summary>
        /// <param name="name">The rank name to be set.</param>
        /// <param name="group">The group to be set.</param>
        public void SetRank(string name, UserGroup group)
        {
            if (ServerStatic.GetPermissionsHandler()._groups.ContainsKey(name))
            {
                ServerStatic.GetPermissionsHandler()._groups[name].BadgeColor = group.BadgeColor;
                ServerStatic.GetPermissionsHandler()._groups[name].BadgeText = name;
                ServerStatic.GetPermissionsHandler()._groups[name].HiddenByDefault = !group.Cover;
                ServerStatic.GetPermissionsHandler()._groups[name].Cover = group.Cover;

                ReferenceHub.serverRoles.SetGroup(ServerStatic.GetPermissionsHandler()._groups[name], false, false, group.Cover);
            }
            else
            {
                ServerStatic.GetPermissionsHandler()._groups.Add(name, group);

                ReferenceHub.serverRoles.SetGroup(group, false, false, group.Cover);
            }

            if (ServerStatic.GetPermissionsHandler()._members.ContainsKey(UserId))
                ServerStatic.GetPermissionsHandler()._members[UserId] = name;
            else
                ServerStatic.GetPermissionsHandler()._members.Add(UserId, name);
        }

        /// <summary>
        /// Handcuff the player as administrator.
        /// </summary>
        public void Handcuff()
        {
            ReferenceHub.inventory.SetDisarmedStatus(null);
            DisarmedPlayers.Entries.Add(new DisarmedPlayers.DisarmedEntry(referenceHub.networkIdentity.netId, 0U));
            new DisarmedPlayersListMessage(DisarmedPlayers.Entries).SendToAuthenticated(0);
        }

        /// <summary>
        /// Handcuff the player.
        /// </summary>
        /// <param name="cuffer">The cuffer player.</param>
        public void Handcuff(Player cuffer)
        {
            if (cuffer?.ReferenceHub is null)
                return;

            if (!IsCuffed && (Vector3.Distance(Position, cuffer.Position) <= 130f))
            {
                Cuffer = cuffer;
            }
        }

        /// <summary>
        /// Removes handcuffs.
        /// </summary>
        public void RemoveHandcuffs()
        {
            Inventory.SetDisarmedStatus(null);
            new DisarmedPlayersListMessage(DisarmedPlayers.Entries).SendToAuthenticated();
        }

        /// <summary>
        /// Sets the player's <see cref="RoleType"/>.
        /// </summary>
        /// <param name="newRole">The new <see cref="RoleType"/> to be set.</param>
        /// <param name="reason">The <see cref="SpawnReason"/> defining why the player's role was changed.</param>
        /// <param name="lite">Indicates whether or not it should preserve the position and inventory after changing the role.</param>
        public void SetRole(RoleType newRole, SpawnReason reason = SpawnReason.ForceClass, bool lite = false)
        {
            ReferenceHub.characterClassManager.SetPlayersClass(newRole, GameObject, (CharacterClassManager.SpawnReason)reason, lite);
        }

        /// <summary>
        /// Broadcasts the given <see cref="Features.Broadcast"/> to the player.
        /// </summary>
        /// <param name="broadcast">The <see cref="Features.Broadcast"/> to be broadcasted.</param>
        /// <param name="shouldClearPrevious">Clears all player's broadcasts before sending the new one.</param>
        public void Broadcast(Broadcast broadcast, bool shouldClearPrevious = false)
        {
            if (broadcast.Show)
                Broadcast(broadcast.Duration, broadcast.Content, broadcast.Type, shouldClearPrevious);
        }

        /// <summary>
        /// Drops an item from the player's inventory.
        /// </summary>
        /// <param name="item">The item to be dropped.</param>
        public void DropItem(Item item) => Inventory.ServerDropItem(item.Serial);

        /// <summary>
        /// Drops the held item.
        /// </summary>
        public void DropHeldItem() => DropItem(CurrentItem);

        /// <summary>
        /// Indicates whether or not the player has an item.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns><see langword="true"/>, if the player has it; otherwise, <see langword="false"/>.</returns>
        public bool HasItem(Item item) => Inventory.UserInventory.Items.ContainsValue(item.Base);

        /// <summary>
        /// Indicates whether or not the player has an item type.
        /// </summary>
        /// <param name="type">The type to search for.</param>
        /// <returns><see langword="true"/>, if the player has it; otherwise, <see langword="false"/>.</returns>
        public bool HasItem(ItemType type) =>
            Inventory.UserInventory.Items.Any(tempItem => tempItem.Value.ItemTypeId == type);

        /// <summary>
        /// Counts how many items of a certain <see cref="ItemType"/> a player has.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>How many items of that <see cref="ItemType"/> the player has.</returns>
        /// <remarks>For counting ammo, see <see cref="GetAmmo(AmmoType)"/>.</remarks>
        public int CountItem(ItemType item) =>
            Inventory.UserInventory.Items.Count(tempItem => tempItem.Value.ItemTypeId == item);

        /// <summary>
        /// Counts how many items of a certain <see cref="GrenadeType"/> a player has.
        /// </summary>
        /// <param name="grenadeType">The GrenadeType to search for.</param>
        /// <returns>How many items of that <see cref="GrenadeType"/> the player has.</returns>
        public int CountItem(GrenadeType grenadeType) =>
            Inventory.UserInventory.Items.Count(tempItem => tempItem.Value.ItemTypeId == grenadeType.GetItemType());

        /// <summary>
        /// Counts how many items of a certain <see cref="ItemCategory"/> a player has.
        /// </summary>
        /// <param name="category">The category to search for.</param>
        /// <returns>How many items of that <see cref="ItemCategory"/> the player has.</returns>
        public int CountItem(ItemCategory category) => category switch
        {
            ItemCategory.Ammo => Inventory.UserInventory.ReserveAmmo.Where(ammo => ammo.Value > 0).Count(),
            _ => Inventory.UserInventory.Items.Count(tempItem => tempItem.Value.Category == category),
        };

        /// <summary>
        /// Removes an <see cref="Item"/> from the player's inventory.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to remove.</param>
        /// <param name="destroy">Whether or not to destroy the item.</param>
        /// <returns>A value indicating whether or not the <see cref="Item"/> was removed.</returns>
        public bool RemoveItem(Item item, bool destroy = true)
        {
            if (!ItemsValue.Contains(item))
            {
                return false;
            }

            if (!Inventory.UserInventory.Items.ContainsKey(item.Serial))
            {
                ItemsValue.Remove(item);
                return false;
            }

            if (destroy)
            {
                Inventory.ServerRemoveItem(item.Serial, null);
            }
            else
            {
                if (CurrentItem is not null && (CurrentItem.Serial == item.Serial))
                    Inventory.NetworkCurItem = ItemIdentifier.None;

                Inventory.UserInventory.Items.Remove(item.Serial);
                ItemsValue.Remove(item);
                Inventory.SendItemsNextFrame = true;
            }

            return true;
        }

        /// <summary>
        /// Removes all <see cref="Item"/>'s that satisfy the condition from the player's inventory.
        /// </summary>
        /// <param name="predicate">The condition to satisfy.</param>
        /// <param name="destroy">Whether or not to destroy the items.</param>
        /// <returns>Count of a successfully removed <see cref="Item"/>'s.</returns>
        public int RemoveItem(Func<Item, bool> predicate, bool destroy = true)
        {
            List<Item> enumeratedItems = ListPool<Item>.Shared.Rent(ItemsValue);
            int count = 0;
            foreach (Item item in enumeratedItems)
            {
                if (predicate(item) && RemoveItem(item, destroy))
                {
                    ++count;
                }
            }

            ListPool<Item>.Shared.Return(enumeratedItems);
            return count;
        }

        /// <summary>
        /// Removes the held <see cref="ItemBase"/> from the player's inventory.
        /// </summary>
        /// <returns>Returns a value indicating whether or not the <see cref="ItemBase"/> was removed.</returns>
        [Obsolete("Use RemoveHeldItem(bool) instead.", true)]
        public bool RemoveHeldItem() => RemoveItem(CurrentItem);

        /// <summary>
        /// Removes the held <see cref="ItemBase"/> from the player's inventory.
        /// </summary>
        /// <param name="destroy">Whether or not to destroy the item.</param>
        /// <returns>Returns a value indicating whether or not the <see cref="ItemBase"/> was removed.</returns>
        public bool RemoveHeldItem(bool destroy = true) => RemoveItem(CurrentItem, destroy);

        /// <summary>
        /// Sends a console message to the player's console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="color">The message color.</param>
        public void SendConsoleMessage(string message, string color) => SendConsoleMessage(this, message, color);

        /// <summary>
        /// Sends a console message to a <see cref="Player"/>.
        /// </summary>
        /// <param name="target">The message target.</param>
        /// <param name="message">The message to be sent.</param>
        /// <param name="color">The message color.</param>
        public void SendConsoleMessage(Player target, string message, string color) =>
            ReferenceHub.characterClassManager.TargetConsolePrint(target.Connection, message, color);

        /// <summary>
        /// Disconnects the player.
        /// </summary>
        /// <param name="reason">The disconnection reason.</param>
        public void Disconnect(string reason = null) =>
            ServerConsole.Disconnect(GameObject, string.IsNullOrEmpty(reason) ? string.Empty : reason);

        /// <summary>
        /// Resets the player's stamina.
        /// </summary>
        public void ResetStamina() => ReferenceHub.fpc.ResetStamina();

        /// <summary>
        /// Hurts the player.
        /// </summary>
        /// <param name="damageHandlerBase">The <see cref="DamageHandlerBase"/> used to deal damage.</param>
        public void Hurt(DamageHandlerBase damageHandlerBase) => ReferenceHub.playerStats.DealDamage(damageHandlerBase);

        /// <summary>
        /// Hurts the player.
        /// </summary>
        /// <param name="attacker">The <see cref="Player"/> attacking player.</param>
        /// <param name="amount">The <see langword="float"/> amount of damage to deal.</param>
        /// <param name="damageType">The <see cref="DamageType"/> of the damage dealt.</param>
        /// <param name="cassieAnnouncement">The <see cref="CustomHandlerBase.CassieAnnouncement"/> cassie announcement to make if the damage kills the player.</param>
        public void Hurt(Player attacker, float amount, DamageType damageType = DamageType.Unknown, CassieAnnouncement cassieAnnouncement = null) =>
            Hurt(new GenericDamageHandler(this, attacker, amount, damageType, cassieAnnouncement));

        /// <summary>
        /// Hurts the player.
        /// </summary>
        /// <param name="attacker">The <see cref="Player"/> attacking player.</param>
        /// <param name="amount">The <see langword="float"/> amount of damage to deal.</param>
        /// <param name="damageType">The <see cref="DamageType"/> of the damage dealt.</param>
        /// <param name="cassieAnnouncement">The <see cref="CustomHandlerBase.CassieAnnouncement"/> cassie announcement to make if the damage kills the player.</param>
        /// <param name="deathText"> The <see langword="string"/> death text to appear on <see cref="Player"/> screen. </param>
        public void Hurt(Player attacker, float amount, DamageType damageType = DamageType.Unknown, CassieAnnouncement cassieAnnouncement = null, string deathText = null) =>
            Hurt(new GenericDamageHandler(this, attacker, amount, damageType, cassieAnnouncement, deathText));

        /// <summary>
        /// Hurts the player.
        /// </summary>
        /// <param name="attacker">The <see cref="Player"/> attacking player.</param>
        /// <param name="damage">The <see langword="float"/> amount of damage to deal.</param>
        /// <param name="force">The throw force.</param>
        /// <param name="armorPenetration">The armor penetration amount.</param>
        public void Hurt(Player attacker, float damage, Vector3 force = default, int armorPenetration = 0) =>
            Hurt(new ExplosionDamageHandler(attacker.Footprint, force, damage, armorPenetration));

        /// <summary>
        /// Hurts the player.
        /// </summary>
        /// <param name="amount">The <see langword="float"/> amount of damage to deal.</param>
        /// <param name="damageType">The <see cref="DamageType"/> of the damage dealt.</param>
        /// <param name="cassieAnnouncement">The <see langword="string"/> cassie announcement to make if the damage kills the player.</param>
        public void Hurt(float amount, DamageType damageType = DamageType.Unknown, string cassieAnnouncement = "") =>
            Hurt(new CustomReasonDamageHandler(DamageTypeExtensions.TranslationConversion.FirstOrDefault(k => k.Value == damageType).Key.LogLabel, amount, cassieAnnouncement));

        /// <summary>
        /// Hurts the player.
        /// </summary>
        /// <param name="damage">The amount of damage to deal.</param>
        /// <param name="damageReason"> The reason for the damage being dealt.</param>
        /// <param name="cassieAnnouncement">The cassie announcement to make.</param>
        public void Hurt(float damage, string damageReason, string cassieAnnouncement = "") =>
            Hurt(new CustomReasonDamageHandler(damageReason, damage, cassieAnnouncement));

        /// <summary>
        /// Heals the player.
        /// </summary>
        /// <param name="amount">The amount of health to heal.</param>
        /// <param name="overrideMaxHealth">Whether or not healing should exceed their max health.</param>
        public void Heal(float amount, bool overrideMaxHealth = false)
        {
            if (!overrideMaxHealth)
                ((HealthStat)ReferenceHub.playerStats.StatModules[0]).ServerHeal(amount);
            else
                Health += amount;
        }

        /// <summary>
        /// Forces the player to use an item.
        /// </summary>
        /// <param name="usableItem">The ItemType to be used.</param>
        public void UseItem(ItemType usableItem) => UseItem(Item.Create(usableItem));

        /// <summary>
        /// Forces the player to use an item.
        /// </summary>
        /// <param name="item">The item to be used.</param>
        public void UseItem(Item item)
        {
            if (item is not Usable usableItem)
                throw new Exception($"The provided item [{item.Type}] is not a usable item.");

            usableItem.Base.Owner = referenceHub;
            usableItem.Base.ServerOnUsingCompleted();
            if (usableItem.Base is not null)
                usableItem.Destroy();
        }

        /// <summary>
        /// Kills the player.
        /// </summary>
        /// <param name="damageType">The <see cref="DamageType"/> the player has been killed.</param>
        /// <param name="cassieAnnouncement">The cassie announcement to make upon death.</param>
        public void Kill(DamageType damageType, string cassieAnnouncement = "")
        {
            if ((Role.Side != Side.Scp) && !string.IsNullOrEmpty(cassieAnnouncement))
                Cassie.Message(cassieAnnouncement);

            ReferenceHub.playerStats.KillPlayer(new CustomReasonDamageHandler(DamageTypeExtensions.TranslationConversion.FirstOrDefault(k => k.Value == damageType).Key.LogLabel, float.MaxValue, cassieAnnouncement));
        }

        /// <summary>
        /// Kills the player.
        /// </summary>
        /// <param name="deathReason">The reason the player has been killed.</param>
        /// <param name="cassieAnnouncement">The cassie announcement to make upon death.</param>
        public void Kill(string deathReason, string cassieAnnouncement = "")
        {
            if ((Role.Side != Side.Scp) && !string.IsNullOrEmpty(cassieAnnouncement))
                Cassie.Message(cassieAnnouncement);

            ReferenceHub.playerStats.KillPlayer(new CustomReasonDamageHandler(deathReason, float.MaxValue, cassieAnnouncement));
        }

        /// <summary>
        /// Bans the player.
        /// </summary>
        /// <param name="duration">The ban duration.</param>
        /// <param name="reason">The ban reason.</param>
        /// <param name="issuer">The ban issuer nickname.</param>
        public void Ban(int duration, string reason, string issuer = "Console") =>
            Server.BanPlayer.BanUser(GameObject, duration, reason, issuer, false);

        /// <summary>
        /// Kicks the player.
        /// </summary>
        /// <param name="reason">The kick reason.</param>
        /// <param name="issuer">The kick issuer nickname.</param>
        public void Kick(string reason, string issuer = "Console") => Ban(0, reason, issuer);

        /// <summary>
        /// Persistently mutes the player. For temporary mutes, see <see cref="Player.IsMuted"/> and <see cref="Player.IsIntercomMuted"/>.
        /// </summary>
        /// <param name="intercom">Whether or not this mute is for the intercom only.</param>
        public void Mute(bool intercom = false) =>
            MuteHandler.IssuePersistentMute(intercom ? "ICOM-" + UserId : UserId);

        /// <summary>
        /// Revokes a persistent mute. For temporary mutes, see <see cref="Player.IsMuted"/> and <see cref="Player.IsIntercomMuted"/>.
        /// </summary>
        /// <param name="intercom">Whether or not this un-mute is for the intercom only.</param>
        public void UnMute(bool intercom = false) =>
            MuteHandler.RevokePersistentMute(intercom ? "ICOM-" + UserId : UserId);

        /// <summary>
        /// Blink the player's tag.
        /// </summary>
        /// <returns>Used to wait.</returns>
        public IEnumerator<float> BlinkTag()
        {
            yield return Timing.WaitForOneFrame;

            BadgeHidden = !BadgeHidden;

            yield return Timing.WaitForOneFrame;

            BadgeHidden = !BadgeHidden;
        }

        /// <summary>
        /// Sends a message to the player's Remote Admin console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="success">Indicates whether or not the message should be highlighted as success.</param>
        /// <param name="pluginName">The plugin name.</param>
        public void RemoteAdminMessage(string message, bool success = true, string pluginName = null)
        {
            Sender.RaReply((pluginName ?? Assembly.GetCallingAssembly().GetName().Name) + "#" + message, success, true, string.Empty);
        }

        /// <summary>
        /// Shows a broadcast to the player. Doesn't get logged to the console and can be monospaced.
        /// </summary>
        /// <param name="duration">The broadcast duration.</param>
        /// <param name="message">The message to be broadcasted.</param>
        /// <param name="type">The broadcast type.</param>
        /// <param name="shouldClearPrevious">Clears all player's broadcasts before sending the new one.</param>
        public void Broadcast(ushort duration, string message, global::Broadcast.BroadcastFlags type = global::Broadcast.BroadcastFlags.Normal, bool shouldClearPrevious = false)
        {
            if (shouldClearPrevious)
                ClearBroadcasts();

            Server.Broadcast.TargetAddElement(Connection, message, duration, type);
        }

        /// <summary>
        /// Clears the player's brodcast. Doesn't get logged to the console.
        /// </summary>
        public void ClearBroadcasts() => Server.Broadcast.TargetClearElements(Connection);

        /// <summary>
        /// Adds the amount of a specified <see cref="AmmoType">ammo type</see> to the player's inventory.
        /// </summary>
        /// <param name="ammoType">The <see cref="AmmoType"/> to be added.</param>
        /// <param name="amount">The amount of ammo to be added.</param>
        public void AddAmmo(AmmoType ammoType, ushort amount) =>
            Inventory.ServerAddAmmo(ammoType.GetItemType(), amount);

        /// <summary>
        /// Adds the amount of a weapon's <see cref="AmmoType">ammo type</see> to the player's inventory.
        /// </summary>
        /// <param name="weaponType">The <see cref="ItemType"/> of the weapon.</param>
        /// <param name="amount">The amount of ammo to be added.</param>
        public void AddAmmo(ItemType weaponType, ushort amount) => AddAmmo(weaponType.GetWeaponAmmoType(), amount);

        /// <summary>
        /// Sets the amount of a specified <see cref="AmmoType">ammo type</see> to the player's inventory.
        /// </summary>
        /// <param name="ammoType">The <see cref="AmmoType"/> to be set.</param>
        /// <param name="amount">The amount of ammo to be set.</param>
        public void SetAmmo(AmmoType ammoType, ushort amount) =>
            Inventory.ServerSetAmmo(ammoType.GetItemType(), amount);

        /// <summary>
        /// Gets the ammo count of a specified <see cref="AmmoType">ammo type</see> in a player's inventory.
        /// </summary>
        /// <param name="ammoType">The <see cref="AmmoType"/> to be searched for in the player's inventory.</param>
        /// <returns>The specified <see cref="AmmoType">ammo</see> count.</returns>
        public ushort GetAmmo(AmmoType ammoType) => Inventory.GetCurAmmo(ammoType.GetItemType());

        /// <summary>
        /// Drops a specific <see cref="AmmoType"/> out of the player's inventory.
        /// </summary>
        /// <param name="ammoType">The <see cref="AmmoType"/> that will be dropped.</param>
        /// <param name="amount">The amount of ammo that will be dropped.</param>
        /// <param name="checkMinimals">Whether or not ammo limits will be taken into consideration.</param>
        /// <returns><see langword="true"/> if ammo was dropped; otherwise, <see langword="false"/>.</returns>
        public bool DropAmmo(AmmoType ammoType, ushort amount, bool checkMinimals = false) =>
            Inventory.ServerDropAmmo(ammoType.GetItemType(), amount, checkMinimals);

        /// <summary>
        /// Gets the maximum amount of ammo the player can hold, given the ammo <see cref="AmmoType"/>.
        /// This method factors in the armor the player is wearing, as well as server configuration.
        /// For the maximum amount of ammo that can be given regardless of worn armor and server configuration, see <see cref="Features.Items.Ammo.AmmoLimit"/>.
        /// </summary>
        /// <param name="type">The <see cref="AmmoType"/> of the ammo to check.</param>
        /// <returns>The maximum amount of ammo this player can carry. Guaranteed to be between <c>0</c> and <see cref="Features.Items.Ammo.AmmoLimit"/>.</returns>
        public int GetAmmoLimit(AmmoType type) =>
            InventorySystem.Configs.InventoryLimits.GetAmmoLimit(type.GetItemType(), referenceHub);

        /// <summary>
        /// Gets the maximum amount of an <see cref="ItemCategory"/> the player can hold, based on the armor the player is wearing, as well as server configuration.
        /// </summary>
        /// <param name="category">The <see cref="ItemCategory"/> to check.</param>
        /// <returns>The maximum amount of items in the category that the player can hold.</returns>
        public int GetCategoryLimit(ItemCategory category) =>
            InventorySystem.Configs.InventoryLimits.GetCategoryLimit(category, referenceHub);

        /// <summary>
        /// Add an item of the specified type with default durability(ammo/charge) and no mods to the player's inventory.
        /// </summary>
        /// <param name="itemType">The item to be added.</param>
        /// <param name="identifiers">The attachments to be added to the item.</param>
        /// <returns>The <see cref="Item"/> given to the player.</returns>
        public Item AddItem(ItemType itemType, IEnumerable<AttachmentIdentifier> identifiers = null)
        {
            Item item = Item.Get(Inventory.ServerAddItem(itemType));
            if (item is Firearm firearm)
            {
                if (identifiers is not null)
                {
                    firearm.AddAttachment(identifiers);
                }
                else if (Preferences is not null && Preferences.TryGetValue(itemType, out AttachmentIdentifier[] attachments))
                {
                    firearm.Base.ApplyAttachmentsCode(attachments.GetAttachmentsCode(), true);
                }

                FirearmStatusFlags flags = FirearmStatusFlags.MagazineInserted;
                if (firearm.Attachments.Any(a => a.Name == AttachmentName.Flashlight))
                    flags |= FirearmStatusFlags.FlashlightEnabled;
                firearm.Base.Status =
                    new FirearmStatus(firearm.MaxAmmo, flags, firearm.Base.GetCurrentAttachmentsCode());
            }

            return item;
        }

        /// <summary>
        /// Add the amount of items of the specified type with default durability(ammo/charge) and no mods to the player's inventory.
        /// </summary>
        /// <param name="itemType">The item to be added.</param>
        /// <param name="amount">The amount of items to be added.</param>
        /// <returns>An <see cref="IEnumerable{Item}"/> containing the items given.</returns>
        public IEnumerable<Item> AddItem(ItemType itemType, int amount)
        {
            List<Item> items = new(amount > 0 ? amount : 0);
            if (amount > 0)
            {
                for (int i = 0; i < amount; i++)
                    items.Add(AddItem(itemType));
            }

            return items;
        }

        /// <summary>
        /// Add the amount of items of the specified type with default durability(ammo/charge) and no mods to the player's inventory.
        /// </summary>
        /// <param name="itemType">The item to be added.</param>
        /// <param name="amount">The amount of items to be added.</param>
        /// <param name="identifiers">The attachments to be added to the item.</param>
        /// <returns>An <see cref="IEnumerable{Item}"/> containing the items given.</returns>
        public IEnumerable<Item> AddItem(ItemType itemType, int amount, IEnumerable<AttachmentIdentifier> identifiers)
        {
            List<Item> items = new(amount > 0 ? amount : 0);
            if (amount > 0)
            {
                IEnumerable<AttachmentIdentifier> attachmentIdentifiers = identifiers.ToList();
                for (int i = 0; i < amount; i++)
                {
                    items.Add(AddItem(itemType, attachmentIdentifiers));
                }
            }

            return items;
        }

        /// <summary>
        /// Add the list of items of the specified type with default durability(ammo/charge) and no mods to the player's inventory.
        /// </summary>
        /// <param name="items">The list of items to be added.</param>
        /// <returns>An <see cref="IEnumerable{Item}"/> containing the items given.</returns>
        public IEnumerable<Item> AddItem(IEnumerable<ItemType> items)
        {
            List<ItemType> enumeratedItems = ListPool<ItemType>.Shared.Rent(items);
            List<Item> returnedItems = new(enumeratedItems.Count);

            foreach (ItemType type in enumeratedItems)
                returnedItems.Add(AddItem(type));

            ListPool<ItemType>.Shared.Return(enumeratedItems);
            return returnedItems;
        }

        /// <summary>
        /// Add the list of items of the specified type with default durability(ammo/charge) and no mods to the player's inventory.
        /// </summary>
        /// <param name="items">The <see cref="Dictionary{TKey, TValue}"/> of <see cref="ItemType"/> and <see cref="IEnumerable{T}"/> of <see cref="AttachmentIdentifier"/> to be added.</param>
        /// <returns>An <see cref="IEnumerable{Item}"/> containing the items given.</returns>
        public IEnumerable<Item> AddItem(Dictionary<ItemType, IEnumerable<AttachmentIdentifier>> items)
        {
            List<Item> returnedItems = new(items.Count);

            foreach (KeyValuePair<ItemType, IEnumerable<AttachmentIdentifier>> item in items)
                returnedItems.Add(AddItem(item.Key, item.Value));

            return returnedItems;
        }

        /// <summary>
        /// Add an item to the player's inventory.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        public void AddItem(Item item)
        {
            try
            {
                if (item.Base is null)
                    item = new Item(item.Type);

                AddItem(item.Base, item);
            }
            catch (Exception e)
            {
                Log.Error($"{nameof(Player)}.{nameof(AddItem)}(Item): {e}");
            }
        }

        /// <summary>
        /// Add an item to the player's inventory.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        /// <param name="identifiers">The attachments to be added to the item.</param>
        public void AddItem(Item item, IEnumerable<AttachmentIdentifier> identifiers)
        {
            try
            {
                if (item.Base is null)
                    item = new Item(item.Type);

                if (item is Firearm firearm && identifiers is not null)
                    firearm.AddAttachment(identifiers);

                AddItem(item.Base, item);
            }
            catch (Exception e)
            {
                Log.Error($"{nameof(Player)}.{nameof(AddItem)}(Item): {e}");
            }
        }

        /// <summary>
        /// Adds an item to the player's inventory.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> of the item to be added.</param>
        /// <returns>The <see cref="Item"/> that was added.</returns>
        public Item AddItem(Pickup pickup) =>
            Item.Get(Inventory.ServerAddItem(pickup.Type, pickup.Serial, pickup.Base));

        /// <summary>
        /// Adds an item to the player's inventory.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> of the item to be added.</param>
        /// <param name="identifiers">The attachments to be added to <see cref="Pickup"/> of the item.</param>
        /// <returns>The <see cref="Item"/> that was added.</returns>
        public Item AddItem(Pickup pickup, IEnumerable<AttachmentIdentifier> identifiers)
        {
            Item item = Item.Get(Inventory.ServerAddItem(pickup.Type, pickup.Serial, pickup.Base));

            if (item is Firearm firearm && identifiers is not null)
                firearm.AddAttachment(identifiers);

            return item;
        }

        /// <summary>
        /// Add an item to the player's inventory.
        /// </summary>
        /// <param name="itemBase">The item to be added.</param>
        /// <param name="item">The <see cref="Item"/> object of the item.</param>
        /// <returns>The item that was added.</returns>
        public Item AddItem(ItemBase itemBase, Item item = null)
        {
            try
            {
                if (item is null)
                    item = Item.Get(itemBase);

                int ammo = -1;
                if (item is Firearm firearm1)
                {
                    ammo = firearm1.Ammo;
                }

                itemBase.Owner = ReferenceHub;
                Inventory.UserInventory.Items[item.Serial] = itemBase;
                if (itemBase.PickupDropModel is not null)
                {
                    itemBase.OnAdded(itemBase.PickupDropModel);
                }

                if (itemBase is InventorySystem.Items.Firearms.Firearm firearm)
                {
                    if (Preferences is not null && Preferences.TryGetValue(firearm.ItemTypeId, out AttachmentIdentifier[] attachments))
                    {
                        firearm.ApplyAttachmentsCode(attachments.GetAttachmentsCode(), true);
                    }

                    FirearmStatusFlags flags = FirearmStatusFlags.MagazineInserted;
                    if (firearm.Attachments.Any(a => a.Name == AttachmentName.Flashlight))
                        flags |= FirearmStatusFlags.FlashlightEnabled;
                    firearm.Status = new FirearmStatus(ammo > -1 ? (byte)ammo : firearm.AmmoManagerModule.MaxAmmo, flags, firearm.GetCurrentAttachmentsCode());
                }

                if (itemBase is IAcquisitionConfirmationTrigger acquisitionConfirmationTrigger)
                {
                    acquisitionConfirmationTrigger.ServerConfirmAcqusition();
                }

                ItemsValue.Add(item);

                Inventory.SendItemsNextFrame = true;
                return item;
            }
            catch (Exception e)
            {
                Log.Error($"{nameof(Player)}.{nameof(AddItem)}(ItemBase, [Item]): {e}");
            }

            return null;
        }

        /// <summary>
        /// Add the amount of items to the player's inventory.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        /// <param name="amount">The amount of items to be added.</param>
        public void AddItem(Item item, int amount)
        {
            if (amount > 0)
            {
                for (int i = 0; i < amount; i++)
                    AddItem(item);
            }
        }

        /// <summary>
        /// Add the amount of items to the player's inventory.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        /// <param name="amount">The amount of items to be added.</param>
        /// <param name="identifiers">The attachments to be added to the item.</param>
        public void AddItem(Item item, int amount, IEnumerable<AttachmentIdentifier> identifiers)
        {
            if (amount > 0)
            {
                for (int i = 0; i < amount; i++)
                    AddItem(item, identifiers);
            }
        }

        /// <summary>
        /// Add the list of items to the player's inventory.
        /// </summary>
        /// <param name="items">The list of items to be added.</param>
        public void AddItem(IEnumerable<Item> items)
        {
            foreach (Item item in items)
                AddItem(item);
        }

        /// <summary>
        /// Add the list of items to the player's inventory.
        /// </summary>
        /// <param name="items">The <see cref="Dictionary{TKey, TValue}"/> of <see cref="Item"/> and <see cref="IEnumerable{T}"/> of <see cref="AttachmentIdentifier"/> to be added.</param>
        public void AddItem(Dictionary<Item, IEnumerable<AttachmentIdentifier>> items)
        {
            if (items.Count > 0)
            {
                foreach (KeyValuePair<Item, IEnumerable<AttachmentIdentifier>> item in items)
                    AddItem(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Gives the player a specific candy. Will give the player a bag if they do not already have one.
        /// </summary>
        /// <param name="candyType">The <see cref="CandyKindID"/> to give.</param>
        /// <returns><see langword="true"/> if a candy was given.</returns>
        public bool TryAddCandy(CandyKindID candyType)
        {
            if (Scp330Bag.TryGetBag(ReferenceHub, out Scp330Bag bag))
            {
                bool flag = bag.TryAddSpecific(candyType);
                if (flag)
                    bag.ServerRefreshBag();
                return flag;
            }

            if (Items.Count > 7)
                return false;

            Scp330 scp330 = (Scp330)AddItem(ItemType.SCP330);
            Timing.CallDelayed(
                0.02f,
                () =>
                {
                    scp330.Base.Candies.Clear();
                    scp330.AddCandy(candyType);
                });

            return true;
        }

        /// <summary>
        /// Resets the player's inventory to the provided list of items, clearing any items it already possess.
        /// </summary>
        /// <param name="newItems">The new items that have to be added to the inventory.</param>
        public void ResetInventory(IEnumerable<ItemType> newItems)
        {
            ClearInventory();

            Timing.CallDelayed(
                0.5f,
                () =>
                {
                    if (newItems.IsEmpty())
                        return;

                    foreach (ItemType item in newItems)
                        AddItem(item);
                });
        }

        /// <summary>
        /// Resets the player's inventory to the provided list of items, clearing any items it already possess.
        /// </summary>
        /// <param name="newItems">The new items that have to be added to the inventory.</param>
        public void ResetInventory(IEnumerable<Item> newItems)
        {
            ClearInventory();

            if (newItems.Any())
            {
                foreach (Item item in newItems)
                {
                    AddItem(item.Base is null ? new Item(item.Type) : item);
                }
            }
        }

        /// <summary>
        /// Clears the player's inventory, including all ammo and items.
        /// </summary>
        /// <param name="destroy">Whether or not to fully destroy the old items.</param>
        public void ClearInventory(bool destroy = true)
        {
            while (Items.Count > 0)
                RemoveItem(Items.ElementAt(0), destroy);
        }

        /// <summary>
        /// Drops all items in the player's inventory, including all ammo and items.
        /// </summary>
        public void DropItems() => Inventory.ServerDropEverything();

        /// <summary>
        /// Causes the player to throw a grenade.
        /// </summary>
        /// <param name="type">The <see cref="GrenadeType"/> to be thrown.</param>
        /// <param name="fullForce">Whether to throw with full or half force.</param>
        /// <returns>The <see cref="Throwable"/> item that was spawned.</returns>
        public Throwable ThrowGrenade(GrenadeType type, bool fullForce = true)
        {
            Throwable throwable = type switch
            {
                GrenadeType.Flashbang => new FlashGrenade(),
                GrenadeType.Scp2176 => new Scp2176(),
                _ => new ExplosiveGrenade(type.GetItemType()),
            };
            ThrowItem(throwable, fullForce);
            return throwable;
        }

        /// <summary>
        /// Throw an item.
        /// </summary>
        /// <param name="throwable">The <see cref="Throwable"/> to be thrown.</param>
        /// <param name="fullForce">Whether to throw with full or half force.</param>
        public void ThrowItem(Throwable throwable, bool fullForce = true)
        {
            throwable.Base.Owner = ReferenceHub;
            throwable.Throw(fullForce);
        }

        /// <summary>
        /// Show a hint to the player.
        /// </summary>
        /// <param name="message">The message to be shown.</param>
        /// <param name="duration">The duration the text will be on screen.</param>
        public void ShowHint(string message, float duration = 3f)
        {
            HintParameter[] parameters = new HintParameter[]
            {
                new StringHintParameter(message),
            };

            HintDisplay.Show(new TextHint(message, parameters, null, duration));
        }

        /// <summary>
        /// Sends a HitMarker to the player.
        /// </summary>
        /// <param name="size">The size of the hitmarker (Do not exceed <see cref="Hitmarker.MaxSize"/>).</param>
        public void ShowHitMarker(float size = 1f) =>
            Hitmarker.SendHitmarker(Connection, size > Hitmarker.MaxSize ? Hitmarker.MaxSize : size);

        /// <summary>
        /// Safely gets an <see cref="object"/> from <see cref="SessionVariables"/>, then casts it to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The returned object type.</typeparam>
        /// <param name="key">The key of the object to get.</param>
        /// <param name="result">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter is used.</param>
        /// <returns><see langword="true"/> if the SessionVariables contains an element with the specified key; otherwise, <see langword="false"/>.</returns>
        public bool TryGetSessionVariable<T>(string key, out T result)
        {
            if (SessionVariables.TryGetValue(key, out object value) && value is T type)
            {
                result = type;
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Gets a <see cref="StatBase"/> module from the player's <see cref="PlayerStats"/> component.
        /// </summary>
        /// <typeparam name="T">The returned object type.</typeparam>
        /// <returns>The <typeparamref name="T"/> module that was requested.</returns>
        public T GetModule<T>()
            where T : StatBase
            => ReferenceHub.playerStats.GetModule<T>();

        /// <summary>
        /// Gets a <see cref="bool"/> describing whether or not the given <see cref="PlayerEffect">status effect</see> is currently enabled.
        /// </summary>
        /// <typeparam name="T">The <see cref="PlayerEffect"/> to check.</typeparam>
        /// <returns>A <see cref="bool"/> determining whether or not the player effect is active.</returns>
        public bool GetEffectActive<T>()
            where T : PlayerEffect
        {
            if (ReferenceHub.playerEffectsController.AllEffects.TryGetValue(typeof(T), out PlayerEffect playerEffect))
                return playerEffect.IsEnabled;

            return false;
        }

        /// <summary>
        /// Disables all currently active <see cref="PlayerEffect">status effects</see>.
        /// </summary>
        public void DisableAllEffects()
        {
            foreach (KeyValuePair<Type, PlayerEffect> effect in ReferenceHub.playerEffectsController.AllEffects)
            {
                effect.Value.IsEnabled = false;
            }
        }

        /// <summary>
        /// Disables all currently active <see cref="PlayerEffect">status effects</see>.
        /// </summary>
        /// <param name="category">A category to filter the disabled effects.</param>
        public void DisableAllEffects(EffectCategory category)
        {
            if (category is EffectCategory.None)
                return;

            foreach (KeyValuePair<Type, PlayerEffect> effect in ReferenceHub.playerEffectsController.AllEffects)
            {
                if (Enum.TryParse(effect.Key.Name, out EffectType effectType) && effectType.GetCategories().HasFlag(category))
                {
                    effect.Value.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Disables a specific <see cref="PlayerEffect">status effect</see> on the player.
        /// </summary>
        /// <typeparam name="T">The <see cref="PlayerEffect"/> to disable.</typeparam>
        public void DisableEffect<T>()
            where T : PlayerEffect => ReferenceHub.playerEffectsController.DisableEffect<T>();

        /// <summary>
        /// Disables a specific <see cref="EffectType">status effect</see> on the player.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/> to disable.</param>
        public void DisableEffect(EffectType effect)
        {
            if (TryGetEffect(effect, out PlayerEffect playerEffect))
                playerEffect.IsEnabled = false;
        }

        /// <summary>
        /// Disables a <see cref="IEnumerable{T}"/> of <see cref="EffectType"/> on the player.
        /// </summary>
        /// <param name="effects">The <see cref="IEnumerable{T}"/> of <see cref="EffectType"/> to disable.</param>
        public void DisableEffects(IEnumerable<EffectType> effects)
        {
            foreach (EffectType effect in effects)
                DisableEffect(effect);
        }

        /// <summary>
        /// Enables a <see cref="PlayerEffect">status effect</see> on the player.
        /// </summary>
        /// <typeparam name="T">The <see cref="PlayerEffect"/> to enable.</typeparam>
        /// <param name="duration">The amount of time the effect will be active for.</param>
        /// <param name="addDurationIfActive">If the effect is already active, setting to <see langword="true"/> will add this duration onto the effect.</param>
        public void EnableEffect<T>(float duration = 0f, bool addDurationIfActive = false)
            where T : PlayerEffect =>
            ReferenceHub.playerEffectsController.EnableEffect<T>(duration, addDurationIfActive);

        /// <summary>
        /// Enables a <see cref="PlayerEffect">status effect</see> on the player.
        /// </summary>
        /// <param name="effect">The name of the <see cref="PlayerEffect"/> to enable.</param>
        /// <param name="duration">The amount of time the effect will be active for.</param>
        /// <param name="addDurationIfActive">If the effect is already active, setting to <see langword="true"/> will add this duration onto the effect.</param>
        public void EnableEffect(PlayerEffect effect, float duration = 0f, bool addDurationIfActive = false)
            => ReferenceHub.playerEffectsController.EnableEffect(effect, duration, addDurationIfActive);

        /// <summary>
        /// Enables a <see cref="PlayerEffect">status effect</see> on the player.
        /// </summary>
        /// <param name="effect">The name of the <see cref="PlayerEffect"/> to enable.</param>
        /// <param name="duration">The amount of time the effect will be active for.</param>
        /// <param name="addDurationIfActive">If the effect is already active, setting to <see langword="true"/> will add this duration onto the effect.</param>
        /// <returns>A bool indicating whether or not the effect was valid and successfully enabled.</returns>
        public bool EnableEffect(string effect, float duration = 0f, bool addDurationIfActive = false)
            => ReferenceHub.playerEffectsController.EnableByString(effect, duration, addDurationIfActive);

        /// <summary>
        /// Enables a <see cref="EffectType">status effect</see> on the player.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/> to enable.</param>
        /// <param name="duration">The amount of time the effect will be active for.</param>
        /// <param name="addDurationIfActive">If the effect is already active, setting to <see langword="true"/> will add this duration onto the effect.</param>
        public void EnableEffect(EffectType effect, float duration = 0f, bool addDurationIfActive = false)
        {
            if (TryGetEffect(effect, out PlayerEffect pEffect))
                ReferenceHub.playerEffectsController.EnableEffect(pEffect, duration, addDurationIfActive);
        }

        /// <summary>
        /// Enables a random <see cref="EffectType"/> on the player.
        /// </summary>
        /// <param name="category">An optional category to filter the applied effect. Set to <see cref="EffectCategory.None"/> for any effect.</param>
        /// <param name="duration">The amount of time the effect will be active for.</param>
        /// <param name="addDurationIfActive">If the effect is already active, setting to <see langword="true"/> will add this duration onto the effect.</param>
        /// <returns>A <see cref="EffectType"/> that was given to the player.</returns>
        public EffectType ApplyRandomEffect(EffectCategory category = EffectCategory.None, float duration = 0f, bool addDurationIfActive = false)
        {
            Array effectTypes = Enum.GetValues(typeof(EffectType));
            IEnumerable<EffectType> validEffects = effectTypes.ToArray<EffectType>().Where(effect => effect.GetCategories().HasFlag(category));
            EffectType effectType = validEffects.ElementAt(Random.Range(0, effectTypes.Length));
            EnableEffect(effectType, duration, addDurationIfActive);
            return effectType;
        }

        /// <summary>
        /// Enables a <see cref="IEnumerable{T}"/> of <see cref="EffectType"/> on the player.
        /// </summary>
        /// <param name="effects">The <see cref="IEnumerable{T}"/> of <see cref="EffectType"/> to enable.</param>
        /// <param name="duration">The amount of time the effects will be active for.</param>
        /// <param name="addDurationIfActive">If an effect is already active, setting to <see langword="true"/> will add this duration onto the effect.</param>
        public void EnableEffects(IEnumerable<EffectType> effects, float duration = 0f, bool addDurationIfActive = false)
        {
            foreach (EffectType effect in effects)
            {
                if (TryGetEffect(effect, out PlayerEffect pEffect))
                    EnableEffect(pEffect, duration, addDurationIfActive);
            }
        }

        /// <summary>
        /// Gets an instance of <see cref="PlayerEffect"/> by <see cref="EffectType"/>.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/>.</param>
        /// <returns>The <see cref="PlayerEffect"/>.</returns>
        public PlayerEffect GetEffect(EffectType effect)
        {
            ReferenceHub.playerEffectsController.AllEffects.TryGetValue(effect.Type(), out PlayerEffect playerEffect);

            return playerEffect;
        }

        /// <summary>
        /// Tries to get an instance of <see cref="PlayerEffect"/> by <see cref="EffectType"/>.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/>.</param>
        /// <param name="playerEffect">The <see cref="PlayerEffect"/>.</param>
        /// <returns>A bool indicating whether or not the <paramref name="playerEffect"/> was successfully gotten.</returns>
        public bool TryGetEffect(EffectType effect, out PlayerEffect playerEffect)
        {
            playerEffect = GetEffect(effect);

            return playerEffect is not null;
        }

        /// <summary>
        /// Gets a <see cref="byte"/> indicating the intensity of the given <see cref="PlayerEffect">status effect</see>.
        /// </summary>
        /// <typeparam name="T">The <see cref="PlayerEffect"/> to check.</typeparam>
        /// <exception cref="ArgumentException">Thrown if the given type is not a valid <see cref="PlayerEffect"/>.</exception>
        /// <returns>The intensity of the effect.</returns>
        public byte GetEffectIntensity<T>()
            where T : PlayerEffect
        {
            if (ReferenceHub.playerEffectsController.AllEffects.TryGetValue(typeof(T), out PlayerEffect playerEffect))
                return playerEffect.Intensity;

            throw new ArgumentException("The given type is invalid.");
        }

        /// <summary>
        /// Changes the intensity of a <see cref="PlayerEffect">status effect</see>.
        /// </summary>
        /// <typeparam name="T">The <see cref="PlayerEffect"/> to change the intensity of.</typeparam>
        /// <param name="intensity">The intensity of the effect.</param>
        public void ChangeEffectIntensity<T>(byte intensity)
            where T : PlayerEffect => ReferenceHub.playerEffectsController.ChangeEffectIntensity<T>(intensity);

        /// <summary>
        /// Changes the intensity of a <see cref="PlayerEffect"/>.
        /// </summary>
        /// <param name="type">The <see cref="EffectType"/> to change.</param>
        /// <param name="intensity">The new intensity to use.</param>
        /// <param name="duration">The new duration to add to the effect.</param>
        public void ChangeEffectIntensity(EffectType type, byte intensity, float duration = 0)
        {
            if (TryGetEffect(type, out PlayerEffect pEffect))
            {
                pEffect.Intensity = intensity;
                pEffect.ServerChangeDuration(duration, true);
            }
        }

        /// <summary>
        /// Changes the intensity of a <see cref="PlayerEffect">status effect</see>.
        /// </summary>
        /// <param name="effect">The name of the <see cref="PlayerEffect"/> to enable.</param>
        /// <param name="intensity">The intensity of the effect.</param>
        /// <param name="duration">The new length of the effect. Defaults to infinite length.</param>
        public void ChangeEffectIntensity(string effect, byte intensity, float duration = 0) =>
            ReferenceHub.playerEffectsController.ChangeByString(effect, intensity, duration);

        /// <summary>
        /// Opens the report window.
        /// </summary>
        /// <param name="text">The text to send.</param>
        public void OpenReportWindow(string text) => SendConsoleMessage($"[REPORTING] {text}", "white");

        /// <summary>
        /// Places a Tantrum (SCP-173's ability) under the player.
        /// </summary>
        /// <returns>The tantrum's <see cref="GameObject"/>.</returns>
        public GameObject PlaceTantrum() => Map.PlaceTantrum(Position);

        /// <summary>
        /// Gives a new <see cref="AhpStat">to the player</see>.
        /// </summary>
        /// <param name="amount">The amount to give the player.</param>
        /// <param name="limit">The maximum AHP for this stat.</param>
        /// <param name="decay">How much value is lost per second.</param>
        /// <param name="efficacy">Percent of incoming damage absorbed by this stat.</param>
        /// <param name="sustain">The number of seconds to delay the start of the decay.</param>
        /// <param name="persistant">Whether or not the process is removed when the value hits 0.</param>
        public void AddAhp(float amount, float limit = 75f, float decay = 1.2f, float efficacy = 0.7f, float sustain = 0f, bool persistant = false)
        {
            ReferenceHub.playerStats.GetModule<AhpStat>()
                .ServerAddProcess(amount, limit, decay, efficacy, sustain, persistant);
        }

        /// <summary>
        /// Makes noise given a specified distance intensity.
        /// </summary>
        /// <param name="distanceIntensity">The distance from which is able to hear the noise.</param>
        public void MakeNoise(float distanceIntensity) =>
            ReferenceHub.footstepSync._visionController.MakeNoise(distanceIntensity);

        /// <summary>
        /// Reconnects player to the server. Can be used to redirect them to another server on a different port but same IP.
        /// </summary>
        /// <param name="newPort">New port.</param>
        /// <param name="delay">Player reconnection delay.</param>
        /// <param name="reconnect">Whether or not player should be reconnected.</param>
        /// <param name="roundRestartType">Type of round restart.</param>
        public void Reconnect(ushort newPort = 0, float delay = 5, bool reconnect = true, RoundRestartType roundRestartType = RoundRestartType.FullRestart)
        {
            if (newPort != 0)
            {
                if ((newPort == Server.Port) && (roundRestartType == RoundRestartType.RedirectRestart))
                    roundRestartType = RoundRestartType.FullRestart;
                else
                    roundRestartType = RoundRestartType.RedirectRestart;
            }

            Connection.Send(new RoundRestartMessage(roundRestartType, delay, newPort, reconnect, false));
        }

        /// <inheritdoc cref="MirrorExtensions.PlayGunSound(Player, Vector3, ItemType, byte, byte)"/>
        public void PlayGunSound(ItemType type, byte volume, byte audioClipId = 0) =>
            MirrorExtensions.PlayGunSound(this, Position, type, volume, audioClipId);

        /// <inheritdoc cref="Map.PlaceBlood(Vector3, BloodType, float)"/>
        public void PlaceBlood(BloodType type, float multiplier = 1f) => Map.PlaceBlood(Position, type, multiplier);

        /// <inheritdoc cref="Map.GetNearCameras(Vector3, float)"/>
        public IEnumerable<Camera> GetNearCameras(float toleration = 15f) => Map.GetNearCameras(Position, toleration);

        /// <summary>
        /// Teleports the player to the given <see cref="Vector3"/> coordinates.
        /// </summary>
        /// <param name="position">The <see cref="Vector3"/> coordinates to move the player to.</param>
        public void Teleport(Vector3 position) => Position = position;

        /// <summary>
        /// Teleports the player to the given object.
        /// </summary>
        /// <param name="obj">The object to teleport the player to.</param>
        public void Teleport(object obj)
        {
            switch (obj)
            {
                case Camera camera:
                    Teleport(camera.Position + Vector3.down);
                    break;
                case Door door:
                    Teleport(door.Position + Vector3.up);
                    break;
                case DoorType doorType:
                    Teleport(Door.Get(doorType).Position + Vector3.up);
                    break;
                case SpawnLocation sp:
                    Teleport(sp.GetPosition());
                    break;
                case Spawn.SpawnPoint sp:
                    Teleport(sp.Position);
                    break;
                case RoomType roomType:
                    Teleport(Room.Get(roomType).Position + Vector3.up);
                    break;
                case Enums.CameraType cameraType:
                    Teleport(Camera.Get(cameraType).Position);
                    break;
                case ElevatorType elevatorType:
                    Teleport(Lift.Get(elevatorType).Position + Vector3.up);
                    break;
                case Room room:
                    Teleport(room.Position + Vector3.up);
                    break;
                case TeslaGate teslaGate:
                    Teleport(
                        teslaGate.Position + Vector3.up +
                        (teslaGate.Room.Transform.rotation == new Quaternion(0f, 0f, 0f, 1f)
                            ? new Vector3(3, 0, 0)
                            : new Vector3(0, 0, 3)));
                    break;
                case Scp914Controller scp914:
                    Teleport(scp914._knobTransform.position + Vector3.up);
                    break;
                case Player player:
                    Teleport(player.Position);
                    break;
                case Pickup pickup:
                    Teleport(pickup.Position + Vector3.up);
                    break;
                case Ragdoll ragdoll:
                    Teleport(ragdoll.Position + Vector3.up);
                    break;
                case Locker locker:
                    Teleport(locker.transform.position + Vector3.up);
                    break;
                case LockerChamber chamber:
                    Teleport(chamber._spawnpoint.position + Vector3.up);
                    break;
                case Generator generator:
                    Teleport(generator.Position + Vector3.up);
                    break;
                case Window window:
                    Teleport(window.Position + Vector3.up);
                    break;
                case Toys.AdminToy toy:
                    Teleport(toy.Position + Vector3.up);
                    break;
                case Elevator elevator:
                    Teleport(elevator.Position + Vector3.up);
                    break;
                case EActor ea:
                    Teleport(ea.Position + Vector3.up);
                    break;
                case Item item:
                    if (item.Owner is not null)
                        Teleport(item.Owner.Position);
                    else
                        Log.Warn($"{nameof(Teleport)}: {Assembly.GetCallingAssembly().GetName().Name}: Invalid item teleport (item is missing Owner).");
                    break;

                // Unity
                case Vector3 v3: // I wouldn't be surprised if someone calls this method with a Vector3.
                    Teleport(v3);
                    break;
                case Component comp:
                    Teleport(comp.transform.position + Vector3.up);
                    break;
                case GameObject go:
                    Teleport(go.transform.position + Vector3.up);
                    break;

                default:
                    Log.Warn($"{nameof(Teleport)}: {Assembly.GetCallingAssembly().GetName().Name}: Invalid type declared: {obj.GetType()}");
                    break;
            }
        }

        /// <summary>
        /// Teleports player to a random object of a specific type.
        /// </summary>
        /// <param name="type">Object for teleport.</param>
        public void RandomTeleport(Type type)
        {
            LockerChamber[] chambers;

            object randomObject = type.Name switch
            {
                nameof(Camera) => Camera.CamerasValue[Random.Range(0, Camera.CamerasValue.Count)],
                nameof(Door) => Door.Random(),
                nameof(Room) => Room.RoomsValue[Random.Range(0, Room.RoomsValue.Count)],
                nameof(TeslaGate) => TeslaGate.TeslasValue[Random.Range(0, TeslaGate.TeslasValue.Count)],
                nameof(Player) => Dictionary.Values.ElementAt(Random.Range(0, Dictionary.Count)),
                nameof(Pickup) => Map.Pickups[Random.Range(0, Map.Pickups.Count)],
                nameof(Ragdoll) => Map.RagdollsValue[Random.Range(0, Map.RagdollsValue.Count)],
                nameof(Locker) => Map.GetRandomLocker(),
                nameof(Generator) => Generator.GeneratorValues[Random.Range(0, Generator.GeneratorValues.Count)],
                nameof(Window) => Window.WindowValue[Random.Range(0, Window.WindowValue.Count)],
                nameof(Scp914) => Scp914.Scp914Controller,
                nameof(LockerChamber) => (chambers = Map.GetRandomLocker().Chambers)[Random.Range(0, chambers.Length)],
                _ => null,
            };

            Teleport(randomObject);
        }

        /// <summary>
        /// Teleports player to a random object of a specific type.
        /// </summary>
        /// <typeparam name="T">Object for teleport.</typeparam>
        public void RandomTeleport<T>() => RandomTeleport(typeof(T));

        /// <summary>
        /// Adds a component to the player.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="EActor"/> to be added.</typeparam>
        /// <returns>The added <see cref="EActor"/> component.</returns>
        public T AddComponent<T>()
            where T : EActor
        {
            T component = EObject.CreateDefaultSubobject<T>(GameObject);

            if (component is null)
                return null;

            components.Add(component);
            return component;
        }

        /// <summary>
        /// Adds a component to the player.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="EActor"/> to be added.</param>
        /// <returns>The added <see cref="EActor"/> component.</returns>
        public EActor AddComponent(Type type)
        {
            EActor component = EObject.CreateDefaultSubobject(type, GameObject).Cast<EActor>();

            if (component is null)
                return null;

            components.Add(component);
            return component;
        }

        /// <summary>
        /// Adds a component to the player.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> cast <see cref="EActor"/> type.</typeparam>
        /// <param name="type">The <see cref="Type"/> of the <see cref="EActor"/> to be added.</param>
        /// <returns>The added <see cref="EActor"/> component.</returns>
        public T AddComponent<T>(Type type)
            where T : EActor
        {
            T component = EObject.CreateDefaultSubobject<T>(type, GameObject);
            if (component is null)
                return null;

            components.Add(component);
            return component;
        }

        /// <summary>
        /// Gets a component to the player.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="EActor"/> to look for.</typeparam>
        /// <returns>The <see cref="EActor"/> component.</returns>
        public T GetComponent<T>()
            where T : EActor => components.FirstOrDefault(comp => typeof(T) == comp.GetType()).Cast<T>();

        /// <summary>
        /// Gets a component from the player.
        /// </summary>
        /// <typeparam name="T">The cast <typeparamref name="T"/> <see cref="EActor"/>.</typeparam>
        /// <param name="type">The <see cref="Type"/> of the <see cref="EActor"/> to look for.</param>
        /// <returns>The <see cref="EActor"/> component.</returns>
        public T GetComponent<T>(Type type)
            where T : EActor => components.FirstOrDefault(comp => type == comp.GetType()).Cast<T>();

        /// <summary>
        /// Gets a component from the player.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="EActor"/> to look for.</param>
        /// <returns>The <see cref="EActor"/> component.</returns>
        public EActor GetComponent(Type type) => components.FirstOrDefault(comp => type == comp.GetType());

        /// <summary>
        /// Tries to get a component from the player.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="EActor"/> to look for.</typeparam>
        /// <param name="component">The <typeparamref name="T"/> <see cref="EActor"/>.</param>
        /// <returns><see langword="true"/> if the component was found; otherwise, <see langword="false"/>.</returns>
        public bool TryGetComponent<T>(out T component)
            where T : EActor
        {
            component = GetComponent<T>();

            return component is not null;
        }

        /// <summary>
        /// Tries to get a component from the player.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="EActor"/> to get.</param>
        /// <param name="component">The found component.</param>
        /// <returns><see langword="true"/> if the component was found; otherwise, <see langword="false"/>.</returns>
        public bool TryGetComponent(Type type, out EActor component)
        {
            component = GetComponent(type);

            return component is not null;
        }

        /// <summary>
        /// Tries to get a component from the player.
        /// </summary>
        /// <typeparam name="T">The cast <typeparamref name="T"/> <see cref="EActor"/>.</typeparam>
        /// <param name="type">The <see cref="Type"/> of the <see cref="EActor"/> to get.</param>
        /// <param name="component">The found component.</param>
        /// <returns><see langword="true"/> if the component was found; otherwise, <see langword="false"/>.</returns>
        public bool TryGetComponent<T>(Type type, out T component)
            where T : EActor
        {
            component = GetComponent<T>(type);

            return component is not null;
        }

        /// <summary>
        /// Checks if the player has an active component.
        /// </summary>
        /// <typeparam name="T">The <see cref="EActor"/> to look for.</typeparam>
        /// <param name="depthInheritance">A value indicating whether or not subclasses should be considered.</param>
        /// <returns><see langword="true"/> if the component was found; otherwise, <see langword="false"/>.</returns>
        public bool HasComponent<T>(bool depthInheritance = false)
            where T : EActor => depthInheritance
            ? components.Any(comp => typeof(T).IsSubclassOf(comp.GetType()))
            : components.Any(comp => typeof(T) == comp.GetType());

        /// <summary>
        /// Checks if the player has an active component.
        /// </summary>
        /// <param name="type">The <see cref="EActor"/> to look for.</param>
        /// <param name="depthInheritance">A value indicating whether or not subclasses should be considered.</param>
        /// <returns><see langword="true"/> if the component was found; otherwise, <see langword="false"/>.</returns>
        public bool HasComponent(Type type, bool depthInheritance = false) => depthInheritance
            ? components.Any(comp => type.IsSubclassOf(comp.GetType()))
            : components.Any(comp => type == comp.GetType());

        /// <summary>
        /// Teleports the player to a random object.
        /// </summary>
        /// <param name="types">The list of object types to choose from.</param>
        public void RandomTeleport(IEnumerable<Type> types)
        {
            Type[] array = types as Type[] ?? types.ToArray();
            if (array.Length == 0)
                return;
            RandomTeleport(array[Random.Range(0, array.Length)]);
        }

        /// <summary>
        /// Resets the player speed.
        /// </summary>
        public void ResetSpeed()
        {
            WalkingSpeed = ServerConfigSynchronizer.Singleton.NetworkHumanWalkSpeedMultiplier;
            RunningSpeed = ServerConfigSynchronizer.Singleton.NetworkHumanSprintSpeedMultiplier;
        }

        /// <summary>
        /// Converts the player in a human-readable format.
        /// </summary>
        /// <returns>A string containing Player-related data.</returns>
        public override string ToString() => $"{Id} ({Nickname}) [{UserId}] *{(Role is null ? "No role" : Role)}*";
    }
}