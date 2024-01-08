// -----------------------------------------------------------------------
// <copyright file="RepNotify.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Generics
{
    using System.Reflection;

    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Attributes;
    using Exiled.API.Features.DynamicEvents;
    using Exiled.API.Interfaces;

    /// <summary>
    /// Represents a class for managing the replication of a reference associated with a specific owner type.
    /// </summary>
    /// <typeparam name="TRep">The type of the replicated value.</typeparam>
    [ManagedObjectType]
    public class RepNotify<TRep> : EObject, IRepNotify<TRep>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepNotify{TRep}"/> class.
        /// </summary>
        public RepNotify()
        {
            if (GetType().GetCustomAttribute<ManagedObjectTypeAttribute>() is not null && GetObjectTypeFromRegisteredTypes(GetType()) is null)
                RegisterObjectType(GetType(), GetType().Name);
        }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired before replicating the <see cref="RepNotify{TRep}"/>.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<ReplicatingReferenceEventArgs<TRep>> ReplicatingReferenceDispatcher { get; protected set; }

        /// <inheritdoc/>
        public virtual bool Replicates { get; set; }

        /// <inheritdoc/>
        public virtual TRep Value { get; protected set; }

        /// <inheritdoc/>
        public virtual TRep ReplicatedValue { get; protected set; }

        /// <inheritdoc/>
        public virtual void Replicate(bool closeTransaction = false)
        {
            if (!Replicates)
                return;

            Value = ReplicatedValue;

            if (closeTransaction)
                Destroy();
        }

        /// <inheritdoc/>
        public virtual void Replicate(TRep value)
        {
            if (!Replicates)
                return;

            Replicates = false;
            Value = value;
            Replicates = true;
        }

        /// <inheritdoc/>
        public virtual void Send(TRep value)
        {
            if (!Replicates)
                return;

            ReplicatedValue = value;
        }

        /// <summary>
        /// Provides pre-replication logic to be used along with <see cref="Value"/> setter.
        /// </summary>
        /// <param name="val">The value to replicate.</param>
        protected virtual void PreReplication(TRep val)
        {
            ReplicatingReferenceEventArgs<TRep> ev = new(this, val, true);
            ReplicatingReferenceDispatcher.InvokeAll(ev);

            if (!ev.IsAllowed)
                return;

            ReplicatedValue = ev.ReplicatingValue;

            Replicate();
        }
    }
}