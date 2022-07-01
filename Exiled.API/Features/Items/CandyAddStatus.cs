// -----------------------------------------------------------------------
// <copyright file="CandyAddStatus.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    /// <summary>
    /// Provides status codes for Scp330.
    /// </summary>
    public partial class Scp330
    {
        /// <summary>
        /// Candy enumeration status.
        /// </summary>
        public enum CandyAddStatus
        {
            /// <summary>
            /// If no candy was able to be added.
            /// </summary>
            NoCandyAdded,

            /// <summary>
            /// If at least one candy was added.
            /// </summary>
            SomeCandyAdded,

            /// <summary>
            /// If all candies provided were added.
            /// </summary>
            AllCandyAdded,
        }
    }
}
