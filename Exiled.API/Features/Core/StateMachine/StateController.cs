// -----------------------------------------------------------------------
// <copyright file="StateController.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.StateMachine
{
    using System.Collections.Generic;

    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.DynamicEvents;

    /// <summary>
    /// The base controller which handles actors using in-context states.
    /// </summary>
    public abstract class StateController : EActor
    {
        private readonly List<State> states = new();
        private State currentState;

        /// <summary>
        /// Gets all handled states.
        /// </summary>
        public IEnumerable<State> States => states;

        /// <summary>
        /// Gets or sets the current state.
        /// </summary>
        public State CurrentState
        {
            get => currentState;
            set
            {
                if (currentState.Id == value.Id)
                    return;

                (PreviousState = currentState).OnExit(this);
                (currentState = value).OnEnter(this);

                OnStateChanged();
            }
        }

        /// <summary>
        /// Gets or sets the previous state.
        /// </summary>
        public State PreviousState { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when entering a new state.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<State> BeginStateMulticastDispatcher { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all the delegates fired when exiting the current state.
        /// </summary>
        [DynamicEventDispatcher]
        public TDynamicEventDispatcher<State> EndStateMulticastDispatcher { get; set; }

        /// <summary>
        /// Fired every tick from the current state.
        /// </summary>
        /// <param name="state">The state firing the update.</param>
        public virtual void StateUpdate(State state)
        {
        }

        /// <summary>
        /// Fired when the state is changed.
        /// </summary>
        protected virtual void OnStateChanged()
        {
            EndStateMulticastDispatcher.InvokeAll(PreviousState);
            BeginStateMulticastDispatcher.InvokeAll(currentState);
        }
    }
}