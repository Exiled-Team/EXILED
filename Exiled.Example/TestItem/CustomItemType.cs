// -----------------------------------------------------------------------
// <copyright file="CustomItemType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.TestItem
{
    using Exiled.CustomModules.API.Enums;

    /// <inheritdoc />
    public class CustomItemType : UUCustomItemType
    {
        /// <summary>
        /// Initializes a new custom item id for test item.
        /// </summary>
        public static readonly CustomItemType TestItem = new();

        /// <summary>
        /// Initializes a new custom item id for test pickup.
        /// </summary>
        public static readonly CustomItemType TestPickup = new();
    }
}