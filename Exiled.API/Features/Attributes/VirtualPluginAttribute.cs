// -----------------------------------------------------------------------
// <copyright file="VirtualPluginAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes
{
    using System;

    using Exiled.API.Features.VirtualAssemblies;

    /// <summary>
    /// This attribute determines whether the class which is being applied to should be treated as <see cref="VirtualPlugin"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class VirtualPluginAttribute : Attribute
    {
#pragma warning disable SA1401 // Fields should be private
        /// <summary>
        /// The master branch's name.
        /// </summary>
        internal readonly string Master;

        /// <summary>
        /// The branch name.
        /// </summary>
        internal readonly string Name;

        /// <summary>
        /// The branch prefix.
        /// </summary>
        internal readonly string Prefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualPluginAttribute"/> class.
        /// </summary>
        /// <param name="master">The master project.</param>
        /// <param name="name">The branch name.</param>
        /// <param name="prefix">The branch prefix.</param>
        public VirtualPluginAttribute(string master, string name, string prefix)
        {
            Master = master;
            Name = name;
            Prefix = prefix;
        }
    }
}