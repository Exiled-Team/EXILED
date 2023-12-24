// -----------------------------------------------------------------------
// <copyright file="EItemBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core
{
    using Exiled.API.Features.Core.Generic;
    using Exiled.API.Features.Items;

    /// <summary>
    /// <see cref="EItemBehaviour"/> is a versatile component designed to enhance the functionality of items.
    /// <br>It can be easily integrated with various types of items, making it a valuable tool for user-defined item behaviours.</br>
    /// </summary>
    /// <remarks>
    /// This abstract class serves as a versatile component within the framework, specifically designed to enhance the functionality
    /// of items. It provides a foundation for user-defined behaviours that can be easily integrated with various types
    /// of items, making it a valuable tool for customizing and extending the behavior of item entities.
    /// </remarks>
    public abstract class EItemBehaviour : EBehaviour<Item>
    {
        /// <inheritdoc/>
        protected override void FindOwner() => Owner = Item.Get(Base);
    }
}