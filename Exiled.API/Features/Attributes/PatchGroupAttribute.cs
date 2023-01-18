// -----------------------------------------------------------------------
// <copyright file="PatchGroupAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes
{
    using System;
    using System.Reflection;

    /// <summary>
    /// This attribute is used within Harmony patches and the relative annotations to define a target element.
    /// <br>A target element can be a <see cref="Type"/>, <see langword="class"/> or <see cref="MethodInfo"/>.</br>
    /// <para>
    /// <br><see cref="PatchGroupAttribute"/> allows to define elements made for patching.</br>
    /// <br>A target-patching approach is useful to patch specific elements without having to directly patch everything without considering excluded elements.</br>
    /// <br>Target-patching also allows to emit patches without having to manually define and/or supply parameters and elements made for patching</br>
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PatchGroupAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PatchGroupAttribute"/> class.
        /// </summary>
        /// <param name="groupId">The group of target-patch.</param>
        public PatchGroupAttribute(string groupId)
        {
            GroupId = groupId;
        }

        /// <summary>
        /// Gets the group id.
        /// </summary>
        public string GroupId { get; }
    }
}