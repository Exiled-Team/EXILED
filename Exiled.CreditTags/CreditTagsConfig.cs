// -----------------------------------------------------------------------
// <copyright file="CreditTagsConfig.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CreditTags
{
    using System.ComponentModel;

    using Exiled.API.Interfaces;

    /// <inheritdoc />
    public sealed class CreditTagsConfig : IConfig
    {
        public enum InfoSide
        {
            /// <summary>
            /// Uses badge.
            /// </summary>
            Badge,

            /// <summary>
            /// Uses Custom Player Info area
            /// </summary>
            CustomPlayerInfo,

            /// <summary>
            /// Includes both of them.
            /// </summary>
            Both,
        }

        /// <inheritdoc/>
        [Description("Is the plugin enabled?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Info side - Badge, CustomPlayerInfo, Both")]
        public InfoSide Mode { get; set; } = InfoSide.Both;

        [Description("Overrides badge if exists")]
        public bool BadgeOverride { get; set; } = true;

        [Description("Overrides Custom Player Info if exists")]
        public bool CustomPlayerInfoOverride { get; set; } = true;

        public bool UseBadge() => Mode == InfoSide.Both || Mode == InfoSide.Badge;

        public bool UseCustomPlayerInfo() => Mode == InfoSide.Both || Mode == InfoSide.CustomPlayerInfo;
    }
}
