// -----------------------------------------------------------------------
// <copyright file="CustomTeamType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.TestTeam
{
    using Exiled.CustomModules.API.Enums;

    /// <summary>
    /// The custom team type.
    /// </summary>
    public class CustomTeamType : UUCustomTeamType
    {
        /// <summary>
        /// Initializes a new custom team id.
        /// </summary>
        public static readonly CustomTeamType TestTeam = new();
    }
}