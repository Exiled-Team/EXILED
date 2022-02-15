// -----------------------------------------------------------------------
// <copyright file="CreditTags.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.CreditTags
{
    using System;
    using System.Collections.Generic;

    using SEXiled.API.Extensions;
    using SEXiled.API.Features;
    using SEXiled.CreditTags.Enums;
    using SEXiled.CreditTags.Events;
    using SEXiled.CreditTags.Features;

    using UnityEngine;

    using PlayerEvents = SEXiled.Events.Handlers.Player;

    /// <summary>
    /// Handles credits for SEXiled Devs, Contributors and Plugin devs.
    /// </summary>
    public sealed class CreditTags : Plugin<Config>
    {
        private const string Url = "https://sexiled.host/utilities/checkcredits.php";

        private static readonly CreditTags Singleton = new CreditTags();

        private CreditsHandler handler;

        private CreditTags()
        {
        }

        /// <summary>
        /// Gets a static reference to this class.
        /// </summary>
        public static CreditTags Instance => Singleton;

        /// <inheritdoc/>
        public override string Prefix { get; } = "exiled_credits";

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey,TValue}"/> of SEXiled Credit ranks.
        /// </summary>
        internal Dictionary<RankType, Rank> Ranks { get; } = new Dictionary<RankType, Rank>
        {
            [RankType.Dev] = new Rank("SEXiled Developer", "aqua", "33DEFF"),
            [RankType.Contributor] = new Rank("SEXiled Contributor", "magenta", "B733FF"),
            [RankType.PluginDev] = new Rank("SEXiled Plugin Developer", "crimson", "E60909"),
            [RankType.TournamentParticipant] = new Rank("SEXiled Tournament Participant", "pink", "FF96DE"),
            [RankType.TournamentChampion] = new Rank("SEXiled Tournament Champion", "deep_pink", "FF1493"),
        };

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey,TValue}"/> of recently cached userIds and their ranks.
        /// </summary>
        internal Dictionary<string, RankType> RankCache { get; } = new Dictionary<string, RankType>();

        /// <inheritdoc/>
        public override void OnEnabled()
        {
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
            ThreadSafeRequest.Go($"{Url}?userid={userId.GetHashedUserId()}", errorHandler, successHandler, issuer);
        }

        // returns true if the player was in the cache
        internal bool ShowCreditTag(Player player, Action errorHandler, Action successHandler, bool force = false)
        {
            if (player.DoNotTrack && !Instance.Config.IgnoreDntFlag && !force)
                return false;

            if (RankCache.TryGetValue(player.UserId, out RankType cachedRank))
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
                if (Enum.TryParse(result, out RankType kind))
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
                bool canReceiveCreditBadge = force ||
                                             (((string.IsNullOrEmpty(player.RankName) &&
                                                string.IsNullOrEmpty(player.ReferenceHub.serverRoles.HiddenBadge)) ||
                                               Config.BadgeOverride) && player.GlobalBadge == null);
                bool canReceiveCreditCustomInfo =
                    string.IsNullOrEmpty(player.CustomInfo) || Config.CustomPlayerInfoOverride;

                if (Ranks.TryGetValue(rank, out Rank value))
                {
                    switch (Config.Mode)
                    {
                        case InfoSide.Badge:
                            if (canReceiveCreditBadge)
                            {
                              SetCreditBadge(player, value);
                            }

                            break;
                        case InfoSide.CustomPlayerInfo:
                            if (canReceiveCreditCustomInfo)
                            {
                                SetCreditCustomInfo(player, value);
                            }

                            break;
                        case InfoSide.FirstAvailable:
                            if (canReceiveCreditBadge)
                            {
                                SetCreditBadge(player, value);
                            }
                            else if (canReceiveCreditCustomInfo)
                            {
                                SetCreditCustomInfo(player, value);
                            }

                            break;
                    }
                }
            }
        }

        private void SetCreditBadge(Player player, Rank value)
        {
            player.RankName = value.Name;
            player.RankColor = value.Color;
        }

        private void SetCreditCustomInfo(Player player, Rank value)
        {
            player.CustomInfo = $"<color=#{value.HexValue}>{value.Name}</color>";
        }

        private void RefreshHandler() => handler = new CreditsHandler();

        private void AttachHandler() => PlayerEvents.Verified += handler.OnPlayerVerify;

        private void UnattachHandler() => PlayerEvents.Verified -= handler.OnPlayerVerify;
    }
}
