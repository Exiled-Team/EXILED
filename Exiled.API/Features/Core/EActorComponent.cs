// -----------------------------------------------------------------------
// <copyright file="EActorComponent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core
{
    /// <summary>
    /// <see cref="EActorComponent"/> is the base class for <see cref="EActor"/> components.
    /// </summary>
    public abstract class EActorComponent : EActor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EActorComponent"/> class.
        /// </summary>
        public EActorComponent()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EActorComponent"/> class.
        /// </summary>
        /// <param name="root"><inheritdoc cref="RootComponent"/></param>
        public EActorComponent(EActor root)
            : this() => RootComponent = root;

        /// <summary>
        /// Gets or sets the root <see cref="EActorComponent"/>.
        /// </summary>
        public abstract EActor RootComponent { get; protected set; }
    }
}
