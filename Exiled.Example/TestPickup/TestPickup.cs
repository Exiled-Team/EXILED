// -----------------------------------------------------------------------
// <copyright file="TestPickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.TestPickup
{
    using Exiled.API.Enums;
    using Exiled.CustomModules.API.Features.Attributes;
    using Exiled.CustomModules.API.Features.CustomItems;
    using Exiled.Example.TestItem;

    /// <inheritdoc/>
    [ModuleIdentifier]
    public class TestPickup : CustomItem<TestPickupBehaviour>
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "TestPickup";

        /// <inheritdoc/>
        public override uint Id { get; set; } = CustomItemType.TestPickup;

        /// <inheritdoc/>
        public override bool IsEnabled { get; set; } = true;

        /// <inheritdoc/>
        public override string Description { get; set; } = "Custom pickup for testing purposes.";

        /// <inheritdoc/>
        public override ItemType ItemType { get; set; } = ItemType.Medkit;

        /// <inheritdoc/>
        public override SettingsBase Settings { get; set; } = new Settings()
        {
            PickedUpText = new("You picked up a test pickup!", 5, channel: TextChannelType.Broadcast),
        };
    }
}