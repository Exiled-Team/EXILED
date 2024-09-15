// -----------------------------------------------------------------------
// <copyright file="TestPickupBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.TestPickup
{
    using Exiled.API.Features;
    using Exiled.CustomModules.API.Features.CustomItems.Pickups;
    using Exiled.Events.EventArgs.Player;

    /// <inheritdoc />
    public class TestPickupBehaviour : PickupBehaviour
    {
        /// <inheritdoc/>
        protected override void OnPickingUp(PickingUpItemEventArgs ev)
        {
            base.OnPickingUp(ev);

            Log.InfoWithContext("Test Pickup is being picked up.");
        }

        /// <inheritdoc/>
        protected override void OnAcquired(Player player, bool displayMessage = true)
        {
            base.OnAcquired(player, displayMessage);

            Log.InfoWithContext("Test Pickup has been picked up.");

            player.Heal(10f, true);
        }
    }
}