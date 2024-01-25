// -----------------------------------------------------------------------
// <copyright file="IAdditiveSettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Interfaces
{
    /// <summary>
    /// Defines additive settings set up through user-defined properties.
    /// </summary>
    /// <typeparam name="T">The <see cref="EPlayerBehaviour"/> type.</typeparam>
    public interface IAdditiveSettings<T> : IAdditivePipe
        where T : IAdditiveProperty
    {
        /// <summary>
        /// Gets or sets the <typeparamref name="T"/> settings.
        /// </summary>
        public T Settings { get; set; }

        /// <summary>
        /// Gets or sets the configs.
        /// </summary>
        public object Config { get; set; }
    }
}