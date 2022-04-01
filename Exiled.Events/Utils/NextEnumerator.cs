// -----------------------------------------------------------------------
// <copyright file="NextEnumerator.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Utils {
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// An implementation of <see cref="IEnumerator{T}"/> that
    /// allows to get two elements in one move.
    /// Very useful in patches.
    /// </summary>
    /// <typeparam name="T"><inheritdoc cref="IEnumerator{T}"/></typeparam>
    internal class NextEnumerator<T> : IEnumerator<T> {
        private readonly IEnumerator<T> inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="NextEnumerator{T}"/> class.
        /// </summary>
        /// <param name="enumerator">The innner <see cref="IEnumerator{T}"/>.</param>
        public NextEnumerator(IEnumerator<T> enumerator) => inner = enumerator ?? throw new ArgumentNullException(nameof(inner));

        /// <inheritdoc cref="IEnumerator{T}.Current"/>
        public T Current { get; private set; }

        /// <summary>
        /// Gets the next element in the collection.
        /// Might be null if there's no element after <see cref="Current"/>.
        /// </summary>
        public T NextCurrent { get; private set; }

        /// <inheritdoc/>
        object IEnumerator.Current => Current;

        /// <inheritdoc/>
        public void Dispose() => inner.Dispose();

        /// <inheritdoc/>
        public void Reset() {
            inner.Reset();
            ResetValues();
        }

        /// <inheritdoc/>
        public bool MoveNext() {
            if (inner.MoveNext()) {
                Current = inner.Current;
                inner.MoveNext();
                NextCurrent = inner.Current;
                return true;
            }

            ResetValues();
            return false;
        }

        private void ResetValues() {
            Current = default;
            NextCurrent = default;
        }
    }
}
