// -----------------------------------------------------------------------
// <copyright file="CustomItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems
{
    using System;

    /// <inheritdoc/>
    public abstract class CustomItem<T> : CustomItem
        where T : ItemBehaviour
    {
        /// <inheritdoc/>
        public override Type BehaviourComponent => typeof(T);
    }
}
