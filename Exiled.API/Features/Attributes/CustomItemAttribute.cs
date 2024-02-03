// -----------------------------------------------------------------------
// <copyright file="CustomItemAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

<<<<<<< HEAD:Exiled.API/Features/Attributes/CustomItemAttribute.cs
namespace Exiled.API.Features.Attributes
=======
namespace Exiled.CustomModules.API.Features.Attributes
>>>>>>> apis-rework:Exiled.CustomModules/API/Features/Attributes/CustomItemAttribute.cs
{
    using System;

    /// <summary>
    /// An attribute to easily manage CustomItem initialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class CustomItemAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomItemAttribute"/> class.
        /// </summary>
        /// <param name="type">The <see cref="global::ItemType"/> to serialize.</param>
        public CustomItemAttribute(ItemType type)
        {
            ItemType = type;
        }

        /// <summary>
        /// Gets the attribute's <see cref="global::ItemType"/>.
        /// </summary>
        public ItemType ItemType { get; }
    }
}