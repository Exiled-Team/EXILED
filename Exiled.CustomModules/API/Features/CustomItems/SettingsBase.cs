// -----------------------------------------------------------------------
// <copyright file="SettingsBase.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomItems
{
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.API.Features.Spawn;

    using UnityEngine;

    /// <summary>
    /// Defines the contract for config features related to custom entities.
    /// </summary>
    public abstract class SettingsBase : TypeCastObject<SettingsBase>, IAdditiveProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsBase"/> class.
        /// </summary>
        public SettingsBase() => SettingsType = GetType().Name;

        /// <summary>
        /// Gets or sets the settings type.
        /// <br>This value is automatically set.</br>
        /// </summary>
        public virtual string SettingsType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Features.Spawn.SpawnProperties"/>.
        /// </summary>
        public virtual SpawnProperties SpawnProperties { get; set; } = new();

        /// <summary>
        /// Gets or sets the weight of the entity.
        /// </summary>
        public virtual float Weight { get; set; } = -1f;

        /// <summary>
        /// Gets or sets the scale of the entity.
        /// </summary>
        public virtual Vector3 Scale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets the <see cref="TextDisplay"/> to be displayed when the item has been picked up.
        /// </summary>
        public virtual TextDisplay PickedUpText { get; set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="TextDisplay"/> to be displayed when the item has been selected.
        /// </summary>
        public virtual TextDisplay SelectedText { get; set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether the item's name should be displayed to spectators when spectating the owner of the item.
        /// </summary>
        public virtual bool NotifyItemToSpectators { get; set; }
    }
}