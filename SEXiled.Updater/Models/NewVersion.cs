// -----------------------------------------------------------------------
// <copyright file="NewVersion.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Updater.Models
{
    using SEXiled.Updater.GHApi.Models;

    public readonly struct NewVersion
    {
        public readonly Release Release;
        public readonly ReleaseAsset Asset;

        public NewVersion(Release release, ReleaseAsset asset)
        {
            Release = release;
            Asset = asset;
        }
    }
}
