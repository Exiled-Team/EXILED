// -----------------------------------------------------------------------
// <copyright file="Pickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups.Projectiles;

    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.ThrowableProjectiles;
    using InventorySystem.Items.Usables.Scp244;

    using Mirror;

    using UnityEngine;

    using BaseAmmoPickup = InventorySystem.Items.Firearms.Ammo.AmmoPickup;
    using BaseBodyArmorPickup = InventorySystem.Items.Armor.BodyArmorPickup;
    using BaseFirearmPickup = InventorySystem.Items.Firearms.FirearmPickup;
    using BaseKeycardPickup = InventorySystem.Items.Keycards.KeycardPickup;
    using BaseMicroHIDPickup = InventorySystem.Items.MicroHID.MicroHIDPickup;
    using BaseRadioPickup = InventorySystem.Items.Radio.RadioPickup;
    using BaseScp018Projectile = InventorySystem.Items.ThrowableProjectiles.Scp018Projectile;
    using BaseScp2176Projectile = InventorySystem.Items.ThrowableProjectiles.Scp2176Projectile;
    using BaseScp330Pickup = InventorySystem.Items.Usables.Scp330.Scp330Pickup;

    /// <summary>
    /// A wrapper class for <see cref="ItemPickupBase"/>.
    /// </summary>
    public class Pickup
    {
        /// <summary>
        /// A dictionary of all <see cref="ItemBase"/>'s that have been converted into <see cref="Items.Item"/>.
        /// </summary>
        internal static readonly Dictionary<ItemPickupBase, Pickup> BaseToItem = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Pickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="ItemPickupBase"/> class.</param>
        internal Pickup(ItemPickupBase pickupBase)
        {
            Base = pickupBase;
            Serial = pickupBase.NetworkInfo.Serial == 0 ? ItemSerialGenerator.GenerateNext() : pickupBase.NetworkInfo.Serial;
            BaseToItem.Add(pickupBase, this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pickup"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        internal Pickup(ItemType type)
        {
            Item item = Item.Create(type);

            Base = Object.Instantiate(item.Base.PickupDropModel);

            PickupSyncInfo psi = new()
            {
                ItemId = type,
                Serial = item.Serial,
                Weight = item.Weight,
            };

            Serial = psi.Serial;
            Base.Info = psi;
            Base.NetworkInfo = Base.Info;
            BaseToItem.Add(Base, this);
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Pickup"/> which contains all the <see cref="Pickup"/> instances.
        /// </summary>
        public static IEnumerable<Pickup> List => BaseToItem.Values;

        /// <summary>
        /// Gets the <see cref="UnityEngine.GameObject"/> of the Pickup.
        /// </summary>
        public GameObject GameObject => Base.gameObject;

        /// <summary>
        /// Gets or sets the unique serial number for the item.
        /// </summary>
        public ushort Serial
        {
            get => Base.Info.Serial;
            set
            {
                Base.Info.Serial = value;
                Base.NetworkInfo = Base.Info;
            }
        }

        /// <summary>
        /// Gets or sets the pickup's scale value.
        /// </summary>
        public Vector3 Scale
        {
            get => GameObject.transform.localScale;
            set
            {
                UnSpawn();
                GameObject.transform.localScale = value;
                Spawn();
            }
        }

        /// <summary>
        /// Gets or sets the weight of the item.
        /// </summary>
        public float Weight
        {
            get => Base.NetworkInfo.Weight;
            set
            {
                Base.Info.Weight = value;
                Base.NetworkInfo = Base.Info;
            }
        }

        /// <summary>
        /// Gets the <see cref="ItemBase"/> of the item.
        /// </summary>
        public ItemPickupBase Base { get; }

        /// <summary>
        /// Gets the <see cref="ItemType"/> of the item.
        /// </summary>
        public ItemType Type => Base.NetworkInfo.ItemId;

        /// <summary>
        /// Gets or sets a value indicating whether the pickup is locked (can't be picked up).
        /// </summary>
        public bool Locked
        {
            get => Base.NetworkInfo.Locked;
            set
            {
                Base.Info.Locked = value;
                Base.NetworkInfo = Base.Info;
            }
        }

        /// <summary>
        /// Gets or sets the previous owner of this item.
        /// </summary>
        public Player PreviousOwner
        {
            get => Player.Get(Base.PreviousOwner.Hub);
            set => Base.PreviousOwner = value.Footprint;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pickup is currently in use.
        /// </summary>
        public bool InUse
        {
            get => Base.NetworkInfo.InUse;
            set
            {
                Base.Info.InUse = value;
                Base.NetworkInfo = Base.Info;
            }
        }

        /// <summary>
        /// Gets or sets the pickup position.
        /// </summary>
        public Vector3 Position
        {
            get => Base.NetworkInfo.Position;
            set
            {
                Base.Rb.position = value;

                Base.RefreshPositionAndRotation();
            }
        }

        /// <summary>
        /// Gets or sets the pickup rotation.
        /// </summary>
        public Quaternion Rotation
        {
            get => Base.NetworkInfo.Rotation.Value;
            set
            {
                Base.Rb.rotation = value;

                Base.RefreshPositionAndRotation();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this pickup is spawned.
        /// </summary>
        public bool Spawned { get; private set; }

        /// <summary>
        /// Gets an existing <see cref="Pickup"/> or creates a new instance of one.
        /// </summary>
        /// <param name="pickupBase">The <see cref="ItemPickupBase"/> to convert into a <see cref="Pickup"/>.</param>
        /// <returns>The <see cref="Pickup"/> wrapper for the given <see cref="ItemPickupBase"/>.</returns>
        public static Pickup Get(ItemPickupBase pickupBase)
        {
            if (pickupBase is null)
                return null;
            if (BaseToItem.TryGetValue(pickupBase, out Pickup pickup))
                return pickup;

            return pickupBase switch
            {
                Scp244DeployablePickup scp244 => new Scp244Pickup(scp244),
                BaseAmmoPickup ammoPickup => new AmmoPickup(ammoPickup),
                BaseRadioPickup radioPickup => new RadioPickup(radioPickup),
                BaseMicroHIDPickup microHidPickup => new MicroHIDPickup(microHidPickup),
                TimedGrenadePickup timeGrenade => new GrenadePickup(timeGrenade),
                BaseFirearmPickup firearmPickup => new FirearmPickup(firearmPickup),
                BaseKeycardPickup keycardPickup => new KeycardPickup(keycardPickup),
                BaseBodyArmorPickup bodyArmorPickup => new BodyArmorPickup(bodyArmorPickup),
                BaseScp330Pickup scp330Pickup => new Scp330Pickup(scp330Pickup),
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
        /// creates.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        /// <returns>pickup.</returns>
        public static Pickup Create(ItemType type) => type switch
        {
            ItemType.SCP244a or ItemType.SCP244b => new Scp244Pickup(type),
            ItemType.Ammo9x19 or ItemType.Ammo12gauge or ItemType.Ammo44cal or ItemType.Ammo556x45 or ItemType.Ammo762x39 => new AmmoPickup(type),
            ItemType.Radio => new RadioPickup(),
            ItemType.MicroHID => new MicroHIDPickup(),
            ItemType.GrenadeHE or ItemType.SCP018 or ItemType.GrenadeFlash or ItemType.SCP2176 => new GrenadePickup(type),
            ItemType.GunCrossvec or ItemType.GunLogicer or ItemType.GunRevolver or ItemType.GunShotgun or ItemType.GunAK or ItemType.GunCOM15 or ItemType.GunCOM18 or ItemType.GunE11SR or ItemType.GunFSP9 or ItemType.ParticleDisruptor => new FirearmPickup(type),
            ItemType.KeycardGuard or ItemType.KeycardJanitor or ItemType.KeycardO5 or ItemType.KeycardScientist or ItemType.KeycardContainmentEngineer or ItemType.KeycardFacilityManager or ItemType.KeycardResearchCoordinator or ItemType.KeycardZoneManager or ItemType.KeycardNTFCommander or ItemType.KeycardNTFLieutenant or ItemType.KeycardNTFOfficer => new KeycardPickup(type),
            ItemType.ArmorLight or ItemType.ArmorCombat or ItemType.ArmorHeavy => new BodyArmorPickup(type),
            ItemType.SCP330 => new Scp330Pickup(),
            _ => new Pickup(type),
        };

        /// <summary>
        /// Spawns pickup on server.
        /// </summary>
        public void Spawn()
        {
            if (!Spawned)
            {
                NetworkServer.Spawn(GameObject);
                Spawned = true;
            }
        }

        /// <summary>
        /// Unspawns pickup on server.
        /// </summary>
        public void UnSpawn()
        {
            if (Spawned)
            {
                Spawned = false;
                NetworkServer.UnSpawn(GameObject);
            }
        }

        /// <summary>
        /// Destroys the pickup.
        /// </summary>
        public void Destroy() => Base.DestroySelf();

        /// <summary>
        /// Returns the Pickup in a human readable format.
        /// </summary>
        /// <returns>A string containing Pickup-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Position}| -{Locked}- ={InUse}=";
    }
}
