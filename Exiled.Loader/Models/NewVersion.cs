// -----------------------------------------------------------------------
// <copyright file="NewVersion.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader.Models
{
    using Exiled.Loader.GHApi.Models;

    /// <summary>
    /// An asset containing all data about a new version.
    /// </summary>
    public readonly struct NewVersion
    {
        /// <summary>
        /// The release.
        /// </summary>
        public readonly Release Release;

        /// <summary>
        /// The asset of the release.
        /// </summary>
        public readonly ReleaseAsset Asset;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewVersion"/> struct.
        /// </summary>
        /// <param name="release"><inheritdoc cref="Release"/></param>
        /// <param name="asset"><inheritdoc cref="Asset"/></param>
        public NewVersion(Release release, ReleaseAsset asset)
        {
            Release = release;
            Asset = asset;
        }
    }
}