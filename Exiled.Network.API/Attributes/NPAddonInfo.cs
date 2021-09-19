// -----------------------------------------------------------------------
// <copyright file="NPAddonInfo.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API.Attributes
{
    using System;

    /// <summary>
    /// Info about the addon.
    /// </summary>
    public class NPAddonInfo : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NPAddonInfo"/> class.
        /// </summary>
        /// <param name="addonId">Addon Id.</param>
        /// <param name="addonName">Addon Name.</param>
        /// <param name="addonVersion">Version.</param>
        /// <param name="addonAuthor">Author.</param>
        public NPAddonInfo(string addonId, string addonName, string addonAuthor, string addonVersion)
        {
            this.AddonID = addonId;
            this.AddonName = addonName;
            this.AddonAuthor = addonAuthor;
            this.AddonVersion = new Version(addonVersion);
        }

        /// <summary>
        /// Gets or sets the AddonID.
        /// </summary>
        public string AddonID { get; set; }

        /// <summary>
        /// Gets or sets the AddonName.
        /// </summary>
        public string AddonName { get; set; }

        /// <summary>
        /// Gets or sets the AddonVersion.
        /// </summary>
        public Version AddonVersion { get; set; }

        /// <summary>
        /// Gets or sets AddonAuthor.
        /// </summary>
        public string AddonAuthor { get; set; }
    }
}
