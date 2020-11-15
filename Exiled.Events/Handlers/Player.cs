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
    using Exiled.Events.Extensions;

    using static Exiled.Events.Events;

    /// <summary>
    /// Player related events.
    /// </summary>
    public static class Player
    {
        /// <summary>
        /// Invoked before authenticating a player.
        /// </summary>
        public static event CustomEventHandler<PreAuthenticatingEventArgs> PreAuthenticating;

        /// <summary>
        /// Invoked before kicking a player from the server.
        /// </summary>
        public static event CustomEventHandler<KickingEventArgs> Kicking;

        /// <summary>
        /// Invoked after a player has been kicked from the server.
        /// </summary>
        public static event CustomEventHandler<KickedEventArgs> Kicked;

        /// <summary>
        /// Invoked before banning a player from the server.
        /// </summary>
        public static event CustomEventHandler<BanningEventArgs> Banning;

        /// <summary>
        /// Invoked after a player has been banned from the server.
        /// </summary>
        public static event CustomEventHandler<BannedEventArgs> Banned;

        /// <summary>
        /// Invoked after a player uses a medical item.
        /// </summary>
        public static event CustomEventHandler<UsedMedicalItemEventArgs> MedicalItemUsed;

        /// <summary>
        /// Invoked after a player has stopped the use of a medical item.
        /// </summary>
        public static event CustomEventHandler<StoppingMedicalItemEventArgs> StoppingMedicalItem;

        /// <summary>
        /// Invoked after a player interacted with something.
        /// </summary>
        public static event CustomEventHandler<InteractedEventArgs> Interacted;

        /// <summary>
        /// Invoked before spawning a player's ragdoll.
        /// </summary>
        public static event CustomEventHandler<SpawningRagdollEventArgs> SpawningRagdoll;

        /// <summary>
        /// Invoked before activating the warhead panel.
        /// </summary>
        public static event CustomEventHandler<ActivatingWarheadPanelEventArgs> ActivatingWarheadPanel;

        /// <summary>
        /// Invoked before using a medical item.
        /// </summary>
        public static event CustomEventHandler<UsingMedicalItemEventArgs> UsingMedicalItem;

        /// <summary>
        /// Invoked after a player has joined the server.
        /// </summary>
        public static event CustomEventHandler<JoinedEventArgs> Joined;

        /// <summary>
        /// Invoked after a player has left the server.
        /// </summary>
        public static event CustomEventHandler<LeftEventArgs> Left;

        /// <summary>
        /// Invoked before hurting a player.
        /// </summary>
        public static event CustomEventHandler<HurtingEventArgs> Hurting;

        /// <summary>
        /// Invoked before a player dies.
        /// </summary>
        public static event CustomEventHandler<DyingEventArgs> Dying;

        /// <summary>
        /// Invoked after a player died.
        /// </summary>
        public static event CustomEventHandler<DiedEventArgs> Died;

        /// <summary>
        /// Invoked before changing a player's role.
        /// </summary>
        public static event CustomEventHandler<ChangingRoleEventArgs> ChangingRole;

        /// <summary>
        /// Invoked before throwing a grenade.
        /// </summary>
        public static event CustomEventHandler<ThrowingGrenadeEventArgs> ThrowingGrenade;

        /// <summary>
        /// Invoked before dropping an item.
        /// </summary>
        public static event CustomEventHandler<DroppingItemEventArgs> DroppingItem;

        /// <summary>
        /// Invoked after an item has been dropped.
        /// </summary>
        public static event CustomEventHandler<ItemDroppedEventArgs> ItemDropped;

        /// <summary>
        /// Invoked before picking up an item.
        /// </summary>
        public static event CustomEventHandler<PickingUpItemEventArgs> PickingUpItem;

        /// <summary>
        /// Invoked before a player interacts with SCP-330.
        /// </summary>
        [Obsolete("SCP-330 has been removed.", true)]
        public static event CustomEventHandler<PickingUpScp330EventArgs> PickingUpScp330;

        /// <summary>
        /// Invoked before handcuffing a player.
        /// </summary>
        public static event CustomEventHandler<HandcuffingEventArgs> Handcuffing;

        /// <summary>
        /// Invoked before freeing a handcuffed player.
        /// </summary>
        public static event CustomEventHandler<RemovingHandcuffsEventArgs> RemovingHandcuffs;

        /// <summary>
        /// Invoked before a player escapes.
        /// </summary>
        public static event CustomEventHandler<EscapingEventArgs> Escaping;

        /// <summary>
        /// Invoked before a player begins speaking to the intercom.
        /// </summary>
        public static event CustomEventHandler<IntercomSpeakingEventArgs> IntercomSpeaking;

        /// <summary>
        /// Invoked after a player shoots a weapon.
        /// </summary>
        public static event CustomEventHandler<ShotEventArgs> Shot;

        /// <summary>
        /// Invoked before a player shoots a weapon.
        /// </summary>
        public static event CustomEventHandler<ShootingEventArgs> Shooting;

        /// <summary>
        /// Invoked before a player enters the pocket dimension.
        /// </summary>
        public static event CustomEventHandler<EnteringPocketDimensionEventArgs> EnteringPocketDimension;

        /// <summary>
        /// Invoked before a player escapes the pocket dimension.
        /// </summary>
        public static event CustomEventHandler<EscapingPocketDimensionEventArgs> EscapingPocketDimension;

        /// <summary>
        /// Invoked before a player fails to escape the pocket dimension.
        /// </summary>
        public static event CustomEventHandler<FailingEscapePocketDimensionEventArgs> FailingEscapePocketDimension;

        /// <summary>
        /// Invoked before a player reloads a weapon.
        /// </summary>
        public static event CustomEventHandler<ReloadingWeaponEventArgs> ReloadingWeapon;

        /// <summary>
        /// Invoked before spawning a player.
        /// </summary>
        public static event CustomEventHandler<SpawningEventArgs> Spawning;

        /// <summary>
        /// Invoked before a player enters the femur breaker.
        /// </summary>
        public static event CustomEventHandler<EnteringFemurBreakerEventArgs> EnteringFemurBreaker;

        /// <summary>
        /// Invoked before syncing player's data.
        /// </summary>
        public static event CustomEventHandler<SyncingDataEventArgs> SyncingData;

        /// <summary>
        /// Invoked before a player's held item changes.
        /// </summary>
        public static event CustomEventHandler<ChangingItemEventArgs> ChangingItem;

        /// <summary>
        /// Invoked before changing a player's group.
        /// </summary>
        public static event CustomEventHandler<ChangingGroupEventArgs> ChangingGroup;

        /// <summary>
        /// Invoked before a player interacts with a door.
        /// </summary>
        public static event CustomEventHandler<InteractingDoorEventArgs> InteractingDoor;

        /// <summary>
        /// Invoked before a player interacts with an elevator.
        /// </summary>
        public static event CustomEventHandler<InteractingElevatorEventArgs> InteractingElevator;

        /// <summary>
        /// Invoked before a player interacts with a locker.
        /// </summary>
        public static event CustomEventHandler<InteractingLockerEventArgs> InteractingLocker;

        /// <summary>
        /// Invoked before a player triggers a tesla gate.
        /// </summary>
        public static event CustomEventHandler<TriggeringTeslaEventArgs> TriggeringTesla;

        /// <summary>
        /// Invoked before a player unlocks a generator.
        /// </summary>
        public static event CustomEventHandler<UnlockingGeneratorEventArgs> UnlockingGenerator;

        /// <summary>
        /// Invoked before a player opens a generator.
        /// </summary>
        public static event CustomEventHandler<OpeningGeneratorEventArgs> OpeningGenerator;

        /// <summary>
        /// Invoked before a player closes a generator.
        /// </summary>
        public static event CustomEventHandler<ClosingGeneratorEventArgs> ClosingGenerator;

        /// <summary>
        /// Invoked before a player inserts a workstation tablet into a generator.
        /// </summary>
        public static event CustomEventHandler<InsertingGeneratorTabletEventArgs> InsertingGeneratorTablet;

        /// <summary>
        /// Invoked before a player ejects the workstation tablet out of a generator.
        /// </summary>
        public static event CustomEventHandler<EjectingGeneratorTabletEventArgs> EjectingGeneratorTablet;

        /// <summary>
        /// Invoked before a player receives a status effect.
        /// </summary>
        public static event CustomEventHandler<ReceivingEffectEventArgs> ReceivingEffect;

        /// <summary>
        /// Invoked before a workstation is activated.
        /// </summary>
        public static event CustomEventHandler<ActivatingWorkstationEventArgs> ActivatingWorkstation;

        /// <summary>
        /// Invoked before a workstation is deactivated.
        /// </summary>
        public static event CustomEventHandler<DeactivatingWorkstationEventArgs> DeactivatingWorkstation;

        /// <summary>
        /// Invoked before an user's mute status is changed.
        /// </summary>
        public static event CustomEventHandler<ChangingMuteStatusEventArgs> ChangingMuteStatus;

        /// <summary>
        /// Invoked before an user's intercom mute status is changed.
        /// </summary>
        public static event CustomEventHandler<ChangingIntercomMuteStatusEventArgs> ChangingIntercomMuteStatus;

        /// <summary>
        /// Called before pre-authenticating a player.
        /// </summary>
        /// <param name="ev">The <see cref="PreAuthenticatingEventArgs"/> instance.</param>
        public static void OnPreAuthenticating(PreAuthenticatingEventArgs ev) => PreAuthenticating.InvokeSafely(ev);

        /// <summary>
        /// Called before kicking a player from the server.
        /// </summary>
        /// <param name="ev">The <see cref="KickingEventArgs"/> instance.</param>
        public static void OnKicking(KickingEventArgs ev) => Kicking.InvokeSafely(ev);

        /// <summary>
        /// Called after a player has been kicked from the server.
        /// </summary>
        /// <param name="ev">The <see cref="KickedEventArgs"/> instance.</param>
        public static void OnKicked(KickedEventArgs ev) => Kicked.InvokeSafely(ev);

        /// <summary>
        /// Called before banning a player from the server.
        /// </summary>
        /// <param name="ev">The <see cref="BanningEventArgs"/> instance.</param>
        public static void OnBanning(BanningEventArgs ev) => Banning.InvokeSafely(ev);

        /// <summary>
        /// Called after a player has been banned from the server.
        /// </summary>
        /// <param name="ev">The <see cref="BannedEventArgs"/> instance.</param>
        public static void OnBanned(BannedEventArgs ev) => Banned.InvokeSafely(ev);

        /// <summary>
        /// Called after a player used a medical item.
        /// </summary>
        /// <param name="ev">The <see cref="MedicalItemUsed"/> instance.</param>
        public static void OnMedicalItemUsed(UsedMedicalItemEventArgs ev) => MedicalItemUsed.InvokeSafely(ev);

        /// <summary>
        /// Called after a player has stopped the use of a medical item.
        /// </summary>
        /// <param name="ev">The <see cref="StoppingMedicalItemEventArgs"/> instance.</param>
        public static void OnStoppingMedicalItem(StoppingMedicalItemEventArgs ev) => StoppingMedicalItem.InvokeSafely(ev);

        /// <summary>
        /// Called after a player interacted with something.
        /// </summary>
        /// <param name="ev">The <see cref="InteractedEventArgs"/> instance.</param>
        public static void OnInteracted(InteractedEventArgs ev) => Interacted.InvokeSafely(ev);

        /// <summary>
        /// Called before spawning a player's ragdoll.
        /// </summary>
        /// <param name="ev">The <see cref="SpawningRagdollEventArgs"/> instance.</param>
        public static void OnSpawningRagdoll(SpawningRagdollEventArgs ev) => SpawningRagdoll.InvokeSafely(ev);

        /// <summary>
        /// Called before activating the warhead panel.
        /// </summary>
        /// <param name="ev">The <see cref="ActivatingWarheadPanelEventArgs"/> instance.</param>
        public static void OnActivatingWarheadPanel(ActivatingWarheadPanelEventArgs ev) => ActivatingWarheadPanel.InvokeSafely(ev);

        /// <summary>
        /// Called before using a medical item.
        /// </summary>
        /// <param name="ev">The <see cref="UsingMedicalItemEventArgs"/> instance.</param>
        public static void OnUsingMedicalItem(UsingMedicalItemEventArgs ev) => UsingMedicalItem.InvokeSafely(ev);

        /// <summary>
        /// Called after a player has joined the server.
        /// </summary>
        /// <param name="ev">The <see cref="JoinedEventArgs"/> instance.</param>
        public static void OnJoined(JoinedEventArgs ev) => Joined.InvokeSafely(ev);

        /// <summary>
        /// Called after a player has left the server.
        /// </summary>
        /// <param name="ev">The <see cref="LeftEventArgs"/> instance.</param>
        public static void OnLeft(LeftEventArgs ev) => Left.InvokeSafely(ev);

        /// <summary>
        /// Called before hurting a player.
        /// </summary>
        /// <param name="ev">The <see cref="HurtingEventArgs"/> instance.</param>
        public static void OnHurting(HurtingEventArgs ev) => Hurting.InvokeSafely(ev);

        /// <summary>
        /// Called before a player dies.
        /// </summary>
        /// <param name="ev"><see cref="DyingEventArgs"/> instance.</param>
        public static void OnDying(DyingEventArgs ev) => Dying.InvokeSafely(ev);

        /// <summary>
        /// Called after a player died.
        /// </summary>
        /// <param name="ev">The <see cref="DiedEventArgs"/> instance.</param>
        public static void OnDied(DiedEventArgs ev) => Died.InvokeSafely(ev);

        /// <summary>
        /// Called before changing a player's role.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingRoleEventArgs"/> instance.</param>
        public static void OnChangingRole(ChangingRoleEventArgs ev) => ChangingRole.InvokeSafely(ev);

        /// <summary>
        /// Called before throwing a grenade.
        /// </summary>
        /// <param name="ev">The <see cref="ThrowingGrenadeEventArgs"/> instance.</param>
        public static void OnThrowingGrenade(ThrowingGrenadeEventArgs ev) => ThrowingGrenade.InvokeSafely(ev);

        /// <summary>
        /// Called before dropping an item.
        /// </summary>
        /// <param name="ev">The <see cref="DroppingItemEventArgs"/> instance.</param>
        public static void OnDroppingItem(DroppingItemEventArgs ev) => DroppingItem.InvokeSafely(ev);

        /// <summary>
        /// Called after a player drops an item.
        /// </summary>
        /// <param name="ev">The <see cref="ItemDroppedEventArgs"/> instance.</param>
        public static void OnItemDropped(ItemDroppedEventArgs ev) => ItemDropped.InvokeSafely(ev);

        /// <summary>
        /// Called before a user picks up an item.
        /// </summary>
        /// <param name="ev">The <see cref="PickingUpItemEventArgs"/> instance.</param>
        public static void OnPickingUpItem(PickingUpItemEventArgs ev) => PickingUpItem.InvokeSafely(ev);

        /// <summary>
        /// Called before a player picks up an item.
        /// </summary>
        /// <param name="ev">The <see cref="PickingUpScp330EventArgs"/> instance.</param>
        public static void OnPickingUpScp330(PickingUpScp330EventArgs ev) => PickingUpScp330.InvokeSafely(ev);

        /// <summary>
        /// Called before handcuffing a player.
        /// </summary>
        /// <param name="ev">The <see cref="HandcuffingEventArgs"/> instance.</param>
        public static void OnHandcuffing(HandcuffingEventArgs ev) => Handcuffing.InvokeSafely(ev);

        /// <summary>
        /// Called before freeing a handcuffed player.
        /// </summary>
        /// <param name="ev">The <see cref="RemovingHandcuffsEventArgs"/> instance.</param>
        public static void OnRemovingHandcuffs(RemovingHandcuffsEventArgs ev) => RemovingHandcuffs.InvokeSafely(ev);

        /// <summary>
        /// Called before a player escapes.
        /// </summary>
        /// <param name="ev">The <see cref="EscapingEventArgs"/> instance.</param>
        public static void OnEscaping(EscapingEventArgs ev) => Escaping.InvokeSafely(ev);

        /// <summary>
        /// Called before a player begins speaking to the intercom.
        /// </summary>
        /// <param name="ev">The <see cref="IntercomSpeakingEventArgs"/> instance.</param>
        public static void OnIntercomSpeaking(IntercomSpeakingEventArgs ev) => IntercomSpeaking.InvokeSafely(ev);

        /// <summary>
        /// Called after a player shoots a weapon.
        /// </summary>
        /// <param name="ev">The <see cref="ShotEventArgs"/> instance.</param>
        public static void OnShot(ShotEventArgs ev) => Shot.InvokeSafely(ev);

        /// <summary>
        /// Called before a player shoots a weapon.
        /// </summary>
        /// <param name="ev">The <see cref="ShootingEventArgs"/> instance.</param>
        public static void OnShooting(ShootingEventArgs ev) => Shooting.InvokeSafely(ev);

        /// <summary>
        /// Called before a player enters the pocket dimension.
        /// </summary>
        /// <param name="ev">The <see cref="EnteringPocketDimensionEventArgs"/> instance.</param>
        public static void OnEnteringPocketDimension(EnteringPocketDimensionEventArgs ev) => EnteringPocketDimension.InvokeSafely(ev);

        /// <summary>
        /// Called before a player escapes the pocket dimension.
        /// </summary>
        /// <param name="ev">The <see cref="EscapingPocketDimensionEventArgs"/> instance.</param>
        public static void OnEscapingPocketDimension(EscapingPocketDimensionEventArgs ev) => EscapingPocketDimension.InvokeSafely(ev);

        /// <summary>
        /// Called before a player fails to escape the pocket dimension.
        /// </summary>
        /// <param name="ev">The <see cref="FailingEscapePocketDimensionEventArgs"/> instance.</param>
        public static void OnFailingEscapePocketDimension(FailingEscapePocketDimensionEventArgs ev) => FailingEscapePocketDimension.InvokeSafely(ev);

        /// <summary>
        /// Called before a player reloads a weapon.
        /// </summary>
        /// <param name="ev">The <see cref="ReloadingWeaponEventArgs"/> instance.</param>
        public static void OnReloadingWeapon(ReloadingWeaponEventArgs ev) => ReloadingWeapon.InvokeSafely(ev);

        /// <summary>
        /// Called before spawning a player.
        /// </summary>
        /// <param name="ev">The <see cref="SpawningEventArgs"/> instance.</param>
        public static void OnSpawning(SpawningEventArgs ev) => Spawning.InvokeSafely(ev);

        /// <summary>
        /// Called before a player enters the femur breaker.
        /// </summary>
        /// <param name="ev">The <see cref="EnteringFemurBreakerEventArgs"/> instance.</param>
        public static void OnEnteringFemurBreaker(EnteringFemurBreakerEventArgs ev) => EnteringFemurBreaker.InvokeSafely(ev);

        /// <summary>
        /// Called before syncing player's data.
        /// </summary>
        /// <param name="ev">The <see cref="SyncingDataEventArgs"/> instance.</param>
        public static void OnSyncingData(SyncingDataEventArgs ev) => SyncingData.InvokeSafely(ev);

        /// <summary>
        /// Called before a player's held item changes.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingItemEventArgs"/> instance.</param>
        public static void OnChangingItem(ChangingItemEventArgs ev) => ChangingItem.InvokeSafely(ev);

        /// <summary>
        /// Called before changing a player's group.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingGroupEventArgs"/> instance.</param>
        public static void OnChangingGroup(ChangingGroupEventArgs ev) => ChangingGroup.InvokeSafely(ev);

        /// <summary>
        /// Called before a player interacts with a door.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingDecalEventArgs"/> instance.</param>
        public static void OnInteractingDoor(InteractingDoorEventArgs ev) => InteractingDoor.InvokeSafely(ev);

        /// <summary>
        /// Called before a player interacts with an elevator.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingDecalEventArgs"/> instance.</param>
        public static void OnInteractingElevator(InteractingElevatorEventArgs ev) => InteractingElevator.InvokeSafely(ev);

        /// <summary>
        /// Called before a player interacts with a locker.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingDecalEventArgs"/> instance.</param>
        public static void OnInteractingLocker(InteractingLockerEventArgs ev) => InteractingLocker.InvokeSafely(ev);

        /// <summary>
        /// Called before a player triggers a tesla.
        /// </summary>
        /// <param name="ev">The <see cref="TriggeringTeslaEventArgs"/> instance.</param>
        public static void OnTriggeringTesla(TriggeringTeslaEventArgs ev) => TriggeringTesla.InvokeSafely(ev);

        /// <summary>
        /// Called before a player unlocks a generator.
        /// </summary>
        /// <param name="ev">The <see cref="UnlockingGeneratorEventArgs"/> instance.</param>
        public static void OnUnlockingGenerator(UnlockingGeneratorEventArgs ev) => UnlockingGenerator.InvokeSafely(ev);

        /// <summary>
        /// Called before a player opens a generator.
        /// </summary>
        /// <param name="ev">The <see cref="OpeningGeneratorEventArgs"/> instance.</param>
        public static void OnOpeningGenerator(OpeningGeneratorEventArgs ev) => OpeningGenerator.InvokeSafely(ev);

        /// <summary>
        /// Called before a player closes a generator.
        /// </summary>
        /// <param name="ev">The <see cref="ClosingGeneratorEventArgs"/> instance.</param>
        public static void OnClosingGenerator(ClosingGeneratorEventArgs ev) => ClosingGenerator.InvokeSafely(ev);

        /// <summary>
        /// Called before a player inserts a workstation tablet into a generator.
        /// </summary>
        /// <param name="ev">The <see cref="InsertingGeneratorTabletEventArgs"/> instance.</param>
        public static void OnInsertingGeneratorTablet(InsertingGeneratorTabletEventArgs ev) => InsertingGeneratorTablet.InvokeSafely(ev);

        /// <summary>
        /// Called before a player ejects the workstation tablet out of a generator.
        /// </summary>
        /// <param name="ev">The <see cref="EjectingGeneratorTabletEventArgs"/> instance.</param>
        public static void OnEjectingGeneratorTablet(EjectingGeneratorTabletEventArgs ev) => EjectingGeneratorTablet.InvokeSafely(ev);

        /// <summary>
        /// Called before a player receives a status effect.
        /// </summary>
        /// <param name="ev">The <see cref="ReceivingEffectEventArgs"/> instance.</param>
        public static void OnReceivingEffect(ReceivingEffectEventArgs ev) => ReceivingEffect.InvokeSafely(ev);

        /// <summary>
        /// Called before a workstation is activated.
        /// </summary>
        /// <param name="ev">The <see cref="ActivatingWorkstationEventArgs"/> instance.</param>
        public static void OnActivatingWorkstation(ActivatingWorkstationEventArgs ev) => ActivatingWorkstation.InvokeSafely(ev);

        /// <summary>
        /// Called before a workstation is deactivated.
        /// </summary>
        /// <param name="ev">The <see cref="DeactivatingWorkstationEventArgs"/> instance.</param>
        public static void OnDeactivatingWorkstation(DeactivatingWorkstationEventArgs ev) => DeactivatingWorkstation.InvokeSafely(ev);

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
    }
}
