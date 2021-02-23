// -----------------------------------------------------------------------
// <copyright file="RoundHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems
{
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;

    using UnityEngine;

    using static CustomItems;

    /// <summary>
    /// Event Handlers for the CustomItem API.
    /// </summary>
    internal sealed class RoundHandler
    {
        /// <inheritdoc cref="Events.Handlers.Server.OnRoundStarted"/>
        public void OnRoundStarted()
        {
            foreach (CustomItem customItem in Instance.ItemManagers)
                customItem?.SpawnAll();
        }
    }
}
