// -----------------------------------------------------------------------
// <copyright file="ReplicatedRef.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Generics
{
    using Exiled.API.Features.Core.Attributes;

    /// <summary>
    /// Represents a class for managing the replication of a reference.
    /// </summary>
    /// <typeparam name="TRef">The type of the replicated value.</typeparam>
    [ManagedObjectType]
    public class ReplicatedRef<TRef> : RepNotify<TRef>
    {
#pragma warning disable SA1401 // Fields should be private
        /// <summary>
        /// The current value of the <see cref="ReplicatedRef{T}"/>.
        /// </summary>
        protected TRef value;

        /// <summary>
        /// The replicated value of the <see cref="ReplicatedRef{T}"/>.
        /// </summary>
        protected TRef replicatedValue;
#pragma warning restore SA1401 // Fields should be private

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplicatedRef{T}"/> class.
        /// </summary>
        /// <param name="value">The value to be replicated.</param>
        public ReplicatedRef(ref TRef value)
            : base()
        {
            this.value = value;
            replicatedValue = value;
        }

        /// <inheritdoc/>
        public override TRef Value
        {
            get => value;
            protected set
            {
                if (!Replicates)
                {
                    this.value = value;
                    Replicates = true;
                    return;
                }

                ReplicatingReferenceEventArgs<TRef> ev = new(this, value, true);
                ReplicatingReferenceDispatcher.InvokeAll(ev);

                if (!ev.IsAllowed)
                    return;

                ReplicatedValue = ev.ReplicatingValue;

                Replicate();
            }
        }

        /// <inheritdoc/>
        public override TRef ReplicatedValue
        {
            get => replicatedValue;
            protected set => Send(value);
        }
    }
}