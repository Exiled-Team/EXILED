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

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class EventPatchAttribute : Attribute
    {
        public IEvent Event { get; }
        public EventPatchAttribute(IEvent @event)
        {
            Event = @event;
        }
    }
}
