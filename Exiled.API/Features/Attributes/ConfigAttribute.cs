// -----------------------------------------------------------------------
// <copyright file="ConfigAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes
{
    using System;

    /// <summary>
    /// This attribute determines whether the class which is being applied to should be treated as <see cref="ConfigSubsystem"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigAttribute"/> class.
        /// </summary>
        /// <param name="folder">
        /// The folder where the config file is stored.
        /// This value is used to determine the path of the configuration file.
        /// </param>
        /// <param name="name">
        /// The name of the configuration file.
        /// This is the unique name used to identify the configuration file.
        /// </param>
        /// <param name="isParent">
        /// A value indicating whether this configuration acts as a parent config.
        /// If <see langword="true"/>, this config will manage child configurations.
        /// </param>
        /// <param name="isStandAlone">
        /// A value indicating whether this configuration is stand-alone and should not manage or be managed from other configurations.
        /// </param>
        public ConfigAttribute(
            string folder = null,
            string name = null,
            bool isParent = false,
            bool isStandAlone = false)
        {
            Folder = folder;
            Name = name;
            IsParent = isParent;
            IsStandAlone = isStandAlone;
        }

        /// <summary>
        /// Gets the folder's name.
        /// </summary>
        public string Folder { get; }

        /// <summary>
        /// Gets the file's name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the class on which this attribute is being applied to should be treated as parent <see cref="ConfigSubsystem"/>.
        /// </summary>
        public bool IsParent { get; }

        /// <summary>
        /// Gets a value indicating whether the config is individual.
        /// </summary>
        public bool IsStandAlone { get; }
    }
}