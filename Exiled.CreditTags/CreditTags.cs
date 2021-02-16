// -----------------------------------------------------------------------
// <copyright file="CreditTags.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CreditTags
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net;

    using Exiled.API.Features;

    using PlayerEvents = Exiled.Events.Handlers.Player;

    /// <summary>
    /// Handles credits for Exiled Devs, Contributors and Plugin devs.
    /// </summary>
    public sealed class CreditTags : Plugin<CreditTagsConfig>
    {
        /// <summary>
        /// A static reference to this class.
        /// </summary>
        public static CreditTags Singleton;

        private const string Url = "https://exiled.host/checkcredits.php";

        private CreditsHandler handler;

        /// <inheritdoc/>
        public override string Prefix { get; } = "exiled_credits";

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey,TValue}"/> of Exiled Credit ranks.
        /// </summary>
        internal Dictionary<int, Rank> Ranks { get; } = new Dictionary<int, Rank>
        {
            { 1, new Rank("Exiled Developer", "aqua", "33DEFF") },
            { 2, new Rank("Exiled Contributor", "magenta", "B733FF") },
            { 3, new Rank("Exiled Plugin Developer", "crimson", "E60909") },
        };

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey,TValue}"/> of recently cached userIds and their ranks.
        /// </summary>
        internal Dictionary<string, int> RankCache { get; } = new Dictionary<string, int>();

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Singleton = this;
            base.OnEnabled();
            RefreshHandler();
            AttachHandler();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            base.OnDisabled();
            UnattachHandler();
        }

        /// <summary>
        /// Check to see if a player has EXILED Credit.
        /// </summary>
        /// <param name="userid">The <see cref="Player.UserId"/> of the player to check.</param>
        /// <returns>The rank ID assigned to the user's Userid. 0 means no credit was assigned.</returns>
        internal int CheckForExiledCredit(string userid)
        {
            userid = userid.Substring(0, userid.IndexOf('@') - 1);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"{Url}?userid={userid}");
            request.AutomaticDecompression = DecompressionMethods.GZip;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            if (!int.TryParse(responseString, out int rankId))
            {
                Log.Debug($"{nameof(CheckForExiledCredit)}: Response from server: {responseString}", Loader.Loader.ShouldDebugBeShown);
                return 0;
            }

            return rankId;
        }

        /// <summary>
        /// Shows a player's credit tag, if it exists.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who's tag should be shown.</param>
        /// <returns>Whether or not a tag was shown.</returns>
        internal bool ShowCreditTag(Player player)
        {
            int rankId;
            if (RankCache.ContainsKey(player.UserId))
            {
                rankId = RankCache[player.UserId];
            }
            else
            {
                rankId = CheckForExiledCredit(player.UserId);
                RankCache.Add(player.UserId, rankId);
            }

            if (Ranks.ContainsKey(rankId))
            {
                if ((string.IsNullOrEmpty(player.RankName) || Config.BadgeOverride) && Config.UseBadge)
                {
                    player.RankName = Ranks[rankId].Name;
                    player.RankColor = Ranks[rankId].Color;

                    return true;
                }

                if ((string.IsNullOrEmpty(player.CustomInfo) || Config.CPTOverride) && !Config.UseBadge)
                {
                    player.CustomInfo =
                        $"<color=${Ranks[rankId].HexValue}{Ranks[rankId].Name}</color>";
                }

                return true;
            }

            return false;
        }

        private void RefreshHandler() => handler = new CreditsHandler(this);

        private void AttachHandler() => PlayerEvents.Verified += handler.OnPlayerVerify;

        private void UnattachHandler() => PlayerEvents.Verified -= handler.OnPlayerVerify;
    }
}
