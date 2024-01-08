// -----------------------------------------------------------------------
// <copyright file="ReplicatingReferenceEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Generics
{
    using System;

    using Exiled.API.Interfaces;

    /// <summary>
    /// Contains all information before replicating a <see cref="IRepNotify{TRep}"/>.
    /// </summary>
    /// <typeparam name="T">The type of data to be replicated.</typeparam>
    public class ReplicatingReferenceEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReplicatingReferenceEventArgs{T}"/> class.
        /// </summary>
        /// <param name="repNotify"><inheritdoc cref="RepNotify"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        /// <param name="replicatingValue"><inheritdoc cref="ReplicatingValue"/></param>
        public ReplicatingReferenceEventArgs(IRepNotify<T> repNotify, T replicatingValue, bool isAllowed = true)
        {
            RepNotify = repNotify;
            ReplicatingValue = replicatingValue;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="IRepNotify{T}"/>.
        /// </summary>
        public IRepNotify<T> RepNotify { get; }

        /// <summary>
        /// Gets or sets the value which is being replicated.
        /// </summary>
        public T ReplicatingValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="RepNotify"/> can be replicated.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}