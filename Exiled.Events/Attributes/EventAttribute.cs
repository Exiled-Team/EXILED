// -----------------------------------------------------------------------
// <copyright file="EventAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Attributes
{
    using System;

    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Attribute to allow patching.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class EventAttribute : Attribute
    {
        private readonly Type handlerType;
        private readonly string eventName;
        private readonly Func<bool> registerCondition;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventAttribute"/> class.
        /// </summary>
        /// <param name="eventName">The <see cref="Type"/> of the handler class that contains the event.</param>
        /// <param name="handlerType">The name of the event.</param>
        /// <param name="registerCondition">The condition. If null will always register, otherwise will check condition.</param>
        public EventAttribute(Type handlerType, string eventName, Func<bool> registerCondition = null)
        {
            this.handlerType = handlerType;
            this.eventName = eventName;
            this.registerCondition = registerCondition;
        }

        /// <summary>
        /// Gets the <see cref="IExiledEvent"/> that will be raised by this patch.
        /// </summary>
        public IExiledEvent Event => (IExiledEvent)handlerType.GetProperty(eventName)?.GetValue(null);

        /// <summary>
        /// Gets a value indicating whether or not the register condition was satisfied.
        /// </summary>
        public bool CheckCondition => registerCondition is null || registerCondition();
    }
}