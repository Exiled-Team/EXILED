// -----------------------------------------------------------------------
// <copyright file="SummaryInfo.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomRoles
{
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using PlayerRoles;

    /// <summary>
    /// A tool to easily manage summary info.
    /// </summary>
    public struct SummaryInfo
    {
        /// <summary>
        /// The foundation forces.
        /// </summary>
        public int FoundationForces;

        /// <summary>
        /// The Chaos Insurgency forces.
        /// </summary>
        public int ChaosInsurgency;

        /// <summary>
        /// The anomalies.
        /// </summary>
        public int Anomalies;

        /// <summary>
        /// The neutral forces.
        /// </summary>
        public int Neutral;

        /// <summary>
        /// Gets the current summary.
        /// </summary>
        /// <returns>The current summary.</returns>
        public static SummaryInfo GetSummary()
        {
            SummaryInfo summary = new();

            foreach (Player alive in Player.List)
            {
                if (alive is not Pawn pawn || Round.IgnoredPlayers.Contains(alive))
                    continue;

                switch (RoleExtensions.GetTeam(alive.Role.Type))
                {
                    case Team.Scientists:
                    case Team.FoundationForces:
                        ++summary.FoundationForces;
                        break;
                    case Team.ClassD:
                    case Team.ChaosInsurgency:
                        ++summary.ChaosInsurgency;
                        break;
                    case Team.SCPs:
                        ++summary.Anomalies;
                        break;
                    default:
                        ++summary.Neutral;
                        break;
                }
            }

            return summary;
        }

        /// <summary>
        /// Updates the summary.
        /// </summary>
        public void Update()
        {
            foreach (Player alive in Player.List)
            {
                if (alive is not Pawn pawn || Round.IgnoredPlayers.Contains(alive))
                    continue;

                switch (RoleExtensions.GetTeam(alive.Role.Type))
                {
                    case Team.Scientists:
                    case Team.FoundationForces:
                        ++FoundationForces;
                        break;
                    case Team.ClassD:
                    case Team.ChaosInsurgency:
                        ++ChaosInsurgency;
                        break;
                    case Team.SCPs:
                        ++Anomalies;
                        break;
                    default:
                        ++Neutral;
                        break;
                }
            }
        }
    }
}