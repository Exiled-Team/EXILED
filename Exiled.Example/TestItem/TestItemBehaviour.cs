// -----------------------------------------------------------------------
// <copyright file="TestItemBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.TestItem
{
    using Exiled.API.Features;
    using Exiled.CustomModules.API.Features.CustomItems.Items;
    using Exiled.Events.EventArgs.Player;

    /// <inheritdoc />
    public class TestItemBehaviour : ItemBehaviour
    {
        /// <inheritdoc/>
        protected override void OnPickingUp(PickingUpItemEventArgs ev)
        {
            base.OnPickingUp(ev);

            Log.InfoWithContext("Test Item is being picked up.");
        }
    }
}