// -----------------------------------------------------------------------
// <copyright file="DynamicDelegate.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.DynamicEvents
{
    using System;

    using Exiled.API.Features.Core;

    /// <summary>
    /// The <see cref="DynamicDelegate"/> allows user-defined delegate routes bound to an <see cref="object"/> reference.
    /// </summary>
    public class DynamicDelegate : TypeCastObject<DynamicEventDispatcher>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDelegate"/> class.
        /// </summary>
        public DynamicDelegate()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDelegate"/> class.
        /// </summary>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="delegate"><inheritdoc cref="Delegate"/></param>
        public DynamicDelegate(object target, Action @delegate)
        {
            Target = target;
            Delegate = @delegate;
        }

        /// <summary>
        /// Gets the <see cref="DynamicDelegate"/>'s target.
        /// </summary>
        public object Target { get; }

        /// <summary>
        /// Gets the <see cref="DynamicDelegate"/>'s delegate.
        /// </summary>
        public Action Delegate { get; }
    }
}