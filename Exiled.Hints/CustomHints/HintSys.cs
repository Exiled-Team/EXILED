// -----------------------------------------------------------------------
// <copyright file="HintSys.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Hints;
using RueI.Displays;
using RueI.Elements;
using RueI.Parsing;
using RueI.Parsing.Records;

namespace Exiled.API.Features.CustomHints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Hints;
    using MEC;
    using Mirror;

    /// <summary>
    /// A powerful hint system that allows multiple concurrent hints.
    /// </summary>
    public static class HintSys
    {
        /// <summary>
        /// A dictionary containing the caller and a tuple containing the hint, a list of players to receive the hint, and the timestamp the hint was sent at.
        /// </summary>
        private static Dictionary<string, Tuple<Hint, List<Player>, long>> hints = new();

        private static Dictionary<Player, List<string>> playerHints = new();

        private static CoroutineHandle? hintHandle;

        /// <summary>
        /// Gets the hints.
        /// </summary>
        /// <returns>A dictionary containing all hints, modifying will do nothing.</returns>
        public static Dictionary<string, Tuple<Hint, List<Player>, long>> GetHints()
        {
            return new Dictionary<string, Tuple<Hint, List<Player>, long>>(hints);
        }

        /// <summary>
        /// Adds a hint to the display queue, if there is already a hint by this caller, replaces it.
        /// </summary>
        /// <param name="content">The content of the hint.</param>
        /// <param name="duration">The duration in seconds.</param>
        /// <param name="players">A list of players to add to the hint.</param>
        /// <param name="caller">Do not populate.</param>
        public static void Display(string content, float duration, List<Player> players, [CallerMemberName] string caller = "")
        {
            Hint hint = new()
            {
                Content = content,
                Duration = duration,
            };
            hints[caller] = new Tuple<Hint, List<Player>, long>(hint, players, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            foreach (Player player in players)
            {
                playerHints.GetOrAdd(player, () => new List<string>()).Add(caller);
            }

            hintHandle ??= Timing.RunCoroutine(HintDisplayLoop());
        }

        private static void Send(string hintText, Player player)
        {
            HintDisplay hintDisplay = player.ReferenceHub.hints;
            if (hintDisplay.isLocalPlayer)
                throw new InvalidOperationException("Cannot display a hint to the local player (headless server).");
            if (!NetworkServer.active)
                return;
            NetworkConnection connectionToClient = hintDisplay.netIdentity.connectionToClient;
            if (HintDisplay.SuppressedReceivers.Contains(connectionToClient))
                return;
            TextHint hint = new TextHint(hintText, [new StringHintParameter(hintText)], null);
            connectionToClient.Send(new HintMessage(hint));
        }

        private static IEnumerator<float> HintDisplayLoop()
        {
            for (; ;)
            {
                foreach (KeyValuePair<Player, List<string>> keyValuePair in playerHints)
                {
                    List<ParsedData> parserContexts = [];
                    foreach (string se in keyValuePair.Value.ToList())
                    {
                        Tuple<Hint, List<Player>, long> item = hints[se];
                        if (!item.Item2.Contains(keyValuePair.Key))
                            continue;
                        if ((item.Item3 - DateTimeOffset.UtcNow.ToUnixTimeSeconds()) > item.Item1.Duration)
                        {
                            hints.Remove(se);
                            keyValuePair.Value.Remove(se);
                            continue;
                        }

                        parserContexts.Add(Parser.DefaultParser.Parse(item.Item1.Content));
                    }

                    string hint = ElemCombiner.Combine(parserContexts);
                    Send(hint, keyValuePair.Key);
                }

                if (hints.Count == 0)
                {
                    hintHandle = null;
                    break;
                }

                yield return Timing.WaitForSeconds(0.572f);
            }
        }
    }
}