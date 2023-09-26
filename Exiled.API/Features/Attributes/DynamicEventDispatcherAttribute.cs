// -----------------------------------------------------------------------
// <copyright file="DynamicEventDispatcherAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes
{
    using System;

    using Exiled.API.Features.DynamicEvents;

    /// <summary>
    /// An attribute to easily manage <see cref="DynamicEventDispatcher"/> initialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DynamicEventDispatcherAttribute : Attribute
    {
    }
}