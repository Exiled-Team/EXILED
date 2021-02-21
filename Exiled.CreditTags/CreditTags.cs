// -----------------------------------------------------------------------
// <copyright file="CreditTags.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CreditTags
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Features;

    using UnityEngine;

    using PlayerEvents = Exiled.Events.Handlers.Player;

    /// <summary>
    /// Handles credits for Exiled Devs, Contributors and Plugin devs.
    /// </summary>
    public sealed class CreditTags : Plugin<CreditTagsConfig>
    {
        private const string Url = "https://exiled.host/utilities/checkcredits.php";

        private CreditsHandler handler;

        /// <summary>
        /// Gets a static reference to this class.
        /// </summary>
        public static CreditTags Singleton { get; private set; }

        /// <inheritdoc/>
        public override string Prefix { get; } = "exiled_credits";

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey,TValue}"/> of Exiled Credit ranks.
        /// </summary>
        internal Dictionary<RankKind, Rank> Ranks { get; } = new Dictionary<RankKind, Rank>
        {
            [RankKind.Dev] = new Rank("Exiled Developer", "aqua", "33DEFF"),
            [RankKind.Contributor] = new Rank("Exiled Contributor", "magenta", "B733FF"),
            [RankKind.PluginDev] = new Rank("Exiled Plugin Developer", "crimson", "E60909"),
        };

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey,TValue}"/> of recently cached userIds and their ranks.
        /// </summary>
        internal Dictionary<string, RankKind> RankCache { get; } = new Dictionary<string, RankKind>();

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

        internal void MakeRequst(string userid, Action<ThreadSafeRequest> errorHandler, Action<string> resultHandler, GameObject issuer)
        {
            userid = userid.Substring(0, userid.IndexOf('@'));
            var url = $"{Url}?userid={userid}";

            ThreadSafeRequest.Go(url, errorHandler, resultHandler, issuer);
            /*
             * Log.Debug($"{nameof(CheckForExiledCredit)}: Response from server: {responseString}", Loader.Loader.ShouldDebugBeShown);
             */
        }

        // returns true if the player was in the cache
        internal bool ShowCreditTag(Player player, Action errorHandler, Action happyHandler, bool force = false)
        {
            if (RankCache.TryGetValue(player.UserId, out var cachedRank))
            {
                ShowRank(cachedRank);
                return true;
            }
            else
            {
                MakeRequst(player.UserId, ErrorHandler, HappyHandler, player.GameObject);
                return false;
            }

            void HappyHandler(string result)
            {
                if (Enum.TryParse<RankKind>(result, out var kind))
                {
                    RankCache[player.UserId] = kind;
                    ShowRank(kind);

                    if (happyHandler != null)
                        happyHandler();
                }
                else
                {
                    Log.Debug($"{nameof(HappyHandler)}: Invalid RankKind - response: {result}", Loader.Loader.ShouldDebugBeShown);
                }
            }

            void ErrorHandler(ThreadSafeRequest request)
            {
                Log.Debug($"{nameof(ErrorHandler)}: Response: {request.Result} Code: {request.Code}", Loader.Loader.ShouldDebugBeShown);

                if (errorHandler != null)
                    errorHandler();
            }

            void ShowRank(RankKind rank)
            {
                if (Ranks.TryGetValue(rank, out var value))
                {
                    if (Config.UseBadge())
                    {
                        if (force || (string.IsNullOrEmpty(player.RankName) || Config.BadgeOverride))
                        {
                            player.RankName = value.Name;
                            player.RankColor = value.Color;
                        }
                    }

                    if (Config.UseCustomPlayerInfo() && (string.IsNullOrEmpty(player.CustomInfo) || Config.CustomPlayerInfoOverride))
                    {
                        player.CustomInfo = $"<color=#{value.HexValue}>{value.Name}</color>";
                    }
                }
            }
        }

        private void RefreshHandler() => handler = new CreditsHandler(this);

        private void AttachHandler() => PlayerEvents.Verified += handler.OnPlayerVerify;

        private void UnattachHandler() => PlayerEvents.Verified -= handler.OnPlayerVerify;
    }
}
