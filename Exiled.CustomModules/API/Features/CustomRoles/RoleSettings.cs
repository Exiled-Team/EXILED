// -----------------------------------------------------------------------
// <copyright file="RoleSettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features
{
    using System.Collections.Generic;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.API.Features.Roles;
    using Exiled.API.Features.Spawn;
    using PlayerRoles;

    /// <summary>
    /// A tool to easily setup roles.
    /// </summary>
    public class RoleSettings : TypeCastObject<RoleSettings>, IAdditiveProperty
    {
        /// <summary>
        /// Gets the default <see cref="RoleSettings"/> values.
        /// <para>It refers to the base-game human roles behavior.</para>
        /// </summary>
        public static RoleSettings Default { get; } = new();

        /// <summary>
        /// Gets the SCP preset referring to the base-game SCPs behavior.
        /// </summary>
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
        public virtual bool IsRoleDynamic { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the player's role should use the specified <see cref="Role"/> only.
        /// </summary>
        public virtual bool UseDefaultRoleOnly { get; set; } = true;

        /// <summary>
        /// Gets or sets a <see cref="string"/> for the console message given to players when they receive a role.
        /// </summary>
        public virtual string ConsoleMessage { get; set; } = "You have spawned as {role}!";

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        public virtual float Scale { get; set; } = 1f;

        /// <summary>
        /// Gets or sets the initial health.
        /// </summary>
        public virtual float Health { get; set; } = 100f;

        /// <summary>
        /// Gets or sets the max health.
        /// </summary>
        public virtual int MaxHealth { get; set; } = 100;

        /// <summary>
        /// Gets or sets the initial artificial health.
        /// </summary>
        public virtual float ArtificialHealth { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the max artificial health.
        /// </summary>
        public virtual float MaxArtificialHealth { get; set; } = 100f;

        /// <summary>
        /// Gets or sets the broadcast to be displayed as soon as the role is assigned.
        /// </summary>
        public virtual Broadcast InitialBroadcast { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Features.Spawn.SpawnProperties"/>.
        /// </summary>
        public virtual SpawnProperties SpawnProperties { get; set; }

        /// <summary>
        /// Gets or sets the unique <see cref="RoleTypeId"/> defining the role to be assigned.
        /// </summary>
        public virtual RoleTypeId UniqueRole { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="CustomRole"/> assignment should maintain the player's current position.
        /// </summary>
        public virtual bool PreservePosition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="CustomRole"/> assignment should maintain the player's current inventory.
        /// </summary>
        public virtual bool PreserveInventory { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="RoleTypeId"/>[] containing all dynamic roles.
        /// <para>Dynamic roles are specific roles that, if assigned, do not result in the removal of the <see cref="CustomRole"/> from the player.</para>
        /// </summary>
        public virtual RoleTypeId[] DynamicRoles { get; set; } = new RoleTypeId[] { };

        /// <summary>
        /// Gets or sets a <see cref="DamageType"/>[] containing all the ignored damage types.
        /// </summary>
        public virtual DamageType[] IgnoredDamageTypes { get; set; } = new DamageType[] { };

        /// <summary>
        /// Gets or sets a <see cref="DamageType"/>[] containing all the allowed damage types.
        /// </summary>
        public virtual DamageType[] AllowedDamageTypes { get; set; } = new DamageType[] { };

        /// <summary>
        /// Gets or sets a <see cref="DoorType"/>[] containing all doors that can be bypassed.
        /// </summary>
        public virtual DoorType[] BypassableDoors { get; set; } = new DoorType[] { };

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing cached <see cref="string"/> and their  <see cref="Dictionary{TKey, TValue}"/> which is cached Role with FF multiplier.
        /// </summary>
        public virtual Dictionary<RoleTypeId, float> FriendlyFireMultiplier { get; set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether SCPs can be hurt.
        /// </summary>
        public virtual bool CanHurtScps { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether SCPs can hurt the owner.
        /// </summary>
        public virtual bool CanBeHurtByScps { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can enter pocket dimension.
        /// </summary>
        public virtual bool CanEnterPocketDimension { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can use intercom.
        /// </summary>
        public virtual bool CanUseIntercom { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can use the voicechat.
        /// </summary>
        public virtual bool CanUseVoiceChat { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can place blood.
        /// </summary>
        public virtual bool CanPlaceBlood { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can be handcuffed.
        /// </summary>
        public virtual bool CanBeHandcuffed { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can use elevators.
        /// </summary>
        public virtual bool CanUseElevators { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can bypass checkpoints.
        /// </summary>
        public virtual bool CanBypassCheckpoints { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can activate warhead.
        /// </summary>
        public virtual bool CanActivateWarhead { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can activate workstations.
        /// </summary>
        public virtual bool CanActivateWorkstations { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can activate generators.
        /// </summary>
        public virtual bool CanActivateGenerators { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can pickup items.
        /// </summary>
        public virtual bool CanPickupItems { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can drop items.
        /// </summary>
        public virtual bool CanDropItems { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can select items from their inventory.
        /// </summary>
        public virtual bool CanSelectItems { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can look at Scp173.
        /// </summary>
        public virtual bool DoesLookingAffectScp173 { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the owner can trigger Scp096.
        /// </summary>
        public virtual bool DoesLookingAffectScp096 { get; set; } = true;

        /// <summary>
        /// Gets or sets the custom info.
        /// </summary>
        public virtual string CustomInfo { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="PlayerInfoArea"/> should be hidden.
        /// </summary>
        public virtual bool HideInfoArea { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the C.A.S.S.I.E death announcement should be played when the owner dies.
        /// </summary>
        public virtual bool IsDeathAnnouncementEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets the C.A.S.S.I.E announcement to be played when the owner dies from an unhandled or unknown termination cause.
        /// </summary>
        public virtual string UnknownTerminationCauseAnnouncement { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing all the C.A.S.S.I.E announcements
        /// to be played when the owner gets killed by a player with the corresponding <see cref="RoleTypeId"/>.
        /// </summary>
        public virtual Dictionary<RoleTypeId, string> KilledByRoleAnnouncements { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing all the C.A.S.S.I.E announcements
        /// to be played when the owner gets killed by a player with the corresponding <see cref="object"/>.
        /// </summary>
        public virtual Dictionary<object, string> KilledByCustomRoleAnnouncements { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing all the C.A.S.S.I.E announcements
        /// to be played when the owner gets killed by a player belonging to the corresponding <see cref="Team"/>.
        /// </summary>
        public virtual Dictionary<Team, string> KilledByTeamAnnouncements { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing all the C.A.S.S.I.E announcements
        /// to be played when the owner gets killed by a player belonging to the corresponding <see cref="object"/>.
        /// </summary>
        public virtual Dictionary<object, string> KilledByCustomTeamAnnouncements { get; set; } = new();
    }
}