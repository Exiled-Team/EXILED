// -----------------------------------------------------------------------
// <copyright file="GameState.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features
{
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Interfaces;

    /// <summary>
    /// Represents the state of the game on the server within the custom game mode, derived from <see cref="EActor"/> and implementing <see cref="IAdditiveSettings{T}"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="GameState"/> class encapsulates the current state of the game during rounds within the custom game mode.
    /// </para>
    /// <para>
    /// It serves as the foundation for defining and enforcing rules, conditions, and settings specific to the custom game mode on the server.
    /// </para>
    /// </remarks>
    public abstract class GameState : EActor, IAdditiveSettings<GameModeSettings>
    {
        /// <summary>
        /// Gets or sets the settings associated with the custom game mode.
        /// </summary>
        public GameModeSettings Settings { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="GameState"/>'s config.
        /// </summary>
        public object Config { get; set; }

        /// <inheritdoc/>
        public virtual void AdjustAdditivePipe()
        {
        }
    }
}