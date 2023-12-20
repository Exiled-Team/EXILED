// -----------------------------------------------------------------------
// <copyright file="TypeCastMono.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core
{
    using Exiled.API.Features.Core.Interfaces;
    using UnityEngine;

    /// <summary>
    /// The interface which allows defined mono behaviours to be cast to each other.
    /// </summary>
    /// <typeparam name="T">The type of the mono behaviour to cast.</typeparam>
    public abstract class TypeCastMono<T> : MonoBehaviour, ITypeCast<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeCastMono{T}"/> class.
        /// </summary>
        protected TypeCastMono()
        {
        }

        /// <inheritdoc/>
        public TObject Cast<TObject>()
            where TObject : class, T => this as TObject;

        /// <inheritdoc/>
        public bool Cast<TObject>(out TObject param)
            where TObject : class, T
        {
            if (this is not TObject cast)
            {
                param = default;
                return false;
            }

            param = cast;
            return true;
        }

        /// <inheritdoc/>
        public TObject As<TObject>()
            where TObject : class, T => Cast<TObject>();

        /// <inheritdoc/>
        public bool Is<TObject>(out TObject param)
            where TObject : class, T => Cast(out param);
    }
}