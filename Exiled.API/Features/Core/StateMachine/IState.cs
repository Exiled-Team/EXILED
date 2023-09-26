// -----------------------------------------------------------------------
// <copyright file="IState.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.StateMachine
{
    /// <summary>
    /// Defines the contract for basic state features.
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Gets the state's id.
        /// </summary>
        public byte Id { get; }

        /// <summary>
        /// Gets the state's name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the state's description.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Fired when entering the state.
        /// </summary>
        /// <param name="stateController">The state controller.</param>
        public abstract void OnEnter(StateController stateController);

        /// <summary>
        /// Fired when exiting the state.
        /// </summary>
        /// <param name="stateController">The state controller.</param>
        public abstract void OnExit(StateController stateController);
    }
}