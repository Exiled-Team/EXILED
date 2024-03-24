// -----------------------------------------------------------------------
// <copyright file="Settings.cs" company="Exiled Team">
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
    public abstract class Settings : TypeCastObject<Settings>, IAdditiveProperty
    {
        /// <summary>
        /// Gets or sets the custom entity's <see cref="global::ItemType"/>.
        /// </summary>
        public virtual ItemType ItemType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Exiled.API.Features.Spawn.SpawnProperties"/>.
        /// </summary>
        public virtual SpawnProperties SpawnProperties { get; set; }

        /// <summary>
        /// Gets or sets the weight of the entity.
        /// </summary>
        public virtual float Weight { get; set; }

        /// <summary>
        /// Gets or sets the scale of the entity.
        /// </summary>
        public virtual Vector3 Scale { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TextDisplay"/> to be displayed when the entity has been picked up.
        /// </summary>
        public virtual TextDisplay PickedUpText { get; set; }
    }
}