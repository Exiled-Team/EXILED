// -----------------------------------------------------------------------
// <copyright file="IAdditiveSettingsCollection.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Interfaces
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a <see cref="EBehaviour"/> which is being set up through user-defined settings.
    /// </summary>
    /// <typeparam name="T">The <see cref="EBehaviour"/> type.</typeparam>
    public interface IAdditiveSettingsCollection<T> : IAdditivePipe
        where T : IAdditiveProperty
    {
        /// <summary>
        /// Gets or sets the <typeparamref name="T"/> settings.
        /// </summary>
        public List<T> Settings { get; set; }
    }
}