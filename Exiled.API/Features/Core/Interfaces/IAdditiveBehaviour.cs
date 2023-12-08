// -----------------------------------------------------------------------
// <copyright file="IAdditiveBehaviour.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Interfaces
{
    using System;

    /// <summary>
    /// Defines a <see cref="EBehaviour"/> which is being set up through user-defined type component.
    /// </summary>
    public interface IAdditiveBehaviour : IAdditiveIdentifier
    {
        /// <summary>
        /// Gets the behaviour component.
        /// </summary>
        public Type BehaviourComponent { get; }
    }
}