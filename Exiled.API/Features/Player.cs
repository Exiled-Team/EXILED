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
    using System.Text;

    using Core;
    using CustomPlayerEffects;
    using CustomPlayerEffects.Danger;
    using DamageHandlers;
    using Enums;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core.Attributes;
    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.API.Features.CustomStats;
    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Hazards;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Lockers;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Roles;
    using Exiled.API.Interfaces;
    using Exiled.API.Structs;
    using Extensions;
    using Footprinting;
    using global::Scp914;
    using Hints;
    using Interactables.Interobjects;
    using InventorySystem;
    using InventorySystem.Disarming;
    using InventorySystem.Items;
    using InventorySystem.Items.Armor;
    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Firearms.BasicMessages;
    using InventorySystem.Items.Usables;
    using InventorySystem.Items.Usables.Scp330;
    using MEC;
    using Mirror;
    using Mirror.LiteNetLib4Mirror;
    using NorthwoodLib;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.PlayableScps;
    using PlayerRoles.PlayableScps.Scp049.Zombies;
    using PlayerRoles.RoleAssign;
    using PlayerRoles.Spectating;
    using PlayerRoles.Voice;
    using PlayerStatsSystem;
    using PluginAPI.Core;
    using RelativePositioning;
    using RemoteAdmin;
    using Respawning;
    using Respawning.NamingRules;
    using RoundRestarting;
    using UnityEngine;
    using Utils;
    using Utils.Networking;
    using VoiceChat;
    using VoiceChat.Playbacks;

    using static DamageHandlers.DamageHandlerBase;
    using static global::Broadcast;

    using DamageHandlerBase = PlayerStatsSystem.DamageHandlerBase;
    using Firearm = Items.Firearm;
    using FirearmPickup = Pickups.FirearmPickup;
    using HumanRole = Roles.HumanRole;
    using PHumanRole = PlayerRoles.HumanRole;

    /// <summary>
    /// Represents the in-game player, by encapsulating a <see cref="global::ReferenceHub"/>.
    /// </summary>
    [EClass(category: nameof(Player))]
    [DefaultPlayerClass]
    public class Player : GameEntity
    {
#pragma warning disable SA1310
        /// <summary>
        /// Category for player roles.
        /// </summary>
        public const string ROLES_CATEGORY = "Player_Roles";

        /// <summary>
        /// Category for player stats.
        /// </summary>
        public const string STATS_CATEGORY = "Player_Roles";

        /// <summary>
        /// Category for player movement.
        /// </summary>
        public const string MOVEMENT_CATEGORY = "Player_Movement";

        /// <summary>
        /// Category for player effects.
        /// </summary>
        public const string EFFECTS_CATEGORY = "Player_Effects";

        /// <summary>
        /// Category for player world space.
        /// </summary>
        public const string WORLDSPACE_CATEGORY = "Player_WorldSpace";

        /// <summary>
        /// Category for player inventory.
        /// </summary>
        public const string INVENTORY_CATEGORY = "Player_Inventory";

        /// <summary>
        /// Category for player hints.
        /// </summary>
        public const string HINTS_CATEGORY = "Player_Hints";

        /// <summary>
        /// Category for general player information.
        /// </summary>
        public const string INFO_CATEGORY = "Player_Info";

#pragma warning disable SA1401
        /// <summary>
        /// The default player class.
        /// </summary>
        internal static Type DEFAULT_PLAYER_CLASS = typeof(Player);
#pragma warning restore SA1310

        /// <summary>
        /// A list of the player's items.
        /// </summary>
        internal readonly List<Item> ItemsValue = new(8);

        /// <summary>
        /// A dictionary of custom item category limits.
        /// </summary>
        internal Dictionary<ItemCategory, sbyte> CustomCategoryLimits = new();

        /// <summary>
        /// A dictionary of custom ammo limits.
        /// </summary>
        internal Dictionary<AmmoType, ushort> CustomAmmoLimits = new();
#pragma warning restore SA1401

        private ReferenceHub referenceHub;
        private CustomHealthStat healthStat;
        private CustomHumeShieldStat humeShieldStat;
        private CustomStaminaStat staminaStat;
        private Role role;

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="referenceHub">The <see cref="global::ReferenceHub"/> of the player to be encapsulated.</param>
        public Player(ReferenceHub referenceHub)
            : base(referenceHub.gameObject)
        {
            ReferenceHub = referenceHub;
            Items = ItemsValue.AsReadOnly();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="gameObject">The <see cref="UnityEngine.GameObject"/> of the player.</param>
        public Player(GameObject gameObject)
            : base(gameObject)
        {
            ReferenceHub = ReferenceHub.GetHub(gameObject);
            Items = ItemsValue.AsReadOnly();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Player"/> class.
        /// </summary>
        ~Player()
        {
            DictionaryPool<string, object>.Pool.Return(SessionVariables);
            DictionaryPool<RoleTypeId, float>.Pool.Return(FriendlyFireMultiplier);
            DictionaryPool<string, Dictionary<RoleTypeId, float>>.Pool.Return(CustomRoleFriendlyFireMultiplier);
        }

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing all <see cref="Player"/>'s on the server.
        /// </summary>
        public static Dictionary<GameObject, Player> Dictionary { get; } = new(Server.MaxPlayerCount, new ReferenceHub.GameObjectComparer());

        /// <summary>
        /// Gets a list of all <see cref="Player"/>'s on the server.
        /// </summary>
        public static new IReadOnlyCollection<Player> List => Dictionary.Values;

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing cached <see cref="Player"/> and their user ids.
        /// </summary>
        public static Dictionary<string, Player> UserIdsCache { get; } = new(20);

        /// <summary>
        /// Gets a randomly selected <see cref="Player"/>.
        /// </summary>
        /// <returns>A randomly selected <see cref="Player"/> object.</returns>
        public static Player Random => List.Random();

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing cached <see cref="RoleTypeId"/> and their FF multiplier. This is for non-unique roles.
        /// </summary>
        public Dictionary<RoleTypeId, float> FriendlyFireMultiplier { get; set; } = DictionaryPool<RoleTypeId, float>.Pool.Get();

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing cached <see cref="string"/> and their  <see cref="Dictionary{TKey, TValue}"/> which is cached Role with FF multiplier. This is for unique custom roles.
        /// </summary>
        /// <remarks> Consider adding this as object, Dict so that CustomRoles, and Strings can be parsed. </remarks>
        public Dictionary<string, Dictionary<RoleTypeId, float>> CustomRoleFriendlyFireMultiplier { get; set; } = DictionaryPool<string, Dictionary<RoleTypeId, float>>.Pool.Get();

        /// <summary>
        /// Gets or sets a unique custom role that does not adbide to base game for this player. Used in conjunction with <see cref="CustomRoleFriendlyFireMultiplier"/>.
        /// </summary>
        [EProperty(category: ROLES_CATEGORY)]
        public string UniqueRole { get; set; } = string.Empty;

        /// <summary>
        /// Gets the encapsulated <see cref="global::ReferenceHub"/>.
        /// </summary>
        public ReferenceHub ReferenceHub
        {
            get => referenceHub;
            private set
            {
                referenceHub = value ? value : throw new NullReferenceException("Player's ReferenceHub cannot be null!");
                GameObject = value.gameObject;
                HintDisplay = value.hints;
                Inventory = value.inventory;
                CameraTransform = value.PlayerCameraReference;

                value.playerStats._dictionarizedTypes[typeof(HealthStat)] = value.playerStats.StatModules[Array.IndexOf(PlayerStats.DefinedModules, typeof(HealthStat))] = healthStat = new CustomHealthStat { Hub = value };
                value.playerStats._dictionarizedTypes[typeof(HumeShieldStat)] = value.playerStats.StatModules[Array.IndexOf(PlayerStats.DefinedModules, typeof(HumeShieldStat))] = humeShieldStat = new CustomHumeShieldStat { Hub = value };
                value.playerStats._dictionarizedTypes[typeof(StaminaStat)] = value.playerStats.StatModules[Array.IndexOf(PlayerStats.DefinedModules, typeof(StaminaStat))] = staminaStat = new CustomStaminaStat { Hub = value };
            }
        }

        /// <summary>
        /// Gets the <see cref="PlayerRoleManager"/>.
        /// </summary>
        public PlayerRoleManager RoleManager => ReferenceHub.roleManager;

        /// <summary>
        /// Gets the player's ammo.
        /// </summary>
        public Dictionary<ItemType, ushort> Ammo => Inventory.UserInventory.ReserveAmmo;

        /// <summary>
        /// Gets the <see cref="ReferenceHub"/>'s <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public override Transform Transform => ReferenceHub.transform;

        /// <summary>
        /// Gets the hint currently watched by the player.
        /// </summary>
        /// May be <see langword="null"/>.
        [EProperty(readOnly: true, category: HINTS_CATEGORY)]
        public Hint CurrentHint { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether or not the player is viewing a hint.
        /// </summary>
        [EProperty(readOnly: true, category: HINTS_CATEGORY)]
        public bool HasHint => CurrentHint != null;

        /// <summary>
        /// Gets the <see cref="ReferenceHub"/>'s <see cref="VoiceModule"/>, can be null.
        /// </summary>
        public VoiceModuleBase VoiceModule => RoleManager.CurrentRole is IVoiceRole voiceRole ? voiceRole.VoiceModule : null;

        /// <summary>
        /// Gets the <see cref="ReferenceHub"/>'s <see cref="PersonalRadioPlayback"/>, can be null.
        /// </summary>
        public PersonalRadioPlayback RadioPlayback => VoiceModule is IRadioVoiceModule radioVoiceModule ? radioVoiceModule.RadioPlayback : null;

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
        /// Gets or sets the player's <see cref="VcMuteFlags"/>.
        /// </summary>
        public VcMuteFlags VoiceChatMuteFlags
        {
            get => VoiceChatMutes.GetFlags(ReferenceHub);
            set => VoiceChatMutes.SetFlags(ReferenceHub, value);
        }

        /// <summary>
        /// Gets or sets the player's id.
        /// </summary>
        [EProperty(category: INFO_CATEGORY)]
        public int Id
        {
            get => ReferenceHub.PlayerId;
            set => ReferenceHub._playerId = new(value);
        }

        /// <summary>
        /// Gets the player's user id.
        /// </summary>
        [EProperty(readOnly: true, category: INFO_CATEGORY)]
        public string UserId => referenceHub.authManager.UserId;

        /// <summary>
        /// Gets the player's user id without the authentication.
        /// </summary>
        [EProperty(readOnly: true, category: INFO_CATEGORY)]
        public string RawUserId { get; internal set; }

        /// <summary>
        /// Gets the player's authentication token.
        /// </summary>
        public string AuthenticationToken => ReferenceHub.authManager.GetAuthToken();

        /// <summary>
        /// Gets the player's authentication type.
        /// </summary>
        public AuthenticationType AuthenticationType
        {
            get
            {
                if (string.IsNullOrEmpty(UserId))
                    return AuthenticationType.Unknown;

                return UserId.Substring(UserId.LastIndexOf('@') + 1) switch
                {
                    "steam" => AuthenticationType.Steam,
                    "discord" => AuthenticationType.Discord,
                    "northwood" => AuthenticationType.Northwood,
                    "localhost" => AuthenticationType.LocalHost,
                    "ID_Dedicated" => AuthenticationType.DedicatedServer,
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
        /// Gets or sets a value indicating whether or not the player is a NPC.
        /// </summary>
        public bool IsNPC { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the player has an active CustomName.
        /// </summary>
        public bool HasCustomName => ReferenceHub.nicknameSync.HasCustomName;

        /// <summary>
        /// Gets or sets the player's nickname displayed to other player.
        /// </summary>
        [EProperty(category: INFO_CATEGORY)]
        public string DisplayNickname
        {
            get => ReferenceHub.nicknameSync.DisplayName;
            set => ReferenceHub.nicknameSync.DisplayName = value;
        }

        /// <summary>
        /// Gets or sets the player's nickname, if null it sets the original nickname.
        /// </summary>
        [EProperty(category: INFO_CATEGORY)]
        public string CustomName
        {
            get => ReferenceHub.nicknameSync.Network_displayName ?? Nickname;
            set => ReferenceHub.nicknameSync.Network_displayName = value;
        }

        /// <summary>
        /// Gets the player's nickname.
        /// </summary>
        [EProperty(readOnly: true, category: INFO_CATEGORY)]
        public string Nickname => ReferenceHub.nicknameSync.Network_myNickSync;

        /// <summary>
        /// Gets or sets the player's player info area bitmask.
        /// This property can be used to hide player name elements, such as the player's name, badges, etc.
        /// </summary>
        [EProperty(category: INFO_CATEGORY)]
        public PlayerInfoArea InfoArea
        {
            get => ReferenceHub.nicknameSync.Network_playerInfoToShow;
            set => ReferenceHub.nicknameSync.Network_playerInfoToShow = value;
        }

        /// <summary>
        /// Gets or sets the player's custom player info string. This string is displayed along with the player's <see cref="InfoArea"/>.
        /// </summary>
        [EProperty(category: INFO_CATEGORY)]
        public string CustomInfo
        {
            get => ReferenceHub.nicknameSync.Network_customPlayerInfoString;
            set
            {
                // NW Client check.
                if (value is not null && value.Contains('<'))
                {
                    foreach (string token in value.Split('<'))
                    {
                        if (token.StartsWith("/", StringComparison.Ordinal) ||
                            token.StartsWith("b>", StringComparison.Ordinal) ||
                            token.StartsWith("i>", StringComparison.Ordinal) ||
                            token.StartsWith("size=", StringComparison.Ordinal) ||
                            token.Length is 0)
                            continue;

                        if (token.StartsWith("color=", StringComparison.Ordinal))
                        {
                            if (token.Length < 14 || token[13] != '>')
                                Log.Error($"Custom info of player {Nickname} has been REJECTED. \nreason: (Bad text reject) \ntoken: {token} \nInfo: {value}");
                            else if (!Misc.AllowedColors.ContainsValue(token.Substring(6, 7)))
                                Log.Error($"Custom info of player {Nickname} has been REJECTED. \nreason: (Bad color reject) \ntoken: {token} \nInfo: {value}");
                        }
                        else if (token.StartsWith("#", StringComparison.Ordinal))
                        {
                            if (token.Length < 8 || token[7] != '>')
                                Log.Error($"Custom info of player {Nickname} has been REJECTED. \nreason: (Bad text reject) \ntoken: {token} \nInfo: {value}");
                            else if (!Misc.AllowedColors.ContainsValue(token.Substring(0, 7)))
                                Log.Error($"Custom info of player {Nickname} has been REJECTED. \nreason: (Bad color reject) \ntoken: {token} \nInfo: {value}");
                        }
                        else
                        {
                            Log.Error($"Custom info of player {Nickname} has been REJECTED. \nreason: (Bad color reject) \ntoken: {token} \nInfo: {value}");
                        }
                    }
                }

                InfoArea = InfoArea.ModifyFlags(!string.IsNullOrEmpty(value), PlayerInfoArea.CustomInfo);
                ReferenceHub.nicknameSync.Network_customPlayerInfoString = value;
            }
        }

        /// <summary>
        /// Gets or sets the range at which this player's info can be viewed by others.
        /// </summary>
        [EProperty(category: INFO_CATEGORY)]
        public float InfoViewRange
        {
            get => ReferenceHub.nicknameSync.NetworkViewRange;
            set => ReferenceHub.nicknameSync.NetworkViewRange = value;
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
        public Dictionary<string, object> SessionVariables { get; } = DictionaryPool<string, object>.Pool.Get();

        /// <summary>
        /// Gets a value indicating whether or not the player has Do Not Track (DNT) enabled. If this value is <see langword="true"/>, data about the player unrelated to server security shouldn't be stored.
        /// </summary>
        public bool DoNotTrack => ReferenceHub.authManager.DoNotTrack;

        /// <summary>
        /// Gets a value indicating whether the player is fully connected to the server.
        /// </summary>
        public bool IsConnected => GameObject != null;

        /// <summary>
        /// Gets a value indicating whether or not the player has a reserved slot.
        /// </summary>
        /// <seealso cref="GiveReservedSlot(bool)"/>
        /// <seealso cref="AddReservedSlot(string, bool)"/>
        public bool HasReservedSlot => ReservedSlot.HasReservedSlot(UserId, out _);

        /// <summary>
        /// Gets a value indicating whether or not the player is in whitelist.
        /// </summary>
        /// <remarks>It will always return <see langword="true"/> if a whitelist is disabled on the server.</remarks>
        /// <seealso cref="GrantWhitelist(bool)"/>
        /// <seealso cref="AddToWhitelist(string, bool)"/>
        public bool IsWhitelisted => WhiteList.IsWhitelisted(UserId);

        /// <summary>
        /// Gets a value indicating whether or not the player has Remote Admin access.
        /// </summary>
        public bool RemoteAdminAccess => ReferenceHub.serverRoles.RemoteAdmin;

        /// <summary>
        /// Gets a value indicating whether or not the player has Admin Chat access.
        /// </summary>
        public bool AdminChatAccess => ReferenceHub.serverRoles.AdminChatPerms;

        /// <summary>
        /// Gets a value indicating a player's kick power.
        /// </summary>
        public byte KickPower => ReferenceHub.serverRoles.KickPower;

        /// <summary>
        /// Gets or sets a value indicating whether or not the player's overwatch is enabled.
        /// </summary>
        [EProperty(category: ROLES_CATEGORY)]
        public bool IsOverwatchEnabled
        {
            get => ReferenceHub.serverRoles.IsInOverwatch;
            set => ReferenceHub.serverRoles.IsInOverwatch = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player is allowed to enter noclip mode.
        /// </summary>
        /// <remarks>For forcing the player into noclip mode, see <see cref="IsNoclipEnabled"/>.</remarks>
        /// <seealso cref="IsNoclipEnabled"/>
        [EProperty(category: MOVEMENT_CATEGORY)]
        public bool IsNoclipPermitted
        {
            get => FpcNoclip.IsPermitted(ReferenceHub);
            set
            {
                if (value)
                    FpcNoclip.PermitPlayer(ReferenceHub);
                else
                    FpcNoclip.UnpermitPlayer(ReferenceHub);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player has noclip enabled.
        /// </summary>
        /// <returns><see cref="bool"/> indicating status.</returns>
        /// <remarks>For permitting a player to enter and exit noclip freely, see <see cref="IsNoclipPermitted"/>.</remarks>
        /// <seealso cref="IsNoclipPermitted"/>
        [EProperty(category: MOVEMENT_CATEGORY)]
        public bool IsNoclipEnabled
        {
            get => ReferenceHub.playerStats.GetModule<AdminFlagsStat>().HasFlag(AdminFlags.Noclip);
            set => ReferenceHub.playerStats.GetModule<AdminFlagsStat>().SetFlag(AdminFlags.Noclip, value);
        }

        /// <summary>
        /// Gets or sets a value indicating the <see cref="Player"/> that currently has the player cuffed.
        /// <para>
        /// This value will be <see langword="null"/> if the player is not cuffed. Setting this value to <see langword="null"/> will uncuff the player if they are cuffed.
        /// </para>
        /// </summary>
        public Player Cuffer
        {
            get => Get(DisarmedPlayers.Entries.FirstOrDefault(entry => entry.DisarmedPlayer == NetworkIdentity.netId).Disarmer);
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
        /// <seealso cref="Teleport(Vector3)"/>
        /// <seealso cref="Teleport(object)"/>
        [EProperty(category: WORLDSPACE_CATEGORY)]
        public override Vector3 Position
        {
            get => Transform.position;
            set => ReferenceHub.TryOverridePosition(value, Vector3.zero);
        }

        /// <summary>
        /// Gets or sets the relative player's position.
        /// </summary>
        /// <remarks>The value will be default if the player's role is not an <see cref="FpcRole"/>.</remarks>
        [EProperty(category: WORLDSPACE_CATEGORY)]
        public RelativePosition RelativePosition
        {
            get => Role is FpcRole fpcRole ? fpcRole.RelativePosition : default;
            set => Position = value.Position;
        }

        /// <summary>
        /// Gets or sets the player's rotation.
        /// </summary>
        /// <returns>Returns the direction the player is looking at.</returns>
        [EProperty(category: WORLDSPACE_CATEGORY)]
        public override Quaternion Rotation
        {
            get => Transform.rotation;
            set => ReferenceHub.TryOverridePosition(Position, value.eulerAngles - Transform.eulerAngles);
        }

        /// <summary>
        /// Gets the <see cref="Player"/>'s current movement speed.
        /// </summary>
        [EProperty(readOnly: true, category: WORLDSPACE_CATEGORY)]
        public Vector3 Velocity => ReferenceHub.GetVelocity();

        /// <summary>
        /// Gets the player's <see cref="Enums.LeadingTeam"/>.
        /// </summary>
        public LeadingTeam LeadingTeam => Role.Team.GetLeadingTeam();

        /// <summary>
        /// Gets or sets a value indicating the actual RA permissions.
        /// </summary>
        public PlayerPermissions RemoteAdminPermissions
        {
            get => (PlayerPermissions)ReferenceHub.serverRoles.Permissions;
            set => ReferenceHub.serverRoles.Permissions = (ulong)value;
        }

        /// <summary>
        /// Gets a <see cref="Roles.Role"/> that is unique to this player and this class. This allows modification of various aspects related to the role solely.
        /// <para>
        /// The type of the Role is different based on the <see cref="RoleTypeId"/> of the player, and casting should be used to modify the role.
        /// <br /><see cref="RoleTypeId.Spectator"/> = <see cref="Roles.SpectatorRole"/>.
        /// <br /><see cref="RoleTypeId.Overwatch"/> = <see cref="Roles.OverwatchRole"/>.
        /// <br /><see cref="RoleTypeId.None"/> = <see cref="Roles.NoneRole"/>.
        /// <br /><see cref="RoleTypeId.Scp049"/> = <see cref="Scp049Role"/>.
        /// <br /><see cref="RoleTypeId.Scp0492"/> = <see cref="Scp0492Role"/>.
        /// <br /><see cref="RoleTypeId.Scp079"/> = <see cref="Scp079Role"/>.
        /// <br /><see cref="RoleTypeId.Scp096"/> = <see cref="Scp096Role"/>.
        /// <br /><see cref="RoleTypeId.Scp106"/> = <see cref="Scp106Role"/>.
        /// <br /><see cref="RoleTypeId.Scp173"/> = <see cref="Scp173Role"/>.
        /// <br /><see cref="RoleTypeId.Scp3114"/> = <see cref="Scp3114Role"/>.
        /// <br /><see cref="RoleTypeId.Scp939"/> = <see cref="Scp939Role"/>.
        /// <br />If not listed above, the type of Role will be <see cref="HumanRole"/>.
        /// </para>
        /// <para>
        /// If the role object is stored, it may become invalid if the player changes roles. Thus, the <see cref="Role.IsValid"/> property can be checked. If this property is <see langword="false"/>, the role should be discarded and this property should be used again to get the new Role.
        /// This role is automatically cached until it changes, and it is recommended to use this property directly rather than storing the property yourself.
        /// </para>
        /// <para>
        /// Roles and RoleTypeIds can be compared directly. <c>Player.Role == RoleTypeId.Scp079</c> is valid and will return <see langword="true"/> if the player is SCP-079. To set the player's role, see <see cref="Role.Set(RoleTypeId, SpawnReason, RoleSpawnFlags)"/>.
        /// </para>
        /// </summary>
        /// <seealso cref="Role.Set(RoleTypeId, SpawnReason, RoleSpawnFlags)"/>
        [EProperty(readOnly: true, category: ROLES_CATEGORY)]
        public Role Role
        {
            get => role ??= Role.Create(RoleManager.CurrentRole);
            internal set
            {
                PreviousRole = role;
                role = value;
            }
        }

        /// <summary>
        /// Gets the previous player's role.
        /// </summary>
        [EProperty(readOnly: true, category: ROLES_CATEGORY)]
        public Role PreviousRole { get; private set; }

        /// <summary>
        /// Gets or sets the player's SCP preferences.
        /// </summary>
        [EProperty(category: INFO_CATEGORY)]
        public ScpSpawnPreferences.SpawnPreferences ScpPreferences
        {
            get
            {
                if (ScpSpawnPreferences.Preferences.TryGetValue(Connection.connectionId, out ScpSpawnPreferences.SpawnPreferences value))
                    return value;

                return default;
            }
            set => ScpSpawnPreferences.Preferences[Connection.connectionId] = value;
        }

        /// <summary>
        /// Gets a value indicating whether or not the player is cuffed.
        /// </summary>
        /// <remarks>Players can be cuffed without another player being the cuffer.</remarks>
        public bool IsCuffed => Inventory.IsDisarmed();

        /// <summary>
        /// Gets a value indicating whether or not the player is reloading a weapon.
        /// </summary>
        public bool IsReloading => CurrentItem is Firearm firearm && !firearm.Base.AmmoManagerModule.Standby;

        /// <summary>
        /// Gets a value indicating whether or not the player is aiming with a weapon.
        /// </summary>
        public bool IsAimingDownWeapon => CurrentItem is Firearm firearm && firearm.Aiming;

        /// <summary>
        /// Gets a value indicating whether or not the player has enabled weapon's flashlight module.
        /// </summary>
        public bool HasFlashlightModuleEnabled => CurrentItem is Firearm firearm && firearm.FlashlightEnabled;

        /// <summary>
        /// Gets a value indicating whether or not the player is jumping.
        /// </summary>
        public bool IsJumping => Role is FpcRole fpc && fpc.FirstPersonController.FpcModule.Motor.IsJumping;

        /// <summary>
        /// Gets the player's IP address.
        /// </summary>
        public string IPAddress => ReferenceHub.networkIdentity.connectionToClient.address;

        /// <summary>
        /// Gets the player's command sender instance.
        /// </summary>
        public PlayerCommandSender Sender => ReferenceHub.queryProcessor._sender;

        /// <summary>
        /// Gets player's <see cref="NetworkConnection"/>.
        /// </summary>
        public NetworkConnection Connection => ReferenceHub.connectionToClient;

        /// <summary>
        /// Gets the player's <see cref="Mirror.NetworkIdentity"/>.
        /// </summary>
        public NetworkIdentity NetworkIdentity => ReferenceHub.networkIdentity;

        /// <summary>
        /// Gets the player's net ID.
        /// </summary>
        public uint NetId => ReferenceHub.netId;

        /// <summary>
        /// Gets a value indicating whether or not the player is the host.
        /// </summary>
        public bool IsHost => ReferenceHub.isLocalPlayer;

        /// <summary>
        /// Gets a value indicating whether or not the player is alive.
        /// </summary>
        [EProperty(readOnly: true, category: ROLES_CATEGORY)]
        public bool IsAlive => !IsDead;

        /// <summary>
        /// Gets a value indicating whether or not the player is dead.
        /// </summary>
        [EProperty(readOnly: true, category: ROLES_CATEGORY)]
        public bool IsDead => Role?.IsDead ?? false;

        /// <summary>
        /// Gets a value indicating whether or not the player's <see cref="RoleTypeId"/> is any NTF rank.
        /// Equivalent to checking the player's <see cref="Team"/>.
        /// </summary>
        [EProperty(readOnly: true, category: ROLES_CATEGORY)]
        public bool IsNTF => Role?.Team is Team.FoundationForces;

        /// <summary>
        /// Gets a value indicating whether or not the player's <see cref="RoleTypeId"/> is any Chaos rank.
        /// Equivalent to checking the player's <see cref="Team"/>.
        /// </summary>
        [EProperty(readOnly: true, category: ROLES_CATEGORY)]
        public bool IsCHI => Role?.Team is Team.ChaosInsurgency;

        /// <summary>
        /// Gets a value indicating whether or not the player's <see cref="RoleTypeId"/> is any SCP.
        /// Equivalent to checking the player's <see cref="Team"/>.
        /// </summary>
        [EProperty(readOnly: true, category: ROLES_CATEGORY)]
        public bool IsScp => Role?.Team is Team.SCPs;

        /// <summary>
        /// Gets a value indicating whether or not the player's <see cref="RoleTypeId"/> is any human rank.
        /// </summary>
        [EProperty(readOnly: true, category: ROLES_CATEGORY)]
        public bool IsHuman => Role is not null && Role.Is(out HumanRole _);

        /// <summary>
        /// Gets a value indicating whether or not the player's <see cref="RoleTypeId"/> is equal to <see cref="RoleTypeId.Tutorial"/>.
        /// </summary>
        [EProperty(readOnly: true, category: ROLES_CATEGORY)]
        public bool IsTutorial => Role?.Type is RoleTypeId.Tutorial;

        /// <summary>
        /// Gets a value indicating whether or not the player's friendly fire is enabled.
        /// <br>This property only determines if this player can deal damage to players on the same team;</br>
        /// <br>This player can be damaged by other players on their own team even if this property is <see langword="false"/>.</br>
        /// </summary>
        [EProperty(readOnly: true, category: ROLES_CATEGORY)]
        public bool IsFriendlyFireEnabled => FriendlyFireMultiplier.Count > 0 || CustomRoleFriendlyFireMultiplier.Count > 0;

        /// <summary>
        /// Gets or sets the player's scale.
        /// </summary>
        [EProperty(category: WORLDSPACE_CATEGORY)]
        public Vector3 Scale
        {
            get => ReferenceHub.transform.localScale;
            set
            {
                if (value == Scale)
                    return;

                try
                {
                    ReferenceHub.transform.localScale = value;

                    foreach (Player target in List)
                        MirrorExtensions.SendSpawnMessageMethodInfo?.Invoke(null, new object[] { NetworkIdentity, target.Connection });
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
        [EProperty(category: ROLES_CATEGORY)]
        public bool IsBypassModeEnabled
        {
            get => ReferenceHub.serverRoles.BypassMode;
            set => ReferenceHub.serverRoles.BypassMode = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player is muted.
        /// </summary>
        /// <remarks>This property will NOT persistently mute and unmute the player. For persistent mutes, see <see cref="Mute(bool)"/> and <see cref="UnMute(bool)"/>.</remarks>
        [EProperty(category: INFO_CATEGORY)]
        public bool IsMuted
        {
            get => VoiceChatMutes.QueryLocalMute(UserId, false);
            set => VoiceChatMuteFlags = VoiceChatMuteFlags.ModifyFlags(value, VcMuteFlags.LocalRegular);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player is global muted.
        /// </summary>
        /// <remarks>This property will NOT persistently mute and unmute the player. For persistent mutes, see <see cref="Mute(bool)"/> and <see cref="UnMute(bool)"/>.</remarks>
        [EProperty(category: INFO_CATEGORY)]
        public bool IsGlobalMuted
        {
            get => VoiceChatMutes.Mutes.Contains(UserId) && VoiceChatMuteFlags.HasFlag(VcMuteFlags.GlobalRegular);
            set => VoiceChatMuteFlags = VoiceChatMuteFlags.ModifyFlags(value, VcMuteFlags.GlobalRegular);
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player is intercom muted.
        /// </summary>
        /// <remarks>This property will NOT persistently mute and unmute the player. For persistent mutes, see <see cref="Mute(bool)"/> and <see cref="UnMute(bool)"/>.</remarks>
        [EProperty(category: INFO_CATEGORY)]
        public bool IsIntercomMuted
        {
            get => VoiceChatMutes.QueryLocalMute(UserId, true);
            set => VoiceChatMuteFlags = VoiceChatMuteFlags.ModifyFlags(value, VcMuteFlags.LocalIntercom);
        }

        /// <summary>
        /// Gets a value indicating whether or not the player is speaking.
        /// </summary>
        public bool IsSpeaking => VoiceModule != null && VoiceModule.IsSpeaking;

        /// <summary>
        /// Gets the loudness of a player when speaking.
        /// </summary>
        [EProperty(readOnly: true, category: INFO_CATEGORY)]
        public float Loudness => !IsSpeaking || VoiceModule is not StandardVoiceModule standardModule ? 0f : standardModule.GlobalPlayback.Loudness;

        /// <summary>
        /// Gets the player's voice color.
        /// </summary>
        [EProperty(readOnly: true, category: INFO_CATEGORY)]
        public Color VoiceColor => ReferenceHub.serverRoles.GetVoiceColor();

        /// <summary>
        /// Gets or sets the player's voice channel.
        /// </summary>
        [EProperty(category: INFO_CATEGORY)]
        public VoiceChatChannel VoiceChannel
        {
            get => VoiceModule == null ? VoiceChatChannel.None : VoiceModule.CurrentChannel;
            set
            {
                if (VoiceModule == null)
                    return;

                VoiceModule.CurrentChannel = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the player is transmitting on a Radio.
        /// </summary>
        public bool IsTransmitting => PersonalRadioPlayback.IsTransmitting(ReferenceHub);

        /// <summary>
        /// Gets or sets a value indicating whether or not the player has godmode enabled.
        /// </summary>
        [EProperty(category: ROLES_CATEGORY)]
        public bool IsGodModeEnabled
        {
            get => ReferenceHub.characterClassManager.GodMode;
            set => ReferenceHub.characterClassManager.GodMode = value;
        }

        /// <summary>
        /// Gets the player's unit name.
        /// </summary>
        [EProperty(readOnly: true, category: INFO_CATEGORY)]
        public string UnitName => Role.Base is PlayerRoles.HumanRole humanRole ? UnitNameMessageHandler.GetReceived(humanRole.AssignedSpawnableTeam, humanRole.UnitNameId) : string.Empty;

        /// <summary>
        /// Gets or sets the player's unit id.
        /// </summary>
        [EProperty(category: INFO_CATEGORY)]
        public byte UnitId
        {
            get => Role.Base is PlayerRoles.HumanRole humanRole ? humanRole.UnitNameId : byte.MinValue;
            set => _ = Role.Base is PlayerRoles.HumanRole humanRole ? humanRole.UnitNameId = value : _ = value;
        }

        /// <summary>
        /// Gets an array of <see cref="DangerStackBase"/> if the Scp1853 effect is enabled or an empty array if it is not enabled.
        /// </summary>
        public DangerStackBase[] Dangers => !TryGetEffect(out Scp1853 scp1853Effect) || !scp1853Effect.IsEnabled ? Array.Empty<DangerStackBase>() : scp1853Effect.Dangers;

        /// <summary>
        /// Gets a list of active <see cref="DangerStackBase"/> the player has.
        /// </summary>
        public IEnumerable<DangerStackBase> ActiveDangers => Dangers.Where(d => d.IsActive);

        /// <summary>
        /// Gets or sets the player's health.
        /// </summary>
        [EProperty(category: STATS_CATEGORY)]
        public float Health
        {
            get => healthStat.CurValue;
            set => healthStat.CurValue = value;
        }

        /// <summary>
        /// Gets or sets the player's maximum health.
        /// </summary>
        [EProperty(category: STATS_CATEGORY)]
        public float MaxHealth
        {
            get => healthStat.MaxValue;
            set => healthStat.CustomMaxValue = value;
        }

        /// <summary>
        /// Gets or sets the player's artificial health.
        /// If the health is greater than the <see cref="MaxArtificialHealth"/>, it will also be changed to match the artificial health.
        /// </summary>
        [EProperty(category: STATS_CATEGORY)]
        public float ArtificialHealth
        {
            get => ActiveArtificialHealthProcesses.FirstOrDefault()?.CurrentAmount ?? 0f;
            set
            {
                AhpStat.AhpProcess ahp = ActiveArtificialHealthProcesses.FirstOrDefault();

                if (ahp is not null)
                    ahp.CurrentAmount = value;
                else
                    AddAhp(value);
            }
        }

        /// <summary>
        /// Gets or sets the player's maximum artificial health.
        /// </summary>
        [EProperty(category: STATS_CATEGORY)]
        public float MaxArtificialHealth
        {
            get => ActiveArtificialHealthProcesses.FirstOrDefault()?.Limit ?? 0f;
            set
            {
                AhpStat.AhpProcess ahp = ActiveArtificialHealthProcesses.FirstOrDefault();

                if (ahp is not null)
                    ahp.Limit = value;
                else
                    AddAhp(value, value);
            }
        }

        /// <summary>
        /// Gets the <see cref="AhpStat"/> of the Player.
        /// </summary>
        public AhpStat AhpStat => ReferenceHub.playerStats.GetModule<AhpStat>();

        /// <summary>
        /// Gets a <see cref="List{T}"/> of all active Artificial Health processes on the player.
        /// </summary>
        public List<AhpStat.AhpProcess> ActiveArtificialHealthProcesses => AhpStat._activeProcesses;

        /// <summary>
        /// Gets or sets the player's Hume Shield.
        /// </summary>
        /// <remarks>This value can bypass the role's hume shield maximum. However, this value will only be visible to the end-player as Hume Shield if <see cref="FpcRole.IsHumeShieldedRole"/> is <see langword="true"/>. Otherwise, the game will treat the player as though they have the amount of Hume Shield specified, even though they cannot see it.</remarks>
        [EProperty(category: STATS_CATEGORY)]
        public float HumeShield
        {
            get => HumeShieldStat.CurValue;
            set => HumeShieldStat.CurValue = value;
        }

        /// <summary>
        /// Gets or sets the players maximum Hume Shield.
        /// </summary>
        public float MaxHumeShield
        {
            get => HumeShieldStat.MaxValue;
            set => HumeShieldStat.CustomMaxValue = value;
        }

        /// <summary>
        /// Gets or sets the players multiplier for gaining HumeShield.
        /// </summary>
        public float HumeShieldRegenerationMultiplier
        {
            get => HumeShieldStat.ShieldRegenerationMultiplier;
            set => HumeShieldStat.ShieldRegenerationMultiplier = value;
        }

        /// <summary>
        /// Gets the player's <see cref="PlayerStatsSystem.HumeShieldStat"/>.
        /// </summary>
        public CustomHumeShieldStat HumeShieldStat => humeShieldStat;

        /// <summary>
        /// Gets or sets the item in the player's hand. Value will be <see langword="null"/> if the player is not holding anything.
        /// </summary>
        /// <seealso cref="DropHeldItem()"/>
        [EProperty(category: INVENTORY_CATEGORY)]
        public Item CurrentItem
        {
            get => Item.Get(Inventory.CurInstance);
            set
            {
                if (value is null || value.Type == ItemType.None)
                {
                    Inventory.ServerSelectItem(0);
                    return;
                }

                if (!Inventory.UserInventory.Items.TryGetValue(value.Serial, out _))
                {
                    if (IsInventoryFull)
                        return;

                    AddItem(value.Base);
                }

                Inventory.ServerSelectItem(value.Serial);
            }
        }

        /// <summary>
        /// Gets the armor that the player is currently wearing. Value will be <see langword="null"/> if the player is not wearing any armor.
        /// </summary>
        [EProperty(readOnly: true, category: INVENTORY_CATEGORY)]
        public Armor CurrentArmor => Inventory.TryGetBodyArmor(out BodyArmor armor) ? (Armor)Item.Get(armor) : null;

        /// <summary>
        /// Gets the <see cref="StaminaStat"/> class.
        /// </summary>
        public CustomStaminaStat StaminaStat => staminaStat;

        /// <summary>
        /// Gets or sets the amount of stamina the player has.
        /// </summary>
        /// <remarks>This will always be a value between <c>0-1</c>, <c>0</c> representing no stamina and <c>1</c> representing maximum stamina.</remarks>
        [EProperty(category: STATS_CATEGORY)]
        public float Stamina
        {
            get => StaminaStat.CurValue;
            set => StaminaStat.CurValue = value;
        }

        /// <summary>
        /// Gets or sets the players maximum stamina.
        /// </summary>
        public float MaxStamina
        {
            get => StaminaStat.MaxValue;
            set => StaminaStat.CustomMaxValue = value;
        }

        /// <summary>
        /// Gets a value indicating whether or not the staff bypass is enabled.
        /// </summary>
        public bool IsStaffBypassEnabled => ReferenceHub.authManager.BypassBansFlagSet;

        /// <summary>
        /// Gets or sets the player's group name.
        /// </summary>
        public string GroupName
        {
            get => ServerStatic.PermissionsHandler._members.TryGetValue(UserId, out string groupName) ? groupName : null;
            set => ServerStatic.PermissionsHandler._members[UserId] = value;
        }

        /// <summary>
        /// Gets the current <see cref="Room"/> the player is in.
        /// </summary>
        [EProperty(readOnly: true, category: WORLDSPACE_CATEGORY)]
        public Room CurrentRoom => Room.FindParentRoom(GameObject);

        /// <summary>
        /// Gets the current zone the player is in.
        /// </summary>
        [EProperty(readOnly: true, category: WORLDSPACE_CATEGORY)]
        public ZoneType Zone => CurrentRoom ? CurrentRoom.Zone : ZoneType.Unspecified;

        /// <summary>
        /// Gets the current <see cref="Features.Lift"/> the player is in. Can be <see langword="null"/>.
        /// </summary>
        public Lift Lift => Lift.Get(Position);

        /// <summary>
        /// Gets all effects from the player, including non active ones.
        /// </summary>
        public IEnumerable<StatusEffectBase> AllEffects => referenceHub.playerEffectsController.AllEffects;

        /// <summary>
        /// Gets all currently active <see cref="StatusEffectBase">effects</see>.
        /// </summary>
        /// <seealso cref="EnableEffect(EffectType, float, bool)"/>
        /// <seealso cref="EnableEffect(StatusEffectBase, float, bool)"/>
        /// <seealso cref="EnableEffect(string, float, bool)"/>
        /// <seealso cref="EnableEffect{T}(float, bool)"/>
        /// <seealso cref="EnableEffects(IEnumerable{EffectType}, float, bool)"/>
        public IEnumerable<StatusEffectBase> ActiveEffects => referenceHub.playerEffectsController.AllEffects.Where(effect => effect.Intensity > 0);

        /// <summary>
        /// Gets all currently active <see cref="EffectType">effects</see>.
        /// </summary>
        [EProperty(readOnly: true, category: EFFECTS_CATEGORY)]
        public IEnumerable<EffectType> ActiveEffectTypes => ActiveEffects.Select(effect => effect.GetEffectType());

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
        [EProperty(category: INFO_CATEGORY)]
        public string RankColor
        {
            get => ReferenceHub.serverRoles.Network_myColor;
            set => ReferenceHub.serverRoles.SetColor(value);
        }

        /// <summary>
        /// Gets or sets the player's rank name.
        /// </summary>
        [EProperty(category: INFO_CATEGORY)]
        public string RankName
        {
            get => ReferenceHub.serverRoles.Network_myText;
            set => ReferenceHub.serverRoles.SetText(value);
        }

        /// <summary>
        /// Gets the global badge of the player. Value will be <see langword="null"/> if the player does not have a global badge.
        /// </summary>
        [EProperty(category: INFO_CATEGORY)]
        public Badge? GlobalBadge
        {
            get
            {
                if (string.IsNullOrEmpty(ReferenceHub.serverRoles.NetworkGlobalBadge))
                    return null;

                ServerRoles serverRoles = ReferenceHub.serverRoles;

                return new Badge(serverRoles._bgt, serverRoles._bgc, true);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player's badge is hidden.
        /// </summary>
        [EProperty(category: INFO_CATEGORY)]
        public bool BadgeHidden
        {
            get => !string.IsNullOrEmpty(ReferenceHub.serverRoles.HiddenBadge);
            set
            {
                if (value)
                    ReferenceHub.serverRoles.TryHideTag();
                else
                    ReferenceHub.serverRoles.RefreshLocalTag();
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the player is Northwood staff.
        /// </summary>
        public bool IsNorthwoodStaff => ReferenceHub.authManager.NorthwoodStaff;

        /// <summary>
        /// Gets a value indicating whether or not the player is a global moderator.
        /// </summary>
        public bool IsGlobalModerator => ReferenceHub.authManager.RemoteAdminGlobalAccess;

        /// <summary>
        /// Gets a value indicating whether or not the player is in the pocket dimension.
        /// </summary>
        [EProperty(readOnly: true, category: WORLDSPACE_CATEGORY)]
        public bool IsInPocketDimension => CurrentRoom && CurrentRoom.Type is RoomType.Pocket;

        /// <summary>
        /// Gets or sets a value indicating whether or not the player should use stamina system.
        /// </summary>
        [EProperty(category: STATS_CATEGORY)]
        public bool IsUsingStamina { get; set; } = true;

        /// <summary>
        /// Gets the player's ping.
        /// </summary>
        public int Ping => LiteNetLib4MirrorServer.GetPing(Connection.connectionId);

        /// <summary>
        /// Gets the player's items.
        /// </summary>
        [EProperty(readOnly: true, category: INVENTORY_CATEGORY)]
        public IReadOnlyCollection<Item> Items { get; }

        /// <summary>
        /// Gets a value indicating whether or not the player's inventory is empty.
        /// </summary>
        [EProperty(readOnly: true, category: INVENTORY_CATEGORY)]
        public bool IsInventoryEmpty => Items.Count is 0;

        /// <summary>
        /// Gets a value indicating whether or not the player's inventory is full.
        /// </summary>
        [EProperty(readOnly: true, category: INVENTORY_CATEGORY)]
        public bool IsInventoryFull => Items.Count >= Inventory.MaxSlots;

        /// <summary>
        /// Gets a value indicating whether the player is a target of SCP-096.
        /// </summary>
        /// <remarks>
        /// This property checks if the player is present in the list of targets maintained by SCP-096.
        /// </remarks>
        /// <returns><see langword="true"/> if the player is a target of SCP-096; otherwise, <see langword="false"/>.</returns>
        [EProperty(readOnly: true, category: INFO_CATEGORY)]
        public bool IsScp096Target => Player.List.Any(x => x.Role is Roles.Scp096Role scp096Role && scp096Role.Targets.Contains(x));

        /// <summary>
        /// Gets a value indicating whether the player is an observer of SCP-173.
        /// </summary>
        /// <remarks>
        /// This property checks if the player is present in the list of observers maintained by SCP-173.
        /// </remarks>
        /// <returns><see langword="true"/> if the player is an observer of SCP-173; otherwise, <see langword="false"/>.</returns>
        public bool IsScp173Observer => Player.List.Any(x => x.Role is Roles.Scp173Role scp173Role && scp173Role.ObservingPlayers.Contains(x));

        /// <summary>
        /// Gets a value indicating whether or not the player has agreed to microphone recording.
        /// </summary>
        public bool AgreedToRecording => VoiceChatPrivacySettings.CheckUserFlags(ReferenceHub, VcPrivacyFlags.SettingsSelected | VcPrivacyFlags.AllowRecording | VcPrivacyFlags.AllowMicCapture);

        /// <summary>
        /// Gets or sets a value indicating whether or not player will be affected by a flashbang.
        /// </summary>
        [EProperty(category: EFFECTS_CATEGORY)]
        public bool FlashImmune { get; set; } = false;

        /// <summary>
        /// Gets a <see cref="Player"/> <see cref="IEnumerable{T}"/> of spectators that are currently spectating this <see cref="Player"/>.
        /// </summary>
        public IEnumerable<Player> CurrentSpectatingPlayers => List.Where(player => ReferenceHub.IsSpectatedBy(player.ReferenceHub));

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> which contains all player's preferences.
        /// </summary>
        public Dictionary<FirearmType, AttachmentIdentifier[]> Preferences => Firearm.PlayerPreferences.FirstOrDefault(kvp => kvp.Key == this).Value;

        /// <summary>
        /// Gets the player's <see cref="Footprinting.Footprint"/>.
        /// </summary>
        public Footprint Footprint => new(ReferenceHub);

        /// <summary>
        /// Gets or sets a value indicating whether or not the player is spawn protected.
        /// </summary>
        [EProperty(category: INFO_CATEGORY)]
        public bool IsSpawnProtected
        {
            get => IsEffectActive<SpawnProtected>();
            set
            {
                if (value)
                    EnableEffect<SpawnProtected>(SpawnProtected.SpawnDuration);
                else
                    DisableEffect<SpawnProtected>();
            }
        }

        /// <summary>
        /// Gets a dictionary for storing player objects of connected but not yet verified players.
        /// </summary>
        internal static ConditionalWeakTable<GameObject, Player> UnverifiedPlayers { get; } = new();

        /// <summary>
        /// Converts NwPluginAPI player to EXILED player.
        /// </summary>
        /// <param name="player">The NwPluginAPI player.</param>
        /// <returns>EXILED player.</returns>
        public static implicit operator Player(PluginAPI.Core.Player player) => Get(player);

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
        public static IEnumerable<Player> Get(RoleTypeId role) => List.Where(player => player.Role == role);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Player"/> filtered based on a predicate.
        /// </summary>
        /// <param name="predicate">The condition to satisfy.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Player"/> which contains elements that satisfy the condition.</returns>
        public static IEnumerable<Player> Get(Func<Player, bool> predicate) => List.Where(predicate);

        /// <summary>
        /// Gets the nearest player to the specified <see cref="Vector3"/> within the given distance.
        /// </summary>
        /// <param name="vector">The <see cref="Vector3"/> to compare.</param>
        /// <param name="distance">The maximum distance the player can be from the <paramref name="vector"/> to be included.</param>
        /// <returns>The nearest <see cref="Player"/> within the specified distance, or <see langword="null"/> if no player is found.</returns>
        public static Player GetNearestPlayer(Vector3 vector, float distance) => GetNearestPlayers(vector, distance).OrderBy(p => MathExtensions.DistanceSquared(vector, p.Position)).FirstOrDefault();

        /// <summary>
        /// Gets all players near the specified <see cref="Vector3"/> within the given distance.
        /// </summary>
        /// <param name="vector">The <see cref="Vector3"/> to compare.</param>
        /// <param name="distance">The maximum distance the player can be from the <paramref name="vector"/> to be included.</param>
        /// <returns>The filtered collection of <see cref="Player"/> objects.</returns>
        public static IEnumerable<Player> GetNearestPlayers(Vector3 vector, float distance) => List.Where(p => MathExtensions.DistanceSquared(vector, p.Position) <= distance * distance);

        /// <summary>
        /// Gets the farthest player from the specified <see cref="Vector3"/> within the given distance.
        /// </summary>
        /// <param name="vector">The <see cref="Vector3"/> to compare.</param>
        /// <param name="distance">The minimum distance the player can be from the <paramref name="vector"/> to be included.</param>
        /// <returns>The farthest <see cref="Player"/> from the specified <paramref name="vector"/> within the given distance, or <see langword="null"/> if no player is found.</returns>
        public static Player GetFarthestPlayer(Vector3 vector, float distance) => GetFarthestPlayers(vector, distance).OrderByDescending(p => MathExtensions.DistanceSquared(vector, p.Position)).FirstOrDefault();

        /// <summary>
        /// Gets all players that have a distance greater than the specified distance from the given <see cref="Vector3"/>.
        /// </summary>
        /// <param name="vector">The <see cref="Vector3"/> to compare.</param>
        /// <param name="distance">The minimum distance the player can be from the <paramref name="vector"/> to be included.</param>
        /// <returns>The filtered collection of <see cref="Player"/> objects.</returns>
        public static IEnumerable<Player> GetFarthestPlayers(Vector3 vector, float distance) => List.Where(p => MathExtensions.DistanceSquared(vector, p.Position) >= distance * distance);

        /// <summary>
        /// Gets the <see cref="Player"/> belonging to the <see cref="CommandSystem.ICommandSender"/>, if any.
        /// </summary>
        /// <param name="sender">The command sender.</param>
        /// <returns>A <see cref="Player"/> or <see langword="null"/> if not found.</returns>
        public static Player Get(CommandSystem.ICommandSender sender) => Get(sender as CommandSender);

        /// <summary>
        /// Gets the <see cref="Player"/> belonging to the <see cref="Footprinting.Footprint"/>, if any.
        /// </summary>
        /// <param name="footprint">The Footprint.</param>
        /// <returns>A <see cref="Player"/> or <see langword="null"/> if not found.</returns>
        public static Player Get(Footprint footprint) => Get(footprint.Hub);

        /// <summary>
        /// Gets the <see cref="Player"/> belonging to the <see cref="CommandSender"/>, if any.
        /// </summary>
        /// <param name="sender">The command sender.</param>
        /// <returns>A <see cref="Player"/> or <see langword="null"/> if not found.</returns>
        public static Player Get(CommandSender sender) => sender is ServerConsoleSender ? Server.Host : Get(sender.SenderId);

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
        /// Gets the all <see cref="Player"/> instances belonging to the given <see cref="global::ReferenceHub"/> instances, if any.
        /// </summary>
        /// <param name="hubs">The reference hubs to retrieve the players from.</param>
        /// <returns>All <see cref="Player"/> instances belonging to the given <see cref="global::ReferenceHub"/> instances.</returns>
        public static IEnumerable<Player> Get(IEnumerable<ReferenceHub> hubs) => hubs.Select(hub => Get(hub));

        /// <summary>
        /// Gets the <see cref="Player"/> belonging to the <see cref="Collider"/>, if any.
        /// </summary>
        /// <param name="collider"><see cref="Collider"/>.</param>
        /// <returns>A <see cref="Player"/> or <see langword="null"/> if not found.</returns>
        public static Player Get(Collider collider) => Get(collider.transform.root.gameObject);

        /// <summary>
        /// Gets the <see cref="Player"/> belonging to a specific netId, if any.
        /// </summary>
        /// <param name="netId">The player's <see cref="NetworkIdentity.netId"/>.</param>
        /// <returns>The <see cref="Player"/> owning the netId, or <see langword="null"/> if not found.</returns>
        public static Player Get(uint netId) => ReferenceHub.TryGetHubNetID(netId, out ReferenceHub hub) ? Get(hub) : null;

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

            if (Dictionary.TryGetValue(gameObject, out Player player))
                return player;

            UnverifiedPlayers.TryGetValue(gameObject, out player);
            return player;
        }

        /// <summary>
        /// Gets the player belonging to the specified id.
        /// </summary>
        /// <param name="id">The player id.</param>
        /// <returns>Returns the player found or <see langword="null"/> if not found.</returns>
        public static Player Get(int id) => ReferenceHub.TryGetHub(id, out ReferenceHub referenceHub) ? Get(referenceHub) : null;

        /// <summary>
        /// Gets the <see cref="Player"/> by identifier.
        /// </summary>
        /// <param name="args">The player's nickname, ID, steamID64 or Discord ID.</param>
        /// <returns>Returns the player found or <see langword="null"/> if not found.</returns>
        public static Player Get(string args)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(args))
                    return null;

                if (UserIdsCache.TryGetValue(args, out Player playerFound) && playerFound.IsConnected)
                    return playerFound;

                if (int.TryParse(args, out int id))
                    return Get(id);

                if (args.EndsWith("@steam") || args.EndsWith("@discord") || args.EndsWith("@northwood"))
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
                    UserIdsCache[playerFound.UserId] = playerFound;

                return playerFound;
            }
            catch (Exception exception)
            {
                Log.Error($"{typeof(Player).FullName}.{nameof(Get)} error: {exception}");
                return null;
            }
        }

        /// <summary>
        /// Gets the <see cref="Player"/> from NwPluginAPI class.
        /// </summary>
        /// <param name="apiPlayer">The <see cref="PluginAPI.Core.Player"/> class.</param>
        /// <returns>A <see cref="Player"/> or <see langword="null"/> if not found.</returns>
        public static Player Get(PluginAPI.Core.Player apiPlayer) => Get(apiPlayer.ReferenceHub);

        /// <summary>
        /// Try-get a player given a <see cref="CommandSystem.ICommandSender"/>.
        /// </summary>
        /// <param name="sender">The <see cref="CommandSystem.ICommandSender"/>.</param>
        /// <param name="player">The player that matches the given <see cref="CommandSystem.ICommandSender"/>, or <see langword="null"/> if no player is found.</param>
        /// <returns>A boolean indicating whether or not a player was found.</returns>
        public static bool TryGet(CommandSystem.ICommandSender sender, out Player player) => (player = Get(sender)) is not null;

        /// <summary>
        /// Try-get a player given a <see cref="Footprinting.Footprint"/>.
        /// </summary>
        /// <param name="footprint">The <see cref="Footprinting.Footprint"/>.</param>
        /// <param name="player">The player that matches the given <see cref="Footprinting.Footprint"/>, or <see langword="null"/> if no player is found.</param>
        /// <returns>A boolean indicating whether or not a player was found.</returns>
        public static bool TryGet(Footprint footprint, out Player player) => (player = Get(footprint)) is not null;

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
        /// Try-get a player given a <see cref="NetworkConnection"/>.
        /// </summary>
        /// <param name="conn">The <see cref="NetworkConnection"/>.</param>
        /// <param name="player">The player that matches the given <see cref="NetworkConnection"/>, or <see langword="null"/> if no player is found.</param>
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
        /// Try-get the <see cref="Player"/> from NwPluginAPI class.
        /// </summary>
        /// <param name="apiPlayer">The <see cref="PluginAPI.Core.Player"/> class.</param>
        /// <param name="player">The player found or <see langword="null"/> if not found.</param>
        /// <returns>A boolean indicating whether or not a player was found.</returns>
        public static bool TryGet(PluginAPI.Core.Player apiPlayer, out Player player) => (player = Get(apiPlayer)) is not null;

        /// <summary>
        /// Try-get player by <see cref="Collider"/>.
        /// </summary>
        /// <param name="collider">The <see cref="Collider"/>.</param>
        /// <param name="player">The player found or <see langword="null"/> if not found.</param>
        /// <returns>A boolean indicating whether or not a player was found.</returns>
        public static bool TryGet(Collider collider, out Player player) => (player = Get(collider)) is not null;

        /// <summary>
        /// Gets an <see cref="IEnumerable{Player}"/> containing all players processed based on the arguments specified.
        /// </summary>
        /// <param name="args">The array segment of strings representing the input arguments to be processed.</param>
        /// <param name="startIndex">The starting index within the array segment.</param>
        /// <param name="newargs">Contains the updated arguments after processing.</param>
        /// <param name="keepEmptyEntries">Determines whether empty entries should be kept in the result.</param>
        /// <returns>An <see cref="IEnumerable{Player}"/> representing the processed players.</returns>
        public static IEnumerable<Player> GetProcessedData(ArraySegment<string> args, int startIndex, out string[] newargs, bool keepEmptyEntries = false) => RAUtils.ProcessPlayerIdOrNamesList(args, startIndex, out newargs, keepEmptyEntries).Select(hub => Get(hub));

        /// <summary>
        /// Gets an <see cref="IEnumerable{Player}"/> containing all players processed based on the arguments specified.
        /// </summary>
        /// <param name="args">The array segment of strings representing the input arguments to be processed.</param>
        /// <param name="startIndex">The starting index within the array segment.</param>
        /// <returns>An <see cref="IEnumerable{Player}"/> representing the processed players.</returns>
        public static IEnumerable<Player> GetProcessedData(ArraySegment<string> args, int startIndex = 0) => GetProcessedData(args, startIndex, out string[] _);

        /// <summary>
        /// Adds a player's UserId to the list of reserved slots.
        /// </summary>
        /// <param name="userId">The UserId of the player to add.</param>
        /// <param name="isPermanent"> Whether or not to add a <see langword="userId"/> permanently. It will write a <see langword="userId"/> to UserIDReservedSlots.txt file.</param>
        /// <returns><see langword="true"/> if the slot was successfully added, or <see langword="false"/> if the provided UserId already has a reserved slot.</returns>
        /// <seealso cref="GiveReservedSlot(bool)"/>
        public static bool AddReservedSlot(string userId, bool isPermanent)
        {
            if (isPermanent)
            {
                if (ReservedSlots.HasReservedSlot(userId))
                    return false;

                ReservedSlots.Add(userId);
                return true;
            }

            return ReservedSlot.Users.Add(userId);
        }

        /// <summary>
        /// Adds a player's UserId to the whitelist.
        /// </summary>
        /// <param name="userId">The UserId of the player to add.</param>
        /// <param name="isPermanent"> Whether or not to add a <see langword="userId"/> permanently. It will write a <see langword="userId"/> to UserIDWhitelist.txt file.</param>
        /// <returns><see langword="true"/> if the record was successfully added, or <see langword="false"/> if the provided UserId already is in whitelist.</returns>
        /// <seealso cref="GrantWhitelist(bool)"/>
        public static bool AddToWhitelist(string userId, bool isPermanent)
        {
            if (isPermanent)
            {
                if (WhiteList.IsOnWhitelist(userId))
                    return false;

                Whitelist.Add(userId);
                return true;
            }

            return WhiteList.Users.Add(userId);
        }

        /// <summary>
        /// Reloads the reserved slot list, clearing all reserved slot changes made with add/remove methods and reverting to the reserved slots files.
        /// </summary>
        public static void ReloadReservedSlots() => ReservedSlot.Reload();

        /// <summary>
        /// Reloads the whitelist, clearing all whitelist changes made with add/remove methods and reverting to the whitelist files.
        /// </summary>
        public static void ReloadWhitelist() => WhiteList.Reload();

        /// <summary>
        /// Adds a player's UserId to the list of reserved slots.
        /// </summary>
        /// <param name="isPermanent"> Whether or not to add a player's UserId permanently. It will write a player's UserId to UserIDReservedSlots.txt file.</param>
        /// <returns><see langword="true"/> if the slot was successfully added, or <see langword="false"/> if the provided UserId already has a reserved slot.</returns>
        /// <seealso cref="AddReservedSlot(string, bool)"/>
        public bool GiveReservedSlot(bool isPermanent) => AddReservedSlot(UserId, isPermanent);

        /// <summary>
        /// Adds a player's UserId to the whitelist.
        /// </summary>
        /// <param name="isPermanent"> Whether or not to add a player's UserId permanently. It will write a player's UserId to UserIDWhitelist.txt file.</param>
        /// <returns><see langword="true"/> if the record was successfully added, or <see langword="false"/> if the provided UserId already is in whitelist.</returns>
        /// <seealso cref="AddToWhitelist(string, bool)"/>
        public bool GrantWhitelist(bool isPermanent) => AddToWhitelist(UserId, isPermanent);

        /// <summary>
        /// Adds a component to the player.
        /// </summary>
        /// <typeparam name="T">The component type to add.</typeparam>
        public void AddComponent<T>()
            where T : Component
            => GameObject.AddComponent<T>();

        /// <summary>
        /// Removes a component from a player.
        /// </summary>
        /// <typeparam name="T">The component type to remove.</typeparam>
        public void RemoveComponent<T>()
            where T : Component
        {
            Component component = GameObject.GetComponent<T>();
            if (component != null)
                UnityEngine.Object.Destroy(component);
        }

        /// <summary>
        /// Gets vision information based on the specified target player and optional mask layer.
        /// </summary>
        /// <param name="target">The Player to target.</param>
        /// <param name="maskLayer">The mask layer to use (default is 0).</param>
        /// <returns>A <see cref="VisionInformation"/> object containing the provided information.</returns>
        public VisionInformation GetVisionInformation(Player target, LayerMaskFlags maskLayer = 0) =>
            VisionInformation.GetVisionInformation(ReferenceHub, CameraTransform, target.Position, target.Role is FpcRole fpc ? fpc.CharacterController.radius : 0, 0, true, true, (int)maskLayer, true);

        /// <summary>
        /// Gets vision information based on the specified target position and optional parameters.
        /// </summary>
        /// <param name="target">The target position as a <see cref="Vector3"/>.</param>
        /// <param name="radius">The radius to check (default is 0).</param>
        /// <param name="visionTriggerDistance">The vision trigger distance (default is 0).</param>
        /// <param name="checkFog">Specifies whether to check fog (default is true).</param>
        /// <param name="checkLineOfSight">Specifies whether to check Line of Sight (default is true).</param>
        /// <param name="maskLayer">The mask layer to use (default is 0).</param>
        /// <param name="checkInDarkness">Specifies whether to check if the player is in darkness (default is true).</param>
        /// <returns>A <see cref="VisionInformation"/> object based on the provided information.</returns>
        public VisionInformation GetVisionInformation(Vector3 target, float radius = 0f, float visionTriggerDistance = 0f, bool checkFog = true, bool checkLineOfSight = true, LayerMaskFlags maskLayer = 0, bool checkInDarkness = true) =>
            VisionInformation.GetVisionInformation(ReferenceHub, CameraTransform, target, radius, visionTriggerDistance, checkFog, checkLineOfSight, (int)maskLayer, checkInDarkness);

        /// <summary>
        /// Tries to add <see cref="RoleTypeId"/> to FriendlyFire rules.
        /// </summary>
        /// <param name="roleToAdd"> Role to add. </param>
        /// <param name="ffMult"> Friendly fire multiplier. </param>
        public void SetFriendlyFire(RoleTypeId roleToAdd, float ffMult) => FriendlyFireMultiplier[roleToAdd] = ffMult;

        /// <summary>
        /// Wrapper to call <see cref="SetFriendlyFire(RoleTypeId, float)"/>.
        /// </summary>
        /// <param name="roleFF"> Role with FF to add even if it exists. </param>
        public void SetFriendlyFire(KeyValuePair<RoleTypeId, float> roleFF) => SetFriendlyFire(roleFF.Key, roleFF.Value);

        /// <summary>
        /// Tries to add <see cref="RoleTypeId"/> to FriendlyFire rules.
        /// </summary>
        /// <param name="roleToAdd"> Role to add. </param>
        /// <param name="ffMult"> Friendly fire multiplier. </param>
        /// <returns> Whether or not the item was able to be added. </returns>
        public bool TryAddFriendlyFire(RoleTypeId roleToAdd, float ffMult)
        {
            if (FriendlyFireMultiplier.ContainsKey(roleToAdd))
                return false;

            FriendlyFireMultiplier.Add(roleToAdd, ffMult);
            return true;
        }

        /// <summary>
        /// Tries to add <see cref="RoleTypeId"/> to FriendlyFire rules.
        /// </summary>
        /// <param name="pairedRoleFF"> Role FF multiplier to add. </param>
        /// <returns> Whether or not the item was able to be added. </returns>
        public bool TryAddFriendlyFire(KeyValuePair<RoleTypeId, float> pairedRoleFF) => TryAddFriendlyFire(pairedRoleFF.Key, pairedRoleFF.Value);

        /// <summary>
        /// Tries to add <see cref="RoleTypeId"/> to FriendlyFire rules.
        /// </summary>
        /// <param name="ffRules"> Roles to add with friendly fire values. </param>
        /// <param name="overwrite"> Whether or not to overwrite current values if they exist. </param>
        /// <returns> Whether or not the item was able to be added. </returns>
        public bool TryAddFriendlyFire(Dictionary<RoleTypeId, float> ffRules, bool overwrite = false)
        {
            Dictionary<RoleTypeId, float> temporaryFriendlyFireRules = DictionaryPool<RoleTypeId, float>.Pool.Get();
            foreach (KeyValuePair<RoleTypeId, float> roleFf in ffRules)
            {
                if (overwrite)
                {
                    SetFriendlyFire(roleFf);
                }
                else
                {
                    if (!FriendlyFireMultiplier.ContainsKey(roleFf.Key))
                        temporaryFriendlyFireRules.Add(roleFf.Key, roleFf.Value);
                    else
                        return false; // Contained Key but overwrite set to false so we do not add any.
                }
            }

            if (!overwrite)
            {
                foreach (KeyValuePair<RoleTypeId, float> roleFF in temporaryFriendlyFireRules)
                    TryAddFriendlyFire(roleFF);
            }

            DictionaryPool<RoleTypeId, float>.Pool.Return(temporaryFriendlyFireRules);
            return true;
        }

        /// <summary>
        /// Tries to add <see cref="RoleTypeId"/> to FriendlyFire rules.
        /// </summary>
        /// <param name="roleTypeId"> Role associated for CustomFF. </param>
        /// <param name="roleToAdd"> Role to add. </param>
        /// <param name="ffMult"> Friendly fire multiplier. </param>
        public void SetCustomRoleFriendlyFire(string roleTypeId, RoleTypeId roleToAdd, float ffMult)
        {
            if (CustomRoleFriendlyFireMultiplier.TryGetValue(roleTypeId, out Dictionary<RoleTypeId, float> currentPairedData))
            {
                if (!currentPairedData.ContainsKey(roleToAdd))
                {
                    currentPairedData.Add(roleToAdd, ffMult);
                    return;
                }

                currentPairedData[roleToAdd] = ffMult;
                return;
            }

            CustomRoleFriendlyFireMultiplier.Add(roleTypeId, new() { { roleToAdd, ffMult } });
        }

        /// <summary>
        /// Wrapper to call <see cref="SetCustomRoleFriendlyFire(string, RoleTypeId, float)"/>.
        /// </summary>
        /// <param name="roleTypeId"> Role associated for CustomFF. </param>
        /// <param name="roleFf"> Role with FF to add even if it exists. </param>
        public void SetCustomRoleFriendlyFire(string roleTypeId, KeyValuePair<RoleTypeId, float> roleFf) => SetCustomRoleFriendlyFire(roleTypeId, roleFf.Key, roleFf.Value);

        /// <summary>
        /// Tries to add <see cref="RoleTypeId"/> to FriendlyFire rules for CustomRole.
        /// </summary>
        /// <param name="roleTypeId"> Role associated for CustomFF. </param>
        /// <param name="roleFf"> Role to add and FF multiplier. </param>
        /// <returns> Whether or not the item was able to be added. </returns>
        public bool TryAddCustomRoleFriendlyFire(string roleTypeId, KeyValuePair<RoleTypeId, float> roleFf) => TryAddCustomRoleFriendlyFire(roleTypeId, roleFf.Key, roleFf.Value);

        /// <summary>
        /// Tries to add <see cref="RoleTypeId"/> to FriendlyFire rules for CustomRole.
        /// </summary>
        /// <param name="roleTypeId"> Role associated for CustomFF. </param>
        /// <param name="roleToAdd"> Role to add. </param>
        /// <param name="ffMult"> Friendly fire multiplier. </param>
        /// <returns> Whether or not the item was able to be added. </returns>
        public bool TryAddCustomRoleFriendlyFire(string roleTypeId, RoleTypeId roleToAdd, float ffMult)
        {
            if (CustomRoleFriendlyFireMultiplier.TryGetValue(roleTypeId, out Dictionary<RoleTypeId, float> currentPairedData))
            {
                if (currentPairedData.ContainsKey(roleToAdd))
                    return false;

                currentPairedData.Add(roleToAdd, ffMult);
            }
            else
            {
                SetCustomRoleFriendlyFire(roleTypeId, roleToAdd, ffMult);
            }

            return true;
        }

        /// <summary>
        /// Tries to add <see cref="RoleTypeId"/> to FriendlyFire rules.
        /// </summary>
        /// <param name="customRoleName"> Role associated for CustomFF. </param>
        /// <param name="ffRules"> Roles to add with friendly fire values. </param>
        /// <param name="overwrite"> Whether to overwrite current values if they exist - does NOT delete previous entries if they are not in provided rules. </param>
        /// <returns> Whether or not the item was able to be added. </returns>
        public bool TryAddCustomRoleFriendlyFire(string customRoleName, Dictionary<RoleTypeId, float> ffRules, bool overwrite = false)
        {
            Dictionary<RoleTypeId, float> temporaryFriendlyFireRules = DictionaryPool<RoleTypeId, float>.Pool.Get();

            if (CustomRoleFriendlyFireMultiplier.TryGetValue(customRoleName, out Dictionary<RoleTypeId, float> pairedRoleFF))
            {
                foreach (KeyValuePair<RoleTypeId, float> roleFF in ffRules)
                {
                    if (overwrite)
                    {
                        SetCustomRoleFriendlyFire(customRoleName, roleFF);
                    }
                    else
                    {
                        if (!pairedRoleFF.ContainsKey(roleFF.Key))
                            temporaryFriendlyFireRules.Add(roleFF.Key, roleFF.Value);
                        else
                            return false; // Contained Key but overwrite set to false so we do not add any.
                    }
                }

                if (!overwrite)
                {
                    foreach (KeyValuePair<RoleTypeId, float> roleFf in temporaryFriendlyFireRules)
                        TryAddCustomRoleFriendlyFire(customRoleName, roleFf);
                }
            }
            else
            {
                foreach (KeyValuePair<RoleTypeId, float> roleFf in ffRules)
                    SetCustomRoleFriendlyFire(customRoleName, roleFf);
            }

            DictionaryPool<RoleTypeId, float>.Pool.Return(temporaryFriendlyFireRules);
            return true;
        }

        /// <summary>
        /// Adds the Custom role to the <see cref="CustomRoleFriendlyFireMultiplier"/> if they did not already exist.
        /// </summary>
        /// <param name="customRoleFriendlyFireMultiplier"> Custom role with FF role rules. </param>
        public void TryAddCustomRoleFriendlyFire(Dictionary<string, Dictionary<RoleTypeId, float>> customRoleFriendlyFireMultiplier)
        {
            foreach (KeyValuePair<string, Dictionary<RoleTypeId, float>> newRolesWithFf in customRoleFriendlyFireMultiplier)
                TryAddCustomRoleFriendlyFire(newRolesWithFf.Key, newRolesWithFf.Value);
        }

        /// <summary>
        /// Sets the <see cref="CustomRoleFriendlyFireMultiplier"/>.
        /// </summary>
        /// <param name="customRoleFriendlyFireMultiplier"> New rules for CustomRoleFriendlyFireMultiplier to set to. </param>
        public void TrySetCustomRoleFriendlyFire(Dictionary<string, Dictionary<RoleTypeId, float>> customRoleFriendlyFireMultiplier)
            => CustomRoleFriendlyFireMultiplier = customRoleFriendlyFireMultiplier;

        /// <summary>
        /// Sets the <see cref="CustomRoleFriendlyFireMultiplier"/>.
        /// </summary>
        /// <param name="roleTypeId"> Role to associate FF rules to. </param>
        /// <param name="customRoleFriendlyFireMultiplier"> New rules for CustomRoleFriendlyFireMultiplier to set to. </param>
        public void TrySetCustomRoleFriendlyFire(string roleTypeId, Dictionary<RoleTypeId, float> customRoleFriendlyFireMultiplier) =>
            CustomRoleFriendlyFireMultiplier[roleTypeId] = customRoleFriendlyFireMultiplier;

        /// <summary>
        /// Tries to remove <see cref="RoleTypeId"/> from FriendlyFire rules.
        /// </summary>
        /// <param name="role"> Role to add. </param>
        /// <returns> Whether or not the item was able to be added. </returns>
        public bool TryRemoveFriendlyFire(RoleTypeId role) => FriendlyFireMultiplier.Remove(role);

        /// <summary>
        /// Tries to remove a <see cref="IEnumerable{T}"/> of <see cref="RoleTypeId"/> from FriendlyFire rules.
        /// </summary>
        /// <param name="roles"> The <see cref="IEnumerable{T}"/> of <see cref="RoleTypeId"/> to remove. </param>
        /// <returns> Whether or not all roles were removed from FriendlyFire rules successfully.</returns>
        public bool TryRemoveFriendlyFire(List<RoleTypeId> roles) => roles.All(role => TryRemoveFriendlyFire(role));

        /// <summary>
        /// Tries to remove <see cref="RoleTypeId"/> from FriendlyFire rules.
        /// </summary>
        /// <param name="role"> Role to add. </param>
        /// <returns> Whether or not the item was able to be added. </returns>
        public bool TryRemoveCustomRoleFriendlyFire(string role) => CustomRoleFriendlyFireMultiplier.Remove(role);

        /// <summary>
        /// Forces the player to reload their current <see cref="Firearm"></see>.
        /// </summary>
        /// <returns><see langword="true"/> if firearm was successfully reloaded. Otherwise, <see langword="false"/>.</returns>
        public bool ReloadWeapon()
        {
            if (CurrentItem is Firearm firearm)
            {
                bool result = firearm.Base.AmmoManagerModule.ServerTryReload();
                if (result)
                    Connection.Send(new RequestMessage(firearm.Serial, RequestType.Reload));
                return result;
            }

            return false;
        }

        /// <summary>
        /// Forces the player to unload their current <see cref="Firearm"></see>.
        /// </summary>
        /// <returns><see langword="true"/> if the weapon unload request is received. Returns <see langword="false"/> otherwise, or if the player is not an <see cref="IFpcRole"/> or is not holding a <see cref="Firearm"/>.</returns>
        public bool UnloadWeapon()
        {
            if (CurrentItem is Firearm firearm)
            {
                bool result = firearm.Base.AmmoManagerModule.ServerTryUnload();
                if (result)
                    Connection.Send(new RequestMessage(firearm.Serial, RequestType.Unload));
                return result;
            }

            return true;
        }

        /// <summary>
        /// Forces the player to toggle the Flashlight Attachment on their current <see cref="Firearm"></see>.
        /// </summary>
        /// <returns><see langword="true"/> if the weapon flashlight toggle request is received. Returns <see langword="false"/> otherwise, or if the player is not an <see cref="IFpcRole"/> or is not holding a <see cref="Firearm"/>.</returns>
        public bool ToggleWeaponFlashlight()
        {
            if (RoleManager.CurrentRole is not IFpcRole || CurrentItem is not Firearm firearm)
                return false;

            bool oldCheck = firearm.FlashlightEnabled; // Temporary Solution
            FirearmBasicMessagesHandler.ServerRequestReceived(ReferenceHub.connectionToClient, new RequestMessage(firearm.Serial, RequestType.ToggleFlashlight));
            return oldCheck != firearm.FlashlightEnabled;
        }

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
        /// Tries to get an items from a player's inventory.
        /// </summary>
        /// <param name="predicate">The predicate to satisfy.</param>
        /// <param name="items">The <see cref="IEnumerable{T}"/> found.</param>
        /// <returns><see langword="true"/> if the item is found, <see langword="false"/> otherwise.</returns>
        public bool TryGetItems(Func<Item, bool> predicate, out IEnumerable<Item> items)
        {
            items = Items.Where(predicate);

            return items.Count() != 0;
        }

        /// <summary>
        /// Sets the player's rank.
        /// </summary>
        /// <param name="name">The rank name to be set.</param>
        /// <param name="group">The group to be set.</param>
        public void SetRank(string name, UserGroup group)
        {
            if (ServerStatic.GetPermissionsHandler()._groups.TryGetValue(name, out UserGroup userGroup))
            {
                userGroup.BadgeColor = group.BadgeColor;
                userGroup.BadgeText = name;
                userGroup.HiddenByDefault = !group.Cover;
                userGroup.Cover = group.Cover;

                ReferenceHub.serverRoles.SetGroup(userGroup, false, false);
            }
            else
            {
                ServerStatic.GetPermissionsHandler()._groups.Add(name, group);

                ReferenceHub.serverRoles.SetGroup(group, false, false);
            }

            ServerStatic.GetPermissionsHandler()._members[UserId] = name;
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
            if (cuffer is not null && !IsCuffed && MathExtensions.DistanceSquared(cuffer.Position, Position) <= DisarmingHandlers.ServerDisarmingDistanceSqrt)
                Cuffer = cuffer;
        }

        /// <summary>
        /// Removes the player's handcuffs.
        /// </summary>
        public void RemoveHandcuffs()
        {
            Inventory.SetDisarmedStatus(null);
            new DisarmedPlayersListMessage(DisarmedPlayers.Entries).SendToAuthenticated();
        }

        /// <summary>
        /// Drops an item from the player's inventory.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to be dropped.</param>
        /// <param name="isThrown">Is the item Thrown?.</param>
        public void DropItem(Item item, bool isThrown = false)
        {
            if (item is null)
                return;
            Inventory.UserCode_CmdDropItem__UInt16__Boolean(item.Serial, isThrown);
        }

        /// <summary>
        /// Drops an item from the player's inventory.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to be dropped.</param>
        /// <returns>dropped <see cref="Pickup"/>.</returns>
        public Pickup DropItem(Item item) => item is not null ? Pickup.Get(Inventory.ServerDropItem(item.Serial)) : null;

        /// <summary>
        /// Drops the held item. Will not do anything if the player is not holding an item.
        /// </summary>
        /// <param name="isThrown">Is the item Thrown?.</param>
        public void DropHeldItem(bool isThrown = false) => DropItem(CurrentItem, isThrown);

        /// <summary>
        /// Drops the held item. Will not do anything if the player is not holding an item.
        /// </summary>
        /// <seealso cref="CurrentItem"/>
        /// <returns>Dropped item's <see cref="Pickup"/>.</returns>
        public Pickup DropHeldItem()
        {
            if (CurrentItem is null)
                return null;

            return DropItem(CurrentItem);
        }

        /// <summary>
        /// Indicates whether or not the player has an item.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns><see langword="true"/>, if the player has it; otherwise, <see langword="false"/>.</returns>
        public bool HasItem(Item item) => Items.Contains(item);

        /// <summary>
        /// Indicates whether or not the player has an item type.
        /// </summary>
        /// <param name="type">The type to search for.</param>
        /// <returns><see langword="true"/>, if the player has it; otherwise, <see langword="false"/>.</returns>
        public bool HasItem(ItemType type) => Items.Any(tempItem => tempItem.Type == type);

        /// <summary>
        /// Counts how many items of a certain <see cref="ItemType"/> a player has.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>How many items of that <see cref="ItemType"/> the player has.</returns>
        /// <remarks>For counting ammo, see <see cref="GetAmmo(AmmoType)"/>.</remarks>
        /// <seealso cref="GetAmmo(AmmoType)"/>
        /// <seealso cref="CountItem(ItemCategory)"/>
        public int CountItem(ItemType item) => Items.Count(tempItem => tempItem.Type == item);

        /// <summary>
        /// Counts how many items of a certain <see cref="ProjectileType"/> a player has.
        /// </summary>
        /// <param name="grenadeType">The ProjectileType to search for.</param>
        /// <returns>How many items of that <see cref="ProjectileType"/> the player has.</returns>
        /// <seealso cref="CountItem(ItemType)"/>
        public int CountItem(ProjectileType grenadeType) => Inventory.UserInventory.Items.Count(tempItem => tempItem.Value.ItemTypeId == grenadeType.GetItemType());

        /// <summary>
        /// Counts how many items of a certain <see cref="ItemCategory"/> a player has.
        /// </summary>
        /// <param name="category">The category to search for.</param>
        /// <returns>How many items of that <see cref="ItemCategory"/> the player has.</returns>
        /// <seealso cref="CountItem(ItemType)"/>
        public int CountItem(ItemCategory category) => category switch
        {
            ItemCategory.Ammo => Inventory.UserInventory.ReserveAmmo.Count(ammo => ammo.Value > 0),
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
                return false;

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
                item.ChangeOwner(this, Server.Host);

                if (CurrentItem == item)
                {
                    Inventory.CurInstance = null;
                }

                if (item.Serial == Inventory.CurItem.SerialNumber)
                    Inventory.NetworkCurItem = ItemIdentifier.None;

                Inventory.UserInventory.Items.Remove(item.Serial);
                typeof(InventoryExtensions).InvokeStaticEvent(nameof(InventoryExtensions.OnItemRemoved), new object[] { ReferenceHub, item.Base, null });

                Inventory.SendItemsNextFrame = true;
            }

            return true;
        }

        /// <summary>
        /// Removes an <see cref="Item"/> from the player's inventory.
        /// </summary>
        /// <param name="serial">The <see cref="Item"/> serial to remove.</param>
        /// <param name="destroy">Whether or not to destroy the item.</param>
        /// <returns>A value indicating whether or not the <see cref="Item"/> was removed.</returns>
        public bool RemoveItem(ushort serial, bool destroy = true) => Items.SingleOrDefault(item => item.Serial == serial) is Item item && RemoveItem(item, destroy);

        /// <summary>
        /// Removes an <see cref="Item"/> from the player's inventory.
        /// </summary>
        /// <param name="itemType">The <see cref="ItemType"/> of the <see cref="Item"/>.</param>
        /// <param name="destroy">Whether or not to destroy the item.</param>
        /// <param name="removeAll">Whether or not to remove all items of the specified <see cref="ItemType"/>.</param>
        public void RemoveItem(ItemType itemType, bool destroy = true, bool removeAll = false)
        {
            bool RemoveIfExisting(Item item) => item.Type == itemType && RemoveItem(item, destroy);

            if (removeAll)
            {
                Items.ToList().ForEach(item => RemoveIfExisting(item));
                return;
            }

            foreach (Item item in Items.ToList())
            {
                if (!RemoveIfExisting(item))
                    continue;

                break;
            }
        }

        /// <summary>
        /// Removes all <see cref="Item"/>'s that satisfy the condition from the player's inventory.
        /// </summary>
        /// <param name="predicate">The condition to satisfy.</param>
        /// <param name="destroy">Whether or not to destroy the items.</param>
        /// <returns>Count of a successfully removed <see cref="Item"/>'s.</returns>
        public int RemoveItem(Func<Item, bool> predicate, bool destroy = true)
        {
            List<Item> enumeratedItems = ListPool<Item>.Pool.Get(ItemsValue);
            int count = 0;

            foreach (Item item in enumeratedItems)
            {
                if (predicate(item) && RemoveItem(item, destroy))
                    ++count;
            }

            ListPool<Item>.Pool.Return(enumeratedItems);
            return count;
        }

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
        public void SendConsoleMessage(string message, string color) => referenceHub.gameConsoleTransmission.SendToClient(message, color);

        /// <summary>
        /// Disconnects the player.
        /// </summary>
        /// <param name="reason">The disconnection reason.</param>
        public void Disconnect(string reason = null) =>
            ServerConsole.Disconnect(GameObject, string.IsNullOrEmpty(reason) ? string.Empty : reason);

        /// <summary>
        /// Resets the <see cref="Player"/>'s stamina.
        /// </summary>
        public void ResetStamina() => Stamina = StaminaStat.MaxValue;

        /// <summary>
        /// Gets a user's SCP preference.
        /// </summary>
        /// <param name="roleType">The SCP RoleType.</param>
        /// <returns>A value from <c>-5</c> to <c>5</c>, representing a player's preference to play as the provided SCP. Will return <c>0</c> for invalid SCPs.</returns>
        public int GetScpPreference(RoleTypeId roleType)
        {
            if (ScpPreferences.Preferences.TryGetValue(roleType, out int value))
                return value;

            return 0;
        }

        /// <summary>
        /// Hurts the player.
        /// </summary>
        /// <param name="damageHandlerBase">The <see cref="DamageHandlerBase"/> used to deal damage.</param>
        public void Hurt(DamageHandlerBase damageHandlerBase) => ReferenceHub.playerStats.DealDamage(damageHandlerBase);

        /// <summary>
        /// Hurts the player.
        /// </summary>
        /// <param name="damageHandlerBase">The <see cref="Features.DamageHandlers.DamageHandlerBase"/> used to deal damage.</param>
        public void Hurt(DamageHandlers.DamageHandlerBase damageHandlerBase) => ReferenceHub.playerStats.DealDamage(damageHandlerBase.Base);

        /// <summary>
        /// Hurts the player.
        /// </summary>
        /// <param name="attacker">The <see cref="Player"/> attacking player.</param>
        /// <param name="amount">The <see langword="float"/> amount of damage to deal.</param>
        /// <param name="damageType">The <see cref="DamageType"/> of the damage dealt.</param>
        /// <param name="cassieAnnouncement">The <see cref="CassieAnnouncement"/> cassie announcement to make if the damage kills the player.</param>
        public void Hurt(Player attacker, float amount, DamageType damageType = DamageType.Unknown, CassieAnnouncement cassieAnnouncement = null) =>
            Hurt(new GenericDamageHandler(this, attacker, amount, damageType, cassieAnnouncement));

        /// <summary>
        /// Hurts the player.
        /// </summary>
        /// <param name="attacker">The <see cref="Player"/> attacking player.</param>
        /// <param name="amount">The <see langword="float"/> amount of damage to deal.</param>
        /// <param name="damageType">The <see cref="DamageType"/> of the damage dealt.</param>
        /// <param name="cassieAnnouncement">The <see cref="CassieAnnouncement"/> cassie announcement to make if the damage kills the player.</param>
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
            if (overrideMaxHealth)
                Health += amount;
            else
                healthStat.ServerHeal(amount);
        }

        /// <summary>
        /// Forces the player to use an item.
        /// </summary>
        /// <param name="usableItem">The ItemType to be used.</param>
        /// <returns><see langword="true"/> if item was used successfully. Otherwise, <see langword="false"/>.</returns>
        public bool UseItem(ItemType usableItem) => UseItem(Item.Create(usableItem));

        /// <summary>
        /// Forces the player to use an item.
        /// </summary>
        /// <param name="item">The item to be used.</param>
        /// <returns><see langword="true"/> if item was used successfully. Otherwise, <see langword="false"/>.</returns>
        public bool UseItem(Item item)
        {
            if (item is not Usable usableItem)
                return false;

            usableItem.Base.Owner = referenceHub;
            usableItem.Base.ServerOnUsingCompleted();

            typeof(UsableItemsController).InvokeStaticEvent(nameof(UsableItemsController.ServerOnUsingCompleted), new object[] { referenceHub, usableItem.Base });

            if (usableItem.Base is not null)
                usableItem.Destroy();

            return true;
        }

        /// <summary>
        /// Kills the player.
        /// </summary>
        /// <param name="damageType">The <see cref="DamageType"/> the player has been killed.</param>
        /// <param name="cassieAnnouncement">The cassie announcement to make upon death.</param>
        public void Kill(DamageType damageType, string cassieAnnouncement = "") =>
            Kill(new CustomReasonDamageHandler(DamageTypeExtensions.TranslationConversion.FirstOrDefault(k => k.Value == damageType).Key.LogLabel, -1, cassieAnnouncement));

        /// <summary>
        /// Kills the player.
        /// </summary>
        /// <param name="deathReason">The reason the player has been killed.</param>
        /// <param name="cassieAnnouncement">The cassie announcement to make upon death.</param>
        public void Kill(string deathReason, string cassieAnnouncement = "") => Kill(new CustomReasonDamageHandler(deathReason, -1, cassieAnnouncement));

        /// <summary>
        /// Kills the player.
        /// </summary>
        /// <param name="damageHandlerBase">The <see cref="DamageHandlerBase"/>.</param>
        public void Kill(DamageHandlerBase damageHandlerBase) => ReferenceHub.playerStats.KillPlayer(damageHandlerBase);

        /// <summary>
        /// Kills the player.
        /// </summary>
        /// <param name="handler">A <see cref="DamageHandler"/> to kill the player with.</param>
        public void Kill(DamageHandler handler) => ReferenceHub.playerStats.KillPlayer(handler);

        /// <summary>
        /// Kills the player and vaporizes the body.
        /// </summary>
        /// <param name="attacker">The <see cref="Player"/> attacking player.</param>
        /// <param name="cassieAnnouncement">The cassie announcement to make upon death.</param>
        public void Vaporize(Player attacker = null, string cassieAnnouncement = "")
        {
            if ((Role.Side != Side.Scp) && !string.IsNullOrEmpty(cassieAnnouncement))
                Cassie.Message(cassieAnnouncement);

            Kill(new DisruptorDamageHandler(attacker?.Footprint ?? Footprint, -1));
        }

        /// <summary>
        /// Bans the player.
        /// </summary>
        /// <param name="duration">The ban duration, in seconds.</param>
        /// <param name="reason">The ban reason.</param>
        /// <param name="issuer">The ban issuer.</param>
        public void Ban(int duration, string reason, Player issuer = null)
            => BanPlayer.BanUser(ReferenceHub, issuer is null || issuer.ReferenceHub == null ? Server.Host.ReferenceHub : issuer.ReferenceHub, reason, duration);

        /// <summary>
        /// Bans the player.
        /// </summary>
        /// <param name="duration">The length of time to ban.</param>
        /// <param name="reason">The ban reason.</param>
        /// <param name="issuer">The ban issuer.</param>
        public void Ban(TimeSpan duration, string reason, Player issuer = null) => Ban((int)duration.TotalSeconds, reason, issuer);

        /// <summary>
        /// Kicks the player.
        /// </summary>
        /// <param name="reason">The kick reason.</param>
        /// <param name="issuer">The kick issuer.</param>
        public void Kick(string reason, Player issuer = null) => Ban(0, reason, issuer);

        /// <summary>
        /// Persistently mutes the player. For temporary mutes, see <see cref="IsMuted"/> and <see cref="IsIntercomMuted"/>.
        /// </summary>
        /// <param name="isIntercom">Whether or not this mute is for the intercom only.</param>
        public void Mute(bool isIntercom = false) => VoiceChatMutes.IssueLocalMute(UserId, isIntercom);

        /// <summary>
        /// Revokes a persistent mute. For temporary mutes, see <see cref="IsMuted"/> and <see cref="IsIntercomMuted"/>.
        /// </summary>
        /// <param name="isIntercom">Whether or not this un-mute is for the intercom only.</param>
        public void UnMute(bool isIntercom = false) => VoiceChatMutes.RevokeLocalMute(UserId, isIntercom);

        /// <summary>
        /// Blinks the player's tag.
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
        public void RemoteAdminMessage(string message, bool success = true, string pluginName = null) =>
            Sender.RaReply((pluginName ?? Assembly.GetCallingAssembly().GetName().Name) + "#" + message, success, true, string.Empty);

        /// <summary>
        /// Sends a message to the player's Remote Admin Chat.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="channel">Indicates whether or not the message should be highlighted as success.</param>
        /// <returns><see langword="true"/> if message was send; otherwise, <see langword="false"/>.</returns>
        public bool SendStaffMessage(string message, EncryptedChannelManager.EncryptedChannel channel = EncryptedChannelManager.EncryptedChannel.AdminChat) =>
            ReferenceHub.encryptedChannelManager.TrySendMessageToClient("!" + NetId + message, channel);

        /// <summary>
        /// Sends a message to the player's Remote Admin Chat.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="channel">Indicates whether or not the message should be highlighted as success.</param>
        /// <returns><see langword="true"/> if message was send; otherwise, <see langword="false"/>.</returns>
        public bool SendStaffPing(string message, EncryptedChannelManager.EncryptedChannel channel = EncryptedChannelManager.EncryptedChannel.AdminChat) =>
            ReferenceHub.encryptedChannelManager.TrySendMessageToClient("!0" + message, channel);

        /// <summary>
        /// Shows a broadcast to the player. Doesn't get logged to the console and can be monospaced.
        /// </summary>
        /// <param name="duration">The broadcast duration.</param>
        /// <param name="message">The message to be broadcasted.</param>
        /// <param name="type">The broadcast type.</param>
        /// <param name="shouldClearPrevious">Clears all player's broadcasts before sending the new one.</param>
        public void Broadcast(ushort duration, string message, BroadcastFlags type = BroadcastFlags.Normal, bool shouldClearPrevious = false)
        {
            if (shouldClearPrevious)
                ClearBroadcasts();

            Server.Broadcast.TargetAddElement(Connection, message, duration, type);
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
        public void AddAmmo(FirearmType weaponType, ushort amount) => AddAmmo(weaponType.GetWeaponAmmoType(), amount);

        /// <summary>
        /// Removes an amount of <see cref="AmmoType"/> from the player's inventory.
        /// </summary>
        /// <param name="ammoType">The <see cref="AmmoType"/> to be removed.</param>
        /// <param name="amount">The amount to be removed.</param>
        /// <returns><see langword="true"/> if the specified amount of ammo was removed entirely or partially; otherwise, <see langword="false"/>.</returns>
        public bool RemoveAmmo(AmmoType ammoType, ushort amount)
        {
            if (!Ammo.TryGetValue(ammoType.GetItemType(), out ushort value))
                return false;

            try
            {
                checked
                {
                    value -= amount;
                }
            }
            catch (OverflowException)
            {
                value = ushort.MinValue;
            }

            SetAmmo(ammoType, value);
            return true;
        }

        /// <summary>
        /// Sets the amount of a specified <see cref="AmmoType">ammo type</see> to the player's inventory.
        /// </summary>
        /// <param name="ammoType">The <see cref="AmmoType"/> to be set.</param>
        /// <param name="amount">The amount of ammo to be set.</param>
        public void SetAmmo(AmmoType ammoType, ushort amount)
        {
            ItemType itemType = ammoType.GetItemType();
            if (itemType is not ItemType.None)
                Inventory.ServerSetAmmo(itemType, amount);
        }

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
            Inventory.ServerDropAmmo(ammoType.GetItemType(), amount, checkMinimals).Any();

        /// <summary>
        /// Gets the maximum amount of ammo the player can hold, given the ammo <see cref="AmmoType"/>.
        /// </summary>
        /// <param name="type">The <see cref="AmmoType"/> of the ammo to check.</param>
        /// <param name="ignoreArmor">If the method should ignore the armor the player is wearing.</param>
        /// <returns>The maximum amount of ammo this player can carry.</returns>
        public ushort GetAmmoLimit(AmmoType type, bool ignoreArmor = false)
        {
            if (ignoreArmor)
            {
                if (CustomAmmoLimits.TryGetValue(type, out ushort limit))
                    return limit;

                ItemType itemType = type.GetItemType();
                return ServerConfigSynchronizer.Singleton.AmmoLimitsSync.FirstOrDefault(x => x.AmmoType == itemType).Limit;
            }

            return InventorySystem.Configs.InventoryLimits.GetAmmoLimit(type.GetItemType(), referenceHub);
        }

        /// <summary>
        /// Gets the maximum amount of ammo the player can hold, given the ammo <see cref="AmmoType"/>.
        /// This limit will scale with the armor the player is wearing.
        /// For armor ammo limits, see <see cref="Armor.AmmoLimits"/>.
        /// </summary>
        /// <param name="ammoType">The <see cref="AmmoType"/> of the ammo to check.</param>
        /// <param name="limit">The <see cref="ushort"/> number that will define the new limit.</param>
        public void SetAmmoLimit(AmmoType ammoType, ushort limit)
        {
            CustomAmmoLimits[ammoType] = limit;

            ItemType itemType = ammoType.GetItemType();
            int index = ServerConfigSynchronizer.Singleton.AmmoLimitsSync.FindIndex(x => x.AmmoType == itemType);
            MirrorExtensions.SendFakeSyncObject(this, ServerConfigSynchronizer.Singleton.netIdentity, typeof(ServerConfigSynchronizer), writer =>
            {
                writer.WriteULong(2ul);
                writer.WriteUInt(1);
                writer.WriteByte((byte)SyncList<ServerConfigSynchronizer.AmmoLimit>.Operation.OP_SET);
                writer.WriteInt(index);
                writer.WriteAmmoLimit(new() { Limit = limit, AmmoType = itemType, });
            });
        }

        /// <summary>
        /// Reset a custom <see cref="AmmoType"/> limit.
        /// </summary>
        /// <param name="ammoType">The <see cref="AmmoType"/> of the ammo to reset.</param>
        public void ResetAmmoLimit(AmmoType ammoType)
        {
            if (!HasCustomAmmoLimit(ammoType))
            {
                Log.Error($"{nameof(Player)}.{nameof(ResetAmmoLimit)}(AmmoType): AmmoType.{ammoType} does not have a custom limit.");
                return;
            }

            CustomAmmoLimits.Remove(ammoType);

            ItemType itemType = ammoType.GetItemType();
            int index = ServerConfigSynchronizer.Singleton.AmmoLimitsSync.FindIndex(x => x.AmmoType == itemType);
            MirrorExtensions.SendFakeSyncObject(this, ServerConfigSynchronizer.Singleton.netIdentity, typeof(ServerConfigSynchronizer), writer =>
            {
                writer.WriteULong(2ul);
                writer.WriteUInt(1);
                writer.WriteByte((byte)SyncList<ServerConfigSynchronizer.AmmoLimit>.Operation.OP_SET);
                writer.WriteInt(index);
                writer.WriteAmmoLimit(ServerConfigSynchronizer.Singleton.AmmoLimitsSync[index]);
            });
        }

        /// <summary>
        /// Check if the player has a custom limit for a specific <see cref="AmmoType"/>.
        /// </summary>
        /// <param name="ammoType">The <see cref="AmmoType"/> to check.</param>
        /// <returns>If the player has a custom limit for the specific <see cref="AmmoType"/>.</returns>
        public bool HasCustomAmmoLimit(AmmoType ammoType) => CustomAmmoLimits.ContainsKey(ammoType);

        /// <summary>
        /// Gets the maximum amount of an <see cref="ItemCategory"/> the player can hold, based on the armor the player is wearing, as well as server configuration.
        /// </summary>
        /// <param name="category">The <see cref="ItemCategory"/> to check.</param>
        /// <param name="ignoreArmor">If the method should ignore the armor the player is wearing.</param>
        /// <returns>The maximum amount of items in the category that the player can hold.</returns>
        public sbyte GetCategoryLimit(ItemCategory category, bool ignoreArmor = false)
        {
            int index = InventorySystem.Configs.InventoryLimits.StandardCategoryLimits.Where(x => x.Value >= 0).OrderBy(x => x.Key).ToList().FindIndex(x => x.Key == category);

            if (ignoreArmor && index != -1)
            {
                if (CustomCategoryLimits.TryGetValue(category, out sbyte customLimit))
                    return customLimit;

                return ServerConfigSynchronizer.Singleton.CategoryLimits[index];
            }

            sbyte limit = InventorySystem.Configs.InventoryLimits.GetCategoryLimit(category, referenceHub);

            return limit == -1 ? (sbyte)1 : limit;
        }

        /// <summary>
        /// Set the maximum amount of an <see cref="ItemCategory"/> the player can hold. Only works with <see cref="ItemCategory.Keycard"/>, <see cref="ItemCategory.Medical"/>, <see cref="ItemCategory.Firearm"/>, <see cref="ItemCategory.Grenade"/> and <see cref="ItemCategory.SCPItem"/>.
        /// This limit will scale with the armor the player is wearing.
        /// For armor category limits, see <see cref="Armor.CategoryLimits"/>.
        /// </summary>
        /// <param name="category">The <see cref="ItemCategory"/> to check.</param>
        /// <param name="limit">The <see cref="int"/> number that will define the new limit.</param>
        public void SetCategoryLimit(ItemCategory category, sbyte limit)
        {
            int index = InventorySystem.Configs.InventoryLimits.StandardCategoryLimits.Where(x => x.Value >= 0).OrderBy(x => x.Key).ToList().FindIndex(x => x.Key == category);

            if (index == -1)
            {
                Log.Error($"{nameof(Player)}.{nameof(SetCategoryLimit)}(ItemCategory, sbyte): Cannot set category limit for ItemCategory.{category}.");
                return;
            }

            CustomCategoryLimits[category] = limit;

            MirrorExtensions.SendFakeSyncObject(this, ServerConfigSynchronizer.Singleton.netIdentity, typeof(ServerConfigSynchronizer), writer =>
            {
                writer.WriteULong(1ul);
                writer.WriteUInt(1);
                writer.WriteByte((byte)SyncList<sbyte>.Operation.OP_SET);
                writer.WriteInt(index);
                writer.WriteSByte(limit);
            });
        }

        /// <summary>
        /// Reset a custom <see cref="ItemCategory"/> limit. Only works with <see cref="ItemCategory.Keycard"/>, <see cref="ItemCategory.Medical"/>, <see cref="ItemCategory.Firearm"/>, <see cref="ItemCategory.Grenade"/> and <see cref="ItemCategory.SCPItem"/>.
        /// </summary>
        /// <param name="category">The <see cref="ItemCategory"/> of the category to reset.</param>
        public void ResetCategoryLimit(ItemCategory category)
        {
            int index = InventorySystem.Configs.InventoryLimits.StandardCategoryLimits.Where(x => x.Value >= 0).OrderBy(x => x.Key).ToList().FindIndex(x => x.Key == category);

            if (index == -1)
            {
                Log.Error($"{nameof(Player)}.{nameof(ResetCategoryLimit)}(ItemCategory, sbyte): Cannot reset category limit for ItemCategory.{category}.");
                return;
            }

            if (!HasCustomCategoryLimit(category))
            {
                Log.Error($"{nameof(Player)}.{nameof(ResetCategoryLimit)}(ItemCategory): ItemCategory.{category} does not have a custom limit.");
                return;
            }

            CustomCategoryLimits.Remove(category);

            MirrorExtensions.SendFakeSyncObject(this, ServerConfigSynchronizer.Singleton.netIdentity, typeof(ServerConfigSynchronizer), writer =>
            {
                writer.WriteULong(1ul);
                writer.WriteUInt(1);
                writer.WriteByte((byte)SyncList<sbyte>.Operation.OP_SET);
                writer.WriteInt(index);
                writer.WriteSByte(ServerConfigSynchronizer.Singleton.CategoryLimits[index]);
            });
        }

        /// <summary>
        /// Check if the player has a custom limit for a specific <see cref="ItemCategory"/>.
        /// </summary>
        /// <param name="category">The <see cref="ItemCategory"/> to check.</param>
        /// <returns>If the player has a custom limit for the specific <see cref="ItemCategory"/>.</returns>
        public bool HasCustomCategoryLimit(ItemCategory category) => CustomCategoryLimits.ContainsKey(category);

        /// <summary>
        /// Adds an item of the specified type with default durability(ammo/charge) and no mods to the player's inventory.
        /// </summary>
        /// <param name="itemType">The item to be added.</param>
        /// <returns>The <see cref="Item"/> given to the player.</returns>
        public Item AddItem(ItemType itemType)
        {
            if (itemType.GetFirearmType() is not FirearmType.None)
            {
                return AddItem(itemType.GetFirearmType(), null);
            }

            Item item = Item.Create(itemType);

            AddItem(item);

            return item;
        }

        /// <summary>
        /// Adds an firearm of the specified type with default durability(ammo/charge) and no mods to the player's inventory.
        /// </summary>
        /// <param name="firearmType">The firearm to be added.</param>
        /// <param name="identifiers">The attachments to be added to the item.</param>
        /// <returns>The <see cref="Item"/> given to the player.</returns>
        public Item AddItem(FirearmType firearmType, IEnumerable<AttachmentIdentifier> identifiers)
        {
            Item item = Item.Create(firearmType.GetItemType());

            if (item is Firearm firearm)
            {
                if (identifiers is not null)
                    firearm.AddAttachment(identifiers);
                else if (Preferences is not null && Preferences.TryGetValue(firearmType, out AttachmentIdentifier[] attachments))
                    firearm.Base.ApplyAttachmentsCode(attachments.GetAttachmentsCode(), true);

                FirearmStatusFlags flags = FirearmStatusFlags.MagazineInserted;

                if (firearm.Attachments.Any(a => a.Name == AttachmentName.Flashlight))
                    flags = flags.AddFlags(FirearmStatusFlags.FlashlightEnabled);

                firearm.Base.Status = new FirearmStatus(firearm.MaxAmmo, flags, firearm.Base.GetCurrentAttachmentsCode());
            }

            AddItem(item);

            return item;
        }

        /// <summary>
        /// Adds the amount of items of the specified type with default durability(ammo/charge) and no mods to the player's inventory.
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
        /// Adds the amount of firearms of the specified type with default durability(ammo/charge) and no mods to the player's inventory.
        /// </summary>
        /// <param name="firearmType">The item to be added.</param>
        /// <param name="amount">The amount of items to be added.</param>
        /// <param name="identifiers">The attachments to be added to the item.</param>
        /// <returns>An <see cref="IEnumerable{Item}"/> containing the items given.</returns>
        public IEnumerable<Item> AddItem(FirearmType firearmType, int amount, IEnumerable<AttachmentIdentifier> identifiers)
        {
            List<Item> items = new(amount > 0 ? amount : 0);

            if (amount > 0)
            {
                IEnumerable<AttachmentIdentifier> attachmentIdentifiers = identifiers.ToList();

                for (int i = 0; i < amount; i++)
                    items.Add(AddItem(firearmType, attachmentIdentifiers));
            }

            return items;
        }

        /// <summary>
        /// Adds the list of items of the specified type with default durability(ammo/charge) and no mods to the player's inventory.
        /// </summary>
        /// <param name="items">The list of items to be added.</param>
        /// <returns>An <see cref="IEnumerable{Item}"/> containing the items given.</returns>
        public IEnumerable<Item> AddItem(IEnumerable<ItemType> items)
        {
            List<ItemType> enumeratedItems = ListPool<ItemType>.Pool.Get(items);
            List<Item> returnedItems = new(enumeratedItems.Count);

            foreach (ItemType type in enumeratedItems)
                returnedItems.Add(AddItem(type));

            ListPool<ItemType>.Pool.Return(enumeratedItems);
            return returnedItems;
        }

        /// <summary>
        /// Adds the list of items of the specified type with default durability(ammo/charge) and no mods to the player's inventory.
        /// </summary>
        /// <param name="items">The <see cref="Dictionary{TKey, TValue}"/> of <see cref="ItemType"/> and <see cref="IEnumerable{T}"/> of <see cref="AttachmentIdentifier"/> to be added.</param>
        /// <returns>An <see cref="IEnumerable{Item}"/> containing the items given.</returns>
        public IEnumerable<Item> AddItem(Dictionary<FirearmType, IEnumerable<AttachmentIdentifier>> items)
        {
            List<Item> returnedItems = new(items.Count);

            foreach (KeyValuePair<FirearmType, IEnumerable<AttachmentIdentifier>> item in items)
                returnedItems.Add(AddItem(item.Key, item.Value));

            return returnedItems;
        }

        /// <summary>
        /// Adds an item to the player's inventory.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        public void AddItem(Item item)
        {
            try
            {
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
        /// <param name="item">The item to be added.</param>
        /// <param name="identifiers">The attachments to be added to the item.</param>
        public void AddItem(Firearm item, IEnumerable<AttachmentIdentifier> identifiers)
        {
            try
            {
                if (identifiers is not null)
                    item.AddAttachment(identifiers);

                AddItem(item.Base, item);
            }
            catch (Exception exception)
            {
                Log.Error($"{nameof(Player)}.{nameof(AddItem)}(Item): {exception}");
            }
        }

        /// <summary>
        /// Adds an item to the player's inventory.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> of the item to be added.</param>
        /// <returns>The <see cref="Item"/> that was added.</returns>
        public Item AddItem(Pickup pickup) => Item.Get(Inventory.ServerAddItem(pickup.Type, pickup.Serial, pickup.Base));

        /// <summary>
        /// Adds an item to the player's inventory.
        /// </summary>
        /// <param name="pickup">The <see cref="FirearmPickup"/> of the item to be added.</param>
        /// <param name="identifiers">The attachments to be added to <see cref="Pickup"/> of the item.</param>
        /// <returns>The <see cref="Item"/> that was added.</returns>
        public Item AddItem(FirearmPickup pickup, IEnumerable<AttachmentIdentifier> identifiers)
        {
            Firearm firearm = (Firearm)Item.Get(Inventory.ServerAddItem(pickup.Type, pickup.Serial, pickup.Base));

            if (identifiers is not null)
                firearm.AddAttachment(identifiers);

            return firearm;
        }

        /// <summary>
        /// Adds an item to the player's inventory.
        /// </summary>
        /// <param name="itemBase">The item to be added.</param>
        /// <param name="item">The <see cref="Item"/> object of the item.</param>
        /// <returns>The <see cref="Item"/> that was added.</returns>
        public Item AddItem(ItemBase itemBase, Item item = null)
        {
            try
            {
                item ??= Item.Get(itemBase);

                Inventory.UserInventory.Items[item.Serial] = itemBase;

                item.ChangeOwner(item.Owner, this);

                if (itemBase is IAcquisitionConfirmationTrigger acquisitionConfirmationTrigger)
                {
                    acquisitionConfirmationTrigger.AcquisitionAlreadyReceived = false;
                }

                typeof(InventoryExtensions).InvokeStaticEvent(nameof(InventoryExtensions.OnItemAdded), new object[] { ReferenceHub, itemBase, null });

                Inventory.SendItemsNextFrame = true;
                return item;
            }
            catch (Exception exception)
            {
                Log.Error($"{nameof(Player)}.{nameof(AddItem)}(ItemBase, [Item]): {exception}");
            }

            return null;
        }

        /// <summary>
        /// Adds the list of items to the player's inventory.
        /// </summary>
        /// <param name="items">The list of items to be added.</param>
        public void AddItem(IEnumerable<Item> items)
        {
            foreach (Item item in items)
                AddItem(item);
        }

        /// <summary>
        /// Adds the list of items to the player's inventory.
        /// </summary>
        /// <param name="firearms">The <see cref="Dictionary{TKey, TValue}"/> of <see cref="Firearm"/> and <see cref="IEnumerable{T}"/> of <see cref="AttachmentIdentifier"/> to be added.</param>
        public void AddItem(Dictionary<Firearm, IEnumerable<AttachmentIdentifier>> firearms)
        {
            if (firearms.Count > 0)
            {
                foreach (KeyValuePair<Firearm, IEnumerable<AttachmentIdentifier>> item in firearms)
                    AddItem(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Grants the player their current role's loadout.
        /// </summary>
        public void GrantLoadout() => GrantLoadout(Role.Type);

        /// <summary>
        /// Grants a player a role's loadout.
        /// </summary>
        /// <param name="role">The role loadout to give.</param>
        public void GrantLoadout(RoleTypeId role)
        {
            InventoryRoleInfo info = role.GetStartingInventory();

            AddItem(info.Items);
            info.Ammo.ForEach(a => AddAmmo(a.Key.GetAmmoType(), a.Value));
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

            Timing.CallDelayed(0.02f, () =>
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
            ClearItems();

            foreach (ItemType item in newItems)
                AddItem(item);
        }

        /// <summary>
        /// Resets the player's inventory to the provided list of items, clearing any items it already possess.
        /// </summary>
        /// <param name="newItems">The new items that have to be added to the inventory.</param>
        public void ResetInventory(IEnumerable<Item> newItems)
        {
            ClearItems();

            foreach (Item item in newItems)
                AddItem(item);
        }

        /// <summary>
        /// Resets the player's inventory to the provided inventory, clearing any items/ammo it already possess.
        /// </summary>
        /// <param name="inventory">The role inventory to add to the inventory.</param>
        public void ResetInventory(InventoryRoleInfo inventory)
        {
            ClearInventory();

            foreach (ItemType item in inventory.Items)
                AddItem(item);

            foreach (KeyValuePair<ItemType, ushort> ammo in inventory.Ammo)
                AddAmmo(ammo.Key.GetAmmoType(), ammo.Value);
        }

        /// <summary>
        /// Clears the player's inventory, including all ammo and items.
        /// </summary>
        /// <param name="destroy">Whether or not to fully destroy the old items.</param>
        /// <seealso cref="ResetInventory(IEnumerable{Item})"/>
        /// <seealso cref="ResetInventory(IEnumerable{ItemType})"/>
        /// <seealso cref="DropItems()"/>
        public void ClearInventory(bool destroy = true)
        {
            ClearItems(destroy);
            ClearAmmo();
        }

        /// <summary>
        /// Clears the player's items.
        /// </summary>
        /// <param name="destroy">Whether or not to fully destroy the old items.</param>
        /// <seealso cref="ResetInventory(IEnumerable{Item})"/>
        /// <seealso cref="ResetInventory(IEnumerable{ItemType})"/>
        /// <seealso cref="DropItems()"/>
        public void ClearItems(bool destroy = true)
        {
            if (CurrentArmor is not null)
                CurrentArmor.RemoveExcessOnDrop = true;

            while (Items.Count > 0)
                RemoveItem(Items.ElementAt(0), destroy);
        }

        /// <summary>
        /// Clears all ammo in the inventory.
        /// </summary>
        /// <seealso cref="ResetInventory(IEnumerable{Item})"/>
        /// <seealso cref="SetAmmo(AmmoType, ushort)"/>
        /// <seealso cref="DropItems()"/>
        public void ClearAmmo()
        {
            ReferenceHub.inventory.UserInventory.ReserveAmmo.Clear();
            ReferenceHub.inventory.SendAmmoNextFrame = true;
        }

        /// <summary>
        /// Drops all items in the player's inventory, including all ammo and items.
        /// </summary>
        /// <seealso cref="ClearInventory(bool)"/>
        public void DropItems() => Inventory.ServerDropEverything();

        /// <summary>
        /// Forces the player to throw a grenade.
        /// </summary>
        /// <param name="type">The <see cref="ProjectileType"/> to be thrown.</param>
        /// <param name="fullForce">Whether to throw with full or half force.</param>
        /// <returns>The <see cref="Throwable"/> item that was spawned.</returns>
        public Throwable ThrowGrenade(ProjectileType type, bool fullForce = true)
        {
            Throwable throwable = type switch
            {
                ProjectileType.Flashbang => new FlashGrenade(),
                ProjectileType.Scp2176 => new Scp2176(),
                _ => new ExplosiveGrenade(type.GetItemType()),
            };

            ThrowItem(throwable, fullForce);
            return throwable;
        }

        /// <summary>
        /// Forcefully throws a <paramref name="throwable"/> item.
        /// </summary>
        /// <param name="throwable">The <see cref="Throwable"/> to be thrown.</param>
        /// <param name="fullForce">Whether to throw with full or half force.</param>
        public void ThrowItem(Throwable throwable, bool fullForce = true)
        {
            throwable.Base.Owner = ReferenceHub;
            throwable.Throw(fullForce);
        }

        /// <summary>
        /// Shows a hint to the player.
        /// </summary>
        /// <param name="message">The message to be shown.</param>
        /// <param name="duration">The duration the text will be on screen.</param>
        public void ShowHint(string message, float duration = 3f)
        {
            message ??= string.Empty;
            HintDisplay.Show(new TextHint(message, new HintParameter[] { new StringHintParameter(message) }, null, duration));
        }

        /// <summary>
        /// Show a hint to the player.
        /// </summary>
        /// <param name="hint">The hint to be shown.</param>
        public void ShowHint(Hint hint)
        {
            if (hint.Show)
                ShowHint(hint.Content, hint.Duration);
        }

        /// <inheritdoc cref="TextDisplay.Show(Player, object[])"/>
        public void ShowTextDisplay(TextDisplay textDisplay, params object[] args) => textDisplay.Show(this, args);

        /// <inheritdoc cref="TextDisplay.Show(Player, string, ushort, TextChannelType, object[])"/>
        public void ShowTextDisplay(string content, ushort duration, TextChannelType textChannel, params object[] args) => TextDisplay.Show(this, content, duration, textChannel, args);

        /// <summary>
        /// Sends a HitMarker to the player.
        /// </summary>
        /// <param name="size">The size of the hitmarker, ranging from <c>0</c> to <c><see cref="Hitmarker.MaxSize"/></c>).</param>
        public void ShowHitMarker(float size = 1f) =>
            Hitmarker.SendHitmarkerDirectly(ReferenceHub, size);

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
        /// Plays the Hume Shield break sound effect from the player.
        /// </summary>
        /// <remarks>This will only function if the player's <see cref="FpcRole.IsHumeShieldedRole"/> is <see langword="true"/>.</remarks>
        public void PlayShieldBreakSound()
        => new PlayerRoles.PlayableScps.HumeShield.DynamicHumeShieldController.ShieldBreakMessage() { Target = ReferenceHub }.SendToAuthenticated();

        /// <summary>
        /// Gets a <see cref="StatBase"/> module from the player's <see cref="PlayerStats"/> component.
        /// </summary>
        /// <typeparam name="T">The returned object type.</typeparam>
        /// <returns>The <typeparamref name="T"/> module that was requested.</returns>
        public T GetModule<T>()
            where T : StatBase
            => ReferenceHub.playerStats.GetModule<T>();

        /// <summary>
        /// Gets a <see cref="bool"/> describing whether or not the given <see cref="StatusEffectBase">status effect</see> is currently enabled.
        /// </summary>
        /// <typeparam name="T">The <see cref="StatusEffectBase"/> to check.</typeparam>
        /// <returns>A <see cref="bool"/> determining whether or not the player effect is active.</returns>
        public bool IsEffectActive<T>()
            where T : StatusEffectBase
        {
            if (ReferenceHub.playerEffectsController._effectsByType.TryGetValue(typeof(T), out StatusEffectBase playerEffect))
                return playerEffect.IsEnabled;

            return false;
        }

        /// <summary>
        /// Disables all currently active <see cref="StatusEffectBase">status effects</see>.
        /// </summary>
        /// <seealso cref="DisableEffects(IEnumerable{EffectType})"/>
        public void DisableAllEffects()
        {
            foreach (StatusEffectBase effect in ReferenceHub.playerEffectsController.AllEffects)
                effect.IsEnabled = false;
        }

        /// <summary>
        /// Disables all currently active <see cref="StatusEffectBase">status effects</see>.
        /// </summary>
        /// <param name="category">A category to filter the disabled effects.</param>
        /// <seealso cref="DisableAllEffects()"/>
        public void DisableAllEffects(EffectCategory category)
        {
            if (category is EffectCategory.None)
                return;

            foreach (KeyValuePair<Type, StatusEffectBase> effect in ReferenceHub.playerEffectsController._effectsByType)
            {
                if (Enum.TryParse(effect.Key.Name, out EffectType effectType) && effectType.GetCategories().HasFlag(category))
                    effect.Value.IsEnabled = false;
            }
        }

        /// <summary>
        /// Disables a specific <see cref="StatusEffectBase">status effect</see> on the player.
        /// </summary>
        /// <typeparam name="T">The <see cref="StatusEffectBase"/> to disable.</typeparam>
        public void DisableEffect<T>()
            where T : StatusEffectBase => ReferenceHub.playerEffectsController.DisableEffect<T>();

        /// <summary>
        /// Disables a specific <see cref="EffectType">status effect</see> on the player.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/> to disable.</param>
        public void DisableEffect(EffectType effect)
        {
            if (TryGetEffect(effect, out StatusEffectBase playerEffect))
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
        /// Enables a <see cref="StatusEffectBase">status effect</see> on the player.
        /// </summary>
        /// <typeparam name="T">The <see cref="StatusEffectBase"/> to enable.</typeparam>
        /// <param name="duration">The amount of time the effect will be active for.</param>
        /// <param name="addDurationIfActive">If the effect is already active, setting to <see langword="true"/> will add this duration onto the effect.</param>
        /// <returns>A bool indicating whether or not the effect was valid and successfully enabled.</returns>
        public bool EnableEffect<T>(float duration = 0f, bool addDurationIfActive = false)
                    where T : StatusEffectBase => EnableEffect<T>(1, duration, addDurationIfActive);

        /// <summary>
        /// Enables a <see cref="StatusEffectBase">status effect</see> on the player.
        /// </summary>
        /// <typeparam name="T">The <see cref="StatusEffectBase"/> to enable.</typeparam>
        /// <param name="intensity">The intensity of the effect will be active for.</param>
        /// <param name="duration">The amount of time the effect will be active for.</param>
        /// <param name="addDurationIfActive">If the effect is already active, setting to <see langword="true"/> will add this duration onto the effect.</param>
        /// <returns>A bool indicating whether or not the effect was valid and successfully enabled.</returns>
        public bool EnableEffect<T>(byte intensity, float duration = 0f, bool addDurationIfActive = false)
            where T : StatusEffectBase => ReferenceHub.playerEffectsController.ChangeState<T>(intensity, duration, addDurationIfActive);

        /// <summary>
        /// Enables a <see cref="StatusEffectBase">status effect</see> on the player.
        /// </summary>
        /// <param name="statusEffect">The name of the <see cref="StatusEffectBase"/> to enable.</param>
        /// <param name="duration">The amount of time the effect will be active for.</param>
        /// <param name="addDurationIfActive">If the effect is already active, setting to <see langword="true"/> will add this duration onto the effect.</param>
        /// <returns>A bool indicating whether or not the effect was valid and successfully enabled.</returns>
        public bool EnableEffect(StatusEffectBase statusEffect, float duration = 0f, bool addDurationIfActive = false)
            => EnableEffect(statusEffect, 1, duration, addDurationIfActive);

        /// <summary>
        /// Enables a <see cref="StatusEffectBase">status effect</see> on the player.
        /// </summary>
        /// <param name="statusEffect">The name of the <see cref="StatusEffectBase"/> to enable.</param>
        /// <param name="intensity">The intensity of the effect will be active for.</param>
        /// <param name="duration">The amount of time the effect will be active for.</param>
        /// <param name="addDurationIfActive">If the effect is already active, setting to <see langword="true"/> will add this duration onto the effect.</param>
        /// <returns>A bool indicating whether or not the effect was valid and successfully enabled.</returns>
        public bool EnableEffect(StatusEffectBase statusEffect, byte intensity, float duration = 0f, bool addDurationIfActive = false)
        {
            if (statusEffect is null)
                return false;

            statusEffect.ServerSetState(intensity, duration, addDurationIfActive);

            return statusEffect.IsEnabled;
        }

        /// <summary>
        /// Enables a <see cref="StatusEffectBase">status effect</see> on the player.
        /// </summary>
        /// <param name="effectName">The name of the <see cref="StatusEffectBase"/> to enable.</param>
        /// <param name="duration">The amount of time the effect will be active for.</param>
        /// <param name="addDurationIfActive">If the effect is already active, setting to <see langword="true"/> will add this duration onto the effect.</param>
        /// <returns>The <see cref="StatusEffectBase"/> instance of the activated effect.</returns>
        public StatusEffectBase EnableEffect(string effectName, float duration = 0f, bool addDurationIfActive = false)
            => EnableEffect(effectName, 1, duration, addDurationIfActive);

        /// <summary>
        /// Enables a <see cref="StatusEffectBase">status effect</see> on the player.
        /// </summary>
        /// <param name="effectName">The name of the <see cref="StatusEffectBase"/> to enable.</param>
        /// <param name="intensity">The intensity of the effect will be active for.</param>
        /// <param name="duration">The amount of time the effect will be active for.</param>
        /// <param name="addDurationIfActive">If the effect is already active, setting to <see langword="true"/> will add this duration onto the effect.</param>
        /// <returns>The <see cref="StatusEffectBase"/> instance of the activated effect.</returns>
        public StatusEffectBase EnableEffect(string effectName, byte intensity, float duration = 0f, bool addDurationIfActive = false)
            => ReferenceHub.playerEffectsController.ChangeState(effectName, intensity, duration, addDurationIfActive);

        /// <summary>
        /// Enables a <see cref="EffectType">status effect</see> on the player.
        /// </summary>
        /// <param name="type">The <see cref="EffectType"/> to enable.</param>
        /// <param name="duration">The amount of time the effect will be active for.</param>
        /// <param name="addDurationIfActive">If the effect is already active, setting to <see langword="true"/> will add this duration onto the effect.</param>
        public void EnableEffect(EffectType type, float duration = 0f, bool addDurationIfActive = false)
            => EnableEffect(type, 1, duration, addDurationIfActive);

        /// <summary>
        /// Enables a <see cref="EffectType">status effect</see> on the player.
        /// </summary>
        /// <param name="type">The <see cref="EffectType"/> to enable.</param>
        /// <param name="intensity">The intensity of the effect will be active for.</param>
        /// <param name="duration">The amount of time the effect will be active for.</param>
        /// <param name="addDurationIfActive">If the effect is already active, setting to <see langword="true"/> will add this duration onto the effect.</param>
        /// <returns>return if the effect has been Enable.</returns>
        public bool EnableEffect(EffectType type, byte intensity, float duration = 0f, bool addDurationIfActive = false)
            => TryGetEffect(type, out StatusEffectBase statusEffect) && EnableEffect(statusEffect, intensity, duration, addDurationIfActive);

        /// <summary>
        /// Syncs the <see cref="Effect">status effect</see> on the player.
        /// </summary>
        /// <param name="effect">The <see cref="Effect"/> to sync.</param>
        public void SyncEffect(Effect effect)
        {
            if (effect.IsEnabled)
            {
                EnableEffect(effect.Type, effect.Intensity, effect.Duration, effect.AddDurationIfActive);
            }
        }

        /// <summary>
        /// Enables a random <see cref="EffectType"/> on the player.
        /// </summary>
        /// <param name="category">An optional category to filter the applied effect. Set to <see cref="EffectCategory.None"/> for any effect.</param>
        /// <param name="duration">The amount of time the effect will be active for.</param>
        /// <param name="addDurationIfActive">If the effect is already active, setting to <see langword="true"/> will add this duration onto the effect.</param>
        /// <returns>A <see cref="EffectType"/> that was given to the player.</returns>
        public EffectType ApplyRandomEffect(EffectCategory category = EffectCategory.None, float duration = 0f, bool addDurationIfActive = false)
            => ApplyRandomEffect(category, 1, duration, addDurationIfActive);

        /// <summary>
        /// Enables a random <see cref="EffectType"/> on the player.
        /// </summary>
        /// <param name="category">An optional category to filter the applied effect. Set to <see cref="EffectCategory.None"/> for any effect.</param>
        /// <param name="intensity">The intensity of the effect will be active for.</param>
        /// <param name="duration">The amount of time the effect will be active for.</param>
        /// <param name="addDurationIfActive">If the effect is already active, setting to <see langword="true"/> will add this duration onto the effect.</param>
        /// <returns>A <see cref="EffectType"/> that was given to the player.</returns>
        public EffectType ApplyRandomEffect(EffectCategory category, byte intensity, float duration = 0f, bool addDurationIfActive = false)
        {
            IEnumerable<EffectType> validEffects = EnumExtensions.QueryValues<EffectType>().Where(effect => effect.GetCategories().HasFlag(category));
            EffectType effectType = validEffects.Random();

            EnableEffect(effectType, intensity, duration, addDurationIfActive);

            return effectType;
        }

        /// <summary>
        /// Enables a <see cref="IEnumerable{T}"/> of <see cref="EffectType"/> on the player.
        /// </summary>
        /// <param name="types">The <see cref="IEnumerable{T}"/> of <see cref="EffectType"/> to enable.</param>
        /// <param name="duration">The amount of time the effects will be active for.</param>
        /// <param name="addDurationIfActive">If an effect is already active, setting to <see langword="true"/> will add this duration onto the effect.</param>
        public void EnableEffects(IEnumerable<EffectType> types, float duration = 0f, bool addDurationIfActive = false)
        {
            foreach (EffectType type in types)
            {
                if (TryGetEffect(type, out StatusEffectBase statusEffect))
                    EnableEffect(statusEffect, duration, addDurationIfActive);
            }
        }

        /// <summary>
        /// Syncs a <see cref="IEnumerable{T}"/> of <see cref="Effect"/> on the player.
        /// </summary>
        /// <param name="effects">The <see cref="IEnumerable{T}"/> of <see cref="Effect"/> to enable.</param>
        public void SyncEffects(IEnumerable<Effect> effects)
        {
            foreach (Effect effect in effects)
                SyncEffect(effect);
        }

        /// <summary>
        /// Gets an instance of <see cref="StatusEffectBase"/> by <see cref="EffectType"/>.
        /// </summary>
        /// <param name="effectType">The <see cref="EffectType"/>.</param>
        /// <returns>The <see cref="StatusEffectBase"/>.</returns>
        public StatusEffectBase GetEffect(EffectType effectType)
        {
            if (!effectType.TryGetType(out Type type))
                return null;
            ReferenceHub.playerEffectsController._effectsByType.TryGetValue(type, out StatusEffectBase playerEffect);
            return playerEffect;
        }

        /// <summary>
        /// Tries to get an instance of <see cref="StatusEffectBase"/> by <see cref="EffectType"/>.
        /// </summary>
        /// <param name="type">The <see cref="EffectType"/>.</param>
        /// <param name="statusEffect">The <see cref="StatusEffectBase"/>.</param>
        /// <returns>A bool indicating whether or not the <paramref name="statusEffect"/> was successfully gotten.</returns>
        public bool TryGetEffect(EffectType type, out StatusEffectBase statusEffect)
        {
            statusEffect = GetEffect(type);

            return statusEffect is not null;
        }

        /// <summary>
        /// Tries to get an instance of <see cref="StatusEffectBase"/> by <see cref="EffectType"/>.
        /// </summary>
        /// <param name="statusEffect">The <see cref="StatusEffectBase"/>.</param>
        /// <typeparam name="T">The <see cref="StatusEffectBase"/> to get.</typeparam>
        /// <returns>A bool indicating whether or not the <paramref name="statusEffect"/> was successfully gotten.</returns>
        public bool TryGetEffect<T>(out T statusEffect)
            where T : StatusEffectBase
            => ReferenceHub.playerEffectsController.TryGetEffect(out statusEffect);

        /// <summary>
        /// Gets a <see cref="byte"/> indicating the intensity of the given <see cref="StatusEffectBase"></see>.
        /// </summary>
        /// <typeparam name="T">The <see cref="StatusEffectBase"/> to check.</typeparam>
        /// <exception cref="ArgumentException">Thrown if the given type is not a valid <see cref="StatusEffectBase"/>.</exception>
        /// <returns>The intensity of the effect.</returns>
        public byte GetEffectIntensity<T>()
            where T : StatusEffectBase
        {
            if (ReferenceHub.playerEffectsController._effectsByType.TryGetValue(typeof(T), out StatusEffectBase statusEffect))
                return statusEffect.Intensity;

            throw new ArgumentException("The given type is invalid.");
        }

        /// <summary>
        /// Changes the intensity of a <see cref="StatusEffectBase">status effect</see>.
        /// </summary>
        /// <typeparam name="T">The <see cref="StatusEffectBase"/> to change the intensity of.</typeparam>
        /// <param name="intensity">The intensity of the effect.</param>
        /// <param name="duration">The new duration to add to the effect.</param>
        /// <param name="addDuration">Whether or not to add the duration..</param>
        public void ChangeEffectIntensity<T>(byte intensity, float duration = 0, bool addDuration = false)
            where T : StatusEffectBase
        {
            if (TryGetEffect(out T statusEffect))
            {
                statusEffect.Intensity = intensity;
                statusEffect.ServerChangeDuration(duration, addDuration);
            }
        }

        /// <summary>
        /// Changes the intensity of a <see cref="StatusEffectBase"/>.
        /// </summary>
        /// <param name="type">The <see cref="EffectType"/> to change.</param>
        /// <param name="intensity">The new intensity to use.</param>
        /// <param name="duration">The new duration to add to the effect.</param>
        /// <param name="addDuration">Whether or not to add the duration..</param>
        public void ChangeEffectIntensity(EffectType type, byte intensity, float duration = 0, bool addDuration = false)
        {
            if (TryGetEffect(type, out StatusEffectBase statusEffect))
            {
                statusEffect.Intensity = intensity;
                statusEffect.ServerChangeDuration(duration, addDuration);
            }
        }

        /// <summary>
        /// Increases intensity of a <see cref="StatusEffectBase"/>.
        /// </summary>
        /// <param name="intensity">The intensity to add.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="addDuration">Whether or not to add the duration on top.</param>
        /// <typeparam name="T">The <see cref="StatusEffectBase"/> to increase intensity of.</typeparam>
        public void AddEffectIntensity<T>(byte intensity, float duration = 0, bool addDuration = true)
            where T : StatusEffectBase
        {
            if (TryGetEffect(out T statusEffect))
            {
                intensity = (byte)Mathf.Clamp(statusEffect.Intensity + intensity, byte.MinValue, byte.MaxValue);

                statusEffect.Intensity = intensity;
                statusEffect.ServerChangeDuration(duration, addDuration);
            }
        }

        /// <summary>
        /// Increases intensity of a <see cref="StatusEffectBase"/>.
        /// </summary>
        /// <param name="type">The <see cref="EffectType"/>to increase intensity of.</param>
        /// <param name="intensity">The intensity to add.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="addDuration">Whether or not to add the duration on top.</param>
        public void AddEffectIntensity(EffectType type, byte intensity, float duration = 0, bool addDuration = true)
        {
            if (TryGetEffect(type, out StatusEffectBase statusEffect))
            {
                intensity = (byte)Mathf.Clamp(statusEffect.Intensity + intensity, byte.MinValue, byte.MaxValue);

                statusEffect.Intensity = intensity;
                statusEffect.ServerChangeDuration(duration, addDuration);
            }
        }

        /// <summary>
        /// Changes the intensity of a <see cref="StatusEffectBase">status effect</see>.
        /// </summary>
        /// <param name="effectName">The name of the <see cref="StatusEffectBase"/> to enable.</param>
        /// <param name="intensity">The intensity of the effect.</param>
        /// <param name="duration">The new length of the effect. Defaults to infinite length.</param>
        public void ChangeEffectIntensity(string effectName, byte intensity, float duration = 0)
        {
            if (Enum.TryParse(effectName, out EffectType type))
                ChangeEffectIntensity(type, intensity, duration);
        }

        /// <summary>
        /// Gets an instance of <see cref="DangerStackBase"/> by <see cref="DangerType"/> if the Scp1853 effect is enabled or null if it is not enabled.
        /// </summary>
        /// <param name="dangerType">The <see cref="DangerType"/>.</param>
        /// <returns>The <see cref="DangerStackBase"/>.</returns>
        public DangerStackBase GetDanger(DangerType dangerType) => Dangers.FirstOrDefault(danger => danger.TryGetDangerType(out DangerType type) && dangerType == type);

        /// <summary>
        /// Tries to get an instance of <see cref="StatusEffectBase"/> by <see cref="EffectType"/> (does not work if the Scp1853 effect is not enabled).
        /// </summary>
        /// <param name="type">The <see cref="EffectType"/>.</param>
        /// <param name="danger">The <see cref="StatusEffectBase"/>.</param>
        /// <returns>A bool indicating whether or not the <paramref name="danger"/> was successfully gotten.</returns>
        public bool TryGetDanger(DangerType type, out DangerStackBase danger) => (danger = GetDanger(type)) is not null;

        /// <summary>
        /// Opens the report window.
        /// </summary>
        /// <param name="text">The text to send.</param>
        public void OpenReportWindow(string text) => SendConsoleMessage($"[REPORTING] {text}", "white");

        /// <summary>
        /// Places a Tantrum (SCP-173's ability) under the player.
        /// </summary>
        /// <param name="isActive">Whether or not the tantrum will apply the <see cref="EffectType.Stained"/> effect.</param>
        /// <remarks>If <paramref name="isActive"/> is <see langword="true"/>, the tantrum is moved slightly up from its original position. Otherwise, the collision will not be detected and the slowness will not work.</remarks>
        /// <returns>The <see cref="TantrumHazard"/> instance..</returns>
        public TantrumHazard PlaceTantrum(bool isActive = true) => TantrumHazard.CreateAndSpawn(Position, isActive);

        /// <summary>
        /// Gives a new <see cref="AhpStat">to the player</see>.
        /// </summary>
        /// <param name="amount">The amount to give the player.</param>
        /// <param name="limit">The maximum AHP for this stat.</param>
        /// <param name="decay">How much value is lost per second.</param>
        /// <param name="efficacy">Percent of incoming damage absorbed by this stat.</param>
        /// <param name="sustain">The number of seconds to delay the start of the decay.</param>
        /// <param name="persistant">Whether or not the process is removed when the value hits 0.</param>
        /// <returns>The <see cref="AhpStat.AhpProcess"/> that was added.</returns>
        public AhpStat.AhpProcess AddAhp(float amount, float limit = 75f, float decay = 1.2f, float efficacy = 0.7f, float sustain = 0f, bool persistant = false)
            => AhpStat.ServerAddProcess(amount, limit, decay, efficacy, sustain, persistant);

        /// <summary>
        /// Reconnects the player to the server. Can be used to redirect them to another server on a different port but same IP.
        /// </summary>
        /// <param name="newPort">New port.</param>
        /// <param name="delay">Player reconnection delay.</param>
        /// <param name="reconnect">Whether or not player should be reconnected.</param>
        /// <param name="roundRestartType">Type of round restart.</param>
        public void Reconnect(ushort newPort = 0, float delay = 5, bool reconnect = true, RoundRestartType roundRestartType = RoundRestartType.FullRestart)
        {
            if (newPort != 0)
                roundRestartType = newPort == Server.Port && roundRestartType is RoundRestartType.RedirectRestart ? RoundRestartType.FullRestart : RoundRestartType.RedirectRestart;

            Connection.Send(new RoundRestartMessage(roundRestartType, delay, newPort, reconnect, false));
        }

        /// <inheritdoc cref="PlayGunSound(Vector3, ItemType, byte, byte)"/>
        public void PlayGunSound(ItemType type, byte volume, byte audioClipId = 0) =>
            PlayGunSound(Position, type, volume, audioClipId);

        /// <inheritdoc cref="Map.PlaceBlood(Vector3, Vector3)"/>
        public void PlaceBlood(Vector3 direction) => Map.PlaceBlood(Position, direction);

        /// <inheritdoc cref="Map.GetNearCameras(Vector3, float)"/>
        public IEnumerable<Camera> GetNearCameras(float toleration = 15f) => Map.GetNearCameras(Position, toleration);

        /// <summary>
        /// Teleports the player to the given <see cref="Vector3"/> coordinates.
        /// </summary>
        /// <param name="position">The <see cref="Vector3"/> coordinates to move the player to.</param>
        public void Teleport(Vector3 position) => Position = position;

        /// <summary>
        /// Teleports the player to the given object, with no offset.
        /// </summary>
        /// <param name="obj">The object to teleport to.</param>
        public void Teleport(object obj)
            => Teleport(obj, Vector3.zero);

        /// <summary>
        /// Teleports the player to the given object, offset by the defined offset value.
        /// </summary>
        /// <param name="obj">The object to teleport the player to.</param>
        /// <param name="offset">The offset to teleport.</param>
        public void Teleport(object obj, Vector3 offset)
        {
            switch (obj)
            {
                case TeslaGate teslaGate:
                    Teleport(
                        teslaGate.Position + offset + Vector3.up +
                        (teslaGate.Room.Transform.rotation == Quaternion.identity
                            ? new Vector3(3, 0, 0)
                            : new Vector3(0, 0, 3)));
                    break;
                case Chamber chamber:
                    Teleport(chamber.UseMultipleSpawnpoints ? chamber.Spawnpoints.Random().transform.position : chamber.Spawnpoint.transform.position + Vector3.up + offset);
                    break;
                case Role role:
                    if (role.Owner is not null)
                        Teleport(role.Owner.Position + offset);
                    else
                        Log.Warn($"{nameof(Teleport)}: {Assembly.GetCallingAssembly().GetName().Name}: Invalid role teleport (role is missing Owner).");
                    break;
                case Item item:
                    if (item.Owner is not null)
                        Teleport(item.Owner.Position + offset);
                    else
                        Log.Warn($"{nameof(Teleport)}: {Assembly.GetCallingAssembly().GetName().Name}: Invalid item teleport (item is missing Owner).");
                    break;
                case GameEntity entity:
                    Teleport(entity.Position + Vector3.up + offset);
                    break;
                case IPosition positionObject:
                    Teleport(positionObject.Position + Vector3.up + offset);
                    break;
                case DoorType doorType:
                    Teleport(Door.Get(doorType).Position + Vector3.up + offset);
                    break;
                case SpawnLocationType sp:
                    Teleport(sp.GetPosition() + offset);
                    break;
                case RoomType roomType:
                    Teleport(Room.Get(roomType).Position + Vector3.up + offset);
                    break;
                case Enums.CameraType cameraType:
                    Teleport(Camera.Get(cameraType).Position + offset);
                    break;
                case ElevatorType elevatorType:
                    Teleport(Lift.Get(elevatorType).Position + Vector3.up + offset);
                    break;
                case Scp914Controller scp914:
                    Teleport(scp914._knobTransform.position + Vector3.up + offset);
                    break;
                case ElevatorChamber elevator:
                    Teleport(elevator.transform.position + Vector3.up + offset);
                    break;

                // Unity
                case Vector3 v3: // I wouldn't be surprised if someone calls this method with a Vector3.
                    Teleport(v3 + offset);
                    break;
                case Component comp:
                    Teleport(comp.transform.position + Vector3.up + offset);
                    break;
                case GameObject go:
                    Teleport(go.transform.position + Vector3.up + offset);
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
            object randomObject = type.Name switch
            {
                nameof(Camera) => Camera.Random,
                nameof(Door) => Door.Random(),
                nameof(Room) => Room.Random(),
                nameof(TeslaGate) => TeslaGate.Random,
                nameof(Player) => Random,
                nameof(Pickup) => Pickup.Random,
                nameof(Ragdoll) => Ragdoll.Random,
                nameof(Lockers.Locker) => Map.GetRandomLocker(),
                nameof(Generator) => Generator.Random,
                nameof(Window) => Window.Random,
                nameof(Scp914) => Scp914.Scp914Controller,
                nameof(Chamber) => Map.GetRandomLocker().Chambers.Random(),
                _ => null,
            };

            Teleport(randomObject);
        }

        /// <summary>
        /// Teleports the player to a random object.
        /// </summary>
        /// <param name="types">The list of object types to choose from.</param>
        public void RandomTeleport(IEnumerable<Type> types)
        {
            Type[] array = types as Type[] ?? types.ToArray();

            if (array.Length == 0)
                return;

            RandomTeleport(array.Random());
        }

        /// <summary>
        /// Teleports player to a random object of a specific type.
        /// </summary>
        /// <typeparam name="T">Object for teleport.</typeparam>
        public void RandomTeleport<T>() => RandomTeleport(typeof(T));

        /// <summary>
        /// Retrieves the cooldown time for the specified ItemType.
        /// </summary>
        /// <param name="itemType">The ItemType to retrieve the cooldown for.</param>
        /// <returns>The cooldown time in seconds, or -1 if not found.</returns>
        public float GetCooldownItem(ItemType itemType)
            => UsableItemsController.GetHandler(ReferenceHub).PersonalCooldowns.TryGetValue(itemType, out float value) ? value : -1;

        /// <summary>
        /// Sets the cooldown time for the specified ItemType.
        /// </summary>
        /// <param name="time">The cooldown time in seconds.</param>
        /// <param name="itemType">The ItemType to set the cooldown for.</param>
        public void SetCooldownItem(float time, ItemType itemType)
            => UsableItemsController.GetHandler(ReferenceHub).PersonalCooldowns[itemType] = Time.timeSinceLevelLoad + time;

        /// <summary>
        /// Triggers an explosion for the player.
        /// </summary>
        public void Explode() => ExplosionUtils.ServerExplode(ReferenceHub);

        /// <summary>
        /// Triggers an explosion for the player.
        /// </summary>
        /// <param name="projectileType">The type of projectile causing the explosion.</param>
        /// <param name="attacker">The player triggering the explosion.</param>
        public void Explode(ProjectileType projectileType, Player attacker = null) => Map.Explode(Position, projectileType, attacker);

        /// <summary>
        /// Spawns an explosion effect for the player.
        /// </summary>
        /// <param name="projectileType">The type of projectile creating the effect.</param>
        public void ExplodeEffect(ProjectileType projectileType) => Map.ExplodeEffect(Position, projectileType);

        /// <summary>
        /// Sets a new rank given the specified values.
        /// </summary>
        /// <param name="rankName">The name of the rank to be set.</param>
        /// <param name="rankColor">The color of the rank.</param>
        public void SetRank(string rankName, string rankColor)
        {
            RankName = rankName;
            RankColor = rankColor;
        }

        /// <summary>
        /// Plays a beep sound that only the player can hear.
        /// </summary>
        public void PlayBeepSound() => MirrorExtensions.SendFakeTargetRpc(ReferenceHub, ReferenceHub.HostHub.networkIdentity, typeof(AmbientSoundPlayer), nameof(AmbientSoundPlayer.RpcPlaySound), 7);

        /// <summary>
        /// Plays a gun sound that only the target can hear.
        /// </summary>
        /// <param name="position">Position to play on.</param>
        /// <param name="itemType">Weapon' sound to play.</param>
        /// <param name="volume">Sound's volume to set.</param>
        /// <param name="audioClipId">GunAudioMessage's audioClipId to set (default = 0).</param>
        public void PlayGunSound(Vector3 position, ItemType itemType, byte volume, byte audioClipId = 0)
        {
            GunAudioMessage message = new()
            {
                Weapon = itemType,
                AudioClipId = audioClipId,
                MaxDistance = volume,
                ShooterHub = ReferenceHub,
                ShooterPosition = new RelativePosition(position),
            };

            Connection.Send(message);
        }

        /// <summary>
        /// Set <see cref="CustomInfo"/> to the player that only <paramref name="target"/> can see.
        /// </summary>
        /// <param name="target">Target to set info.</param>
        /// <param name="info">Setting info.</param>
        public void SetPlayerInfoForTargetOnly(Player target, string info) => MirrorExtensions.SendFakeTargetRpc(ReferenceHub, target.ReferenceHub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_customPlayerInfoString), info);

        /// <summary>
        /// Set <see cref="DisplayNickname"/> to the player that only <paramref name="target"/> can see.
        /// </summary>
        /// <param name="target">Target to set info.</param>
        /// <param name="name">Nickname to set.</param>
        public void SetNameForTargetOnly(Player target, string name) => target.SendFakeSyncVar(NetworkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_displayName), name);

        /// <summary>
        /// Change <see cref="Player"/> character model for appearance.
        /// It will continue until <see cref="Player"/>'s <see cref="RoleTypeId"/> changes.
        /// </summary>
        /// <param name="type">Model type.</param>
        /// <param name="skipJump">Whether or not to skip the little jump that works around an invisibility issue.</param>
        /// <param name="unitId">The UnitNameId to use for the player's new role, if the player's new role uses unit names. (is NTF).</param>
        public void ChangeAppearance(RoleTypeId type, bool skipJump = false, byte unitId = 0) => ChangeAppearance(type, List.Where(x => x.ReferenceHub != ReferenceHub), skipJump, unitId);

        /// <summary>
        /// Change <see cref="Player"/> character model for appearance.
        /// It will continue until <see cref="Player"/>'s <see cref="RoleTypeId"/> changes.
        /// </summary>
        /// <param name="type">Model type.</param>
        /// <param name="playersToAffect">The players who should see the changed appearance.</param>
        /// <param name="skipJump">Whether or not to skip the little jump that works around an invisibility issue.</param>
        /// <param name="unitId">The UnitNameId to use for the player's new role, if the player's new role uses unit names. (is NTF).</param>
        public void ChangeAppearance(RoleTypeId type, IEnumerable<Player> playersToAffect, bool skipJump = false, byte unitId = 0)
        {
            if (ReferenceHub.gameObject == null || !RoleExtensions.TryGetRoleBase(type, out PlayerRoleBase roleBase))
                return;
            bool isRisky = type.GetRoleBase().Team is Team.Dead || IsDead;

            NetworkWriterPooled writer = NetworkWriterPool.Get();
            writer.WriteUShort(38952);
            writer.WriteUInt(NetId);
            writer.WriteRoleType(type);

            if (roleBase is PHumanRole humanRole && humanRole.UsesUnitNames)
            {
                if (Role.Base is not PHumanRole)
                    isRisky = true;
                writer.WriteByte(unitId);
            }

            if (roleBase is FpcStandardRoleBase fpc)
            {
                if (Role.Base is not FpcStandardRoleBase playerfpc)
                    isRisky = true;
                else
                    fpc = playerfpc;

                ushort value = 0;
                fpc?.FpcModule.MouseLook.GetSyncValues(0, out value, out ushort _);
                writer.WriteRelativePosition(RelativePosition);
                writer.WriteUShort(value);
            }

            if (roleBase is ZombieRole)
            {
                if (Role.Base is not ZombieRole)
                    isRisky = true;

                writer.WriteUShort((ushort)Mathf.Clamp(Mathf.CeilToInt(MaxHealth), ushort.MinValue, ushort.MaxValue));
            }

            foreach (Player target in playersToAffect)
            {
                if (target.ReferenceHub != ReferenceHub || !isRisky)
                    target.Connection.Send(writer.ToArraySegment());
                else
                    Log.Error($"Prevent Seld-Desync of {Nickname} with {type}");
            }

            NetworkWriterPool.Return(writer);

            // To counter a bug that makes the player invisible until they move after changing their appearance, we will teleport them upwards slightly to force a new position update for all clients.
            if (!skipJump)
                Position += Vector3.up * 0.25f;
        }

        /// <summary>
        /// Send CASSIE announcement that only <see cref="Player"/> can hear.
        /// </summary>
        /// <param name="words">Announcement words.</param>
        /// <param name="makeHold">Same on <see cref="Cassie.Message(string, bool, bool, bool)"/>'s isHeld.</param>
        /// <param name="makeNoise">Same on <see cref="Cassie.Message(string, bool, bool, bool)"/>'s isNoisy.</param>
        /// <param name="isSubtitles">Same on <see cref="Cassie.Message(string, bool, bool, bool)"/>'s isSubtitles.</param>
        public void PlayCassieAnnouncement(string words, bool makeHold = false, bool makeNoise = true, bool isSubtitles = false)
        {
            foreach (RespawnEffectsController controller in RespawnEffectsController.AllControllers)
            {
                if (controller != null)
                {
                    MirrorExtensions.SendFakeTargetRpc(ReferenceHub, controller.netIdentity, typeof(RespawnEffectsController), nameof(RespawnEffectsController.RpcCassieAnnouncement), words, makeHold, makeNoise, isSubtitles);
                }
            }
        }

        /// <summary>
        /// Send CASSIE announcement with custom subtitles for translation that only <see cref="Player"/> can hear and see it.
        /// </summary>
        /// <param name="words">The message to be reproduced.</param>
        /// <param name="translation">The translation should be show in the subtitles.</param>
        /// <param name="makeHold">Same on <see cref="Cassie.MessageTranslated(string, string, bool, bool, bool)"/>'s isHeld.</param>
        /// <param name="makeNoise">Same on <see cref="Cassie.MessageTranslated(string, string, bool, bool, bool)"/>'s isNoisy.</param>
        /// <param name="isSubtitles">Same on <see cref="Cassie.MessageTranslated(string, string, bool, bool, bool)"/>'s isSubtitles.</param>
        public void SendCassieAnnouncement(string words, string translation, bool makeHold = false, bool makeNoise = true, bool isSubtitles = true)
        {
            StringBuilder announcement = StringBuilderPool.Pool.Get();

            string[] cassies = words.Split('\n');
            string[] translations = translation.Split('\n');

            for (int i = 0; i < cassies.Length; i++)
                announcement.Append($"{translations[i].Replace(' ', ' ')}<size=0> {cassies[i]} </size><split>");

            string message = StringBuilderPool.Pool.ToStringReturn(announcement);

            foreach (RespawnEffectsController controller in RespawnEffectsController.AllControllers)
            {
                if (controller != null)
                {
                    MirrorExtensions.SendFakeTargetRpc(ReferenceHub, controller.netIdentity, typeof(RespawnEffectsController), nameof(RespawnEffectsController.RpcCassieAnnouncement), message, makeHold, makeNoise, isSubtitles);
                }
            }
        }

        /// <summary>
        /// Sends to the player a Fake Change Scene.
        /// </summary>
        /// <param name="newSceneName">The new Scene the client will load.</param>
        public void SendFakeSceneLoading(string newSceneName)
        {
            SceneMessage message = new()
            {
                sceneName = newSceneName,
            };

            Connection.Send(message);
        }

        /// <summary>
        /// Sends to the player a Fake Change Scene.
        /// </summary>
        /// <param name="newSceneName">The new Scene the client will load.</param>
        public void SendFakeSceneLoading(ScenesType newSceneName) => SendFakeSceneLoading(newSceneName.ToString());

        /// <summary>
        /// Scale object for the player.
        /// </summary>
        /// <param name="identity">The <see cref="Mirror.NetworkIdentity"/> to scale.</param>
        /// <param name="scale">The scale the object needs to be.</param>
        public void ScaleNetworkIdentityObject(NetworkIdentity identity, Vector3 scale)
        {
            identity.gameObject.transform.localScale = scale;
            ObjectDestroyMessage objectDestroyMessage = new()
            {
                netId = identity.netId,
            };

            Connection.Send(objectDestroyMessage, 0);
            MirrorExtensions.SendSpawnMessageMethodInfo?.Invoke(null, new object[] { identity, Connection });
        }

        /// <summary>
        /// Converts the player in a human-readable format.
        /// </summary>
        /// <returns>A string containing Player-related data.</returns>
        public override string ToString() => $"{Id} ({Nickname}) [{UserId}] *{(Role is null ? "No role" : Role)}*";
    }
}
