// -----------------------------------------------------------------------
// <copyright file="IAdditivePipe.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Interfaces
{
    /// <summary>
    /// Defines an additive user-defined pipe.
    /// </summary>
    public interface IAdditivePipe : IAdditiveIdentifier
    {
        /// <summary>
        /// Additive property should be adjusted here.
        /// </summary>
        public abstract void AdjustAddittivePipe();
    }
}