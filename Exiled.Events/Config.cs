// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events
{
    using System.ComponentModel;

    using API.Interfaces;

    /// <inheritdoc cref="IConfig"/>
    public sealed class Config : IConfig
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <inheritdoc/>
        public bool Debug { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether events are only patched if they have delegates subscribed to them.
        /// </summary>
        [Description("Indicates whether events are patched only if they have delegates subscribed to them")]
        public bool UseDynamicPatching { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether SCP-173 can be blocked or not by the tutorial.
        /// </summary>
        [Description("Indicates whether SCP-173 can be blocked or not by the tutorial")]
        public bool CanTutorialBlockScp173 { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether SCP-096 can be triggered or not by the tutorial.
        /// </summary>
        [Description("Indicates whether SCP-096 can be triggered or not by the tutorial")]
        public bool CanTutorialTriggerScp096 { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether SCP-049 can activate the sense ability on tutorials.
        /// </summary>
        [Description("Indicates whether SCP-049 can sense tutorial players or not")]
        public bool CanScp049SenseTutorial { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether tutorial is affected by SCP-079 scan.
        /// </summary>
        [Description("Indicates whether tutorial is affected by SCP-079 scan.")]
        public bool TutorialNotAffectedByScp079Scan { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether tutorials can be handcuffed.
        /// </summary>
        [Description("Indicates whether tutorials can be handcuffed.")]
        public bool CanTutorialsBeHandcuffed { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether flashbangs flash original thrower.
        /// </summary>
        [Description("Indicates whether flashbangs flash original thrower.")]
        public bool CanFlashbangsAffectThrower { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the name tracking (invisible EXILED version string added to the end of the server name) is enabled or not.
        /// </summary>
        [Description("Indicates whether the name tracking (invisible EXILED version string added to the end of the server name) is enabled or not")]
        public bool IsNameTrackingEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the inventory should be dropped before being set as spectator, through commands or plugins.
        /// </summary>
        [Description("Indicates whether the inventory should be dropped before being set as spectator, through commands or plugins")]
        public bool ShouldDropInventory { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the blood can be spawned or not.
        /// </summary>
        [Description("Indicates whether the blood can be spawned or not")]
        public bool CanSpawnBlood { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether keycard throw can affect basic doors.
        /// </summary>
        [Description("Indicates whether thrown keycards can affect doors that don't require any permissions")]
        public bool CanKeycardThrowAffectDoors { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether configs has to be reloaded every time a round restarts.
        /// </summary>
        [Description("Indicates whether configs have to be reloaded every round restart")]
        public bool ShouldReloadConfigsAtRoundRestart { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether translations has to be reloaded every time a round restarts.
        /// </summary>
        [Description("Indicates whether translations has to be reloaded every round restart")]
        public bool ShouldReloadTranslationsAtRoundRestart { get; set; }

        /// <summary>
        /// Gets a value indicating whether bans should be logged or not.
        /// </summary>
        [Description("Indicates whether bans should be logged or not")]
        public bool ShouldLogBans { get; private set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to log RA commands.
        /// </summary>
        [Description("Whether or not to log RA commands.")]
        public bool LogRaCommands { get; set; } = true;
    }
}
