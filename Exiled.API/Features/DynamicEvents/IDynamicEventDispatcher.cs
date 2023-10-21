// -----------------------------------------------------------------------
// <copyright file="IDynamicEventDispatcher.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.DynamicEvents
{
    /// <summary>
    /// Defines a dynamic event dispatcher.
    /// </summary>
    public interface IDynamicEventDispatcher
    {
        /// <summary>
        /// Unbinds all the delegates from all the bound delegates.
        /// </summary>
        void UnbindAll();
    }
}