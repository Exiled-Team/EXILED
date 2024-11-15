// -----------------------------------------------------------------------
// <copyright file="TestItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.TestItem
{
    using Exiled.CustomModules.API.Features.Attributes;
    using Exiled.CustomModules.API.Features.CustomItems;

    /// <inheritdoc/>
    [ModuleIdentifier]
    public class TestItem : CustomItem<TestItemBehaviour>
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "TestItem";

        /// <inheritdoc/>
        public override uint Id { get; set; } = CustomItemType.TestItem;

        /// <inheritdoc/>
        public override bool IsEnabled { get; set; } = true;

        /// <inheritdoc/>
        public override string Description { get; set; } = "Custom item for testing purposes.";

        /// <inheritdoc/>
        public override ItemType ItemType { get; set; } = ItemType.Coin;

        /// <inheritdoc/>
        public override SettingsBase Settings { get; set; } = new Settings()
        {
            PickedUpText = new("You picked up a test item!"),
        };
    }
}