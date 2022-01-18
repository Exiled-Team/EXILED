// -----------------------------------------------------------------------
// <copyright file="Player.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------


namespace Exiled.Events.Handlers
{
    using Exiled.API.Events;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Extensions;

    using static Exiled.Events.Events;

    /// <summary>
    /// Player related events.
    /// </summary>
    public static class Player
    {
        /// <summary>
        /// Invoked before authenticating a <see cref="Exiled.API.Features.Player"/>.
        /// </summary>
        public static event CustomEventHandler<PreAuthenticatingEventArgs> PreAuthenticating;

        /// <summary>
        /// Invoked before kicking a <see cref="Exiled.API.Features.Player"/> from the server.
        /// </summary>
        public static event CustomEventHandler<KickingEventArgs> Kicking;

        /// <summary>
        /// Invoked after a <see cref="Exiled.API.Features.Player"/> has been kicked from the server.
        /// </summary>
        public static event CustomEventHandler<KickedEventArgs> Kicked;

        /// <summary>
        /// Invoked before banning a <see cref="Exiled.API.Features.Player"/> from the server.
        /// </summary>
        public static event CustomEventHandler<BanningEventArgs> Banning;

        /// <summary>
        /// Invoked after a <see cref="Exiled.API.Features.Player"/> has been banned from the server.
        /// </summary>
        public static event CustomEventHandler<BannedEventArgs> Banned;

        /// <summary>
        /// Invoked after a <see cref="Exiled.API.Features.Player"/> uses an <see cref="Exiled.API.Features.Items.Item"/>.
        /// </summary>
        /// <remarks>
        /// Invoked after <see cref="ItemUsed"/>, if a player's class has
        /// changed during their health increase, won't fire.
        /// </remarks>
        public static event CustomEventHandler<UsedItemEventArgs> ItemUsed;

        /// <summary>
        /// Invoked after a <see cref="Exiled.API.Features.Player"/> has stopped the use of a <see cref="Exiled.API.Features.Items.Usable"/>.
        /// </summary>
        public static event CustomEventHandler<CancellingItemUseEventArgs> CancellingItemUse;

        /// <summary>
        /// Invoked after a <see cref="Exiled.API.Features.Player"/> interacted with something.
        /// </summary>
        public static event CustomEventHandler<InteractedEventArgs> Interacted;

        /// <summary>
        /// Invoked before spawning a <see cref="Exiled.API.Features.Player"/> <see cref="Exiled.API.Features.Ragdoll"/>.
        /// </summary>
        public static event CustomEventHandler<SpawningRagdollEventArgs> SpawningRagdoll;

        /// <summary>
        /// Invoked before activating the warhead panel.
        /// </summary>
        public static event CustomEventHandler<ActivatingWarheadPanelEventArgs> ActivatingWarheadPanel;

        /// <summary>
        /// Invoked before activating a workstation.
        /// </summary>
        public static event CustomEventHandler<ActivatingWorkstationEventArgs> ActivatingWorkstation;

        /// <summary>
        /// Invoked before deactivating a workstation.
        /// </summary>
        public static event CustomEventHandler<DeactivatingWorkstationEventArgs> DeactivatingWorkstation;

        /// <summary>
        /// Invoked before using an <see cref="Exiled.API.Features.Items.Item"/>.
        /// </summary>
        public static event CustomEventHandler<UsingItemEventArgs> UsingItem;

        /// <summary>
        /// Invoked after a <see cref="Exiled.API.Features.Player"/> has joined the server.
        /// </summary>
        public static event CustomEventHandler<JoinedEventArgs> Joined;

        /// <summary>
        /// Ivoked after a <see cref="Exiled.API.Features.Player"/> has been verified.
        /// </summary>
        public static event CustomEventHandler<VerifiedEventArgs> Verified;

        /// <summary>
        /// Invoked after a <see cref="Exiled.API.Features.Player"/> has left the server.
        /// </summary>
        public static event CustomEventHandler<LeftEventArgs> Left;

        /// <summary>
        /// Invoked before destroying a <see cref="Exiled.API.Features.Player"/>.
        /// </summary>
        public static event CustomEventHandler<DestroyingEventArgs> Destroying;

        /// <summary>
        /// Invoked before hurting a <see cref="Exiled.API.Features.Player"/>.
        /// </summary>
        public static event CustomEventHandler<HurtingEventArgs> Hurting;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> dies.
        /// </summary>
        public static event CustomEventHandler<DyingEventArgs> Dying;

        /// <summary>
        /// Invoked after a <see cref="Exiled.API.Features.Player"/> died.
        /// </summary>
        public static event CustomEventHandler<DiedEventArgs> Died;

        /// <summary>
        /// Invoked before changing a <see cref="Exiled.API.Features.Player"/> role.
        /// </summary>
        /// <remarks>If you set IsAllowed to <see langword="false"/> when Escape is <see langword="true"/>, tickets will still be given to the escapee's team even though they will 'fail' to escape. Use <see cref="Escaping"/> to block escapes instead.</remarks>
        public static event CustomEventHandler<ChangingRoleEventArgs> ChangingRole;

        /// <summary>
        /// Invoked before throwing an <see cref="Exiled.API.Features.Items.Item"/>.
        /// </summary>
        public static event CustomEventHandler<ThrowingItemEventArgs> ThrowingItem;

        /// <summary>
        /// Invoked before dropping an <see cref="Exiled.API.Features.Items.Item"/>.
        /// </summary>
        public static event CustomEventHandler<DroppingItemEventArgs> DroppingItem;

        /// <summary>
        /// Invoked before dropping a null <see cref="Exiled.API.Features.Items.Item"/>.
        /// </summary>
        public static event CustomEventHandler<DroppingNullEventArgs> DroppingNull;

        /// <summary>
        /// Invoked before picking up ammo.
        /// </summary>
        public static event CustomEventHandler<PickingUpAmmoEventArgs> PickingUpAmmo;

        /// <summary>
        /// Invoked before picking up armor.
        /// </summary>
        public static event CustomEventHandler<PickingUpArmorEventArgs> PickingUpArmor;

        /// <summary>
        /// Invoked before picking up an <see cref="Exiled.API.Features.Items.Item"/>.
        /// </summary>
        public static event CustomEventHandler<PickingUpItemEventArgs> PickingUpItem;

        /// <summary>
        /// Invoked before handcuffing a <see cref="Exiled.API.Features.Player"/>.
        /// </summary>
        public static event CustomEventHandler<HandcuffingEventArgs> Handcuffing;

        /// <summary>
        /// Invoked before freeing a handcuffed <see cref="Exiled.API.Features.Player"/>.
        /// </summary>
        public static event CustomEventHandler<RemovingHandcuffsEventArgs> RemovingHandcuffs;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> escapes.
        /// </summary>
        public static event CustomEventHandler<EscapingEventArgs> Escaping;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> begins speaking to the intercom.
        /// </summary>
        public static event CustomEventHandler<IntercomSpeakingEventArgs> IntercomSpeaking;

        /// <summary>
        /// Invoked after a <see cref="Exiled.API.Features.Player"/> gets shot.
        /// </summary>
        public static event CustomEventHandler<ShotEventArgs> Shot;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> shoots a weapon.
        /// </summary>
        public static event CustomEventHandler<ShootingEventArgs> Shooting;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> enters the pocket dimension.
        /// </summary>
        public static event CustomEventHandler<EnteringPocketDimensionEventArgs> EnteringPocketDimension;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> escapes the pocket dimension.
        /// </summary>
        public static event CustomEventHandler<EscapingPocketDimensionEventArgs> EscapingPocketDimension;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> fails to escape the pocket dimension.
        /// </summary>
        public static event CustomEventHandler<FailingEscapePocketDimensionEventArgs> FailingEscapePocketDimension;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> reloads a weapon.
        /// </summary>
        public static event CustomEventHandler<ReloadingWeaponEventArgs> ReloadingWeapon;

        /// <summary>
        /// Invoked before spawning a <see cref="Exiled.API.Features.Player"/>.
        /// </summary>
        public static event CustomEventHandler<SpawningEventArgs> Spawning;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> enters the femur breaker.
        /// </summary>
        public static event CustomEventHandler<EnteringFemurBreakerEventArgs> EnteringFemurBreaker;

        /// <summary>
        /// Invoked before syncing <see cref="Exiled.API.Features.Player"/> data.
        /// </summary>
        public static event CustomEventHandler<SyncingDataEventArgs> SyncingData;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> held <see cref="Exiled.API.Features.Items.Item"/> changes.
        /// </summary>
        public static event CustomEventHandler<ChangingItemEventArgs> ChangingItem;

        /// <summary>
        /// Invoked before changing a <see cref="Exiled.API.Features.Player"/> group.
        /// </summary>
        public static event CustomEventHandler<ChangingGroupEventArgs> ChangingGroup;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> interacts with a door.
        /// </summary>
        public static event CustomEventHandler<InteractingDoorEventArgs> InteractingDoor;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> interacts with an elevator.
        /// </summary>
        public static event CustomEventHandler<InteractingElevatorEventArgs> InteractingElevator;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> interacts with a locker.
        /// </summary>
        public static event CustomEventHandler<InteractingLockerEventArgs> InteractingLocker;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> triggers a tesla gate.
        /// </summary>
        public static event CustomEventHandler<TriggeringTeslaEventArgs> TriggeringTesla;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> unlocks a generator.
        /// </summary>
        public static event CustomEventHandler<UnlockingGeneratorEventArgs> UnlockingGenerator;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> opens a generator.
        /// </summary>
        public static event CustomEventHandler<OpeningGeneratorEventArgs> OpeningGenerator;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> closes a generator.
        /// </summary>
        public static event CustomEventHandler<ClosingGeneratorEventArgs> ClosingGenerator;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> inserts a workstation tablet into a generator.
        /// </summary>
        public static event CustomEventHandler<ActivatingGeneratorEventArgs> ActivatingGenerator;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> ejects the workstation tablet out of a generator.
        /// </summary>
        public static event CustomEventHandler<StoppingGeneratorEventArgs> StoppingGenerator;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> receives a status effect.
        /// </summary>
        public static event CustomEventHandler<ReceivingEffectEventArgs> ReceivingEffect;

        /// <summary>
        /// Invoked before an user's mute status is changed.
        /// </summary>
        public static event CustomEventHandler<ChangingMuteStatusEventArgs> ChangingMuteStatus;

        /// <summary>
        /// Invoked before an user's intercom mute status is changed.
        /// </summary>
        public static event CustomEventHandler<ChangingIntercomMuteStatusEventArgs> ChangingIntercomMuteStatus;

        /// <summary>
        /// Invoked before a user's radio battery charge is changed.
        /// </summary>
        public static event CustomEventHandler<UsingRadioBatteryEventArgs> UsingRadioBattery;

        /// <summary>
        /// Invoked before a user's radio preset is changed.
        /// </summary>
        public static event CustomEventHandler<ChangingRadioPresetEventArgs> ChangingRadioPreset;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> MicroHID state is changed.
        /// </summary>
        public static event CustomEventHandler<ChangingMicroHIDStateEventArgs> ChangingMicroHIDState;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> MicroHID energy is changed.
        /// </summary>
        public static event CustomEventHandler<UsingMicroHIDEnergyEventArgs> UsingMicroHIDEnergy;

        /// <summary>
        /// Called before processing a hotkey.
        /// </summary>
        public static event CustomEventHandler<ProcessingHotkeyEventArgs> ProcessingHotkey;

        /// <summary>
        /// Invoked before dropping ammo.
        /// </summary>
        public static event CustomEventHandler<DroppingAmmoEventArgs> DroppingAmmo;

        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> walks on a sinkhole.
        /// </summary>
        public static event CustomEventHandler<WalkingOnSinkholeEventArgs> WalkingOnSinkhole;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> interacts with a shooting target.
        /// </summary>
        public static event CustomEventHandler<InteractingShootingTargetEventArgs> InteractingShootingTarget;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> damages a shooting target.
        /// </summary>
        public static event CustomEventHandler<DamagingShootingTargetEventArgs> DamagingShootingTarget;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> flips a coin.
        /// </summary>
        public static event CustomEventHandler<FlippingCoinEventArgs> FlippingCoin;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> toggles the flashlight.
        /// </summary>
        public static event CustomEventHandler<TogglingFlashlightEventArgs> TogglingFlashlight;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> unloads a weapon.
        /// </summary>
        public static event CustomEventHandler<UnloadingWeaponEventArgs> UnloadingWeapon;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> triggers an aim action.
        /// </summary>
        public static event CustomEventHandler<AimingDownSightEventArgs> AimingDownSight;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> toggles the weapon's flashlight.
        /// </summary>
        public static event CustomEventHandler<TogglingWeaponFlashlightEventArgs> TogglingWeaponFlashlight;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> dryfires a weapon.
        /// </summary>
        public static event CustomEventHandler<DryfiringWeaponEventArgs> DryfiringWeapon;

        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> walks on a tantrum.
        /// </summary>
        public static event CustomEventHandler<WalkingOnTantrumEventArgs> WalkingOnTantrum;

        /// <summary>
        /// Invoked after a <see cref="Exiled.API.Features.Player"/> presses the voicechat key.
        /// </summary>
        public static event CustomEventHandler<VoiceChattingEventArgs> VoiceChatting;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> makes noise.
        /// </summary>
        public static event CustomEventHandler<MakingNoiseEventArgs> MakingNoise;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> jumps.
        /// </summary>
        public static event CustomEventHandler<JumpingEventArgs> Jumping;

        /// <summary>
        /// Invoked after a <see cref="Exiled.API.Features.Player"/> lands.
        /// </summary>
        public static event CustomEventHandler<LandingEventArgs> Landing;

        /// <summary>
        /// Invoked after a <see cref="Exiled.API.Features.Player"/> presses the transmission key.
        /// </summary>
        public static event CustomEventHandler<TransmittingEventArgs> Transmitting;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> changes move state.
        /// </summary>
        public static event CustomEventHandler<ChangingMoveStateEventArgs> ChangingMoveState;

        /// <summary>
        /// Invoked after a <see cref="Exiled.API.Features.Player"/> changed spectated player.
        /// </summary>
        public static event CustomEventHandler<ChangingSpectatedPlayerEventArgs> ChangingSpectatedPlayer;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> toggles the NoClip mode.
        /// </summary>
        public static event CustomEventHandler<TogglingNoClipEventArgs> TogglingNoClip;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> picks up a SCP-330 candy.
        /// </summary>
        public static event CustomEventHandler<PickingUpScp330EventArgs> PickingUpScp330;

        /// <summary>
        /// Invoked before a <see cref="Exiled.API.Features.Player"/> interacts with SCP-330.
        /// </summary>
        public static event CustomEventHandler<InteractingScp330EventArgs> InteractingScp330;

        /// <summary>
        /// Called before pre-authenticating a <see cref="Exiled.API.Features.Player"/>.
        /// </summary>
        /// <param name="ev">The <see cref="PreAuthenticatingEventArgs"/> instance.</param>
        public static void OnPreAuthenticating(PreAuthenticatingEventArgs ev) => EventManager.Instance.Invoke<PreAuthenticatingEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnPreAuthenticating(PreAuthenticatingEventArgs ev) => PreAuthenticating.InvokeSafely(ev);


        /// <summary>
        /// Called before kicking a <see cref="Exiled.API.Features.Player"/> from the server.
        /// </summary>
        /// <param name="ev">The <see cref="KickingEventArgs"/> instance.</param>
        public static void OnKicking(KickingEventArgs ev) => EventManager.Instance.Invoke<KickingEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnKicking(KickingEventArgs ev) => Kicking.InvokeSafely(ev);


        /// <summary>
        /// Called after a <see cref="Exiled.API.Features.Player"/> has been kicked from the server.
        /// </summary>
        /// <param name="ev">The <see cref="KickedEventArgs"/> instance.</param>
        public static void OnKicked(KickedEventArgs ev) => EventManager.Instance.Invoke<KickedEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnKicked(KickedEventArgs ev) => Kicked.InvokeSafely(ev);


        /// <summary>
        /// Called before banning a <see cref="Exiled.API.Features.Player"/> from the server.
        /// </summary>
        /// <param name="ev">The <see cref="BanningEventArgs"/> instance.</param>
        public static void OnBanning(BanningEventArgs ev) => EventManager.Instance.Invoke<BanningEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnBanning(BanningEventArgs ev) => Banning.InvokeSafely(ev);


        /// <summary>
        /// Called after a player has been banned from the server.
        /// </summary>
        /// <param name="ev">The <see cref="BannedEventArgs"/> instance.</param>
        public static void OnBanned(BannedEventArgs ev) => EventManager.Instance.Invoke<BannedEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnBanned(BannedEventArgs ev) => Banned.InvokeSafely(ev);


        /// <summary>
        /// Called after a <see cref="Exiled.API.Features.Player"/> used a medical item.
        /// </summary>
        /// <param name="ev">The <see cref="UsedItemEventArgs"/> instance.</param>
        public static void OnItemUsed(UsedItemEventArgs ev) => EventManager.Instance.Invoke<UsedItemEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnItemUsed(UsedItemEventArgs ev) => ItemUsed.InvokeSafely(ev);


        /// <summary>
        /// Called after a <see cref="Exiled.API.Features.Player"/> has stopped the use of a medical item.
        /// </summary>
        /// <param name="ev">The <see cref="CancellingItemUseEventArgs"/> instance.</param>
        public static void OnCancellingItemUse(CancellingItemUseEventArgs ev) => EventManager.Instance.Invoke<CancellingItemUseEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnCancellingItemUse(CancellingItemUseEventArgs ev) => CancellingItemUse.InvokeSafely(ev);


        /// <summary>
        /// Called after a <see cref="Exiled.API.Features.Player"/> interacted with something.
        /// </summary>
        /// <param name="ev">The <see cref="InteractedEventArgs"/> instance.</param>
        public static void OnInteracted(InteractedEventArgs ev) => EventManager.Instance.Invoke<InteractedEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnInteracted(InteractedEventArgs ev) => Interacted.InvokeSafely(ev);


        /// <summary>
        /// Called before spawning a <see cref="Exiled.API.Features.Player"/> ragdoll.
        /// </summary>
        /// <param name="ev">The <see cref="SpawningRagdollEventArgs"/> instance.</param>
        public static void OnSpawningRagdoll(SpawningRagdollEventArgs ev) => EventManager.Instance.Invoke<SpawningRagdollEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnSpawningRagdoll(SpawningRagdollEventArgs ev) => SpawningRagdoll.InvokeSafely(ev);


        /// <summary>
        /// Called before activating the warhead panel.
        /// </summary>
        /// <param name="ev">The <see cref="ActivatingWarheadPanelEventArgs"/> instance.</param>
        public static void OnActivatingWarheadPanel(ActivatingWarheadPanelEventArgs ev) => EventManager.Instance.Invoke<ActivatingWarheadPanelEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnActivatingWarheadPanel(ActivatingWarheadPanelEventArgs ev) => ActivatingWarheadPanel.InvokeSafely(ev);


        /// <summary>
        /// Called before activating a workstation.
        /// </summary>
        /// <param name="ev">The <see cref="ActivatingWorkstation"/> instance.</param>
        public static void OnActivatingWorkstation(ActivatingWorkstationEventArgs ev) => EventManager.Instance.Invoke<ActivatingWorkstationEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnActivatingWorkstation(ActivatingWorkstationEventArgs ev) => ActivatingWorkstation.InvokeSafely(ev);


        /// <summary>
        /// Called before deactivating a workstation.
        /// </summary>
        /// <param name="ev">The <see cref="DeactivatingWorkstationEventArgs"/> instance.</param>
        public static void OnDeactivatingWorkstation(DeactivatingWorkstationEventArgs ev) => EventManager.Instance.Invoke<DeactivatingWorkstationEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnDeactivatingWorkstation(DeactivatingWorkstationEventArgs ev) => DeactivatingWorkstation.InvokeSafely(ev);


        /// <summary>
        /// Called before using a medical item.
        /// </summary>
        /// <param name="ev">The <see cref="UsingItemEventArgs"/> instance.</param>
        public static void OnUsingItem(UsingItemEventArgs ev) => EventManager.Instance.Invoke<UsingItemEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnUsingItem(UsingItemEventArgs ev) => UsingItem.InvokeSafely(ev);


        /// <summary>
        /// Called after a <see cref="Exiled.API.Features.Player"/> has joined the server.
        /// </summary>
        /// <param name="ev">The <see cref="JoinedEventArgs"/> instance.</param>
        public static void OnJoined(JoinedEventArgs ev) => EventManager.Instance.Invoke<JoinedEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnJoined(JoinedEventArgs ev) => Joined.InvokeSafely(ev);


        /// <summary>
        /// Called after a <see cref="Exiled.API.Features.Player"/> has been verified.
        /// </summary>
        /// <param name="ev">The <see cref="VerifiedEventArgs"/> instance.</param>
        public static void OnVerified(VerifiedEventArgs ev) => EventManager.Instance.Invoke<VerifiedEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnVerified(VerifiedEventArgs ev) => Verified.InvokeSafely(ev);


        /// <summary>
        /// Called after a <see cref="Exiled.API.Features.Player"/> has left the server.
        /// </summary>
        /// <param name="ev">The <see cref="LeftEventArgs"/> instance.</param>
        public static void OnLeft(LeftEventArgs ev) => EventManager.Instance.Invoke<LeftEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnLeft(LeftEventArgs ev) => Left.InvokeSafely(ev);


        /// <summary>
        /// Called before destroying a <see cref="Exiled.API.Features.Player"/>.
        /// </summary>
        /// <param name="ev">The <see cref="DestroyingEventArgs"/> instance.</param>
        public static void OnDestroying(DestroyingEventArgs ev) => EventManager.Instance.Invoke<DestroyingEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnDestroying(DestroyingEventArgs ev) => Destroying.InvokeSafely(ev);


        /// <summary>
        /// Called before hurting a player.
        /// </summary>
        /// <param name="ev">The <see cref="HurtingEventArgs"/> instance.</param>
        public static void OnHurting(HurtingEventArgs ev) => EventManager.Instance.Invoke<HurtingEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnHurting(HurtingEventArgs ev) => Hurting.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> dies.
        /// </summary>
        /// <param name="ev"><see cref="DyingEventArgs"/> instance.</param>
        public static void OnDying(DyingEventArgs ev) => EventManager.Instance.Invoke<DyingEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnDying(DyingEventArgs ev) => Dying.InvokeSafely(ev);


        /// <summary>
        /// Called after a <see cref="Exiled.API.Features.Player"/> died.
        /// </summary>
        /// <param name="ev">The <see cref="DiedEventArgs"/> instance.</param>
        public static void OnDied(DiedEventArgs ev) => EventManager.Instance.Invoke<DiedEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnDied(DiedEventArgs ev) => Died.InvokeSafely(ev);


        /// <summary>
        /// Called before changing a <see cref="Exiled.API.Features.Player"/> role.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingRoleEventArgs"/> instance.</param>
        /// <remarks>If you set IsAllowed to false when Escape is true, tickets will still be given to the escapee's team even though they will 'fail' to escape. Use <see cref="Escaping"/> to block escapes instead.</remarks>
        public static void OnChangingRole(ChangingRoleEventArgs ev) => EventManager.Instance.Invoke<ChangingRoleEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnChangingRole(ChangingRoleEventArgs ev) => ChangingRole.InvokeSafely(ev);


        /// <summary>
        /// Called before throwing a grenade.
        /// </summary>
        /// <param name="ev">The <see cref="ThrowingItemEventArgs"/> instance.</param>
        public static void OnThrowingItem(ThrowingItemEventArgs ev) => EventManager.Instance.Invoke<ThrowingItemEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnThrowingItem(ThrowingItemEventArgs ev) => ThrowingItem.InvokeSafely(ev);


        /// <summary>
        /// Called before dropping an item.
        /// </summary>
        /// <param name="ev">The <see cref="DroppingItemEventArgs"/> instance.</param>
        public static void OnDroppingItem(DroppingItemEventArgs ev) => EventManager.Instance.Invoke<DroppingItemEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnDroppingItem(DroppingItemEventArgs ev) => DroppingItem.InvokeSafely(ev);


        /// <summary>
        /// Called before dropping a null item.
        /// </summary>
        /// <param name="ev">The <see cref="DroppingNullEventArgs"/> instance.</param>
        public static void OnDroppingNull(DroppingNullEventArgs ev) => EventManager.Instance.Invoke<DroppingNullEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnDroppingNull(DroppingNullEventArgs ev) => DroppingNull.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> picks up ammo.
        /// </summary>
        /// <param name="ev">The <see cref="PickingUpAmmoEventArgs"/> instance.</param>
        public static void OnPickingUpAmmo(PickingUpAmmoEventArgs ev) => EventManager.Instance.Invoke<PickingUpAmmoEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnPickingUpAmmo(PickingUpAmmoEventArgs ev) => PickingUpAmmo.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> picks up armor.
        /// </summary>
        /// <param name="ev">The <see cref="PickingUpArmorEventArgs"/> instance.</param>
        public static void OnPickingUpArmor(PickingUpArmorEventArgs ev) => EventManager.Instance.Invoke<PickingUpArmorEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnPickingUpArmor(PickingUpArmorEventArgs ev) => PickingUpArmor.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> picks up an item.
        /// </summary>
        /// <param name="ev">The <see cref="PickingUpItemEventArgs"/> instance.</param>
        public static void OnPickingUpItem(PickingUpItemEventArgs ev) => EventManager.Instance.Invoke<PickingUpItemEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnPickingUpItem(PickingUpItemEventArgs ev) => PickingUpItem.InvokeSafely(ev);


        /// <summary>
        /// Called before handcuffing a <see cref="Exiled.API.Features.Player"/>.
        /// </summary>
        /// <param name="ev">The <see cref="HandcuffingEventArgs"/> instance.</param>
        public static void OnHandcuffing(HandcuffingEventArgs ev) => EventManager.Instance.Invoke<HandcuffingEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnHandcuffing(HandcuffingEventArgs ev) => Handcuffing.InvokeSafely(ev);


        /// <summary>
        /// Called before freeing a handcuffed <see cref="Exiled.API.Features.Player"/>.
        /// </summary>
        /// <param name="ev">The <see cref="RemovingHandcuffsEventArgs"/> instance.</param>
        public static void OnRemovingHandcuffs(RemovingHandcuffsEventArgs ev) => EventManager.Instance.Invoke<RemovingHandcuffsEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnRemovingHandcuffs(RemovingHandcuffsEventArgs ev) => RemovingHandcuffs.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> escapes.
        /// </summary>
        /// <param name="ev">The <see cref="EscapingEventArgs"/> instance.</param>
        public static void OnEscaping(EscapingEventArgs ev) => EventManager.Instance.Invoke<EscapingEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnEscaping(EscapingEventArgs ev) => Escaping.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> begins speaking to the intercom.
        /// </summary>
        /// <param name="ev">The <see cref="IntercomSpeakingEventArgs"/> instance.</param>
        public static void OnIntercomSpeaking(IntercomSpeakingEventArgs ev) => EventManager.Instance.Invoke<IntercomSpeakingEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnIntercomSpeaking(IntercomSpeakingEventArgs ev) => IntercomSpeaking.InvokeSafely(ev);


        /// <summary>
        /// Called after a <see cref="Exiled.API.Features.Player"/> shoots a weapon.
        /// </summary>
        /// <param name="ev">The <see cref="ShotEventArgs"/> instance.</param>
        public static void OnShot(ShotEventArgs ev) => EventManager.Instance.Invoke<ShotEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnShot(ShotEventArgs ev) => Shot.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> shoots a weapon.
        /// </summary>
        /// <param name="ev">The <see cref="ShootingEventArgs"/> instance.</param>
        public static void OnShooting(ShootingEventArgs ev) => EventManager.Instance.Invoke<ShootingEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnShooting(ShootingEventArgs ev) => Shooting.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> enters the pocket dimension.
        /// </summary>
        /// <param name="ev">The <see cref="EnteringPocketDimensionEventArgs"/> instance.</param>
        public static void OnEnteringPocketDimension(EnteringPocketDimensionEventArgs ev) => EventManager.Instance.Invoke<EnteringPocketDimensionEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnEnteringPocketDimension(EnteringPocketDimensionEventArgs ev) => EnteringPocketDimension.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> escapes the pocket dimension.
        /// </summary>
        /// <param name="ev">The <see cref="EscapingPocketDimensionEventArgs"/> instance.</param>
        public static void OnEscapingPocketDimension(EscapingPocketDimensionEventArgs ev) => EventManager.Instance.Invoke<EscapingPocketDimensionEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnEscapingPocketDimension(EscapingPocketDimensionEventArgs ev) => EscapingPocketDimension.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> fails to escape the pocket dimension.
        /// </summary>
        /// <param name="ev">The <see cref="FailingEscapePocketDimensionEventArgs"/> instance.</param>
        public static void OnFailingEscapePocketDimension(FailingEscapePocketDimensionEventArgs ev) => EventManager.Instance.Invoke<FailingEscapePocketDimensionEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnFailingEscapePocketDimension(FailingEscapePocketDimensionEventArgs ev) => FailingEscapePocketDimension.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> reloads a weapon.
        /// </summary>
        /// <param name="ev">The <see cref="ReloadingWeaponEventArgs"/> instance.</param>
        public static void OnReloadingWeapon(ReloadingWeaponEventArgs ev) => EventManager.Instance.Invoke<ReloadingWeaponEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnReloadingWeapon(ReloadingWeaponEventArgs ev) => ReloadingWeapon.InvokeSafely(ev);


        /// <summary>
        /// Called before spawning a <see cref="Exiled.API.Features.Player"/>.
        /// </summary>
        /// <param name="ev">The <see cref="SpawningEventArgs"/> instance.</param>
        public static void OnSpawning(SpawningEventArgs ev) => EventManager.Instance.Invoke<SpawningEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnSpawning(SpawningEventArgs ev) => Spawning.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> enters the femur breaker.
        /// </summary>
        /// <param name="ev">The <see cref="EnteringFemurBreakerEventArgs"/> instance.</param>
        public static void OnEnteringFemurBreaker(EnteringFemurBreakerEventArgs ev) => EventManager.Instance.Invoke<EnteringFemurBreakerEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnEnteringFemurBreaker(EnteringFemurBreakerEventArgs ev) => EnteringFemurBreaker.InvokeSafely(ev);


        /// <summary>
        /// Called before syncing <see cref="Exiled.API.Features.Player"/> data.
        /// </summary>
        /// <param name="ev">The <see cref="SyncingDataEventArgs"/> instance.</param>
        public static void OnSyncingData(SyncingDataEventArgs ev) => EventManager.Instance.Invoke<SyncingDataEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnSyncingData(SyncingDataEventArgs ev) => SyncingData.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> held item changes.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingItemEventArgs"/> instance.</param>
        public static void OnChangingItem(ChangingItemEventArgs ev) => EventManager.Instance.Invoke<ChangingItemEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnChangingItem(ChangingItemEventArgs ev) => ChangingItem.InvokeSafely(ev);


        /// <summary>
        /// Called before changing a <see cref="Exiled.API.Features.Player"/> group.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingGroupEventArgs"/> instance.</param>
        public static void OnChangingGroup(ChangingGroupEventArgs ev) => EventManager.Instance.Invoke<ChangingGroupEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnChangingGroup(ChangingGroupEventArgs ev) => ChangingGroup.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> interacts with a door.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingBulletHole"/> instance.</param>
        public static void OnInteractingDoor(InteractingDoorEventArgs ev) => EventManager.Instance.Invoke<InteractingDoorEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnInteractingDoor(InteractingDoorEventArgs ev) => InteractingDoor.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> interacts with an elevator.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingBulletHole"/> instance.</param>
        public static void OnInteractingElevator(InteractingElevatorEventArgs ev) => EventManager.Instance.Invoke<InteractingElevatorEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnInteractingElevator(InteractingElevatorEventArgs ev) => InteractingElevator.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> interacts with a locker.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingBulletHole"/> instance.</param>
        public static void OnInteractingLocker(InteractingLockerEventArgs ev) => EventManager.Instance.Invoke<InteractingLockerEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnInteractingLocker(InteractingLockerEventArgs ev) => InteractingLocker.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> triggers a tesla.
        /// </summary>
        /// <param name="ev">The <see cref="TriggeringTeslaEventArgs"/> instance.</param>
        public static void OnTriggeringTesla(TriggeringTeslaEventArgs ev) => EventManager.Instance.Invoke<TriggeringTeslaEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnTriggeringTesla(TriggeringTeslaEventArgs ev) => TriggeringTesla.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> unlocks a generator.
        /// </summary>
        /// <param name="ev">The <see cref="UnlockingGeneratorEventArgs"/> instance.</param>
        public static void OnUnlockingGenerator(UnlockingGeneratorEventArgs ev) => EventManager.Instance.Invoke<UnlockingGeneratorEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnUnlockingGenerator(UnlockingGeneratorEventArgs ev) => UnlockingGenerator.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> opens a generator.
        /// </summary>
        /// <param name="ev">The <see cref="OpeningGeneratorEventArgs"/> instance.</param>
        public static void OnOpeningGenerator(OpeningGeneratorEventArgs ev) => EventManager.Instance.Invoke<OpeningGeneratorEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnOpeningGenerator(OpeningGeneratorEventArgs ev) => OpeningGenerator.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> closes a generator.
        /// </summary>
        /// <param name="ev">The <see cref="ClosingGeneratorEventArgs"/> instance.</param>
        public static void OnClosingGenerator(ClosingGeneratorEventArgs ev) => EventManager.Instance.Invoke<ClosingGeneratorEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnClosingGenerator(ClosingGeneratorEventArgs ev) => ClosingGenerator.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> inserts a workstation tablet into a generator.
        /// </summary>
        /// <param name="ev">The <see cref="ActivatingGeneratorEventArgs"/> instance.</param>
        public static void OnActivatingGenerator(ActivatingGeneratorEventArgs ev) => EventManager.Instance.Invoke<ActivatingGeneratorEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnActivatingGenerator(ActivatingGeneratorEventArgs ev) => ActivatingGenerator.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> ejects the workstation tablet out of a generator.
        /// </summary>
        /// <param name="ev">The <see cref="StoppingGeneratorEventArgs"/> instance.</param>
        public static void OnStoppingGenerator(StoppingGeneratorEventArgs ev) => EventManager.Instance.Invoke<StoppingGeneratorEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnStoppingGenerator(StoppingGeneratorEventArgs ev) => StoppingGenerator.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> receives a status effect.
        /// </summary>
        /// <param name="ev">The <see cref="ReceivingEffectEventArgs"/> instance.</param>
        public static void OnReceivingEffect(ReceivingEffectEventArgs ev) => EventManager.Instance.Invoke<ReceivingEffectEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnReceivingEffect(ReceivingEffectEventArgs ev) => ReceivingEffect.InvokeSafely(ev);


        /// <summary>
        /// Called before an user's mute status is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingMuteStatusEventArgs"/> instance.</param>
        public static void OnChangingMuteStatus(ChangingMuteStatusEventArgs ev) => EventManager.Instance.Invoke<ChangingMuteStatusEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnChangingMuteStatus(ChangingMuteStatusEventArgs ev) => ChangingMuteStatus.InvokeSafely(ev);


        /// <summary>
        /// Called before an user's intercom mute status is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingIntercomMuteStatusEventArgs"/> instance.</param>
        public static void OnChangingIntercomMuteStatus(ChangingIntercomMuteStatusEventArgs ev) => EventManager.Instance.Invoke<ChangingIntercomMuteStatusEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnChangingIntercomMuteStatus(ChangingIntercomMuteStatusEventArgs ev) => ChangingIntercomMuteStatus.InvokeSafely(ev);


        /// <summary>
        /// Called before a user's radio battery charge is changed.
        /// </summary>
        /// <param name="ev">The <see cref="UsingRadioBatteryEventArgs"/> instance.</param>
        public static void OnUsingRadioBattery(UsingRadioBatteryEventArgs ev) => EventManager.Instance.Invoke<UsingRadioBatteryEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnUsingRadioBattery(UsingRadioBatteryEventArgs ev) => UsingRadioBattery.InvokeSafely(ev);


        /// <summary>
        /// Called before a user's radio preset is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingRadioPresetEventArgs"/> instance.</param>
        public static void OnChangingRadioPreset(ChangingRadioPresetEventArgs ev) => EventManager.Instance.Invoke<ChangingRadioPresetEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnChangingRadioPreset(ChangingRadioPresetEventArgs ev) => ChangingRadioPreset.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> MicroHID state is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingRadioPresetEventArgs"/> instance.</param>
        public static void OnChangingMicroHIDState(ChangingMicroHIDStateEventArgs ev) => EventManager.Instance.Invoke<ChangingMicroHIDStateEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnChangingMicroHIDState(ChangingMicroHIDStateEventArgs ev) => ChangingMicroHIDState.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> MicroHID energy is changed.
        /// </summary>
        /// <param name="ev">The <see cref="UsingMicroHIDEnergyEventArgs"/> instance.</param>
        public static void OnUsingMicroHIDEnergy(UsingMicroHIDEnergyEventArgs ev) => EventManager.Instance.Invoke<UsingMicroHIDEnergyEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnUsingMicroHIDEnergy(UsingMicroHIDEnergyEventArgs ev) => UsingMicroHIDEnergy.InvokeSafely(ev);


        /// <summary>
        /// Called before processing a hotkey.
        /// </summary>
        /// <param name="ev">The <see cref="ProcessingHotkeyEventArgs"/> instance.</param>
        public static void OnProcessingHotkey(ProcessingHotkeyEventArgs ev) => EventManager.Instance.Invoke<ProcessingHotkeyEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnProcessingHotkey(ProcessingHotkeyEventArgs ev) => ProcessingHotkey.InvokeSafely(ev);


        /// <summary>
        /// Called before dropping ammo.
        /// </summary>
        /// <param name="ev">The <see cref="DroppingAmmoEventArgs"/> instance.</param>
        public static void OnDroppingAmmo(DroppingAmmoEventArgs ev) => EventManager.Instance.Invoke<DroppingAmmoEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnDroppingAmmo(DroppingAmmoEventArgs ev) => DroppingAmmo.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> walks on a sinkhole.
        /// </summary>
        /// /// <param name="ev">The <see cref="WalkingOnSinkholeEventArgs"/> instance.</param>
        public static void OnWalkingOnSinkhole(WalkingOnSinkholeEventArgs ev) => EventManager.Instance.Invoke<WalkingOnSinkholeEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnWalkingOnSinkhole(WalkingOnSinkholeEventArgs ev) => WalkingOnSinkhole.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> interacts with a shooting target.
        /// </summary>
        /// <param name="ev">The <see cref="InteractingShootingTargetEventArgs"/> instance.</param>
        public static void OnInteractingShootingTarget(InteractingShootingTargetEventArgs ev) => EventManager.Instance.Invoke<InteractingShootingTargetEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnInteractingShootingTarget(InteractingShootingTargetEventArgs ev) => InteractingShootingTarget.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> damages a shooting target.
        /// </summary>
        /// <param name="ev">The <see cref="DamagingShootingTargetEventArgs"/> instance.</param>
        public static void OnDamagingShootingTarget(DamagingShootingTargetEventArgs ev) => EventManager.Instance.Invoke<DamagingShootingTargetEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnDamagingShootingTarget(DamagingShootingTargetEventArgs ev) => DamagingShootingTarget.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> flips a coin.
        /// </summary>
        /// <param name="ev">The <see cref="FlippingCoinEventArgs"/> instance.</param>
        public static void OnFlippingCoin(FlippingCoinEventArgs ev) => EventManager.Instance.Invoke<FlippingCoinEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnFlippingCoin(FlippingCoinEventArgs ev) => FlippingCoin.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> toggles the flashlight.
        /// </summary>
        /// <param name="ev">The <see cref="TogglingFlashlightEventArgs"/> instance.</param>
        public static void OnTogglingFlashlight(TogglingFlashlightEventArgs ev) => EventManager.Instance.Invoke<TogglingFlashlightEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnTogglingFlashlight(TogglingFlashlightEventArgs ev) => TogglingFlashlight.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> unloads a weapon.
        /// </summary>
        /// <param name="ev">The <see cref="UnloadingWeaponEventArgs"/> instance.</param>
        public static void OnUnloadingWeapon(UnloadingWeaponEventArgs ev) => EventManager.Instance.Invoke<UnloadingWeaponEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnUnloadingWeapon(UnloadingWeaponEventArgs ev) => UnloadingWeapon.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> triggers an aim action.
        /// </summary>
        /// <param name="ev">The <see cref="AimingDownSightEventArgs"/> instance.</param>
        public static void OnAimingDownSight(AimingDownSightEventArgs ev) => EventManager.Instance.Invoke<AimingDownSightEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnAimingDownSight(AimingDownSightEventArgs ev) => AimingDownSight.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> toggles the weapon's flashlight.
        /// </summary>
        /// <param name="ev">The <see cref="TogglingWeaponFlashlightEventArgs"/> instance.</param>
        public static void OnTogglingWeaponFlashlight(TogglingWeaponFlashlightEventArgs ev) => EventManager.Instance.Invoke<TogglingWeaponFlashlightEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnTogglingWeaponFlashlight(TogglingWeaponFlashlightEventArgs ev) => TogglingWeaponFlashlight.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> dryfires a weapon.
        /// </summary>
        /// <param name="ev">The <see cref="DryfiringWeaponEventArgs"/> instance.</param>
        public static void OnDryfiringWeapon(DryfiringWeaponEventArgs ev) => EventManager.Instance.Invoke<DryfiringWeaponEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnDryfiringWeapon(DryfiringWeaponEventArgs ev) => DryfiringWeapon.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> walks on a tantrum.
        /// </summary>
        /// /// <param name="ev">The <see cref="WalkingOnTantrumEventArgs"/> instance.</param>
        public static void OnWalkingOnTantrum(WalkingOnTantrumEventArgs ev) => EventManager.Instance.Invoke<WalkingOnTantrumEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnWalkingOnTantrum(WalkingOnTantrumEventArgs ev) => WalkingOnTantrum.InvokeSafely(ev);


        /// <summary>
        /// Invoked after a <see cref="Exiled.API.Features.Player"/> presses the voicechat key.
        /// </summary>
        /// <param name="ev">The <see cref="VoiceChattingEventArgs"/> instance.</param>
        public static void OnVoiceChatting(VoiceChattingEventArgs ev) => EventManager.Instance.Invoke<VoiceChattingEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnVoiceChatting(VoiceChattingEventArgs ev) => VoiceChatting.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> makes noise.
        /// </summary>
        /// <param name="ev">The <see cref="MakingNoiseEventArgs"/> instance.</param>
        public static void OnMakingNoise(MakingNoiseEventArgs ev) => EventManager.Instance.Invoke<MakingNoiseEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnMakingNoise(MakingNoiseEventArgs ev) => MakingNoise.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> jumps.
        /// </summary>
        /// <param name="ev">The <see cref="JumpingEventArgs"/> instance.</param>
        public static void OnJumping(JumpingEventArgs ev) => EventManager.Instance.Invoke<JumpingEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnJumping(JumpingEventArgs ev) => Jumping.InvokeSafely(ev);


        /// <summary>
        /// Called after a <see cref="Exiled.API.Features.Player"/> lands.
        /// </summary>
        /// <param name="ev">The <see cref="LandingEventArgs"/> instance.</param>
        public static void OnLanding(LandingEventArgs ev) => Landing.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="Exiled.API.Features.Player"/> presses the transmission key.
        /// </summary>
        /// <param name="ev">The <see cref="TransmittingEventArgs"/> instance.</param>
        public static void OnTransmitting(TransmittingEventArgs ev) => EventManager.Instance.Invoke<TransmittingEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnTransmitting(TransmittingEventArgs ev) => Transmitting.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> changes move state.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingMoveStateEventArgs"/> instance.</param>
        public static void OnChangingMoveState(ChangingMoveStateEventArgs ev) => EventManager.Instance.Invoke<ChangingMoveStateEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnChangingMoveState(ChangingMoveStateEventArgs ev) => ChangingMoveState.InvokeSafely(ev);


        /// <summary>
        /// Called after a <see cref="Exiled.API.Features.Player"/> changes spectated player.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingSpectatedPlayerEventArgs"/> instance.</param>
        public static void OnChangingSpectatedPlayer(ChangingSpectatedPlayerEventArgs ev) => EventManager.Instance.Invoke<ChangingSpectatedPlayerEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnChangingSpectatedPlayer(ChangingSpectatedPlayerEventArgs ev) => ChangingSpectatedPlayer.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> toggles the NoClip mode.
        /// </summary>
        /// <param name="ev">The <see cref="TogglingNoClipEventArgs"/> instance.</param>
        public static void OnTogglingNoClip(TogglingNoClipEventArgs ev) => EventManager.Instance.Invoke<TogglingNoClipEventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnTogglingNoClip(TogglingNoClipEventArgs ev) => TogglingNoClip.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> picks up a SCP-330 candy.
        /// </summary>
        /// <param name="ev">The <see cref="PickingUpScp330EventArgs"/> instance.</param>
        public static void OnPickingUp330(PickingUpScp330EventArgs ev) => EventManager.Instance.Invoke<PickingUpScp330EventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnPickingUp330(PickingUpScp330EventArgs ev) => PickingUpScp330.InvokeSafely(ev);


        /// <summary>
        /// Called before a <see cref="Exiled.API.Features.Player"/> interacts with SCP-330.
        /// </summary>
        /// <param name="ev">The <see cref="InteractingScp330EventArgs"/> instance.</param>
        public static void OnInteractingScp330(InteractingScp330EventArgs ev) => EventManager.Instance.Invoke<InteractingScp330EventArgs>(ev);

        /// <summary>
        /// Automatically generated for backwards compatibility.
        /// </summary>
        /// <param name="ev">Input from the event system.</param>
        [Subscribe]
        public static void BackwardsCompatOnInteractingScp330(InteractingScp330EventArgs ev) => InteractingScp330.InvokeSafely(ev);

    }
}
