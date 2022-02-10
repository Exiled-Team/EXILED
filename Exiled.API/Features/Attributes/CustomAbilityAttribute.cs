// -----------------------------------------------------------------------
// <copyright file="CustomAbilityAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes
{
    using System;

    /// <summary>
    /// An attribute to easily manage Exiled features' behavior.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class CustomAbilityAttribute : Attribute
    {
    }
}
