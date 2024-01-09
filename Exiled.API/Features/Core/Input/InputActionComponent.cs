// -----------------------------------------------------------------------
// <copyright file="InputActionComponent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core.Events;
    using Exiled.API.Features.DynamicEvents;
    using UnityEngine.Windows;

    /// <summary>
    /// Manages actions for a player.
    /// </summary>
    public class InputActionComponent : EPlayerBehaviour
    {
        private readonly List<InputBinding> declaredInputs = new();

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all delegates to be fired before processing an action.
        /// </summary>
        [DynamicEventDispatcher]
        public static TDynamicEventDispatcher<ProcessingActionEventArgs> ProcessingActionDispatcher { get; protected set; }

        /// <summary>
        /// Gets or sets the amount of key presses.
        /// </summary>
        public int PressCount { get; set; }

        /// <summary>
        /// Gets an active binding, or <see langword="default"/> if not found.
        /// </summary>
        public virtual InputBinding ActiveBinding => declaredInputs.FirstOrDefault(di => di.Condition());

        /// <summary>
        /// Binds an <see cref="InputBinding"/>.
        /// </summary>
        /// <param name="inputBinding">The <see cref="InputBinding"/> to be bound.</param>
        public void BindInputAction(InputBinding inputBinding)
        {
            InputBinding input = declaredInputs.FirstOrDefault(ib => ib == inputBinding);

            if (input)
                throw new IndexOutOfRangeException($"Another KeypressAction has been declared with the same action: ({input.Name})");

            declaredInputs.Add(inputBinding);
        }

        /// <summary>
        /// Binds an <see cref="InputBinding"/>.
        /// </summary>
        /// <param name="name">The name of the action.</param>
        /// <param name="condition">The condition to invoke the action.</param>
        /// <param name="action">The bound action.</param>
        /// <param name="ktt">The bound <see cref="UUKeypressTriggerType"/>.</param>
        public void BindInputAction(string name, Func<bool> condition, Action action, UUKeypressTriggerType ktt)
        {
            InputBinding input = declaredInputs.FirstOrDefault(action => action.Name == name || action.Condition == condition || action.Keypress == ktt);

            if (input)
                throw new IndexOutOfRangeException($"Another KeypressAction has been declared with the same action: ({input.Name})");

            declaredInputs.Add(InputBinding.Create(name, condition, action, ktt));
        }

        /// <summary>
        /// Unbinds an <see cref="InputBinding"/> based on the specified predicate.
        /// </summary>
        /// <param name="inputBinding">The <see cref="InputBinding"/> to unbind.</param>
        public void UnbindInputAction(InputBinding inputBinding) => declaredInputs.Remove(inputBinding);

        /// <summary>
        /// Unbinds an <see cref="InputBinding"/> based on the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate used to find the input binding to unbind.</param>
        public void UnbindInputAction(Func<InputBinding, bool> predicate)
        {
            InputBinding input = declaredInputs.FirstOrDefault(predicate);

            if (!input)
                return;

            declaredInputs.Remove(input);
        }

        /// <summary>
        /// Unbinds an <see cref="InputBinding"/> based on the specified condition.
        /// </summary>
        /// <param name="condition">The condition used to find the input binding to unbind.</param>
        public void UnbindInputAction(Func<bool> condition) => UnbindInputAction(di => di.Condition == condition);

        /// <summary>
        /// Unbinds an <see cref="InputBinding"/> based on the specified name.
        /// </summary>
        /// <param name="name">The name used to find the input binding to unbind.</param>
        public void UnbindInputAction(string name) => UnbindInputAction(di => di.Name == name);

        /// <summary>
        /// Unbinds an <see cref="InputBinding"/> based on the specified <see cref="DynamicDelegate"/>.
        /// </summary>
        /// <param name="action">The <see cref="DynamicDelegate"/> used to find the input binding to unbind.</param>
        public void UnbindInputAction(Action action) => UnbindInputAction(di => di.Action == action);

        /// <summary>
        /// Unbinds an <see cref="InputBinding"/> based on the specified <see cref="UUKeypressTriggerType"/>.
        /// </summary>
        /// <param name="ktt">The <see cref="UUKeypressTriggerType"/> used to find the input binding to unbind.</param>
        public void UnbindInputAction(UUKeypressTriggerType ktt) => UnbindInputAction(di => di.Keypress == ktt);

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            FixedTickRate = 0.25f;
        }

        /// <inheritdoc/>
        protected override void Tick()
        {
            base.Tick();

            if (!ActiveBinding)
                return;

            ProcessingActionEventArgs ev = new(Owner, ActiveBinding, true);
            ProcessingActionDispatcher.InvokeAll(ev);

            if (!ev.IsAllowed)
                return;

            ev.Input.Action();
            PressCount = 0;
        }
    }
}