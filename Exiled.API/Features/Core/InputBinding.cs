// -----------------------------------------------------------------------
// <copyright file="InputBinding.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features.DynamicEvents;

    /// <summary>
    /// Represents an input binding.
    /// </summary>
    public sealed class InputBinding : NullableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputBinding"/> class.
        /// </summary>
        /// <param name="name"><inheritdoc cref="Name"/></param>
        /// <param name="condition"><inheritdoc cref="Condition"/></param>
        /// <param name="action"><inheritdoc cref="Action"/></param>
        /// <param name="ktt"><inheritdoc cref="Keypress"/></param>
        public InputBinding(string name, Func<bool> condition, DynamicDelegate action, UUKeypressTriggerType ktt)
        {
            Name = name;
            Condition = condition;
            Action = action;
            Keypress = ktt;
        }

        /// <summary>
        /// Gets the name of the binding.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the condition to invoke the action.
        /// </summary>
        public Func<bool> Condition { get; private set; }

        /// <summary>
        /// Gets the action.
        /// </summary>
        public DynamicDelegate Action { get; private set; }

        /// <summary>
        /// Gets the keypress.
        /// </summary>
        public UUKeypressTriggerType Keypress { get; private set; }

        /// <summary>
        /// Creates a new <see cref="InputBinding"/>.
        /// </summary>
        /// <param name="name"><inheritdoc cref="Name"/></param>
        /// <param name="condition"><inheritdoc cref="Condition"/></param>
        /// <param name="action"><inheritdoc cref="Action"/></param>
        /// <param name="ktt"><inheritdoc cref="Keypress"/></param>
        /// <returns>The new <see cref="InputBinding"/>.</returns>
        public static InputBinding Create(string name, Func<bool> condition, DynamicDelegate action, UUKeypressTriggerType ktt) => new(name, condition, action, ktt);
    }
}