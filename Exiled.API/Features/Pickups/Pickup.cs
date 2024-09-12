// -----------------------------------------------------------------------
// <copyright file="Pickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Exiled.API.Features.Pickups
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Extensions;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Attributes;
    using Exiled.API.Features.Pickups.Projectiles;
    using Exiled.API.Interfaces;
    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.ThrowableProjectiles;
    using InventorySystem.Items.Usables.Scp244;
    using Mirror;
    using RelativePositioning;
    using UnityEngine;

    using BaseAmmoPickup = InventorySystem.Items.Firearms.Ammo.AmmoPickup;
    using BaseBodyArmorPickup = InventorySystem.Items.Armor.BodyArmorPickup;
    using BaseFirearmPickup = InventorySystem.Items.Firearms.FirearmPickup;
    using BaseJailbirdPickup = InventorySystem.Items.Jailbird.JailbirdPickup;
    using BaseKeycardPickup = InventorySystem.Items.Keycards.KeycardPickup;
    using BaseMicroHIDPickup = InventorySystem.Items.MicroHID.MicroHIDPickup;
    using BaseRadioPickup = InventorySystem.Items.Radio.RadioPickup;
    using BaseScp018Projectile = InventorySystem.Items.ThrowableProjectiles.Scp018Projectile;
    using BaseScp1576Pickup = InventorySystem.Items.Usables.Scp1576.Scp1576Pickup;
    using BaseScp2176Projectile = InventorySystem.Items.ThrowableProjectiles.Scp2176Projectile;
    using BaseScp330Pickup = InventorySystem.Items.Usables.Scp330.Scp330Pickup;

    /// <summary>
    /// A wrapper class for <see cref="ItemPickupBase"/>.
    /// </summary>
    [EClass(category: nameof(Pickup))]
    public class Pickup : GameEntity, IWrapper<ItemPickupBase>
    {
        /// <summary>
        /// A dictionary of all <see cref="ItemBase"/>'s that have been converted into <see cref="Items.Item"/>.
        /// </summary>
        internal static readonly Dictionary<ItemPickupBase, Pickup> BaseToPickup = new(new ComponentsEqualityComparer());

        private readonly ConstProperty<double> minPickupTime = new(0.245d, new[] { typeof(ItemPickupBase) });
        private readonly ConstProperty<double> weightToTime = new(0.175d, new[] { typeof(ItemPickupBase) });

        /// <summary>
        /// Initializes a new instance of the <see cref="Pickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="ItemPickupBase"/> class.</param>
        internal Pickup(ItemPickupBase pickupBase)
            : base(pickupBase.gameObject)
        {
            Base = pickupBase;

            // prevent prefabs like `InventoryItemLoader.AvailableItems[ItemType.GrenadeHE].PickupDropModel` from adding to pickup list
            if (pickupBase.Info.ItemId is ItemType.None)
                return;

            BaseToPickup.Add(pickupBase, this);

            InitializeProperties(InventoryItemLoader.AvailableItems[pickupBase.Info.ItemId]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pickup"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        internal Pickup(ItemType type)
            : base(type.GetItemBase().gameObject)
        {
            ItemBase itemBase = type.GetItemBase();

            Base = Object.Instantiate(itemBase.PickupDropModel);
            GameObject = Base.gameObject;

            PickupSyncInfo psi = new()
            {
                ItemId = type,
                Serial = ItemSerialGenerator.GenerateNext(),
                WeightKg = itemBase.Weight,
            };

            Info = psi;

            BaseToPickup.Add(Base, this);

            InitializeProperties(itemBase);
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Pickup"/> which contains all the <see cref="Pickup"/> instances.
        /// </summary>
        public static new IEnumerable<Pickup> List => BaseToPickup.Values;

        /// <summary>
        /// Gets a randomly selected <see cref="Pickup"/>.
        /// </summary>
        /// <returns>A randomly selected <see cref="Pickup"/> object.</returns>
        public static Pickup Random => BaseToPickup.Random().Value;

        /// <summary>
        /// Gets the <see cref="UnityEngine.Rigidbody"/> of the Pickup.
        /// </summary>
        public Rigidbody Rigidbody => PhysicsModule?.Rb;

        /// <summary>
        /// Gets the current <see cref="Room"/> the Pickup is in.
        /// </summary>
        public Room Room => Room.FindParentRoom(GameObject);

        /// <summary>
        /// Gets or sets the pickup's PhysicsModule.
        /// </summary>
        public PickupStandardPhysics PhysicsModule
        {
            get => Base.PhysicsModule as PickupStandardPhysics;
            set
            {
                Base.PhysicsModule.DestroyModule();
                Base.PhysicsModule = value;
            }
        }

        /// <summary>
        /// Gets or sets the unique serial number for the item.
        /// </summary>
        [EProperty(category: nameof(Pickup))]
        public ushort Serial
        {
            get => Base.Info.Serial;
            set
            {
                Base.Info.Serial = value;
                Info = Base.Info;
            }
        }

        /// <summary>
        /// Gets or sets the pickup's scale value.
        /// </summary>
        [EProperty(category: nameof(Pickup))]
        public Vector3 Scale
        {
            get => GameObject.transform.localScale;
            set
            {
                if (!IsSpawned)
                {
                    GameObject.transform.localScale = value;
                    return;
                }

                UnSpawn();
                GameObject.transform.localScale = value;
                Spawn();
            }
        }

        /// <summary>
        /// Gets or sets the weight of the item.
        /// </summary>
        /// <seealso cref="PickupTime"/>
        [EProperty(category: nameof(Pickup))]
        public float Weight
        {
            get => Info.WeightKg;
            set
            {
                Base.Info.WeightKg = value;
                Info = Base.Info;
            }
        }

        /// <summary>
        /// Gets or sets the amount of time it takes to pick up this item, based on <see cref="Weight"/>.
        /// </summary>
        /// <remarks>Notes: Changing this value will change the item's <see cref="Weight"/>. This does not account for status effects such as <see cref="Enums.EffectType.Hypothermia"/>; see <see cref="PickupTimeForPlayer(Player)"/> to account for status effects.</remarks>
        /// <seealso cref="Weight"/>
        /// <seealso cref="PickupTimeForPlayer(Player)"/>
        [EProperty(category: nameof(Pickup))]
        public float PickupTime
        {
            get => ItemPickupBase.MinimalPickupTime + (ItemPickupBase.WeightToTime * Weight);
            set => Weight = ItemPickupBase.MinimalPickupTime - (ItemPickupBase.WeightToTime / value);
        }

        /// <summary>
        /// Gets or sets the <see cref="ItemBase"/> of the item.
        /// </summary>
        public ItemPickupBase Base { get; protected set; }

        /// <summary>
        /// Gets the <see cref="ItemType"/> of the item.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Pickup))]
        public ItemType Type => Base.NetworkInfo.ItemId;

        /// <summary>
        /// Gets the <see cref="ItemCategory"/> of the item.
        /// </summary>
        public ItemCategory Category => Type.GetCategory();

        /// <summary>
        /// Gets or sets a value indicating whether the pickup is locked (can't be picked up).
        /// </summary>
        [EProperty(category: nameof(Pickup))]
        public bool IsLocked
        {
            get => Info.Locked;
            set
            {
                Base.Info.Locked = value;
                Info = Base.Info;
            }
        }

        /// <summary>
        /// Gets or sets the pickup information.
        /// </summary>
        [EProperty(category: nameof(Pickup))]
        public PickupSyncInfo Info
        {
            get => Base.NetworkInfo;
            set => Base.NetworkInfo = value;
        }

        /// <summary>
        /// Gets or sets the previous owner of this item.
        /// </summary>
        /// <seealso cref="CreateAndSpawn(ItemType, Vector3, Quaternion?, Player)"/>
        public Player PreviousOwner
        {
            get => Player.Get(Base.PreviousOwner.Hub);
            set => Base.PreviousOwner = value is null ? Server.Host.Footprint : value.Footprint;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pickup is currently in use.
        /// </summary>
        [EProperty(category: nameof(Pickup))]
        public bool InUse
        {
            get => Info.InUse;
            set
            {
                Base.Info.InUse = value;
                Info = Base.Info;
            }
        }

        /// <summary>
        /// Gets or sets the pickup position.
        /// </summary>
        /// <seealso cref="CreateAndSpawn(ItemType, Vector3, Quaternion?, Player)"/>
        [EProperty(category: nameof(Pickup))]
        public override Vector3 Position
        {
            get => Base.Position;
            set => Base.Position = value;
        }

        /// <summary>
        /// Gets or sets the relative position of the pickup.
        /// </summary>
        [EProperty(category: nameof(Pickup))]
        public RelativePosition RelativePosition
        {
            get => new(Room.Transform.TransformPoint(Position));
            set => Position = value.Position;
        }

        /// <summary>
        /// Gets or sets the pickup rotation.
        /// </summary>
        /// <seealso cref="CreateAndSpawn(ItemType, Vector3, Quaternion?, Player)"/>
        [EProperty(category: nameof(Pickup))]
        public override Quaternion Rotation
        {
            get => Base.Rotation;
            set => Base.Rotation = value;
        }

        /// <summary>
        /// Gets a value indicating whether this pickup is spawned.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Pickup))]
        public bool IsSpawned { get; internal set; }

        /// <summary>
        /// Gets a <see cref="API.Features.Lift"/> in which pickup is now. Can be <see langword="null"/>.
        /// </summary>
        public Lift Lift => Lift.Get(Position);

        /// <summary>
        /// Gets or sets a multiplier to convert weight to time.
        /// </summary>
        public double WeightToTime
        {
            get => weightToTime;
            set => weightToTime.Value = value;
        }

        /// <summary>
        /// Gets or sets a time to pick up item.
        /// </summary>
        /// <remarks><see cref="MinTimeToPick"/> + <see cref="WeightToTime"/> * <see cref="Weight"/> = Picking up time.</remarks>
        public double MinTimeToPick
        {
            get => minPickupTime;
            set => minPickupTime.Value = value;
        }

        /// <summary>
        /// Gets an existing <see cref="Pickup"/> or creates a new instance of one.
        /// </summary>
        /// <param name="pickupBase">The <see cref="ItemPickupBase"/> to convert into a <see cref="Pickup"/>.</param>
        /// <returns>The <see cref="Pickup"/> wrapper for the given <see cref="ItemPickupBase"/>.</returns>
        public static Pickup Get(ItemPickupBase pickupBase)
        {
            if (pickupBase == null)
                return null;

            if (BaseToPickup.TryGetValue(pickupBase, out Pickup pickup))
                return pickup;

            return pickupBase switch
            {
                Scp244DeployablePickup scp244 => new Scp244Pickup(scp244),
                BaseAmmoPickup ammoPickup => new AmmoPickup(ammoPickup),
                BaseRadioPickup radioPickup => new RadioPickup(radioPickup),
                BaseMicroHIDPickup microHidPickup => new MicroHIDPickup(microHidPickup),
                TimedGrenadePickup timeGrenade => timeGrenade.NetworkInfo.ItemId switch
                {
                    ItemType.GrenadeHE => new ExplosiveGrenadePickup(timeGrenade),
                    ItemType.GrenadeFlash => new FlashGrenadePickup(timeGrenade),
                    _ => new GrenadePickup(timeGrenade),
                },
                BaseFirearmPickup firearmPickup => new FirearmPickup(firearmPickup),
                BaseKeycardPickup keycardPickup => new KeycardPickup(keycardPickup),
                BaseBodyArmorPickup bodyArmorPickup => new BodyArmorPickup(bodyArmorPickup),
                BaseScp330Pickup scp330Pickup => new Scp330Pickup(scp330Pickup),
                BaseScp1576Pickup scp1576Pickup => new Scp1576Pickup(scp1576Pickup),
                BaseJailbirdPickup jailbirdPickup => new JailbirdPickup(jailbirdPickup),
                ThrownProjectile thrownProjectile => thrownProjectile switch
                {
                    BaseScp018Projectile scp018 => new Projectiles.Scp018Projectile(scp018),
                    ExplosionGrenade explosionGrenade => new ExplosionGrenadeProjectile(explosionGrenade),
                    FlashbangGrenade flashGrenade => new FlashbangProjectile(flashGrenade),
                    BaseScp2176Projectile scp2176 => new Projectiles.Scp2176Projectile(scp2176),
                    EffectGrenade effectGrenade => new EffectGrenadeProjectile(effectGrenade),
                    TimeGrenade timeGrenade => new TimeGrenadeProjectile(timeGrenade),
                    _ => new Projectile(thrownProjectile),
                },
                _ => new Pickup(pickupBase),
            };
        }

        /// <summary>
        /// Gets the <see cref="Pickup"/> given a <see cref="Serial"/>.
        /// </summary>
        /// <param name="serial">The serial to look for.</param>
        /// <returns>The <see cref="Pickup"/> given the specified serial.</returns>
        public static Pickup Get(ushort serial) => List.SingleOrDefault(x => x.Serial == serial);

        /// <summary>
        /// Gets the <see cref="Pickup"/> given a <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        /// <param name="gameObject">The <see cref="UnityEngine.GameObject"/> to check.</param>
        /// <returns>The <see cref="Pickup"/> given the specified <see cref="UnityEngine.GameObject"/>.</returns>
        public static Pickup Get(GameObject gameObject) => !gameObject || !gameObject.TryGetComponent(out ItemPickupBase ipb) ? null : Get(ipb);

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> of <see cref="Pickup"/> containing all existing <see cref="ItemPickupBase"/> instances given an <see cref="IEnumerable{T}"/> of <see cref="ItemPickupBase"/>.
        /// </summary>
        /// <param name="pickups">An <see cref="IEnumerable{T}"/> of <see cref="ItemPickupBase"/> to convert into an <see cref="IEnumerable{T}"/> of <see cref="Pickup"/>.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Pickup"/> containing all existing <see cref="ItemPickupBase"/> instances.</returns>
        public static IEnumerable<Pickup> Get(IEnumerable<ItemPickupBase> pickups) => pickups.Select(ipb => Get(ipb));

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> of <see cref="Pickup"/> containing all existing <see cref="ItemPickupBase"/> instances given an <see cref="ItemType"/>.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> to look for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Pickup"/> containing all existing <see cref="ItemPickupBase"/> instances.</returns>
        /// <seealso cref="Map.GetRandomPickup(ItemType)"/>
        public static IEnumerable<Pickup> Get(ItemType type) => List.Where(x => x.Type == type);

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> of <see cref="Pickup"/> containing all existing <see cref="ItemPickupBase"/> instances given an <see cref="IEnumerable{T}"/> of <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        /// <param name="gameObjects">The <see cref="UnityEngine.GameObject"/>'s to check.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Pickup"/> containing all existing <see cref="ItemPickupBase"/> instances.</returns>
        public static IEnumerable<Pickup> Get(IEnumerable<GameObject> gameObjects)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                Pickup pickup = Get(gameObject);
                if (pickup is null)
                    continue;

                yield return pickup;
            }
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> of <see cref="Pickup"/> containing all existing <see cref="ItemPickupBase"/> instances given a <see cref="IEnumerable{T}"/> of <see cref="ItemPickupBase"/>.
        /// </summary>
        /// <typeparam name="T">The type <typeparamref name="T"/> to cast the pickups to.</typeparam>
        /// <param name="pickups">An <see cref="IEnumerable{T}"/> of <see cref="ItemPickupBase"/> to convert into an <see cref="IEnumerable{T}"/> of <see cref="Pickup"/>.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Pickup"/> containing all existing <see cref="ItemPickupBase"/> instances.</returns>
        public static IEnumerable<T> Get<T>(IEnumerable<ItemPickupBase> pickups)
            where T : Pickup
        {
            foreach (ItemPickupBase ipb in pickups)
            {
                Pickup pickup = Get(ipb);
                if (pickup is null || !pickup.Cast(out T param))
                    continue;

                yield return param;
            }
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> containing all existing <see cref="ItemPickupBase"/> instances given an <see cref="ItemType"/>.
        /// </summary>
        /// <typeparam name="T">The type <typeparamref name="T"/> to cast the pickups to.</typeparam>
        /// <param name="type">The <see cref="ItemType"/> to look for.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Pickup"/>.</returns>
        /// <seealso cref="Map.GetRandomPickup(ItemType)"/>
        public static IEnumerable<T> Get<T>(ItemType type)
            where T : Pickup
        {
            foreach (Pickup pickup in List)
            {
                if (pickup.Type != type || !pickup.Cast(out T param))
                    continue;

                yield return param;
            }
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> containing all existing <see cref="ItemPickupBase"/> instances given a <see cref="IEnumerable{T}"/> of <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        /// <typeparam name="T">The type <typeparamref name="T"/> to cast the pickups to.</typeparam>
        /// <param name="gameObjects">The <see cref="UnityEngine.GameObject"/>'s to check.</param>
        /// <returns>The <see cref="Pickup"/> given the specified <see cref="UnityEngine.GameObject"/>.</returns>
        public static IEnumerable<T> Get<T>(IEnumerable<GameObject> gameObjects)
            where T : Pickup
        {
            foreach (GameObject gameObject in gameObjects)
            {
                Pickup pickup = Get(gameObject);
                if (pickup is null || !pickup.Cast(out T param))
                    continue;

                yield return param;
            }
        }

        /// <summary>
        /// Creates and returns a new <see cref="Pickup"/> with the proper inherited subclass.
        /// <para>
        /// Based on the <paramref name="type"/>, the returned <see cref="Pickup"/> can be cast into a subclass to gain more control over the object.
        /// <br />- All valid ammo should be cast to the <see cref="AmmoPickup"/> class.
        /// <br />- All valid firearms (not including the Micro HID) should be cast to the <see cref="FirearmPickup"/> class.
        /// <br />- All valid keycards should be cast to the <see cref="KeycardPickup"/> class.
        /// <br />- All valid armor should be cast to the <see cref="BodyArmorPickup"/> class.
        /// <br />- All grenades and throwables (not including SCP-018 and SCP-2176) should be cast to the <see cref="GrenadePickup"/> class.
        /// </para>
        /// <para>
        /// <br />The following have their own respective classes:
        /// <br />- Radios can be cast to <see cref="RadioPickup"/>.
        /// <br />- The Micro HID can be cast to <see cref="MicroHIDPickup"/>.
        /// <br />- SCP-244 A and B variants can be cast to <see cref="Scp244Pickup"/>.
        /// <br />- SCP-330 can be cast to <see cref="Scp330Pickup"/>.
        /// <br />- SCP-018 can be cast to <see cref="Projectiles.Scp018Projectile"/>.
        /// <br />- SCP-2176 can be cast to <see cref="Projectiles.Scp2176Projectile"/>.
        /// </para>
        /// <para>
        /// Items that are not listed above do not have a subclass, and can only use the base <see cref="Pickup"/> class.
        /// </para>
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        /// <returns>The created <see cref="Pickup"/>.</returns>
        /// <seealso cref="Projectile.Create(Enums.ProjectileType)"/>
        public static Pickup Create(ItemType type) => type switch
        {
            ItemType.SCP244a or ItemType.SCP244b => new Scp244Pickup(type),
            ItemType.Ammo9x19 or ItemType.Ammo12gauge or ItemType.Ammo44cal or ItemType.Ammo556x45 or ItemType.Ammo762x39 => new AmmoPickup(type),
            ItemType.Radio => new RadioPickup(),
            ItemType.MicroHID => new MicroHIDPickup(),
            ItemType.GrenadeFlash => new FlashGrenadePickup(),
            ItemType.GrenadeHE => new ExplosiveGrenadePickup(),
            ItemType.GunCrossvec or ItemType.GunLogicer or ItemType.GunRevolver or ItemType.GunShotgun or ItemType.GunAK or ItemType.GunCOM15 or ItemType.GunCOM18 or ItemType.GunE11SR or ItemType.GunFSP9 or ItemType.ParticleDisruptor or ItemType.GunA7 or ItemType.GunFRMG0 => new FirearmPickup(type),
            ItemType.KeycardGuard or ItemType.KeycardJanitor or ItemType.KeycardO5 or ItemType.KeycardScientist or ItemType.KeycardContainmentEngineer or ItemType.KeycardFacilityManager or ItemType.KeycardResearchCoordinator or ItemType.KeycardZoneManager or ItemType.KeycardMTFCaptain or ItemType.KeycardMTFOperative or ItemType.KeycardMTFPrivate => new KeycardPickup(type),
            ItemType.ArmorLight or ItemType.ArmorCombat or ItemType.ArmorHeavy => new BodyArmorPickup(type),
            ItemType.SCP330 => new Scp330Pickup(),
            ItemType.SCP500 or ItemType.SCP268 or ItemType.SCP207 or ItemType.SCP1853 or ItemType.Painkillers or ItemType.Medkit or ItemType.Adrenaline or ItemType.AntiSCP207 => new UsablePickup(type),
            ItemType.Jailbird => new JailbirdPickup(),
            ItemType.SCP1576 => new Scp1576Pickup(),
            ItemType.SCP2176 => new Projectiles.Scp2176Projectile(),
            ItemType.SCP018 => new Projectiles.Scp018Projectile(),
            _ => new Pickup(type),
        };

        /// <summary>
        /// Creates and spawns a <see cref="Pickup"/>.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        /// <param name="position">The position to spawn the <see cref="Pickup"/> at.</param>
        /// <param name="rotation">The rotation to spawn the <see cref="Pickup"/>.</param>
        /// <param name="previousOwner">An optional previous owner of the item.</param>
        /// <returns>The <see cref="Pickup"/>. See documentation of <see cref="Create(ItemType)"/> for more information on casting.</returns>
        /// <seealso cref="Projectile.CreateAndSpawn(Enums.ProjectileType, Vector3, Quaternion?, bool, Player)"/>
        public static Pickup CreateAndSpawn(ItemType type, Vector3 position, Quaternion? rotation = null, Player previousOwner = null) => Create(type).Spawn(position, rotation, previousOwner);

        /// <summary>
        /// Returns the amount of time it will take for the provided <paramref name="player"/> to pick up this item, based on <see cref="Weight"/> and active status effects.
        /// </summary>
        /// <param name="player">The player to check search time.</param>
        /// <exception cref="System.ArgumentNullException">player cannot be null.</exception>
        /// <returns>The amount of time it will take for the provided <paramref name="player"/> to pick up this item.</returns>
        /// <seealso cref="PickupTime"/>
        public float PickupTimeForPlayer(Player player)
        {
            if (player is null)
                throw new System.ArgumentNullException(nameof(player));

            return Base.SearchTimeForPlayer(player.ReferenceHub);
        }

        /// <summary>
        /// Spawns pickup on a server.
        /// </summary>
        /// <seealso cref="UnSpawn"/>
        public void Spawn()
        {
            // condition for projectiles
            if (!GameObject.activeSelf)
                GameObject.SetActive(true);

            if (!IsSpawned)
            {
                NetworkServer.Spawn(GameObject);
                IsSpawned = true;
            }
        }

        /// <summary>
        /// Spawns pickup on a server.
        /// </summary>
        /// <param name="position">The position to spawn the <see cref="Pickup"/> at.</param>
        /// <param name="rotation">The rotation to spawn the <see cref="Pickup"/>.</param>
        /// <param name="previousOwner">An optional previous owner of the item.</param>
        /// <returns>The spawned <see cref="Pickup"/>.</returns>
        /// <seealso cref="Projectile.Spawn(Vector3, Quaternion?, bool, Player)"/>
        public Pickup Spawn(Vector3 position, Quaternion? rotation = null, Player previousOwner = null)
        {
            Position = position;
            Rotation = rotation ?? Quaternion.identity;
            PreviousOwner = previousOwner;
            Spawn();

            return this;
        }

        /// <summary>
        /// Unspawns pickup on server.
        /// </summary>
        /// <seealso cref="Spawn()"/>
        /// <seealso cref="Destroy"/>
        public void UnSpawn()
        {
            if (IsSpawned)
            {
                IsSpawned = false;
                NetworkServer.UnSpawn(GameObject);
            }
        }

        /// <summary>
        /// Destroys the already spawned pickup.
        /// </summary>
        /// <seealso cref="UnSpawn"/>
        public void Destroy()
        {
            Base.DestroySelf();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Clones the current pickup with a different serial.
        /// </summary>
        /// <returns> Cloned pickup object. </returns>
        public virtual Pickup Clone() => new(Type)
        {
            Scale = Scale,
            PreviousOwner = PreviousOwner,
            Info = Info,
        };

        /// <summary>
        /// Returns the Pickup in a human readable format.
        /// </summary>
        /// <returns>A string containing Pickup-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Position}| -{IsLocked}- ={InUse}=";

        /// <summary>
        /// Helper method for saving data between items and pickups.
        /// </summary>
        /// <param name="item"> <see cref="Items.Item"/>-related data to give to the <see cref="Pickup"/>.</param>
        internal virtual void ReadItemInfo(Items.Item item)
        {
            if (item is not null)
                Scale = item.Scale;
        }

        /// <summary>
        /// initialize item properties.
        /// </summary>
        /// <param name="itemBase">target item.</param>
        protected virtual void InitializeProperties(ItemBase itemBase)
        {
        }
    }
}
