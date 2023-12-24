// -----------------------------------------------------------------------
// <copyright file="EPlayerBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core
{
    using Exiled.API.Features;
    using Exiled.API.Features.Core.Generic;

    /// <summary>
    /// <see cref="EPlayerBehaviour"/> is a versatile component designed to enhance the functionality of playable characters.
    /// <br>It can be easily integrated with various types of playable characters, making it a valuable tool for user-defined playable character behaviours.</br>
    /// </summary>
    /// <remarks>
    /// This abstract class serves as a versatile component within the framework, specifically designed to enhance the functionality
    /// of playable characters. It provides a foundation for user-defined behaviours that can be easily integrated with various types
    /// of playable characters, making it a valuable tool for customizing and extending the behavior of player entities.
    /// </remarks>
    public abstract class EPlayerBehaviour : EBehaviour<Player>
    {
        /// <inheritdoc/>
        protected override void FindOwner() => Owner = Player.Get(Base);
    }
}