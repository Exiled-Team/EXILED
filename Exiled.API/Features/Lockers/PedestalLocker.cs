// -----------------------------------------------------------------------
// <copyright file="PedestalLocker.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Lockers
{
    using Exiled.API.Interfaces;
    using MapGeneration.Distributors;

    public class PedestalLocker : Locker, IWrapper<PedestalScpLocker>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PedestalLocker"/> class.
        /// </summary>
        /// <param name="locker"><see cref="PedestalScpLocker"/> instance.</param>
        public PedestalLocker(PedestalScpLocker locker)
            : base(locker)
        {
            Base = locker;
        }

        /// <inheritdoc/>
        public new PedestalScpLocker Base { get; }
    }
}