// -----------------------------------------------------------------------
// <copyright file="SubscribeAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Events
{
    using System;

    /// <summary>
    /// Signal for a Subscription to the event service.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class SubscribeAttribute : Attribute
    {
    }
}
