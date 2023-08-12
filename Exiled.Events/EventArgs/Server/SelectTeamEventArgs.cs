// -----------------------------------------------------------------------
// <copyright file="SelectTeamEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using Exiled.Events.EventArgs.Interfaces;
    using PlayerRoles;
    using Respawning;

    /// <summary>
    ///     Contains all information for the selction of next wave of <see cref="SpawnableTeamType.NineTailedFox" /> or
    ///     <see cref="SpawnableTeamType.ChaosInsurgency" />.
    /// </summary>
    public class SelectTeamEventArgs : IDeniableEvent
    {
        private CustomTeamRespawnInfo? customSpawnableTeam;
        private SpawnableTeamType selectedTeam;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SelectTeamEventArgs"/> class.
        /// </summary>
        /// <param name="timeBeforeSpawning">
        ///      <inheritdoc cref="TimeBeforeSpawning" />
        /// </param>
        /// <param name="selectedTeam">
        ///     <inheritdoc cref="SelectedTeam" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public SelectTeamEventArgs(float timeBeforeSpawning, SpawnableTeamType selectedTeam, bool isAllowed = true)
        {
            TimeBeforeSpawning = timeBeforeSpawning;
            SelectedTeam = selectedTeam;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets or sets a value indicating the time before spawning of the next team.
        /// </summary>
        public float TimeBeforeSpawning { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating what the slected respawnable team is.
        ///     If the value is <see cref="SpawnableTeamType.None"/> check the customTeam.
        /// </summary>
        public SpawnableTeamType SelectedTeam
        {
            get => selectedTeam;
            set
            {
                if (value == SpawnableTeamType.None)
                {
                    if (customSpawnableTeam == null)
                        IsAllowed = false;
                }
                else
                {
                    if (customSpawnableTeam != null)
                        customSpawnableTeam = null;
                }

                selectedTeam = value;
            }
        }

        /// <summary>
        ///     Gets the selected spawnable team.
        ///     <see langword="null"/> if none team is slected or a custom team is selected.
        /// </summary>
        public SpawnableTeamHandlerBase SpawnableTeam
            => RespawnManager.SpawnableTeams.TryGetValue(SelectedTeam, out SpawnableTeamHandlerBase @base) ? @base : null;

        /// <summary>
        ///     Gets or sets the selected custom spawnable team.
        /// </summary>
        public CustomTeamRespawnInfo CustomSpawnableTeam
        {
            get => customSpawnableTeam.HasValue ? customSpawnableTeam.Value : default(CustomTeamRespawnInfo);
            set
            {
                customSpawnableTeam = value;
                selectedTeam = SpawnableTeamType.None;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the spawn can occur.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Contains values ​​use when calling <see cref="Exiled.Events.Handlers.Server.RespawningCustomTeam"/>.
        /// </summary>
        public struct CustomTeamRespawnInfo
        {
            /// <summary>
            ///     The team selected.
            /// </summary>
            public uint TeamId;

            /// <summary>
            ///     The numbers of players respawning.
            /// </summary>
            public int PlayerAmount;
        }
    }
}