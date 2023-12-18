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
    using Exiled.API.Features.Pickups;

    /// <summary>
    /// <see cref="EPickupBehaviour"/> is a versatile component designed to enhance the functionality of pickups.
    /// <br>It can be easily integrated with various types of pickups, making it a valuable tool for user-defined pickup behaviours.</br>
    /// </summary>
    /// <remarks>
    /// This abstract class serves as a versatile component within the framework, specifically designed to enhance the functionality
    /// of pickups. It provides a foundation for user-defined behaviours that can be easily integrated with various types
    /// of pickups, making it a valuable tool for customizing and extending the behavior of pickup entities.
    /// </remarks>
    public abstract class EPickupBehaviour : EBehaviour<Pickup>
    {
        /// <inheritdoc/>
        protected override void FindOwner() => Owner = Pickup.Get(Base);
    }
}