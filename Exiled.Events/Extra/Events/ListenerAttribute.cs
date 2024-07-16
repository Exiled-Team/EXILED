// -----------------------------------------------------------------------
// <copyright file="ListenerAttribute.cs" company="Exiled Team">
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
    public sealed class ListenerAttribute : Attribute
    {
    }
}