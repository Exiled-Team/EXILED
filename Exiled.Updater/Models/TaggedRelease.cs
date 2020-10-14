// -----------------------------------------------------------------------
// <copyright file="TaggedRelease.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Updater.Models
{
    using Exiled.Updater.GHApi.Models;

    public readonly struct TaggedRelease
    {
        public readonly Release Release;
        public readonly SemVer2Version Version;

        public TaggedRelease(Release release)
        {
            Release = release;
            Version = SemVer2Version.Parse(release.TagName);
        }
    }
}
