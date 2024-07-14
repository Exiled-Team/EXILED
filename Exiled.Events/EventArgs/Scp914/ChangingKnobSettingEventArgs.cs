// -----------------------------------------------------------------------
// <copyright file="ChangingKnobSettingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp914
{
    using API.Features;

    using global::Scp914;

    using Interfaces;

    /// <summary>
    /// Contains all information before a player changes the SCP-914 knob setting.
    /// </summary>
    public class ChangingKnobSettingEventArgs : IPlayerEvent, IDeniableEvent
    {
        private Scp914KnobSetting knobSetting;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingKnobSettingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="knobSetting">
        /// <inheritdoc cref="KnobSetting" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public ChangingKnobSettingEventArgs(Player player, Scp914KnobSetting knobSetting, bool isAllowed = true)
        {
            Player = player;
            KnobSetting = knobSetting;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the SCP-914 knob setting.
        /// </summary>
        public Scp914KnobSetting KnobSetting
        {
            get => knobSetting;
            set => knobSetting = value > Scp914KnobSetting.VeryFine ? Scp914KnobSetting.Coarse : value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether SCP-914's knob setting can be changed.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the player who's changing the SCP-914 knob setting.
        /// </summary>
        public Player Player { get; }
    }
}