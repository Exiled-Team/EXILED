// -----------------------------------------------------------------------
// <copyright file="CustomEscape.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomEscapes
{
    using System;

    /// <summary>
    /// A class to easily manage escaping behavior.
    /// </summary>
    /// <typeparam name="T">The <see cref="EscapeBehaviour"/> type.</typeparam>
    public abstract class CustomEscape<T> : CustomEscape
        where T : EscapeBehaviour
    {
        /// <inheritdoc/>
        public override Type BehaviourComponent => typeof(T);
    }
}