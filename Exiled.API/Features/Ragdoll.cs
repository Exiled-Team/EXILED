// -----------------------------------------------------------------------
// <copyright file="Ragdoll.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.Linq;

    using UnityEngine;

    using RagDoll = global::Ragdoll;

    /// <summary>
    /// A set of tools to handle the ragdolls more easily.
    /// </summary>
    public class Ragdoll
    {
        private readonly RagDoll ragdoll;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ragdoll"/> class.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleType"/> to use as ragdoll.</param>
        /// <param name="ragdollInfo"><see cref="RagDoll.Info"/> object containing the ragdoll's info.</param>
        /// <param name="position">Where the ragdoll will be spawned.</param>
        /// <param name="rotation">The rotation for the ragdoll.</param>
        /// <param name="velocity">The initial velocity the ragdoll will have, as if it was exploded.</param>
        /// <param name="allowRecall">Sets this ragdoll as respawnable by SCP-049.</param>
        /// <param name="scp096Death">Sets this ragdoll as Scp096's victim.</param>
        public Ragdoll(RoleType roleType, RagDoll.Info ragdollInfo, Vector3 position, Quaternion rotation = default, Vector3 velocity = default, bool allowRecall = false, bool scp096Death = false)
        {
            Role role = CharacterClassManager._staticClasses.SafeGet(roleType);
            GameObject gameObject = Object.Instantiate(role.model_ragdoll, position + role.ragdoll_offset.position, Quaternion.Euler(rotation.eulerAngles + role.ragdoll_offset.rotation));
            ragdoll = gameObject.GetComponent<RagDoll>();
            ragdoll.Networkowner = ragdollInfo != null ? ragdollInfo : DefaultRagdollOwner;
            ragdoll.NetworkPlayerVelo = velocity;
            ragdoll.NetworkallowRecall = allowRecall;
            ragdoll.NetworkSCP096Death = scp096Death;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ragdoll"/> class.
        /// </summary>
        /// <param name="ragdoll">The encapsulated <see cref="RagDoll"/>.</param>
        internal Ragdoll(RagDoll ragdoll) => this.ragdoll = ragdoll;

        /// <summary>
        /// Gets the Default <see cref="RagDoll.Info"/>,
        /// used in <see cref="Spawn(RoleType, string, PlayerStats.HitInfo, Vector3, Quaternion, Vector3, bool, int, bool, string)"/>.
        /// </summary>
        /// <remarks>
        /// This value can be modified to change the default Ragdoll's info.
        /// </remarks>
        public static RagDoll.Info DefaultRagdollOwner => new RagDoll.Info()
        {
            ownerHLAPI_id = null,
            PlayerId = -1,
            DeathCause = new PlayerStats.HitInfo(-1f, "[REDACTED]", DamageTypes.None, -1, false),
            ClassColor = new Color(1f, 0.556f, 0f),
            FullName = "Class-D",
            Nick = "[REDACTED]",
        };

        /// <summary>
        /// Gets the <see cref="RagDoll"/>.
        /// </summary>
        public RagDoll Base => ragdoll;

        /// <summary>
        /// Gets the ragdoll's name.
        /// </summary>
        public string Name => ragdoll.name;

        /// <summary>
        /// Gets the ragdoll's GameObject.
        /// </summary>
        public GameObject GameObject => ragdoll.gameObject;

        /// <summary>
        /// Gets the owner <see cref="Player"/>. Can be null if the ragdoll does not have an owner.
        /// </summary>
        public Player Owner => Player.Get(ragdoll.owner.PlayerId);

        /// <summary>
        /// Gets the killing <see cref="Player"/>. Can be null if the killer is not a player.
        /// </summary>
        public Player Killer => ragdoll.owner.DeathCause.IsPlayer ? Player.Get(ragdoll.owner.DeathCause.RHub) : null;

        /// <summary>
        /// Gets the <see cref="RoleType"/> of the ragdoll.
        /// </summary>
        public RoleType Role => CharacterClassManager._staticClasses.FirstOrDefault(role => role.fullName == ragdoll.owner.FullName).roleId;

        /// <summary>
        /// Gets or sets a value indicating whether or not the ragdoll is respawnable by SCP-049.
        /// </summary>
        public bool AllowRecall
        {
            get => ragdoll.allowRecall;
            set => ragdoll.allowRecall = value;
        }

        /// <summary>
        /// Gets a value indicating whether or not the ragdoll death is caused by Scp096.
        /// </summary>
        public bool Scp096Death => ragdoll.SCP096Death;

        /// <summary>
        /// Gets the <see cref="Room"/> the ragdoll is located in.
        /// </summary>
        public Room Room => Map.FindParentRoom(GameObject);

        /// <summary>
        /// Gets or sets the ragdoll's position.
        /// </summary>
        public Vector3 Position
        {
            get => ragdoll.transform.position;
            set
            {
                Mirror.NetworkServer.UnSpawn(GameObject);
                ragdoll.transform.position = value;
                Mirror.NetworkServer.Spawn(GameObject);
            }
        }

        /// <summary>
        /// Gets or sets the ragdoll's scale.
        /// </summary>
        public Vector3 Scale
        {
            get => ragdoll.transform.localScale;
            set
            {
                Mirror.NetworkServer.UnSpawn(GameObject);
                ragdoll.transform.localScale = value;
                Mirror.NetworkServer.Spawn(GameObject);
            }
        }

        /// <summary>
        /// Spawns a ragdoll for a player on a certain position.
        /// </summary>
        /// <param name="victim">Victim, represented as a player.</param>
        /// <param name="deathCause">The message to be displayed as his death.</param>
        /// <param name="position">Where the ragdoll will be spawned.</param>
        /// <param name="rotation">The rotation for the ragdoll.</param>
        /// <param name="velocity">The initial velocity the ragdoll will have, as if it was exploded.</param>
        /// <param name="allowRecall">Sets this ragdoll as respawnable by SCP-049.</param>
        /// <param name="scp096Death">Sets this ragdoll as Scp096's victim.</param>
        /// <returns>The spawned Ragdoll.</returns>
        public static Ragdoll Spawn(Player victim, DamageTypes.DamageType deathCause, Vector3 position, Quaternion rotation = default, Vector3 velocity = default, bool allowRecall = true, bool scp096Death = false)
        {
            return Spawn(
                        victim.Role,
                        deathCause,
                        victim.DisplayNickname,
                        position,
                        rotation,
                        velocity,
                        allowRecall,
                        victim.Id,
                        scp096Death,
                        victim.GameObject.GetComponent<Dissonance.Integrations.MirrorIgnorance.MirrorIgnorancePlayer>().PlayerId);
        }

        /// <summary>
        /// Spawns a ragdoll on the map based on the different arguments.
        /// </summary>
        /// <remarks>
        /// Tip: You can do, for example, '<paramref name="velocity"/>: "Vector3.up * 3"' to skip parameters.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Code to spawn a fake ragdoll
        /// if (ev.Player == MyPlugin.TheInmortalPlayer)
        /// {
        ///     var fakeRagdoll = Ragdoll.Spawn(ev.Player.Role, ev.Player.Position, victimNick: ev.Player.DisplayNickname, playerId: ev.Player.Id);
        /// }
        /// </code>
        /// </example>
        /// <param name="roleType">The <see cref="RoleType"/> to use as ragdoll.</param>
        /// <param name="ragdollInfo"><see cref="RagDoll.Info"/> object containing the ragdoll's info.</param>
        /// <param name="position">Where the ragdoll will be spawned.</param>
        /// <param name="rotation">The rotation for the ragdoll.</param>
        /// <param name="velocity">The initial velocity the ragdoll will have, as if it was exploded.</param>
        /// <param name="allowRecall">Sets this ragdoll as respawnable by SCP-049.</param>
        /// <param name="scp096Death">Sets this ragdoll as Scp096's victim.</param>
        /// <returns>The spawned Ragdoll.</returns>
        public static Ragdoll Spawn(
                RoleType roleType,
                RagDoll.Info ragdollInfo,
                Vector3 position,
                Quaternion rotation = default,
                Vector3 velocity = default,
                bool allowRecall = false,
                bool scp096Death = false)
        {
            Role role = CharacterClassManager._staticClasses.SafeGet(roleType);
            GameObject gameObject = Object.Instantiate(role.model_ragdoll, position + role.ragdoll_offset.position, Quaternion.Euler(rotation.eulerAngles + role.ragdoll_offset.rotation));

            if (role.model_ragdoll == null)
                return null;

            RagDoll ragdoll = gameObject.GetComponent<global::Ragdoll>();
            ragdoll.Networkowner = ragdollInfo != null ? ragdollInfo : DefaultRagdollOwner;
            ragdoll.NetworkallowRecall = allowRecall;
            ragdoll.NetworkPlayerVelo = velocity;
            ragdoll.NetworkSCP096Death = scp096Death;

            Mirror.NetworkServer.Spawn(gameObject);

            return new Ragdoll(ragdoll);
        }

        /// <summary>
        /// Spawns a ragdoll on the map based on the different arguments.
        /// </summary>
        /// <remarks>
        /// Tip: You can do, for example, '<paramref name="velocity"/>: "Vector3.up * 3"' to skip parameters.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Code to spawn a fake ragdoll
        /// if (ev.Player == MyPlugin.TheInmortalPlayer)
        /// {
        ///     var fakeRagdoll = Ragdoll.Spawn(ev.Player.Role, ev.Player.Position, victimNick: ev.Player.DisplayNickname, playerId: ev.Player.Id);
        /// }
        /// </code>
        /// </example>
        /// <param name="role">The <see cref="Role"/> to use as ragdoll.</param>
        /// <param name="ragdollInfo"><see cref="RagDoll.Info"/> object containing the ragdoll's info.</param>
        /// <param name="position">Where the ragdoll will be spawned.</param>
        /// <param name="rotation">The rotation for the ragdoll.</param>
        /// <param name="velocity">The initial velocity the ragdoll will have, as if it was exploded.</param>
        /// <param name="allowRecall">Sets this ragdoll as respawnable by SCP-049.</param>
        /// <param name="scp096Death">Sets this ragdoll as Scp096's victim.</param>
        /// <returns>The spawned Ragdoll.</returns>
        public static Ragdoll Spawn(
                Role role,
                RagDoll.Info ragdollInfo,
                Vector3 position,
                Quaternion rotation = default,
                Vector3 velocity = default,
                bool allowRecall = false,
                bool scp096Death = false)
        {
            GameObject gameObject = Object.Instantiate(role.model_ragdoll, position + role.ragdoll_offset.position, Quaternion.Euler(rotation.eulerAngles + role.ragdoll_offset.rotation));

            if (role.model_ragdoll == null)
                return null;

            RagDoll ragdoll = gameObject.GetComponent<global::Ragdoll>();
            ragdoll.Networkowner = ragdollInfo != null ? ragdollInfo : DefaultRagdollOwner;
            ragdoll.NetworkallowRecall = allowRecall;
            ragdoll.NetworkPlayerVelo = velocity;
            ragdoll.NetworkSCP096Death = scp096Death;

            Mirror.NetworkServer.Spawn(gameObject);

            return new Ragdoll(ragdoll);
        }

        /// <summary>
        /// Spawns a ragdoll on the map based on the different arguments.
        /// </summary>
        /// <remarks>
        /// Tip: You can do, for example, '<paramref name="velocity"/>: "Vector3.up * 3"' to skip parameters.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Code to spawn a fake ragdoll
        /// if (ev.Player == MyPlugin.TheInmortalPlayer)
        /// {
        ///     var fakeRagdoll = Ragdoll.Spawn(ev.Player.Role, ev.Player.Position, victimNick: ev.Player.DisplayNickname, playerId: ev.Player.Id);
        /// }
        /// </code>
        /// </example>
        /// <param name="roleType">The <see cref="RoleType"/> to use as ragdoll.</param>
        /// <param name="victimNick">The name from the victim, who the corpse belongs to.</param>
        /// <param name="hitInfo">The <see cref="PlayerStats.HitInfo"/> that displays who killed this ragdoll, and using which tool.</param>
        /// <param name="position">Where the ragdoll will be spawned.</param>
        /// <param name="rotation">The rotation for the ragdoll.</param>
        /// <param name="velocity">The initial velocity the ragdoll will have, as if it was exploded.</param>
        /// <param name="allowRecall">Sets this ragdoll as respawnable by SCP-049.</param>
        /// <param name="playerId">Used for recall. The <see cref="Player.Id"/> to be recalled.</param>
        /// <param name="scp096Death">Sets this ragdoll as Scp096's victim.</param>
        /// <param name="mirrorOwnerId">Can be ignored. The <see cref="Dissonance.Integrations.MirrorIgnorance.MirrorIgnorancePlayer"/>'s PlayerId field, likely used in the client.</param>
        /// <returns>The spawned Ragdoll.</returns>
        public static Ragdoll Spawn(
                RoleType roleType,
                string victimNick,
                PlayerStats.HitInfo hitInfo,
                Vector3 position,
                Quaternion rotation = default,
                Vector3 velocity = default,
                bool allowRecall = false,
                int playerId = -1,
                bool scp096Death = false,
                string mirrorOwnerId = null)
        {
            Role role = CharacterClassManager._staticClasses.SafeGet(roleType);

            if (role.model_ragdoll == null)
                return null;
            var @default = DefaultRagdollOwner;

            var ragdollInfo = new RagDoll.Info()
            {
                ownerHLAPI_id = mirrorOwnerId ?? @default.ownerHLAPI_id,
                PlayerId = playerId,
                DeathCause = hitInfo != default ? hitInfo : @default.DeathCause,
                ClassColor = role.classColor,
                FullName = role.fullName,
                Nick = victimNick,
            };

            return Spawn(role, ragdollInfo, position, rotation, velocity, allowRecall, scp096Death);
        }

        /// <summary>
        /// Spawns a ragdoll on the map based on the different arguments.
        /// </summary>
        /// <remarks>
        /// Tip: You can do '<paramref name="allowRecall"/>: true, <paramref name="playerId"/>: MyPlayer.Id' to skip parameters.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Code to spawn a fake ragdoll
        /// if (ev.Player == MyPlugin.TheInmortalPlayer)
        /// {
        ///     var fakeRagdoll = Ragdoll.Spawn(RoleType.ClassD, DamageTypes.Fall, "The Falling Guy", new Vector3(1234f, -1f, 4321f));
        /// }
        /// </code>
        /// </example>
        /// <param name="roleType">The <see cref="RoleType"/> to use as ragdoll.</param>
        /// <param name="deathCause">The death cause, expressed as a <see cref="DamageTypes.DamageType"/>.</param>
        /// <param name="victimNick">The name from the victim, who the corpse belongs to.</param>
        /// <param name="position">Where the ragdoll will be spawned.</param>
        /// <param name="rotation">The rotation for the ragdoll.</param>
        /// <param name="velocity">The initial velocity the ragdoll will have, as if it was exploded.</param>
        /// <param name="allowRecall">Sets this ragdoll as respawnable by SCP-049. Must have a valid <paramref name="playerId"/>.</param>
        /// <param name="playerId">Used for recall. The <see cref="Player.Id"/> to be recalled.</param>
        /// <param name="scp096Death">Sets this ragdoll as Scp096's victim.</param>
        /// <param name="mirrorOwnerId">Can be ignored. The <see cref="Dissonance.Integrations.MirrorIgnorance.MirrorIgnorancePlayer"/>'s PlayerId field.</param>
        /// <returns>The spawned Ragdoll.</returns>
        public static Ragdoll Spawn(
                RoleType roleType,
                DamageTypes.DamageType deathCause,
                string victimNick,
                Vector3 position,
                Quaternion rotation = default,
                Vector3 velocity = default,
                bool allowRecall = false,
                int playerId = -1,
                bool scp096Death = false,
                string mirrorOwnerId = null)
        {
            var @default = DefaultRagdollOwner;
            return Spawn(roleType, victimNick, new PlayerStats.HitInfo(@default.DeathCause.Amount, @default.DeathCause.Attacker, deathCause, -1, false), position, rotation, velocity, allowRecall, playerId, scp096Death, mirrorOwnerId);
        }

        /// <summary>
        /// Deletes the ragdoll.
        /// </summary>
        public void Delete()
        {
            Object.Destroy(GameObject);
            Map.RagdollsValue.Remove(this);
        }

        /// <summary>
        /// Spawns the ragdoll.
        /// </summary>
        public void Spawn() => Mirror.NetworkServer.Spawn(GameObject);
    }
}
