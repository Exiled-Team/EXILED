// -----------------------------------------------------------------------
// <copyright file="ServerHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.EventHandlers
{
    using Exiled.API.Features;
    using Exiled.CustomModules.API.Enums;
    using Exiled.CustomModules.API.Features;
    using Exiled.CustomModules.API.Features.CustomItems;

    /// <summary>
    /// Handles <see cref="Server"/> events.
    /// </summary>
    internal sealed class ServerHandler
    {
        private ModuleInfo CustomItemsModuleInfo { get; } = ModuleInfo.Get(UUModuleType.CustomItems.Name);

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRoundStarted"/>
        public void OnRoundStarted()
        {
            if (!CustomItemsModuleInfo.IsCurrentlyLoaded)
                return;

            foreach (CustomItem customItem in CustomItem.List)
                customItem.SpawnAll();
        }
    }
}