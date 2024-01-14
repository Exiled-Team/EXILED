// -----------------------------------------------------------------------
// <copyright file="PlayerState.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features
{
    using Exiled.API.Features.Core;

    /// <summary>
    /// Represents the state of an individual player within the custom game mode, derived from <see cref="EPlayerBehaviour"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="PlayerState"/> class manages the behavior and interactions of a single player within the custom game mode.
    /// </para>
    /// <para>
    /// It serves as a base class for defining player-specific actions and responses within the context of the custom game mode.
    /// </para>
    /// </remarks>
    public abstract class PlayerState : EPlayerBehaviour
    {
    }
}