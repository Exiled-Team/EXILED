// -----------------------------------------------------------------------
// <copyright file="RoundHandler.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.CustomItems
{
    using SEXiled.CustomItems.API.Features;

    /// <summary>
    /// Event Handlers for the CustomItem API.
    /// </summary>
    internal sealed class RoundHandler
    {
        /// <inheritdoc cref="Events.Handlers.Server.OnRoundStarted"/>
        public void OnRoundStarted()
        {
            foreach (CustomItem customItem in CustomItem.Registered)
                customItem?.SpawnAll();
        }
    }
}
