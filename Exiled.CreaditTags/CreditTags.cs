// -----------------------------------------------------------------------
// <copyright file="CreditTags.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CreaditTags
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net;

    using Exiled.API.Features;

    using PlayerEvents = Exiled.Events.Handlers.Player;

    public sealed class CreditTags : Plugin<CreditTagsConfig>
    {
        private CreditsHandler handler;

        private CreditTags()
        {
        }

        public static CreditTags Instance { get; } = new CreditTags();

        public override string Prefix { get; } = "exiled_credits";

        public override void OnEnabled()
        {
            base.OnEnabled();
            RefreshHandler();
            AttachHandler();
        }

        public override void OnDisabled()
        {
            base.OnDisabled();
            UnattachHandler();
        }

        private Dictionary<string, string> GetURL()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://gist.githubusercontent.com/babyboucher/89b23cd569e759c57f458df0411588fe/raw/PublicIDs");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                Dictionary<string, string> dict = Utf8Json.JsonSerializer.Deserialize<Dictionary<string, string>>(reader.ReadToEnd());
                return dict;
            }
        }

        private void RefreshHandler() => handler = new CreditsHandler(GetURL());

        private void AttachHandler() => PlayerEvents.Verified += handler.OnPlayerVerify;

        private void UnattachHandler() => PlayerEvents.Verified -= handler.OnPlayerVerify;
    }
}
