// -----------------------------------------------------------------------
// <copyright file="ArmorSettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems.Items.Armors
{
    using System.ComponentModel;

    /// <summary>
    /// A tool to easily setup armors.
    /// </summary>
    public class ArmorSettings : Settings
    {
        /// <summary>
        /// Gets or sets how much faster stamina will drain when wearing this armor.
        /// </summary>
        [Description("Indicates how much faster stamina will drain when wearing this armor. The value must be above 1 and below 2")]
        public virtual float StaminaUseMultiplier { get; set; } = 1.15f;

        /// <summary>
        /// Gets or sets how strong the helmet on the armor is.
        /// </summary>
        [Description("Indicates how strong the helmet on the armor is. The value must be above 0 and below 100")]
        public virtual int HelmetEfficacy { get; set; } = 80;

        /// <summary>
        /// Gets or sets how strong the vest on the armor is.
        /// </summary>
        [Description("Indicates how strong the vest on the armor is. The value must be above 0 and below 100")]
        public virtual int VestEfficacy { get; set; } = 80;
    }
}
