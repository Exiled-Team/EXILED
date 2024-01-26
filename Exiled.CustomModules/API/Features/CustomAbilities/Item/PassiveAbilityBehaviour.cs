// -----------------------------------------------------------------------
// <copyright file="PassiveAbilityBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.ItemAbilities
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.CustomModules.API.Features.CustomAbilities;

    /// <summary>
    /// Represents the base class for ability behaviors associated with an item, providing support for passive usage.
    /// </summary>
    public abstract class PassiveAbilityBehaviour : PassiveAbilityBehaviour<Item>
    {
        /// <inheritdoc/>
        public override bool DisposeOnNullOwner { get; protected set; } = false;

        /// <summary>
        /// Gets the item's owner.
        /// </summary>
        public Player ItemOwner => Owner.Owner;

        /// <inheritdoc/>
        protected override void FindOwner() => Owner = Item.Get(Base);
    }
}