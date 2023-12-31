// -----------------------------------------------------------------------
// <copyright file="ItemBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems
{
    using System.Reflection;

    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.Events.EventArgs.Player;

    /// <summary>
    /// A tool to easily handle the custom item's logic.
    /// </summary>
    public abstract class ItemBehaviour : EItemBehaviour, IAdditiveSettings<ItemSettings>
    {
        /// <inheritdoc/>
        public ItemSettings Settings { get; set; }

        /// <summary>
        /// Gets the item's configs.
        /// </summary>
        protected virtual object ConfigRaw { get; private set; }

        /// <inheritdoc/>
        public abstract void AdjustAddittivePipe();

        /// <summary>
        /// Loads the given config.
        /// </summary>
        /// <param name="config">The config load.</param>
        protected virtual void LoadConfigs(object config)
        {
            if (config is null)
                return;

            foreach (PropertyInfo propertyInfo in config.GetType().GetProperties())
            {
                PropertyInfo targetInfo = typeof(ItemSettings).GetProperty(propertyInfo.Name);
                if (targetInfo is null)
                    continue;

                targetInfo.SetValue(Settings, propertyInfo.GetValue(config, null));
            }

            AdjustAddittivePipe();
        }

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            LoadConfigs(ConfigRaw);

            SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }
    }
}