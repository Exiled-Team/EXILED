// -----------------------------------------------------------------------
// <copyright file="IRepNotify.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces
{
    using Exiled.API.Features.Core.Generic;

    /// <summary>
    /// Represents an interface for objects that provide replication notifications.
    /// </summary>
    /// <typeparam name="TRep">The type of the replicated value.</typeparam>
    public interface IRepNotify<TRep>
    {
        /// <summary>
        /// Gets the value of the <see cref="IRepNotify{T}"/>.
        /// </summary>
        public TRep Value { get; }

        /// <summary>
        /// Gets the replicated value.
        /// </summary>
        public TRep ReplicatedValue { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="IRepNotify{T}"/> is able to replicate.
        /// </summary>
        public bool Replicates { get; set; }

        /// <summary>
        /// Replicates the current <see cref="ReplicatedValue"/> to <see cref="Value"/>.
        /// </summary>
        /// <param name="closeTransaction">A value indicating whether the <see cref="ReplicatedRef{T}"/> replication transaction should be closed and disposed.</param>
        public void Replicate(bool closeTransaction = false);

        /// <summary>
        /// Replicates <paramref name="value"/> to <see cref="Value"/>.
        /// <para/>
        /// The <see cref="IRepNotify{TRep}"/> will have different values, and they will need to be replicated again using <see cref="Replicate(bool)"/>.
        /// </summary>
        /// <param name="value">The value to be replicated.</param>
        public void Replicate(TRep value);

        /// <summary>
        /// Sends a new value to the <see cref="ReplicatedValue"/>.
        /// <para/>
        /// The <see cref="IRepNotify{TRep}"/> will have different values, and they will need to be replicated again using <see cref="Replicate(bool)"/>.
        /// </summary>
        /// <param name="value">The value to be replicated.</param>
        public void Send(TRep value);
    }
}
