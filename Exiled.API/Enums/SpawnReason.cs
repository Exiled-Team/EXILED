// -----------------------------------------------------------------------
// <copyright file="SpawnReason.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable CS1591
#pragma warning disable SA1602
namespace Exiled.API.Enums
{
    /// <summary>
    /// Possible spawn reasons.
    /// </summary>
    public enum SpawnReason : byte // TOTO: Remove this file and use Basegame
    {
        None = 0,
        RoundStart = 1,
        LateJoin = 2,
        Respawn = 3,
        Died = 4,
        Escaped = 5,
        Revived = 6,
        RemoteAdmin = 7,
        Destroyed = 8,
        Plugin = byte.MaxValue,
    }
}