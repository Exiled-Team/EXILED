// -----------------------------------------------------------------------
// <copyright file="TaggedRelease.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Sexiled.Updater.GHApi.Models;

namespace Sexiled.Updater.Models
{
    using Sexiled.Updater.GHApi.Models;

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
