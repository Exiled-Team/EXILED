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
    using System.Linq;

    using CustomPlayerEffects;
    using Enums;
    using PlayerRoles;
    using Respawning;
    using Respawning.NamingRules;
    using UnityEngine;

    /// <summary>
    /// A set of tools to handle team respawns more easily.
    /// </summary>
    public static class Respawn
    {
        private const string MtfChopperGameObjectName = "Chopper";
        private const string ChaosVanGameObjectName = "CIVanArrive";

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
                    ntfHelicopterGameObject = GameObject.Find(MtfChopperGameObjectName);

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
                    chaosCarGameObject = GameObject.Find(ChaosVanGameObjectName);

                return chaosCarGameObject;
            }
        }

        /// <summary>
        /// Gets or sets the next known <see cref="SpawnableTeamType"/> that will spawn.
        /// </summary>
        public static SpawnableTeamType NextKnownTeam
        {
            get => RespawnManager.Singleton.NextKnownTeam;
            set => RespawnManager.Singleton.NextKnownTeam = value;
        }

        /// <summary>
        /// Gets or sets the amount of seconds before the next respawn phase will occur.
        /// </summary>
        public static float TimeUntilNextPhase
        {
            get => RespawnManager.Singleton._timeForNextSequence - (float)RespawnManager.Singleton._stopwatch.Elapsed.TotalSeconds;
            set => RespawnManager.Singleton._timeForNextSequence = (float)RespawnManager.Singleton._stopwatch.Elapsed.TotalSeconds + value;
        }

        /// <summary>
        /// Gets a <see cref="TimeSpan"/> indicating the amount of time before the next respawn wave will occur.
        /// </summary>
        public static TimeSpan TimeUntilSpawnWave => TimeSpan.FromSeconds(TimeUntilNextPhase);

        /// <summary>
        /// Gets a <see cref="DateTime"/> indicating the moment in UTC time the next respawn wave will occur.
        /// </summary>
        public static DateTime NextTeamTime => DateTime.UtcNow.AddSeconds(TimeUntilSpawnWave.TotalSeconds);

        /// <summary>
        /// Gets a value indicating whether or not a team is currently being spawned or the animations are playing for a team.
        /// </summary>
        public static bool IsSpawning => RespawnManager.Singleton._curSequence is RespawnManager.RespawnSequencePhase.PlayingEntryAnimations or RespawnManager.RespawnSequencePhase.SpawningSelectedTeam;

        /// <summary>
        /// Gets or sets the amount of spawn tickets belonging to the Chaos Insurgency.
        /// </summary>
        /// <seealso cref="NtfTickets"/>
        public static float ChaosTickets
        {
            get => RespawnTokensManager.Counters[0].Amount;
            set => RespawnTokensManager.ModifyTokens(SpawnableTeamType.ChaosInsurgency, value);
        }

        /// <summary>
        /// Gets or sets the amount of spawn tickets belonging to the NTF.
        /// </summary>
        /// <seealso cref="ChaosTickets"/>
        public static float NtfTickets
        {
            get => RespawnTokensManager.Counters[1].Amount;
            set => RespawnTokensManager.ModifyTokens(SpawnableTeamType.NineTailedFox, value);
        }

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
        /// Generates a queue.
        /// </summary>
        /// <param name="team">The team to get queue of.</param>
        /// <param name="amount">Amount to get.</param>
        /// <returns>A queue of <see cref="RoleTypeId"/>.</returns>
        public static Queue<RoleTypeId> GenerateQueue(SpawnableTeamType team, int amount)
        {
            Queue<RoleTypeId> queue = new();

            if (!RespawnManager.SpawnableTeams.TryGetValue(team, out SpawnableTeamHandlerBase handler))
                return queue;

            handler.GenerateQueue(queue, amount);

            return queue;
        }

        /// <summary>
        /// Play an effect when a certain class spawns.
        /// </summary>
        /// <param name="effect">The effect to be played.</param>
        public static void PlayEffect(byte effect) => PlayEffects(new[] { effect });

        /// <summary>
        /// Play an effect when a certain class spawns.
        /// </summary>
        /// <param name="effect">The effect to be played.</param>
        public static void PlayEffect(RespawnEffectType effect) => PlayEffects(new[] { effect });

        /// <summary>
        /// Play effects when a certain class spawns.
        /// </summary>
        /// <param name="effects">The effects to be played.</param>
        public static void PlayEffects(byte[] effects)
        {
            foreach (RespawnEffectsController controller in RespawnEffectsController.AllControllers)
            {
                if (!controller)
                    continue;

                controller.RpcPlayEffects(effects);
            }
        }

        /// <summary>
        /// Play effects when a certain class spawns.
        /// </summary>
        /// <param name="effects">The effects to be played.</param>
        public static void PlayEffects(RespawnEffectType[] effects) => PlayEffects(effects.Select(effect => (byte)effect).ToArray());

        /// <summary>
        /// Summons the NTF chopper.
        /// </summary>
        public static void SummonNtfChopper() => PlayEffects(new[] { RespawnEffectType.SummonNtfChopper });

        /// <summary>
        /// Summons the <see cref="Side.ChaosInsurgency"/> van.
        /// </summary>
        /// <param name="playMusic">Whether or not to play the Chaos Insurgency spawn music.</param>
        public static void SummonChaosInsurgencyVan(bool playMusic = true)
        {
            PlayEffects(playMusic ? new[]
                {
                    RespawnEffectType.PlayChaosInsurgencyMusic,
                    RespawnEffectType.SummonChaosInsurgencyVan,
                }
                : new[]
                {
                    RespawnEffectType.SummonChaosInsurgencyVan,
                });
        }

        /// <summary>
        /// Grants tickets to a <see cref="SpawnableTeamType"/>.
        /// </summary>
        /// <param name="team">The <see cref="SpawnableTeamType"/> to grant tickets to.</param>
        /// <param name="amount">The amount of tickets to grant.</param>
        public static void GrantTickets(SpawnableTeamType team, float amount) => RespawnTokensManager.GrantTokens(team, Math.Max(0, amount));

        /// <summary>
        /// Removes tickets from a <see cref="SpawnableTeamType"/>.
        /// </summary>
        /// <param name="team">The <see cref="SpawnableTeamType"/> to remove tickets from.</param>
        /// <param name="amount">The amount of tickets to remove.</param>
        public static void RemoveTickets(SpawnableTeamType team, float amount) => RespawnTokensManager.RemoveTokens(team, Math.Max(0, amount));

        /// <summary>
        /// Modify tickets from a <see cref="SpawnableTeamType"/>.
        /// </summary>
        /// <param name="team">The <see cref="SpawnableTeamType"/> to modify tickets from.</param>
        /// <param name="amount">The amount of tickets to modify.</param>
        public static void ModifyTickets(SpawnableTeamType team, float amount) => RespawnTokensManager.ModifyTokens(team, amount);

        /// <summary>
        /// Forces a spawn of the given <see cref="SpawnableTeamType"/>.
        /// </summary>
        /// <param name="team">The <see cref="SpawnableTeamType"/> to spawn.</param>
        /// <param name="playEffects">Whether or not effects will be played with the spawn.</param>
        public static void ForceWave(SpawnableTeamType team, bool playEffects = false)
        {
            if (playEffects)
                RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.Selection, team);

            RespawnManager.Singleton.ForceSpawnTeam(team);
        }
    }
}