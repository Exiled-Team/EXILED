// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.CreditTags
{
    using System.ComponentModel;

    using SEXiled.API.Interfaces;
    using SEXiled.CreditTags.Enums;

    /// <inheritdoc />
    public sealed class Config : IConfig
    {
        /// <inheritdoc/>
        [Description("Is the plugin enabled?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Info side - Badge, CustomPlayerInfo, FirstAvailable")]
        public InfoSide Mode { get; private set; } = InfoSide.FirstAvailable;

        [Description("Overrides badge if exists")]
        public bool BadgeOverride { get; private set; } = false;

        [Description("Overrides Custom Player Info if exists")]
        public bool CustomPlayerInfoOverride { get; private set; } = false;

        [Description("Whether or not the plugin should ignore a player's DNT flag. By default (false), players with DNT flag will not be checked for credit tags.")]
        public bool IgnoreDntFlag { get; private set; } = true;
    }
}
