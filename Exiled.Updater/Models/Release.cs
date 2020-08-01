// -----------------------------------------------------------------------
// <copyright file="Release.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Updater.Models
{
    using System;
    using System.Runtime.Serialization;

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1401 // Fields should be private

    public struct Release
    {
        [DataMember(Name = "id")]
        public int Id;
        [DataMember(Name = "tag_name")]
        public string TagName;
        [DataMember(Name = "prerelease")]
        public bool PreRelease;
        [DataMember(Name = "created_at")]
        public DateTime CreatedAt;
        [DataMember(Name = "assets")]
        public ReleaseAsset[] Assets;
    }
}
