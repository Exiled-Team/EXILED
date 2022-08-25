// -----------------------------------------------------------------------
// <copyright file="Release.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Updater.GHApi.Models
{
    using System;
    using System.Runtime.Serialization;

    using Utf8Json;

    public readonly struct Release : IJsonSerializable
    {
        [DataMember(Name = "id")] public readonly int Id;
        [DataMember(Name = "tag_name")] public readonly string TagName;
        [DataMember(Name = "prerelease")] public readonly bool PreRelease;
        [DataMember(Name = "created_at")] public readonly DateTime CreatedAt;
        [DataMember(Name = "assets")] public readonly ReleaseAsset[] Assets;

        [SerializationConstructor]
        public Release(int id, string tag_name, bool prerelease, DateTime created_at, ReleaseAsset[] assets)
        {
            Id = id;
            TagName = tag_name;
            PreRelease = prerelease;
            CreatedAt = created_at;
            Assets = assets;
        }
    }
}