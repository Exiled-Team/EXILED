// -----------------------------------------------------------------------
// <copyright file="TestPickupConfig.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.TestPickup
{
    using Exiled.CustomModules.API.Features.Attributes;
    using Exiled.CustomModules.API.Features.CustomItems;
    using Exiled.CustomModules.API.Features.Generic;
    using Exiled.Example.TestItem;

    /// <inheritdoc/>
    [ModuleIdentifier]
    public class TestPickupConfig : ModulePointer<CustomItem>
    {
        /// <inheritdoc/>
        public override uint Id { get; set; } = CustomItemType.TestPickup;

        /// <summary>
        /// Gets or sets a string value.
        /// </summary>
        public string ValueString { get; set; } = "Value";
    }
}