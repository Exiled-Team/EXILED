// -----------------------------------------------------------------------
// <copyright file="IAdditiveBehaviours.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Interfaces
{
    using System;

    /// <summary>
    /// Defines a behaviours array which is being set up through user-defined type components.
    /// </summary>
    public interface IAdditiveBehaviours : IAdditiveIdentifier
    {
        /// <summary>
        /// Gets the behaviour components.
        /// </summary>
        public Type[] BehaviourComponents { get; }
    }
}