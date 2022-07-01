// -----------------------------------------------------------------------
// <copyright file="Item.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Extensions;
    using Exiled.API.Structs;

    using InventorySystem.Items;
    using InventorySystem.Items.Armor;
    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Ammo;
    using InventorySystem.Items.Flashlight;
    using InventorySystem.Items.Keycards;
    using InventorySystem.Items.MicroHID;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.Radio;
    using InventorySystem.Items.ThrowableProjectiles;
    using InventorySystem.Items.Usables;
    using InventorySystem.Items.Usables.Scp244;
    using InventorySystem.Items.Usables.Scp330;

    using Mirror;

    using UnityEngine;

    using Object = UnityEngine.Object;

    /// <summary>
    /// A wrapper class for <see cref="ItemBase"/>.
    /// </summary>
    public class Item
    {
        /// <summary>
        /// A dictionary of all <see cref="ItemBase"/>'s that have been converted into <see cref="Item"/>.
        /// </summary>
        internal static readonly Dictionary<ItemBase, Item> BaseToItem = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        /// <param name="itemBase">The <see cref="ItemBase"/> to encapsulate.</param>
        public Item(ItemBase itemBase)
        {
            Base = itemBase;
            if (Serial == 0)
            {
                ushort serial = ItemSerialGenerator.GenerateNext();
                Serial = serial;
                Base.OnAdded(null);
#if DEBUG
                Log.Debug($"{nameof(Item)}.ctor: Generating new serial number. Serial should now be: {serial}. // {Serial}");
#endif
            }
#if DEBUG
            Log.Debug($"{nameof(Item)}.ctor: New item created with Serial: {Serial}");
#endif
            BaseToItem.Add(itemBase, this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the item to create.</param>
        internal Item(ItemType type)
            : this(Server.Host.Inventory.CreateItemInstance(type, false))
        {
        }

        /// <summary>
        /// Gets or sets the unique serial number for the item.
        /// </summary>
        public ushort Serial
        {
            get => Base.ItemSerial;

            set => Base.ItemSerial = value;
        }

        /// <summary>
        /// Gets or sets the scale for the item.
        /// </summary>
        public Vector3 Scale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets the <see cref="ItemBase"/> of the item.
        /// </summary>
        public ItemBase Base { get; }

        /// <summary>
        /// Gets the <see cref="ItemType"/> of the item.
        /// </summary>
        public ItemType Type => Base.ItemTypeId;

        /// <summary>
        /// Gets the <see cref="ItemCategory"/> of the item.
        /// </summary>
        public ItemCategory Category => Base.Category;

        /// <summary>
        /// Gets the Weight of the item.
        /// </summary>
        public float Weight => Base.Weight;

        /// <summary>
        /// Gets a value indicating whether or not this item is ammunition.
        /// </summary>
        public bool IsAmmo => Type.IsAmmo();

        /// <summary>
        /// Gets a value indicating whether or not this item is armor.
        /// </summary>
        public bool IsArmor => Type.IsArmor();

        /// <summary>
        /// Gets a value indicating whether or not this item is a keycard.
        /// </summary>
        public bool IsKeycard => Type.IsKeycard();

        /// <summary>
        /// Gets a value indicating whether or not this item is a medical item.
        /// </summary>
        public bool IsMedical => Type.IsMedical();

        /// <summary>
        /// Gets a value indicating whether or not this item is an SCP item.
        /// </summary>
        public bool IsScp => Type.IsScp();

        /// <summary>
        /// Gets a value indicating whether or not this item is a throwable item.
        /// </summary>
        public bool IsThrowable => Type.IsThrowable();

        /// <summary>
        /// Gets a value indicating whether or not this item is a utility item.
        /// </summary>
        public bool IsUtility => Type.IsUtility();

        /// <summary>
        /// Gets a value indicating whether or not this item is a weapon.
        /// </summary>
        public bool IsWeapon => Type.IsWeapon();

        /// <summary>
        /// Gets the <see cref="Player"/> who owns the item.
        /// </summary>
        public Player Owner => Player.Get(Base.Owner);

        /// <summary>
        /// Gets an existing <see cref="Item"/> or creates a new instance of one.
        /// </summary>
        /// <param name="itemBase">The <see cref="ItemBase"/> to convert into an item.</param>
        /// <returns>The item wrapper for the given <see cref="ItemBase"/>.</returns>
        public static Item Get(ItemBase itemBase)
        {
            if (itemBase is null)
                return null;

            if (BaseToItem.TryGetValue(itemBase, out Item item))
                return item;

            return itemBase switch
            {
                InventorySystem.Items.Firearms.Firearm firearm => new Firearm(firearm),
                KeycardItem keycard => new Keycard(keycard),
                UsableItem usable => usable switch
                {
                    Scp330Bag scp330Bag => new Scp330(scp330Bag),
                    Scp244Item scp244Item => new Scp244(scp244Item),
                    _ => new Usable(usable)
                },
                RadioItem radio => new Radio(radio),
                MicroHIDItem micro => new MicroHid(micro),
                BodyArmor armor => new Armor(armor),
                AmmoItem ammo => new Ammo(ammo),
                FlashlightItem flashlight => new Flashlight(flashlight),
                ThrowableItem throwable => throwable.Projectile switch
                {
                    FlashbangGrenade => new FlashGrenade(throwable),
                    ExplosionGrenade => new ExplosiveGrenade(throwable),
                    _ => new Throwable(throwable),
                },
                _ => new Item(itemBase)
            };
        }

        /// <summary>
        /// Creates a new <see cref="Item"/> with the proper inherited subclass.
        /// <para>
        /// Based on the <paramref name="type"/>, the returned <see cref="Item"/> can be casted into a subclass to gain more control over the object.
        /// <br />- Usable items (Adrenaline, Medkit, Painkillers, SCP-207, SCP-268, and SCP-500) should be casted to the <see cref="Usable"/> class.
        /// <br />- All valid ammo should be casted to the <see cref="Ammo"/> class.
        /// <br />- All valid firearms (not including the Micro HID) should be casted to the <see cref="Firearm"/> class.
        /// <br />- All valid keycards should be casted to the <see cref="Keycard"/> class.
        /// <br />- All valid armor should be casted to the <see cref="Armor"/> class.
        /// <br />- Explosive grenades and SCP-018 should be casted to the <see cref="ExplosiveGrenade"/> class.
        /// <br />- Flash grenades should be casted to the <see cref="FlashGrenade"/> class.
        /// <br />- SCP-2176 can be casted to the <see cref="Throwable"/> class.
        /// </para>
        /// <para>
        /// <br />The following have their own respective classes:
        /// <br />- Flashlights can be casted to <see cref="Flashlight"/>.
        /// <br />- Radios can be casted to <see cref="Radio"/>.
        /// <br />- The Micro HID can be casted to <see cref="MicroHid"/>.
        /// <br />- SCP-244 A and B variants can be casted to <see cref="Scp244"/>.
        /// <br />- SCP-330 can be casted to <see cref="Scp330"/>.
        /// </para>
        /// <para>
        /// Items that are not listed above do not have a subclass, and can only use the base <see cref="Item"/> class.
        /// </para>
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the item to create.</param>
        /// <param name="owner">The <see cref="Player"/> who owns the item by default.</param>
        /// <returns>The <see cref="Item"/> created. This can be cast as a subclass.</returns>
        public static Item Create(ItemType type, Player owner = null) => type switch
        {
            ItemType.Adrenaline or ItemType.Medkit or ItemType.Painkillers or ItemType.SCP500 or ItemType.SCP207 or ItemType.SCP268 => new Usable(type),
            ItemType.SCP244a or ItemType.SCP244b => new Scp244(type),
            ItemType.Ammo9x19 or ItemType.Ammo12gauge or ItemType.Ammo44cal or ItemType.Ammo556x45 or ItemType.Ammo762x39 => new Ammo(type),
            ItemType.Flashlight => new Flashlight(),
            ItemType.Radio => new Radio(),
            ItemType.MicroHID => new MicroHid(),
            ItemType.GrenadeFlash => new FlashGrenade(owner),
            ItemType.GrenadeHE or ItemType.SCP018 => new ExplosiveGrenade(type, owner),
            ItemType.GunCrossvec or ItemType.GunLogicer or ItemType.GunRevolver or ItemType.GunShotgun or ItemType.GunAK or ItemType.GunCOM15 or ItemType.GunCOM18 or ItemType.GunE11SR or ItemType.GunFSP9 or ItemType.ParticleDisruptor => new Firearm(type),
            ItemType.KeycardGuard or ItemType.KeycardJanitor or ItemType.KeycardO5 or ItemType.KeycardScientist or ItemType.KeycardChaosInsurgency or ItemType.KeycardContainmentEngineer or ItemType.KeycardFacilityManager or ItemType.KeycardResearchCoordinator or ItemType.KeycardZoneManager or ItemType.KeycardNTFCommander or ItemType.KeycardNTFLieutenant or ItemType.KeycardNTFOfficer => new Keycard(type),
            ItemType.ArmorLight or ItemType.ArmorCombat or ItemType.ArmorHeavy => new Armor(type),
            ItemType.SCP330 => new Scp330(),
            ItemType.SCP2176 => new Throwable(type),
            _ => new Item(type),
        };

        /// <summary>
        /// Gives this item to a <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to give the item to.</param>
        public void Give(Player player) => player.AddItem(Base);

        /// <summary>
        /// Spawns the item on the map.
        /// </summary>
        /// <param name="position">The location to spawn the item.</param>
        /// <param name="rotation">The rotation of the item.</param>
        /// <param name="identifiers">The attachments to be added.</param>
        /// <returns>The <see cref="Pickup"/> created by spawning this item.</returns>
        public virtual Pickup Spawn(Vector3 position, Quaternion rotation = default, IEnumerable<AttachmentIdentifier> identifiers = null)
        {
            PickupSyncInfo info = new()
            {
                ItemId = Type,
                Position = position,
                Weight = Weight,
                Rotation = new LowPrecisionQuaternion(rotation),
            };

            ItemPickupBase ipb = Object.Instantiate(Base.PickupDropModel, position, rotation);

            ipb.NetworkInfo = info;

            if (ipb is FirearmPickup firearmPickup)
            {
                if (this is Firearm firearm)
                {
                    if (identifiers is not null)
                        firearm.AddAttachment(identifiers);

                    firearmPickup.Status = new FirearmStatus(firearm.Ammo, FirearmStatusFlags.MagazineInserted, firearmPickup.Status.Attachments);
                }
                else
                {
                    byte ammo = Base switch
                    {
                        AutomaticFirearm auto => auto._baseMaxAmmo,
                        Shotgun shotgun => shotgun._ammoCapacity,
                        Revolver revolver => revolver.AmmoManagerModule.MaxAmmo,
                        _ => 0,
                    };
                    uint code = identifiers is not null ? (uint)firearmPickup.Info.ItemId.GetBaseCode() + identifiers.GetAttachmentsCode() : firearmPickup.Status.Attachments;
                    firearmPickup.Status = new FirearmStatus(ammo, FirearmStatusFlags.MagazineInserted, code);
                }

                firearmPickup.NetworkStatus = firearmPickup.Status;
            }

            NetworkServer.Spawn(ipb.gameObject);
            ipb.InfoReceived(default, Base.PickupDropModel.NetworkInfo);
            Pickup pickup = Pickup.Get(ipb);
            pickup.PreviousOwner = Owner;
            pickup.Scale = Scale;
            return pickup;
        }

        /// <summary>
        /// Spawns the item on the map.
        /// </summary>
        /// <param name="position">The location to spawn the item.</param>
        /// <param name="rotation">The rotation of the item.</param>
        /// <returns>The <see cref="Pickup"/> created by spawning this item.</returns>
        public virtual Pickup Spawn(Vector3 position, Quaternion rotation = default)
        {
            Base.PickupDropModel.Info.ItemId = Type;
            Base.PickupDropModel.Info.Position = position;
            Base.PickupDropModel.Info.Weight = Weight;
            Base.PickupDropModel.Info.Rotation = new LowPrecisionQuaternion(rotation);
            Base.PickupDropModel.NetworkInfo = Base.PickupDropModel.Info;

            ItemPickupBase ipb = Object.Instantiate(Base.PickupDropModel, position, rotation);
            if (ipb is FirearmPickup firearmPickup)
            {
                if (this is Firearm firearm)
                {
                    firearmPickup.Status = new FirearmStatus(firearm.Ammo, FirearmStatusFlags.MagazineInserted, firearmPickup.Status.Attachments);
                }
                else
                {
                    byte ammo = Base switch
                    {
                        AutomaticFirearm auto => auto._baseMaxAmmo,
                        Shotgun shotgun => shotgun._ammoCapacity,
                        Revolver => 6,
                        _ => 0,
                    };
                    firearmPickup.Status = new FirearmStatus(ammo, FirearmStatusFlags.MagazineInserted, firearmPickup.Status.Attachments);
                }

                firearmPickup.NetworkStatus = firearmPickup.Status;
            }

            NetworkServer.Spawn(ipb.gameObject);
            ipb.InfoReceived(default, Base.PickupDropModel.NetworkInfo);
            Pickup pickup = Pickup.Get(ipb);
            pickup.Scale = Scale;
            return pickup;
        }

        /// <summary>
        /// Spawns the item on the map.
        /// </summary>
        /// <param name="position">The location to spawn the item.</param>
        /// <returns>The <see cref="Pickup"/> created by spawning this item.</returns>
        public virtual Pickup Spawn(Vector3 position) => Spawn(position, default);

        /// <summary>
        /// Returns the Item in a human readable format.
        /// </summary>
        /// <returns>A string containing Item-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}*";
    }
}
