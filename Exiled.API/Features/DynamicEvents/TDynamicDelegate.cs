// -----------------------------------------------------------------------
// <copyright file="TDynamicDelegate.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.DynamicEvents
{
    using System;

    using Exiled.API.Features.Core;

    /// <summary>
    /// The <see cref="TDynamicDelegate{T}"/> allows user-defined delegate routes bound to an <see cref="object"/> reference.
    /// </summary>
    /// <typeparam name="T">The delegate type parameter.</typeparam>
    public class TDynamicDelegate<T> : TypeCastObject<DynamicEventDispatcher>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TDynamicDelegate{T}"/> class.
        /// </summary>
        public TDynamicDelegate()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TDynamicDelegate{T}"/> class.
        /// </summary>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="delegate"><inheritdoc cref="Delegate"/></param>
        public TDynamicDelegate(object target, Action<T> @delegate)
        {
            Target = target;
            Delegate = @delegate;
        }

        /// <summary>
        /// Gets the <see cref="TDynamicDelegate{T}"/>'s target.
        /// </summary>
        public object Target { get; }

        /// <summary>
        /// Gets the <see cref="TDynamicDelegate{T}"/>'s delegate.
        /// </summary>
        public Action<T> Delegate { get; }

        /// <summary>
        /// Implicitly converts the <see cref="TDynamicDelegate{T}"/> instance to a <see cref="Action{T}"/>.
        /// </summary>
        /// <param name="del">The <see cref="TDynamicDelegate{T}"/> instance.</param>
        public static implicit operator Action<T>(TDynamicDelegate<T> del) => del.Delegate;

        /// <summary>
        /// Declares a new <see cref="TDynamicDelegate{T}"/> instance.
        /// </summary>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="delegate"><inheritdoc cref="Delegate"/></param>
        /// <returns>The new <see cref="TDynamicDelegate{T}"/> instance.</returns>
        public static TDynamicDelegate<T> DeclareMulticastDelegate(object target, Action<T> @delegate) => new(target, @delegate);
    }
}