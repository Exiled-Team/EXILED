// -----------------------------------------------------------------------
// <copyright file="Scp939.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers
{
#pragma warning disable SA1623 // Property summary documentation should match accessors

    using Exiled.Events.EventArgs.Scp939;
    using Exiled.Events.Features;

    /// <summary>
    ///     Handles SCP-939 related events.
    /// </summary>
    public static class Scp939
    {
        /// <summary>
        ///     Invoked before SCP-939 changes its target focus.
        /// </summary>
        public static Event<ChangingFocusEventArgs> ChangingFocus { get; set; } = new();

        /// <summary>
        ///     Invoked before SCP-939 uses its lunge ability.
        /// </summary>
        public static Event<LungingEventArgs> Lunging { get; set; } = new();

        /// <summary>
        ///     Invoked before SCP-939 uses its amnestic cloud ability.
        /// </summary>
        public static Event<PlacingAmnesticCloudEventArgs> PlacingAmnesticCloud { get; set; } = new();

        /// <summary>
        ///     Invoked before SCP-939 plays a stolen voice.
        /// </summary>
        public static Event<PlayingVoiceEventArgs> PlayingVoice { get; set; } = new();

        /// <summary>
        ///     Invoked before SCP-939 will save Human voice.
        /// </summary>
        public static Event<SavingVoiceEventArgs> SavingVoice { get; set; } = new();

        /// <summary>
        ///     Invoked before SCP-939 plays a sound effect.
        /// </summary>
        public static Event<PlayingSoundEventArgs> PlayingSound { get; set; } = new();

        /// <summary>
        /// Invoked after SCP-939 attack.
        /// </summary>
        /// <remarks>This event is calling only when attack doesn't have target.</remarks>
        public static Event<ClawedEventArgs> Clawed { get; set; } = new();

        /// <summary>
        /// Invoked when checking visibility for players.
        /// </summary>
        public static Event<ValidatingVisibilityEventArgs> ValidatedVisibility { get; set; } = new();

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
        /// <param name="ev">The <see cref="ClawedEventArgs"/> instance.</param>
        public static void OnClawed(ClawedEventArgs ev) => Clawed.InvokeSafely(ev);

        /// <summary>
        /// Called after visibility for player has been checked
        /// </summary>
        /// <param name="ev">The <see cref="ValidatingVisibilityEventArgs"/> instance.</param>
        public static void OnValidatedVisibility(ValidatingVisibilityEventArgs ev) => ValidatedVisibility.InvokeSafely(ev);
    }
}