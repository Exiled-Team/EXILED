// -----------------------------------------------------------------------
// <copyright file="TaggedRelease.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Updater.Models
{
    using SEXiled.Updater.GHApi.Models;

    using SemVer;

    public readonly struct TaggedRelease
    {
        public readonly Release Release;
        public readonly Version Version;

        public TaggedRelease(Release release)
        {
            Release = release;
            Version = Version.Parse(release.TagName);
        }
    }
}
