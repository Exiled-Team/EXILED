// -----------------------------------------------------------------------
// <copyright file="Item.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.API.Features.Items
{
#pragma warning disable CS0618
    using System.Collections.Generic;
    using System.Linq;

    using SEXiled.API.Extensions;
    using SEXiled.API.Structs;

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
        internal static readonly Dictionary<ItemBase, Item> BaseToItem = new Dictionary<ItemBase, Item>();

        /// <summary>
        /// A dictionary of all <see cref="Serial"/>s that have been assigned to an item.
        /// </summary>
        internal static readonly Dictionary<ushort, Item> SerialToItem = new Dictionary<ushort, Item>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        /// <param name="itemBase">The <see cref="ItemBase"/> to encapsulate.</param>
        public Item(ItemBase itemBase)
        {
            Base = itemBase;
            Type = itemBase.ItemTypeId;
            Serial = Base.OwnerInventory.UserInventory.Items.FirstOrDefault(i => i.Value == Base).Key;
            if (Serial == 0)
            {
                ushort serial = ItemSerialGenerator.GenerateNext();
                Serial = serial;
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
        public ItemType Type { get; internal set; }

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
            if (itemBase == null)
                return null;

            if (BaseToItem.TryGetValue(itemBase, out Item item))
                return item;

            switch (itemBase)
            {
                case InventorySystem.Items.Firearms.Firearm firearm:
                    return new Firearm(firearm);
                case KeycardItem keycard:
                    return new Keycard(keycard);
                case UsableItem usable:
                {
                    if (usable is Scp330Bag scp330Bag)
                        return new Scp330(scp330Bag);
                    else if (usable is Scp244Item scp244Item)
                        return new Scp244(scp244Item);
                    return new Usable(usable);
                }

                case RadioItem radio:
                    return new Radio(radio);
                case MicroHIDItem micro:
                    return new MicroHid(micro);
                case BodyArmor armor:
                    return new Armor(armor);
                case AmmoItem ammo:
                    return new Ammo(ammo);
                case FlashlightItem flashlight:
                    return new Flashlight(flashlight);
                case ThrowableItem throwable:
                    switch (throwable.Projectile)
                    {
                        case FlashbangGrenade _:
                            return new FlashGrenade(throwable);
                        case ExplosionGrenade _:
                            return new ExplosiveGrenade(throwable);
                        default:
                            return new Throwable(throwable);
                    }

                default:
                    return new Item(itemBase);
            }
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
        public static Item Create(ItemType type, Player owner = null)
        {
            switch (type)
            {
                case ItemType.Adrenaline:
                case ItemType.Medkit:
                case ItemType.Painkillers:
                case ItemType.SCP500:
                case ItemType.SCP207:
                case ItemType.SCP268:
                    return new Usable(type);
                case ItemType.SCP244a:
                case ItemType.SCP244b:
                    return new Scp244(type);
                case ItemType.Ammo9x19:
                case ItemType.Ammo12gauge:
                case ItemType.Ammo44cal:
                case ItemType.Ammo556x45:
                case ItemType.Ammo762x39:
                    return new Ammo(type);
                case ItemType.Flashlight:
                    return new Flashlight();
                case ItemType.Radio:
                    return new Radio();
                case ItemType.MicroHID:
                    return new MicroHid();
                case ItemType.GrenadeFlash:
                    return new FlashGrenade(owner);
                case ItemType.GrenadeHE:
                case ItemType.SCP018:
                    return new ExplosiveGrenade(type, owner);
                case ItemType.GunCrossvec:
                case ItemType.GunLogicer:
                case ItemType.GunRevolver:
                case ItemType.GunShotgun:
                case ItemType.GunAK:
                case ItemType.GunCOM15:
                case ItemType.GunCOM18:
                case ItemType.GunE11SR:
                case ItemType.GunFSP9:
                    return new Firearm(type);
                case ItemType.KeycardGuard:
                case ItemType.KeycardJanitor:
                case ItemType.KeycardO5:
                case ItemType.KeycardScientist:
                case ItemType.KeycardChaosInsurgency:
                case ItemType.KeycardContainmentEngineer:
                case ItemType.KeycardFacilityManager:
                case ItemType.KeycardResearchCoordinator:
                case ItemType.KeycardZoneManager:
                case ItemType.KeycardNTFCommander:
                case ItemType.KeycardNTFLieutenant:
                case ItemType.KeycardNTFOfficer:
                    return new Keycard(type);
                case ItemType.ArmorLight:
                case ItemType.ArmorCombat:
                case ItemType.ArmorHeavy:
                    return new Armor(type);
                case ItemType.SCP330:
                    return new Scp330();
                case ItemType.SCP2176:
                    return new Throwable(type);
                default:
                    return new Item(type);
            }
        }

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
                    if (identifiers != null)
                        firearm.AddAttachment(identifiers);

                    firearmPickup.Status = new FirearmStatus(firearm.Ammo, FirearmStatusFlags.MagazineInserted, firearmPickup.Status.Attachments);
                }
                else
                {
                    byte ammo;
                    switch (Base)
                    {
                        case AutomaticFirearm auto:
                            ammo = auto._baseMaxAmmo;
                            break;
                        case Shotgun shotgun:
                            ammo = shotgun._ammoCapacity;
                            break;
                        case Revolver revolver:
                            ammo = revolver.AmmoManagerModule.MaxAmmo;
                            break;
                        default:
                            ammo = 0;
                            break;
                    }

                    uint code = identifiers != null ? (uint)firearmPickup.Info.ItemId.GetBaseCode() + identifiers.GetAttachmentsCode() : firearmPickup.Status.Attachments;
                    firearmPickup.Status = new FirearmStatus(ammo, FirearmStatusFlags.MagazineInserted, code);
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
                    byte ammo;
                    switch (Base)
                    {
                        case AutomaticFirearm auto:
                            ammo = auto._baseMaxAmmo;
                            break;
                        case Shotgun shotgun:
                            ammo = shotgun._ammoCapacity;
                            break;
                        case Revolver _:
                            ammo = 6;
                            break;
                        default:
                            ammo = 0;
                            break;
                    }

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
        public override string ToString()
        {
            return $"{Type} ({Serial}) [{Weight}] *{Scale}*";
        }
    }
}
