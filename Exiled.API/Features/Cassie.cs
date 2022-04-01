// -----------------------------------------------------------------------
// <copyright file="Cassie.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features {
    using MEC;

    using PlayerStatsSystem;

    using Respawning;

    using static PlayerStatsSystem.PlayerStats;

    /// <summary>
    /// A set of tools to use in-game C.A.S.S.I.E.
    /// </summary>
    public static class Cassie {
        /// <summary>
        /// Gets a value indicating whether or not C.A.S.S.I.E is currently announcing. Does not include decontamination messages.
        /// </summary>
        public static bool IsSpeaking => NineTailedFoxAnnouncer.singleton.queue.Count != 0;

        /// <summary>
        /// Reproduce a non-glitched C.A.S.S.I.E message.
        /// </summary>
        /// <param name="message">The message to be reproduced.</param>
        /// <param name="isHeld">Indicates whether C.A.S.S.I.E has to hold the message.</param>
        /// <param name="isNoisy">Indicates whether C.A.S.S.I.E has to make noises or not during the message.</param>
        public static void Message(string message, bool isHeld = false, bool isNoisy = true) =>
            RespawnEffectsController.PlayCassieAnnouncement(message, isHeld, isNoisy);

        /// <summary>
        /// Reproduce a glitchy C.A.S.S.I.E announcement.
        /// </summary>
        /// <param name="message">The message to be reproduced.</param>
        /// <param name="glitchChance">The chance of placing a glitch between each word.</param>
        /// <param name="jamChance">The chance of jamming each word.</param>
        public static void GlitchyMessage(string message, float glitchChance, float jamChance) =>
            NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(message, glitchChance, jamChance);

        /// <summary>
        /// Reproduce a non-glitched C.A.S.S.I.E message after a certain amount of seconds.
        /// </summary>
        /// <param name="message">The message to be reproduced.</param>
        /// <param name="delay">The seconds that have to pass before reproducing the message.</param>
        /// <param name="isHeld">Indicates whether C.A.S.S.I.E has to hold the message.</param>
        /// <param name="isNoisy">Indicates whether C.A.S.S.I.E has to make noises or not during the message.</param>
        public static void DelayedMessage(string message, float delay, bool isHeld = false, bool isNoisy = true) =>
            Timing.CallDelayed(delay, () => RespawnEffectsController.PlayCassieAnnouncement(message, isHeld, isNoisy));

        /// <summary>
        /// Reproduce a glitchy C.A.S.S.I.E announcement after a certain period of seconds.
        /// </summary>
        /// <param name="message">The message to be reproduced.</param>
        /// <param name="delay">The seconds that have to pass before reproducing the message.</param>
        /// <param name="glitchChance">The chance of placing a glitch between each word.</param>
        /// <param name="jamChance">The chance of jamming each word.</param>
        public static void DelayedGlitchyMessage(string message, float delay, float glitchChance, float jamChance) =>
            Timing.CallDelayed(delay, () => NineTailedFoxAnnouncer.singleton.ServerOnlyAddGlitchyPhrase(message, glitchChance, jamChance));

        /// <summary>
        /// Calculates duration of a C.A.S.S.I.E message.
        /// </summary>
        /// <param name="message">The message, which duration will be calculated.</param>
        /// <param name="rawNumber">Determines if a number won't be converted to its full pronunciation.</param>
        /// <returns>Duration (in seconds) of specified message.</returns>
        public static float CalculateDuration(string message, bool rawNumber = false)
            => NineTailedFoxAnnouncer.singleton.CalculateDuration(message, rawNumber);

        /// <summary>
        /// Converts a Team into a Cassie-Readable <c>CONTAINMENTUNIT</c>.
        /// </summary>
        /// <param name="team"><see cref="Team"/>.</param>
        /// <param name="unitName">Unit Name.</param>
        /// <returns><see cref="string"/> Containment Unit text.</returns>
        public static string ConvertTeam(Team team, string unitName)
            => NineTailedFoxAnnouncer.ConvertTeam(team, unitName);

        /// <summary>
        /// Converts Number into Cassie-Readable String.
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
        public static void SCPTermination(Player scp, DamageHandlerBase info)
            => NineTailedFoxAnnouncer.AnnounceScpTermination(scp.ReferenceHub, info);

        /// <summary>
        /// Announce the termination of a custom SCP name.
        /// </summary>
        /// <param name="scpname">SCP Name.</param>
        /// <param name="info">Hit Information.</param>
        public static void CustomSCPTermination(string scpname, DamageHandlerBase info) {
            string result = scpname;
            if (info is MicroHidDamageHandler)
                result += " SUCCESSFULLY TERMINATED BY AUTOMATIC SECURITY SYSTEM";
            else if (info is WarheadDamageHandler)
                result += " SUCCESSFULLY TERMINATED BY ALPHA WARHEAD";
            else if (info is UniversalDamageHandler)
                result += " LOST IN DECONTAMINATION SEQUENCE";
            else if (info is FirearmDamageHandler firearmDamageHandler && Player.Get(firearmDamageHandler.Attacker.Hub) is Player attacker)
                result += " CONTAINEDSUCCESSFULLY " + ConvertTeam(attacker.Team, attacker.UnitName);
            else
                result += " SUCCESSFULLY TERMINATED . TERMINATION CAUSE UNSPECIFIED";

            float num = (AlphaWarheadController.Host.timeToDetonation <= 0f) ? 3.5f : 1f;
            GlitchyMessage(result, UnityEngine.Random.Range(0.1f, 0.14f) * num, UnityEngine.Random.Range(0.07f, 0.08f) * num);
        }

        /// <summary>
        /// Clears the C.A.S.S.I.E queue.
        /// </summary>
        public static void Clear() => RespawnEffectsController.ClearQueue();
    }
}
