// -----------------------------------------------------------------------
// <copyright file="IPermissionCommand.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces
{
    using CommandSystem;

    /// <summary>
    /// Represents an interface for commands with permissions.
    /// </summary>
    public interface IPermissionCommand : ICommand
    {
        /// <summary>
        /// Gets the permissions of a command.
        /// </summary>
        public string Permission { get; }
    }
}