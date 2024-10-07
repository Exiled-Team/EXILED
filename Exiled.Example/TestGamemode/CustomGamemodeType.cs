// -----------------------------------------------------------------------
// <copyright file="CustomGamemodeType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.TestGamemode
{
    using Exiled.CustomModules.API.Enums;

    /// <summary>
    /// The custom gamemode type.
    /// </summary>
    public class CustomGamemodeType : UUGameModeType
    {
        /// <summary>
        /// Initializes a new custom gamemode id.
        /// </summary>
        public static readonly CustomGamemodeType TestGamemode = new();
    }
}