// -----------------------------------------------------------------------
// <copyright file="EventAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Extra.Events
{
    using System;

    /// <summary>
    /// An attribute to mark listeners.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
#pragma warning disable SA1649
    public sealed class ListenerAttribute : Attribute
#pragma warning restore SA1649
    {
    }
}