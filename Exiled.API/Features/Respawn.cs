// -----------------------------------------------------------------------
// <copyright file="Respawn.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;

    using CustomPlayerEffects;
    using Enums;
    using PlayerRoles;
    using Respawning;
    using Respawning.NamingRules;
    using Respawning.Waves;
    using Respawning.Waves.Generic;
    using UnityEngine;

    /// <summary>
    /// A set of tools to handle team respawns more easily.
    /// </summary>
    public static class Respawn
    {
        private static GameObject ntfHelicopterGameObject;
        private static GameObject chaosCarGameObject;

        /// <summary>
        /// Gets the NTF Helicopter's <see cref="GameObject"/>.
        /// </summary>
        public static GameObject NtfHelicopter
        {
            get
            {
                if (ntfHelicopterGameObject == null)
                    ntfHelicopterGameObject = GameObject.Find("Chopper");

                return ntfHelicopterGameObject;
            }
        }

        /// <summary>
        /// Gets the Chaos Van's <see cref="GameObject"/>.
        /// </summary>
        public static GameObject ChaosVan
        {
            get
            {
                if (chaosCarGameObject == null)
                    chaosCarGameObject = GameObject.Find("CIVanArrive");

                return chaosCarGameObject;
            }
        }

        /// <summary>
        /// Gets or sets the next known <see cref="Faction"/> that will spawn.
        /// </summary>
        public static Faction NextKnownFaction
        {
            get => WaveManager._nextWave.TargetFaction;
            set => WaveManager._nextWave = WaveManager.Waves.Find(x => x.TargetFaction == value);
        }

        /// <summary>
        /// Gets the next known <see cref="SpawnableTeamType"/> that will spawn.
        /// </summary>
        public static SpawnableTeamType NextKnownTeam => NextKnownFaction.GetSpawnableTeam();

        /// <summary>
        /// Gets the current queue state of the <see cref="WaveManager"/>.
        /// </summary>
        public static WaveManager.WaveQueueState QueueState => WaveManager.State;

        /// <summary>
        /// Gets a value indicating whether a wave is spawning or not.
        /// </summary>
        public static bool IsSpawning => QueueState == WaveManager.WaveQueueState.WaveSpawning;

        /// <summary>
        /// Gets or sets a value indicating whether or not spawn protection is enabled.
        /// </summary>
        public static bool ProtectionEnabled
        {
            get => SpawnProtected.IsProtectionEnabled;
            set => SpawnProtected.IsProtectionEnabled = value;
        }

        /// <summary>
        /// Gets or sets the spawn protection time, in seconds.
        /// </summary>
        public static float ProtectionTime
        {
            get => SpawnProtected.SpawnDuration;
            set => SpawnProtected.SpawnDuration = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not spawn protected players can shoot.
        /// </summary>
        public static bool ProtectedCanShoot
        {
            get => SpawnProtected.CanShoot;
            set => SpawnProtected.CanShoot = value;
        }

        /// <summary>
        /// Gets a <see cref="List{T}"/> of <see cref="Team"/> that have spawn protection.
        /// </summary>
        public static List<Team> ProtectedTeams => SpawnProtected.ProtectedTeams;

        /// <summary>
        /// Gets a string array of possible names for NTF.
        /// </summary>
        public static string[] NtfNamingCodes => NineTailedFoxNamingRule.PossibleCodes;

        /// <summary>
        /// Play effects when a certain class spawns.
        /// </summary>
        /// <param name="wave">The <see cref="SpawnableWaveBase"/> for which effects should be played.</param>
        public static void PlayEffect(SpawnableWaveBase wave)
        {
            WaveUpdateMessage.ServerSendUpdate(wave, UpdateMessageFlags.Trigger);
        }

        /// <summary>
        /// Tries to get a <see cref="SpawnableWaveBase"/>.
        /// </summary>
        /// <param name="wave">Found <see cref="SpawnableWaveBase"/>.</param>
        /// <typeparam name="T">Type of <see cref="SpawnableWaveBase"/>.</typeparam>
        /// <returns>if <paramref name="wave"/> was successfully found, true, if not, false.</returns>
        public static bool TryGetWave<T>(out T wave)
            where T : SpawnableWaveBase => WaveManager.TryGet(out wave);

        /// <summary>
        /// Tries to get a <see cref="SpawnableWaveBase"/> from a <see cref="Faction"/>.
        /// </summary>
        /// <param name="faction">Team's <see cref="Faction"/>.</param>
        /// <param name="wave">Found <see cref="SpawnableWaveBase"/>.</param>
        /// <returns>if <paramref name="wave"/> was successfully found, true, if not, false.</returns>
        public static bool TryGetWave(Faction faction, out SpawnableWaveBase wave)
            => WaveManager.TryGet(faction, out wave);

        /// <summary>
        /// Tries to get a <see cref="SpawnableWaveBase"/> from a <see cref="SpawnableWave"/>.
        /// </summary>
        /// <param name="spawnableWave">Team's <see cref="SpawnableWave"/>.</param>
        /// <param name="wave">Found <see cref="SpawnableWaveBase"/>.</param>
        /// <returns>if <paramref name="wave"/> was successfully found, true, if not, false.</returns>
        public static bool TryGetWave(SpawnableWave spawnableWave, out SpawnableWaveBase wave)
        {
            wave = spawnableWave switch
            {
                SpawnableWave.NtfWave => TryGetWave(out NtfSpawnWave ntfWave) ? ntfWave : null,
                SpawnableWave.NtfMiniWave => TryGetWave(out NtfMiniWave ntfMiniWave) ? ntfMiniWave : null,
                SpawnableWave.ChaosWave => TryGetWave(out ChaosSpawnWave chaosWave) ? chaosWave : null,
                SpawnableWave.ChaosMiniWave => TryGetWave(out ChaosMiniWave chaosMiniWave) ? chaosMiniWave : null,
                _ => null
            };

            return wave != null;
        }

        /// <summary>
        /// Spawns the specified <see cref="SpawnableWaveBase"/>.
        /// </summary>
        /// <param name="wave">The spawnable wave to spawn.</param>
        public static void ForceWave(SpawnableWaveBase wave) => WaveManager.Spawn(wave);

        /// <summary>
        /// Spawns a wave based of a <see cref="Faction"/>.
        /// </summary>
        /// <param name="faction">The faction to spawn.</param>
        public static void ForceWave(Faction faction)
        {
            if (TryGetWave(faction, out SpawnableWaveBase wave))
                ForceWave(wave);
        }

        /// <summary>
        /// Advance the timer from the specified faction.
        /// </summary>
        /// <param name="faction">The faction of the spawn wave.</param>
        /// <param name="time">The time to advance.</param>
        public static void AdvanceTimer(Faction faction, float time) => WaveManager.AdvanceTimer(faction, time);

        /// <summary>
        /// Summons the NTF chopper.
        /// </summary>
        public static void SummonNtfChopper()
        {
            if (TryGetWave(Faction.FoundationStaff, out SpawnableWaveBase wave))
                PlayEffect(wave);
        }

        /// <summary>
        /// Summons the <see cref="Side.ChaosInsurgency"/> van.
        /// </summary>
        public static void SummonChaosInsurgencyVan()
        {
            if (TryGetWave(Faction.FoundationEnemy, out SpawnableWaveBase wave))
                PlayEffect(wave);
        }

        /// <summary>
        /// Grants tickets to a <see cref="Faction"/>.
        /// </summary>
        /// <param name="team">The <see cref="Faction"/> to grant tickets to.</param>
        /// <param name="amount">The amount of tickets to grant.</param>
        public static void GrantTickets(Faction team, int amount)
        {
            if (TryGetWave(team, out SpawnableWaveBase wave) && wave is ILimitedWave limitedWave)
                limitedWave.RespawnTokens += Math.Max(0, limitedWave.RespawnTokens - amount);
        }

        /// <summary>
        /// Removes tickets from a <see cref="Faction"/>.
        /// </summary>
        /// <param name="team">The <see cref="Faction"/> to remove tickets from.</param>
        /// <param name="amount">The amount of tickets to remove.</param>
        public static void RemoveTickets(Faction team, int amount)
        {
            if (TryGetWave(team, out SpawnableWaveBase wave) && wave is ILimitedWave limitedWave)
                limitedWave.RespawnTokens = Math.Max(0, limitedWave.RespawnTokens - amount);
        }

        /// <summary>
        /// Modify tickets from a <see cref="Faction"/>.
        /// </summary>
        /// <param name="team">The <see cref="Faction"/> to modify tickets from.</param>
        /// <param name="amount">The amount of tickets to modify.</param>
        public static void ModifyTickets(Faction team, int amount)
        {
            if (TryGetWave(team, out SpawnableWaveBase wave) && wave is ILimitedWave limitedWave)
                limitedWave.RespawnTokens = amount;
        }
    }
}