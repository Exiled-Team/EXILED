// -----------------------------------------------------------------------
// <copyright file="EventPatchAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Attributes
{
    using System;

    using Exiled.Events.Interfaces;

    /// <summary>
    /// An attribute to contain data about an event patch.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class EventPatchAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventPatchAttribute"/> class.
        /// </summary>
        /// <param name="event">The <see cref="IEvent"/> to be raised by this patch.</param>
        internal EventPatchAttribute(IEvent @event)
        {
            Event = @event;
        }

        /// <summary>
        /// Gets the <see cref="IEvent"/> that will be raised by this patch.
        /// </summary>
        internal IEvent Event { get; }
    }
}
