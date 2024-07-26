// -----------------------------------------------------------------------
// <copyright file="Release.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader.GHApi.Models
{
    using System;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    using Utf8Json;

    /// <summary>
    /// An asset containing all release data.
    /// </summary>
    public readonly struct Release : IJsonSerializable
    {
        /// <summary>
        /// The release's id.
        /// </summary>
        [DataMember(Name = "id")]
        public readonly int Id;

        /// <summary>
        /// The release's tag name.
        /// </summary>
        [DataMember(Name = "tag_name")]
        public readonly string TagName;

        /// <summary>
        /// A value indicating whether the release is a pre-release.
        /// </summary>
        [DataMember(Name = "prerelease")]
        public readonly bool PreRelease;

        /// <summary>
        /// The release's creation date.
        /// </summary>
        [DataMember(Name = "created_at")]
        public readonly DateTime CreatedAt;

        /// <summary>
        /// The release's assets.
        /// </summary>
        [DataMember(Name = "assets")]
        public readonly ReleaseAsset[] Assets;

        /// <summary>
        /// The release's description.
        /// </summary>
        public readonly string Description;

        private const string URL = "https://api.github.com/repos/Exiled-Team/EXILED/releases/tags/";

        /// <summary>
        /// Initializes a new instance of the <see cref="Release"/> struct.
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id"/></param>
        /// <param name="tag_name"><inheritdoc cref="TagName"/></param>
        /// <param name="prerelease"><inheritdoc cref="PreRelease"/></param>
        /// <param name="created_at"><inheritdoc cref="CreatedAt"/></param>
        /// <param name="assets"><inheritdoc cref="Assets"/></param>
        [SerializationConstructor]
        public Release(int id, string tag_name, bool prerelease, DateTime created_at, ReleaseAsset[] assets)
        {
            Id = id;
            TagName = tag_name;
            PreRelease = prerelease;
            CreatedAt = created_at;
            Assets = assets;
            Description = string.Empty;

            using (HttpClient client = new())
            {
                HttpResponseMessage response = client.GetAsync(URL + TagName).GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                    return;

                string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                int startIndex = responseBody.IndexOf("\"body\":") + "\"body\":".Length;
                int endIndex = responseBody.IndexOf("\"draft\":") - 2;
                Description = responseBody.Substring(startIndex, endIndex - startIndex);
            }
        }
    }
}