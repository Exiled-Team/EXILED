// -----------------------------------------------------------------------
// <copyright file="Player.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using System;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Features;

    /// <summary>
    /// Player related events.
    /// </summary>
    public static class Player
    {
        /// <summary>
        /// Invoked before authenticating a <see cref="API.Features.Player"/>.
        /// </summary>
        public static readonly Event<PreAuthenticatingEventArgs> PreAuthenticating = new();

        /// <summary>
        /// Invoked before kicking a <see cref="API.Features.Player"/> from the server.
        /// </summary>
        public static readonly Event<KickingEventArgs> Kicking = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> has been kicked from the server.
        /// </summary>
        public static readonly Event<KickedEventArgs> Kicked = new();

        /// <summary>
        /// Invoked before banning a <see cref="API.Features.Player"/> from the server.
        /// </summary>
        public static readonly Event<BanningEventArgs> Banning = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> has been banned from the server.
        /// </summary>
        public static readonly Event<BannedEventArgs> Banned = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> uses an <see cref="API.Features.Items.Item"/>.
        /// </summary>
        /// <remarks>
        /// Invoked after <see cref="UsedItem"/>, if a player's class has
        /// changed during their health increase, won't fire.
        /// </remarks>
        public static readonly Event<UsedItemEventArgs> UsedItem = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> has stopped the use of a <see cref="API.Features.Items.Usable"/>.
        /// </summary>
        public static readonly Event<CancellingItemUseEventArgs> CancellingItemUse = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> interacted with something.
        /// </summary>
        public static readonly Event<InteractedEventArgs> Interacted = new();

        /// <summary>
        /// Invoked before spawning a <see cref="API.Features.Player"/> <see cref="API.Features.Ragdoll"/>.
        /// </summary>
        public static readonly Event<SpawningRagdollEventArgs> SpawningRagdoll = new();

        /// <summary>
        /// Invoked before activating the warhead panel.
        /// </summary>
        public static readonly Event<ActivatingWarheadPanelEventArgs> ActivatingWarheadPanel = new();

        /// <summary>
        /// Invoked before activating a workstation.
        /// </summary>
        public static readonly Event<ActivatingWorkstationEventArgs> ActivatingWorkstation = new();

        /// <summary>
        /// Invoked before deactivating a workstation.
        /// </summary>
        public static readonly Event<DeactivatingWorkstationEventArgs> DeactivatingWorkstation = new();

        /// <summary>
        /// Invoked before using an <see cref="API.Features.Items.Item"/>.
        /// </summary>
        public static readonly Event<UsingItemEventArgs> UsingItem = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> has joined the server.
        /// </summary>
        public static readonly Event<JoinedEventArgs> Joined = new();

        /// <summary>
        /// Ivoked after a <see cref="API.Features.Player"/> has been verified.
        /// </summary>
        public static readonly Event<VerifiedEventArgs> Verified = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> has left the server.
        /// </summary>
        public static readonly Event<LeftEventArgs> Left = new();

        /// <summary>
        /// Invoked before destroying a <see cref="API.Features.Player"/>.
        /// </summary>
        public static readonly Event<DestroyingEventArgs> Destroying = new();

        /// <summary>
        /// Invoked before hurting a <see cref="API.Features.Player"/>.
        /// </summary>
        public static readonly Event<HurtingEventArgs> Hurting = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> dies.
        /// </summary>
        public static readonly Event<DyingEventArgs> Dying = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> died.
        /// </summary>
        public static readonly Event<DiedEventArgs> Died = new();

        /// <summary>
        /// Invoked before changing a <see cref="API.Features.Player"/> role.
        /// </summary>
        /// <remarks>If you set IsAllowed to <see langword="false"/> when Escape is <see langword="true"/>, tickets will still be given to the escapee's team even though they will 'fail' to escape. Use <see cref="Escaping"/> to block escapes instead.</remarks>
        public static readonly Event<ChangingRoleEventArgs> ChangingRole = new();

        /// <summary>
        /// Invoked before throwing an <see cref="API.Features.Items.Item"/>.
        /// </summary>
        public static readonly Event<ThrowingItemEventArgs> ThrowingItem = new();

        /// <summary>
        /// Invoked before dropping an <see cref="API.Features.Items.Item"/>.
        /// </summary>
        public static readonly Event<DroppingItemEventArgs> DroppingItem = new();

        /// <summary>
        /// Invoked before dropping a null <see cref="API.Features.Items.Item"/>.
        /// </summary>
        public static readonly Event<DroppingNullEventArgs> DroppingNull = new();

        /// <summary>
        /// Invoked before picking up ammo.
        /// </summary>
        public static readonly Event<PickingUpAmmoEventArgs> PickingUpAmmo = new();

        /// <summary>
        /// Invoked before picking up armor.
        /// </summary>
        public static readonly Event<PickingUpArmorEventArgs> PickingUpArmor = new();

        /// <summary>
        /// Invoked before picking up an <see cref="API.Features.Items.Item"/>.
        /// </summary>
        public static readonly Event<PickingUpItemEventArgs> PickingUpItem = new();

        /// <summary>
        /// Invoked before handcuffing a <see cref="API.Features.Player"/>.
        /// </summary>
        public static readonly Event<HandcuffingEventArgs> Handcuffing = new();

        /// <summary>
        /// Invoked before freeing a handcuffed <see cref="API.Features.Player"/>.
        /// </summary>
        public static readonly Event<RemovingHandcuffsEventArgs> RemovingHandcuffs = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> escapes.
        /// </summary>
        public static readonly Event<EscapingEventArgs> Escaping = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> begins speaking to the intercom.
        /// </summary>
        public static readonly Event<IntercomSpeakingEventArgs> IntercomSpeaking = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> gets shot.
        /// </summary>
        public static readonly Event<ShotEventArgs> Shot = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> shoots a weapon.
        /// </summary>
        public static readonly Event<ShootingEventArgs> Shooting = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> enters the pocket dimension.
        /// </summary>
        public static readonly Event<EnteringPocketDimensionEventArgs> EnteringPocketDimension = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> escapes the pocket dimension.
        /// </summary>
        public static readonly Event<EscapingPocketDimensionEventArgs> EscapingPocketDimension = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> fails to escape the pocket dimension.
        /// </summary>
        public static readonly Event<FailingEscapePocketDimensionEventArgs> FailingEscapePocketDimension = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> reloads a weapon.
        /// </summary>
        public static readonly Event<ReloadingWeaponEventArgs> ReloadingWeapon = new();

        /// <summary>
        /// Invoked before spawning a <see cref="API.Features.Player"/>.
        /// </summary>
        public static readonly Event<SpawningEventArgs> Spawning = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> has spawned.
        /// </summary>
        public static readonly Event<SpawnedEventArgs> Spawned = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> enters the femur breaker.
        /// </summary>
        public static readonly Event<EnteringFemurBreakerEventArgs> EnteringFemurBreaker = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> held <see cref="API.Features.Items.Item"/> changes.
        /// </summary>
        public static readonly Event<ChangingItemEventArgs> ChangingItem = new();

        /// <summary>
        /// Invoked before changing a <see cref="API.Features.Player"/> group.
        /// </summary>
        public static readonly Event<ChangingGroupEventArgs> ChangingGroup = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> interacts with a door.
        /// </summary>
        public static readonly Event<InteractingDoorEventArgs> InteractingDoor = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> interacts with an elevator.
        /// </summary>
        public static readonly Event<InteractingElevatorEventArgs> InteractingElevator = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> interacts with a locker.
        /// </summary>
        public static readonly Event<InteractingLockerEventArgs> InteractingLocker = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> triggers a tesla gate.
        /// </summary>
        public static readonly Event<TriggeringTeslaEventArgs> TriggeringTesla = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> unlocks a generator.
        /// </summary>
        public static readonly Event<UnlockingGeneratorEventArgs> UnlockingGenerator = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> opens a generator.
        /// </summary>
        public static readonly Event<OpeningGeneratorEventArgs> OpeningGenerator = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> closes a generator.
        /// </summary>
        public static readonly Event<ClosingGeneratorEventArgs> ClosingGenerator = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> turns on the generator by switching lever.
        /// </summary>
        public static readonly Event<ActivatingGeneratorEventArgs> ActivatingGenerator = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> turns off the generator by switching lever.
        /// </summary>
        public static readonly Event<StoppingGeneratorEventArgs> StoppingGenerator = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> receives a status effect.
        /// </summary>
        public static readonly Event<ReceivingEffectEventArgs> ReceivingEffect = new();

        /// <summary>
        /// Invoked before an user's mute status is changed.
        /// </summary>
        public static readonly Event<ChangingMuteStatusEventArgs> ChangingMuteStatus = new();

        /// <summary>
        /// Invoked before an user's intercom mute status is changed.
        /// </summary>
        public static readonly Event<ChangingIntercomMuteStatusEventArgs> ChangingIntercomMuteStatus = new();

        /// <summary>
        /// Invoked before a user's radio battery charge is changed.
        /// </summary>
        public static readonly Event<UsingRadioBatteryEventArgs> UsingRadioBattery = new();

        /// <summary>
        /// Invoked before a user's radio preset is changed.
        /// </summary>
        public static readonly Event<ChangingRadioPresetEventArgs> ChangingRadioPreset = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> MicroHID state is changed.
        /// </summary>
        public static readonly Event<ChangingMicroHIDStateEventArgs> ChangingMicroHIDState = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> MicroHID energy is changed.
        /// </summary>
        public static readonly Event<UsingMicroHIDEnergyEventArgs> UsingMicroHIDEnergy = new();

        /// <summary>
        /// Called before processing a hotkey.
        /// </summary>
        public static readonly Event<ProcessingHotkeyEventArgs> ProcessingHotkey = new();

        /// <summary>
        /// Invoked before dropping ammo.
        /// </summary>
        public static readonly Event<DroppingAmmoEventArgs> DroppingAmmo = new();

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> walks on a sinkhole.
        /// </summary>
        [Obsolete("Use StayingOnEnvironmentalHazard event instead.")]
        public static readonly Event<WalkingOnSinkholeEventArgs> WalkingOnSinkhole = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> interacts with a shooting target.
        /// </summary>
        public static readonly Event<InteractingShootingTargetEventArgs> InteractingShootingTarget = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> damages a shooting target.
        /// </summary>
        public static readonly Event<DamagingShootingTargetEventArgs> DamagingShootingTarget = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> flips a coin.
        /// </summary>
        public static readonly Event<FlippingCoinEventArgs> FlippingCoin = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> toggles the flashlight.
        /// </summary>
        public static readonly Event<TogglingFlashlightEventArgs> TogglingFlashlight = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> unloads a weapon.
        /// </summary>
        public static readonly Event<UnloadingWeaponEventArgs> UnloadingWeapon = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> triggers an aim action.
        /// </summary>
        public static readonly Event<AimingDownSightEventArgs> AimingDownSight = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> toggles the weapon's flashlight.
        /// </summary>
        public static readonly Event<TogglingWeaponFlashlightEventArgs> TogglingWeaponFlashlight = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> dryfires a weapon.
        /// </summary>
        public static readonly Event<DryfiringWeaponEventArgs> DryfiringWeapon = new();

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> walks on a tantrum.
        /// </summary>
        [Obsolete("Use StayingOnEnvironmentalHazard event instead.")]
        public static readonly Event<WalkingOnTantrumEventArgs> WalkingOnTantrum = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> presses the voicechat key.
        /// </summary>
        public static readonly Event<VoiceChattingEventArgs> VoiceChatting = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> makes noise.
        /// </summary>
        public static readonly Event<MakingNoiseEventArgs> MakingNoise = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> jumps.
        /// </summary>
        public static readonly Event<JumpingEventArgs> Jumping = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> lands.
        /// </summary>
        public static readonly Event<LandingEventArgs> Landing = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> presses the transmission key.
        /// </summary>
        public static readonly Event<TransmittingEventArgs> Transmitting = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> changes move state.
        /// </summary>
        public static readonly Event<ChangingMoveStateEventArgs> ChangingMoveState = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> changed spectated player.
        /// </summary>
        public static readonly Event<ChangingSpectatedPlayerEventArgs> ChangingSpectatedPlayer = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> toggles the NoClip mode.
        /// </summary>
        public static readonly Event<TogglingNoClipEventArgs> TogglingNoClip = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> picks up a SCP-330 candy.
        /// </summary>
        [Obsolete("Use Handlers.Scp330.OnInteractingScp330", true)]
        public static readonly Event<PickingUpScp330EventArgs> PickingUpScp330 = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> interacts with SCP-330.
        /// </summary>
        [Obsolete("Use Handlers.Scp330.InteractingScp330", true)]
        public static readonly Event<InteractingScp330EventArgs> InteractingScp330 = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> searches a Pickup.
        /// </summary>
        public static readonly Event<SearchingPickupEventArgs> SearchingPickup = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> damage a Window.
        /// </summary>
        public static readonly Event<DamagingWindowEventArgs> PlayerDamageWindow = new();

        /// <summary>
        /// Invoked after a <see cref="T:Exiled.API.Features.Player" /> has an item added to their inventory.
        /// </summary>
        public static readonly Event<ItemAddedEventArgs> ItemAdded = new();

        /// <summary>
        /// Invoked before KillPlayer is called.
        /// </summary>
        public static readonly Event<KillingPlayerEventArgs> KillingPlayer = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> enters in an environmental hazard.
        /// </summary>
        public static readonly Event<EnteringEnvironmentalHazardEventArgs> EnteringEnvironmentalHazard = new();

        /// <summary>
        /// Invoked when a <see cref="API.Features.Player"/> stays on an environmental hazard.
        /// </summary>
        public static readonly Event<StayingOnEnvironmentalHazardEventArgs> StayingOnEnvironmentalHazard = new();

        /// <summary>
        /// Invoked when a <see cref="API.Features.Player"/> exists from an environmental hazard.
        /// </summary>
        public static readonly Event<ExitingEnvironmentalHazardEventArgs> ExitingEnvironmentalHazard = new();

        /// <summary>
        /// Called before pre-authenticating a <see cref="API.Features.Player"/>.
        /// </summary>
        /// <param name="ev">The <see cref="PreAuthenticatingEventArgs"/> instance.</param>
        public static void OnPreAuthenticating(PreAuthenticatingEventArgs ev) => PreAuthenticating.InvokeSafely(ev);

        /// <summary>
        /// Called before kicking a <see cref="API.Features.Player"/> from the server.
        /// </summary>
        /// <param name="ev">The <see cref="KickingEventArgs"/> instance.</param>
        public static void OnKicking(KickingEventArgs ev) => Kicking.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> has been kicked from the server.
        /// </summary>
        /// <param name="ev">The <see cref="KickedEventArgs"/> instance.</param>
        public static void OnKicked(KickedEventArgs ev) => Kicked.InvokeSafely(ev);

        /// <summary>
        /// Called before banning a <see cref="API.Features.Player"/> from the server.
        /// </summary>
        /// <param name="ev">The <see cref="BanningEventArgs"/> instance.</param>
        public static void OnBanning(BanningEventArgs ev) => Banning.InvokeSafely(ev);

        /// <summary>
        /// Called after a player has been banned from the server.
        /// </summary>
        /// <param name="ev">The <see cref="BannedEventArgs"/> instance.</param>
        public static void OnBanned(BannedEventArgs ev) => Banned.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> used a medical item.
        /// </summary>
        /// <param name="ev">The <see cref="UsedItemEventArgs"/> instance.</param>
        public static void OnUsedItem(UsedItemEventArgs ev) => UsedItem.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> has stopped the use of a medical item.
        /// </summary>
        /// <param name="ev">The <see cref="CancellingItemUseEventArgs"/> instance.</param>
        public static void OnCancellingItemUse(CancellingItemUseEventArgs ev) => CancellingItemUse.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> interacted with something.
        /// </summary>
        /// <param name="ev">The <see cref="InteractedEventArgs"/> instance.</param>
        public static void OnInteracted(InteractedEventArgs ev) => Interacted.InvokeSafely(ev);

        /// <summary>
        /// Called before spawning a <see cref="API.Features.Player"/> ragdoll.
        /// </summary>
        /// <param name="ev">The <see cref="SpawningRagdollEventArgs"/> instance.</param>
        public static void OnSpawningRagdoll(SpawningRagdollEventArgs ev) => SpawningRagdoll.InvokeSafely(ev);

        /// <summary>
        /// Called before activating the warhead panel.
        /// </summary>
        /// <param name="ev">The <see cref="ActivatingWarheadPanelEventArgs"/> instance.</param>
        public static void OnActivatingWarheadPanel(ActivatingWarheadPanelEventArgs ev) => ActivatingWarheadPanel.InvokeSafely(ev);

        /// <summary>
        /// Called before activating a workstation.
        /// </summary>
        /// <param name="ev">The <see cref="ActivatingWorkstation"/> instance.</param>
        public static void OnActivatingWorkstation(ActivatingWorkstationEventArgs ev) => ActivatingWorkstation.InvokeSafely(ev);

        /// <summary>
        /// Called before deactivating a workstation.
        /// </summary>
        /// <param name="ev">The <see cref="DeactivatingWorkstationEventArgs"/> instance.</param>
        public static void OnDeactivatingWorkstation(DeactivatingWorkstationEventArgs ev) => DeactivatingWorkstation.InvokeSafely(ev);

        /// <summary>
        /// Called before using a usable item.
        /// </summary>
        /// <param name="ev">The <see cref="UsingItemEventArgs"/> instance.</param>
        public static void OnUsingItem(UsingItemEventArgs ev) => UsingItem.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> has joined the server.
        /// </summary>
        /// <param name="ev">The <see cref="JoinedEventArgs"/> instance.</param>
        public static void OnJoined(JoinedEventArgs ev) => Joined.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> has been verified.
        /// </summary>
        /// <param name="ev">The <see cref="VerifiedEventArgs"/> instance.</param>
        public static void OnVerified(VerifiedEventArgs ev) => Verified.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> has left the server.
        /// </summary>
        /// <param name="ev">The <see cref="LeftEventArgs"/> instance.</param>
        public static void OnLeft(LeftEventArgs ev) => Left.InvokeSafely(ev);

        /// <summary>
        /// Called before destroying a <see cref="API.Features.Player"/>.
        /// </summary>
        /// <param name="ev">The <see cref="DestroyingEventArgs"/> instance.</param>
        public static void OnDestroying(DestroyingEventArgs ev) => Destroying.InvokeSafely(ev);

        /// <summary>
        /// Called before hurting a player.
        /// </summary>
        /// <param name="ev">The <see cref="HurtingEventArgs"/> instance.</param>
        public static void OnHurting(HurtingEventArgs ev) => Hurting.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> dies.
        /// </summary>
        /// <param name="ev"><see cref="DyingEventArgs"/> instance.</param>
        public static void OnDying(DyingEventArgs ev) => Dying.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> died.
        /// </summary>
        /// <param name="ev">The <see cref="DiedEventArgs"/> instance.</param>
        public static void OnDied(DiedEventArgs ev) => Died.InvokeSafely(ev);

        /// <summary>
        /// Called before changing a <see cref="API.Features.Player"/> role.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingRoleEventArgs"/> instance.</param>
        /// <remarks>If you set IsAllowed to <see langword="false"/> when Escape is <see langword="true"/>, tickets will still be given to the escapee's team even though they will 'fail' to escape. Use <see cref="Escaping"/> to block escapes instead.</remarks>
        public static void OnChangingRole(ChangingRoleEventArgs ev) => ChangingRole.InvokeSafely(ev);

        /// <summary>
        /// Called before throwing a grenade.
        /// </summary>
        /// <param name="ev">The <see cref="ThrowingItemEventArgs"/> instance.</param>
        public static void OnThrowingItem(ThrowingItemEventArgs ev) => ThrowingItem.InvokeSafely(ev);

        /// <summary>
        /// Called before dropping an item.
        /// </summary>
        /// <param name="ev">The <see cref="DroppingItemEventArgs"/> instance.</param>
        public static void OnDroppingItem(DroppingItemEventArgs ev) => DroppingItem.InvokeSafely(ev);

        /// <summary>
        /// Called before dropping a null item.
        /// </summary>
        /// <param name="ev">The <see cref="DroppingNullEventArgs"/> instance.</param>
        public static void OnDroppingNull(DroppingNullEventArgs ev) => DroppingNull.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> picks up ammo.
        /// </summary>
        /// <param name="ev">The <see cref="PickingUpAmmoEventArgs"/> instance.</param>
        public static void OnPickingUpAmmo(PickingUpAmmoEventArgs ev) => PickingUpAmmo.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> picks up armor.
        /// </summary>
        /// <param name="ev">The <see cref="PickingUpArmorEventArgs"/> instance.</param>
        public static void OnPickingUpArmor(PickingUpArmorEventArgs ev) => PickingUpArmor.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> picks up an item.
        /// </summary>
        /// <param name="ev">The <see cref="PickingUpItemEventArgs"/> instance.</param>
        public static void OnPickingUpItem(PickingUpItemEventArgs ev) => PickingUpItem.InvokeSafely(ev);

        /// <summary>
        /// Called before handcuffing a <see cref="API.Features.Player"/>.
        /// </summary>
        /// <param name="ev">The <see cref="HandcuffingEventArgs"/> instance.</param>
        public static void OnHandcuffing(HandcuffingEventArgs ev) => Handcuffing.InvokeSafely(ev);

        /// <summary>
        /// Called before freeing a handcuffed <see cref="API.Features.Player"/>.
        /// </summary>
        /// <param name="ev">The <see cref="RemovingHandcuffsEventArgs"/> instance.</param>
        public static void OnRemovingHandcuffs(RemovingHandcuffsEventArgs ev) => RemovingHandcuffs.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> escapes.
        /// </summary>
        /// <param name="ev">The <see cref="EscapingEventArgs"/> instance.</param>
        public static void OnEscaping(EscapingEventArgs ev) => Escaping.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> begins speaking to the intercom.
        /// </summary>
        /// <param name="ev">The <see cref="IntercomSpeakingEventArgs"/> instance.</param>
        public static void OnIntercomSpeaking(IntercomSpeakingEventArgs ev) => IntercomSpeaking.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> shoots a weapon.
        /// </summary>
        /// <param name="ev">The <see cref="ShotEventArgs"/> instance.</param>
        public static void OnShot(ShotEventArgs ev) => Shot.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> shoots a weapon.
        /// </summary>
        /// <param name="ev">The <see cref="ShootingEventArgs"/> instance.</param>
        public static void OnShooting(ShootingEventArgs ev) => Shooting.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> enters the pocket dimension.
        /// </summary>
        /// <param name="ev">The <see cref="EnteringPocketDimensionEventArgs"/> instance.</param>
        public static void OnEnteringPocketDimension(EnteringPocketDimensionEventArgs ev) => EnteringPocketDimension.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> escapes the pocket dimension.
        /// </summary>
        /// <param name="ev">The <see cref="EscapingPocketDimensionEventArgs"/> instance.</param>
        public static void OnEscapingPocketDimension(EscapingPocketDimensionEventArgs ev) => EscapingPocketDimension.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> fails to escape the pocket dimension.
        /// </summary>
        /// <param name="ev">The <see cref="FailingEscapePocketDimensionEventArgs"/> instance.</param>
        public static void OnFailingEscapePocketDimension(FailingEscapePocketDimensionEventArgs ev) => FailingEscapePocketDimension.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> reloads a weapon.
        /// </summary>
        /// <param name="ev">The <see cref="ReloadingWeaponEventArgs"/> instance.</param>
        public static void OnReloadingWeapon(ReloadingWeaponEventArgs ev) => ReloadingWeapon.InvokeSafely(ev);

        /// <summary>
        /// Called before spawning a <see cref="API.Features.Player"/>.
        /// </summary>
        /// <param name="ev">The <see cref="SpawningEventArgs"/> instance.</param>
        public static void OnSpawning(SpawningEventArgs ev) => Spawning.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> has spawned.
        /// </summary>
        /// <param name="referenceHub">The <see cref="ReferenceHub"/> instance.</param>
        public static void OnSpawned(ReferenceHub referenceHub) => Spawned.InvokeSafely(new SpawnedEventArgs(referenceHub));

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> enters the femur breaker.
        /// </summary>
        /// <param name="ev">The <see cref="EnteringFemurBreakerEventArgs"/> instance.</param>
        public static void OnEnteringFemurBreaker(EnteringFemurBreakerEventArgs ev) => EnteringFemurBreaker.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> held item changes.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingItemEventArgs"/> instance.</param>
        public static void OnChangingItem(ChangingItemEventArgs ev) => ChangingItem.InvokeSafely(ev);

        /// <summary>
        /// Called before changing a <see cref="API.Features.Player"/> group.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingGroupEventArgs"/> instance.</param>
        public static void OnChangingGroup(ChangingGroupEventArgs ev) => ChangingGroup.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> interacts with a door.
        /// </summary>
        /// <param name="ev">The <see cref="InteractingDoorEventArgs"/> instance.</param>
        public static void OnInteractingDoor(InteractingDoorEventArgs ev) => InteractingDoor.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> interacts with an elevator.
        /// </summary>
        /// <param name="ev">The <see cref="InteractingElevatorEventArgs"/> instance.</param>
        public static void OnInteractingElevator(InteractingElevatorEventArgs ev) => InteractingElevator.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> interacts with a locker.
        /// </summary>
        /// <param name="ev">The <see cref="InteractingLockerEventArgs"/> instance.</param>
        public static void OnInteractingLocker(InteractingLockerEventArgs ev) => InteractingLocker.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> triggers a tesla.
        /// </summary>
        /// <param name="ev">The <see cref="TriggeringTeslaEventArgs"/> instance.</param>
        public static void OnTriggeringTesla(TriggeringTeslaEventArgs ev) => TriggeringTesla.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> unlocks a generator.
        /// </summary>
        /// <param name="ev">The <see cref="UnlockingGeneratorEventArgs"/> instance.</param>
        public static void OnUnlockingGenerator(UnlockingGeneratorEventArgs ev) => UnlockingGenerator.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> opens a generator.
        /// </summary>
        /// <param name="ev">The <see cref="OpeningGeneratorEventArgs"/> instance.</param>
        public static void OnOpeningGenerator(OpeningGeneratorEventArgs ev) => OpeningGenerator.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> closes a generator.
        /// </summary>
        /// <param name="ev">The <see cref="ClosingGeneratorEventArgs"/> instance.</param>
        public static void OnClosingGenerator(ClosingGeneratorEventArgs ev) => ClosingGenerator.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> turns on the generator by switching lever.
        /// </summary>
        /// <param name="ev">The <see cref="ActivatingGeneratorEventArgs"/> instance.</param>
        public static void OnActivatingGenerator(ActivatingGeneratorEventArgs ev) => ActivatingGenerator.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> turns off the generator by switching lever.
        /// </summary>
        /// <param name="ev">The <see cref="StoppingGeneratorEventArgs"/> instance.</param>
        public static void OnStoppingGenerator(StoppingGeneratorEventArgs ev) => StoppingGenerator.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> receives a status effect.
        /// </summary>
        /// <param name="ev">The <see cref="ReceivingEffectEventArgs"/> instance.</param>
        public static void OnReceivingEffect(ReceivingEffectEventArgs ev) => ReceivingEffect.InvokeSafely(ev);

        /// <summary>
        /// Called before an user's mute status is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingMuteStatusEventArgs"/> instance.</param>
        public static void OnChangingMuteStatus(ChangingMuteStatusEventArgs ev) => ChangingMuteStatus.InvokeSafely(ev);

        /// <summary>
        /// Called before an user's intercom mute status is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingIntercomMuteStatusEventArgs"/> instance.</param>
        public static void OnChangingIntercomMuteStatus(ChangingIntercomMuteStatusEventArgs ev) => ChangingIntercomMuteStatus.InvokeSafely(ev);

        /// <summary>
        /// Called before a user's radio battery charge is changed.
        /// </summary>
        /// <param name="ev">The <see cref="UsingRadioBatteryEventArgs"/> instance.</param>
        public static void OnUsingRadioBattery(UsingRadioBatteryEventArgs ev) => UsingRadioBattery.InvokeSafely(ev);

        /// <summary>
        /// Called before a user's radio preset is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingRadioPresetEventArgs"/> instance.</param>
        public static void OnChangingRadioPreset(ChangingRadioPresetEventArgs ev) => ChangingRadioPreset.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> MicroHID state is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingRadioPresetEventArgs"/> instance.</param>
        public static void OnChangingMicroHIDState(ChangingMicroHIDStateEventArgs ev) => ChangingMicroHIDState.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> MicroHID energy is changed.
        /// </summary>
        /// <param name="ev">The <see cref="UsingMicroHIDEnergyEventArgs"/> instance.</param>
        public static void OnUsingMicroHIDEnergy(UsingMicroHIDEnergyEventArgs ev) => UsingMicroHIDEnergy.InvokeSafely(ev);

        /// <summary>
        /// Called before processing a hotkey.
        /// </summary>
        /// <param name="ev">The <see cref="ProcessingHotkeyEventArgs"/> instance.</param>
        public static void OnProcessingHotkey(ProcessingHotkeyEventArgs ev) => ProcessingHotkey.InvokeSafely(ev);

        /// <summary>
        /// Called before dropping ammo.
        /// </summary>
        /// <param name="ev">The <see cref="DroppingAmmoEventArgs"/> instance.</param>
        public static void OnDroppingAmmo(DroppingAmmoEventArgs ev) => DroppingAmmo.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> walks on a sinkhole.
        /// </summary>
        /// /// <param name="ev">The <see cref="WalkingOnSinkholeEventArgs"/> instance.</param>
        public static void OnWalkingOnSinkhole(WalkingOnSinkholeEventArgs ev) => WalkingOnSinkhole.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> interacts with a shooting target.
        /// </summary>
        /// <param name="ev">The <see cref="InteractingShootingTargetEventArgs"/> instance.</param>
        public static void OnInteractingShootingTarget(InteractingShootingTargetEventArgs ev) => InteractingShootingTarget.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> damages a shooting target.
        /// </summary>
        /// <param name="ev">The <see cref="DamagingShootingTargetEventArgs"/> instance.</param>
        public static void OnDamagingShootingTarget(DamagingShootingTargetEventArgs ev) => DamagingShootingTarget.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> flips a coin.
        /// </summary>
        /// <param name="ev">The <see cref="FlippingCoinEventArgs"/> instance.</param>
        public static void OnFlippingCoin(FlippingCoinEventArgs ev) => FlippingCoin.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> toggles the flashlight.
        /// </summary>
        /// <param name="ev">The <see cref="TogglingFlashlightEventArgs"/> instance.</param>
        public static void OnTogglingFlashlight(TogglingFlashlightEventArgs ev) => TogglingFlashlight.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> unloads a weapon.
        /// </summary>
        /// <param name="ev">The <see cref="UnloadingWeaponEventArgs"/> instance.</param>
        public static void OnUnloadingWeapon(UnloadingWeaponEventArgs ev) => UnloadingWeapon.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> triggers an aim action.
        /// </summary>
        /// <param name="ev">The <see cref="AimingDownSightEventArgs"/> instance.</param>
        public static void OnAimingDownSight(AimingDownSightEventArgs ev) => AimingDownSight.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> toggles the weapon's flashlight.
        /// </summary>
        /// <param name="ev">The <see cref="TogglingWeaponFlashlightEventArgs"/> instance.</param>
        public static void OnTogglingWeaponFlashlight(TogglingWeaponFlashlightEventArgs ev) => TogglingWeaponFlashlight.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> dryfires a weapon.
        /// </summary>
        /// <param name="ev">The <see cref="DryfiringWeaponEventArgs"/> instance.</param>
        public static void OnDryfiringWeapon(DryfiringWeaponEventArgs ev) => DryfiringWeapon.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> walks on a tantrum.
        /// </summary>
        /// /// <param name="ev">The <see cref="WalkingOnTantrumEventArgs"/> instance.</param>
        public static void OnWalkingOnTantrum(WalkingOnTantrumEventArgs ev) => WalkingOnTantrum.InvokeSafely(ev);

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> presses the voicechat key.
        /// </summary>
        /// <param name="ev">The <see cref="VoiceChattingEventArgs"/> instance.</param>
        public static void OnVoiceChatting(VoiceChattingEventArgs ev) => VoiceChatting.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> makes noise.
        /// </summary>
        /// <param name="ev">The <see cref="MakingNoiseEventArgs"/> instance.</param>
        public static void OnMakingNoise(MakingNoiseEventArgs ev) => MakingNoise.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> jumps.
        /// </summary>
        /// <param name="ev">The <see cref="JumpingEventArgs"/> instance.</param>
        public static void OnJumping(JumpingEventArgs ev) => Jumping.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> lands.
        /// </summary>
        /// <param name="ev">The <see cref="LandingEventArgs"/> instance.</param>
        public static void OnLanding(LandingEventArgs ev) => Landing.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> presses the transmission key.
        /// </summary>
        /// <param name="ev">The <see cref="TransmittingEventArgs"/> instance.</param>
        public static void OnTransmitting(TransmittingEventArgs ev) => Transmitting.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> changes move state.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingMoveStateEventArgs"/> instance.</param>
        public static void OnChangingMoveState(ChangingMoveStateEventArgs ev) => ChangingMoveState.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> changes spectated player.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingSpectatedPlayerEventArgs"/> instance.</param>
        public static void OnChangingSpectatedPlayer(ChangingSpectatedPlayerEventArgs ev) => ChangingSpectatedPlayer.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> toggles the NoClip mode.
        /// </summary>
        /// <param name="ev">The <see cref="TogglingNoClipEventArgs"/> instance.</param>
        public static void OnTogglingNoClip(TogglingNoClipEventArgs ev) => TogglingNoClip.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> picks up a SCP-330 candy.
        /// </summary>
        /// <param name="ev">The <see cref="PickingUpScp330EventArgs"/> instance.</param>
        [Obsolete("Use Handlers.Scp330.OnPickingUp330", true)]
        public static void OnPickingUp330(PickingUpScp330EventArgs ev) => PickingUpScp330.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> interacts with SCP-330.
        /// </summary>
        /// <param name="ev">The <see cref="InteractingScp330EventArgs"/> instance.</param>
        [Obsolete("Use Handlers.Scp330.OnInteractingScp330", true)]
        public static void OnInteractingScp330(InteractingScp330EventArgs ev) => InteractingScp330.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> searches a Pickup.
        /// </summary>
        /// <param name="ev">The <see cref="SearchingPickupEventArgs"/> instance.</param>
        public static void OnSearchPickupRequest(SearchingPickupEventArgs ev) => SearchingPickup.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> damage a window.
        /// </summary>
        /// <param name="ev">The <see cref="DamagingWindowEventArgs"/> instance.</param>
        public static void OnPlayerDamageWindow(DamagingWindowEventArgs ev) => PlayerDamageWindow.InvokeSafely(ev);

        /// <summary>
        ///  Called before KillPlayer is called.
        /// </summary>
        /// <param name="ev">The <see cref="KillingPlayerEventArgs"/> event handler. </param>
        public static void OnKillPlayer(KillingPlayerEventArgs ev) => KillingPlayer.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="T:Exiled.API.Features.Player" /> has an item added to their inventory.
        /// </summary>
        /// <param name="inventory">The <see cref="InventorySystem.Inventory"/> the item was added to.</param>
        /// <param name="itemBase">The added <see cref="InventorySystem.Items.ItemBase"/>.</param>
        /// <param name="pickupBase">The <see cref="InventorySystem.Items.Pickups.ItemPickupBase"/> the <see cref="InventorySystem.Items.ItemBase"/> originated from, or <see langword="null"/> if the item was not picked up.</param>
        public static void OnItemAdded(InventorySystem.Inventory inventory, InventorySystem.Items.ItemBase itemBase, InventorySystem.Items.Pickups.ItemPickupBase pickupBase) => ItemAdded.InvokeSafely(new ItemAddedEventArgs(inventory, itemBase, pickupBase));

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> enters in an environmental hazard.
        /// </summary>
        /// <param name="ev">The <see cref="EnteringEnvironmentalHazardEventArgs"/> instance. </param>
        public static void OnEnteringEnvironmentalHazard(EnteringEnvironmentalHazardEventArgs ev) => EnteringEnvironmentalHazard.InvokeSafely(ev);

        /// <summary>
        /// Called when a <see cref="API.Features.Player"/> stays on an environmental hazard.
        /// </summary>
        /// <param name="ev">The <see cref="StayingOnEnvironmentalHazardEventArgs"/> instance. </param>
        public static void OnStayingOnEnvironmentalHazard(StayingOnEnvironmentalHazardEventArgs ev) => StayingOnEnvironmentalHazard.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> exits from an environmental hazard.
        /// </summary>
        /// <param name="ev">The <see cref="ExitingEnvironmentalHazardEventArgs"/> instance. </param>
        public static void OnExitingEnvironmentalHazard(ExitingEnvironmentalHazardEventArgs ev) => ExitingEnvironmentalHazard.InvokeSafely(ev);
    }
}
