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

    /// <summary>
    /// Represents the base class for implementing custom item behavior.
    /// </summary>
    public abstract class ItemBehaviour : EItemBehaviour, IAdditiveSettings<ItemSettings>
    {
        /// <summary>
        /// Gets the relative <see cref="CustomItems.CustomItem"/>.
        /// </summary>
        public CustomItem CustomItem { get; private set; }

        /// <inheritdoc/>
        public ItemSettings Settings { get; set; }

        /// <summary>
        /// Gets or sets the item's configs.
        /// </summary>
        public virtual object Config { get; set; }

        /// <inheritdoc/>
        public virtual void AdjustAdditivePipe()
        {
            if (CustomItem.TryGet(GetType(), out CustomItem customItem))
            {
                CustomItem = customItem;
                Settings = CustomItem.Settings;
            }

            if (Config is null)
                return;

            foreach (PropertyInfo propertyInfo in Config.GetType().GetProperties())
            {
                PropertyInfo targetInfo = Config.GetType().GetProperty(propertyInfo.Name);
                if (targetInfo is null)
                    continue;

                targetInfo.SetValue(Settings, propertyInfo.GetValue(Config, null));
            }
        }

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            AdjustAdditivePipe();
        }
    }
}