// -----------------------------------------------------------------------
// <copyright file="Map.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features.Lockers;

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Decals;
    using Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Toys;
    using Hazards;
    using InventorySystem;
    using InventorySystem.Items.Firearms.BasicMessages;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.ThrowableProjectiles;
    using Items;
    using LightContainmentZoneDecontamination;
    using MapGeneration;
    using Mirror;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp173;
    using PlayerRoles.PlayableScps.Scp939;
    using RelativePositioning;
    using UnityEngine;
    using Utils;
    using Utils.Networking;

    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;
    using Scp173GameRole = PlayerRoles.PlayableScps.Scp173.Scp173Role;
    using Scp939GameRole = PlayerRoles.PlayableScps.Scp939.Scp939Role;

    /// <summary>
    /// A set of tools to easily handle the in-game map.
    /// </summary>
    public static class Map
    {
        /// <summary>
        /// A list of <see cref="Locker"/>s on the map.
        /// </summary>
        internal static readonly List<Locker> LockersValue = new(250);

        /// <summary>
        /// A list of <see cref="PocketDimensionTeleport"/>s on the map.
        /// </summary>
        internal static readonly List<PocketDimensionTeleport> TeleportsValue = new(8);

        /// <summary>
        /// A list of <see cref="AdminToy"/>s on the map.
        /// </summary>
        internal static readonly List<AdminToy> ToysValue = new();

        private static readonly ReadOnlyCollection<PocketDimensionTeleport> ReadOnlyTeleportsValue = TeleportsValue.AsReadOnly();
        private static readonly ReadOnlyCollection<Locker> ReadOnlyLockersValue = LockersValue.AsReadOnly();
        private static readonly ReadOnlyCollection<AdminToy> ReadOnlyToysValue = ToysValue.AsReadOnly();

        private static TantrumEnvironmentalHazard tantrumPrefab;
        private static Scp939AmnesticCloudInstance amnesticCloudPrefab;

        /// <summary>
        /// Gets the tantrum prefab.
        /// </summary>
        public static TantrumEnvironmentalHazard TantrumPrefab
        {
            get
            {
                if (tantrumPrefab == null)
                {
                    Scp173GameRole scp173Role = RoleTypeId.Scp173.GetRoleBase() as Scp173GameRole;

                    if (scp173Role.SubroutineModule.TryGetSubroutine(out Scp173TantrumAbility scp173TantrumAbility))
                        tantrumPrefab = scp173TantrumAbility._tantrumPrefab;
                }

                return tantrumPrefab;
            }
        }

        /// <summary>
        /// Gets the amnestic cloud prefab.
        /// </summary>
        public static Scp939AmnesticCloudInstance AmnesticCloudPrefab
        {
            get
            {
                if (amnesticCloudPrefab == null)
                {
                    Scp939GameRole scp939Role = RoleTypeId.Scp939.GetRoleBase() as Scp939GameRole;

                    if (scp939Role.SubroutineModule.TryGetSubroutine(out Scp939AmnesticCloudAbility ability))
                        amnesticCloudPrefab = ability._instancePrefab;
                }

                return amnesticCloudPrefab;
            }
        }

        /// <summary>
        /// Gets a value indicating whether decontamination has begun in the light containment zone.
        /// </summary>
        public static bool IsLczDecontaminated => DecontaminationController.Singleton.IsDecontaminating;

        /// <summary>
        /// Gets all <see cref="PocketDimensionTeleport"/> objects.
        /// </summary>
        public static ReadOnlyCollection<PocketDimensionTeleport> PocketDimensionTeleports => ReadOnlyTeleportsValue;

        /// <summary>
        /// Gets all <see cref="Lockers.Locker"/> objects.
        /// </summary>
        public static ReadOnlyCollection<Lockers.Locker> Lockers => ReadOnlyLockersValue;

        /// <summary>
        /// Gets all <see cref="AdminToy"/> objects.
        /// </summary>
        public static ReadOnlyCollection<AdminToy> Toys => ReadOnlyToysValue;

        /// <summary>
        /// Gets or sets the current seed of the map.
        /// </summary>
        public static int Seed
        {
            get => SeedSynchronizer.Seed;
            set
            {
                if (!SeedSynchronizer.MapGenerated)
                    SeedSynchronizer._singleton.Network_syncSeed = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="global::AmbientSoundPlayer"/>.
        /// </summary>
        public static AmbientSoundPlayer AmbientSoundPlayer { get; internal set; }

        /// <summary>
        /// Tries to find the room that a <see cref="GameObject"/> is inside, first using the <see cref="Transform"/>'s parents, then using a Raycast if no room was found.
        /// </summary>
        /// <param name="objectInRoom">The <see cref="GameObject"/> inside the room.</param>
        /// <returns>The <see cref="Room"/> that the <see cref="GameObject"/> is located inside. Can be <see langword="null"/>.</returns>
        /// <seealso cref="Room.Get(Vector3)"/>
        [Obsolete("Use Room.FindParentRoom(GameObject) instead.")]
        public static Room FindParentRoom(GameObject objectInRoom) => Room.FindParentRoom(objectInRoom);

        /// <summary>
        /// Broadcasts a message to all <see cref="Player">players</see>.
        /// </summary>
        /// <param name="broadcast">The <see cref="Features.Broadcast"/> to be broadcasted.</param>
        /// <param name="shouldClearPrevious">Clears all players' broadcasts before sending the new one.</param>
        public static void Broadcast(Broadcast broadcast, bool shouldClearPrevious = false)
        {
            if (broadcast.Show)
                Broadcast(broadcast.Duration, broadcast.Content, broadcast.Type, shouldClearPrevious);
        }

        /// <summary>
        /// Broadcasts a message to all <see cref="Player">players</see>.
        /// </summary>
        /// <param name="duration">The duration in seconds.</param>
        /// <param name="message">The message that will be broadcast (supports Unity Rich Text formatting).</param>
        /// <param name="type">The broadcast type.</param>
        /// <param name="shouldClearPrevious">Clears all players' broadcasts before sending the new one.</param>
        public static void Broadcast(ushort duration, string message, global::Broadcast.BroadcastFlags type = global::Broadcast.BroadcastFlags.Normal, bool shouldClearPrevious = false)
        {
            if (shouldClearPrevious)
                ClearBroadcasts();

            Server.Broadcast.RpcAddElement(message, duration, type);
        }

        /// <summary>
        /// Shows a hint to all <see cref="Player">players</see>.
        /// </summary>
        /// <param name="message">The message that will be broadcasted (supports Unity Rich Text formatting).</param>
        /// <param name="duration">The duration in seconds.</param>
        public static void ShowHint(string message, float duration = 3f)
        {
            foreach (Player player in Player.List)
                player.ShowHint(message, duration);
        }

        /// <summary>
        /// Clears all <see cref="Player">players</see>' broadcasts.
        /// </summary>
        public static void ClearBroadcasts() => Server.Broadcast.RpcClearElements();

        /// <summary>
        /// Starts the light containment zone decontamination process.
        /// </summary>
        public static void StartDecontamination() => DecontaminationController.Singleton.ForceDecontamination();

        /// <summary>
        /// Turns off all lights in the facility.
        /// </summary>
        /// <param name="duration">The duration of the blackout.</param>
        /// <param name="zoneTypes">The <see cref="ZoneType"/>s to affect.</param>
        public static void TurnOffAllLights(float duration, ZoneType zoneTypes = ZoneType.Unspecified)
        {
            foreach (RoomLightController controller in RoomLightController.Instances)
            {
                Room room = controller.GetComponentInParent<Room>();
                if (room is null)
                    continue;

                if (zoneTypes == ZoneType.Unspecified || (room is not null && (zoneTypes == room.Zone)))
                    controller.ServerFlickerLights(duration);
            }
        }

        /// <summary>
        /// Turns off all lights in the facility.
        /// </summary>
        /// <param name="duration">The duration of the blackout.</param>
        /// <param name="zoneTypes">The <see cref="ZoneType"/>s to affect.</param>
        public static void TurnOffAllLights(float duration, IEnumerable<ZoneType> zoneTypes)
        {
            foreach (ZoneType zone in zoneTypes)
                TurnOffAllLights(duration, zone);
        }

        /// <summary>
        /// Gets a random <see cref="Locker"/>.
        /// </summary>
        /// <returns><see cref="Locker"/> object.</returns>
        public static Locker GetRandomLocker() => Lockers[Random.Range(0, Lockers.Count)];

        /// <summary>
        /// Gets a random <see cref="Pickup"/>.
        /// </summary>
        /// <param name="type">Filters by <see cref="ItemType"/>.</param>
        /// <returns><see cref="Pickup"/> object.</returns>
        public static Pickup GetRandomPickup(ItemType type = ItemType.None)
        {
            List<Pickup> pickups = (type != ItemType.None ? Pickup.List.Where(p => p.Type == type) : Pickup.List).ToList();
            return pickups[Random.Range(0, pickups.Count)];
        }

        /// <summary>
        /// Plays a random ambient sound.
        /// </summary>
        public static void PlayAmbientSound() => AmbientSoundPlayer.GenerateRandom();

        /// <summary>
        /// Plays an ambient sound.
        /// </summary>
        /// <param name="id">The id of the sound to play.</param>
        public static void PlayAmbientSound(int id)
        {
            if (id >= AmbientSoundPlayer.clips.Length)
                throw new IndexOutOfRangeException($"There are only {AmbientSoundPlayer.clips.Length} sounds available.");

            AmbientSoundPlayer.RpcPlaySound(AmbientSoundPlayer.clips[id].index);
        }

        /// <summary>
        /// Places a Tantrum (SCP-173's ability) in the indicated position.
        /// </summary>
        /// <param name="position">The position where you want to spawn the Tantrum.</param>
        /// <param name="isActive">Whether or not the tantrum will apply the <see cref="EffectType.Stained"/> effect.</param>
        /// <remarks>If <paramref name="isActive"/> is <see langword="true"/>, the tantrum is moved slightly up from its original position. Otherwise, the collision will not be detected and the slowness will not work.</remarks>
        /// <returns>The tantrum's <see cref="GameObject"/>.</returns>
        public static GameObject PlaceTantrum(Vector3 position, bool isActive = true)
        {
            TantrumEnvironmentalHazard tantrum = Object.Instantiate(TantrumPrefab);

            if (!isActive)
                tantrum.SynchronizedPosition = new RelativePosition(position);
            else
                tantrum.SynchronizedPosition = new RelativePosition(position + (Vector3.up * 0.25f));

            tantrum._destroyed = !isActive;

            NetworkServer.Spawn(tantrum.gameObject);

            return tantrum.gameObject;
        }

        /// <summary>
        /// Destroy all <see cref="ItemPickupBase"/> objects.
        /// </summary>
        public static void CleanAllItems()
        {
            foreach (Pickup pickup in Pickup.List.ToList())
                pickup.Destroy();
        }

        /// <summary>
        /// Destroy all the <see cref="Pickup"/> objects from the specified list.
        /// </summary>
        /// <param name="pickups">The List of pickups to destroy.</param>
        public static void CleanAllItems(IEnumerable<Pickup> pickups)
        {
            foreach (Pickup pickup in pickups)
                pickup.Destroy();
        }

        /// <summary>
        /// Destroy all <see cref="BasicRagdoll"/> objects.
        /// </summary>
        public static void CleanAllRagdolls()
        {
            foreach (Ragdoll ragDoll in Ragdoll.List.ToList())
                ragDoll.Destroy();
        }

        /// <summary>
        /// Destroy all <see cref="Ragdoll"/> objects from the specified list.
        /// </summary>
        /// <param name="ragDolls">The List of RagDolls to destroy.</param>
        public static void CleanAllRagdolls(IEnumerable<Ragdoll> ragDolls)
        {
            foreach (Ragdoll ragDoll in ragDolls)
                ragDoll.Destroy();
        }

        /// <summary>
        /// Places a blood decal.
        /// </summary>
        /// <param name="position">The position of the blood decal.</param>
        /// <param name="direction">The direction of the blood decal.</param>
        public static void PlaceBlood(Vector3 position, Vector3 direction) => new GunDecalMessage(position, direction, DecalPoolType.Blood).SendToAuthenticated(0);

        /// <summary>
        /// Gets all the near cameras.
        /// </summary>
        /// <param name="position">The position from which starting to search cameras.</param>
        /// <param name="toleration">The maximum toleration to define the radius from which get the cameras.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Camera"/> which contains all the found cameras.</returns>
        public static IEnumerable<Camera> GetNearCameras(Vector3 position, float toleration = 15f)
            => Camera.Get(cam => (position - cam.Position).sqrMagnitude <= toleration * toleration);

        /// <summary>
        /// Explode.
        /// </summary>
        /// <param name="position">The position where explosion will be created.</param>
        /// <param name="projectileType">The projectile that will create the explosion.</param>
        /// <param name="attacker">The player who create the explosion.</param>
        public static void Explode(Vector3 position, ProjectileType projectileType, Player attacker = null)
        {
            ItemType item;
            if ((item = projectileType.GetItemType()) is ItemType.None)
                return;
            attacker ??= Server.Host;
            if (!InventoryItemLoader.TryGetItem(item, out ThrowableItem throwableItem))
                return;
            ExplosionUtils.ServerSpawnEffect(position, item);

            if (throwableItem.Projectile is ExplosionGrenade explosionGrenade)
                ExplosionGrenade.Explode(attacker.Footprint, position, explosionGrenade);
        }

        /// <summary>
        /// Spawn projectile effect.
        /// </summary>
        /// <param name="position">The position where effect will be created.</param>
        /// <param name="projectileType">The projectile that will create the effect.</param>
        public static void ExplodeEffect(Vector3 position, ProjectileType projectileType)
        {
            ItemType item;
            if ((item = projectileType.GetItemType()) is ItemType.None)
                return;
            ExplosionUtils.ServerSpawnEffect(position, item);
        }

        /// <summary>
        /// Clears the lazy loading game object cache.
        /// </summary>
        internal static void ClearCache()
        {
            Room.RoomIdentifierToRoom.Clear();
            Door.DoorVariantToDoor.Clear();
            Lift.ElevatorChamberToLift.Clear();
            Camera.Camera079ToCamera.Clear();
            Window.BreakableWindowToWindow.Clear();
            TeslaGate.BaseTeslaGateToTeslaGate.Clear();
            Pickup.BaseToPickup.Clear();
            Item.BaseToItem.Clear();
            TeleportsValue.Clear();
            LockersValue.Clear();
            Ragdoll.BasicRagdollToRagdoll.Clear();
            Firearm.ItemTypeToFirearmInstance.Clear();
            Firearm.BaseCodesValue.Clear();
            Firearm.AvailableAttachmentsValue.Clear();
            Warhead.InternalBlastDoors.Clear();
            Locker.BaseToExiledLockers.Clear();
            Chamber.Chambers.Clear();
        }
    }
}
