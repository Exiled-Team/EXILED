// -----------------------------------------------------------------------
// <copyright file="CustomAbilityAttribute.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.API.Features.Attributes
{
    using System;

    /// <summary>
    /// An attribute to easily manage CustomAbility initialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class CustomAbilityAttribute : Attribute
    {
    }
}
