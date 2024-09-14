// -----------------------------------------------------------------------
// <copyright file="UEBranchType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using Exiled.API.Features.Core.Generic;

    /// <summary>
    /// All available branch environments.
    /// </summary>
    public class UEBranchType : UnmanagedEnumClass<sbyte, UEBranchType>
    {
        /// <summary>
        /// The production branch.
        /// </summary>
        public static readonly UEBranchType Release = new(0);

        /// <summary>
        /// The debug branch.
        /// </summary>
        public static readonly UEBranchType Debug = new(1);

        /// <summary>
        /// The development branch.
        /// </summary>
        public static readonly UEBranchType Dev = new(2);

        /// <summary>
        /// The beta branch.
        /// </summary>
        public static readonly UEBranchType Beta = new(3);

        /// <summary>
        /// The alpha branch.
        /// </summary>
        public static readonly UEBranchType Alpha = new(4);

        /// <summary>
        /// The pre alpha branch.
        /// </summary>
        public static readonly UEBranchType PreAlpha = new(5);

        /// <summary>
        /// The unstable branch.
        /// </summary>
        public static readonly UEBranchType Unstable = new(6);

        /// <summary>
        /// Initializes a new instance of the <see cref="UEBranchType"/> class.
        /// </summary>
        /// <param name="value">The <see cref="sbyte"/> value.</param>
        protected UEBranchType(sbyte value)
            : base(value)
        {
        }
    }
}