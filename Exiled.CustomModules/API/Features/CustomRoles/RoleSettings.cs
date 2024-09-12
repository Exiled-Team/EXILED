// -----------------------------------------------------------------------
// <copyright file="RoleSettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;

namespace Exiled.CustomModules.API.Features.CustomRoles
{
    using System.Collections.Generic;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.API.Features.Roles;
    using Exiled.API.Features.Spawn;
    using PlayerRoles;
    using YamlDotNet.Serialization;

    /// <summary>
    /// A tool to easily setup roles.
    /// </summary>
    public class RoleSettings : TypeCastObject<RoleSettings>, IAdditiveProperty
    {
        /// <summary>
        /// Gets the default <see cref="RoleSettings"/> values.
        /// <para>It refers to the base-game human roles behavior.</para>
        /// </summary>
        [YamlIgnore]
        public static RoleSettings Default { get; } = new();

        /// <summary>
        /// Gets the SCP preset referring to the base-game SCPs behavior.
        /// </summary>
        [YamlIgnore]
        public static RoleSettings ScpPreset { get; } = new()
        {
            CanHurtScps = false,
            CanPlaceBlood = false,
            CanBypassCheckpoints = true,
            CanEnterPocketDimension = false,
            CanUseIntercom = false,
            CanActivateGenerators = false,
            CanActivateWorkstations = false,
            CanBeHandcuffed = false,
            CanSelectItems = false,
            CanDropItems = false,
            CanPickupItems = false,
            DoesLookingAffectScp096 = false,
            DoesLookingAffectScp173 = false,
        };

        /// <summary>
        /// Gets or sets a value indicating whether the player's role is dynamic.
        /// </summary>
        [Description("Indicates whether the player's role is dynamic.")]
        public virtual bool IsRoleDynamic { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the player's role should use the specified <see cref="Role"/> only.
        /// </summary>
        [Description("Indicates whether the player's role should use the specified Role only.")]
        public virtual bool UseDefaultRoleOnly { get; set; } = true;

        /// <summary>
        /// Gets or sets a <see cref="string"/> for the console message given to players when they receive a role.
        /// </summary>
        [Description("The console message given to players when they receive a role.")]
        public virtual string ConsoleMessage { get; set; } = "You have spawned as {role}!";

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        [Description("The scale of the player.")]
        public virtual float Scale { get; set; } = 1f;

        /// <summary>
        /// Gets or sets the initial health.
        /// </summary>
        [Description("The initial health of the player.")]
        public virtual float Health { get; set; } = 100f;

        /// <summary>
        /// Gets or sets the max health.
        /// </summary>
        [Description("The maximum health of the player.")]
        public virtual int MaxHealth { get; set; } = 100;

        /// <summary>
        /// Gets or sets the initial artificial health.
        /// </summary>
        [Description("The initial artificial health of the player.")]
        public virtual float ArtificialHealth { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the max artificial health.
        /// </summary>
        [Description("The maximum artificial health of the player.")]
        public virtual float MaxArtificialHealth { get; set; } = 100f;

        /// <summary>
        /// Gets or sets the text to be displayed as soon as the role is assigned.
        /// </summary>
        [Description("The text to be displayed as soon as the role is assigned.")]
        public virtual TextDisplay SpawnedText { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Features.Spawn.SpawnProperties"/>.
        /// </summary>
        [Description("The spawn properties for the role.")]
        public virtual SpawnProperties SpawnProperties { get; set; } = new();

        /// <summary>
        /// Gets or sets the unique <see cref="RoleTypeId"/> defining the role to be assigned.
        /// </summary>
        [Description("The unique role identifier to be assigned.")]
        public virtual RoleTypeId UniqueRole { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="CustomRole"/> assignment should maintain the player's current position.
        /// </summary>
        [Description("Indicates whether the role assignment should maintain the player's current position.")]
        public virtual bool PreservePosition { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="RoleSpawnFlags"/>.
        /// </summary>
        [Description("The flags indicating special conditions for role spawning.")]
        public virtual RoleSpawnFlags SpawnFlags { get; set; } = RoleSpawnFlags.All;

        /// <summary>
        /// Gets or sets the <see cref="RoleChangeReason"/>.
        /// </summary>
        [Description("The reason for the role change.")]
        public virtual RoleChangeReason SpawnReason { get; set; } = RoleChangeReason.RemoteAdmin;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="CustomRole"/> assignment should maintain the player's current inventory.
        /// </summary>
        [Description("Indicates whether the role assignment should maintain the player's current inventory.")]
        public virtual bool PreserveInventory { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="RoleTypeId"/>[] containing all dynamic roles.
        /// <para>Dynamic roles are specific roles that, if assigned, do not result in the removal of the <see cref="CustomRole"/> from the player.</para>
        /// </summary>
        [Description("Array of dynamic roles that, if assigned, do not result in removal of the custom role.")]
        public virtual RoleTypeId[] DynamicRoles { get; set; } = new RoleTypeId[] { };

        /// <summary>
        /// Gets or sets a <see cref="DamageType"/>[] containing all the ignored damage types.
        /// </summary>
        [Description("Array of damage types that are ignored.")]
        public virtual DamageType[] IgnoredDamageTypes { get; set; } = new DamageType[] { };

        /// <summary>
        /// Gets or sets a <see cref="DamageType"/>[] containing all the allowed damage types.
        /// </summary>
        [Description("Array of damage types that are allowed.")]
        public virtual DamageType[] AllowedDamageTypes { get; set; } = new DamageType[] { };

        /// <summary>
        /// Gets or sets a <see cref="DoorType"/>[] containing all doors that can be bypassed.
        /// </summary>
        [Description("Array of door types that can be bypassed.")]
        public virtual DoorType[] BypassableDoors { get; set; } = new DoorType[] { };

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing cached <see cref="string"/> and their  <see cref="Dictionary{TKey, TValue}"/> which is cached Role with FF multiplier.
        /// </summary>
        [Description("Dictionary containing cached roles with their friendly fire multiplier.")]
        public virtual Dictionary<RoleTypeId, float> FriendlyFireMultiplier { get; set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether SCPs can be hurt.
        /// </summary>
        [Description("Indicates whether SCPs can be hurt.")]
        public virtual bool CanHurtScps { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether SCPs can hurt the owner.
        /// </summary>
        [Description("Indicates whether SCPs can hurt the owner.")]
        public virtual bool CanBeHurtByScps { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can enter pocket dimension.
        /// </summary>
        [Description("Indicates whether the owner can enter the pocket dimension.")]
        public virtual bool CanEnterPocketDimension { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can use intercom.
        /// </summary>
        [Description("Indicates whether the owner can use the intercom.")]
        public virtual bool CanUseIntercom { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can use the voicechat.
        /// </summary>
        [Description("Indicates whether the owner can use voice chat.")]
        public virtual bool CanUseVoiceChat { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can place blood.
        /// </summary>
        [Description("Indicates whether the owner can place blood.")]
        public virtual bool CanPlaceBlood { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can be handcuffed.
        /// </summary>
        [Description("Indicates whether the owner can be handcuffed.")]
        public virtual bool CanBeHandcuffed { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can use elevators.
        /// </summary>
        [Description("Indicates whether the owner can use elevators.")]
        public virtual bool CanUseElevators { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can bypass checkpoints.
        /// </summary>
        [Description("Indicates whether the owner can bypass checkpoints.")]
        public virtual bool CanBypassCheckpoints { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can activate warhead.
        /// </summary>
        [Description("Indicates whether the owner can activate the warhead.")]
        public virtual bool CanActivateWarhead { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can activate workstations.
        /// </summary>
        [Description("Indicates whether the owner can activate workstations.")]
        public virtual bool CanActivateWorkstations { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can activate generators.
        /// </summary>
        [Description("Indicates whether the owner can activate generators.")]
        public virtual bool CanActivateGenerators { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can pickup items.
        /// </summary>
        [Description("Indicates whether the owner can pick up items.")]
        public virtual bool CanPickupItems { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can drop items.
        /// </summary>
        [Description("Indicates whether the owner can drop items.")]
        public virtual bool CanDropItems { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can select items from their inventory.
        /// </summary>
        [Description("Indicates whether the owner can select items from their inventory.")]
        public virtual bool CanSelectItems { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can look at Scp173.
        /// </summary>
        [Description("Indicates whether the owner can look at SCP-173.")]
        public virtual bool DoesLookingAffectScp173 { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can trigger Scp096.
        /// </summary>
        [Description("Indicates whether the owner can trigger SCP-096.")]
        public virtual bool DoesLookingAffectScp096 { get; set; } = true;

        /// <summary>
        /// Gets or sets the custom info.
        /// </summary>
        [Description("Custom information related to the owner.")]
        public virtual string CustomInfo { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="PlayerInfoArea"/> should be hidden.
        /// </summary>
        [Description("Indicates whether the PlayerInfoArea should be hidden.")]
        public virtual bool HideInfoArea { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the C.A.S.S.I.E death announcement should be played when the owner dies.
        /// </summary>
        [Description("Indicates whether the C.A.S.S.I.E death announcement should be played when the owner dies.")]
        public virtual bool IsDeathAnnouncementEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets the C.A.S.S.I.E announcement to be played when the owner dies from an unhandled or unknown termination cause.
        /// </summary>
        [Description("The C.A.S.S.I.E announcement to be played when the owner dies from an unhandled or unknown cause.")]
        public virtual string UnknownTerminationCauseAnnouncement { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing all the C.A.S.S.I.E announcements
        /// to be played when the owner gets killed by a player with the corresponding <see cref="RoleTypeId"/>.
        /// </summary>
        [Description("Dictionary containing announcements for when the owner is killed by a player with a specific RoleTypeId.")]
        public virtual Dictionary<RoleTypeId, string> KilledByRoleAnnouncements { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing all the C.A.S.S.I.E announcements
        /// to be played when the owner gets killed by a player with the corresponding <see cref="object"/>.
        /// </summary>
        [Description("Dictionary containing announcements for when the owner is killed by a player with a specific custom role.")]
        public virtual Dictionary<object, string> KilledByCustomRoleAnnouncements { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing all the C.A.S.S.I.E announcements
        /// to be played when the owner gets killed by a player belonging to the corresponding <see cref="Team"/>.
        /// </summary>
        [Description("Dictionary containing announcements for when the owner is killed by a player from a specific team.")]
        public virtual Dictionary<Team, string> KilledByTeamAnnouncements { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing all the C.A.S.S.I.E announcements
        /// to be played when the owner gets killed by a player belonging to the corresponding <see cref="object"/>.
        /// </summary>
        [Description("Dictionary containing announcements for when the owner is killed by a player from a specific custom team.")]
        public virtual Dictionary<object, string> KilledByCustomTeamAnnouncements { get; set; } = new();
    }
}