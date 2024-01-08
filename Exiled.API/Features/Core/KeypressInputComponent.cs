// -----------------------------------------------------------------------
// <copyright file="KeypressInputComponent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core.Events;
    using Exiled.API.Features.DynamicEvents;

    /// <summary>
    /// Manages keypress input actions for a player.
    /// </summary>
    public class KeypressInputComponent : EPlayerBehaviour
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

            ev.Input.Action.Delegate();
            PressCount = 0;
        }

        /// <summary>
        /// Binds an action.
        /// </summary>
        /// <param name="name">The name of the action.</param>
        /// <param name="condition">The condition to invoke the action.</param>
        /// <param name="action">The bound action.</param>
        /// <param name="ktt">The bound <see cref="UUKeypressTriggerType"/>.</param>
        protected virtual void BindAction(string name, Func<bool> condition, DynamicDelegate action, UUKeypressTriggerType ktt)
        {
            InputBinding input = declaredInputs.FirstOrDefault(action => action.Name == name || action.Condition == condition || action.Keypress == ktt);

            if (input)
                throw new IndexOutOfRangeException($"Another KeypressAction has been declared with the same action: ({input.Name})");

            declaredInputs.Add(InputBinding.Create(name, condition, action, ktt));
        }

        /// <summary>
        /// Unbinds an action based on the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate used to find the input binding to unbind.</param>
        protected virtual void UnbindAction(Func<InputBinding, bool> predicate)
        {
            InputBinding input = declaredInputs.FirstOrDefault(predicate);

            if (!input)
                return;

            declaredInputs.Remove(input);
        }

        /// <summary>
        /// Unbinds an action based on the specified condition.
        /// </summary>
        /// <param name="condition">The condition used to find the input binding to unbind.</param>
        protected virtual void UnbindAction(Func<bool> condition) => UnbindAction(di => di.Condition == condition);

        /// <summary>
        /// Unbinds an action based on the specified name.
        /// </summary>
        /// <param name="name">The name used to find the input binding to unbind.</param>
        protected virtual void UnbindAction(string name) => UnbindAction(di => di.Name == name);

        /// <summary>
        /// Unbinds an action based on the specified <see cref="DynamicDelegate"/>.
        /// </summary>
        /// <param name="action">The <see cref="DynamicDelegate"/> used to find the input binding to unbind.</param>
        protected virtual void UnbindAction(DynamicDelegate action) => UnbindAction(di => di.Action == action);

        /// <summary>
        /// Unbinds an action based on the specified <see cref="UUKeypressTriggerType"/>.
        /// </summary>
        /// <param name="ktt">The <see cref="UUKeypressTriggerType"/> used to find the input binding to unbind.</param>
        protected virtual void UnbindAction(UUKeypressTriggerType ktt) => UnbindAction(di => di.Keypress == ktt);
    }
}