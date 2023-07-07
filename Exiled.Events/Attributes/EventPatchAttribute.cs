// -----------------------------------------------------------------------
// <copyright file="EventPatchAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Attributes
{
    using System;

    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// An attribute to contain data about an event patch.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class EventPatchAttribute : Attribute
    {
        private readonly Type handlerType;
        private readonly string eventName;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPatchAttribute"/> class.
        /// </summary>
        /// <param name="eventName">The <see cref="Type"/> of the handler class that contains the event.</param>
        /// <param name="handlerType">The name of the event.</param>
        internal EventPatchAttribute(Type handlerType, string eventName)
        {
            this.handlerType = handlerType;
            this.eventName = eventName;
        }

        /// <summary>
        /// Gets the <see cref="IExiledEvent"/> that will be raised by this patch.
        /// </summary>
        internal IExiledEvent Event => (IExiledEvent)handlerType.GetProperty(eventName)?.GetValue(null);
    }
}