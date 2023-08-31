// -----------------------------------------------------------------------
// <copyright file="Player.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
#pragma warning disable IDE0079
#pragma warning disable IDE0060
#pragma warning disable SA1623 // Property summary documentation should match accessors

    using System;

    using Exiled.Events.EventArgs.Player;

    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.Features;

    using PluginAPI.Core.Attributes;
    using PluginAPI.Enums;
    using PluginAPI.Events;

    /// <summary>
    /// Player related events.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Invoked before authenticating a <see cref="API.Features.Player"/>.
        /// </summary>
        public static Event<PreAuthenticatingEventArgs> PreAuthenticating { get; set; } = new();

        /// <summary>
        /// Invoked before reserved slot is finalized for a <see cref="API.Features.Player"/>.
        /// </summary>
        public static Event<ReservedSlotsCheckEventArgs> ReservedSlot { get; set; } = new();

        /// <summary>
        /// Invoked before kicking a <see cref="API.Features.Player"/> from the server.
        /// </summary>
        public static Event<KickingEventArgs> Kicking { get; set; } = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> has been kicked from the server.
        /// </summary>
        public static Event<KickedEventArgs> Kicked { get; set; } = new();

        /// <summary>
        /// Invoked before banning a <see cref="API.Features.Player"/> from the server.
        /// </summary>
        public static Event<BanningEventArgs> Banning { get; set; } = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> has been banned from the server.
        /// </summary>
        public static Event<BannedEventArgs> Banned { get; set; } = new();

        /// <summary>
        /// Invoked before using an <see cref="API.Features.Items.Item"/>.
        /// </summary>
        public static event CustomEventHandler<UsingItemEventArgs> UsingItem;

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> uses an <see cref="API.Features.Items.Usable"/>.
        /// </summary>
        public static event CustomEventHandler<UsingItemCompletedEventArgs> UsingItemCompleted;

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> uses an <see cref="API.Features.Items.Usable"/>.
        /// </summary>
        /// <remarks>
        /// Invoked after <see cref="UsingItem"/>, if a player's class has
        /// changed during their health increase, won't fire.
        /// </remarks>
        public static Event<UsedItemEventArgs> UsedItem { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> has stopped the use of a <see cref="API.Features.Items.Usable"/>.
        /// </summary>
        public static Event<CancellingItemUseEventArgs> CancellingItemUse { get; set; } = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> has stopped the use of a <see cref="API.Features.Items.Usable"/>.
        /// </summary>
        public static event CustomEventHandler<CancelledItemUseEventArgs> CancelledItemUse;

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> interacted with something.
        /// </summary>
        public static Event<InteractedEventArgs> Interacted { get; set; } = new();

        /// <summary>
        /// Invoked before spawning a <see cref="API.Features.Player"/> <see cref="API.Features.Ragdoll"/>.
        /// </summary>
        public static Event<SpawningRagdollEventArgs> SpawningRagdoll { get; set; } = new();

        /// <summary>
        /// Invoked before activating the warhead panel.
        /// </summary>
        public static Event<ActivatingWarheadPanelEventArgs> ActivatingWarheadPanel { get; set; } = new();

        /// <summary>
        /// Invoked before activating a workstation.
        /// </summary>
        public static Event<ActivatingWorkstationEventArgs> ActivatingWorkstation { get; set; } = new();

        /// <summary>
        /// Invoked before deactivating a workstation.
        /// </summary>
        public static Event<DeactivatingWorkstationEventArgs> DeactivatingWorkstation { get; set; } = new();

        /// <summary>
        /// Invoked before using an <see cref="API.Features.Items.Item"/>.
        /// </summary>
        public static Event<UsingItemEventArgs> UsingItem { get; set; } = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> has joined the server.
        /// </summary>
        public static Event<JoinedEventArgs> Joined { get; set; } = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> has been verified.
        /// </summary>
        public static Event<VerifiedEventArgs> Verified { get; set; } = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> has left the server.
        /// </summary>
        public static Event<LeftEventArgs> Left { get; set; } = new();

        /// <summary>
        /// Invoked before destroying a <see cref="API.Features.Player"/>.
        /// </summary>
        public static Event<DestroyingEventArgs> Destroying { get; set; } = new();

        /// <summary>
        /// Invoked before hurting a <see cref="API.Features.Player"/>.
        /// </summary>
        public static Event<HurtingEventArgs> Hurting { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> dies.
        /// </summary>
        public static Event<DyingEventArgs> Dying { get; set; } = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> died.
        /// </summary>
        public static Event<DiedEventArgs> Died { get; set; } = new();

        /// <summary>
        /// Invoked before changing a <see cref="API.Features.Player"/> role.
        /// </summary>
        /// <remarks>If <see cref="ChangingRoleEventArgs.IsAllowed"/> is set to <see langword="false"/> when Escape is <see langword="true"/>, tickets will still be given to the escapee's team even though they will 'fail' to escape. Use <see cref="Escaping"/> to block escapes instead.</remarks>
        public static Event<ChangingRoleEventArgs> ChangingRole { get; set; } = new();

        /// <summary>
        /// Invoked afer throwing an <see cref="API.Features.Items.Throwable"/>.
        /// </summary>
        public static Event<ThrownProjectileEventArgs> ThrownProjectile { get; set; } = new();

        /// <summary>
        /// Invoked before receving a throwing request an <see cref="API.Features.Items.Throwable"/>.
        /// </summary>
        public static Event<ThrowingRequestEventArgs> ThrowingRequest { get; set; } = new();

        /// <summary>
        /// Invoked before dropping an <see cref="API.Features.Items.Item"/>.
        /// </summary>
        public static Event<DroppingItemEventArgs> DroppingItem { get; set; } = new();

        /// <summary>
        /// Invoked before dropping a null <see cref="API.Features.Items.Item"/>.
        /// </summary>
        public static Event<DroppingNothingEventArgs> DroppingNothing { get; set; } = new();

        /// <summary>
        /// Invoked before picking up an <see cref="API.Features.Items.Item"/>.
        /// </summary>
        public static Event<PickingUpItemEventArgs> PickingUpItem { get; set; } = new();

        /// <summary>
        /// Invoked before handcuffing a <see cref="API.Features.Player"/>.
        /// </summary>
        public static Event<HandcuffingEventArgs> Handcuffing { get; set; } = new();

        /// <summary>
        /// Invoked before freeing a handcuffed <see cref="API.Features.Player"/>.
        /// </summary>
        public static Event<RemovingHandcuffsEventArgs> RemovingHandcuffs { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> escapes.
        /// </summary>
        public static Event<EscapingEventArgs> Escaping { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> begins speaking to the intercom.
        /// </summary>
        public static Event<IntercomSpeakingEventArgs> IntercomSpeaking { get; set; } = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> gets shot.
        /// </summary>
        public static Event<ShotEventArgs> Shot { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> shoots a weapon.
        /// </summary>
        public static Event<ShootingEventArgs> Shooting { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> enters the pocket dimension.
        /// </summary>
        public static Event<EnteringPocketDimensionEventArgs> EnteringPocketDimension { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> escapes the pocket dimension.
        /// </summary>
        public static Event<EscapingPocketDimensionEventArgs> EscapingPocketDimension { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> fails to escape the pocket dimension.
        /// </summary>
        public static Event<FailingEscapePocketDimensionEventArgs> FailingEscapePocketDimension { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> enters killer collision.
        /// </summary>
        public static Event<EnteringKillerCollisionEventArgs> EnteringKillerCollision { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> reloads a weapon.
        /// </summary>
        public static Event<ReloadingWeaponEventArgs> ReloadingWeapon { get; set; } = new();

        /// <summary>
        /// Invoked before spawning a <see cref="API.Features.Player"/>.
        /// </summary>
        public static Event<SpawningEventArgs> Spawning { get; set; } = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> has spawned.
        /// </summary>
        public static Event<SpawnedEventArgs> Spawned { get; set; } = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> held <see cref="API.Features.Items.Item"/> changes.
        /// </summary>
        public static Event<ChangedItemEventArgs> ChangedItem { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> held <see cref="API.Features.Items.Item"/> changes.
        /// </summary>
        public static Event<ChangingItemEventArgs> ChangingItem { get; set; } = new();

        /// <summary>
        /// Invoked before changing a <see cref="API.Features.Player"/> group.
        /// </summary>
        public static Event<ChangingGroupEventArgs> ChangingGroup { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> interacts with a door.
        /// </summary>
        public static Event<InteractingDoorEventArgs> InteractingDoor { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> interacts with an elevator.
        /// </summary>
        public static Event<InteractingElevatorEventArgs> InteractingElevator { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> interacts with a locker.
        /// </summary>
        public static Event<InteractingLockerEventArgs> InteractingLocker { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> triggers a tesla gate.
        /// </summary>
        public static Event<TriggeringTeslaEventArgs> TriggeringTesla { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> unlocks a generator.
        /// </summary>
        public static Event<UnlockingGeneratorEventArgs> UnlockingGenerator { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> opens a generator.
        /// </summary>
        public static Event<OpeningGeneratorEventArgs> OpeningGenerator { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> closes a generator.
        /// </summary>
        public static Event<ClosingGeneratorEventArgs> ClosingGenerator { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> turns on the generator by switching lever.
        /// </summary>
        public static Event<ActivatingGeneratorEventArgs> ActivatingGenerator { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> turns off the generator by switching lever.
        /// </summary>
        public static Event<StoppingGeneratorEventArgs> StoppingGenerator { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> receives a status effect.
        /// </summary>
        public static Event<ReceivingEffectEventArgs> ReceivingEffect { get; set; } = new();

        /// <summary>
        /// Invoked before muting a user.
        /// </summary>
        public static Event<IssuingMuteEventArgs> IssuingMute { get; set; } = new();

        /// <summary>
        /// Invoked before unmuting a user.
        /// </summary>
        public static Event<RevokingMuteEventArgs> RevokingMute { get; set; } = new();

        /// <summary>
        /// Invoked before a user's radio battery charge is changed.
        /// </summary>
        public static Event<UsingRadioBatteryEventArgs> UsingRadioBattery { get; set; } = new();

        /// <summary>
        /// Invoked before a user's radio preset is changed.
        /// </summary>
        public static Event<ChangingRadioPresetEventArgs> ChangingRadioPreset { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> MicroHID state is changed.
        /// </summary>
        public static Event<ChangingMicroHIDStateEventArgs> ChangingMicroHIDState { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> MicroHID energy is changed.
        /// </summary>
        public static Event<UsingMicroHIDEnergyEventArgs> UsingMicroHIDEnergy { get; set; } = new();

        /// <summary>
        /// Invoked before dropping ammo.
        /// </summary>
        public static Event<DroppingAmmoEventArgs> DroppingAmmo { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> interacts with a shooting target.
        /// </summary>
        public static Event<InteractingShootingTargetEventArgs> InteractingShootingTarget { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> damages a shooting target.
        /// </summary>
        public static Event<DamagingShootingTargetEventArgs> DamagingShootingTarget { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> flips a coin.
        /// </summary>
        public static Event<FlippingCoinEventArgs> FlippingCoin { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> toggles the flashlight.
        /// </summary>
        public static Event<TogglingFlashlightEventArgs> TogglingFlashlight { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> unloads a weapon.
        /// </summary>
        public static Event<UnloadingWeaponEventArgs> UnloadingWeapon { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> triggers an aim action.
        /// </summary>
        public static Event<AimingDownSightEventArgs> AimingDownSight { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> toggles the weapon's flashlight.
        /// </summary>
        public static Event<TogglingWeaponFlashlightEventArgs> TogglingWeaponFlashlight { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> dryfires a weapon.
        /// </summary>
        public static Event<DryfiringWeaponEventArgs> DryfiringWeapon { get; set; } = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> presses the voicechat key.
        /// </summary>
        public static Event<VoiceChattingEventArgs> VoiceChatting { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> makes noise.
        /// </summary>
        public static Event<MakingNoiseEventArgs> MakingNoise { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> jumps.
        /// </summary>
        public static Event<JumpingEventArgs> Jumping { get; set; } = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> lands.
        /// </summary>
        public static Event<LandingEventArgs> Landing { get; set; } = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> presses the transmission key.
        /// </summary>
        public static Event<TransmittingEventArgs> Transmitting { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> changes move state.
        /// </summary>
        public static Event<ChangingMoveStateEventArgs> ChangingMoveState { get; set; } = new();

        /// <summary>
        /// Invoked after a <see cref="API.Features.Player"/> changed spectated player.
        /// </summary>
        public static Event<ChangingSpectatedPlayerEventArgs> ChangingSpectatedPlayer { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> toggles the NoClip mode.
        /// </summary>
        public static Event<TogglingNoClipEventArgs> TogglingNoClip { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> toggles overwatch.
        /// </summary>
        public static Event<TogglingOverwatchEventArgs> TogglingOverwatch { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> searches a Pickup.
        /// </summary>
        public static Event<SearchingPickupEventArgs> SearchingPickup { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> damage a Window.
        /// </summary> // TODO: DamagingWindow instead of PlayerDamageWindow
        public static Event<DamagingWindowEventArgs> PlayerDamageWindow { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> damage a Door.
        /// </summary>
        public static Event<DamagingDoorEventArgs> DamagingDoor { get; set; } = new();

        /// <summary>
        /// Invoked after a <see cref="T:Exiled.API.Features.Player" /> has an item added to their inventory.
        /// </summary>
        public static Event<ItemAddedEventArgs> ItemAdded { get; set; } = new();

        /// <summary>
        /// Invoked before KillPlayer is called.
        /// </summary>
        public static Event<KillingPlayerEventArgs> KillingPlayer { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/> enters in an environmental hazard.
        /// </summary>
        public static Event<EnteringEnvironmentalHazardEventArgs> EnteringEnvironmentalHazard { get; set; } = new();

        /// <summary>
        /// Invoked when a <see cref="API.Features.Player"/> stays on an environmental hazard.
        /// </summary>
        public static Event<StayingOnEnvironmentalHazardEventArgs> StayingOnEnvironmentalHazard { get; set; } = new();

        /// <summary>
        /// Invoked when a <see cref="API.Features.Player"/> exists from an environmental hazard.
        /// </summary>
        public static Event<ExitingEnvironmentalHazardEventArgs> ExitingEnvironmentalHazard { get; set; } = new();

        /// <summary>
        /// Invoked before a <see cref="API.Features.Player"/>'s nickname is changed.
        /// </summary>
        public static Event<ChangingNicknameEventArgs> ChangingNickname { get; set; } = new();

        /// <summary>
        /// Called before reserved slot is resolved for a <see cref="API.Features.Player"/>.
        /// </summary>
        /// <param name="ev">The <see cref="ReservedSlotsCheckEventArgs"/> instance.</param>
        public static void OnReservedSlot(ReservedSlotsCheckEventArgs ev) => ReservedSlot.InvokeSafely(ev);

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
        /// Called before using a usable item.
        /// </summary>
        /// <param name="ev">The <see cref="UsingItemEventArgs"/> instance.</param>
        public static void OnUsingItem(UsingItemEventArgs ev) => UsingItem.InvokeSafely(ev);

        /// <summary>
        /// Called before completed using of a usable item.
        /// </summary>
        /// <param name="ev">The <see cref="UsingItemEventArgs"/> instance.</param>
        public static void OnUsingItemCompleted(UsingItemCompletedEventArgs ev) => UsingItemCompleted.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> used a <see cref="API.Features.Items.Usable"/> item.
        /// </summary>
        /// <param name="ev">The <see cref="UsedItemEventArgs"/> instance.</param>
        public static void OnUsedItem(UsedItemEventArgs ev) => UsedItem.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> has stopped the use of a <see cref="API.Features.Items.Usable"/> item.
        /// </summary>
        /// <param name="ev">The <see cref="CancellingItemUseEventArgs"/> instance.</param>
        public static void OnCancellingItemUse(CancellingItemUseEventArgs ev) => CancellingItemUse.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> has stopped the use of a <see cref="API.Features.Items.Usable"/> item.
        /// </summary>
        /// <param name="ev">The <see cref="CancelledItemUseEventArgs"/> instance.</param>
        public static void OnCancelledItemUse(CancelledItemUseEventArgs ev) => CancelledItemUse.InvokeSafely(ev);

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
        /// Called after a <see cref="API.Features.Player"/> has left the server.
        /// </summary>
        /// <param name="ev">The <see cref="LeftEventArgs"/> instance.</param>
        public static void OnLeft(LeftEventArgs ev) => Left.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> died.
        /// </summary>
        /// <param name="ev">The <see cref="DiedEventArgs"/> instance.</param>
        public static void OnDied(DiedEventArgs ev) => Died.InvokeSafely(ev);

        /// <summary>
        /// Called before changing a <see cref="API.Features.Player"/> role.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingRoleEventArgs"/> instance.</param>
        /// <remarks>If <see cref="ChangingRoleEventArgs.IsAllowed"/> is set to <see langword="false"/> when Escape is <see langword="true"/>, tickets will still be given to the escapee's team even though they will 'fail' to escape. Use <see cref="Escaping"/> to block escapes instead.</remarks>
        public static void OnChangingRole(ChangingRoleEventArgs ev) => ChangingRole.InvokeSafely(ev);

        /// <summary>
        /// Called before throwing a grenade.
        /// </summary>
        /// <param name="ev">The <see cref="ThrownProjectileEventArgs"/> instance.</param>
        // TODO: rename that to OnThrownProjectile
        public static void OnThrowingProjectile(ThrownProjectileEventArgs ev) => ThrownProjectile.InvokeSafely(ev);

        /// <summary>
        /// Called before receving a throwing request.
        /// </summary>
        /// <param name="ev">The <see cref="ThrowingRequestEventArgs"/> instance.</param>
        public static void OnThrowingRequest(ThrowingRequestEventArgs ev) => ThrowingRequest.InvokeSafely(ev);

        /// <summary>
        /// Called before dropping an item.
        /// </summary>
        /// <param name="ev">The <see cref="DroppingItemEventArgs"/> instance.</param>
        public static void OnDroppingItem(DroppingItemEventArgs ev) => DroppingItem.InvokeSafely(ev);

        /// <summary>
        /// Called before dropping a null item.
        /// </summary>
        /// <param name="ev">The <see cref="DroppingNothingEventArgs"/> instance.</param>
        public static void OnDroppingNothing(DroppingNothingEventArgs ev) => DroppingNothing.InvokeSafely(ev);

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
        /// Called before a <see cref="API.Features.Player"/> enters killer collision.
        /// </summary>
        /// <param name="ev">The <see cref="EnteringKillerCollisionEventArgs"/> instance.</param>
        public static void OnEnteringKillerCollision(EnteringKillerCollisionEventArgs ev) => EnteringKillerCollision.InvokeSafely(ev);

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
        /// <param name="ev">The <see cref="SpawnedEventArgs"/> instance.</param>
        public static void OnSpawned(SpawnedEventArgs ev) => Spawned.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> held item changes.
        /// </summary>
        /// <param name="ev">The <see cref="ChangedItemEventArgs"/> instance.</param>
        public static void OnChangedItem(ChangedItemEventArgs ev) => ChangedItem.InvokeSafely(ev);

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
        /// Called before a <see cref="API.Features.Player"/> receives a status effect.
        /// </summary>
        /// <param name="ev">The <see cref="ReceivingEffectEventArgs"/> instance.</param>
        public static void OnReceivingEffect(ReceivingEffectEventArgs ev) => ReceivingEffect.InvokeSafely(ev);

        /// <summary>
        /// Called before a user's radio battery charge is changed.
        /// </summary>
        /// <param name="ev">The <see cref="UsingRadioBatteryEventArgs"/> instance.</param>
        public static void OnUsingRadioBattery(UsingRadioBatteryEventArgs ev) => UsingRadioBattery.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> MicroHID state is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingMicroHIDStateEventArgs"/> instance.</param>
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
        [Obsolete("No more used by base-game", true)]
        public static void OnProcessingHotkey(ProcessingHotkeyEventArgs ev) => ProcessingHotkey.InvokeSafely(ev);

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
        /// Called before a <see cref="API.Features.Player"/> toggles overwatch.
        /// </summary>
        /// <param name="ev">The <see cref="TogglingOverwatchEventArgs"/> instance.</param>
        public static void OnTogglingOverwatch(TogglingOverwatchEventArgs ev) => TogglingOverwatch.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> searches a Pickup.
        /// </summary>
        /// <param name="ev">The <see cref="SearchingPickupEventArgs"/> instance.</param>
        public static void OnSearchPickupRequest(SearchingPickupEventArgs ev) => SearchingPickup.InvokeSafely(ev);

        /// <summary>
        ///  Called before KillPlayer is called.
        /// </summary>
        /// <param name="ev">The <see cref="KillingPlayerEventArgs"/> event handler. </param>
        public static void OnKillPlayer(KillingPlayerEventArgs ev) => KillingPlayer.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="T:Exiled.API.Features.Player" /> has an item added to their inventory.
        /// </summary>
        /// <param name="referenceHub">The <see cref="ReferenceHub"/> the item was added to.</param>
        /// <param name="itemBase">The added <see cref="InventorySystem.Items.ItemBase"/>.</param>
        /// <param name="pickupBase">The <see cref="InventorySystem.Items.Pickups.ItemPickupBase"/> the <see cref="InventorySystem.Items.ItemBase"/> originated from, or <see langword="null"/> if the item was not picked up.</param>
        public static void OnItemAdded(ReferenceHub referenceHub, InventorySystem.Items.ItemBase itemBase, InventorySystem.Items.Pickups.ItemPickupBase pickupBase)
            => ItemAdded.InvokeSafely(new ItemAddedEventArgs(referenceHub, itemBase, pickupBase));

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

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> damage a window.
        /// </summary>
        /// <param name="ev">The <see cref="DamagingWindowEventArgs"/> instance. </param>
        public static void OnPlayerDamageWindow(DamagingWindowEventArgs ev) => PlayerDamageWindow.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> damage a window.
        /// </summary>
        /// <param name="ev">The <see cref="DamagingDoorEventArgs"/> instance. </param>
        public static void OnDamagingDoor(DamagingDoorEventArgs ev) => DamagingDoor.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> unlocks a generator.
        /// </summary>
        /// <param name="ev">The <see cref="UnlockingGeneratorEventArgs"/> instance. </param>
        public static void OnUnlockingGenerator(UnlockingGeneratorEventArgs ev) => UnlockingGenerator.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> opens a generator.
        /// </summary>
        /// <param name="ev">The <see cref="OpeningGeneratorEventArgs"/> instance. </param>
        public static void OnOpeningGenerator(OpeningGeneratorEventArgs ev) => OpeningGenerator.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> closes a generator.
        /// </summary>
        /// <param name="ev">The <see cref="ClosingGeneratorEventArgs"/> instance. </param>
        public static void OnClosingGenerator(ClosingGeneratorEventArgs ev) => ClosingGenerator.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> turns on the generator by switching lever.
        /// </summary>
        /// <param name="ev">The <see cref="ActivatingGeneratorEventArgs"/> instance. </param>
        public static void OnActivatingGenerator(ActivatingGeneratorEventArgs ev) => ActivatingGenerator.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> turns off the generator by switching lever.
        /// </summary>
        /// <param name="ev">The <see cref="StoppingGeneratorEventArgs"/> instance. </param>
        public static void OnStoppingGenerator(StoppingGeneratorEventArgs ev) => StoppingGenerator.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> interacts with a door.
        /// </summary>
        /// <param name="ev">The <see cref="InteractingDoorEventArgs"/> instance. </param>
        public static void OnInteractingDoor(InteractingDoorEventArgs ev) => InteractingDoor.InvokeSafely(ev);

        /// <summary>
        /// Called before dropping ammo.
        /// </summary>
        /// <param name="ev">The <see cref="DroppingAmmoEventArgs"/> instance. </param>
        public static void OnDroppingAmmo(DroppingAmmoEventArgs ev) => DroppingAmmo.InvokeSafely(ev);

        /// <summary>
        /// Called before muting a user.
        /// </summary>
        /// <param name="ev">The <see cref="IssuingMuteEventArgs"/> instance. </param>
        public static void OnIssuingMute(IssuingMuteEventArgs ev) => IssuingMute.InvokeSafely(ev);

        /// <summary>
        /// Called before unmuting a user.
        /// </summary>
        /// <param name="ev">The <see cref="RevokingMuteEventArgs"/> instance. </param>
        public static void OnRevokingMute(RevokingMuteEventArgs ev) => RevokingMute.InvokeSafely(ev);

        /// <summary>
        /// Called before a user's radio preset is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingRadioPresetEventArgs"/> instance. </param>
        public static void OnChangingRadioPreset(ChangingRadioPresetEventArgs ev) => ChangingRadioPreset.InvokeSafely(ev);

        /// <summary>
        /// Called before hurting a player.
        /// </summary>
        /// <param name="ev">The <see cref="HurtingEventArgs"/> instance. </param>
        public static void OnHurting(HurtingEventArgs ev) => Hurting.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="API.Features.Player"/> dies.
        /// </summary>
        /// <param name="ev">The <see cref="DyingEventArgs"/> instance. </param>
        public static void OnDying(DyingEventArgs ev) => Dying.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> has joined the server.
        /// </summary>
        /// <param name="ev">The <see cref="JoinedEventArgs"/> instance. </param>
        public static void OnJoined(JoinedEventArgs ev) => Joined.InvokeSafely(ev);

        /// <summary>
        /// Called after a <see cref="API.Features.Player"/> has been verified.
        /// </summary>
        /// <param name="ev">The <see cref="VerifiedEventArgs"/> instance. </param>
        public static void OnVerified(VerifiedEventArgs ev) => Verified.InvokeSafely(ev);

        /// <summary>
        /// Called before destroying a <see cref="API.Features.Player"/>.
        /// </summary>
        /// <param name="ev">The <see cref="DestroyingEventArgs"/> instance. </param>
        public static void OnDestroying(DestroyingEventArgs ev) => Destroying.InvokeSafely(ev);

        /// <summary>
        /// Called before a <see cref="Player"/>'s custom display name is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingNicknameEventArgs"/> instance.</param>
        public static void OnChangingNickname(ChangingNicknameEventArgs ev) => ChangingNickname.InvokeSafely(ev);

        /// <summary>
        /// Called before pre-authenticating a <see cref="API.Features.Player"/>.
        /// </summary>
        /// <param name="userId"><inheritdoc cref="PreAuthenticatingEventArgs.UserId"/></param>
        /// <param name="ipAddress"><inheritdoc cref="PreAuthenticatingEventArgs.IpAddress"/></param>
        /// <param name="expiration"><inheritdoc cref="PreAuthenticatingEventArgs.Expiration"/></param>
        /// <param name="flags"><inheritdoc cref="PreAuthenticatingEventArgs.Flags"/></param>
        /// <param name="country"><inheritdoc cref="PreAuthenticatingEventArgs.Country"/></param>
        /// <param name="signature"><inheritdoc cref="PreAuthenticatingEventArgs.Signature"/></param>
        /// <param name="request"><inheritdoc cref="PreAuthenticatingEventArgs.Request"/></param>
        /// <param name="readerStartPosition"><inheritdoc cref="PreAuthenticatingEventArgs.ReaderStartPosition"/></param>
        /// <returns>Returns the <see cref="PreauthCancellationData"/> instance.</returns>
        [PluginEvent(ServerEventType.PlayerPreauth)]
        public PreauthCancellationData OnPreAuthenticating(
            string userId,
            string ipAddress,
            long expiration,
            CentralAuthPreauthFlags flags,
            string country,
            byte[] signature,
            LiteNetLib.ConnectionRequest request,
            int readerStartPosition)
        {
            PreAuthenticatingEventArgs ev = new(userId, ipAddress, expiration, flags, country, signature, request, readerStartPosition);
            PreAuthenticating.InvokeSafely(ev);

            return ev.CachedPreauthData;
        }
    }
}