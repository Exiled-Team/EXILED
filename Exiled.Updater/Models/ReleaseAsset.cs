// -----------------------------------------------------------------------
// <copyright file="ReleaseAsset.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Updater.Models
{
    using System.Runtime.Serialization;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1401 // Fields should be private

    public struct ReleaseAsset
    {
        [DataMember(Name = "id")]
        public int Id;
        [DataMember(Name = "name")]
        public string Name;
        [DataMember(Name = "size")]
        public int Size;
        [DataMember(Name = "url")]
        public string Url;
        [DataMember(Name = "browser_download_url")]
        public string BrowserDownloadUrl;
    }
}
