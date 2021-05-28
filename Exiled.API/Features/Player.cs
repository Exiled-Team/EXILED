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

    using Grenades;

    using Hints;

    using MEC;

    using Mirror;
    using Mirror.LiteNetLib4Mirror;

    using NorthwoodLib;
    using NorthwoodLib.Pools;

    using PlayableScps;

    using RemoteAdmin;

    using UnityEngine;

    /// <summary>
    /// Represents the in-game player, by encapsulating a <see cref="ReferenceHub"/>.
    /// </summary>
    public class Player
    {
        private ReferenceHub referenceHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="referenceHub">The <see cref="ReferenceHub"/> of the player to be encapsulated.</param>
        public Player(ReferenceHub referenceHub) => ReferenceHub = referenceHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="gameObject">The <see cref="GameObject"/> of the player.</param>
        public Player(GameObject gameObject) => ReferenceHub = ReferenceHub.GetHub(gameObject);

        /// <summary>
        /// Finalizes an instance of the <see cref="Player"/> class.
        /// </summary>
        ~Player()
        {
            HashSetPool<int>.Shared.Return(TargetGhostsHashSet);
#pragma warning disable CS0618 // Type or member is obsolete
            ListPool<int>.Shared.Return(TargetGhosts);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> containing all <see cref="Player"/> on the server.
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
        /// Gets the encapsulated <see cref="ReferenceHub"/>.
        /// </summary>
        public ReferenceHub ReferenceHub
        {
            get => referenceHub;
            private set
            {
                if (value == null)
                    throw new NullReferenceException("Player's ReferenceHub cannot be null!");

                referenceHub = value;
                GameObject = value.gameObject;
                Ammo = value.ammoBox;
                HintDisplay = value.hints;
                Inventory = value.inventory;
                CameraTransform = value.PlayerCameraReference;
                GrenadeManager = value.GetComponent<GrenadeManager>();
            }
        }

        /// <summary>
        /// Gets the encapsulated <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        public GameObject GameObject { get; private set; }

        /// <summary>
        /// Gets the player's ammo.
        /// </summary>
        public AmmoBox Ammo { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the player is viewing a hint.
        /// </summary>
        public bool HasHint { get; internal set; }

        /// <summary>
        /// Gets the HintDisplay of the player.
        /// </summary>
        public HintDisplay HintDisplay { get; private set; }

        /// <summary>
        /// Gets the player's inventory.
        /// </summary>
        public Inventory Inventory { get; private set; }

        /// <summary>
        /// Gets the encapsulated <see cref="ReferenceHub"/>'s PlayerCamera.
        /// </summary>
        [Obsolete("Use CameraTransform instead.", true)]
        public Transform PlayerCamera => CameraTransform;

        /// <summary>
        /// Gets the encapsulated <see cref="ReferenceHub"/>'s PlayerCamera.
        /// </summary>
        public Transform CameraTransform { get; private set; }

        /// <summary>
        /// Gets the player's grenade manager.
        /// </summary>
        public GrenadeManager GrenadeManager { get; private set; }

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
        public string UserId => referenceHub.characterClassManager.UserId;

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
        public string AuthenticationToken => ReferenceHub.characterClassManager.AuthToken;

        /// <inheritdoc cref="Enums.AuthenticationType"/>
        public AuthenticationType AuthenticationType
        {
            get
            {
                if (string.IsNullOrEmpty(UserId))
                    return AuthenticationType.Unknown;

                int index = UserId.LastIndexOf('@');

                if (index == -1)
                    return AuthenticationType.Unknown;

                switch (UserId.Substring(index + 1))
                {
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
        /// Gets a value indicating whether or not the player is verified.
        /// </summary>
        /// <remarks>
        /// This is always false if online_mode is set to false.
        /// </remarks>
        public bool IsVerified { get; internal set; }

        /// <summary>
        /// Gets or sets the player's display nickname.
        /// May be null.
        /// </summary>
        public string DisplayNickname
        {
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
        public PlayerInfoArea InfoArea
        {
            get => ReferenceHub.nicknameSync.Network_playerInfoToShow;
            set => ReferenceHub.nicknameSync.Network_playerInfoToShow = value;
        }

        /// <summary>
        /// Gets or sets the player's player info area bitmask.
        /// You can hide player info elements with this.
        /// </summary>
        [Obsolete("Use InfoArea instead.", true)]
        public PlayerInfoArea PlayerInfoArea
        {
            get => InfoArea;
            set => InfoArea = value;
        }

        /// <summary>
        /// Gets or sets the player's custom player info string.
        /// </summary>
        public string CustomInfo
        {
            get => ReferenceHub.nicknameSync.Network_customPlayerInfoString;
            set => ReferenceHub.nicknameSync.Network_customPlayerInfoString = value;
        }

        /// <summary>
        /// Gets or sets the player's custom player info string.
        /// </summary>
        [Obsolete("Use CustomInfo instead.", true)]
        public string CustomPlayerInfo
        {
            get => CustomInfo;
            set => CustomInfo = value;
        }

        /// <summary>
        /// Gets the dictionary of player's session variables. It is not being saved on player disconnect.
        /// </summary>
        public Dictionary<string, object> SessionVariables { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets a value indicating whether or not the player is invisible.
        /// </summary>
        public bool IsInvisible { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the player can be tracked.
        /// </summary>
        public bool DoNotTrack => ReferenceHub.serverRoles.DoNotTrack;

        /// <summary>
        /// Gets a value indicating whether or not the player is connected to the server.
        /// </summary>
        public bool IsConnected => GameObject != null;

        /// <summary>
        /// Gets a list of player ids who can't see the player.
        /// </summary>
        [Obsolete("Use 'TargetGhostsSet' instead, will be removed in future releases.")]
        public List<int> TargetGhosts { get; } = ListPool<int>.Shared.Rent();

        /// <summary>
        /// Gets a list of player ids who can't see the player.
        /// </summary>
        public HashSet<int> TargetGhostsHashSet { get; } = HashSetPool<int>.Shared.Rent();

        /// <summary>
        /// Gets a value indicating whether or not the player has Remote Admin access.
        /// </summary>
        public bool RemoteAdminAccess => ReferenceHub.serverRoles.RemoteAdmin;

        /// <summary>
        /// Gets or sets a value indicating whether or not the player's overwatch is enabled.
        /// </summary>
        public bool IsOverwatchEnabled
        {
            get => ReferenceHub.serverRoles.OverwatchEnabled;
            set => ReferenceHub.serverRoles.SetOverwatchStatus(value);
        }

        /// <summary>
        /// Gets or sets a value indicating the cuffer <see cref="Player"/> id.
        /// </summary>
        public int CufferId
        {
            get => ReferenceHub.handcuffs.NetworkCufferId;
            set => ReferenceHub.handcuffs.NetworkCufferId = value;
        }

        /// <summary>
        /// Gets or sets the player's position.
        /// </summary>
        public Vector3 Position
        {
            get => ReferenceHub.playerMovementSync.GetRealPosition();
            set => ReferenceHub.playerMovementSync.OverridePosition(value, 0f);
        }

        /// <summary>
        /// Gets or sets the player's rotations.
        /// </summary>
        /// <returns>Returns a <see cref="Vector2"/> representing the rotation of the player.</returns>
        public Vector2 Rotations
        {
            get => ReferenceHub.playerMovementSync.RotationSync;
            set => ReferenceHub.playerMovementSync.RotationSync = value;
        }

        /// <summary>
        /// Gets or sets the player's rotation.
        /// </summary>
        /// <returns>Returns the direction he's looking at, useful for Raycasts.</returns>
        public Vector3 Rotation
        {
            get => ReferenceHub.PlayerCameraReference.forward;
            set => ReferenceHub.PlayerCameraReference.forward = value;
        }

        /// <summary>
        /// Gets the player's <see cref="Team"/>.
        /// </summary>
        public Team Team => Role.GetTeam();

        /// <summary>
        /// Gets or sets the player's <see cref="RoleType"/>.
        /// </summary>
        public RoleType Role
        {
            get => ReferenceHub.characterClassManager.NetworkCurClass;
            set => SetRole(value);
        }

        /// <summary>
        /// Gets the <see cref="Color"/> of the player's <see cref="RoleType">role</see>.
        /// </summary>
        public Color RoleColor => Role.GetColor();

        /// <summary>
        /// Gets a value indicating whether or not the player is cuffed.
        /// </summary>
        public bool IsCuffed => CufferId != -1;

        /// <summary>
        /// Gets a value indicating whether or not the player is reloading a weapon.
        /// </summary>
        public bool IsReloading => ReferenceHub.weaponManager.IsReloading();

        /// <summary>
        /// Gets a value indicating whether or not the player is zooming with a weapon.
        /// </summary>
        public bool IsZooming => ReferenceHub.weaponManager.NetworksyncZoomed;

        /// <summary>
        /// Gets the player's current <see cref="PlayerMovementState"/>.
        /// </summary>
        public PlayerMovementState MoveState => ReferenceHub.animationController.MoveState;

        /// <summary>
        /// Gets a value indicating whether or not the player is jumping.
        /// </summary>
        public bool IsJumping => ReferenceHub.animationController.curAnim == 2;

        /// <summary>
        /// Gets the player's IP address.
        /// </summary>
        public string IPAddress => ReferenceHub.networkIdentity.connectionToClient.address;

        /// <summary>
        /// Gets or sets a value indicating whether or not the <see cref="Player"/> has No-clip enabled.
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
        [Obsolete("Use Sender instead.", true)]
        public CommandSender CommandSender => Sender;

        /// <summary>
        /// Gets the player's command sender instance.
        /// </summary>
        public PlayerCommandSender Sender => ReferenceHub.queryProcessor._sender;

        /// <summary>
        /// Gets player's <see cref="NetworkConnection"/>.
        /// </summary>
        public NetworkConnection Connection => ReferenceHub.scp079PlayerScript.connectionToClient;

        /// <summary>
        /// Gets a value indicating whether or not the player is the host.
        /// </summary>
        public bool IsHost => ReferenceHub.isDedicatedServer;

        /// <summary>
        /// Gets a value indicating whether or not the player is alive.
        /// </summary>
        public bool IsAlive => !IsDead;

        /// <summary>
        /// Gets a value indicating whether or not the player is dead.
        /// </summary>
        public bool IsDead => Team == Team.RIP;

        /// <summary>
        /// Gets a value indicating whether or not the player's <see cref="RoleType"/> is any NTF rank.
        /// Equivalent to checking the player's <see cref="Team"/>.
        /// </summary>
        public bool IsNTF => Team == Team.MTF;

        /// <summary>
        /// Gets a value indicating whether or not the player's <see cref="RoleType"/> is any SCP rank.
        /// </summary>
        public bool IsScp => Team == Team.SCP;

        /// <summary>
        /// Gets a value indicating whether or not the player's <see cref="RoleType"/> is any human rank (except the tutorial role).
        /// </summary>
        public bool IsHuman => Team == Team.MTF || Team == Team.CDP || Team == Team.CHI || Team == Team.MTF || Team == Team.RSC;

        /// <summary>
        /// Gets or sets the camera SCP-079 is currently controlling.
        /// Only applies if the player is SCP-079.
        /// </summary>
        public Camera079 Camera
        {
            get => ReferenceHub.scp079PlayerScript.currentCamera;
            set => SetCamera(value.cameraId);
        }

        /// <summary>
        /// Gets the player's <see cref="Enums.Side"/> they're currently in.
        /// </summary>
        public Side Side => Team.GetSide();

        /// <summary>
        /// Gets or sets a value indicating whether the player friendly fire is enabled or not.
        /// This only isAllowed to deal friendly fire damage, not take friendly fire damage.
        /// </summary>
        public bool IsFriendlyFireEnabled { get; set; }

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
        /// Gets or sets a value indicating whether the player's bypass mode is enabled or not.
        /// </summary>
        public bool IsBypassModeEnabled
        {
            get => ReferenceHub.serverRoles.BypassMode;
            set => ReferenceHub.serverRoles.BypassMode = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player is muted.
        /// </summary>
        public bool IsMuted
        {
            get => ReferenceHub.characterClassManager.NetworkMuted;
            set => ReferenceHub.characterClassManager.NetworkMuted = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player is intercom muted.
        /// </summary>
        public bool IsIntercomMuted
        {
            get => ReferenceHub.characterClassManager.NetworkIntercomMuted;
            set => ReferenceHub.characterClassManager.NetworkIntercomMuted = value;
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
            get => ReferenceHub.playerStats.Health;
            set
            {
                ReferenceHub.playerStats.Health = value;

                if (value > MaxHealth)
                    MaxHealth = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the player's maximum health.
        /// </summary>
        public int MaxHealth
        {
            get => ReferenceHub.playerStats.maxHP;
            set => ReferenceHub.playerStats.maxHP = value;
        }

        /// <summary>
        /// Gets or sets the player's artificial health.
        /// If the health is greater than the <see cref="MaxArtificialHealth"/>, it will also be changed to match the artificial health.
        /// </summary>
        public float ArtificialHealth
        {
            get => ReferenceHub.playerStats.unsyncedArtificialHealth;
            set
            {
                ReferenceHub.playerStats.unsyncedArtificialHealth = value;

                if (value > MaxArtificialHealth)
                    MaxArtificialHealth = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the player's artificial health decay.
        /// </summary>
        public float ArtificialHealthDecay
        {
            get => ReferenceHub.playerStats.artificialHpDecay;
            set => ReferenceHub.playerStats.artificialHpDecay = value;
        }

        /// <summary>
        /// Gets or sets the player's adrenaline health.
        /// If the health is greater than the <see cref="MaxAdrenalineHealth"/>, the MaxAdrenalineHealth will also be changed to match the adrenaline health.
        /// </summary>
        [Obsolete("Use ArtificialHealth instead.", true)]
        public float AdrenalineHealth
        {
            get => ArtificialHealth;
            set => ArtificialHealth = value;
        }

        /// <summary>
        /// Gets or sets the player's maximum artificial health.
        /// </summary>
        public int MaxArtificialHealth
        {
            get => ReferenceHub.playerStats.maxArtificialHealth;
            set => ReferenceHub.playerStats.maxArtificialHealth = value;
        }

        /// <summary>
        /// Gets or sets the player's maximum adrenaline health.
        /// </summary>
        [Obsolete("Use MaxArtificialHealth instead.", true)]
        public int MaxAdrenalineHealth
        {
            get => MaxArtificialHealth;
            set => MaxArtificialHealth = value;
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
        public Inventory.SyncItemInfo CurrentItem
        {
            get => Inventory.GetItemInHand();
            set
            {
                Inventory.SetCurItem(value.id);
                Inventory.CallCmdSetUnic(value.uniq);
            }
        }

        /// <summary>
        /// Gets the index of the current item in hand.
        /// </summary>
        public int CurrentItemIndex => Inventory.GetItemIndex();

        /// <summary>
        /// Gets or sets the abilities of SCP-079. Can be null.
        /// Only applies if the player is SCP-079.
        /// </summary>
        public Scp079PlayerScript.Ability079[] Abilities
        {
            get => ReferenceHub.scp079PlayerScript?.abilities;
            set
            {
                if (ReferenceHub.scp079PlayerScript != null)
                    ReferenceHub.scp079PlayerScript.abilities = value;
            }
        }

        /// <summary>
        /// Gets or sets the levels of SCP-079. Can be null.
        /// Only applies if the player is SCP-079.
        /// </summary>
        public Scp079PlayerScript.Level079[] Levels
        {
            get => ReferenceHub.scp079PlayerScript?.levels;
            set
            {
                if (ReferenceHub.scp079PlayerScript != null)
                    ReferenceHub.scp079PlayerScript.levels = value;
            }
        }

        /// <summary>
        /// Gets or sets the speaker this player is currently using. Can be null.
        /// Only applies if the player is SCP-079.
        /// </summary>
        public string Speaker
        {
            get => ReferenceHub.scp079PlayerScript?.Speaker;
            set
            {
                if (ReferenceHub.scp079PlayerScript != null)
                    ReferenceHub.scp079PlayerScript.Speaker = value;
            }
        }

        /// <summary>
        /// Gets or sets the doors this player has locked. Can be null.
        /// Only applies if the player is SCP-079.
        /// </summary>
        public SyncListUInt LockedDoors
        {
            get => ReferenceHub.scp079PlayerScript?.lockedDoors;
            set
            {
                if (ReferenceHub.scp079PlayerScript != null)
                    ReferenceHub.scp079PlayerScript.lockedDoors = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of experience this player has.
        /// Only applies if the player is SCP-079.
        /// </summary>
        public float Experience
        {
            get => ReferenceHub.scp079PlayerScript != null ? ReferenceHub.scp079PlayerScript.Exp : float.NaN;
            set
            {
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
        public byte Level
        {
            get => ReferenceHub.scp079PlayerScript != null ? ReferenceHub.scp079PlayerScript.Lvl : byte.MinValue;
            set
            {
                if (ReferenceHub.scp079PlayerScript == null || ReferenceHub.scp079PlayerScript.Lvl == value)
                    return;

                ReferenceHub.scp079PlayerScript.Lvl = value;

                ReferenceHub.scp079PlayerScript.TargetLevelChanged(Connection, value);
            }
        }

        /// <summary>
        /// Gets or sets this player's max energy.
        /// Only applies if the player is SCP-079.
        /// </summary>
        public float MaxEnergy
        {
            get => ReferenceHub.scp079PlayerScript != null ? ReferenceHub.scp079PlayerScript.NetworkmaxMana : float.NaN;
            set
            {
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
        public float Energy
        {
            get => ReferenceHub.scp079PlayerScript != null ? ReferenceHub.scp079PlayerScript.Mana : float.NaN;
            set
            {
                if (ReferenceHub.scp079PlayerScript == null)
                    return;

                ReferenceHub.scp079PlayerScript.Mana = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the staff bypass is enabled or not.
        /// </summary>
        public bool IsStaffBypassEnabled => ReferenceHub.serverRoles.BypassStaff;

        /// <summary>
        /// Gets or sets the player's group name.
        /// </summary>
        public string GroupName
        {
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
            get => ReferenceHub.serverRoles.NetworkMyColor;
            set => ReferenceHub.serverRoles.SetColor(value);
        }

        /// <summary>
        /// Gets or sets the player's rank name.
        /// </summary>
        public string RankName
        {
            get => ReferenceHub.serverRoles.NetworkMyText;
            set => ReferenceHub.serverRoles.SetText(value);
        }

        /// <summary>
        /// Gets the global badge of the player, can be null if none.
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
            get => string.IsNullOrEmpty(ReferenceHub.serverRoles.HiddenBadge);
            set
            {
                if (value)
                    ReferenceHub.characterClassManager.CmdRequestHideTag();
                else
                    ReferenceHub.characterClassManager.CallCmdRequestShowTag(false);
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the player is in the pocket dimension.
        /// </summary>
        public bool IsInPocketDimension => Map.FindParentRoom(GameObject).Type == RoomType.Pocket;

        /// <summary>
        /// Gets or sets a value indicating whether player should use stamina system.
        /// </summary>
        public bool IsUsingStamina { get; set; } = true;

        /// <summary>
        /// Gets or sets a player's SCP-330 usages counter.
        /// </summary>
        [Obsolete("Removed from the base-game.", true)]
        public int Scp330Usages
        {
            get => -1;
            set { }
        }

        /// <summary>
        /// Gets a value indicating whether player has hands.
        /// </summary>
        [Obsolete("Removed from the base-game.", true)]
        public bool HasHands => false;

        /// <summary>
        /// Gets player's items.
        /// </summary>
        public Inventory.SyncListItemInfo Items => Inventory.items;

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
        /// Gets the Player belonging to the GameObject, if any.
        /// </summary>
        /// <param name="gameObject">The player's <see cref="UnityEngine.GameObject"/>.</param>
        /// <returns>Returns a player or null if not found.</returns>
        public static Player Get(GameObject gameObject)
        {
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
        public static Player Get(int id)
        {
            if (IdsCache.TryGetValue(id, out Player player) && player?.ReferenceHub != null)
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
        /// Gets the player by his identifier.
        /// </summary>
        /// <param name="args">The player's nickname, steamID64 or Discord ID.</param>
        /// <returns>Returns the player found or null if not found.</returns>
        public static Player Get(string args)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(args))
                    return null;

                if (UserIdsCache.TryGetValue(args, out Player playerFound) && playerFound?.ReferenceHub != null)
                    return playerFound;

                if (int.TryParse(args, out int id))
                    return Get(id);

                if (args.EndsWith("@steam") || args.EndsWith("@discord") || args.EndsWith("@northwood") || args.EndsWith("@patreon"))
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
                    int maxNameLength = 31, lastnameDifference = 31;
                    string firstString = args.ToLower();

                    foreach (Player player in Dictionary.Values)
                    {
                        if (!player.IsVerified || player.Nickname == null)
                            continue;

                        if (!player.Nickname.Contains(args, StringComparison.OrdinalIgnoreCase))
                            continue;

                        if (firstString.Length < maxNameLength)
                        {
                            int x = maxNameLength - firstString.Length;
                            int y = maxNameLength - player.Nickname.Length;
                            string secondString = player.Nickname;

                            for (int i = 0; i < x; i++)
                                firstString += "z";

                            for (int i = 0; i < y; i++)
                                secondString += "z";

                            int nameDifference = firstString.GetDistance(secondString);
                            if (nameDifference < lastnameDifference)
                            {
                                lastnameDifference = nameDifference;
                                playerFound = player;
                            }
                        }
                    }
                }

                if (playerFound != null)
                    UserIdsCache[args] = playerFound;

                return playerFound;
            }
            catch (Exception exception)
            {
                Log.Error($"{typeof(Player).FullName}.{nameof(Get)} error: {exception}");
                return null;
            }
        }

        /// <inheritdoc cref="Map.GetCameraById(ushort)"/>
        [Obsolete("Use Map.GetCameraById instead.")]
        public Camera079 GetCameraById(ushort cameraId) => Map.GetCameraById(cameraId);

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
        /// Handcuff the player.
        /// </summary>
        /// <param name="cuffer">The cuffer player.</param>
        public void Handcuff(Player cuffer)
        {
            if (cuffer?.ReferenceHub == null)
                return;

            if (!IsCuffed &&
                cuffer.Inventory.items.Any(item => item.id == ItemType.Disarmer) &&
                Vector3.Distance(Position, cuffer.Position) <= 130f)
            {
                CufferId = cuffer.Id;
            }
        }

        /// <summary>
        /// Sets the player's <see cref="RoleType"/>.
        /// </summary>
        /// <param name="newRole">The new <see cref="RoleType"/> to be set.</param>
        /// <param name="lite">Indicates whether it should preserve the position and inventory after changing the role or not.</param>
        /// <param name="isEscaped">Indicates whether the player is escaped or not.</param>
        public void SetRole(RoleType newRole, bool lite = false, bool isEscaped = false)
        {
            ReferenceHub.characterClassManager.SetPlayersClass(newRole, GameObject, lite, isEscaped);
        }

        /// <summary>
        /// Broadcasts the given <see cref="Features.Broadcast"/> to the player.
        /// </summary>
        /// <param name="broadcast">The <see cref="Features.Broadcast"/> to be broadcasted.</param>
        public void Broadcast(Broadcast broadcast)
        {
            if (broadcast.Show)
                Broadcast(broadcast.Duration, broadcast.Content, broadcast.Type);
        }

        /// <summary>
        /// Drops an item from the player's inventory.
        /// </summary>
        /// <param name="item">The item to be dropped.</param>
        public void DropItem(Inventory.SyncItemInfo item)
        {
            Inventory.SetPickup(item.id, item.durability, Position, Inventory.camera.transform.rotation, item.modSight, item.modBarrel, item.modOther);
            Inventory.items.Remove(item);
        }

        /// <summary>
        /// Indicates whether or not the player has an item.
        /// </summary>
        /// <param name="targetItem">The item to search for.</param>
        /// <returns>true, if the player has it; otherwise, false.</returns>
        public bool HasItem(ItemType targetItem)
        {
            foreach (Inventory.SyncItemInfo item in this.Inventory.items)
            {
                if (item.id == targetItem)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Counts how many items of a certain <see cref="ItemType"/> a player has.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>How many items of that <see cref="ItemType"/> the player has.</returns>
        public int CountItem(ItemType item) => Inventory.items.Count(inventoryItem => inventoryItem.id == item);

        /// <summary>
        /// Removes an item from the player's inventory.
        /// </summary>
        /// <param name="item">The item to be removed.</param>
        public void RemoveItem(Inventory.SyncItemInfo item) => Inventory.items.Remove(item);

        /// <summary>
        /// Removes the held item from the player's inventory.
        /// </summary>
        public void RemoveItem() => Inventory.items.Remove(ReferenceHub.inventory.GetItemInHand());

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
        /// Hurts the player.
        /// </summary>
        /// <param name="damage">The damage to be inflicted.</param>
        /// <param name="damageType">The damage type.</param>
        /// <param name="attackerName">The attacker name.</param>
        /// <param name="attackerId">The attacker player id.</param>
        public void Hurt(float damage, DamageTypes.DamageType damageType = default, string attackerName = "WORLD", int attackerId = 0)
        {
            ReferenceHub.playerStats.HurtPlayer(new PlayerStats.HitInfo(damage, attackerName, damageType ?? DamageTypes.None, attackerId), GameObject);
        }

        /// <summary>
        /// Hurts the player.
        /// </summary>
        /// <param name="damage">The damage to be inflicted.</param>
        /// <param name="attacker">The attacker.</param>
        /// <param name="damageType">The damage type.</param>
        public void Hurt(float damage, Player attacker, DamageTypes.DamageType damageType = default) => Hurt(damage, damageType, attacker?.Nickname, attacker?.Id ?? 0);

        /// <summary>
        /// Kills the player.
        /// </summary>
        /// <param name="damageType">The <see cref="DamageTypes.DamageType"/> that will kill the player.</param>
        public void Kill(DamageTypes.DamageType damageType = default) => Hurt(-1f, damageType);

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
        /// <param name="success">Indicates whether the message should be highlighted as success or not.</param>
        /// <param name="pluginName">The plugin name.</param>
        public void RemoteAdminMessage(string message, bool success = true, string pluginName = null)
        {
            Sender.RaReply((pluginName ?? Assembly.GetCallingAssembly().GetName().Name) + "#" + message, success, true, string.Empty);
        }

        /// <summary>
        /// A simple broadcast to a <see cref="ReferenceHub"/>. Doesn't get logged to the console and can be monospaced.
        /// </summary>
        /// <param name="duration">The broadcast duration.</param>
        /// <param name="message">The message to be broadcasted.</param>
        /// <param name="type">The broadcast type.</param>
        public void Broadcast(ushort duration, string message, global::Broadcast.BroadcastFlags type = global::Broadcast.BroadcastFlags.Normal)
        {
            Server.Broadcast.TargetAddElement(Connection, message, duration, type);
        }

        /// <summary>
        /// Clears the player's brodcast. Doesn't get logged to the console.
        /// </summary>
        public void ClearBroadcasts() => Server.Broadcast.TargetClearElements(Connection);

        /// <summary>
        /// Add an item of the specified type with default durability(ammo/charge) and no mods to the player's inventory.
        /// </summary>
        /// <param name="itemType">The item to be added.</param>
        public void AddItem(ItemType itemType) => Inventory.AddNewItem(itemType);

        /// <summary>
        /// Add an item to the player's inventory.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        public void AddItem(Inventory.SyncItemInfo item) => Inventory.AddNewItem(item.id, item.durability, item.modSight, item.modBarrel, item.modOther);

        /// <summary>
        /// Resets the player's inventory to the provided list of items, clearing any items it already possess.
        /// </summary>
        /// <param name="newItems">The new items that have to be added to the inventory.</param>
        public void ResetInventory(List<ItemType> newItems)
        {
            ClearInventory();

            if (newItems.Count > 0)
            {
                foreach (ItemType item in newItems)
                    AddItem(item);
            }
        }

        /// <summary>
        /// Resets the player's inventory to the provided list of items, clearing any items it already possess.
        /// </summary>
        /// <param name="newItems">The new items that have to be added to the inventory.</param>
        public void ResetInventory(List<Inventory.SyncItemInfo> newItems) => ResetInventory(newItems.Select(item => item.id).ToList());

        /// <summary>
        /// Clears the player's inventory, including all ammo and items.
        /// </summary>
        public void ClearInventory() => Inventory.Clear();

        /// <summary>
        /// Drops all items in the player's inventory, including all ammo and items.
        /// </summary>
        public void DropItems() => Inventory.ServerDropAll();

        /// <summary>
        /// Sets the amount of a specified <see cref="AmmoType">ammo type</see>.
        /// </summary>
        /// <param name="ammoType">The <see cref="AmmoType"/> to be set.</param>
        /// <param name="amount">The amount of ammo to be set.</param>
        [Obsolete("Use Ammo instead.", true)]
        public void SetAmmo(AmmoType ammoType, uint amount) => ReferenceHub.ammoBox[(int)ammoType] = amount;

        /// <summary>
        /// Gets the amount of a specified <see cref="AmmoType"/>.
        /// </summary>
        /// <param name="ammoType">The <see cref="AmmoType"/> to get the amount from.</param>
        /// <returns>Returns the amount of the chosen <see cref="AmmoType"/>.</returns>
        [Obsolete("Use Ammo instead.", true)]
        public uint GetAmmo(AmmoType ammoType) => ReferenceHub.ammoBox[(int)ammoType];

        /// <summary>
        /// Simple way to show a hint to the player.
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
        /// Gets player's ping.
        /// </summary>
        /// <returns>Return player ping.</returns>
        public int Ping => LiteNetLib4MirrorServer.GetPing(Connection.connectionId);

        /// Safely gets an <see cref="object"/> from <see cref="Player.SessionVariables"/>, then casts it to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The returned object type.</typeparam>
        /// <param name="key">The key of the object to get.</param>
        /// <param name="result">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter is used.</param>
        /// <returns><see langword="true"/> if the SessionVariables contains an element with the specified key; otherwise, <see langword="false"/>.</returns>
        public bool TryGetSessionVariable<T>(string key, out T result)
        {
            if (SessionVariables.TryGetValue(key, out var value) && value is T type)
            {
                result = type;
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Gets a <see cref="bool"/> describing whether or not the given <see cref="PlayerEffect">status effect</see> is currently enabled.
        /// </summary>
        /// <typeparam name="T">The <see cref="PlayerEffect"/> to check.</typeparam>
        /// <returns>A <see cref="bool"/> determining whether or not the player effect is active.</returns>
        public bool GetEffectActive<T>()
            where T : PlayerEffect
        {
            if (ReferenceHub.playerEffectsController.AllEffects.TryGetValue(typeof(T), out PlayerEffect playerEffect))
                return playerEffect.Enabled;

            return false;
        }

        /// <summary>
        ///  Disables all currently active <see cref="PlayerEffect">status effects</see>.
        /// </summary>
        public void DisableAllEffects()
        {
            foreach (KeyValuePair<Type, PlayerEffect> effect in ReferenceHub.playerEffectsController.AllEffects)
            {
                if (effect.Value.Enabled)
                    effect.Value.ServerDisable();
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
            if (TryGetEffect(effect, out var playerEffect))
                playerEffect.ServerDisable();
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
        /// <returns>A bool indicating whether or not the effect was valid and successfully enabled.</returns>
        public bool EnableEffect(string effect, float duration = 0f, bool addDurationIfActive = false)
            => ReferenceHub.playerEffectsController.EnableByString(effect, duration, addDurationIfActive);

        /// <summary>
        /// Enables a <see cref="EffectType">status effect</see> on the player.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/> to enable.</param>
        /// <param name="duration">The amount of time the effect will be active for.</param>
        /// <param name="addDurationIfActive">If the effect is already active, setting to true will add this duration onto the effect.</param>
        public void EnableEffect(EffectType effect, float duration = 0f, bool addDurationIfActive = false)
        {
            if (TryGetEffect(effect, out var pEffect))
                ReferenceHub.playerEffectsController.EnableEffect(pEffect, duration, addDurationIfActive);
        }

        /// <summary>
        /// Gets an instance of <see cref="PlayerEffect"/> by <see cref="EffectType"/>.
        /// </summary>
        /// <param name="effect">The <see cref="EffectType"/>.</param>
        /// <returns>The <see cref="PlayerEffect"/>.</returns>
        public PlayerEffect GetEffect(EffectType effect)
        {
            ReferenceHub.playerEffectsController.AllEffects.TryGetValue(effect.Type(), out var playerEffect);

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

            return playerEffect != null;
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
        /// Changes the intensity of a <see cref="PlayerEffect">status effect</see>.
        /// </summary>
        /// <param name="effect">The name of the <see cref="PlayerEffect"/> to enable.</param>
        /// <param name="intensity">The intensity of the effect.</param>
        /// <param name="duration">The new length of the effect. Defaults to infinite length.</param>
        public void ChangeEffectIntensity(string effect, byte intensity, float duration = 0) => ReferenceHub.playerEffectsController.ChangeByString(effect, intensity, duration);

        /// <summary>
        /// Removes the player's hands.
        /// </summary>
        [Obsolete("Removed from the base-game.", true)]
        public void RemoveHands()
        {
        }

        /// <inheritdoc/>
        public override string ToString() => $"{Id} {Nickname} {UserId} {Role} {Team}";
    }
}
