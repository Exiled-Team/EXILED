// -----------------------------------------------------------------------
// <copyright file="ReleaseAsset.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Updater.GHApi.Models {
    using System.Runtime.Serialization;

    using Utf8Json;

    public readonly struct ReleaseAsset {
        [DataMember(Name = "id")] public readonly int Id;
        [DataMember(Name = "name")] public readonly string Name;
        [DataMember(Name = "size")] public readonly int Size;
        [DataMember(Name = "url")] public readonly string Url;
        [DataMember(Name = "browser_download_url")] public readonly string BrowserDownloadUrl;

        [SerializationConstructor]
        public ReleaseAsset(int id, string name, int size, string url, string browser_download_url) {
            Id = id;
            Name = name;
            Size = size;
            Url = url;
            BrowserDownloadUrl = browser_download_url;
        }
    }
}
