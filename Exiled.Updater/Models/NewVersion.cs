// -----------------------------------------------------------------------
// <copyright file="NewVersion.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Updater.Models
{
    using Exiled.Updater.GHApi.Models;

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