// -----------------------------------------------------------------------
// <copyright file="Player.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using CustomPlayerEffects;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Items;

    using Footprinting;

    using Hints;

    using InventorySystem;
    using InventorySystem.Disarming;
    using InventorySystem.Items;
    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Firearms.BasicMessages;

    using MEC;

    using Mirror;
    using Mirror.LiteNetLib4Mirror;

    using NorthwoodLib;
    using NorthwoodLib.Pools;

    using PlayableScps;
    using PlayableScps.ScriptableObjects;

    using PlayerStatsSystem;

    using RemoteAdmin;

    using UnityEngine;

    using Utils.Networking;

    using Firearm = Exiled.API.Features.Items.Firearm;

    /// <summary>
    /// Represents the in-game player, by encapsulating a <see cref="global::ReferenceHub"/>.
    /// </summary>
    public class Player {
#pragma warning disable SA1401
        /// <summary>
        /// A list of the player's items.
        /// </summary>
        internal readonly List<Item> ItemsValue = new List<Item>(8);
#pragma warning restore SA1401

        private readonly IReadOnlyCollection<Item> readOnlyItems;
        private ReferenceHub referenceHub;
        private CustomHealthStat healthStat;

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="referenceHub">The <see cref="global::ReferenceHub"/> of the player to be encapsulated.</param>
        public Player(ReferenceHub referenceHub) {
            readOnlyItems = ItemsValue.AsReadOnly();
            ReferenceHub = referenceHub;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="gameObject">The <see cref="UnityEngine.GameObject"/> of the player.</param>
        public Player(GameObject gameObject) {
            readOnlyItems = ItemsValue.AsReadOnly();
            ReferenceHub = ReferenceHub.GetHub(gameObject);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Player"/> class.
        /// </summary>
        ~Player() => HashSetPool<int>.Shared.Return(TargetGhostsHashSet);

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing all <see cref="Player"/>'s on the server.
        /// </summary>
        public static Dictionary<GameObject, Player> Dictionary { get; } = new Dictionary<GameObject, Player>(20);

        /// <summary>
        /// Gets a list of all <see cref="Player"/>'s on the server.
        /// </summary>
        public static IEnumerable<Player> List => Dictionary.Values;

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing cached <see cref="Player"/> and their user ids.
        /// </summary>
        public static Dictionary<string, Player> UserIdsCache { get; } = new Dictionary<string, Player>(20);

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing cached <see cref="Player"/> and their ids.
        /// </summary>
        public static Dictionary<int, Player> IdsCache { get; } = new Dictionary<int, Player>(20);

        /// <summary>
        /// Gets the encapsulated <see cref="global::ReferenceHub"/>.
        /// </summary>
        public ReferenceHub ReferenceHub {
            get => referenceHub;
            private set {
                if (value == null)
                    throw new NullReferenceException("Player's ReferenceHub cannot be null!");

                referenceHub = value;
                GameObject = value.gameObject;
                HintDisplay = value.hints;
                Inventory = value.inventory;
                CameraTransform = value.PlayerCameraReference;

                value.playerStats.StatModules[0] = healthStat = new CustomHealthStat();
                if (!value.playerStats._dictionarizedTypes.ContainsKey(typeof(HealthStat)))
                    value.playerStats._dictionarizedTypes.Add(typeof(HealthStat), healthStat);
            }
        }

        /// <summary>
        /// Gets the player's ammo.
        /// </summary>
        public Dictionary<ItemType, ushort> Ammo => Inventory.UserInventory.ReserveAmmo;

        /// <summary>
        /// Gets the encapsulated <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        public GameObject GameObject { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the player is viewing a hint.
        /// </summary>
        public bool HasHint { get; internal set; }

        /// <summary>
        /// Gets the encapsulated <see cref="ReferenceHub"/>'s <see cref="global::Radio"/>.
        /// </summary>
        public global::Radio Radio => ReferenceHub.radio;

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
        public Assets._Scripts.Dissonance.DissonanceUserSetup DissonanceUserSetup => referenceHub.dissonanceUserSetup;

        /// <summary>
        /// Gets or sets the player's id.
        /// </summary>
        public int Id {
            get => ReferenceHub.queryProcessor.NetworkPlayerId;
            set => ReferenceHub.queryProcessor.NetworkPlayerId = value;
        }

        /// <summary>
        /// Gets the player's user id.
        /// </summary>
        public string UserId => referenceHub.characterClassManager.UserId;

        /// <summary>
        /// Gets or sets the player's custom user id.
        /// </summary>
        public string CustomUserId {
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
        public string AuthenticationToken => ReferenceHub.characterClassManager.AuthToken;

        /// <inheritdoc cref="Enums.AuthenticationType"/>
        public AuthenticationType AuthenticationType {
            get {
                if (string.IsNullOrEmpty(UserId))
                    return AuthenticationType.Unknown;

                int index = UserId.LastIndexOf('@');

                if (index == -1)
                    return AuthenticationType.Unknown;

                switch (UserId.Substring(index + 1)) {
                    case "steam":
                        return AuthenticationType.Steam;

                    case "discord":
                        return AuthenticationType.Discord;

                    case "northwood":
                        return AuthenticationType.Northwood;

                    case "patreon":
                        return AuthenticationType.Patreon;

                    default:
                        return AuthenticationType.Unknown;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the player is verified.
        /// </summary>
        /// <remarks>
        /// This is always false if online_mode is set to false.
        /// </remarks>
        public bool IsVerified { get; internal set; }

        /// <summary>
        /// Gets or sets the player's display nickname.
        /// May be null.
        /// </summary>
        public string DisplayNickname {
            get => ReferenceHub.nicknameSync.Network_displayName;
            set => ReferenceHub.nicknameSync.Network_displayName = value;
        }

        /// <summary>
        /// Gets the player's nickname.
        /// </summary>
        public string Nickname => ReferenceHub.nicknameSync.Network_myNickSync;

        /// <summary>
        /// Gets or sets the player's player info area bitmask.
        /// You can hide player info elements with this.
        /// </summary>
        public PlayerInfoArea InfoArea {
            get => ReferenceHub.nicknameSync.Network_playerInfoToShow;
            set => ReferenceHub.nicknameSync.Network_playerInfoToShow = value;
        }

        /// <summary>
        /// Gets or sets the player's custom player info string.
        /// </summary>
        public string CustomInfo {
            get => ReferenceHub.nicknameSync.Network_customPlayerInfoString;
            set => ReferenceHub.nicknameSync.Network_customPlayerInfoString = value;
        }

        /// <summary>
        /// Gets the dictionary of player's session variables. It is not being saved on player disconnect.
        /// </summary>
        public Dictionary<string, object> SessionVariables { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets a value indicating whether the player is invisible.
        /// </summary>
        public bool IsInvisible { get; set; }

        /// <summary>
        /// Gets a value indicating whether the player can be tracked.
        /// </summary>
        public bool DoNotTrack => ReferenceHub.serverRoles.DoNotTrack;

        /// <summary>
        /// Gets a value indicating whether the player is connected to the server.
        /// </summary>
        public bool IsConnected => GameObject != null;

        /// <summary>
        /// Gets a list of player ids who can't see the player.
        /// </summary>
        public HashSet<int> TargetGhostsHashSet { get; } = HashSetPool<int>.Shared.Rent();

        /// <summary>
        /// Gets a value indicating whether the player has Remote Admin access.
        /// </summary>
        public bool RemoteAdminAccess => ReferenceHub.serverRoles.RemoteAdmin;

        /// <summary>
        /// Gets or sets a value indicating whether the player's overwatch is enabled.
        /// </summary>
        public bool IsOverwatchEnabled {
            get => ReferenceHub.serverRoles.OverwatchEnabled;
            set => ReferenceHub.serverRoles.SetOverwatchStatus(value);
        }

        /// <summary>
        /// Gets or sets a value indicating the cuffer <see cref="Player"/>.
        /// </summary>
        public Player Cuffer {
            get {
                foreach (DisarmedPlayers.DisarmedEntry disarmed in DisarmedPlayers.Entries) {
                    if (Get(disarmed.DisarmedPlayer) == this)
                        return Get(disarmed.Disarmer);
                }

                return null;
            }

            set {
                for (int i = 0; i < DisarmedPlayers.Entries.Count; i++) {
                    if (DisarmedPlayers.Entries[i].DisarmedPlayer == Inventory.netId) {
                        DisarmedPlayers.Entries.RemoveAt(i);
                        break;
                    }
                }

                if (value != null) {
                    Inventory.SetDisarmedStatus(value.Inventory);
                    new DisarmedPlayersListMessage(DisarmedPlayers.Entries).SendToAuthenticated();
                }
            }
        }

        /// <summary>
        /// Gets or sets the player's position.
        /// </summary>
        public Vector3 Position {
            get => ReferenceHub.playerMovementSync.GetRealPosition();
            set => ReferenceHub.playerMovementSync.OverridePosition(value, 0f);
        }

        /// <summary>
        /// Gets or sets the player's rotations.
        /// </summary>
        /// <returns>Returns a <see cref="Vector2"/> representing the rotation of the player.</returns>
        public Vector2 Rotations {
            get => ReferenceHub.playerMovementSync.RotationSync;
            set => ReferenceHub.playerMovementSync.RotationSync = value;
        }

        /// <summary>
        /// Gets or sets the player's rotation.
        /// </summary>
        /// <returns>Returns the direction he's looking at, useful for Raycasts.</returns>
        public Vector3 Rotation {
            get => ReferenceHub.PlayerCameraReference.forward;
            set => ReferenceHub.PlayerCameraReference.forward = value;
        }

        /// <summary>
        /// Gets the player's <see cref="Team"/>.
        /// </summary>
        public Team Team => Role.GetTeam();

        /// <summary>
        /// Gets the player's <see cref="LeadingTeam"/>.
        /// </summary>
        public LeadingTeam LeadingTeam => Team.GetLeadingTeam();

        /// <summary>
        /// Gets or sets the player's <see cref="RoleType"/>.
        /// </summary>
        public RoleType Role {
            get => ReferenceHub.characterClassManager.NetworkCurClass;
            set => SetRole(value);
        }

        /// <summary>
        /// Gets the <see cref="Color"/> of the player's <see cref="RoleType">role</see>.
        /// </summary>
        public Color RoleColor => Role.GetColor();

        /// <summary>
        /// Gets a value indicating whether the player is cuffed. Note: players can be cuffed without another player being the cuffer.
        /// </summary>
        public bool IsCuffed => Cuffer != null;

        /// <summary>
        /// Gets a value indicating whether the player is reloading a weapon.
        /// </summary>
        public bool IsReloading => CurrentItem is Firearm firearm && !firearm.Base.AmmoManagerModule.Standby;

        /// <summary>
        /// Gets a value indicating whether the player is aiming with a weapon.
        /// </summary>
        public bool IsAimingDownWeapon => CurrentItem is Firearm firearm && firearm.Aiming;

        /// <summary>
        /// Gets a value indicating whether the player has enabled weapon's flashlight module.
        /// </summary>
        public bool HasFlashlightModuleEnabled => CurrentItem is Firearm firearm && firearm.FlashlightEnabled;

        /// <summary>
        /// Gets or sets the player's current <see cref="PlayerMovementState"/>.
        /// </summary>
        public PlayerMovementState MoveState {
            get => ReferenceHub.animationController.MoveState;
            set => ReferenceHub.animationController.MoveState = value;
        }

        /// <summary>
        /// Gets a value indicating whether the player is jumping.
        /// </summary>
        public bool IsJumping => ReferenceHub.fpc.isJumping;

        /// <summary>
        /// Gets a value indicating whether the player is sprinting.
        /// </summary>
        public bool IsSprinting => MoveState == PlayerMovementState.Sprinting;

        /// <summary>
        /// Gets a value indicating whether the player is walking.
        /// </summary>
        public bool IsWalking => MoveState == PlayerMovementState.Walking;

        /// <summary>
        /// Gets a value indicating whether the player is sneaking.
        /// </summary>
        public bool IsSneaking => MoveState == PlayerMovementState.Sneaking;

        /// <summary>
        /// Gets the player's IP address.
        /// </summary>
        public string IPAddress => ReferenceHub.networkIdentity.connectionToClient.address;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Player"/> has No-clip enabled.
        /// </summary>
        /// <returns><see cref="bool"/> indicating status.</returns>
        public bool NoClipEnabled {
            get => ReferenceHub.serverRoles.NoclipReady;
            set => ReferenceHub.serverRoles.NoclipReady = value;
        }

        /// <summary>
        /// Gets the player's command sender instance.
        /// </summary>
        public PlayerCommandSender Sender => ReferenceHub.queryProcessor._sender;

        /// <summary>
        /// Gets player's <see cref="NetworkConnection"/>.
        /// </summary>
        public NetworkConnection Connection => ReferenceHub.scp079PlayerScript.connectionToClient;

        /// <summary>
        /// Gets the player's <see cref="Mirror.NetworkIdentity"/>.
        /// </summary>
        public NetworkIdentity NetworkIdentity => ReferenceHub.networkIdentity;

        /// <summary>
        /// Gets a value indicating whether the player is the host.
        /// </summary>
        public bool IsHost => ReferenceHub.isDedicatedServer;

        /// <summary>
        /// Gets a value indicating whether the player is alive.
        /// </summary>
        public bool IsAlive => !IsDead;

        /// <summary>
        /// Gets a value indicating whether the player is dead.
        /// </summary>
        public bool IsDead => Team == Team.RIP;

        /// <summary>
        /// Gets a value indicating whether the player's <see cref="RoleType"/> is any NTF rank.
        /// Equivalent to checking the player's <see cref="Team"/>.
        /// </summary>
        public bool IsNTF => Team == Team.MTF;

        /// <summary>
        /// Gets a value indicating whether or not the player's <see cref="RoleType"/> is any Chaos rank.
        /// Equivalent to checking the player's <see cref="Team"/>.
        /// </summary>
        public bool IsCHI => Team == Team.CHI;

        /// <summary>
        /// Gets a value indicating whether the player's <see cref="RoleType"/> is any SCP rank.
        /// </summary>
        public bool IsScp => Team == Team.SCP;

        /// <summary>
        /// Gets a value indicating whether the player's <see cref="RoleType"/> is any human rank (except the tutorial role).
        /// </summary>
        public bool IsHuman => Team == Team.MTF || Team == Team.CDP || Team == Team.CHI || Team == Team.RSC;

        /// <summary>
        /// Gets or sets the camera SCP-079 is currently controlling.
        /// Only applies if the player is SCP-079.
        /// </summary>
        public Camera079 Camera {
            get => ReferenceHub.scp079PlayerScript.currentCamera;
            set => SetCamera(value.cameraId);
        }

        /// <summary>
        /// Gets the player's <see cref="Enums.Side"/> they're currently in.
        /// </summary>
        public Side Side => Team.GetSide();

        /// <summary>
        /// Gets or sets a value indicating whether the player friendly fire is enabled.
        /// This only isAllowed to deal friendly fire damage, not take friendly fire damage.
        /// </summary>
        public bool IsFriendlyFireEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets the player's scale.
        /// </summary>
        public Vector3 Scale {
            get => ReferenceHub.transform.localScale;
            set {
                try {
                    ReferenceHub.transform.localScale = value;

                    foreach (Player target in List)
                        Server.SendSpawnMessage?.Invoke(null, new object[] { ReferenceHub.characterClassManager.netIdentity, target.Connection });
                }
                catch (Exception exception) {
                    Log.Error($"{nameof(Scale)} error: {exception}");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the player's bypass mode is enabled.
        /// </summary>
        public bool IsBypassModeEnabled {
            get => ReferenceHub.serverRoles.BypassMode;
            set => ReferenceHub.serverRoles.BypassMode = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the player is muted.
        /// </summary>
        public bool IsMuted {
            get => ReferenceHub.dissonanceUserSetup.AdministrativelyMuted;
            set {
                ReferenceHub.dissonanceUserSetup.AdministrativelyMuted = value;

                if (value)
                    MuteHandler.IssuePersistentMute(UserId);
                else
                    MuteHandler.RevokePersistentMute(UserId);
            }
        }

        /// <summary>
        /// Gets or sets the player's <see cref="Assets._Scripts.Dissonance.VoicechatMuteStatus"/>.
        /// </summary>
        public Assets._Scripts.Dissonance.VoicechatMuteStatus MuteStatus {
            get => ReferenceHub.dissonanceUserSetup.NetworkmuteStatus;
            set => ReferenceHub.dissonanceUserSetup.NetworkmuteStatus = value;
        }

        /// <summary>
        /// Gets or sets the player's <see cref="Assets._Scripts.Dissonance.SpeakingFlags"/>.
        /// <para >Note: voicechat channels are handled by the client, therefore any changes will be ignored.</para>
        /// </summary>
        public Assets._Scripts.Dissonance.SpeakingFlags SpeakingFlags {
            get => ReferenceHub.dissonanceUserSetup.NetworkspeakingFlags;
            set => ReferenceHub.dissonanceUserSetup.NetworkspeakingFlags = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the player is intercom muted.
        /// </summary>
        public bool IsIntercomMuted {
            get => ReferenceHub.characterClassManager.NetworkIntercomMuted;
            set => ReferenceHub.characterClassManager.NetworkIntercomMuted = value;
        }

        /// <summary>
        /// Gets a value indicating whether the player is voice chatting.
        /// </summary>
        public bool IsVoiceChatting => ReferenceHub.radio.UsingVoiceChat;

        /// <summary>
        /// Gets a value indicating whether the player is transmitting.
        /// </summary>
        public bool IsTransmitting => ReferenceHub.radio.UsingRadio;

        /// <summary>
        /// Gets or sets a value indicating whether the player has godmode enabled.
        /// </summary>
        public bool IsGodModeEnabled {
            get => ReferenceHub.characterClassManager.GodMode;
            set => ReferenceHub.characterClassManager.GodMode = value;
        }

        /// <summary>
        /// Gets or sets the player's unit name.
        /// </summary>
        public string UnitName {
            get => ReferenceHub.characterClassManager.NetworkCurUnitName;
            set => ReferenceHub.characterClassManager.NetworkCurUnitName = value;
        }

        /// <summary>
        /// Gets or sets the player's health.
        /// If the health is greater than the <see cref="MaxHealth"/>, the MaxHealth will also be changed to match the health.
        /// </summary>
        public float Health {
            get => healthStat.CurValue;
            set {
                healthStat.CurValue = value;

                if (value > MaxHealth)
                    MaxHealth = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the player's maximum health.
        /// </summary>
        public int MaxHealth {
            get => (int)healthStat.MaxValue;
            set => healthStat.CustomMaxValue = value;
        }

        /// <summary>
        /// Gets or sets the player's artificial health.
        /// If the health is greater than the <see cref="MaxArtificialHealth"/>, it will also be changed to match the artificial health.
        /// </summary>
        public float ArtificialHealth {
            get => ReferenceHub.playerStats.StatModules[1].CurValue;
            set {
                if (value > MaxArtificialHealth)
                    MaxArtificialHealth = Mathf.CeilToInt(value);

                ReferenceHub.playerStats.StatModules[1].CurValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the player's maximum artificial health.
        /// </summary>
        public float MaxArtificialHealth {
            get => ((AhpStat)ReferenceHub.playerStats.StatModules[1])._maxSoFar;
            set => ((AhpStat)ReferenceHub.playerStats.StatModules[1])._maxSoFar = value;
        }

        /// <summary>
        /// Gets a list of all active Artificial Health processes on the player.
        /// </summary>
        public List<AhpStat.AhpProcess> ActiveArtificialHealthProcesses {
            get => ((AhpStat)ReferenceHub.playerStats.StatModules[1])._activeProcesses;
        }

        /// <summary>
        /// Gets or sets the player's current SCP.
        /// </summary>
        public PlayableScp CurrentScp {
            get => ReferenceHub.scpsController.CurrentScp;
            set => ReferenceHub.scpsController.CurrentScp = value;
        }

        /// <summary>
        /// Gets or sets the item in the player's hand, returns the default value if empty.
        /// </summary>
        public Item CurrentItem {
            get => Item.Get(Inventory.CurInstance);

            set {
                if (value == null || value.Type == ItemType.None) {
                    Inventory.ServerSelectItem(0);
                }
                else {
                    if (!Inventory.UserInventory.Items.TryGetValue(value.Serial, out _)) {
                        AddItem(value.Base);
                    }

                    Timing.CallDelayed(0.5f, () => Inventory.ServerSelectItem(value.Serial));
                }
            }
        }

        /// <summary>
        /// Gets or sets the abilities of SCP-079. Can be null.
        /// Only applies if the player is SCP-079.
        /// </summary>
        public Scp079PlayerScript.Ability079[] Abilities {
            get => ReferenceHub.scp079PlayerScript?.abilities;
            set {
                if (ReferenceHub.scp079PlayerScript != null)
                    ReferenceHub.scp079PlayerScript.abilities = value;
            }
        }

        /// <summary>
        /// Gets or sets the levels of SCP-079. Can be null.
        /// Only applies if the player is SCP-079.
        /// </summary>
        public Scp079PlayerScript.Level079[] Levels {
            get => ReferenceHub.scp079PlayerScript?.levels;
            set {
                if (ReferenceHub.scp079PlayerScript != null)
                    ReferenceHub.scp079PlayerScript.levels = value;
            }
        }

        /// <summary>
        /// Gets or sets the speaker this player is currently using. Can be null.
        /// Only applies if the player is SCP-079.
        /// </summary>
        public string Speaker {
            get => ReferenceHub.scp079PlayerScript?.Speaker;
            set {
                if (ReferenceHub.scp079PlayerScript != null)
                    ReferenceHub.scp079PlayerScript.Speaker = value;
            }
        }

        /// <summary>
        /// Gets or sets the doors this player has locked. Can be null.
        /// Only applies if the player is SCP-079.
        /// </summary>
        public SyncList<uint> LockedDoors {
            get => ReferenceHub.scp079PlayerScript?.lockedDoors;
            set {
                if (ReferenceHub.scp079PlayerScript != null)
                    ReferenceHub.scp079PlayerScript.lockedDoors = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of experience this player has.
        /// Only applies if the player is SCP-079.
        /// </summary>
        public float Experience {
            get => ReferenceHub.scp079PlayerScript != null ? ReferenceHub.scp079PlayerScript.Exp : float.NaN;
            set {
                if (ReferenceHub.scp079PlayerScript == null)
                    return;

                ReferenceHub.scp079PlayerScript.Exp = value;
                ReferenceHub.scp079PlayerScript.OnExpChange();
            }
        }

        /// <summary>
        /// Gets the <see cref="Stamina"/> class.
        /// </summary>
        public Stamina Stamina => ReferenceHub.fpc.staminaController;

        /// <summary>
        /// Gets or sets this player's level.
        /// Only applies if the player is SCP-079.
        /// </summary>
        public byte Level {
            get => ReferenceHub.scp079PlayerScript != null ? ReferenceHub.scp079PlayerScript.Lvl : byte.MinValue;
            set {
                if (ReferenceHub.scp079PlayerScript == null || ReferenceHub.scp079PlayerScript.Lvl == value)
                    return;

                ReferenceHub.scp079PlayerScript.ForceLevel(value, true);
            }
        }

        /// <summary>
        /// Gets or sets this player's max energy.
        /// Only applies if the player is SCP-079.
        /// </summary>
        public float MaxEnergy {
            get => ReferenceHub.scp079PlayerScript != null ? ReferenceHub.scp079PlayerScript.NetworkmaxMana : float.NaN;
            set {
                if (ReferenceHub.scp079PlayerScript == null)
                    return;

                ReferenceHub.scp079PlayerScript.NetworkmaxMana = value;
                ReferenceHub.scp079PlayerScript.levels[Level].maxMana = value;
            }
        }

        /// <summary>
        /// Gets or sets this player's energy.
        /// Only applies if the player is SCP-079.
        /// </summary>
        public float Energy {
            get => ReferenceHub.scp079PlayerScript != null ? ReferenceHub.scp079PlayerScript.Mana : float.NaN;
            set {
                if (ReferenceHub.scp079PlayerScript == null)
                    return;

                ReferenceHub.scp079PlayerScript.Mana = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the staff bypass is enabled.
        /// </summary>
        public bool IsStaffBypassEnabled => ReferenceHub.serverRoles.BypassStaff;

        /// <summary>
        /// Gets or sets the player's group name.
        /// </summary>
        public string GroupName {
            get => ServerStatic.PermissionsHandler._members.TryGetValue(UserId, out string groupName) ? groupName : null;
            set => ServerStatic.PermissionsHandler._members[UserId] = value;
        }

        /// <summary>
        /// Gets the current room the player is in.
        /// </summary>
        public Room CurrentRoom => Map.FindParentRoom(GameObject);

        /// <summary>
        /// Gets the current zone the player is in.
        /// </summary>
        public ZoneType Zone => CurrentRoom.Zone;

        /// <summary>
        /// Gets all currently active <see cref="PlayerEffect">status effects</see>.
        /// </summary>
        public IEnumerable<PlayerEffect> ActiveEffects => referenceHub.playerEffectsController.AllEffects.Values.Where(effect => effect.Intensity > 0);

        /// <summary>
        /// Gets or sets the player's group.
        /// </summary>
        public UserGroup Group {
            get => ReferenceHub.serverRoles.Group;
            set => ReferenceHub.serverRoles.SetGroup(value, false);
        }

        /// <summary>
        /// Gets or sets the player's rank color.
        /// </summary>
        public string RankColor {
            get => ReferenceHub.serverRoles.Network_myColor;
            set => ReferenceHub.serverRoles.SetColor(value);
        }

        /// <summary>
        /// Gets or sets the player's rank name.
        /// </summary>
        public string RankName {
            get => ReferenceHub.serverRoles.Network_myText;
            set => ReferenceHub.serverRoles.SetText(value);
        }

        /// <summary>
        /// Gets the global badge of the player, can be null if none.
        /// </summary>
        public Badge? GlobalBadge {
            get {
                if (string.IsNullOrEmpty(ReferenceHub.serverRoles.NetworkGlobalBadge))
                    return null;

                ServerRoles serverRoles = ReferenceHub.serverRoles;

                return new Badge(serverRoles._bgt, serverRoles._bgc, serverRoles.GlobalBadgeType, true);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the player's badge is hidden.
        /// </summary>
        public bool BadgeHidden {
            get => !string.IsNullOrEmpty(ReferenceHub.serverRoles.HiddenBadge);
            set {
                if (value)
                    ReferenceHub.characterClassManager.UserCode_CmdRequestHideTag();
                else
                    ReferenceHub.characterClassManager.UserCode_CmdRequestShowTag(false);
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not a player is Northwood staff.
        /// </summary>
        public bool IsNorthwoodStaff => ReferenceHub.serverRoles.Staff;

        /// <summary>
        /// Gets a value indicating whether or not a player is a global moderator.
        /// </summary>
        public bool IsGlobalModerator => ReferenceHub.serverRoles.RaEverywhere;

        /// <summary>
        /// Gets a value indicating whether the player is in the pocket dimension.
        /// </summary>
        public bool IsInPocketDimension => Map.FindParentRoom(GameObject)?.Type == RoomType.Pocket;

        /// <summary>
        /// Gets or sets a value indicating whether player should use stamina system.
        /// </summary>
        public bool IsUsingStamina { get; set; } = true;

        /// <summary>
        /// Gets player's ping.
        /// </summary>
        /// <returns>Return player ping.</returns>
        public int Ping => LiteNetLib4MirrorServer.GetPing(Connection.connectionId);

        /// <summary>
        /// Gets the player's items.
        /// </summary>
        public IReadOnlyCollection<Item> Items => readOnlyItems;

        /// <summary>
        /// Gets or sets a value indicating whether the player can send inputs.
        /// </summary>
        public bool CanSendInputs {
            get => !ReferenceHub.fpc.NetworkforceStopInputs;
            set => ReferenceHub.fpc.NetworkforceStopInputs = !value;
        }

        /// <summary>
        /// Gets a <see cref="Player"/> <see cref="IEnumerable{T}"/> of spectators that are currently spectating this <see cref="Player"/>.
        /// </summary>
        public IEnumerable<Player> CurrentSpectatingPlayers {
            get {
                foreach (ReferenceHub referenceHub in ReferenceHub.spectatorManager.ServerCurrentSpectatingPlayers) {
                    Player spectator = Get(referenceHub);

                    if (spectator == this || spectator.IsDead)
                        yield return spectator;
                }
            }
        }

        /// <summary>
        /// Gets or sets currently spectated player by this <see cref="Player"/>. May be null.
        /// </summary>
        public Player SpectatedPlayer {
            get {
                Player spectatedPlayer = Get(ReferenceHub.spectatorManager.CurrentSpectatedPlayer);

                if (spectatedPlayer == this)
                    return null;

                return spectatedPlayer;
            }

            set {
                if (IsAlive)
                    throw new InvalidOperationException("You cannot set Spectated Player when you are alive!");

                ReferenceHub.spectatorManager.CurrentSpectatedPlayer = value.ReferenceHub;
                ReferenceHub.spectatorManager.CmdSendPlayer(value.Id);
            }
        }

        /// <summary>
        /// Gets the player's <see cref="Footprint"/>.
        /// </summary>
        public Footprint Footprint => new Footprint(ReferenceHub);

        /// <summary>
        /// Gets a dictionary for storing player objects of connected but not yet verified players.
        /// </summary>
        internal static ConditionalWeakTable<ReferenceHub, Player> UnverifiedPlayers { get; } = new ConditionalWeakTable<ReferenceHub, Player>();

        /// <summary>
        /// Gets a <see cref="Player"/> <see cref="IEnumerable{T}"/> filtered by side.
        /// </summary>
        /// <param name="side">The players' side.</param>
        /// <returns>Returns the filtered <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<Player> Get(Side side) => List.Where(player => player.Side == side);

        /// <summary>
        /// Gets a <see cref="Player"/> <see cref="IEnumerable{T}"/> filtered by team.
        /// </summary>
        /// <param name="team">The players' team.</param>
        /// <returns>Returns the filtered <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<Player> Get(Team team) => List.Where(player => player.Team == team);

        /// <summary>
        /// Gets a <see cref="Player"/> <see cref="IEnumerable{T}"/> filtered by role.
        /// </summary>
        /// <param name="role">The players' role.</param>
        /// <returns>Returns the filtered <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<Player> Get(RoleType role) => List.Where(player => player.Role == role);

        /// <summary>
        /// Gets the <see cref="Player"/> belonging to the ICommandSender, if any.
        /// </summary>
        /// <param name="sender">The command sender.</param>
        /// <returns>Returns a player or null if not found.</returns>
        public static Player Get(CommandSystem.ICommandSender sender) => Get(sender as CommandSender);

        /// <summary>
        /// Gets the <see cref="Player"/> belonging to the CommandSender, if any.
        /// </summary>
        /// <param name="sender">The command sender.</param>
        /// <returns>Returns a player or null if not found.</returns>
        public static Player Get(CommandSender sender) => Get(sender.SenderId);

        /// <summary>
        /// Gets the Player belonging to the ReferenceHub, if any.
        /// </summary>
        /// <param name="referenceHub">The player's <see cref="ReferenceHub"/>.</param>
        /// <returns>Returns a player or null if not found.</returns>
        public static Player Get(ReferenceHub referenceHub) => referenceHub == null ? null : Get(referenceHub.gameObject);

        /// <summary>
        /// Gets the Player belonging to a specific NetID, if any.
        /// </summary>
        /// <param name="netId">The player's <see cref="Mirror.NetworkIdentity.netId"/>.</param>
        /// <returns>The player owning the netId, or null if not found.</returns>
        public static Player Get(uint netId) => ReferenceHub.TryGetHubNetID(netId, out ReferenceHub hub) ? Get(hub) : null;

        /// <summary>
        /// Gets the Player belonging to the GameObject, if any.
        /// </summary>
        /// <param name="gameObject">The player's <see cref="UnityEngine.GameObject"/>.</param>
        /// <returns>Returns a player or null if not found.</returns>
        public static Player Get(GameObject gameObject) {
            if (gameObject == null)
                return null;

            Dictionary.TryGetValue(gameObject, out Player player);

            return player;
        }

        /// <summary>
        /// Gets the player belonging to the player with the specified id.
        /// </summary>
        /// <param name="id">The player id.</param>
        /// <returns>Returns the player found or null if not found.</returns>
        public static Player Get(int id) {
            if (IdsCache.TryGetValue(id, out Player player) && player?.ReferenceHub != null)
                return player;

            foreach (Player playerFound in Dictionary.Values) {
                if (playerFound.Id != id)
                    continue;

                IdsCache[id] = playerFound;

                return playerFound;
            }

            return null;
        }

        /// <summary>
        /// Gets the player by his identifier.
        /// </summary>
        /// <param name="args">The player's nickname, steamID64 or Discord ID.</param>
        /// <returns>Returns the player found or null if not found.</returns>
        public static Player Get(string args) {
            try {
                if (string.IsNullOrWhiteSpace(args))
                    return null;

                if (UserIdsCache.TryGetValue(args, out Player playerFound) && playerFound?.ReferenceHub != null)
                    return playerFound;

                if (int.TryParse(args, out int id))
                    return Get(id);

                if (args.EndsWith("@steam") || args.EndsWith("@discord") || args.EndsWith("@northwood") || args.EndsWith("@patreon")) {
                    foreach (Player player in Dictionary.Values) {
                        if (player.UserId == args) {
                            playerFound = player;
                            break;
                        }
                    }
                }
                else {
                    int lastnameDifference = 31;
                    string firstString = args.ToLower();

                    foreach (Player player in Dictionary.Values) {
                        if (!player.IsVerified || player.Nickname == null)
                            continue;

                        if (!player.Nickname.Contains(args, StringComparison.OrdinalIgnoreCase))
                            continue;

                        string secondString = player.Nickname;

                        int nameDifference = secondString.Length - firstString.Length;
                        if (nameDifference < lastnameDifference) {
                            lastnameDifference = nameDifference;
                            playerFound = player;
                        }
                    }
                }

                if (playerFound != null)
                    UserIdsCache[args] = playerFound;

                return playerFound;
            }
            catch (Exception exception) {
                Log.Error($"{typeof(Player).FullName}.{nameof(Get)} error: {exception}");
                return null;
            }
        }

        /// <summary>
        /// Sets the camera the player is currently located at.
        /// Only applies if the player is SCP-079.
        /// </summary>
        /// <param name="cameraId">Camera ID.</param>
        public void SetCamera(ushort cameraId) => ReferenceHub.scp079PlayerScript?.RpcSwitchCamera(cameraId, false);

        /// <summary>
        /// Sets the camera the player is currently located at.
        /// Only applies if the player is SCP-079.
        /// </summary>
        /// <param name="camera">The <see cref="Camera079"/> object to switch to.</param>
        public void SetCamera(Camera079 camera) => SetCamera(camera.cameraId);

        /// <summary>
        /// Forces the player to reload their current weapon.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the item is not a firearm.</exception>
        public void ReloadWeapon() {
            if (CurrentItem is Firearm firearm) {
                firearm.Base.AmmoManagerModule.ServerTryReload();
                Connection.Send(new RequestMessage(firearm.Serial, RequestType.Reload));
            }
            else {
                throw new InvalidOperationException("You may only reload weapons.");
            }
        }

        /// <summary>
        /// Tries to get an item from a player's inventory.
        /// </summary>
        /// <param name="serial">The <see cref="ushort"/> unique identifier of the item.</param>
        /// <param name="item">The <see cref="ItemBase"/> found. Null if it doesn't exist.</param>
        /// <returns>True if the item is found.</returns>
        public bool TryGetItem(ushort serial, out ItemBase item) => Inventory.UserInventory.Items.TryGetValue(serial, out item);

        /// <summary>
        /// Sets the player's rank.
        /// </summary>
        /// <param name="name">The rank name to be set.</param>
        /// <param name="group">The group to be set.</param>
        public void SetRank(string name, UserGroup group) {
            if (ServerStatic.GetPermissionsHandler()._groups.ContainsKey(name)) {
                ServerStatic.GetPermissionsHandler()._groups[name].BadgeColor = group.BadgeColor;
                ServerStatic.GetPermissionsHandler()._groups[name].BadgeText = name;
                ServerStatic.GetPermissionsHandler()._groups[name].HiddenByDefault = !group.Cover;
                ServerStatic.GetPermissionsHandler()._groups[name].Cover = group.Cover;

                ReferenceHub.serverRoles.SetGroup(ServerStatic.GetPermissionsHandler()._groups[name], false, false, group.Cover);
            }
            else {
                ServerStatic.GetPermissionsHandler()._groups.Add(name, group);

                ReferenceHub.serverRoles.SetGroup(group, false, false, group.Cover);
            }

            if (ServerStatic.GetPermissionsHandler()._members.ContainsKey(UserId))
                ServerStatic.GetPermissionsHandler()._members[UserId] = name;
            else
                ServerStatic.GetPermissionsHandler()._members.Add(UserId, name);
        }

        /// <summary>
        /// Handcuff the player.
        /// </summary>
        /// <param name="cuffer">The cuffer player.</param>
        public void Handcuff(Player cuffer) {
            if (cuffer?.ReferenceHub == null)
                return;

            if (!IsCuffed && Vector3.Distance(Position, cuffer.Position) <= 130f) {
                Cuffer = cuffer;
            }
        }

        /// <summary>
        /// Removes handcuffs.
        /// </summary>
        public void RemoveHandcuffs() {
            Inventory.SetDisarmedStatus(null);
            new DisarmedPlayersListMessage(DisarmedPlayers.Entries).SendToAuthenticated();
        }

        /// <summary>
        /// Sets the player's <see cref="RoleType"/>.
        /// </summary>
        /// <param name="newRole">The new <see cref="RoleType"/> to be set.</param>
        /// <param name="reason">The <see cref="SpawnReason"/> defining why the player's role was changed.</param>
        /// <param name="lite">Indicates whether it should preserve the position and inventory after changing the role.</param>
        public void SetRole(RoleType newRole, SpawnReason reason = SpawnReason.ForceClass, bool lite = false) {
            ReferenceHub.characterClassManager.SetPlayersClass(newRole, GameObject, (CharacterClassManager.SpawnReason)reason, lite);
        }

        /// <summary>
        /// Broadcasts the given <see cref="Features.Broadcast"/> to the player.
        /// </summary>
        /// <param name="broadcast">The <see cref="Features.Broadcast"/> to be broadcasted.</param>
        /// <param name="shouldClearPrevious">Clears all player's broadcasts before sending the new one.</param>
        public void Broadcast(Broadcast broadcast, bool shouldClearPrevious = false) {
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
        /// Indicates whether the player has an item.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>true, if the player has it; otherwise, false.</returns>
        public bool HasItem(Item item) => Inventory.UserInventory.Items.ContainsValue(item.Base);

        /// <summary>
        /// Indicates whether the player has an item type.
        /// </summary>
        /// <param name="type">The type to search for.</param>
        /// <returns>true, if the player has it; otherwise, false.</returns>
        public bool HasItem(ItemType type) => Inventory.UserInventory.Items.Any(tempItem => tempItem.Value.ItemTypeId == type);

        /// <summary>
        /// Counts how many items of a certain <see cref="ItemType"/> a player has.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>How many items of that <see cref="ItemType"/> the player has.</returns>
        public int CountItem(ItemType item) => Inventory.UserInventory.Items.Count(tempItem => tempItem.Value.ItemTypeId == item);

        /// <summary>
        /// Removes an <see cref="Item"/> from the player's inventory.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to remove.</param>
        /// <param name="destroy">Whether or not to destroy the item.</param>
        /// <returns>A value indicating whether the <see cref="Item"/> was removed.</returns>
        public bool RemoveItem(Item item, bool destroy = true) {
            if (!ItemsValue.Contains(item)) {
                return false;
            }

            if (!Inventory.UserInventory.Items.ContainsKey(item.Serial)) {
                ItemsValue.Remove(item);
                return false;
            }

            if (destroy) {
                Inventory.ServerRemoveItem(item.Serial, null);
            }
            else {
                if (CurrentItem != null && CurrentItem.Serial == item.Serial)
                    Inventory.NetworkCurItem = ItemIdentifier.None;

                Inventory.UserInventory.Items.Remove(item.Serial);
                ItemsValue.Remove(item);
                Inventory.SendItemsNextFrame = true;
            }

            return true;
        }

        /// <summary>
        /// Removes the held <see cref="ItemBase"/> from the player's inventory.
        /// </summary>
        /// <returns>Returns a value indicating whether the <see cref="ItemBase"/> was removed.</returns>
        public bool RemoveHeldItem() => RemoveItem(CurrentItem);

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
        public void SendConsoleMessage(Player target, string message, string color) => ReferenceHub.characterClassManager.TargetConsolePrint(target.Connection, message, color);

        /// <summary>
        /// Disconnects a <see cref="ReferenceHub">player</see>.
        /// </summary>
        /// <param name="reason">The disconnection reason.</param>
        public void Disconnect(string reason = null) => ServerConsole.Disconnect(GameObject, string.IsNullOrEmpty(reason) ? string.Empty : reason);

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
        /// <param name="damageReason"> The reason for the damage being dealt.</param>
        /// <param name="damage">The amount of damage to deal.</param>
        /// <param name="cassieAnnouncement">The cassie announcement to make.</param>
        public void Hurt(string damageReason, float damage, string cassieAnnouncement = "") => ReferenceHub.playerStats.DealDamage(new CustomReasonDamageHandler(damageReason, damage, cassieAnnouncement));

        /// <summary>
        /// Heals the player.
        /// </summary>
        /// <param name="amount">The amount of health to heal.</param>
        /// <param name="overrideMaxHealth">Whether healing should exceed their max health.</param>
        public void Heal(float amount, bool overrideMaxHealth = false) {
            if (!overrideMaxHealth)
                ((HealthStat)ReferenceHub.playerStats.StatModules[0]).ServerHeal(amount);
            else
                Health += amount;
        }

        /// <summary>
        /// Kills the player.
        /// </summary>
        /// <param name="deathReason">The reason the player has been killed.</param>
        /// <param name="cassieAnnouncement">The cassie announcement to make upon death.</param>
        public void Kill(string deathReason, string cassieAnnouncement = "") => ReferenceHub.playerStats.KillPlayer(new CustomReasonDamageHandler(deathReason, float.MaxValue, cassieAnnouncement));

        /// <summary>
        /// Bans the player.
        /// </summary>
        /// <param name="duration">The ban duration.</param>
        /// <param name="reason">The ban reason.</param>
        /// <param name="issuer">The ban issuer nickname.</param>
        public void Ban(int duration, string reason, string issuer = "Console") => Server.BanPlayer.BanUser(GameObject, duration, reason, issuer, false);

        /// <summary>
        /// Kicks the player.
        /// </summary>
        /// <param name="reason">The kick reason.</param>
        /// <param name="issuer">The kick issuer nickname.</param>
        public void Kick(string reason, string issuer = "Console") => Ban(0, reason, issuer);

        /// <summary>
        /// Blink the player's tag.
        /// </summary>
        /// <returns>Used to wait.</returns>
        public IEnumerator<float> BlinkTag() {
            yield return Timing.WaitForOneFrame;

            BadgeHidden = !BadgeHidden;

            yield return Timing.WaitForOneFrame;

            BadgeHidden = !BadgeHidden;
        }

        /// <summary>
        /// Sends a message to the player's Remote Admin console.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="success">Indicates whether the message should be highlighted as success.</param>
        /// <param name="pluginName">The plugin name.</param>
        public void RemoteAdminMessage(string message, bool success = true, string pluginName = null) {
            Sender.RaReply((pluginName ?? Assembly.GetCallingAssembly().GetName().Name) + "#" + message, success, true, string.Empty);
        }

        /// <summary>
        /// A simple broadcast to a <see cref="ReferenceHub"/>. Doesn't get logged to the console and can be monospaced.
        /// </summary>
        /// <param name="duration">The broadcast duration.</param>
        /// <param name="message">The message to be broadcasted.</param>
        /// <param name="type">The broadcast type.</param>
        /// <param name="shouldClearPrevious">Clears all player's broadcasts before sending the new one.</param>
        public void Broadcast(ushort duration, string message, global::Broadcast.BroadcastFlags type = global::Broadcast.BroadcastFlags.Normal, bool shouldClearPrevious = false) {
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
        public void AddAmmo(AmmoType ammoType, ushort amount) => Inventory.ServerAddAmmo(ammoType.GetItemType(), amount);

        /// <summary>
        /// Sets the amount of a specified <see cref="AmmoType">ammo type</see> to the player's inventory.
        /// </summary>
        /// <param name="ammoType">The <see cref="AmmoType"/> to be set.</param>
        /// <param name="amount">The amount of ammo to be set.</param>
        public void SetAmmo(AmmoType ammoType, ushort amount) => Inventory.ServerSetAmmo(ammoType.GetItemType(), amount);

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
        /// <param name="checkMinimals">Whether ammo limits will be taken into consideration.</param>
        /// <returns>Whether ammo was dropped.</returns>
        public bool DropAmmo(AmmoType ammoType, ushort amount, bool checkMinimals = false) => Inventory.ServerDropAmmo(ammoType.GetItemType(), amount, checkMinimals);

        /// <summary>
        /// Add an item of the specified type with default durability(ammo/charge) and no mods to the player's inventory.
        /// </summary>
        /// <param name="itemType">The item to be added.</param>
        /// <returns>The <see cref="ItemBase"/> given to the player.</returns>
        public Item AddItem(ItemType itemType) {
            Item item = Item.Get(Inventory.ServerAddItem(itemType));
            if (item is Firearm firearm) {
                if (AttachmentsServerHandler.PlayerPreferences.TryGetValue(ReferenceHub, out Dictionary<ItemType, uint> dict) &&
                    dict.TryGetValue(itemType, out uint code)) {
                    firearm.Base.ApplyAttachmentsCode(code, true);
                }

                FirearmStatusFlags flags = FirearmStatusFlags.MagazineInserted;
                if (firearm.Base.CombinedAttachments.AdditionalPros.HasFlagFast(AttachmentDescriptiveAdvantages.Flashlight))
                    flags |= FirearmStatusFlags.FlashlightEnabled;
                firearm.Base.Status = new FirearmStatus(firearm.MaxAmmo, flags, firearm.Base.GetCurrentAttachmentsCode());
            }

            return item;
        }

        /// <summary>
        /// Add the amount of items of the specified type with default durability(ammo/charge) and no mods to the player's inventory.
        /// </summary>
        /// <param name="itemType">The item to be added.</param>
        /// <param name="amount">The amount of items to be added.</param>
        public void AddItem(ItemType itemType, int amount) {
            if (amount > 0) {
                for (int i = 0; i < amount; i++)
                    AddItem(itemType);
            }
        }

        /// <summary>
        /// Add the list of items of the specified type with default durability(ammo/charge) and no mods to the player's inventory.
        /// </summary>
        /// <param name="items">The list of items to be added.</param>
        public void AddItem(List<ItemType> items) {
            if (items.Count > 0) {
                for (int i = 0; i < items.Count; i++)
                    AddItem(items[i]);
            }
        }

        /// <summary>
        /// Add an item to the player's inventory.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        public void AddItem(Item item) {
            try {
                if (item.Base == null) {
                    item = new Item(item.Type);
                }

                AddItem(item.Base, item);
            }
            catch (Exception e) {
                Log.Error($"{nameof(Player)}.{nameof(AddItem)}(Item): {e}");
            }
        }

        /// <summary>
        /// Adds an item to the player's inventory.
        /// </summary>
        /// <param name="pickup">The <see cref="Pickup"/> of the item to be added.</param>
        /// <returns>The <see cref="Item"/> that was added.</returns>
        public Item AddItem(Pickup pickup) => Item.Get(Inventory.ServerAddItem(pickup.Type, pickup.Serial, pickup.Base));

        /// <summary>
        /// Add an item to the player's inventory.
        /// </summary>
        /// <param name="itemBase">The item to be added.</param>
        /// <param name="item">The <see cref="Item"/> object of the item.</param>
        /// <returns>The item that was added.</returns>
        public Item AddItem(ItemBase itemBase, Item item = null) {
            try {
                if (item == null)
                    item = Item.Get(itemBase);

                int ammo = -1;
                if (item is Firearm firearm1) {
                    ammo = firearm1.Ammo;
                }

                itemBase.Owner = ReferenceHub;
                Inventory.UserInventory.Items[item.Serial] = itemBase;
                if (itemBase.PickupDropModel != null) {
                    itemBase.OnAdded(itemBase.PickupDropModel);
                }

                if (itemBase is InventorySystem.Items.Firearms.Firearm firearm) {
                    if (AttachmentsServerHandler.PlayerPreferences.TryGetValue(ReferenceHub, out Dictionary<ItemType, uint> dict) &&
                        dict.TryGetValue(item.Type, out uint code)) {
                        firearm.ApplyAttachmentsCode(code, true);
                    }

                    FirearmStatusFlags flags = FirearmStatusFlags.MagazineInserted;
                    if (firearm.CombinedAttachments.AdditionalPros.HasFlagFast(AttachmentDescriptiveAdvantages
                        .Flashlight))
                        flags |= FirearmStatusFlags.FlashlightEnabled;
                    firearm.Status = new FirearmStatus(ammo > -1 ? (byte)ammo : firearm.AmmoManagerModule.MaxAmmo, flags, firearm.GetCurrentAttachmentsCode());
                }

                if (itemBase is IAcquisitionConfirmationTrigger acquisitionConfirmationTrigger) {
                    acquisitionConfirmationTrigger.ServerConfirmAcqusition();
                }

                ItemsValue.Add(item);

                Inventory.SendItemsNextFrame = true;
                return item;
            }
            catch (Exception e) {
                Log.Error($"{nameof(Player)}.{nameof(AddItem)}(ItemBase, [Item]): {e}");
            }

            return null;
        }

        /// <summary>
        /// Add the amount of items to the player's inventory.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        /// <param name="amount">The amount of items to be added.</param>
        public void AddItem(Item item, int amount) {
            if (amount > 0) {
                for (int i = 0; i < amount; i++)
                    AddItem(item);
            }
        }

        /// <summary>
        /// Add the list of items to the player's inventory.
        /// </summary>
        /// <param name="items">The list of items to be added.</param>
        public void AddItem(List<Item> items) {
            if (items.Count > 0) {
                for (int i = 0; i < items.Count; i++)
                    AddItem(items[i]);
            }
        }

        /// <summary>
        /// Resets the player's inventory to the provided list of items, clearing any items it already possess.
        /// </summary>
        /// <param name="newItems">The new items that have to be added to the inventory.</param>
        public void ResetInventory(List<ItemType> newItems) {
            ClearInventory();

            Timing.CallDelayed(0.5f, () => {
                if (newItems.Count > 0) {
                    foreach (ItemType item in newItems)
                        AddItem(item);
                }
            });
        }

        /// <summary>
        /// Resets the player's inventory to the provided list of items, clearing any items it already possess.
        /// </summary>
        /// <param name="newItems">The new items that have to be added to the inventory.</param>
        public void ResetInventory(List<Item> newItems) {
            ClearInventory();

            if (newItems.Count > 0) {
                foreach (Item item in newItems) {
                    AddItem(item.Base == null ? new Item(item.Type) : item);
                }
            }
        }

        /// <summary>
        /// Clears the player's inventory, including all ammo and items.
        /// </summary>
        /// <param name="destroy">Whether ot not to fully destroy the old items.</param>
        public void ClearInventory(bool destroy = true) {
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
        public Throwable ThrowGrenade(GrenadeType type, bool fullForce = true) {
            Throwable throwable;
            switch (type) {
                case GrenadeType.Flashbang:
                    throwable = new FlashGrenade(ItemType.GrenadeFlash);
                    break;
                default:
                    throwable = new ExplosiveGrenade(type == GrenadeType.Scp018 ? ItemType.SCP018 : ItemType.GrenadeHE);
                    break;
            }

            ThrowItem(throwable, fullForce);
            return throwable;
        }

        /// <summary>
        /// Throw an item.
        /// </summary>
        /// <param name="throwable">The <see cref="Throwable"/> to be thrown.</param>
        /// <param name="fullForce">Whether to throw with full or half force.</param>
        public void ThrowItem(Throwable throwable, bool fullForce = true) {
            throwable.Base.Owner = ReferenceHub;
            throwable.Throw(fullForce);
        }

        /// <summary>
        /// Simple way to show a hint to the player.
        /// </summary>
        /// <param name="message">The message to be shown.</param>
        /// <param name="duration">The duration the text will be on screen.</param>
        public void ShowHint(string message, float duration = 3f) {
            HintParameter[] parameters = new HintParameter[]
            {
                new StringHintParameter(message),
            };

            HintDisplay.Show(new TextHint(message, parameters, null, duration));
        }

        /// <summary>
        /// Sends a HitMarker to the player.
        /// </summary>
        [Obsolete("Use Player::ShowHitMarker(float) instead.", true)]
        public void ShowHitMarker() => Hitmarker.SendHitmarker(Connection, 1f);

        /// <summary>
        /// Sends a HitMarker to the player.
        /// </summary>
        /// <param name="size">The size of the hitmarker (Do not exceed <see cref="Hitmarker.MaxSize"/>).</param>
        public void ShowHitMarker(float size = 1f) => Hitmarker.SendHitmarker(Connection, size > Hitmarker.MaxSize ? Hitmarker.MaxSize : size);

        /// <summary>
        /// Safely gets an <see cref="object"/> from <see cref="Player.SessionVariables"/>, then casts it to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The returned object type.</typeparam>
        /// <param name="key">The key of the object to get.</param>
        /// <param name="result">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter is used.</param>
        /// <returns><see langword="true"/> if the SessionVariables contains an element with the specified key; otherwise, <see langword="false"/>.</returns>
        public bool TryGetSessionVariable<T>(string key, out T result) {
            if (SessionVariables.TryGetValue(key, out object value) && value is T type) {
                result = type;
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Gets a <see cref="bool"/> describing whether the given <see cref="PlayerEffect">status effect</see> is currently enabled.
        /// </summary>
        /// <typeparam name="T">The <see cref="PlayerEffect"/> to check.</typeparam>
        /// <returns>A <see cref="bool"/> determining whether the player effect is active.</returns>
        public bool GetEffectActive<T>()
            where T : PlayerEffect {
            if (ReferenceHub.playerEffectsController.AllEffects.TryGetValue(typeof(T), out PlayerEffect playerEffect))
                return playerEffect.IsEnabled;

            return false;
        }

        /// <summary>
        ///  Disables all currently active <see cref="PlayerEffect">status effects</see>.
        /// </summary>
        public void DisableAllEffects() {
            foreach (KeyValuePair<Type, PlayerEffect> effect in ReferenceHub.playerEffectsController.AllEffects)
                effect.Value.IsEnabled = false;
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
        public void DisableEffect(EffectType effect) {
            if (TryGetEffect(effect, out PlayerEffect playerEffect))
                playerEffect.IsEnabled = false;
        }

        /// <summary>
        /// Disables a <see cref="IEnumerable{T}"/> of <see cref="EffectType"/> on the player.
        /// </summary>
        /// <param name="effects">The <see cref="IEnumerable{T}"/> of <see cref="EffectType"/> to disable.</param>
        public void DisableEffects(IEnumerable<EffectType> effects) {
            foreach (EffectType effect in effects)
                DisableEffect(effect);
        }

        /// <summary>
        /// Enables a <see cref="PlayerEffect">status effect</see> on the player.
        /// </summary>
        /// <typeparam name="T">The <see cref="PlayerEffect"/> to enable.</typeparam>
        /// <param name="duration">The amount of time the effect will be active for.</param>
        /// <param name="addDurationIfActive">If the effect is already active, setting to true will add this duration onto the effect.</param>
        public void EnableEffect<T>(float duration = 0f, bool addDurationIfActive = false)
            where T : PlayerEffect => ReferenceHub.playerEffectsController.EnableEffect<T>(duration, addDurationIfActive);

        /// <summary>
        /// Enables a <see cref="PlayerEffect">status effect</see> on the player.
        /// </summary>
        /// <param name="effect">The name of the <see cref="PlayerEffect"/> to enable.</param>
        /// <param name="duration">The amount of time the effect will be active for.</param>
        /// <param name="addDurationIfActive">If the effect is already active, setting to true will add this duration onto the effect.</param>
        public void EnableEffect(PlayerEffect effect, float duration = 0f, bool addDurationIfActive = false)
            => ReferenceHub.playerEffectsController.EnableEffect(effect, duration, addDurationIfActive);

        /// <summary>
        /// Enables a <see cref="PlayerEffect">status effect</see> on the player.
        /// </summary>
        /// <param name="effect">The name of the <see cref="PlayerEffect"/> to enable.</param>
        /// <param name="duration">The amount of time the effect will be active for.</param>
        /// <param name="addDurationIfActive">If the effect is already active, setting to true will add this duration onto the effect.</param>
        /// <returns>A bool indicating whether the effect was valid and successfully enabled.</returns>
        public bool EnableEffect(string effect, float duration = 0f, bool addDurationIfActive = false)
            => ReferenceHub.playerEffectsController.EnableByString(effect, duration, addDurationIfActive);

        /// <summary>
        /// Enables a <see cref="EffectType">status effect</see> on the player.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/> to enable.</param>
        /// <param name="duration">The amount of time the effect will be active for.</param>
        /// <param name="addDurationIfActive">If the effect is already active, setting to true will add this duration onto the effect.</param>
        public void EnableEffect(EffectType effect, float duration = 0f, bool addDurationIfActive = false) {
            if (TryGetEffect(effect, out PlayerEffect pEffect))
                ReferenceHub.playerEffectsController.EnableEffect(pEffect, duration, addDurationIfActive);
        }

        /// <summary>
        /// Enables a random <see cref="EffectType"/> on the player.
        /// </summary>
        /// <param name="duration">The amount of time the effect will be active for.</param>
        /// <param name="addDurationIfActive">If the effect is already active, setting to true will add this duration onto the effect.</param>
        /// <returns>A <see cref="EffectType"/> that was given to the player.</returns>
        public EffectType ApplyRandomEffect(float duration = 0f, bool addDurationIfActive = false) {
            EffectType effectType = (EffectType)Enum.GetValues(typeof(EffectType)).GetValue(UnityEngine.Random.Range(0, Enum.GetValues(typeof(EffectType)).Length));
            EnableEffect(effectType, duration, addDurationIfActive);
            return effectType;
        }

        /// <summary>
        /// Enables a <see cref="IEnumerable{T}"/> of <see cref="EffectType"/> on the player.
        /// </summary>
        /// <param name="effects">The <see cref="IEnumerable{T}"/> of <see cref="EffectType"/> to enable.</param>
        /// <param name="duration">The amount of time the effects will be active for.</param>
        /// <param name="addDurationIfActive">If an effect is already active, setting to true will add this duration onto the effect.</param>
        public void EnableEffects(IEnumerable<EffectType> effects, float duration = 0f, bool addDurationIfActive = false) {
            foreach (EffectType effect in effects) {
                if (TryGetEffect(effect, out PlayerEffect pEffect))
                    EnableEffect(pEffect, duration, addDurationIfActive);
            }
        }

        /// <summary>
        /// Gets an instance of <see cref="PlayerEffect"/> by <see cref="EffectType"/>.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/>.</param>
        /// <returns>The <see cref="PlayerEffect"/>.</returns>
        public PlayerEffect GetEffect(EffectType effect) {
            ReferenceHub.playerEffectsController.AllEffects.TryGetValue(effect.Type(), out PlayerEffect playerEffect);

            return playerEffect;
        }

        /// <summary>
        /// Tries to get an instance of <see cref="PlayerEffect"/> by <see cref="EffectType"/>.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/>.</param>
        /// <param name="playerEffect">The <see cref="PlayerEffect"/>.</param>
        /// <returns>A bool indicating whether the <paramref name="playerEffect"/> was successfully gotten.</returns>
        public bool TryGetEffect(EffectType effect, out PlayerEffect playerEffect) {
            playerEffect = GetEffect(effect);

            return playerEffect != null;
        }

        /// <summary>
        /// Gets a <see cref="byte"/> indicating the intensity of the given <see cref="PlayerEffect">status effect</see>.
        /// </summary>
        /// <typeparam name="T">The <see cref="PlayerEffect"/> to check.</typeparam>
        /// <exception cref="ArgumentException">Thrown if the given type is not a valid <see cref="PlayerEffect"/>.</exception>
        /// <returns>The intensity of the effect.</returns>
        public byte GetEffectIntensity<T>()
            where T : PlayerEffect {
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
        public void ChangeEffectIntensity(EffectType type, byte intensity, float duration = 0) {
            if (TryGetEffect(type, out PlayerEffect pEffect)) {
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
        public void ChangeEffectIntensity(string effect, byte intensity, float duration = 0) => ReferenceHub.playerEffectsController.ChangeByString(effect, intensity, duration);

        /// <summary>
        /// Opens the report window.
        /// </summary>
        /// <param name="text">The text to send.</param>
        public void OpenReportWindow(string text) => SendConsoleMessage($"[REPORTING] {text}", "white");

        /// <summary>
        /// Places a Tantrum (Scp173's ability) under the player.
        /// </summary>
        /// <returns>The tantrum's <see cref="GameObject"/>.</returns>
        public GameObject PlaceTantrum() => Map.PlaceTantrum(Position);

        /// <inheritdoc/>
        public override string ToString() => $"{Id} {Nickname} {UserId} {Role} {Team}";
    }
}
