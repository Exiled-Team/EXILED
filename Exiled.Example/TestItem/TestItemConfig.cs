// -----------------------------------------------------------------------
// <copyright file="TestItemConfig.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.TestItem
{
    using Exiled.CustomModules.API.Features.Attributes;
    using Exiled.CustomModules.API.Features.CustomItems;
    using Exiled.CustomModules.API.Features.Generic;

    /// <inheritdoc/>
    [ModuleIdentifier]
    public class TestItemConfig : ModulePointer<CustomItem>
    {
        /// <inheritdoc/>
        public override uint Id { get; set; } = CustomItemType.TestItem;

        /// <summary>
        /// Gets or sets a string value.
        /// </summary>
        public string ValueString { get; set; } = "Value";
    }
}