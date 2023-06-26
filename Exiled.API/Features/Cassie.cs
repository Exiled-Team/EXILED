// -----------------------------------------------------------------------
// <copyright file="Cassie.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Exiled.API.Features.Pools;

    using MEC;

    using PlayerRoles;

    using PlayerStatsSystem;

    using Respawning;

    using CustomFirearmHandler = DamageHandlers.FirearmDamageHandler;
    using CustomHandlerBase = DamageHandlers.DamageHandlerBase;

    /// <summary>
    /// A set of tools to use in-game C.A.S.S.I.E.
    /// </summary>
    public static class Cassie
    {
        /// <summary>
        /// Gets the <see cref="NineTailedFoxAnnouncer"/> singleton.
        /// </summary>
        public static NineTailedFoxAnnouncer Announcer => NineTailedFoxAnnouncer.singleton;

        /// <summary>
        /// Gets a value indicating whether or not C.A.S.S.I.E is currently announcing. Does not include decontamination messages.
        /// </summary>
        public static bool IsSpeaking => Announcer.queue.Count != 0;

        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}"/> of <see cref="NineTailedFoxAnnouncer.VoiceLine"/> objects that C.A.S.S.I.E recognizes.
        /// </summary>
        public static IReadOnlyCollection<NineTailedFoxAnnouncer.VoiceLine> VoiceLines => Announcer.voiceLines;

        /// <summary>
        /// Reproduce a non-glitched C.A.S.S.I.E message.
        /// </summary>
        /// <param name="message">The message to be reproduced.</param>
        /// <param name="isHeld">Indicates whether C.A.S.S.I.E has to hold the message.</param>
        /// <param name="isNoisy">Indicates whether C.A.S.S.I.E has to make noises or not during the message.</param>
        /// <param name="isSubtitles">Indicates whether C.A.S.S.I.E has to make subtitles.</param>
        public static void Message(string message, bool isHeld = false, bool isNoisy = true, bool isSubtitles = false) =>
            RespawnEffectsController.PlayCassieAnnouncement(message, isHeld, isNoisy, isSubtitles);

        /// <summary>
        /// Reproduce a non-glitched C.A.S.S.I.E message with a possibility to custom the subtitles.
        /// </summary>
        /// <param name="message">The message to be reproduced.</param>
        /// <param name="translation">The translation should be show in the subtitles.</param>
        /// <param name="isHeld">Indicates whether C.A.S.S.I.E has to hold the message.</param>
        /// <param name="isNoisy">Indicates whether C.A.S.S.I.E has to make noises or not during the message.</param>
        /// <param name="isSubtitles">Indicates whether C.A.S.S.I.E has to make subtitles.</param>
        public static void MessageTranslated(string message, string translation, bool isHeld = false, bool isNoisy = true, bool isSubtitles = true)
        {
            StringBuilder announcement = StringBuilderPool.Pool.Get();
            string[] cassies = message.Split('\n');
            string[] translations = translation.Split('\n');
            for (int i = 0; i < cassies.Length; i++)
                announcement.Append($"{translations[i].Replace(' ', ' ')}<size=0> {cassies[i]} </size><split>");

            RespawnEffectsController.PlayCassieAnnouncement(announcement.ToString(), isHeld, isNoisy, isSubtitles);
            StringBuilderPool.Pool.Return(announcement);
        }

        /// <summary>
        /// Reproduce a glitchy C.A.S.S.I.E announcement.
        /// </summary>
        /// <param name="message">The message to be reproduced.</param>
        /// <param name="glitchChance">The chance of placing a glitch between each word.</param>
        /// <param name="jamChance">The chance of jamming each word.</param>
        public static void GlitchyMessage(string message, float glitchChance, float jamChance) =>
            Announcer.ServerOnlyAddGlitchyPhrase(message, glitchChance, jamChance);

        /// <summary>
        /// Reproduce a non-glitched C.A.S.S.I.E message after a certain amount of seconds.
        /// </summary>
        /// <param name="message">The message to be reproduced.</param>
        /// <param name="delay">The seconds that have to pass before reproducing the message.</param>
        /// <param name="isHeld">Indicates whether C.A.S.S.I.E has to hold the message.</param>
        /// <param name="isNoisy">Indicates whether C.A.S.S.I.E has to make noises or not during the message.</param>
        /// <param name="isSubtitles">Indicates whether C.A.S.S.I.E has to make subtitles.</param>
        public static void DelayedMessage(string message, float delay, bool isHeld = false, bool isNoisy = true, bool isSubtitles = false) =>
            Timing.CallDelayed(delay, () => RespawnEffectsController.PlayCassieAnnouncement(message, isHeld, isNoisy, isSubtitles));

        /// <summary>
        /// Reproduce a glitchy C.A.S.S.I.E announcement after a certain period of seconds.
        /// </summary>
        /// <param name="message">The message to be reproduced.</param>
        /// <param name="delay">The seconds that have to pass before reproducing the message.</param>
        /// <param name="glitchChance">The chance of placing a glitch between each word.</param>
        /// <param name="jamChance">The chance of jamming each word.</param>
        public static void DelayedGlitchyMessage(string message, float delay, float glitchChance, float jamChance) =>
            Timing.CallDelayed(delay, () => Announcer.ServerOnlyAddGlitchyPhrase(message, glitchChance, jamChance));

        /// <summary>
        /// Calculates the duration of a C.A.S.S.I.E message.
        /// </summary>
        /// <param name="message">The message, which duration will be calculated.</param>
        /// <param name="rawNumber">Determines if a number won't be converted to its full pronunciation.</param>
        /// <returns>Duration (in seconds) of specified message.</returns>
        public static float CalculateDuration(string message, bool rawNumber = false) // TODO: Add Speed property
            => Announcer.CalculateDuration(message, rawNumber);

        /// <summary>
        /// Converts a <see cref="Team"/> into a Cassie-Readable <c>CONTAINMENTUNIT</c>.
        /// </summary>
        /// <param name="team"><see cref="Team"/>.</param>
        /// <param name="unitName">Unit Name.</param>
        /// <returns><see cref="string"/> Containment Unit text.</returns>
        public static string ConvertTeam(Team team, string unitName)
            => NineTailedFoxAnnouncer.ConvertTeam(team, unitName);

        /// <summary>
        /// Converts a number into a Cassie-Readable String.
        /// </summary>
        /// <param name="num">Number to convert.</param>
        /// <returns>A CASSIE-readable <see cref="string"/> representing the number.</returns>
        public static string ConvertNumber(int num)
            => NineTailedFoxAnnouncer.ConvertNumber(num);

        /// <summary>
        /// Announce a SCP Termination.
        /// </summary>
        /// <param name="scp">SCP to announce termination of.</param>
        /// <param name="info">HitInformation.</param>
        public static void ScpTermination(Player scp, DamageHandlerBase info)
            => NineTailedFoxAnnouncer.AnnounceScpTermination(scp.ReferenceHub, info);

        /// <summary>
        /// Announces the termination of a custom SCP name.
        /// </summary>
        /// <param name="scpName">SCP Name. Note that for larger numbers, C.A.S.S.I.E will pronounce the place (eg. "457" -> "four hundred fifty seven"). Spaces can be used to prevent this behavior.</param>
        /// <param name="info">Hit Information.</param>
        public static void CustomScpTermination(string scpName, CustomHandlerBase info)
        {
            string result = scpName;
            if (info.Is(out MicroHidDamageHandler _))
                result += " SUCCESSFULLY TERMINATED BY AUTOMATIC SECURITY SYSTEM";
            else if (info.Is(out WarheadDamageHandler _))
                result += " SUCCESSFULLY TERMINATED BY ALPHA WARHEAD";
            else if (info.Is(out UniversalDamageHandler _))
                result += " LOST IN DECONTAMINATION SEQUENCE";
            else if (info.BaseIs(out CustomFirearmHandler firearmDamageHandler) && firearmDamageHandler.Attacker is Player attacker)
                result += " CONTAINEDSUCCESSFULLY " + ConvertTeam(attacker.Role.Team, attacker.UnitName);

            // result += "To be changed";
            else
                result += " SUCCESSFULLY TERMINATED . TERMINATION CAUSE UNSPECIFIED";

            float num = AlphaWarheadController.TimeUntilDetonation <= 0f ? 3.5f : 1f;
            GlitchyMessage(result, UnityEngine.Random.Range(0.1f, 0.14f) * num, UnityEngine.Random.Range(0.07f, 0.08f) * num);
        }

        /// <summary>
        /// Clears the C.A.S.S.I.E queue.
        /// </summary>
        public static void Clear() => RespawnEffectsController.ClearQueue();

        /// <summary>
        /// Gets a value indicating whether or not the given word is a valid C.A.S.S.I.E word.
        /// </summary>
        /// <param name="word">The word to check.</param>
        /// <returns><see langword="true"/> if the word is valid; otherwise, <see langword="false"/>.</returns>
        public static bool IsValid(string word) => Announcer.voiceLines.Any(line => line.apiName.ToUpper() == word.ToUpper());

        /// <summary>
        /// Gets a value indicating whether or not the given sentence is all valid C.A.S.S.I.E word.
        /// </summary>
        /// <param name="sentence">The sentence to check.</param>
        /// <returns><see langword="true"/> if the sentence is valid; otherwise, <see langword="false"/>.</returns>
        public static bool IsValidSentence(string sentence) => sentence.Split(' ').All(word => string.IsNullOrWhiteSpace(word) || IsValid(word));
    }
}