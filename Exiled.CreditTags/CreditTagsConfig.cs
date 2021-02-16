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
        /// <inheritdoc/>
        [Description("Is the plugin enabled?")]
        public bool IsEnabled { get; set; } = true;

        [Description("If true a badge will be given, if false custom player text will be given")]
        public bool UseBadge { get; set; } = true;

        [Description("Should badge override existing badges?")]
        public bool BadgeOverride { get; set; } = true;

        [Description("Should CPT override existing CPT?")]
        public bool CPTOverride { get; set; } = true;
    }
}
