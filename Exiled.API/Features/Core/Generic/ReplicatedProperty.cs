// -----------------------------------------------------------------------
// <copyright file="ReplicatedProperty.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Generic
{
    using System;

    using Exiled.API.Features.Core.Attributes;

    /// <summary>
    /// Represents a class for managing the replication of a reference associated with a specific owner type.
    /// </summary>
    /// <typeparam name="TOwner">The type that owns or manages the replicated object.</typeparam>
    /// <typeparam name="TValue">The type of the replicated value.</typeparam>
    [ManagedObjectType]
    public class ReplicatedProperty<TOwner, TValue> : RepNotify<TValue>
    {
        private readonly Func<TOwner, TValue> getter;
        private readonly Action<TOwner, TValue> setter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplicatedProperty{TOwner, TValue}"/> class.
        /// </summary>
        /// <param name="getter">A function to get the value from the owner.</param>
        /// <param name="setter">An action to set the value for the owner.</param>
        /// <param name="initialOwner">The initial owner for which the value should be replicated.</param>
        /// <exception cref="ArgumentNullException">Thrown if either the getter or setter is null.</exception>
        public ReplicatedProperty(Func<TOwner, TValue> getter, Action<TOwner, TValue> setter, TOwner initialOwner)
            : base()
        {
            this.getter = getter ?? throw new ArgumentNullException(nameof(getter));
            this.setter = setter ?? throw new ArgumentNullException(nameof(setter));
            Owner = initialOwner;
        }

        /// <inheritdoc/>
        public override TValue Value
        {
            get => getter.Invoke(Owner);
            protected set
            {
                if (!Replicates)
                {
                    setter.Invoke(Owner, value);
                    return;
                }

                PreReplication(value);
            }
        }

        /// <summary>
        /// Gets or sets the owner of the <see cref="ReplicatedProperty{TOwner, TValue}"/>.
        /// </summary>
        public TOwner Owner { get; set; }

        /// <inheritdoc/>
        public override void Replicate(bool closeTransaction = false)
        {
            if (!Replicates)
                return;

            setter.Invoke(Owner, ReplicatedValue);

            if (closeTransaction)
                Destroy();
        }
    }
}