// -----------------------------------------------------------------------
// <copyright file="EventPatchAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Attributes
{
    using System;
    using System.Reflection;

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
        /// <param name="patchedType">The <see cref="Type"/> the patched method exists inside.</param>
        /// <param name="patchedMethodName">The name of the method to be patched.</param>
        /// <param name="event">The <see cref="IEvent"/> to be raised by this patch.</param>
        internal EventPatchAttribute(Type patchedType, string patchedMethodName, IEvent @event)
        {
            PatchedMethod = patchedType.GetMethod(patchedMethodName);
            Event = @event;
        }

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> that will be patched.
        /// </summary>
        internal MethodInfo PatchedMethod { get; }

        /// <summary>
        /// Gets the <see cref="IEvent"/> that will be raised by this patch.
        /// </summary>
        internal IEvent Event { get; }
    }
}
