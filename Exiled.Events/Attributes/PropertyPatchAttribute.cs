// -----------------------------------------------------------------------
// <copyright file="PropertyPatchAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Attributes
{
    using System;

    using Exiled.API.Interfaces;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// An attribute to contain data about an event patch.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class PropertyPatchAttribute : Attribute
    {
        private readonly Type handlerType;
        private readonly string propertyName;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyPatchAttribute"/> class.
        /// </summary>
        /// <param name="eventName">The <see cref="Type"/> of the handler class that contains the event.</param>
        /// <param name="handlerType">The name of the event.</param>
        internal PropertyPatchAttribute(Type handlerType, string eventName)
        {
            this.handlerType = handlerType;
            this.propertyName = eventName;
        }

        /// <summary>
        /// Gets the <see cref="IProperty"/> that will be raised by this patch.
        /// </summary>
        internal IProperty Event => (IProperty)handlerType.GetProperty(propertyName)?.GetValue(null);
    }
}