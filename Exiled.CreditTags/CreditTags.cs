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

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CreditTags.Enums;
    using Exiled.CreditTags.Events;
    using Exiled.CreditTags.Features;

    using UnityEngine;

    using PlayerEvents = Exiled.Events.Handlers.Player;

    /// <summary>
    /// Handles credits for Exiled Devs, Contributors and Plugin devs.
    /// </summary>
    public sealed class CreditTags : Plugin<Config>
    {
        private const string Url = "https://exiled.host/utilities/checkcredits.php";

        private CreditsHandler handler;

        /// <summary>
        /// Gets a static reference to this class.
        /// </summary>
        public static CreditTags Instance { get; private set; }

        /// <inheritdoc/>
        public override string Prefix { get; } = "exiled_credits";

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey,TValue}"/> of Exiled Credit ranks.
        /// </summary>
        internal Dictionary<RankType, Rank> Ranks { get; } = new Dictionary<RankType, Rank>
        {
            [RankType.Dev] = new Rank("Exiled Developer", "aqua", "33DEFF"),
            [RankType.Contributor] = new Rank("Exiled Contributor", "magenta", "B733FF"),
            [RankType.PluginDev] = new Rank("Exiled Plugin Developer", "crimson", "E60909"),
        };

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey,TValue}"/> of recently cached userIds and their ranks.
        /// </summary>
        internal Dictionary<string, RankType> RankCache { get; } = new Dictionary<string, RankType>();

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;
            RefreshHandler();
            AttachHandler();

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            UnattachHandler();

            base.OnDisabled();
        }

        internal void MakeRequest(string userId, Action<ThreadSafeRequest> errorHandler, Action<string> successHandler, GameObject issuer)
        {
            ThreadSafeRequest.Go($"{Url}?userid={userId.GetRawUserId()}", errorHandler, successHandler, issuer);
        }

        // returns true if the player was in the cache
        internal bool ShowCreditTag(Player player, Action errorHandler, Action successHandler, bool force = false)
        {
            Log.Debug($"{player.Nickname} thingy");
            if (RankCache.TryGetValue(player.UserId, out var cachedRank))
            {
                ShowRank(cachedRank);
                return true;
            }
            else
            {
                MakeRequest(player.UserId, ErrorHandler, SuccessHandler, player.GameObject);
                return false;
            }

            void SuccessHandler(string result)
            {
                if (Enum.TryParse<RankType>(result, out var kind))
                {
                    RankCache[player.UserId] = kind;
                    ShowRank(kind);

                    successHandler?.Invoke();
                }
                else
                {
                    Log.Debug($"{nameof(SuccessHandler)}: Invalid RankKind - response: {result}", Loader.Loader.ShouldDebugBeShown);
                }
            }

            void ErrorHandler(ThreadSafeRequest request)
            {
                Log.Debug($"{nameof(ErrorHandler)}: Response: {request.Result} Code: {request.Code}", Loader.Loader.ShouldDebugBeShown);

                errorHandler?.Invoke();
            }

            void ShowRank(RankType rank)
            {
                if (Ranks.TryGetValue(rank, out var value))
                {
                    if (Config.UseBadge())
                    {
                        if (force || ((string.IsNullOrEmpty(player.RankName) || Config.BadgeOverride) &&
                                      player.GlobalBadge == null))
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

        private void RefreshHandler() => handler = new CreditsHandler();

        private void AttachHandler() => PlayerEvents.Verified += handler.OnPlayerVerify;

        private void UnattachHandler() => PlayerEvents.Verified -= handler.OnPlayerVerify;
    }
}
