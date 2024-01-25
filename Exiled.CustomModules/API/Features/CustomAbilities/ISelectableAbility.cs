// -----------------------------------------------------------------------
// <copyright file="ISelectableAbility.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomAbilities
{
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.CustomModules.API.Features.PlayerAbilities;

    /// <summary>
    /// Represents a marker interface for custom ability that can be selected.
    /// </summary>
    public interface ISelectableAbility
    {
        /// <summary>
        /// Gets a value indicating whether the ability can be selected.
        /// </summary>
        public bool IsSelectable { get; }

        /// <summary>
        /// Gets a value indicating whether the ability is selected.
        /// </summary>
        public bool IsSelected { get; }

        /// <summary>
        /// Selects the ability.
        /// </summary>
        public void Select();

        /// <summary>
        /// Unselects the ability.
        /// </summary>
        public void Unselect();
    }
}