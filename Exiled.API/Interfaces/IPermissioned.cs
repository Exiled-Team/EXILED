// -----------------------------------------------------------------------
// <copyright file="IPermissioned.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces
{
    /// <summary>
    /// Represents an interface for commands with permissions.
    /// </summary>
    public interface IPermissioned
    {
        /// <summary>
        /// Gets the permissions of a command.
        /// </summary>
        public string Permission { get; }
    }
}