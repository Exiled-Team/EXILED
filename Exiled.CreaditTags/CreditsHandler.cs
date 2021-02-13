// -----------------------------------------------------------------------
// <copyright file="CreditsHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CreaditTags
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;

    using Exiled.Events.EventArgs;

    internal sealed class CreditsHandler
    {
        private readonly Dictionary<string, string> tagsContainer;

        public CreditsHandler(Dictionary<string, string> container) => tagsContainer = container;

        private CreditTagsConfig Config => CreditTags.Instance.Config;

        public void OnPlayerVerify(VerifiedEventArgs ev)
        {
            string playerID = HashSh1(ev.Player.UserId);
            if (tagsContainer.ContainsKey(playerID))
            {
                if ((ev.Player.RankName == null || Config.BadgeOverride) && Config.UseBadge)
                {
                    ev.Player.RankName = tagsContainer[playerID];
                    switch (tagsContainer[playerID])
                    {
                        case "Exiled Plugin Dev":
                            ev.Player.RankColor = "crimson";
                            break;
                        case "Exiled Contributor":
                            ev.Player.RankColor = "cyan";
                            break;
                        case "Exiled Dev":
                            ev.Player.RankColor = "magenta";
                            break;
                    }
                }

                if ((ev.Player.ReferenceHub.nicknameSync.Network_customPlayerInfoString == null || Config.CPTOverride) && !Config.UseBadge)
                {
                    switch (tagsContainer[playerID])
                    {
                        case "Exiled Plugin Dev":
                            ev.Player.ReferenceHub.nicknameSync.Network_customPlayerInfoString = "<color=#DC143C>Exiled Plugin Dev</color>";
                            break;
                        case "Exiled Contributor":
                            ev.Player.ReferenceHub.nicknameSync.Network_customPlayerInfoString = "<color=#800080>Exiled Contributor</color>";
                            break;
                        case "Exiled Dev":
                            ev.Player.ReferenceHub.nicknameSync.Network_customPlayerInfoString = "<color=#00FFFF>Exiled Dev</color>";
                            break;
                    }
                }
            }
        }

        private string HashSh1(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hashSh1 = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));

                // declare stringbuilder
                var sb = new StringBuilder(hashSh1.Length * 2);

                // computing hashSh1
                foreach (byte b in hashSh1)
                {
                    // "x2"
                    sb.Append(b.ToString("X2").ToLower());
                }

                // final output
                Console.WriteLine(string.Format("The SHA1 hash of {0} is: {1}", input, sb.ToString()));

                return sb.ToString();
            }
        }
    }
}
