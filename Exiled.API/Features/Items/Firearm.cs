// -----------------------------------------------------------------------
// <copyright file="Firearm.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CameraShaking;

    using Enums;

    using Exiled.API.Features.Pickups;
    using Exiled.API.Interfaces;
    using Exiled.API.Structs;

    using Extensions;
    using InventorySystem.Items;
    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Firearms.Attachments.Components;
    using InventorySystem.Items.Firearms.BasicMessages;
    using InventorySystem.Items.Firearms.Modules;

    using UnityEngine;

    using BaseFirearm = InventorySystem.Items.Firearms.Firearm;

    /// <summary>
    /// A wrapper class for <see cref="InventorySystem.Items.Firearms.Firearm"/>.
    /// </summary>
    public class Firearm : Item, IWrapper<BaseFirearm>
    {
        /// <summary>
        /// A <see cref="List{T}"/> of <see cref="Firearm"/> which contains all the existing firearms based on all the <see cref="FirearmType"/>s.
        /// </summary>
        internal static readonly Dictionary<FirearmType, Firearm> ItemTypeToFirearmInstance = new();

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> which contains all the base codes expressed in <see cref="FirearmType"/> and <see cref="uint"/>.
        /// </summary>
        internal static readonly Dictionary<FirearmType, uint> BaseCodesValue = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Firearm"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="InventorySystem.Items.Firearms.Firearm"/> class.</param>
        public Firearm(BaseFirearm itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Firearm"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the firearm.</param>
        internal Firearm(ItemType type)
            : this((BaseFirearm)Server.Host.Inventory.CreateItemInstance(new(type, 0), false))
        {
            FirearmStatusFlags firearmStatusFlags = FirearmStatusFlags.MagazineInserted;
            if (Base.HasAdvantageFlag(AttachmentDescriptiveAdvantages.Flashlight))
                firearmStatusFlags |= FirearmStatusFlags.FlashlightEnabled;

            Base.Status = new(Base.AmmoManagerModule.MaxAmmo, firearmStatusFlags, Base.GetCurrentAttachmentsCode());
        }

        /// <inheritdoc cref="BaseCodesValue"/>.
        public static IReadOnlyDictionary<FirearmType, uint> BaseCodes => BaseCodesValue;

        /// <inheritdoc cref="AvailableAttachmentsValue"/>.
        public static IReadOnlyDictionary<FirearmType, AttachmentIdentifier[]> AvailableAttachments => AvailableAttachmentsValue;

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> which represents all the preferences for each <see cref="Player"/>.
        /// </summary>
        public static IReadOnlyDictionary<Player, Dictionary<FirearmType, AttachmentIdentifier[]>> PlayerPreferences
        {
            get
            {
                IEnumerable<KeyValuePair<Player, Dictionary<FirearmType, AttachmentIdentifier[]>>> playerPreferences =
                    AttachmentsServerHandler.PlayerPreferences.Where(
                        kvp => kvp.Key is not null).Select(
                        (KeyValuePair<ReferenceHub, Dictionary<ItemType, uint>> keyValuePair) =>
                        {
                            return new KeyValuePair<Player, Dictionary<FirearmType, AttachmentIdentifier[]>>(
                                Player.Get(keyValuePair.Key),
                                keyValuePair.Value.ToDictionary(
                                    kvp => kvp.Key.GetFirearmType(),
                                    kvp => kvp.Key.GetFirearmType().GetAttachmentIdentifiers(kvp.Value).ToArray()));
                        });

                return playerPreferences.Where(kvp => kvp.Key is not null).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
        }

        /// <summary>
        /// Gets the <see cref="InventorySystem.Items.Firearms.Firearm"/> that this class is encapsulating.
        /// </summary>
        public new BaseFirearm Base { get; }

        /// <summary>
        /// Gets or sets the amount of ammo in the firearm.
        /// </summary>
        public byte Ammo
        {
            get => Base.Status.Ammo;
            set => Base.Status = new FirearmStatus(value, Base.Status.Flags, Base.Status.Attachments);
        }

        /// <summary>
        /// Gets or sets the max ammo for this firearm.
        /// </summary>
        /// <remarks>Disruptor can't be used for MaxAmmo.</remarks>
        public byte MaxAmmo
        {
            get => Base.AmmoManagerModule.MaxAmmo;
            set
            {
                switch (Base.AmmoManagerModule)
                {
                    case TubularMagazineAmmoManager tubularMagazineAmmoManager:
                        tubularMagazineAmmoManager.MaxAmmo = (byte)(value - Base.AttachmentsValue(AttachmentParam.MagazineCapacityModifier) - (Base.Status.Flags.HasFlagFast(FirearmStatusFlags.Cocked) ? tubularMagazineAmmoManager.ChamberedRounds : 0));
                        break;
                    case ClipLoadedInternalMagAmmoManager clipLoadedInternalMagAmmoManager:
                        clipLoadedInternalMagAmmoManager.MaxAmmo = (byte)(value - Base.AttachmentsValue(AttachmentParam.MagazineCapacityModifier));
                        break;
                    case AutomaticAmmoManager automaticAmmoManager:
                        automaticAmmoManager.MaxAmmo = (byte)(value - Base.AttachmentsValue(AttachmentParam.MagazineCapacityModifier) - automaticAmmoManager.ChamberedAmount);
                        break;
                    default:
                        Log.Warn($"MaxAmmo can't be used for this Item: {Type} ({Base.AmmoManagerModule})");
                        return;
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="Enums.FirearmType"/> of the firearm.
        /// </summary>
        public FirearmType FirearmType => Type.GetFirearmType();

        /// <summary>
        /// Gets the <see cref="Enums.AmmoType"/> of the firearm.
        /// </summary>
        public AmmoType AmmoType => Base.AmmoType.GetAmmoType();

        /// <summary>
        /// Gets a value indicating whether the firearm is being aimed.
        /// </summary>
        public bool Aiming => Base.AdsModule.ServerAds;

        /// <summary>
        /// Gets a value indicating whether the firearm Flashlight.
        /// </summary>
        public bool HasFlashlight => Base.HasAdvantageFlag(AttachmentDescriptiveAdvantages.Flashlight);

        /// <summary>
        /// Gets a value indicating whether the firearm's flashlight module is enabled.
        /// </summary>
        public bool FlashlightEnabled => Base.Status.Flags.HasFlagFast(FirearmStatusFlags.FlashlightEnabled);

        /// <summary>
        /// Gets a value indicating whether the firearm's NightVision is being used.
        /// </summary>
        public bool NightVisionEnabled => Aiming && Base.HasAdvantageFlag(AttachmentDescriptiveAdvantages.NightVision);

        /// <summary>
        /// Gets a value indicating whether the firearm's flashlight module is enabled or NightVision is being used.
        /// </summary>
        public bool CanSeeThroughDark => FlashlightEnabled || NightVisionEnabled;

        /// <summary>
        /// Gets a value indicating whether or not the firearm is automatic.
        /// </summary>
        public bool IsAutomatic => Base is AutomaticFirearm;

        /// <summary>
        /// Gets the <see cref="Attachment"/>s of the firearm.
        /// </summary>
        public Attachment[] Attachments => Base.Attachments;

        /// <summary>
        /// Gets the <see cref="AttachmentIdentifier"/>s of the firearm.
        /// </summary>
        public IEnumerable<AttachmentIdentifier> AttachmentIdentifiers
        {
            get
            {
                foreach (Attachment attachment in Attachments.Where(att => att.IsEnabled))
                    yield return AvailableAttachments[FirearmType].FirstOrDefault(att => att == attachment);
            }
        }

        /// <summary>
        /// Gets the base code of the firearm.
        /// </summary>
        public uint BaseCode => BaseCodesValue[FirearmType];

        /// <summary>
        /// Gets the fire rate of the firearm, if it is an automatic weapon.
        /// </summary>
        /// <seealso cref="IsAutomatic"/>
        public float FireRate => Base is AutomaticFirearm auto ? auto._fireRate : 1f;

        /// <summary>
        /// Gets or sets the recoil settings of the firearm, if it's an automatic weapon.
        /// </summary>
        /// <remarks>This property will not do anything if the firearm is not an automatic weapon.</remarks>
        /// <seealso cref="IsAutomatic"/>
        public RecoilSettings Recoil
        {
            get => Base is AutomaticFirearm auto ? auto._recoil : default;
            set
            {
                if (Base is not AutomaticFirearm auto)
                    return;

                auto.ActionModule = new AutomaticAction(
                    Base,
                    auto._semiAutomatic,
                    auto._boltTravelTime,
                    1f / auto._fireRate,
                    auto._dryfireClipId,
                    auto._triggerClipId,
                    auto._gunshotPitchRandomization,
                    value,
                    auto._recoilPattern,
                    false,
                    Mathf.Max(1, auto._chamberSize));
            }
        }

        /// <summary>
        /// Gets the firearm's <see cref="FirearmRecoilPattern"/>. Will be <see langword="null"/> for non-automatic weapons.
        /// </summary>
        public FirearmRecoilPattern RecoilPattern => Base is AutomaticFirearm auto ? auto._recoilPattern : null;

        /// <summary>
        /// Gets the <see cref="FirearmBaseStats"/>.
        /// </summary>
        public FirearmBaseStats Stats => Base.BaseStats;

        /// <summary>
        /// Gets the base damage.
        /// </summary>
        public float BaseDamage => Stats.BaseDamage;

        /// <summary>
        /// Gets the maximum value of the firearm's range.
        /// </summary>
        public float MaxRange => Stats.MaxDistance();

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> of <see cref="ItemType"/> and <see cref="AttachmentIdentifier"/>[] which contains all available attachments for all firearms.
        /// </summary>
        internal static Dictionary<FirearmType, AttachmentIdentifier[]> AvailableAttachmentsValue { get; } = new();

        /// <summary>
        /// Creates and returns a <see cref="Firearm"/> representing the provided <see cref="Enums.FirearmType"/>.
        /// </summary>
        /// <param name="type">The type of firearm to create.</param>
        /// <param name="ammo">The amount of ammo to add. Use null to add max ammo.</param>
        /// <returns>The newly created firearm.</returns>
        public static Firearm Create(FirearmType type, byte? ammo = null)
        {
            Firearm firearm = type is FirearmType.None ? null : (Firearm)Create(type.GetItemType());

            if (firearm is not null)
                firearm.Ammo = ammo ?? firearm.MaxAmmo;

            return firearm;
        }

        /// <summary>
        /// Adds a <see cref="AttachmentIdentifier"/> to the firearm.
        /// </summary>
        /// <param name="identifier">The <see cref="AttachmentIdentifier"/> to add.</param>
        public void AddAttachment(AttachmentIdentifier identifier)
        {
            uint toRemove = 0;
            uint code = 1;

            foreach (Attachment attachment in Base.Attachments)
            {
                if (attachment.Slot == identifier.Slot && attachment.IsEnabled)
                {
                    toRemove = code;
                    break;
                }

                code *= 2;
            }

            uint newCode = identifier.Code == 0
                ? AvailableAttachments[FirearmType].FirstOrDefault(
                    attId =>
                        attId.Name == identifier.Name).Code
                : identifier.Code;

            Base.ApplyAttachmentsCode((Base.GetCurrentAttachmentsCode() & ~toRemove) | newCode, true);
            Base.Status = new FirearmStatus(Math.Min(Ammo, MaxAmmo), Base.Status.Flags, Base.GetCurrentAttachmentsCode());
        }

        /// <summary>
        /// Adds a <see cref="Attachment"/> of the specified <see cref="AttachmentName"/> to the firearm.
        /// </summary>
        /// <param name="attachmentName">The <see cref="AttachmentName"/> to add.</param>
        public void AddAttachment(AttachmentName attachmentName) => AddAttachment(AttachmentIdentifier.Get(FirearmType, attachmentName));

        /// <summary>
        /// Adds a <see cref="IEnumerable{T}"/> of <see cref="AttachmentIdentifier"/> to the firearm.
        /// </summary>
        /// <param name="identifiers">The <see cref="IEnumerable{T}"/> of <see cref="AttachmentIdentifier"/> to add.</param>
        public void AddAttachment(IEnumerable<AttachmentIdentifier> identifiers)
        {
            foreach (AttachmentIdentifier identifier in identifiers)
                AddAttachment(identifier);
        }

        /// <summary>
        /// Adds a <see cref="IEnumerable{T}"/> of <see cref="AttachmentName"/> to the firearm.
        /// </summary>
        /// <param name="attachmentNames">The <see cref="IEnumerable{T}"/> of <see cref="AttachmentName"/> to add.</param>
        public void AddAttachment(IEnumerable<AttachmentName> attachmentNames)
        {
            foreach (AttachmentName attachmentName in attachmentNames)
                AddAttachment(attachmentName);
        }

        /// <summary>
        /// Removes a <see cref="AttachmentIdentifier"/> from the firearm.
        /// </summary>
        /// <param name="identifier">The <see cref="AttachmentIdentifier"/> to remove.</param>
        public void RemoveAttachment(AttachmentIdentifier identifier)
        {
            if (!Attachments.Any(attachment => (attachment.Name == identifier.Name) && attachment.IsEnabled))
                return;

            uint code = identifier.Code;

            Base.ApplyAttachmentsCode(Base.GetCurrentAttachmentsCode() & ~code, true);

            if (identifier.Name == AttachmentName.Flashlight)
                Base.Status = new FirearmStatus(Math.Min(Ammo, MaxAmmo), Base.Status.Flags & ~FirearmStatusFlags.FlashlightEnabled, Base.GetCurrentAttachmentsCode());
            else
                Base.Status = new FirearmStatus(Math.Min(Ammo, MaxAmmo), Base.Status.Flags, Base.GetCurrentAttachmentsCode());
        }

        /// <summary>
        /// Removes a <see cref="Attachment"/> of the specified <see cref="AttachmentName"/> from the firearm.
        /// </summary>
        /// <param name="attachmentName">The <see cref="AttachmentName"/> to remove.</param>
        public void RemoveAttachment(AttachmentName attachmentName)
        {
            uint code = AttachmentIdentifier.Get(FirearmType, attachmentName).Code;

            Base.ApplyAttachmentsCode(Base.GetCurrentAttachmentsCode() & ~code, true);

            if (attachmentName == AttachmentName.Flashlight)
                Base.Status = new FirearmStatus(Math.Min(Ammo, MaxAmmo), Base.Status.Flags & ~FirearmStatusFlags.FlashlightEnabled, Base.GetCurrentAttachmentsCode());
            else
                Base.Status = new FirearmStatus(Math.Min(Ammo, MaxAmmo), Base.Status.Flags, Base.GetCurrentAttachmentsCode());
        }

        /// <summary>
        /// Removes a <see cref="Attachment"/> of the specified <see cref="AttachmentSlot"/> from the firearm.
        /// </summary>
        /// <param name="attachmentSlot">The <see cref="AttachmentSlot"/> to remove.</param>
        public void RemoveAttachment(AttachmentSlot attachmentSlot)
        {
            Attachment firearmAttachment = Attachments.FirstOrDefault(att => (att.Slot == attachmentSlot) && att.IsEnabled);

            if (firearmAttachment is null)
                return;

            uint code = AvailableAttachments[FirearmType].FirstOrDefault(attId => attId == firearmAttachment).Code;

            Base.ApplyAttachmentsCode(Base.GetCurrentAttachmentsCode() & ~code, true);

            if (firearmAttachment.Name == AttachmentName.Flashlight)
                Base.Status = new FirearmStatus(Math.Min(Ammo, MaxAmmo), Base.Status.Flags & ~FirearmStatusFlags.FlashlightEnabled, Base.GetCurrentAttachmentsCode());
            else
                Base.Status = new FirearmStatus(Math.Min(Ammo, MaxAmmo), Base.Status.Flags, Base.GetCurrentAttachmentsCode());
        }

        /// <summary>
        /// Removes a <see cref="IEnumerable{T}"/> of <see cref="AttachmentIdentifier"/> from the firearm.
        /// </summary>
        /// <param name="identifiers">The <see cref="IEnumerable{T}"/> of <see cref="AttachmentIdentifier"/> to remove.</param>
        public void RemoveAttachment(IEnumerable<AttachmentIdentifier> identifiers)
        {
            foreach (AttachmentIdentifier identifier in identifiers)
                RemoveAttachment(identifier);
        }

        /// <summary>
        /// Removes a list of <see cref="Attachment"/> of the specified <see cref="IEnumerable{T}"/> of <see cref="AttachmentName"/> from the firearm.
        /// </summary>
        /// <param name="attachmentNames">The <see cref="IEnumerable{T}"/> of <see cref="AttachmentName"/> to remove.</param>
        public void RemoveAttachment(IEnumerable<AttachmentName> attachmentNames)
        {
            foreach (AttachmentName attachmentName in attachmentNames)
                RemoveAttachment(attachmentName);
        }

        /// <summary>
        /// Removes a list of <see cref="Attachment"/> of the specified <see cref="IEnumerable{T}"/> of <see cref="AttachmentSlot"/> from the firearm.
        /// </summary>
        /// <param name="attachmentSlots">The <see cref="IEnumerable{T}"/> of <see cref="AttachmentSlot"/> to remove.</param>
        public void RemoveAttachment(IEnumerable<AttachmentSlot> attachmentSlots)
        {
            foreach (AttachmentSlot attachmentSlot in attachmentSlots)
                RemoveAttachment(attachmentSlot);
        }

        /// <summary>
        /// Removes all attachments from the firearm.
        /// </summary>
        public void ClearAttachments() => Base.ApplyAttachmentsCode(BaseCode, true);

        /// <summary>
        /// Gets a <see cref="Attachment"/> of the specified <see cref="AttachmentIdentifier"/>.
        /// </summary>
        /// <param name="identifier">The <see cref="AttachmentIdentifier"/> to check.</param>
        /// <returns>The corresponding <see cref="Attachment"/>.</returns>
        public Attachment GetAttachment(AttachmentIdentifier identifier) => Attachments.FirstOrDefault(attachment => attachment == identifier);

        /// <summary>
        /// Tries to get a <see cref="Attachment"/> of the specified <see cref="AttachmentIdentifier"/>.
        /// </summary>
        /// <param name="identifier">The <see cref="AttachmentIdentifier"/> to check.</param>
        /// <param name="firearmAttachment">The corresponding <see cref="Attachment"/>.</param>
        /// <returns>A value indicating whether or not the firearm has the specified <see cref="Attachment"/>.</returns>
        public bool TryGetAttachment(AttachmentIdentifier identifier, out Attachment firearmAttachment)
        {
            firearmAttachment = default;

            if (!Attachments.Any(attachment => attachment.Name == identifier.Name))
                return false;

            firearmAttachment = GetAttachment(identifier);

            return true;
        }

        /// <summary>
        /// Tries to get a <see cref="Attachment"/> of the specified <see cref="AttachmentName"/>.
        /// </summary>
        /// <param name="attachmentName">The <see cref="AttachmentName"/> to check.</param>
        /// <param name="firearmAttachment">The corresponding <see cref="Attachment"/>.</param>
        /// <returns>A value indicating whether or not the firearm has the specified <see cref="Attachment"/>.</returns>
        public bool TryGetAttachment(AttachmentName attachmentName, out Attachment firearmAttachment)
        {
            firearmAttachment = default;

            if (Attachments.All(attachment => attachment.Name != attachmentName))
                return false;

            firearmAttachment = GetAttachment(AttachmentIdentifier.Get(FirearmType, attachmentName));

            return true;
        }

        /// <summary>
        /// Adds or replaces an existing preference to the <see cref="PlayerPreferences"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> of which must be added.</param>
        /// <param name="itemType">The <see cref="Enums.FirearmType"/> to add.</param>
        /// <param name="attachments">The <see cref="AttachmentIdentifier"/>[] to add.</param>
        public void AddPreference(Player player, FirearmType itemType, AttachmentIdentifier[] attachments)
        {
            foreach (KeyValuePair<Player, Dictionary<FirearmType, AttachmentIdentifier[]>> kvp in PlayerPreferences)
            {
                if (kvp.Key != player)
                    continue;

                if (AttachmentsServerHandler.PlayerPreferences.TryGetValue(player.ReferenceHub, out Dictionary<ItemType, uint> dictionary))
                    dictionary[itemType.GetItemType()] = attachments.GetAttachmentsCode();
            }
        }

        /// <summary>
        /// Adds or replaces an existing preference to the <see cref="PlayerPreferences"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> of which must be added.</param>
        /// <param name="preference">The <see cref="KeyValuePair{TKey, TValue}"/> of <see cref="Enums.FirearmType"/> and <see cref="AttachmentIdentifier"/>[] to add.</param>
        public void AddPreference(Player player, KeyValuePair<FirearmType, AttachmentIdentifier[]> preference) => AddPreference(player, preference.Key, preference.Value);

        /// <summary>
        /// Adds or replaces an existing preference to the <see cref="PlayerPreferences"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> of which must be added.</param>
        /// <param name="preference">The <see cref="Dictionary{TKey, TValue}"/> of <see cref="Enums.FirearmType"/> and <see cref="AttachmentIdentifier"/>[] to add.</param>
        public void AddPreference(Player player, Dictionary<FirearmType, AttachmentIdentifier[]> preference)
        {
            foreach (KeyValuePair<FirearmType, AttachmentIdentifier[]> kvp in preference)
                AddPreference(player, kvp);
        }

        /// <summary>
        /// Adds or replaces an existing preference to the <see cref="PlayerPreferences"/>.
        /// </summary>
        /// <param name="players">The <see cref="IEnumerable{T}"/> of <see cref="Player"/> of which must be added.</param>
        /// <param name="type">The <see cref="Enums.FirearmType"/> to add.</param>
        /// <param name="attachments">The <see cref="AttachmentIdentifier"/>[] to add.</param>
        public void AddPreference(IEnumerable<Player> players, FirearmType type, AttachmentIdentifier[] attachments)
        {
            foreach (Player player in players)
                AddPreference(player, type, attachments);
        }

        /// <summary>
        /// Adds or replaces an existing preference to the <see cref="PlayerPreferences"/>.
        /// </summary>
        /// <param name="players">The <see cref="IEnumerable{T}"/> of <see cref="Player"/> of which must be added.</param>
        /// <param name="preference">The <see cref="KeyValuePair{TKey, TValue}"/> of <see cref="Enums.FirearmType"/> and <see cref="AttachmentIdentifier"/>[] to add.</param>
        public void AddPreference(IEnumerable<Player> players, KeyValuePair<FirearmType, AttachmentIdentifier[]> preference)
        {
            foreach (Player player in players)
                AddPreference(player, preference.Key, preference.Value);
        }

        /// <summary>
        /// Adds or replaces an existing preference to the <see cref="PlayerPreferences"/>.
        /// </summary>
        /// <param name="players">The <see cref="IEnumerable{T}"/> of <see cref="Player"/> of which must be added.</param>
        /// <param name="preference">The <see cref="Dictionary{TKey, TValue}"/> of <see cref="Enums.FirearmType"/> and <see cref="AttachmentIdentifier"/>[] to add.</param>
        public void AddPreference(IEnumerable<Player> players, Dictionary<FirearmType, AttachmentIdentifier[]> preference)
        {
            foreach ((Player player, KeyValuePair<FirearmType, AttachmentIdentifier[]> kvp) in players.SelectMany(player => preference.Select(kvp => (player, kvp))))
                AddPreference(player, kvp);
        }

        /// <summary>
        /// Removes a preference from the <see cref="PlayerPreferences"/> if it already exists.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> of which must be removed.</param>
        /// <param name="type">The <see cref="Enums.FirearmType"/> to remove.</param>
        public void RemovePreference(Player player, FirearmType type)
        {
            foreach (KeyValuePair<Player, Dictionary<FirearmType, AttachmentIdentifier[]>> kvp in PlayerPreferences)
            {
                if (kvp.Key != player)
                    continue;

                if (AttachmentsServerHandler.PlayerPreferences.TryGetValue(player.ReferenceHub, out Dictionary<ItemType, uint> dictionary))
                    dictionary[type.GetItemType()] = type.GetBaseCode();
            }
        }

        /// <summary>
        /// Removes a preference from the <see cref="PlayerPreferences"/> if it already exists.
        /// </summary>
        /// <param name="players">The <see cref="IEnumerable{T}"/> of <see cref="Player"/> of which must be removed.</param>
        /// <param name="type">The <see cref="Enums.FirearmType"/> to remove.</param>
        public void RemovePreference(IEnumerable<Player> players, FirearmType type)
        {
            foreach (Player player in players)
                RemovePreference(player, type);
        }

        /// <summary>
        /// Removes a preference from the <see cref="PlayerPreferences"/> if it already exists.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> of which must be removed.</param>
        /// <param name="types">The <see cref="IEnumerable{T}"/> of <see cref="Enums.FirearmType"/> to remove.</param>
        public void RemovePreference(Player player, IEnumerable<FirearmType> types)
        {
            foreach (FirearmType itemType in types)
                RemovePreference(player, itemType);
        }

        /// <summary>
        /// Removes a preference from the <see cref="PlayerPreferences"/> if it already exists.
        /// </summary>
        /// <param name="players">The <see cref="IEnumerable{T}"/> of <see cref="Player"/> of which must be removed.</param>
        /// <param name="types">The <see cref="IEnumerable{T}"/> of <see cref="Enums.FirearmType"/> to remove.</param>
        public void RemovePreference(IEnumerable<Player> players, IEnumerable<FirearmType> types)
        {
            foreach ((Player player, FirearmType firearmType) in players.SelectMany(player => types.Select(itemType => (player, itemType))))
                RemovePreference(player, firearmType);
        }

        /// <summary>
        /// Clears all the existing preferences from <see cref="PlayerPreferences"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> of which must be cleared.</param>
        public void ClearPreferences(Player player)
        {
            if (AttachmentsServerHandler.PlayerPreferences.TryGetValue(player.ReferenceHub, out Dictionary<ItemType, uint> dictionary))
            {
                foreach (KeyValuePair<ItemType, uint> kvp in dictionary)
                    dictionary[kvp.Key] = kvp.Key.GetFirearmType().GetBaseCode();
            }
        }

        /// <summary>
        /// Clears all the existing preferences from <see cref="PlayerPreferences"/>.
        /// </summary>
        /// <param name="players">The <see cref="IEnumerable{T}"/> of <see cref="Player"/> of which must be cleared.</param>
        public void ClearPreferences(IEnumerable<Player> players)
        {
            foreach (Player player in players)
                ClearPreferences(player);
        }

        /// <summary>
        /// Clears all the existing preferences from <see cref="PlayerPreferences"/>.
        /// </summary>
        public void ClearPreferences()
        {
            foreach (Player player in Player.List)
                ClearPreferences(player);
        }

        /// <summary>
        /// Gets the damage based on the specified distance.
        /// </summary>
        /// <param name="distance">The distance to evaluate.</param>
        /// <returns>The corresponding damage based on the specified distance.</returns>
        public float GetDamageAtDistance(float distance) => Stats.DamageAtDistance(Base, distance);

        /// <summary>
        /// Clones current <see cref="Firearm"/> object.
        /// </summary>
        /// <returns> New <see cref="Firearm"/> object. </returns>
        public override Item Clone()
        {
            Firearm cloneableItem = new(Type)
            {
                Ammo = Ammo,
            };

            if (cloneableItem.Base is AutomaticFirearm)
                cloneableItem.Recoil = Recoil;

            cloneableItem.AddAttachment(AttachmentIdentifiers);

            return cloneableItem;
        }

        /// <summary>
        /// Change the owner of the <see cref="Firearm"/>.
        /// </summary>
        /// <param name="oldOwner">old <see cref="Firearm"/> owner.</param>
        /// <param name="newOwner">new <see cref="Firearm"/> owner.</param>
        internal override void ChangeOwner(Player oldOwner, Player newOwner)
        {
            Base.Owner = newOwner.ReferenceHub;

            Base.HitregModule = Base switch
            {
                AutomaticFirearm automaticFirearm =>
                    new SingleBulletHitreg(automaticFirearm, automaticFirearm.Owner, automaticFirearm._recoilPattern),
                Shotgun shotgun =>
                    new BuckshotHitreg(shotgun, shotgun.Owner, shotgun.GetBuckshotPattern),
                ParticleDisruptor particleDisruptor =>
                    new DisruptorHitreg(particleDisruptor, particleDisruptor.Owner, particleDisruptor._explosionSettings),
                Revolver revolver =>
                    new SingleBulletHitreg(revolver, revolver.Owner),
                _ => throw new NotImplementedException("Should never happend"),
            };

            Base._sendStatusNextFrame = true;
            Base._footprintValid = false;
        }
    }
}
