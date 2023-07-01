// -----------------------------------------------------------------------
// <copyright file="Scp939.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
    using Exiled.Events.EventArgs.Scp939;
    using Extensions;

    using static Events;

    /// <summary>
    ///     Handles SCP-939 related events.
    /// </summary>
    public static class Scp939
    {
        /// <summary>
        ///     Invoked before SCP-939 changes its target focus.
        /// </summary>
        public static event CustomEventHandler<ChangingFocusEventArgs> ChangingFocus;

        /// <summary>
        ///     Invoked before SCP-939 uses its lunge ability.
        /// </summary>
        public static event CustomEventHandler<LungingEventArgs> Lunging;

        /// <summary>
        ///     Invoked before SCP-939 uses its amnestic cloud ability.
        /// </summary>
        public static event CustomEventHandler<PlacingAmnesticCloudEventArgs> PlacingAmnesticCloud;

        /// <summary>
        ///     Invoked before SCP-939 plays a stolen voice.
        /// </summary>
        public static event CustomEventHandler<PlayingVoiceEventArgs> PlayingVoice;

        /// <summary>
        ///     Invoked before SCP-939 will save Human voice.
        /// </summary>
        public static event CustomEventHandler<SavingVoiceEventArgs> SavingVoice;

        /// <summary>
        ///     Invoked before SCP-939 plays a sound effect.
        /// </summary>
        public static event CustomEventHandler<PlayingSoundEventArgs> PlayingSound;

        /// <summary>
        /// Invoked after SCP-939 attack.
        /// </summary>
        /// <remarks>This event is calling only when attack doesn't have target.</remarks>
        public static event CustomEventHandler<AttackedEventArgs> Attacked;

        /// <summary>
        ///     Called before SCP-939 changes its target focus.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingFocusEventArgs" /> instance.</param>
        public static void OnChangingFocus(ChangingFocusEventArgs ev) => ChangingFocus.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-939 uses its lunge ability.
        /// </summary>
        /// <param name="ev">The <see cref="LungingEventArgs" /> instance.</param>
        public static void OnLunging(LungingEventArgs ev) => Lunging.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-939 uses its amnestic cloud ability.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingAmnesticCloudEventArgs" /> instance.</param>
        public static void OnPlacingAmnesticCloud(PlacingAmnesticCloudEventArgs ev) => PlacingAmnesticCloud.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-939 plays a stolen voice.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingAmnesticCloudEventArgs" /> instance.</param>
        public static void OnPlayingVoice(PlayingVoiceEventArgs ev) => PlayingVoice.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-939 plays a stolen voice.
        /// </summary>
        /// <param name="ev">The <see cref="PlacingAmnesticCloudEventArgs" /> instance.</param>
        public static void OnSavingVoice(SavingVoiceEventArgs ev) => SavingVoice.InvokeSafely(ev);

        /// <summary>
        ///     Called before SCP-939 plays a sound.
        /// </summary>
        /// <param name="ev">The <see cref="PlayingSoundEventArgs"/> instance.</param>
        public static void OnPlayingSound(PlayingSoundEventArgs ev) => PlayingSound.InvokeSafely(ev);

        /// <summary>
        /// Called after SCP-939 attacks.
        /// </summary>
        /// <param name="ev">The <see cref="AttackedEventArgs"/> instance.</param>
        public static void OnAttacking(AttackedEventArgs ev) => Attacked.InvokeSafely(ev);
    }
}