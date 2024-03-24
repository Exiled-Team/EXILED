// -----------------------------------------------------------------------
// <copyright file="State.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.StateMachine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features;
    using Exiled.API.Features.Core.Attributes;

    /// <summary>
    /// The base class which handles in-context states.
    /// </summary>
    public abstract class State : EActor, IState
    {
        private static readonly List<State> StatesValue = new();

        private readonly HashSet<StateController> controllers = new();
        private readonly HashSet<StateController> activeControllers = new();
        private readonly HashSet<StateController> inactiveControllers = new();

        /// <summary>
        /// Gets all registered states.
        /// </summary>
        public static IEnumerable<State> List => StatesValue;

        /// <inheritdoc/>
        public abstract byte Id { get; }

        /// <inheritdoc/>
        public virtual new string Name { get; }

        /// <inheritdoc/>
        public virtual string Description { get; }

        /// <summary>
        /// Gets all the <see cref="StateController"/>s listening to this <see cref="State"/>.
        /// </summary>
        public IEnumerable<StateController> Controllers => controllers;

        /// <summary>
        /// Gets all the <see cref="StateController"/>s running on this <see cref="State"/>.
        /// </summary>
        public IEnumerable<StateController> ActiveControllers => activeControllers;

        /// <summary>
        /// Gets all the <see cref="StateController"/>s running on a <see cref="State"/> other than this.
        /// </summary>
        public IEnumerable<StateController> InactiveControllers => inactiveControllers;

        /// <summary>
        /// Initializes all states defined in the executing <see cref="Assembly"/>.
        /// </summary>
        /// <param name="useAttribute">A value indicating whether attribute should be used.</param>
        public static void InitializeStates(bool useAttribute = true)
        {
            foreach (Type type in Assembly.GetCallingAssembly().GetTypes().Where(t => t.IsClass && !t.IsAbstract))
            {
                if (!type.IsSubclassOf(typeof(State)) && type.BaseType != typeof(State))
                    continue;

                if (useAttribute)
                {
                    StateAttribute stateAtt = type.GetCustomAttribute<StateAttribute>();
                    if (stateAtt is null)
                    {
                        Log.Error($"A {nameof(State)} cannot be initialized due to missing {nameof(StateAttribute)} attribute");
                        continue;
                    }
                }

                State state = CreateDefaultSubobject<State>(type);
                if (state is null)
                    continue;

                if (StatesValue.Any(s => s.Id == state.Id))
                {
                    Log.Error($"Couldn't initialize the state cannot be initialized due to missing {nameof(StateAttribute)} attribute");
                    continue;
                }

                StatesValue.Add(state);
            }
        }

        /// <summary>
        /// Gets the <see cref="State"/> belonging to the specified id.
        /// </summary>
        /// <param name="id">The state's id.</param>
        /// <returns>The corresponding <see cref="State"/>, or <see langword="null"/> if not found.</returns>
        public static State Get(byte id) => List.FirstOrDefault(s => s.Id == id);

        /// <summary>
        /// Gets the <see cref="State"/> belonging to the specified name.
        /// </summary>
        /// <param name="name">The state's name.</param>
        /// <returns>The corresponding <see cref="State"/>, or <see langword="null"/> if not found.</returns>
        public static State Get(string name) => List.FirstOrDefault(s => s.Name == name);

        /// <summary>
        /// Gets the <see cref="State"/> belonging to the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The state's <see cref="Type"/>.</param>
        /// <returns>The corresponding <see cref="State"/>, or <see langword="null"/> if not found.</returns>
        public static State Get(Type type) => List.FirstOrDefault(s => s.GetType() == type);

        /// <summary>
        /// Gets the <see cref="State"/>s belonging to the specified id between the defined range.
        /// </summary>
        /// <param name="minRange">The minimum inclusive range.</param>
        /// <param name="maxRange">The maximum exclusive range.</param>
        /// <returns>All <see cref="State"/>s belonging to the specified id between the specified range.</returns>
        public static IEnumerable<State> Get(byte minRange, byte maxRange)
        {
            for (byte i = minRange; i < maxRange; i++)
            {
                State state = Get(i);
                if (state is not null)
                    yield return Get(i);
            }
        }

        /// <summary>
        /// Gets the <see cref="State"/>s belonging to the specified id between the defined ids.
        /// </summary>
        /// <param name="ids">The ids to look for.</param>
        /// <returns>All <see cref="State"/>s belonging to the specified id between the defined ids.</returns>
        public static IEnumerable<State> Get(params byte[] ids)
        {
            foreach (byte id in ids)
            {
                State state = Get(id);
                if (state is not null)
                    yield return Get(id);
            }
        }

        /// <summary>
        /// Gets the <see cref="State"/>s belonging to the specified id between the defined names.
        /// </summary>
        /// <param name="names">The names to look for.</param>
        /// <returns>All <see cref="State"/>s belonging to the specified id between the defined names.</returns>
        public static IEnumerable<State> Get(params string[] names)
        {
            foreach (string name in names)
            {
                State state = Get(name);
                if (state is not null)
                    yield return state;
            }
        }

        /// <summary>
        /// Gets the <see cref="State"/>s belonging to the specified id between the defined types.
        /// </summary>
        /// <param name="types">The types to look for.</param>
        /// <returns>All <see cref="State"/>s belonging to the specified id between the defined types.</returns>
        public static IEnumerable<State> Get(params Type[] types)
        {
            foreach (Type type in types)
            {
                State state = Get(type);
                if (state is not null)
                    yield return Get(type);
            }
        }

        /// <summary>
        /// Gets the <see cref="State"/> belonging to the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to look for.</typeparam>
        /// <returns>The corresponding <see cref="State"/>, or <see langword="null"/> if not found.</returns>
        public static T Get<T>()
            where T : State => Get(typeof(T)).Cast<T>();

        /// <summary>
        /// Tries to get a <see cref="State"/> given the specified id.
        /// </summary>
        /// <param name="id">The state's id.</param>
        /// <param name="state">The corresponding <see cref="State"/>, or <see langword="null"/> if not found.</param>
        /// <returns><see langword="true"/> if a <see cref="State"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(byte id, out State state) => (state = Get(id)) is not null;

        /// <summary>
        /// Tries to get a <see cref="State"/> given the specified name.
        /// </summary>
        /// <param name="name">The state's name.</param>
        /// <param name="state">The corresponding <see cref="State"/>, or <see langword="null"/> if not found.</param>
        /// <returns><see langword="true"/> if a <see cref="State"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(string name, out State state) => (state = Get(name)) is not null;

        /// <summary>
        /// Tries to get a <see cref="State"/> given the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The state's <see cref="Type"/>.</param>
        /// <param name="state">The corresponding <see cref="State"/>, or <see langword="null"/> if not found.</param>
        /// <returns><see langword="true"/> if a <see cref="State"/> was found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Type type, out State state) => (state = Get(type)) is not null;

        /// <summary>
        /// Tries to get a <see cref="State"/> given the specified <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">The type to look for.</typeparam>
        /// <param name="state">The corresponding <see cref="State"/>, or <see langword="null"/> if not found.</param>
        /// <returns><see langword="true"/> if a <see cref="State"/> was found; otherwise, <see langword="false"/>.</returns>
        public bool TryGet<T>(out T state)
            where T : State => (state = Get<T>()) is not null;

        /// <summary>
        /// Converts the <see cref="State"/> to a human readable <see cref="string"/> representation.
        /// </summary>
        /// <returns>A human readable <see cref="string"/> representation of the <see cref="State"/> object.</returns>
        public override string ToString() => $"State - {Name} ({Id})";

        /// <inheritdoc/>
        public virtual void OnEnter(StateController stateController)
        {
            if (stateController.CurrentState != this)
            {
                throw new InvalidOperationException($"{nameof(State)}::{nameof(OnEnter)} - State mismatch: ({stateController.CurrentState})/({this})\n" +
                    $"Are you trying to invoke this method from outside the {nameof(StateController)} environment?");
            }

            activeControllers.Add(stateController);
            inactiveControllers.Remove(stateController);
        }

        /// <inheritdoc/>
        public virtual void OnExit(StateController stateController)
        {
            if (stateController.PreviousState != this)
            {
                throw new InvalidOperationException($"{nameof(State)}::{nameof(OnExit)} - State mismatch: ({stateController.PreviousState})/({this})\n" +
                    $"Are you trying to invoke this method from outside the {nameof(StateController)} environment?");
            }

            inactiveControllers.Add(stateController);
            activeControllers.Remove(stateController);
        }

        /// <inheritdoc/>
        protected override void Tick()
        {
            foreach (StateController controller in ActiveControllers)
            {
                if (controller.CanEverTick)
                    controller.StateUpdate(this);
            }
        }
    }
}