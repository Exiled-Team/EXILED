// -----------------------------------------------------------------------
// <copyright file="Player.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using System;

    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.Extensions;

    using static Events;

    /// <summary>
    ///     Player related events.
    /// </summary>
    public static class Player
    {
        /// <summary>
        ///     Invoked before authenticating a <see cref="API.Features.Player" />.
        /// </summary>
        public static event CustomEventHandler<PreAuthenticatingEventArgs> PreAuthenticating;

        /// <summary>
        ///     Invoked before kicking a <see cref="API.Features.Player" /> from the server.
        /// </summary>
        public static event CustomEventHandler<KickingEventArgs> Kicking;

        /// <summary>
        ///     Invoked after a <see cref="API.Features.Player" /> has been kicked from the server.
        /// </summary>
        public static event CustomEventHandler<KickedEventArgs> Kicked;

        /// <summary>
        ///     Invoked before banning a <see cref="API.Features.Player" /> from the server.
        /// </summary>
        public static event CustomEventHandler<BanningEventArgs> Banning;

        /// <summary>
        ///     Invoked after a <see cref="API.Features.Player" /> has been banned from the server.
        /// </summary>
        public static event CustomEventHandler<BannedEventArgs> Banned;

        /// <summary>
        ///     Invoked after a <see cref="API.Features.Player" /> interacted with something.
        /// </summary>
        public static event CustomEventHandler<InteractedEventArgs> Interacted;

        /// <summary>
        ///     Invoked before spawning a <see cref="API.Features.Player" /> <see cref="API.Features.Ragdoll" />.
        /// </summary>
        public static event CustomEventHandler<SpawningRagdollEventArgs> SpawningRagdoll;

        /// <summary>
        ///     Invoked before activating the warhead panel.
        /// </summary>
        public static event CustomEventHandler<ActivatingWarheadPanelEventArgs> ActivatingWarheadPanel;

        /// <summary>
        ///     Invoked before activating a workstation.
        /// </summary>
        public static event CustomEventHandler<ActivatingWorkstationEventArgs> ActivatingWorkstation;

        /// <summary>
        ///     Invoked before deactivating a workstation.
        /// </summary>
        public static event CustomEventHandler<DeactivatingWorkstationEventArgs> DeactivatingWorkstation;

        /// <summary>
        ///     Invoked after a <see cref="API.Features.Player" /> has joined the server.
        /// </summary>
        public static event CustomEventHandler<JoinedEventArgs> Joined;

        /// <summary>
        ///     Ivoked after a <see cref="API.Features.Player" /> has been verified.
        /// </summary>
        public static event CustomEventHandler<VerifiedEventArgs> Verified;

        /// <summary>
        ///     Invoked after a <see cref="API.Features.Player" /> has left the server.
        /// </summary>
        public static event CustomEventHandler<LeftEventArgs> Left;

        /// <summary>
        ///     Invoked before destroying a <see cref="API.Features.Player" />.
        /// </summary>
        public static event CustomEventHandler<DestroyingEventArgs> Destroying;

        /// <summary>
        ///     Invoked before hurting a <see cref="API.Features.Player" />.
        /// </summary>
        public static event CustomEventHandler<HurtingEventArgs> Hurting;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> dies.
        /// </summary>
        public static event CustomEventHandler<DyingEventArgs> Dying;

        /// <summary>
        ///     Invoked after a <see cref="API.Features.Player" /> died.
        /// </summary>
        public static event CustomEventHandler<DiedEventArgs> Died;

        /// <summary>
        ///     Invoked before changing a <see cref="API.Features.Player" /> role.
        /// </summary>
        /// <remarks>
        ///     If you set IsAllowed to <see langword="false" /> when Escape is <see langword="true" />, tickets will still be
        ///     given to the escapee's team even though they will 'fail' to escape. Use <see cref="Escaping" /> to block escapes
        ///     instead.
        /// </remarks>
        public static event CustomEventHandler<ChangingRoleEventArgs> ChangingRole;

        /// <summary>
        ///     Invoked before handcuffing a <see cref="API.Features.Player" />.
        /// </summary>
        public static event CustomEventHandler<HandcuffingEventArgs> Handcuffing;

        /// <summary>
        ///     Invoked before freeing a handcuffed <see cref="API.Features.Player" />.
        /// </summary>
        public static event CustomEventHandler<RemovingHandcuffsEventArgs> RemovingHandcuffs;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> escapes.
        /// </summary>
        public static event CustomEventHandler<EscapingEventArgs> Escaping;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> begins speaking to the intercom.
        /// </summary>
        public static event CustomEventHandler<IntercomSpeakingEventArgs> IntercomSpeaking;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> enters the pocket dimension.
        /// </summary>
        public static event CustomEventHandler<EnteringPocketDimensionEventArgs> EnteringPocketDimension;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> escapes the pocket dimension.
        /// </summary>
        public static event CustomEventHandler<EscapingPocketDimensionEventArgs> EscapingPocketDimension;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> fails to escape the pocket dimension.
        /// </summary>
        public static event CustomEventHandler<FailingEscapePocketDimensionEventArgs> FailingEscapePocketDimension;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> reloads a weapon.
        /// </summary>
        public static event CustomEventHandler<ReloadingWeaponEventArgs> ReloadingWeapon;

        /// <summary>
        ///     Invoked before spawning a <see cref="API.Features.Player" />.
        /// </summary>
        public static event CustomEventHandler<SpawningEventArgs> Spawning;

        /// <summary>
        ///     Invoked after a <see cref="API.Features.Player" /> has spawned.
        /// </summary>
        public static event CustomEventHandler<SpawnedEventArgs> Spawned;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> enters the femur breaker.
        /// </summary>
        public static event CustomEventHandler<EnteringFemurBreakerEventArgs> EnteringFemurBreaker;

        /// <summary>
        ///     Invoked before changing a <see cref="API.Features.Player" /> group.
        /// </summary>
        public static event CustomEventHandler<ChangingGroupEventArgs> ChangingGroup;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> interacts with a door.
        /// </summary>
        public static event CustomEventHandler<InteractingDoorEventArgs> InteractingDoor;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> interacts with an elevator.
        /// </summary>
        public static event CustomEventHandler<InteractingElevatorEventArgs> InteractingElevator;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> interacts with a locker.
        /// </summary>
        public static event CustomEventHandler<InteractingLockerEventArgs> InteractingLocker;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> triggers a tesla gate.
        /// </summary>
        public static event CustomEventHandler<TriggeringTeslaEventArgs> TriggeringTesla;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> unlocks a generator.
        /// </summary>
        public static event CustomEventHandler<UnlockingGeneratorEventArgs> UnlockingGenerator;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> opens a generator.
        /// </summary>
        public static event CustomEventHandler<OpeningGeneratorEventArgs> OpeningGenerator;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> closes a generator.
        /// </summary>
        public static event CustomEventHandler<ClosingGeneratorEventArgs> ClosingGenerator;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> turns on the generator by switching lever.
        /// </summary>
        public static event CustomEventHandler<ActivatingGeneratorEventArgs> ActivatingGenerator;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> turns off the generator by switching lever.
        /// </summary>
        public static event CustomEventHandler<StoppingGeneratorEventArgs> StoppingGenerator;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> receives a status effect.
        /// </summary>
        public static event CustomEventHandler<ReceivingEffectEventArgs> ReceivingEffect;

        /// <summary>
        ///     Invoked before an user's mute status is changed.
        /// </summary>
        public static event CustomEventHandler<ChangingMuteStatusEventArgs> ChangingMuteStatus;

        /// <summary>
        ///     Invoked before an user's intercom mute status is changed.
        /// </summary>
        public static event CustomEventHandler<ChangingIntercomMuteStatusEventArgs> ChangingIntercomMuteStatus;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> interacts with a shooting target.
        /// </summary>
        public static event CustomEventHandler<InteractingShootingTargetEventArgs> InteractingShootingTarget;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> damages a shooting target.
        /// </summary>
        public static event CustomEventHandler<DamagingShootingTargetEventArgs> DamagingShootingTarget;

        /// <summary>
        ///     Invoked after a <see cref="API.Features.Player" /> presses the voicechat key.
        /// </summary>
        public static event CustomEventHandler<VoiceChattingEventArgs> VoiceChatting;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> makes noise.
        /// </summary>
        public static event CustomEventHandler<MakingNoiseEventArgs> MakingNoise;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> jumps.
        /// </summary>
        public static event CustomEventHandler<JumpingEventArgs> Jumping;

        /// <summary>
        ///     Invoked after a <see cref="API.Features.Player" /> lands.
        /// </summary>
        public static event CustomEventHandler<LandingEventArgs> Landing;

        /// <summary>
        ///     Invoked after a <see cref="API.Features.Player" /> presses the transmission key.
        /// </summary>
        public static event CustomEventHandler<TransmittingEventArgs> Transmitting;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> changes move state.
        /// </summary>
        public static event CustomEventHandler<ChangingMoveStateEventArgs> ChangingMoveState;

        /// <summary>
        ///     Invoked after a <see cref="API.Features.Player" /> changed spectated player.
        /// </summary>
        public static event CustomEventHandler<ChangingSpectatedPlayerEventArgs> ChangingSpectatedPlayer;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> toggles the NoClip mode.
        /// </summary>
        public static event CustomEventHandler<TogglingNoClipEventArgs> TogglingNoClip;

        /// <summary>
        ///     Invoked before a <see cref="API.Features.Player" /> damage a Window.
        /// </summary>
        public static event CustomEventHandler<DamagingWindowEventArgs> PlayerDamageWindow;

        /// <summary>
        ///     Invoked before KillPlayer is called.
        /// </summary>
        public static event CustomEventHandler<KillingPlayerEventArgs> KillingPlayer;

        /// <summary>
        ///     Called before pre-authenticating a <see cref="API.Features.Player"/>.
        /// </summary>
        /// <param name="ev">The <see cref="PreAuthenticatingEventArgs" /> instance.</param>
        public static void OnPreAuthenticating(PreAuthenticatingEventArgs ev)
        {
            PreAuthenticating.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before kicking a <see cref="API.Features.Player" /> from the server.
        /// </summary>
        /// <param name="ev">The <see cref="KickingEventArgs" /> instance.</param>
        public static void OnKicking(KickingEventArgs ev)
        {
            Kicking.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called after a <see cref="API.Features.Player" /> has been kicked from the server.
        /// </summary>
        /// <param name="ev">The <see cref="KickedEventArgs" /> instance.</param>
        public static void OnKicked(KickedEventArgs ev)
        {
            Kicked.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before banning a <see cref="API.Features.Player" /> from the server.
        /// </summary>
        /// <param name="ev">The <see cref="BanningEventArgs" /> instance.</param>
        public static void OnBanning(BanningEventArgs ev)
        {
            Banning.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called after a player has been banned from the server.
        /// </summary>
        /// <param name="ev">The <see cref="BannedEventArgs" /> instance.</param>
        public static void OnBanned(BannedEventArgs ev)
        {
            Banned.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called after a <see cref="API.Features.Player" /> interacted with something.
        /// </summary>
        /// <param name="ev">The <see cref="InteractedEventArgs" /> instance.</param>
        public static void OnInteracted(InteractedEventArgs ev)
        {
            Interacted.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before spawning a <see cref="API.Features.Player" /> ragdoll.
        /// </summary>
        /// <param name="ev">The <see cref="SpawningRagdollEventArgs" /> instance.</param>
        public static void OnSpawningRagdoll(SpawningRagdollEventArgs ev)
        {
            SpawningRagdoll.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before activating the warhead panel.
        /// </summary>
        /// <param name="ev">The <see cref="ActivatingWarheadPanelEventArgs" /> instance.</param>
        public static void OnActivatingWarheadPanel(ActivatingWarheadPanelEventArgs ev)
        {
            ActivatingWarheadPanel.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before activating a workstation.
        /// </summary>
        /// <param name="ev">The <see cref="ActivatingWorkstation" /> instance.</param>
        public static void OnActivatingWorkstation(ActivatingWorkstationEventArgs ev)
        {
            ActivatingWorkstation.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before deactivating a workstation.
        /// </summary>
        /// <param name="ev">The <see cref="DeactivatingWorkstationEventArgs" /> instance.</param>
        public static void OnDeactivatingWorkstation(DeactivatingWorkstationEventArgs ev)
        {
            DeactivatingWorkstation.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called after a <see cref="API.Features.Player" /> has joined the server.
        /// </summary>
        /// <param name="ev">The <see cref="JoinedEventArgs" /> instance.</param>
        public static void OnJoined(JoinedEventArgs ev)
        {
            Joined.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called after a <see cref="API.Features.Player" /> has been verified.
        /// </summary>
        /// <param name="ev">The <see cref="VerifiedEventArgs" /> instance.</param>
        public static void OnVerified(VerifiedEventArgs ev)
        {
            Verified.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called after a <see cref="API.Features.Player" /> has left the server.
        /// </summary>
        /// <param name="ev">The <see cref="LeftEventArgs" /> instance.</param>
        public static void OnLeft(LeftEventArgs ev)
        {
            Left.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before destroying a <see cref="API.Features.Player" />.
        /// </summary>
        /// <param name="ev">The <see cref="DestroyingEventArgs" /> instance.</param>
        public static void OnDestroying(DestroyingEventArgs ev)
        {
            Destroying.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before hurting a player.
        /// </summary>
        /// <param name="ev">The <see cref="HurtingEventArgs" /> instance.</param>
        public static void OnHurting(HurtingEventArgs ev)
        {
            Hurting.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> dies.
        /// </summary>
        /// <param name="ev"><see cref="DyingEventArgs" /> instance.</param>
        public static void OnDying(DyingEventArgs ev)
        {
            Dying.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called after a <see cref="API.Features.Player" /> died.
        /// </summary>
        /// <param name="ev">The <see cref="DiedEventArgs" /> instance.</param>
        public static void OnDied(DiedEventArgs ev)
        {
            Died.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before changing a <see cref="API.Features.Player" /> role.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingRoleEventArgs" /> instance.</param>
        /// <remarks>
        ///     If you set IsAllowed to <see langword="false" /> when Escape is <see langword="true" />, tickets will still be
        ///     given to the escapee's team even though they will 'fail' to escape. Use <see cref="Escaping" /> to block escapes
        ///     instead.
        /// </remarks>
        public static void OnChangingRole(ChangingRoleEventArgs ev)
        {
            ChangingRole.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before handcuffing a <see cref="API.Features.Player" />.
        /// </summary>
        /// <param name="ev">The <see cref="HandcuffingEventArgs" /> instance.</param>
        public static void OnHandcuffing(HandcuffingEventArgs ev)
        {
            Handcuffing.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before freeing a handcuffed <see cref="API.Features.Player" />.
        /// </summary>
        /// <param name="ev">The <see cref="RemovingHandcuffsEventArgs" /> instance.</param>
        public static void OnRemovingHandcuffs(RemovingHandcuffsEventArgs ev)
        {
            RemovingHandcuffs.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> escapes.
        /// </summary>
        /// <param name="ev">The <see cref="EscapingEventArgs" /> instance.</param>
        public static void OnEscaping(EscapingEventArgs ev)
        {
            Escaping.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> begins speaking to the intercom.
        /// </summary>
        /// <param name="ev">The <see cref="IntercomSpeakingEventArgs" /> instance.</param>
        public static void OnIntercomSpeaking(IntercomSpeakingEventArgs ev)
        {
            IntercomSpeaking.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> enters the pocket dimension.
        /// </summary>
        /// <param name="ev">The <see cref="EnteringPocketDimensionEventArgs" /> instance.</param>
        public static void OnEnteringPocketDimension(EnteringPocketDimensionEventArgs ev)
        {
            EnteringPocketDimension.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> escapes the pocket dimension.
        /// </summary>
        /// <param name="ev">The <see cref="EscapingPocketDimensionEventArgs" /> instance.</param>
        public static void OnEscapingPocketDimension(EscapingPocketDimensionEventArgs ev)
        {
            EscapingPocketDimension.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> fails to escape the pocket dimension.
        /// </summary>
        /// <param name="ev">The <see cref="FailingEscapePocketDimensionEventArgs" /> instance.</param>
        public static void OnFailingEscapePocketDimension(FailingEscapePocketDimensionEventArgs ev)
        {
            FailingEscapePocketDimension.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> reloads a weapon.
        /// </summary>
        /// <param name="ev">The <see cref="ReloadingWeaponEventArgs" /> instance.</param>
        public static void OnReloadingWeapon(ReloadingWeaponEventArgs ev)
        {
            ReloadingWeapon.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before spawning a <see cref="API.Features.Player" />.
        /// </summary>
        /// <param name="ev">The <see cref="SpawningEventArgs" /> instance.</param>
        public static void OnSpawning(SpawningEventArgs ev)
        {
            Spawning.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called after a <see cref="API.Features.Player" /> has spawned.
        /// </summary>
        /// <param name="referenceHub">The <see cref="ReferenceHub" /> instance.</param>
        public static void OnSpawned(ReferenceHub referenceHub)
        {
            Spawned.InvokeSafely(new SpawnedEventArgs(referenceHub));
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> enters the femur breaker.
        /// </summary>
        /// <param name="ev">The <see cref="EnteringFemurBreakerEventArgs" /> instance.</param>
        public static void OnEnteringFemurBreaker(EnteringFemurBreakerEventArgs ev)
        {
            EnteringFemurBreaker.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before changing a <see cref="API.Features.Player" /> group.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingGroupEventArgs" /> instance.</param>
        public static void OnChangingGroup(ChangingGroupEventArgs ev)
        {
            ChangingGroup.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> interacts with a door.
        /// </summary>
        /// <param name="ev">The <see cref="InteractingDoorEventArgs" /> instance.</param>
        public static void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            InteractingDoor.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> interacts with an elevator.
        /// </summary>
        /// <param name="ev">The <see cref="InteractingElevatorEventArgs" /> instance.</param>
        public static void OnInteractingElevator(InteractingElevatorEventArgs ev)
        {
            InteractingElevator.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> interacts with a locker.
        /// </summary>
        /// <param name="ev">The <see cref="InteractingLockerEventArgs" /> instance.</param>
        public static void OnInteractingLocker(InteractingLockerEventArgs ev)
        {
            InteractingLocker.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> triggers a tesla.
        /// </summary>
        /// <param name="ev">The <see cref="TriggeringTeslaEventArgs" /> instance.</param>
        public static void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            TriggeringTesla.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> unlocks a generator.
        /// </summary>
        /// <param name="ev">The <see cref="UnlockingGeneratorEventArgs" /> instance.</param>
        public static void OnUnlockingGenerator(UnlockingGeneratorEventArgs ev)
        {
            UnlockingGenerator.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> opens a generator.
        /// </summary>
        /// <param name="ev">The <see cref="OpeningGeneratorEventArgs" /> instance.</param>
        public static void OnOpeningGenerator(OpeningGeneratorEventArgs ev)
        {
            OpeningGenerator.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> closes a generator.
        /// </summary>
        /// <param name="ev">The <see cref="ClosingGeneratorEventArgs" /> instance.</param>
        public static void OnClosingGenerator(ClosingGeneratorEventArgs ev)
        {
            ClosingGenerator.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> turns on the generator by switching lever.
        /// </summary>
        /// <param name="ev">The <see cref="ActivatingGeneratorEventArgs" /> instance.</param>
        public static void OnActivatingGenerator(ActivatingGeneratorEventArgs ev)
        {
            ActivatingGenerator.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> turns off the generator by switching lever.
        /// </summary>
        /// <param name="ev">The <see cref="StoppingGeneratorEventArgs" /> instance.</param>
        public static void OnStoppingGenerator(StoppingGeneratorEventArgs ev)
        {
            StoppingGenerator.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> receives a status effect.
        /// </summary>
        /// <param name="ev">The <see cref="ReceivingEffectEventArgs" /> instance.</param>
        public static void OnReceivingEffect(ReceivingEffectEventArgs ev)
        {
            ReceivingEffect.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before an user's mute status is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingMuteStatusEventArgs" /> instance.</param>
        public static void OnChangingMuteStatus(ChangingMuteStatusEventArgs ev)
        {
            ChangingMuteStatus.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before an user's intercom mute status is changed.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingIntercomMuteStatusEventArgs" /> instance.</param>
        public static void OnChangingIntercomMuteStatus(ChangingIntercomMuteStatusEventArgs ev)
        {
            ChangingIntercomMuteStatus.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> interacts with a shooting target.
        /// </summary>
        /// <param name="ev">The <see cref="InteractingShootingTargetEventArgs" /> instance.</param>
        public static void OnInteractingShootingTarget(InteractingShootingTargetEventArgs ev)
        {
            InteractingShootingTarget.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> damages a shooting target.
        /// </summary>
        /// <param name="ev">The <see cref="DamagingShootingTargetEventArgs" /> instance.</param>
        public static void OnDamagingShootingTarget(DamagingShootingTargetEventArgs ev)
        {
            DamagingShootingTarget.InvokeSafely(ev);
        }


        /// <summary>
        ///     Invoked after a <see cref="API.Features.Player" /> presses the voicechat key.
        /// </summary>
        /// <param name="ev">The <see cref="VoiceChattingEventArgs" /> instance.</param>
        public static void OnVoiceChatting(VoiceChattingEventArgs ev)
        {
            VoiceChatting.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> makes noise.
        /// </summary>
        /// <param name="ev">The <see cref="MakingNoiseEventArgs" /> instance.</param>
        public static void OnMakingNoise(MakingNoiseEventArgs ev)
        {
            MakingNoise.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> jumps.
        /// </summary>
        /// <param name="ev">The <see cref="JumpingEventArgs" /> instance.</param>
        public static void OnJumping(JumpingEventArgs ev)
        {
            Jumping.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called after a <see cref="API.Features.Player" /> lands.
        /// </summary>
        /// <param name="ev">The <see cref="LandingEventArgs" /> instance.</param>
        public static void OnLanding(LandingEventArgs ev)
        {
            Landing.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called after a <see cref="API.Features.Player" /> presses the transmission key.
        /// </summary>
        /// <param name="ev">The <see cref="TransmittingEventArgs" /> instance.</param>
        public static void OnTransmitting(TransmittingEventArgs ev)
        {
            Transmitting.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> changes move state.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingMoveStateEventArgs" /> instance.</param>
        public static void OnChangingMoveState(ChangingMoveStateEventArgs ev)
        {
            ChangingMoveState.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called after a <see cref="API.Features.Player" /> changes spectated player.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingSpectatedPlayerEventArgs" /> instance.</param>
        public static void OnChangingSpectatedPlayer(ChangingSpectatedPlayerEventArgs ev)
        {
            ChangingSpectatedPlayer.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> toggles the NoClip mode.
        /// </summary>
        /// <param name="ev">The <see cref="TogglingNoClipEventArgs" /> instance.</param>
        public static void OnTogglingNoClip(TogglingNoClipEventArgs ev)
        {
            TogglingNoClip.InvokeSafely(ev);
        }

        /// <summary>
        ///     Called before a <see cref="API.Features.Player" /> damage a window.
        /// </summary>
        /// <param name="ev">The <see cref="DamagingWindowEventArgs"/> instance.</param>
        public static void OnPlayerDamageWindow(DamagingWindowEventArgs ev) => PlayerDamageWindow.InvokeSafely(ev);

        /// <summary>
        ///  Called before KillPlayer is called.
        /// </summary>
        /// <param name="ev">The <see cref="KillingPlayerEventArgs"/> event handler. </param>
        public static void OnKillPlayer(KillingPlayerEventArgs ev) => KillingPlayer.InvokeSafely(ev);
    }
}
