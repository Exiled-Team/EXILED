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

            Log.ErrorWithContext("Test Item is being picked up.");
        }

        /// <inheritdoc/>
        protected override void OnAcquired(bool displayMessage = true)
        {
            base.OnAcquired(displayMessage);

            Log.ErrorWithContext("Test Item is was picked up.");
        }

        /// <inheritdoc/>
        protected override void OnDropping(DroppingItemEventArgs ev)
        {
            base.OnDropping(ev);

            Log.ErrorWithContext("Test Item is being dropped.");
        }
    }
}